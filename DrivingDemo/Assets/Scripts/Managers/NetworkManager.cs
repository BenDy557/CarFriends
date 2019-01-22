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

/// <summary>
/// Network manager.
/// The network manager is the first and last part of the application where network 
/// data passes through. It contains all the sockets within the project and is what
/// gets called by gameplay related scripts when a network object needs to send
/// its state to other network members. The network manager also notifies any relevant
/// scripts when a network message is received via events. 
/// </summary>
public class NetworkManager : Singleton<NetworkManager>
{
    private NetworkRole m_networkRole = NetworkRole.NONE;
    public NetworkRole NetworkRole { get { return m_networkRole; } }
    private bool NetworkRoleStarted { get { return m_networkRole != NetworkRole.NONE; } }

    private List<IPAddress> m_localIPAddrs = new List<IPAddress>();
    private IPAddress m_localIPAddrsIPV4;

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


    //Server sockets
    private int m_serverPort = 4915;
    private IPEndPoint m_serverLocalEndPoint;
    private IPEndPoint m_serverRemoteEndPoint;
    private UdpClient m_serverSocket = null;
    public bool IsServerSet { get { return m_serverSocket != null; } }

    //Server broadcast socket
    private float m_broadcastInterval = 0.5f;
    private float m_broadcastMsgTimer = 0f;
    private int m_broadcastPort = 4910;
    //Socket that broadcasts out this computer's IP address for clients to join
    private IPEndPoint m_broadcastEndPoint;
    private UdpClient m_broadcastSocket;

    //Socket that receives server broadcasts then stores that IP address
    private IPEndPoint m_receiveBroadcastEndPoint;
    private UdpClient m_receiveBroadcastSocket;

    //ClientSockets
    private int m_firstPort = 4916;
    private int m_lastPort = 4920;
    private Dictionary<int, UdpClient> m_sockets = new Dictionary<int, UdpClient>();

    private IEnumerable<UdpClient> AllSockets
    {
        get
        {
            foreach (UdpClient client in m_sockets.Values)
            {
                yield return client;
            }

            if (m_broadcastSocket != null)
                yield return m_broadcastSocket;

            if (m_receiveBroadcastSocket != null)
                yield return m_receiveBroadcastSocket;

            if (m_serverSocket != null)
                yield return m_serverSocket;
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

            if (tempIP.AddressFamily == AddressFamily.InterNetwork && tempIP.AddressFamily != AddressFamily.InterNetworkV6)
            {
                m_localIPAddrsIPV4 = tempIP;
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

        Debug.Log("IsServerSet: " + IsServerSet);

        int count = 0;
        foreach (UdpClient socket in AllSockets)
            count++;
        Debug.Log(count + " SOCKETS");

        count = 0;

        foreach (UdpClient socket in AllSockets)
        {
            Debug.Log("Socket: " + count);
            Debug.Log("LocalEP: " + socket.Client.LocalEndPoint);
            Debug.Log("RemoteEP: " + socket.Client.RemoteEndPoint);
            count++;
        }
        
        ReceiveMessage();
    }

    [Button]
    private void StartServer()
    {
        if (NetworkRoleStarted)
            return;

        m_networkRole = NetworkRole.SERVER;

        m_broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, m_broadcastPort);
        m_broadcastSocket = new UdpClient(m_broadcastPort);
        m_broadcastSocket.EnableBroadcast = true;
        m_broadcastSocket.Client.Blocking = false;
        m_broadcastSocket.Client.MulticastLoopback = true;
        m_broadcastSocket.Connect(m_broadcastEndPoint);

        SetLocalServer();

        Unibus.Dispatch(EventTags.OnServerStart);
    }

    [Button]
    private void StartClient()
    {
        if (NetworkRoleStarted)
            return;

        m_networkRole = NetworkRole.CLIENT;

        m_receiveBroadcastEndPoint = new IPEndPoint(IPAddress.Any, m_broadcastPort);
        m_receiveBroadcastSocket = new UdpClient(m_receiveBroadcastEndPoint);
        m_receiveBroadcastSocket.EnableBroadcast = true;
        m_receiveBroadcastSocket.Client.Blocking = false;
        m_receiveBroadcastSocket.Client.MulticastLoopback = true;

        Unibus.Dispatch(EventTags.OnClientStart);
    }

    private void BroadcastUpdate()
    {
        if (m_broadcastMsgTimer < 0)
        {
            m_broadcastMsgTimer = m_broadcastInterval;
            foreach (IPAddress ip in m_localIPAddrs)
            {
                SendData(m_broadcastSocket, new NetworkData(NetworkData.NetworkDataType.SERVER_BROADCAST, ip.ToString()));
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
                    //nothing//wsa wouldblock should return
                    //TODO//should catch other exceptions
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

        switch (networkData.DataType)
        {
            /*case NetworkData.NetworkDataType.MESSAGE:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Message, networkData);
                Debug.Log(networkData.Message);
                break;
            case NetworkData.NetworkDataType.JOIN:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Join, networkData);
                break;*/
            case NetworkData.NetworkDataType.LOCOMOTION:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Locomotion, networkData);
                Debug.Log(networkData.LocomotionData.Position);
                break;
            case NetworkData.NetworkDataType.INPUT:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Input, networkData);
                break;
            case NetworkData.NetworkDataType.NETWORK_MESSAGE:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Network_Message, networkData);
                break;
            case NetworkData.NetworkDataType.SERVER_BROADCAST:
                Debug.Log("BroadcastReceived");
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Server_Brodacast, networkData);
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

    public void SendDataToServer(NetworkData data)
    {
        if (!IsServerSet)
        {
            Debug.LogError("ServerIsNotSet");
            return;
        }

        SendData(m_serverSocket, data);
    }

    public void SendDataToClient(UdpClient client, NetworkData networkData)
    {
        SendData(client, networkData);
    }

    public void SetLocalServer()
    {
        /*m_broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, m_broadcastPort);
        m_broadcastSocket = new UdpClient();
        m_broadcastSocket.EnableBroadcast = true;
        m_broadcastSocket.Client.Blocking = false;
        m_broadcastSocket.Client.MulticastLoopback = true;
        m_broadcastSocket.Connect(m_broadcastEndPoint);*/



        m_serverLocalEndPoint = new IPEndPoint(IPAddress.Any, m_serverPort);
        m_serverRemoteEndPoint = new IPEndPoint(IPAddress.None, m_serverPort);
        m_serverSocket = new UdpClient(m_serverPort);
        m_serverSocket.EnableBroadcast = true;
        m_serverSocket.Client.Blocking = false;
        m_serverSocket.Client.MulticastLoopback = true;
        m_serverSocket.Connect(m_serverRemoteEndPoint);
    }

    public void SetRemoteServerAddress(string ipAddress)
    {
        m_serverLocalEndPoint = new IPEndPoint(m_localIPAddrsIPV4, m_serverPort);
        m_serverRemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), m_serverPort);
        m_serverSocket = new UdpClient(m_serverLocalEndPoint);// m_serverEndPoint);
        m_serverSocket.Connect(m_serverRemoteEndPoint);
        m_serverSocket.EnableBroadcast = true;
        m_serverSocket.Client.Blocking = false;
        m_serverSocket.Client.MulticastLoopback = true;
    }

    public UdpClient AddClient(string ipAddress)
    {
        int portNumber = m_firstPort;
        //make socket attach to new port number
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), m_firstPort);

        UdpClient clientSocket = new UdpClient(clientEndPoint);

        m_sockets.Add(portNumber, clientSocket);

        return clientSocket;
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