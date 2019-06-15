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

    private int m_maxLocalPlayerCount = 4;

    [SerializeField]
    private HashSet<Vehicle> m_localPlayers = new HashSet<Vehicle>();
    public IEnumerable<Vehicle> LocalPlayers
    {
        get
        {
            foreach (Vehicle vehicle in m_localPlayers)
            {
                yield return vehicle;
            }
        }
    }

    //TODO// should probably be a hashset or dictionary, only want unique players
    private List<NetworkPlayer> m_players = new List<NetworkPlayer>();

    private void OnEnable()
    {
        this.BindUntilDestroy(EventTags.OnServerStart, StartServer);
        this.BindUntilDestroy(EventTags.OnClientStart, StartClient);
    }

    private void Update()
    {
        CheckForLocalJoinRequests();



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

    private void CheckForLocalJoinRequests()
    {
        int freeID;
        if (!GetFreePlayerID(out freeID))
            return;

        foreach (Rewired.Joystick joystick in Rewired.ReInput.controllers.Joysticks)
        {
            if (joystick.GetAnyButtonDown())
            {
                bool controllerInUse = false;

                foreach (Rewired.Player player in Rewired.ReInput.players.GetPlayers(false))
                {
                    if (player.controllers.ContainsController(joystick))
                    {
                        controllerInUse = true;
                        break;
                    }
                }

                if (controllerInUse)
                    continue;


                Rewired.Player tempPlayer = Rewired.ReInput.players.GetPlayer(freeID);

                if (tempPlayer.isPlaying)
                    continue;

                tempPlayer.controllers.AddController(joystick, true);
                tempPlayer.isPlaying = true;
                SpawnLocalPlayer(freeID);

                /*foreach (Rewired.Player player in Rewired.ReInput.players.GetPlayers(false))
                {
                    if (player.controllers.ContainsController(joystick))
                        break;
                    else
                    {
                        player.controllers.AddController(joystick, true);
                        player.isPlaying = true;
                        SpawnLocalPlayer();
                        break;
                    }
                }*/
            }
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
    public void StartGame()
    {
        if (m_gameStarted)
            return;

        m_gameStarted = true;
        //SpawnSelf

        int freeID;
        GetFreePlayerID(out freeID);

        SpawnLocalPlayer(freeID);
    }

    [Button]
    public void SpawnLocalPlayer()
    {
        SpawnLocalPlayer(-1);
    }

    public void SpawnLocalPlayer(int controllerID)
    {
        if (m_spawnPoint != null)
            SpawnVehicle(m_spawnPoint.position, m_spawnPoint.rotation, controllerID, -1, true);
        else
            SpawnVehicle(Vector3.one * 10f, Quaternion.identity, controllerID, -1, true);
    }

    private bool GetFreePlayerID(out int freeID)
    {
        List<int> usedIds = new List<int>();
        foreach (Vehicle vehicle in m_localPlayers)
        {
            usedIds.Add(vehicle.Controller.PlayerID);
        }


        freeID = -1;
        int possibleId = 0;
        while (possibleId < m_maxLocalPlayerCount)
        {
            if (usedIds.Contains(possibleId))
            {
                possibleId++;
            }
            else
            {
                freeID = possibleId;
                break;
            }
        }

        return (freeID != -1);
    }

    //SERVER//////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    #region ServerMethods
    private void StartServer()
    {
        //subsribe to join message
        Unibus.Subscribe<NetworkData>(EventTags.NetDataReceived_Network_Message, NetworkMessageReceived);

        StartGame();

        foreach (Vehicle localPlayer in m_localPlayers)
        {
            m_players.Add(new NetworkPlayer(localPlayer.NetID, "Server", null, localPlayer));
        }
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
            if (m_players[clientIndex].ClientRemoteEndPoint == null || m_localPlayers.Contains(m_players[clientIndex].Vehicle))
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
        foreach (Vehicle localPlayer in m_localPlayers)
        {
            localPlayer.NetObject.Init(false);
        }
        Vehicle vehicle = SpawnVehicle(dataIn.LocomotionData.Position, dataIn.LocomotionData.Rotation,-1,-1,false);//TODO//Should use dedicated spawner
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

        foreach (Vehicle localPlayer in m_localPlayers)
        {
            m_players.Add(new NetworkPlayer(localPlayer.NetID, "Client", null, localPlayer));
        }
    }

    private void ClientUpdate()
    {
        if (NetworkManager.Instance.NetworkRole != NetworkRole.CLIENT)
        {
            Debug.LogError("JoinRequest when not client");
            return;
        }

        if (NetworkManager.Instance.IsServerSet)
        {
            Debug.LogWarning("BadCode");
            //network manager should probably concatinate all the data sent to it and then on late update send it all in one packet or something
            //not sure what best practice is RESEARCH is needed
            foreach (Vehicle localPlayer in m_localPlayers)
            {
                NetworkData netData = localPlayer.GetNetworkData();
                NetworkManager.Instance.SendDataToServer(netData);
            }
        }
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
        foreach (Vehicle localPlayer in m_localPlayers)
        {
            Debug.LogWarning("BadCode");
            Debug.LogError("Only one networkID to share with all each local player");
            localPlayer.NetObject.Init(false, dataIn.NetworkObjectID);
        }

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
        if (NetworkManager.Instance.NetworkRole != NetworkRole.SERVER)
        {
            Vehicle newPlayer = SpawnVehicle(netData.LocomotionData.Position, netData.LocomotionData.Rotation, -1, netData.NetworkObjectID, false);
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

    //TODO//should use dedicated spawner//replace Instantiate with pooler
    private Vehicle SpawnVehicle(Vector3 position, Quaternion rotation,int localControllerID = -1, int netID = -1, bool offlineMode = true)
    {
        Vehicle vehicle = Instantiate(PrefabLibrary.Get(PrefabLibrary.VehicleDefault), position, rotation).GetComponent<Vehicle>();

        vehicle.Init(localControllerID, netID);

        Debug.LogWarning("BadCode");
        //Shouldnt be accessing public variables like this
        if (vehicle.Controller.IsPlayer)
        {
            m_localPlayers.Add(vehicle);
            
            UnityStandardAssets.Cameras.AutoCam localCameraController = Instantiate(PrefabLibrary.Get(PrefabLibrary.CameraDefault)).GetComponent<UnityStandardAssets.Cameras.AutoCam>();
            localCameraController.SetTarget(vehicle.transform);
            Camera localCamera = localCameraController.GetComponentInChildren<Camera>();
            //Shouldnt be using get component
            SplitscreenManager.Instance.AddScreen(localCamera);

            Debug.LogWarning("BadCode");//shouldnt just be spawning and setting this directly
            UIVehicleHUD tempHUD = Instantiate(PrefabLibrary.Get(PrefabLibrary.HUDDefault)).GetComponent<UIVehicleHUD>();
            tempHUD.Initialise(vehicle, localCamera);

            WaypointManager.AddLocalPlayer(vehicle);
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
        if (m_spawnPoint != null)
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
