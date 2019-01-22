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
    private string message = "butt";

    [SerializeField]
    private string m_remoteIpAddress;
    [SerializeField]
    private int m_remotePort;
    
    UdpClient clientSending;
    UdpClient clientReceiving;

    private void OnEnable()
    {
        clientSending = new UdpClient();
        clientReceiving = new UdpClient();
    }

    private void OnDisable()
    {
        clientSending.Close();
        clientSending = null;

        clientReceiving.Close();
        clientReceiving = null;
    }

    //Just make this button send a message to a certain ip address and port
    [NaughtyAttributes.Button]
    private void SendMessage()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("can only send message in play mode");
            return;
        }

        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(m_remoteIpAddress), m_remotePort);
        byte[] data = new byte[16];
        int dataSize = 0;
        clientSending.Send(data, dataSize, remoteEndPoint);
    }


    //Make this button create a new socket with the given local address and port number
    //there should be an updtae function that receives the data, this button should just update the address and port
    [NaughtyAttributes.Button]
    private void RefreshServerSocket()
    {

    }

}