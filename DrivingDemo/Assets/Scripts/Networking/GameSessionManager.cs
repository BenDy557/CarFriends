using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnibusEvent;
using System.Net;
using System.Net.Sockets;
using NetworkBon;

public class GameSessionManager : Singleton<GameSessionManager>
{
    private bool m_gameStarted = false;

    [SerializeField]
    private Transform m_spawnPoint;

    [SerializeField, Required]
    private GameObject m_vehiclePrefab = null;
    [SerializeField, Required]
    private GameObject m_cameraPrefab = null;

    [SerializeField]
    private Vehicle m_localPlayer = null;
    private List<NetworkPlayer> m_players = new List<NetworkPlayer>();

    private void OnEnable()
    {
        this.BindUntilDestroy(EventTags.OnServerStart, StartServer);
        this.BindUntilDestroy(EventTags.OnClientStart, StartClient);
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

    private void NetworkMessageReceived(NetworkData dataIn)
    {
        switch (dataIn.MessageType)
        {
            case NetworkMessageType.NONE:
                break;
            case NetworkMessageType.JOIN_REQUEST:
                OnJoinRequest(dataIn);
                break;
            case NetworkMessageType.JOIN_DENIED:
                break;
            case NetworkMessageType.JOIN_ACCEPT:
                SyncSessionWithServer(dataIn);
                break;
        }
    }

    [Button]
    private void StartGame()
    {
        if (m_gameStarted)
            return;

        m_gameStarted = true;
        //SpawnSelf
        SpawnVehicle(true, m_spawnPoint.position, m_spawnPoint.rotation,-1,true);
    }

    //SERVER//////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    #region ServerMethods
    private void StartServer()
    {
        //subsribe to join message
        Unibus.Subscribe<NetworkData>(EventTags.NetDataReceived_Network_Message, NetworkMessageReceived);

        StartGame();

        m_players.Add(new NetworkPlayer(m_localPlayer.NetID, "Server", null, m_localPlayer));
    }

    private void ServerUpdate()
    {
        if (NetworkManager.Instance.NetworkRole != NetworkRole.SERVER)
        {
            Debug.LogError("NotServer");
            return;
        }

        //send player data to all clients
        for (int clientIndex = 0; clientIndex < m_players.Count; clientIndex++)
        {
            //if player is server or there is no socket set up
            if (m_players[clientIndex].ClientRemoteEndPoint == null || m_players[clientIndex].Vehicle == m_localPlayer)
                continue;

            for (int vehicleIndexToSend = 0; vehicleIndexToSend < m_players.Count; vehicleIndexToSend++)
            {
                //dont send data back where it came from
                if (clientIndex == vehicleIndexToSend)
                    continue;

                NetworkData vehicleData = m_players[vehicleIndexToSend].Vehicle.GetNetworkData();
                NetworkManager.Instance.SendDataToClient(m_players[clientIndex].ClientRemoteEndPoint, vehicleData);
            }
        }
    }

    private void OnJoinRequest(NetworkData dataIn)
    {
        if (NetworkManager.Instance.NetworkRole != NetworkRole.SERVER)
        {
            Debug.LogError("JoinRequest when not server");
            return;
        }

        Debug.LogWarning("BadCode");

        string playerName = dataIn.Message;//TODO//message should contain actual player name, this message currently contains the ip addres, not the name
        //UdpClient socket = NetworkManager.Instance.AddClient(dataIn.Message);
        Debug.LogWarning("BadCode");
        m_localPlayer.NetObject.Init(false);
        Vehicle vehicle = SpawnVehicle(false, dataIn.LocomotionData.Position, dataIn.LocomotionData.Rotation,-1,false);//TODO//Should use dedicated spawner
        //need to send a reply to new player, message should specify network id.

        NetworkPlayer networkPlayer = new NetworkPlayer(vehicle.NetID, playerName, NetworkManager.Instance.GetClientEndPoint(dataIn.Message), vehicle);
        m_players.Add(networkPlayer);
        //On join request accepted
        NetworkData tempData = new NetworkData(NetworkDataType.NETWORK_MESSAGE, NetworkMessageType.JOIN_ACCEPT, vehicle.NetID);

        NetworkManager.Instance.SendDataToClient(networkPlayer.ClientRemoteEndPoint, tempData);
        Debug.Log("JOIN_ACCEPT sent");
    }
    #endregion

    //CLIENT//////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    #region ClientMethods
    private void StartClient()
    {
        Unibus.Subscribe<NetworkData>(EventTags.NetDataReceived_Server_Brodacast, ServerFound);
        Unibus.Subscribe<NetworkData>(EventTags.NetDataReceived_Network_Message, NetworkMessageReceived);

        StartGame();

        m_players.Add(new NetworkPlayer(m_localPlayer.NetID, "Client", null, m_localPlayer));
    }

    private void ClientUpdate()
    {
        if (NetworkManager.Instance.NetworkRole != NetworkRole.CLIENT)
        {
            Debug.LogError("JoinRequest when not client");
            return;
        }

        NetworkData netData = m_localPlayer.GetNetworkData();
        NetworkManager.Instance.SendDataToServer(netData);
    }

    private void ServerFound(NetworkData dataIn)
    {
        if (NetworkManager.Instance.NetworkRole != NetworkRole.CLIENT)
        {
            Debug.LogError("Server found when not client");
            return;
        }

        //I've received a brodcast but has my server socket already been set up
        if (NetworkManager.Instance.IsServerSet)
            return;

        Debug.Log("broadcast from: " + dataIn.Message);
        NetworkManager.Instance.SetRemoteServerAddress(dataIn.Message);
        LocomotionData locomotionData = new LocomotionData(m_spawnPoint.position, m_spawnPoint.rotation);
        NetworkData tempData = new NetworkData(NetworkDataType.NETWORK_MESSAGE, NetworkMessageType.JOIN_REQUEST, NetworkManager.Instance.LocalIPAddrsIPV4.ToString(), locomotionData);
        NetworkManager.Instance.SendDataToServer(tempData);

        Debug.Log("JOIN_REQUEST sent");
    }

    //JOIN_ACCEPT message received
    private void SyncSessionWithServer(NetworkData dataIn)
    {
        Debug.Log("JOIN_ACCEPT received");

        if (NetworkManager.Instance.NetworkRole != NetworkRole.CLIENT)
        {
            Debug.LogError("cant sync with server unless client");
            return;
        }

        //InitialiseOwnNetObject
        m_localPlayer.NetObject.Init(false, dataIn.NetworkObjectID);

        Unibus.Subscribe<NetworkData>(EventTags.NetDataReceived_Locomotion, OnLocomotionReceipt);
    }
    #endregion

    private void OnLocomotionReceipt(NetworkData netData)
    {
        foreach (NetworkPlayer player in m_players)
        {
            if (player.ID != netData.NetworkObjectID)
                continue;

            player.Vehicle.ReceiveNetworkData(netData);
            return;
        }


        //never accept unauthorised locomotion data when acting as server
        if (NetworkManager.Instance.NetworkRole == NetworkRole.CLIENT)
        {
            Vehicle newPlayer = SpawnVehicle(false, netData.LocomotionData.Position, netData.LocomotionData.Rotation, netData.NetworkObjectID, false);
            NetworkPlayer newNetworkPlayer = new NetworkPlayer(netData.NetworkObjectID, "NewPlayer", null, newPlayer);
            newPlayer.ReceiveNetworkData(netData);
            m_players.Add(newNetworkPlayer);
        }
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

    //TODO//should use dedicated spawner
    private Vehicle SpawnVehicle(bool isPlayer, Vector3 position, Quaternion rotation, int netID = -1, bool offlineMode = true)
    {
        Vehicle vehicle = Instantiate(m_vehiclePrefab, position, rotation).GetComponent<Vehicle>();

        Debug.LogWarning("BadCode");
        //Shouldnt be accessing public variables like this
        if (isPlayer)
        {
            m_localPlayer = vehicle;
            if (!offlineMode)
                vehicle.NetObject.Init(false, netID);

            vehicle.GetComponent<VehicleController>().isPlayer = true;
            UnityStandardAssets.Cameras.AutoCam localCamera = Instantiate(m_cameraPrefab).GetComponent<UnityStandardAssets.Cameras.AutoCam>();
            localCamera.SetTarget(vehicle.transform);
        }
        else
        {
            if (!offlineMode)
                vehicle.NetObject.Init(true, netID);
        }

        return vehicle;
    }

    private void OnDrawGizmos()
    {
        Utils.DrawCross(m_spawnPoint.position, Color.magenta);
    }
}

public class NetworkPlayer
{
    public NetworkPlayer(int netID, string nameIn, IPEndPoint iPEndPoint, Vehicle vehicleIn)
    {
        m_id = netID;
        m_userName = nameIn;
        m_clientRemoteEndPoint = iPEndPoint;
        m_vehicle = vehicleIn;
    }

    private int m_id;
    public int ID { get { return m_id; } }
    private string m_userName;
    public string UserName { get { return m_userName; } }
    private IPEndPoint m_clientRemoteEndPoint;
    public IPEndPoint ClientRemoteEndPoint { get { return m_clientRemoteEndPoint; } }
    private Vehicle m_vehicle;
    public Vehicle Vehicle { get { return m_vehicle; } }
}
