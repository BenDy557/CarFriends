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

    private void OnEnable()
    {
        this.BindUntilDestroy(EventTags.OnServerStart, StartServer);
    }

    private void Update()
    {
        //receive updates from all clients
        switch (NetworkManager.Instance.NetworkRole)
        {
            case NetworkRole.NONE:
                break;
            case NetworkRole.SERVER:
                ServerUpdate();
                break;
            case NetworkRole.CLIENT:
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

    private void StartServer()
    {
        //subsribe to join message
        Unibus.Subscribe<NetworkData>(EventTags.NetDataReceived_Join, PlayerJoin);

        m_localPlayer = SpawnVehicle(m_spawnPosition,Quaternion.identity);
        Debug.LogWarning("BadCode");
        //Shouldnt be accessing public variables like this
        m_localPlayer.GetComponent<NetObject>().Init(1, false);
        m_localPlayer.GetComponent<VehicleController>().isPlayer = true;
        UnityStandardAssets.Cameras.AutoCam localCamera = Instantiate(m_cameraPrefab).GetComponent<UnityStandardAssets.Cameras.AutoCam>();
        localCamera.SetTarget(m_localPlayer.transform);
    }

    private void StartClient()
    {
        /*m_localPlayer = SpawnVehicle(m_spawnPosition, Quaternion.identity);
        Debug.LogWarning("BadCode");
        //Shouldnt be accessing public variables like this
        m_localPlayer.GetComponent<NetObject>().Init(2, false);
        m_localPlayer.GetComponent<VehicleController>().isPlayer = true;
        UnityStandardAssets.Cameras.AutoCam localCamera = Instantiate(m_cameraPrefab).GetComponent<UnityStandardAssets.Cameras.AutoCam>();
        localCamera.SetTarget(m_localPlayer.transform);*/

        //LocomotionData locomotionData = new LocomotionData(m_spawnPosition, Quaternion.identity);
        //NetworkData tempData = new NetworkData(NetworkData.NetworkMessageType.JOIN, 2, locomotionData);

        //NetworkManager.Instance.SendData(tempData);


        Unibus.Subscribe<NetworkData>(EventTags.NetDataReceived_Server_Brodacast, ServerFound);
    }

    private void ServerFound(NetworkData dataIn)
    {
        Debug.Log("broadcast from: " + dataIn.Message);
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
