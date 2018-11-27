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

        NetworkData data = new NetworkData(NetworkData.NetworkMessageType.MESSAGE, message);
        NetworkManager.Instance.SendData(data);

        Debug.Log("Message Sent" + data.Message);
    }
}