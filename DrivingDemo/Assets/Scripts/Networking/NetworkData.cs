using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
public struct NetworkData
{
    public enum NetworkMessageType
    {
        MESSAGE,//just a message
        JOIN,//Player join
        LOCOMOTION,//velocity and position
        INPUT,//input state
    }

    public NetworkData(NetworkMessageType type, int networkObjectID)
    {
        MessageType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        Position = Vector3.zero;
        Velocity = Vector3.zero;
        //Input = null;
    }

    public NetworkData(NetworkMessageType type, int networkObjectID, Vector3 position, Vector3 velocity, VehicleInput input)
    {
        MessageType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        Position = position;
        Velocity = velocity;
        //Input = input;
    }

    public NetworkData(NetworkMessageType type, int networkObjectID, Vector3 position, Vector3 velocity)
    {
        MessageType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        Position = position;
        Velocity = velocity;
        //Input = null;
    }

    public NetworkData(NetworkMessageType type, int networkObjectID, VehicleInput input)
    {
        MessageType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        Position = Vector3.zero;
        Velocity = Vector3.zero;
        //Input = input;
    }

    public NetworkData(NetworkMessageType type, string message)
    {
        MessageType = type;
        NetworkObjectID = -1;
        Message = message;
        Position = Vector3.zero;
        Velocity = Vector3.zero;
        //Input = null;
    }

    public NetworkMessageType MessageType;
    public int NetworkObjectID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public string Message;

    public Vector3 Position;
    public Vector3 Velocity;
    //public VehicleInput Input;
}

