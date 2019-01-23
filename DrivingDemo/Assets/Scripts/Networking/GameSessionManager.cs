using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnibusEvent;
using System.Net.Sockets;
using NetworkBon;

public class GameSessionManager : Singleton<GameSessionManager>
{
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

    //SERVER//////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    #region ServerMethods
    private void StartServer()
    {
        //subsribe to join message
        Unibus.Subscribe<NetworkData>(EventTags.NetDataReceived_Network_Message, NetworkMessageReceived);
        SpawnVehicle(true, m_spawnPoint.position, m_spawnPoint.rotation);
    }

    private void ServerUpdate()
    {
        if (NetworkManager.Instance.NetworkRole != NetworkRole.SERVER)
        {
            Debug.LogError("NotServer");
            return;
        }

        //wait for connection messages
        //send player data to all clients
        /*for (int playerIndexToSendTo = 0; playerIndexToSendTo < m_players.Count; playerIndexToSendTo++)
        {
            for (int vehicleIndexToSend = 0; vehicleIndexToSend < m_players.Count; vehicleIndexToSend++)
            {

            }
        }*/
    }

    private void OnJoinRequest(NetworkData dataIn)
    {
        if (NetworkManager.Instance.NetworkRole != NetworkRole.SERVER)
        {
            Debug.LogError("JoinRequest when not server");
            return;
        }

        Debug.Log("JOIN_REQUEST received");

        Debug.LogWarning("BadCode");

        string playerName = dataIn.Message;//TODO//message should contain actual player name, this message currently contains the ip addres, not the name
        UdpClient socket = NetworkManager.Instance.AddClient(dataIn.Message);
        Debug.LogWarning("BadCode");
        Vehicle vehicle = SpawnVehicle(false, dataIn.LocomotionData.Position, dataIn.LocomotionData.Rotation);//TODO//Should use dedicated spawner
        //need to send a reply to new player, message should specify network id.

        NetworkPlayer networkPlayer = new NetworkPlayer(playerName, socket, vehicle);
        m_players.Add(networkPlayer);
        //On join request accepted
        NetworkData tempData = new NetworkData(NetworkDataType.NETWORK_MESSAGE, NetworkMessageType.JOIN_ACCEPT, vehicle.NetID);

        NetworkManager.Instance.SendDataToClient(networkPlayer.Socket, tempData);
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
    }

    private void ClientUpdate()
    {
        if (NetworkManager.Instance.NetworkRole != NetworkRole.CLIENT)
        {
            Debug.LogError("JoinRequest when not client");
            return;
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
        NetworkData tempData = new NetworkData(NetworkDataType.NETWORK_MESSAGE, NetworkMessageType.JOIN_REQUEST, locomotionData);
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

        //SpawnYourself
        SpawnVehicle(true, m_spawnPoint.position, m_spawnPoint.rotation, dataIn.NetworkObjectID);
    }
    #endregion

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
    private Vehicle SpawnVehicle(bool isPlayer, Vector3 position, Quaternion rotation, int netID = -1)
    {
        Vehicle vehicle = Instantiate(m_vehiclePrefab, position, rotation).GetComponent<Vehicle>();

        Debug.LogWarning("BadCode");
        //Shouldnt be accessing public variables like this
        if (isPlayer)
        {
            m_localPlayer = vehicle;
            vehicle.NetObject.Init(false, netID);
            vehicle.GetComponent<VehicleController>().isPlayer = true;
            UnityStandardAssets.Cameras.AutoCam localCamera = Instantiate(m_cameraPrefab).GetComponent<UnityStandardAssets.Cameras.AutoCam>();
            localCamera.SetTarget(vehicle.transform);
        }
        else
        {
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
    public NetworkPlayer(string nameIn, UdpClient socketIn, Vehicle vehicleIn)
    {
        m_userName = nameIn;
        m_socket = socketIn;
        m_vehicle = vehicleIn;
    }

    private string m_userName;
    public string UserName { get { return m_userName; } }
    private UdpClient m_socket;
    public UdpClient Socket { get { return m_socket; } }
    private Vehicle m_vehicle;
    public Vehicle Vehicle { get { return m_vehicle; } }
}