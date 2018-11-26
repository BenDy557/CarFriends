using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class NetworkManager : Singleton<NetworkManager>
{
    private IPAddress m_localIPAddr;

    [SerializeField, NaughtyAttributes.Dropdown("m_savedIPAddr")]
    private string m_targetIPAddr = "127.0.0.1";

    private DropdownList<string> m_savedIPAddr = new DropdownList<string>()
    {
        {"LocalHost", "127.0.0.1"},
        {"WorkPC-wifi", "192.168.3.161" },
        {"WorkMac-wifi", "192.168.3.160" },
    };

    private int port = 2349;
    private System.Net.IPEndPoint m_remoteEndPoint;

    private UdpClient m_udpClient;
    ////////////////////////
    private enum NetworkMessageType
    {
        LOCOMOTION,//velocity and position
        INPUT,//input state
    }

    

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

    private void SendMessage(byte[] dataToSend)
    {
        m_udpClient.Send(dataToSend, dataToSend.Length, m_remoteEndPoint);
        Debug.Log("Message Sent");
    }

    public void SendPositionMessage()
    {

    }

    private void ReceiveMessage()
    {
        try
        {
            IPEndPoint senderEndPoint = null;
            byte[] dataReceived = m_udpClient.Receive(ref senderEndPoint);
            string text = Encoding.ASCII.GetString(dataReceived);
            Debug.Log(text);

            byte[] dataSend = Encoding.ASCII.GetBytes("Shit");
            m_udpClient.Send(dataSend, dataSend.Length, senderEndPoint);
        }
        catch
        {
            //nothing
        }

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
