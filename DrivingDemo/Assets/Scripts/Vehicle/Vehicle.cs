using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;
using NaughtyAttributes;
using NetworkBon;

public class Vehicle : MonoBehaviour, INetObject
{
    //[SerializeField]
    //private VehicleSetUpData vehicleSetupData;
    //WheelCollider m_frontWheelPrefab;
    //WheelCollider m_rearWheelPrefab;
    [SerializeField]
    private VehicleSetUpData m_setupData;

    [SerializeField]
    private VehicleController m_controller;
    public VehicleController Controller { get { return m_controller; } }

    [SerializeField]
    private NetObject m_netObject = null;
    public NetObject NetObject { get { return m_netObject; } }

    public int NetID
    {
        get
        {
            return m_netObject.ID;
        }
    }

    [SerializeField]
    private Rigidbody m_rigidBody;
    public Rigidbody Rigidbody { get { return m_rigidBody; } }

    [SerializeField]
    private Chassis m_chassis;

    [SerializeField]
    private Drive m_engine;
    public Drive Engine { get { return m_engine; } }

    private bool m_initialised = false;
    public bool Initialised { get { return m_initialised; } }

    [SerializeField]
    private List<VehicleEffect> m_effects = new List<VehicleEffect>();

    [SerializeField]
    private GameObject m_draftingPrefab = null;
    [SerializeField]
    private float m_draftingSpeedLimit = 1f;
    private BoostZone m_prevDraftingObject;
    private float m_draftingSpawnDistance = 2f;

    //Input
    VehicleInput m_vehicleInput = new VehicleInput();
    public VehicleInput VehicleInput { get { return m_vehicleInput; } }

    private float m_accelerationInput;
    private float m_deccelerationInput;
    private float m_steeringInput;
    //private bool m_flip = false;
    //private bool m_handbrake = false;

    public bool IsGrounded
    {
        get
        {
            foreach (Wheel wheel in Wheels)
            {
                if (wheel.Collider.IsGrounded)
                    return true;
            }

            return false;
        }
    }


    [ReadOnly]    
    private float m_maxEngineTorque;
    public float MaxEngineTorque
    {
        get { return m_maxEngineTorque; }
        set
        {
            //find a better way of modifying vehicle data between components
            Debug.LogWarning("BAD CODE");
            m_maxEngineTorque = value;
        }
    }


    public IEnumerable<Wheel> Wheels
    {
        get
        {
            foreach (Axle axle in m_engine.Axles)
            {
                foreach (Wheel wheel in axle.Wheels)
                {
                    yield return wheel;
                }
            }
        }
    }

    private void Awake()
    {
        SubscribeToEvents();
    }

    private void Start()
    {
        Init(-1, -1);
    }

    private void Update()
    {
        for(int i = m_effects.Count -1;i>=0;i--)
        {
            m_effects[i].Update();
            if (m_effects[i].IsFinished)
                m_effects.RemoveAt(i);
        }

        if (m_rigidBody.velocity.sqrMagnitude > m_draftingSpeedLimit * m_draftingSpeedLimit)
        {
            if (m_prevDraftingObject == null || Vector3.Distance(m_prevDraftingObject.transform.position, transform.position) > m_draftingSpawnDistance)
            {
                m_prevDraftingObject = Instantiate(m_draftingPrefab, transform.position, Quaternion.identity).GetComponent<BoostZone>();
                m_prevDraftingObject.AddToIgnoreList(this);
                Destroy(m_prevDraftingObject.gameObject, 1.2f);
            }
        }

        if (m_rigidBody.velocity.magnitude < 0.1f && (Vector3.Angle(transform.up, Vector3.down) < 45f))
            Flip();


        if (m_vehicleInput.jump && IsGrounded)
            Jump();
    }

    public void Init(int localControllerID, int netID)
    {
        if (m_initialised)
            return;

        m_initialised = true;

        NetObject.Init(false, netID);
        m_controller.Init(localControllerID);//.isPlayer = true;

        m_engine.SetVehicleInput(m_vehicleInput);

        if (m_setupData != null)
            ApplySetupData(m_setupData);

    }

    [Button]
    private void ApplySetupData()
    {
        ApplySetupData(m_setupData);
    }

    private void ApplySetupData(VehicleSetUpData setupData)
    {
        m_rigidBody.mass = setupData.Mass;
        //m_rigidBody.drag = 0f;
        //m_rigidBody.angularDrag = 0f;
        //m_rigidBody.useGravity = true;
        //m_rigidBody.isKinematic = false;
        //m_rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        //m_rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        //m_rigidBody.constraints = RigidbodyConstraints.None;
        m_rigidBody.centerOfMass = m_chassis.CentreOfMass;

        m_maxEngineTorque = setupData.MaxEngineTorque;

        m_engine.ApplySetupData(setupData, m_vehicleInput);


        //setupData.MaxBrakingTorque
        //setupData.MaxEngineTorque
        //setupData.HandBrakeTorque
        //setupData.Axles
    }

    /*private void Update()
    {
        if (!m_netObject.IsNetworkControlled)
        {
            SendLocomotionInfo();
        }
    }*/
    public void Flip()
    {
        m_rigidBody.AddForce(Vector3.up* 7f, ForceMode.VelocityChange);
        m_rigidBody.AddRelativeTorque(Vector3.forward* 2.5f, ForceMode.VelocityChange);
    }

    public void Jump()
    {
        m_rigidBody.AddForce(transform.up * 7f, ForceMode.VelocityChange);
    }

    #region Effects
    public void ApplyEffect(VehicleEffect effectIn)
    {
        if (m_effects.Count >= 1)
            return;

        m_effects.Add(effectIn);
        effectIn.Start(this);
    }
    #endregion

    #region Network


    public void ReceiveNetworkData(NetworkData networkData) { }
    public void ReceiveMessageData(NetworkData networkData) { }
    public void ReceiveLocomotionData(NetworkData networkData) { }
    public void ReceiveInputData(NetworkData networkData) { }
    public void ReceiveJoinData(NetworkData networkData) { }

    public NetworkData GetNetworkData()
    {
        LocomotionData tempData = new LocomotionData(m_rigidBody.position, m_rigidBody.rotation, m_rigidBody.velocity, m_rigidBody.angularVelocity);
        //NetworkManager.Instance.SendData(new NetworkData(NetworkData.NetworkMessageType.LOCOMOTION, NetID, tempData));

        return new NetworkData(NetworkDataType.LOCOMOTION, NetID, tempData);
    }

    private void ReceiveLocomotionInfo(NetworkData dataIn)
    {
        if (dataIn.NetworkObjectID != NetID && !m_netObject.IsNetworkControlled)
            return;

        m_rigidBody.MovePosition(dataIn.LocomotionData.Position);
        m_rigidBody.MoveRotation(dataIn.LocomotionData.Rotation);
        m_rigidBody.velocity = dataIn.LocomotionData.Velocity;
        m_rigidBody.angularVelocity = dataIn.LocomotionData.AngularVelocity;
    }

    private void SubscribeToEvents()
    {
        this.BindUntilDestroy<NetworkData>(EventTags.NetDataReceived_Locomotion, ReceiveLocomotionInfo);
    }
    #endregion
}

[System.Serializable]
public class VehicleInput
{
    public float steering;
    public float acceleration;
    public float braking;
    public bool handBrake;
    public bool jump;
    //public bool flip;
}


