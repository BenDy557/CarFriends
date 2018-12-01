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
    private IPAddress m_localIPAddr;

    [SerializeField, NaughtyAttributes.Dropdown("m_savedIPAddr")]
    private string m_targetIPAddr = "127.0.0.1";

    private DropdownList<string> m_savedIPAddr = new DropdownList<string>()
    {
        {"LocalHost", "127.0.0.1"},
        {"WorkPC-wifi", "192.168.3.161" },
        {"WorkMac-wifi", "192.168.3.156" },
        {"HomeMac-wifi", "192.168.0.9" },
        {"HomePC-wifi", "192.168.0.7" },
    };

    private int port = 2349;
    private System.Net.IPEndPoint m_remoteEndPoint;

    private UdpClient m_udpClient;
    ////////////////////////

    public void OnEnable()
    {
        IPHostEntry tempHostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress tempIP in tempHostEntry.AddressList)
        {
            if (tempIP.AddressFamily == AddressFamily.InterNetwork)
            {
                m_localIPAddr = tempIP;
            }

            Debug.Log(tempIP.AddressFamily.ToString() + " " + tempIP.ToString());
        }

        System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(m_targetIPAddr);
        m_remoteEndPoint = new IPEndPoint(ipAdd, port);
        m_udpClient = new UdpClient(port);
        m_udpClient.Client.Blocking = false;
        m_udpClient.Client.MulticastLoopback = true;
        //m_udpClient.Client.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.UseLoopback, true);
    }

    private void OnDisable()
    {
        Debug.Log("Closed");
        m_udpClient.Close();
    }

    private void Update()
    {
        ReceiveMessage();
    }

    private void SendRawData(byte[] dataToSend)
    {
        m_udpClient.Send(dataToSend, dataToSend.Length, m_remoteEndPoint);
        Debug.Log("Message Sent");
    }

    private void ReceiveMessage()
    {
        byte[] rawDataReceived;

        try
        {
            IPEndPoint senderEndPoint = null;
            rawDataReceived = m_udpClient.Receive(ref senderEndPoint);
        }
        catch
        {
            //nothing
            return;
        }

        NetworkData networkDataReceived = ConvertToNetworkData(rawDataReceived);
        DispatchNetworkEvents(networkDataReceived);
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
                Debug.Log(networkData.Position);
                break;
            case NetworkData.NetworkMessageType.INPUT:
                Unibus.Dispatch<NetworkData>(EventTags.NetDataReceived_Input, networkData);
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

    public void SendData(NetworkData data)
    {
        byte[] buffer = ConvertToRawData(data);
        SendRawData(buffer);
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
