using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

class NetworkTest : MonoBehaviour
{
    [SerializeField]
    private string m_targetIPAddr = "127.0.0.1";
    //192.168.0.7 windows laptop

    private Socket m_sendSocket;
    private Socket m_recieveSocket;
    private int port = 7001;

    string host;

    public void OnEnable()
    {
        //sending socket
        m_sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(m_targetIPAddr);
        System.Net.IPEndPoint remoteEndPoint = new IPEndPoint(ipAdd, port);
        m_sendSocket.Connect(remoteEndPoint);

        //RecievingSocket
        m_recieveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        m_recieveSocket.Blocking = false;
        m_recieveSocket.Bind(remoteEndPoint);
    }

    private void OnDisable()
    {
        Debug.Log("Closed");
        if (m_sendSocket != null)
            m_sendSocket.Close();
        if (m_recieveSocket != null)
            m_recieveSocket.Close();
    }

    private void Update()
    {
        //And for reading back..
        byte[] buffer = new byte[1024];
        int iRx = -1;
        char[] chars = new char[0];
        try
        {
            iRx = m_recieveSocket.Receive(buffer);
            chars = new char[iRx];

            System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
            int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
            System.String recv = new System.String(chars);

            Debug.Log(recv);
        }
        catch (SocketException e)
        {
            switch (e.SocketErrorCode)
            {
                case SocketError.WouldBlock:
                    //WouldBlock
                    break;
                default:
                    Debug.LogError("Exception not caught. Error code: " + e.SocketErrorCode + " " + (int)e.SocketErrorCode);
                    break;
            }
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

        //Start sending stuf..
        byte[] byData = System.Text.Encoding.ASCII.GetBytes("un:" + "Shit");
        m_sendSocket.Send(byData);

        Debug.Log("Message Sent");
    }
}