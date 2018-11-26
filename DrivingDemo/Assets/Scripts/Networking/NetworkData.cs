using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NetworkData
{
    public enum NetworkMessageType
    {
        MESSAGE,//just a message
        LOCOMOTION,//velocity and position
        INPUT,//input state
    }

    NetworkData(NetworkMessageType type, int networkObjectID)
    {
        MessageType = type;
        NetworkObjectID = networkObjectID;
    }

    public NetworkMessageType MessageType;
    public int NetworkObjectID;



}

