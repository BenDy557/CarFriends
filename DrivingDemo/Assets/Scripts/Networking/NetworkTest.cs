using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

class NetworkTest : MonoBehaviour
{
    private IPAddress m_localIPAddr;

    [SerializeField]
    private string m_targetIPAddr = "127.0.0.1";
    //192.168.0.7 windows laptop
    //192.168.0.9 mac laptop
    //172.16.20.138 work desktop

    private int port = 2349;

    private System.Net.IPEndPoint m_remoteEndPoint;

    string host;

    ////////////////////////
    private UdpClient m_udpClient;

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

    [NaughtyAttributes.Button]
    private void SendMessage()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("can only send message in play mode");
            return;
        }

        /*for (int i = 0; i < 100; i++)
        {
            //Start sending stuf..
            byte[] byData = System.Text.Encoding.ASCII.GetBytes("Message from: " + GetLocalIPAddress());
            m_sendSocket.SendTo(byData, m_remoteEndPoint);
        }*/

        byte[] dataSend = Encoding.ASCII.GetBytes("Shit");
        m_udpClient.Send(dataSend, dataSend.Length, m_remoteEndPoint);


        Debug.Log("Message Sent");
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