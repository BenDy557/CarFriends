using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Needs thought out
public interface INetObject
{
    int NetID { get; }
    NetworkData GetNetworkData();

    void ReceiveNetworkData(NetworkData networkData);
    void ReceiveMessageData(NetworkData networkData);
    void ReceiveLocomotionData(NetworkData networkData);
    void ReceiveInputData(NetworkData networkData);
    void ReceiveJoinData(NetworkData networkData);

}
