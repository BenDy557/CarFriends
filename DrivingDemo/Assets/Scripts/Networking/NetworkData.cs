using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
public struct NetworkData
{
    public NetworkMessageType MessageType { get; private set; }
    public int NetworkObjectID { get; private set; }
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public string Message;
    public LocomotionData LocomotionData { get; private set;}

    //public VehicleInput Input;

    public NetworkData(NetworkMessageType type, int networkObjectID)
    {
        MessageType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        LocomotionData = new LocomotionData();
        //Input = null;
    }

    public NetworkData(NetworkMessageType type, int networkObjectID, LocomotionData locomotionData, VehicleInput input)
    {
        MessageType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        LocomotionData = locomotionData;
        //Input = input;
    }

    public NetworkData(NetworkMessageType type, int networkObjectID, LocomotionData locomotionData)
    {
        MessageType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        LocomotionData = locomotionData;
        //Input = null;
    }

    public NetworkData(NetworkMessageType type, int networkObjectID, VehicleInput input)
    {
        MessageType = type;
        NetworkObjectID = networkObjectID;
        Message = null;
        LocomotionData = new LocomotionData();
        //Input = input;
    }

    public NetworkData(NetworkMessageType type, string message)
    {
        MessageType = type;
        NetworkObjectID = -1;
        Message = message;
        LocomotionData = new LocomotionData();
        //Input = null;
    }



    public enum NetworkMessageType
    {
        MESSAGE,//just a message
        JOIN,//Player join
        LOCOMOTION,//velocity and position and rotation
        INPUT,//input state
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
}

