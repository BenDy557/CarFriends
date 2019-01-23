using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using UnibusEvent;
using NetworkBon;

public class VehicleController : MonoBehaviour
{
    [SerializeField]
    private Vehicle m_vehicle;

    private VehicleInput m_vehicleInput;
    public VehicleInput VehicleInput
    {
        get
        {
            return m_vehicleInput;
        }
    }

    public bool isPlayer = true;

    [SerializeField]
    private Transform m_target;

    [SerializeField]
    private AnimationCurve m_steeringScalarCurve;


    [SerializeField]
    private NetObject m_netObject = null;
    public int NetID
    {
        get
        {
            return m_netObject.ID;
        }
    }


    private void Awake()
    {
        SubscribeToEvents();
    }

    private void Start()
    {
        m_vehicleInput = m_vehicle.VehicleInput;
    }

    // Update is called once per frame
    private void Update ()
    {
        if (isPlayer)//TODO// remove
        {
            m_vehicleInput.steering = Input.GetAxis("Steering");
            m_vehicleInput.acceleration = (Input.GetAxis("Acceleration"));// + 1) * 0.5f;
            //m_vehicleInput.acceleration = (Input.GetAxis("Acceleration") + 1) * 0.5f;
            m_vehicleInput.braking = (Input.GetAxis("Decceleration"));// + 1) * 0.5f;
            m_vehicleInput.handBrake = Input.GetButton("Handbrake");
        }
        else if (!m_netObject.IsNetworkControlled)
        {
            m_vehicleInput.acceleration = 0.5f;
            SteerTowards(m_target);
        }
    }

    private void SteerTowards(Transform target)
    {
        if (target == null)
            return;

        float angleDiff = Vector3.SignedAngle(transform.forward.Flatten(), (m_target.transform.position - transform.position).Flatten(), Vector3.up);


        m_vehicleInput.steering = m_steeringScalarCurve.Evaluate(angleDiff);
    }

    private void ReceiveInputData(NetworkData dataIn)
    {
        //m_vehicleInput = dataIn.Input
    }

    private void SubscribeToEvents()
    {
        this.BindUntilDestroy<NetworkData>(EventTags.NetDataReceived_Input, ReceiveInputData);
    }

}

