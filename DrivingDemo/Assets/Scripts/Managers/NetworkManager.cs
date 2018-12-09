using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using UnibusEvent;

public class NetworkManager : Singleton<NetworkManager>
{
    private NetworkRole m_networkRole = NetworkRole.NONE;
    public NetworkRole NetworkRole { get { return m_networkRole; } }
    private bool NetworkRoleStarted { get { return m_networkRole != NetworkRole.NONE; } }

    private List<IPAddress> m_localIPAddrs = new List<IPAddress>();

    [SerializeField, NaughtyAttributes.Dropdown("m_savedIPAddr")]
    private string m_targetIPAddr = "127.0.0.1";

    private DropdownList<string> m_savedIPAddr = new DropdownList<string>()
    {
        {"LocalHost", "127.0.0.1"},
        {"WorkPC-wifi", "192.168.3.161" },
        {"WorkMac-wifi", "192.168.3.164" },
        {"HomeMac-wifi", "192.168.0.9" },
        {"HomePC-wifi", "192.168.0.7" },
    };

    private int startPort = 4925;
    private Dictionary<int, UdpClient> m_sockets = new Dictionary<int, UdpClient>();
           

    //Server broadcast socket
    private float m_broadcastInterval = 0.5f;
    private float m_broadcastMsgTimer = 0f;
    private int m_broadcastPort = 5555;
    private IPEndPoint m_broadcastEndPoint;
    private UdpClient m_broadcastSocket;

    private IPEndPoint m_receiveBroadcastEndPoint;
    private UdpClient m_receiveBroadcastSocket;

    private IEnumerable<UdpClient> AllSockets
    {
        get
        {
            for (int i = 0; i < m_sockets.Count; i++)
            {
                yield return m_sockets[i];
            }

            if (m_broadcastSocket != null)
                yield return m_broadcastSocket;

            if (m_receiveBroadcastSocket != null)
                yield return m_receiveBroadcastSocket;
        }
    }

    public void OnEnable()
    {
        //Assign local IPs
        IPHostEntry tempHostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress tempIP in tempHostEntry.AddressList)
        {
            if (tempIP.AddressFamily == AddressFamily.InterNetwork)
            {
                m_localIPAddrs.Add(tempIP);
            }

            Debug.Log(tempIP.AddressFamily.ToString() + " " + tempIP.ToString());
        }

        /*System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(m_targetIPAddr);
        m_remoteEndPoint = new IPEndPoint(ipAdd, port);
        m_udpClient = new UdpClient(port);
        m_udpClient.Client.Blocking = false;
        m_udpClient.Client.MulticastLoopback = true;*/
        //m_udpClient.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.UseLoopback, true);
    }

    private void OnDisable()
    {
        Debug.Log("Closed");
        foreach (UdpClient socket in AllSockets)
        {
            socket.Client.Close();
        }
    }

    private void Update()
    {
        switch (m_networkRole)
        {
            case NetworkRole.NONE:
                break;
            case NetworkRole.SERVER:
            BroadcastUpdate();
                break;
            case NetworkRole.CLIENT:
                break;
        }

        ReceiveMessage();
    }

    [Button]
    private void StartServer()
    {
        if (NetworkRoleStarted)
            return;

        m_networkRole = NetworkRole.SERVER;

        //m_broadcastEndPoint = new IPEndPoint(IPAddress.Any, m_broadcastPort);
        //m_broadcastEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), m_broadcastPort);

        m_broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, m_broadcastPort);

        m_broadcastSocket = new UdpClient();
        m_broadcastSocket.EnableBroadcast = true;
        m_broadcastSocket.Client.Blocking = false;
        m_broadcastSocket.Client.MulticastLoopback = true;
        m_broadcastSocket.Connect(m_broadcastEndPoint);

        Unibus.Dispatch(EventTags.OnServerStart);
    }

    [Button]
    private void StartClient()
    {
        if (NetworkRoleStarted)
            return;

        m_networkRole = NetworkRole.CLIENT;

        m_receiveBroadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, m_broadcastPort);
        m_receiveBroadcastSocket = new UdpClient();
        m_receiveBroadcastSocket.EnableBroadcast = true;
        m_receiveBroadcastSocket.Client.Blocking = false;
        m_receiveBroadcastSocket.Client.MulticastLoopback = true;
        m_receiveBroadcastSocket.Connect(m_receiveBroadcastEndPoint);

        Unibus.Dispatch(EventTags.OnClientStart);
    }

    private void BroadcastUpdate()
    {
        if (m_broadcastMsgTimer < 0)
        {
            m_broadcastMsgTimer = m_broadcastInterval;
            foreach (IPAddress ip in m_localIPAddrs)
            {
                SendData(m_broadcastSocket, new NetworkData(NetworkData.NetworkMessageType.SERVER_BROADCAST, ip.ToString()));
            }
        }
        m_broadcastMsgTimer -= Time.unscaledDeltaTime;
    }

    private void SendRawData(UdpClient socket, byte[] dataToSend)
    {
        socket.Send(dataToSend, dataToSend.Length);
        Debug.Log("Message Sent");
    }

    private void ReceiveMessage()
    {
        byte[] rawDataReceived;

        foreach (UdpClient socket in AllSockets)
        {
            while (true)
            {
                try
                {
                    IPEndPoint senderEndPoint = null;
                    rawDataReceived = socket.Receive(ref senderEndPoint);
                }
                catch
                {
                    //nothing
                    return;
                }

                NetworkData networkDataReceived = ConvertToNetworkData(rawDataReceived);
                DispatchNetworkEvents(networkDataReceived);
            }
        }
    }

    /// <summary>
    /// Filters the dispatch of network events based on type of data received
    /// </summary>
    /// <param name="networkData">Network data.</param>
    private void DispatchNetworkEvents(NetworkData networkData)
    {
        Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived, networkData);

        switch (networkData.MessageType)
        {
            case NetworkData.NetworkMessageType.MESSAGE:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Message, networkData);
                Debug.Log(networkData.Message);
                break;
            case NetworkData.NetworkMessageType.JOIN:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Join, networkData);
                break;
            case NetworkData.NetworkMessageType.LOCOMOTION:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Locomotion, networkData);
                Debug.Log(networkData.LocomotionData.Position);
                break;
            case NetworkData.NetworkMessageType.INPUT:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Input, networkData);
                break;
            case NetworkData.NetworkMessageType.SERVER_BROADCAST:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Server_Brodacast, networkData);
                Debug.Log("BroadcastReceived");
                break;
        }
    }

    private NetworkData ConvertToNetworkData(byte[] rawData)
    {
        int size = Marshal.SizeOf(typeof(NetworkData));
        IntPtr pointer = Marshal.AllocHGlobal(size);
        Marshal.Copy(rawData, 0, pointer, size);
        return (NetworkData)Marshal.PtrToStructure(pointer, typeof(NetworkData));
    }

    private byte[] ConvertToRawData(NetworkData networkData)
    {
        int size = Marshal.SizeOf(networkData);
        byte[] rawData = new byte[size];

        IntPtr pointer = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(networkData, pointer, true);
        Marshal.Copy(pointer, rawData, 0, size);
        Marshal.FreeHGlobal(pointer);
        return rawData;
    }

    public void SendData(UdpClient socket, NetworkData data)
    {
        byte[] buffer = ConvertToRawData(data);
        SendRawData(socket, buffer);
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return null;
    }
}

public enum NetworkRole
{
    NONE,
    SERVER,
    CLIENT,
}