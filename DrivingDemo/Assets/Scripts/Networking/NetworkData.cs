using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public struct NetworkData
{
    public NetworkDataType DataType { get; private set; }
    public int NetworkObjectID { get; private set; }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public string Message;
    public LocomotionData LocomotionData { get; private set; }
    public NetworkMessageType MessageType { get; private set; }

    //public VehicleInput Input;

    public NetworkData(NetworkDataType type, int networkObjectID)
    {
        DataType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        LocomotionData = new LocomotionData();
        MessageType = NetworkMessageType.NONE;
        //Input = null;
    }

    public NetworkData(NetworkDataType type, int networkObjectID, LocomotionData locomotionData, VehicleInput input)
    {
        DataType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        LocomotionData = locomotionData;
        MessageType = NetworkMessageType.NONE;
        //Input = input;
    }

    public NetworkData(NetworkDataType type, int networkObjectID, LocomotionData locomotionData)
    {
        DataType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        LocomotionData = locomotionData;
        MessageType = NetworkMessageType.NONE;
        //Input = null;
    }

    public NetworkData(NetworkDataType type, int networkObjectID, VehicleInput input)
    {
        DataType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        LocomotionData = new LocomotionData();
        MessageType = NetworkMessageType.NONE;
        //Input = input;
    }

    public NetworkData(NetworkDataType type, string message)
    {
        DataType = type;
        NetworkObjectID = -1;
        Message = message;
        LocomotionData = new LocomotionData();
        MessageType = NetworkMessageType.NONE;
        //Input = null;
    }

    public NetworkData(NetworkDataType type, NetworkMessageType messageType)
    {
        DataType = type;
        NetworkObjectID = -1;
        Message = null;
        LocomotionData = new LocomotionData();
        MessageType = messageType;
        //Input = null;
    }

    public NetworkData(NetworkDataType type, NetworkMessageType messageType, LocomotionData locomotionData)
    {
        DataType = type;
        NetworkObjectID = -1;
        Message = null;
        LocomotionData = locomotionData;
        MessageType = messageType;
        //Input = null;
    }

    public enum NetworkDataType
    {
        NETWORK_MESSAGE,//join, leave, kick, 
        SERVER_BROADCAST,//hey guys, I'm here
        LOCOMOTION,//velocity and position and rotation
        INPUT,//input state
    }

    public enum NetworkMessageType
    {
        NONE,
        JOIN_REQUEST,//send locomotion data
        JOIN_ACCEPT,//send back netID
        JOIN_DENIED,
    }
}

public struct LocomotionData
{
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public Vector3 Velocity { get; private set; }
    public Vector3 AngularVelocity { get; private set; }

    public LocomotionData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVel)
    {
        Position = position;
        Rotation = rotation;
        Velocity = velocity;
        AngularVelocity = angularVel;
    }

    public LocomotionData(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
        Velocity = Vector3.zero;
        AngularVelocity = Vector3.zero;
    }

    public LocomotionData(Vector3 velocity, Vector3 angularVel)
    {
        Position = Vector3.zero;
        Rotation = Quaternion.identity;
        Velocity = velocity;
        AngularVelocity = angularVel;
    }
}

