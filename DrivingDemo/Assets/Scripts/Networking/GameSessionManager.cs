using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnibusEvent;

public class GameSessionManager : Singleton<GameSessionManager>
{
    [SerializeField]
    private Vector3 m_spawnPosition;

    [SerializeField, Required]
    private GameObject m_vehiclePrefab = null;
    [SerializeField, Required]
    private GameObject m_cameraPrefab = null;

    [SerializeField]
    private Vehicle m_localPlayer = null;
    private List<Vehicle> m_players = new List<Vehicle>();

    public enum Role
    {
        NONE,
        SERVER,
        CLIENT,
    }

    private Role m_role = Role.NONE;
    private bool SessionStarted { get { return m_role != Role.NONE; } }


    private void Update()
    {
        //receive updates from all clients
        switch (m_role)
        {
            case Role.NONE:
                break;
            case Role.SERVER:
                ServerUpdate();
                break;
            case Role.CLIENT:
                ClientUpdate();
                break;
        }
    }

    private void ServerUpdate()
    {
        //wait for connection messages
        //send player data to all clients
        /*for (int playerIndexToSendTo = 0; playerIndexToSendTo < m_players.Count; playerIndexToSendTo++)
        {
            for (int vehicleIndexToSend = 0; vehicleIndexToSend < m_players.Count; vehicleIndexToSend++)
            {

            }
        }*/
    }

    private void ClientUpdate()
    {

    }


    [Button]
    private void StartServer()
    {
        if (SessionStarted)
            return;
        m_role = Role.SERVER;

        //subsribe to join message
        Unibus.Subscribe<NetworkData>(EventTags.NetDataReceived_Join, PlayerJoin);

        m_localPlayer = SpawnVehicle(m_spawnPosition,Quaternion.identity);
        Debug.LogWarning("BadCode");
        //Shouldnt be accessing public variables like this
        m_localPlayer.GetComponent<VehicleController>().isPlayer = true;
        UnityStandardAssets.Cameras.AutoCam localCamera = Instantiate(m_cameraPrefab).GetComponent<UnityStandardAssets.Cameras.AutoCam>();
        localCamera.SetTarget(m_localPlayer.transform);
    }

    [Button]
    private void StartClient()
    {
        if (SessionStarted)
            return;
        m_role = Role.CLIENT;


        m_localPlayer = SpawnVehicle(m_spawnPosition, Quaternion.identity);
        Debug.LogWarning("BadCode");
        //Shouldnt be accessing public variables like this
        m_localPlayer.GetComponent<VehicleController>().isPlayer = true;
        UnityStandardAssets.Cameras.AutoCam localCamera = Instantiate(m_cameraPrefab).GetComponent<UnityStandardAssets.Cameras.AutoCam>();
        localCamera.SetTarget(m_localPlayer.transform);


        //LocomotionData locomotionData = new LocomotionData(m_localPlayer.transform.position, m_localPlayer.transform.rotation);
        //NetworkData tempData = new NetworkData(NetworkData.NetworkMessageType.JOIN, m_localPlayer.NetID, locomotionData);
        LocomotionData locomotionData = new LocomotionData(m_spawnPosition, Quaternion.identity);
        NetworkData tempData = new NetworkData(NetworkData.NetworkMessageType.JOIN, 2, locomotionData);

        NetworkManager.instance.SendData(tempData);
    }

    /*[Button]
    private void StopSession()
    {
        for (int i = 0; i < m_players.Count; i++)
        {
            NetworkData tempData = new NetworkData(NetworkData.NetworkMessageType.CLOSE, m_players[i].NetID);
            NetworkManager.instance.SendData(tempData);
        }
    }*/

    private void PlayerJoin(NetworkData dataIn)
    {
        Debug.LogWarning("BadCode");
        //Should use dedicated spawner
        Vehicle spawnedVehicle = SpawnVehicle(dataIn.LocomotionData.Position, dataIn.LocomotionData.Rotation);
        spawnedVehicle.NetObject.Init(dataIn.NetworkObjectID, true);
    }

    private Vehicle SpawnVehicle(Vector3 position, Quaternion rotation)
    {
        return Instantiate(m_vehiclePrefab, position, rotation).GetComponent<Vehicle>();
    }

    private void OnDrawGizmos()
    {
        Utils.DrawCross(m_spawnPosition,Color.magenta);
    }
}
