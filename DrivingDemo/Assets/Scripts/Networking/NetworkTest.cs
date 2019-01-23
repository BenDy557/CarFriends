using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System;
using NetworkBon;

class NetworkTest : MonoBehaviour
{
    [Serializable]
    public struct NetworkTestData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Message;
    }

    [SerializeField]
    private NetworkTestData m_messageToSend;

    [SerializeField]
    private NetworkTestData m_messageReceived;


    [SerializeField]
    private NetworkData m_networkDataToSend;
    [SerializeField]
    private NetworkData m_networkDataReceived;


    [SerializeField]
    private string m_remoteIpAddress;
    [SerializeField]
    private int m_TestPort = 6001;
    
    UdpClient m_socket;

    private void OnEnable()
    {
        //m_socket = new UdpClient();
        //clientReceiving = new UdpClient();
        RefreshSocket();
    }

    private void OnDisable()
    {
        m_socket.Close();
        m_socket = null;

        //clientReceiving.Close();
        //clientReceiving = null;
    }

    private void Update()
    {
        byte[] rawDataReceived;

        while (true)
        {
            try
            {
                IPEndPoint senderEndPoint = null;
                //rawDataReceived = clientReceiving.Receive(ref senderEndPoint);
                rawDataReceived = m_socket.Receive(ref senderEndPoint);
            }
            catch
            {
                //nothing//wsa wouldblock should return
                //TODO//should catch other exceptions
                return;
            }

            int size = Marshal.SizeOf(typeof(NetworkTestData));
            IntPtr pointer = Marshal.AllocHGlobal(size);
            Marshal.Copy(rawDataReceived, 0, pointer, size);
            m_messageReceived =  (NetworkTestData)Marshal.PtrToStructure(pointer, typeof(NetworkTestData));

            Debug.Log("TEST"+m_messageReceived.Message);
        }
    }

    //Just make this button send a message to a certain ip address and port
    [NaughtyAttributes.Button]
    private void SendNetMessage()
    {
        SendNetMessage(m_messageToSend);
    }

    private void SendNetMessage<T>(T messageToSend)
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("can only send message in play mode");
            return;
        }

        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(m_remoteIpAddress), m_TestPort);
        m_socket.Connect(remoteEndPoint);

        int dataSize = Marshal.SizeOf(typeof(T));
        byte[] data = new byte[dataSize];

        IntPtr pointer = Marshal.AllocHGlobal(dataSize);
        Marshal.StructureToPtr(messageToSend, pointer, true);
        Marshal.Copy(pointer, data, 0, dataSize);
        Marshal.FreeHGlobal(pointer);

        m_socket.Send(data, dataSize);// remoteEndPoint);
    }

    [NaughtyAttributes.Button]
    private void SendJoinRequest()
    {
        LocomotionData tempLocomotionData = new LocomotionData(Vector3.zero, Quaternion.identity);
        NetworkData tempData = new NetworkData(NetworkDataType.NETWORK_MESSAGE, NetworkMessageType.JOIN_REQUEST, tempLocomotionData);

        SendNetMessage<NetworkData>(tempData);
    }

    //Make this button create a new socket with the given local address and port number
    //there should be an updtae function that receives the data, this button should just update the address and port
    [NaughtyAttributes.Button]
    private void RefreshSocket()
    {
        if (m_socket != null)
            m_socket.Close();
        m_socket = new UdpClient(m_TestPort);
        m_socket.Client.Blocking = false;
        m_socket.Client.MulticastLoopback = true;
        //clientReceiving.Close();

        //clientReceiving = new UdpClient(m_TestPort);
        //clientReceiving.Client.Blocking = false;
        //clientReceiving.Client.MulticastLoopback = true;
    }

}