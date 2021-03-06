﻿using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.InteropServices;
using UnibusEvent;

using NetworkBon;

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
    public IPAddress LocalIPAddrsIPV4 { get { return m_localIPAddrsIPV4; } }

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

    private List<byte[]> m_dataReceived = new List<byte[]>();


    //Server sockets
    private int m_gameDataPort = 4915;
    //private UdpClient m_gameplaySocket;
    //private IPEndPoint m_serverLocalEndPoint;
    //private IPEndPoint m_serverRemoteEndPoint;
    private UdpClient m_serverSocket = null;
    public bool IsServerSet { get { return m_serverSocket != null; } }

    //Server broadcast socket
    private float m_broadcastInterval = 0.5f;
    private float m_broadcastMsgTimer = 0f;
    private int m_broadcastPort = 4910;
    //Socket that broadcasts out this computer's IP address for clients to join
    //private IPEndPoint m_broadcastEndPoint;
    private UdpClient m_serverBroadcastSocket;

    //Socket that receives server broadcasts then stores that IP address
    //private IPEndPoint m_clientBroadcastLocalEndPoint;
    private UdpClient m_clientBroadcastSocket;

    //ClientSockets
    //private int m_firstPort = 4916;
    //private int m_lastPort = 4920;
    //private Dictionary<int, UdpClient> m_sockets = new Dictionary<int, UdpClient>();

    private IEnumerable<UdpClient> AllSockets
    {
        get
        {
            /*foreach (UdpClient client in m_sockets.Values)
            {
                yield return client;
            }*/

            /*if (m_gameplaySocket != null)
                yield return m_gameplaySocket;*/

            if (m_serverBroadcastSocket != null)
                yield return m_serverBroadcastSocket;

            if (m_clientBroadcastSocket != null)
                yield return m_clientBroadcastSocket;

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

        ReadFromReceiveBuffer();
    }

    [Button]
    public void StartServer()
    {
        if (NetworkRoleStarted)
            return;

        m_networkRole = NetworkRole.SERVER;

        SetServerBroadcastSocket();
        SetLocalServer();

        Unibus.Dispatch(EventTags.OnServerStart);
    }

    [Button]
    public void StartClient()
    {
        if (NetworkRoleStarted)
            return;

        m_networkRole = NetworkRole.CLIENT;
        SetClientBroadcastSocket();
        

        Unibus.Dispatch(EventTags.OnClientStart);
    }

    private void BroadcastUpdate()
    {
        if (m_broadcastMsgTimer < 0)
        {
            m_broadcastMsgTimer = m_broadcastInterval;
            foreach (IPAddress ip in m_localIPAddrs)
            {
                SendData(m_serverBroadcastSocket, new NetworkData(NetworkDataType.SERVER_BROADCAST, ip.ToString()));
            }
        }
        m_broadcastMsgTimer -= Time.unscaledDeltaTime;
    }

    private void SendRawData(UdpClient socket, byte[] dataToSend)
    {
        socket.Send(dataToSend, dataToSend.Length);
    }

    private void BeginAsyncReceiveMessage()
    {
        byte[] rawDataReceived;

        foreach (UdpClient socket in AllSockets)
        {
            //UdpState udpState = new UdpState();
            //udpState.socket = socket;
            //udpState.endpoint = (IPEndPoint)socket.Client.LocalEndPoint;

            //socket.BeginReceive(AsyncReceiveCallback, udpState);


            /*while (true)
            {
                try
                {
                    //IPEndPoint senderEndPoint = null;

                    //socket.BeginReceive(new AsyncCallback(AsyncReceiveCallback), udpState);
                    //rawDataReceived = socket.Receive(ref senderEndPoint);
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message);
                    //nothing//wsa wouldblock should return
                    //TODO//should catch other exceptions
                    return;
                }

                //NetworkData networkDataReceived = ConvertToNetworkData(rawDataReceived);
                //DispatchNetworkEvents(networkDataReceived);
            }*/
        }
    }

    private void ReadFromReceiveBuffer()
    {
        if (m_dataReceived.Count == 0)
        {
            //Debug.Log("NoData");
            return;
        }
        else
            Debug.Log("DataHere!" + m_dataReceived.Count);

        NetworkData networkDataReceived;
        for (int i = m_dataReceived.Count-1; i>=0; i--)
        {
            networkDataReceived = ConvertToNetworkData(m_dataReceived[i]);
            DispatchNetworkEvents(networkDataReceived);
            m_dataReceived.RemoveAt(i);
        }

        Debug.Log("DataLeft: " + m_dataReceived.Count);
    }

    public struct UdpState
    {
        public UdpClient socket;
        public IPEndPoint endpoint;
    }


    private void AsyncReceiveCallback(IAsyncResult ar)
    {
        UdpClient socket = ((UdpState)(ar.AsyncState)).socket;
        IPEndPoint endPoint = ((UdpState)(ar.AsyncState)).endpoint;

        int dataSize = Marshal.SizeOf(typeof(NetworkData));

        byte[] receiveBytes = socket.EndReceive(ar, ref endPoint);
        m_dataReceived.Add(receiveBytes);

        socket.BeginReceive(AsyncReceiveCallback, (UdpState)(ar.AsyncState));

        Debug.Log("Data Reveived");
    }

    /*private void ReceiveCallback(byte[] data)
    {
        m_dataReceived.Add(data);
    }*/

    /// <summary>
    /// Filters the dispatch of network events based on type of data received
    /// </summary>
    /// <param name="networkData">Network data.</param>
    private void DispatchNetworkEvents(NetworkData networkData)
    {
        Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived, networkData);

        switch (networkData.DataType)
        {
            case NetworkDataType.NONE:
                Debug.Log("CorruptNeworkMessage");
                break;
            /*case NetworkData.NetworkDataType.MESSAGE:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Message, networkData);
                Debug.Log(networkData.Message);
                break;
            case NetworkData.NetworkDataType.JOIN:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Join, networkData);
                break;*/
            case NetworkDataType.LOCOMOTION:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Locomotion, networkData);
                Debug.Log(networkData.DataType + " received " + networkData.LocomotionData.Position);
                break;
            case NetworkDataType.INPUT:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Input, networkData);
                break;
            case NetworkDataType.NETWORK_MESSAGE:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Network_Message, networkData);
                Debug.Log(networkData.DataType + " " + networkData.MessageType +" received");
                break;
            case NetworkDataType.SERVER_BROADCAST:
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
        Debug.Log("Message Sent: " + data.DataType + " Message type: " + data.MessageType + " SentTo: " + socket.Client.RemoteEndPoint.ToString());
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

    public void SendDataToClient(IPEndPoint destination, NetworkData networkData)
    {
        m_serverSocket.Connect(destination);
        SendData(m_serverSocket, networkData);
    }

    //Server
    private void SetServerBroadcastSocket()
    {
        IPEndPoint broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, m_broadcastPort);
        m_serverBroadcastSocket = new UdpClient(m_broadcastPort);
        m_serverBroadcastSocket.EnableBroadcast = true;
        m_serverBroadcastSocket.Client.Blocking = false;
        m_serverBroadcastSocket.Client.MulticastLoopback = true;
        m_serverBroadcastSocket.Connect(broadcastEndPoint);

        //Start Async Receive
        StartAsyncReceive(m_serverBroadcastSocket);
    }

    private void SetLocalServer()
    {
        //m_serverRemoteEndPoint = new IPEndPoint(IPAddress.None, m_serverPort);
        //IPEndPoint serverLocalEndPoint = new IPEndPoint(IPAddress.Any, m_serverPort);
        m_serverSocket = new UdpClient(m_gameDataPort);
        m_serverSocket.EnableBroadcast = true;
        m_serverSocket.Client.Blocking = false;
        m_serverSocket.Client.MulticastLoopback = true;
        //m_serverSocket.Connect(m_serverRemoteEndPoint);
        StartAsyncReceive(m_serverSocket);
        //Start Async Receive
        
    }

    private void SetClientBroadcastSocket()
    {
        IPEndPoint clientBroadcastLocalEndPoint = new IPEndPoint(IPAddress.Any, m_broadcastPort);
        m_clientBroadcastSocket = new UdpClient(clientBroadcastLocalEndPoint);
        m_clientBroadcastSocket.EnableBroadcast = true;
        m_clientBroadcastSocket.Client.Blocking = false;
        m_clientBroadcastSocket.Client.MulticastLoopback = true;
        //m_clientBroadcastSocket.Connect(IPAddress.Any,m_broadcastPort);
        StartAsyncReceive(m_clientBroadcastSocket);
    }

    public void SetRemoteServerAddress(string ipAddress)
    {
        m_serverSocket = new UdpClient(m_gameDataPort);// m_serverEndPoint);
        m_serverSocket.EnableBroadcast = true;
        m_serverSocket.Client.Blocking = false;
        m_serverSocket.Client.MulticastLoopback = true;
        IPEndPoint serverRemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), m_gameDataPort);
        m_serverSocket.Connect(serverRemoteEndPoint);
        StartAsyncReceive(m_serverSocket);
    }

    private void StartAsyncReceive(UdpClient socket)
    {
        UdpState udpState = new UdpState();
        udpState.socket = socket;
        udpState.endpoint = (IPEndPoint)socket.Client.LocalEndPoint;
        socket.BeginReceive(AsyncReceiveCallback, udpState);
    }

    /*public UdpClient AddClient(string ipAddress)
    {
        //int portNumber = m_firstPort;
        //make socket attach to new port number
        Debug.Log("IP RECEIVED: " + ipAddress);
        //IPEndPoint clientRemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), m_firstPort);
        IPEndPoint clientRemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), m_serverPort);

        UdpClient clientSocket = new UdpClient(m_firstPort);
        clientSocket.Client.Blocking = false;
        clientSocket.Client.MulticastLoopback = true;
        clientSocket.Connect(clientRemoteEndPoint);

        StartAsyncReceive(clientSocket);

        m_sockets.Add(portNumber, clientSocket);

        return clientSocket;
    }*/

    public IPEndPoint GetClientEndPoint(string ipAddress)
    {
        return new IPEndPoint(IPAddress.Parse(ipAddress), m_gameDataPort);
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