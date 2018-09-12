﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour
{
	/*[SerializeField]
	private VehicleSetUpData vehicleSetupData;*/

    [SerializeField]
    private Rigidbody m_rigidBody;
    public Rigidbody RigidBody {get{return m_rigidBody;}}

    [SerializeField]
    private Axle m_front;

    [SerializeField]
    private Axle m_rear;

    private List<Axle> m_axles = new List<Axle>();
    public List<Axle> Axles {get{return m_axles;}}
    /*private List<Wheel> m_wheels;
    public IList<Wheel> Wheels 
    {
        get
        {
            return m_wheels;
        }    
    }*/

    //Input
	private VehicleInput m_vehicleInput = null;
    private bool m_flip = false;//TODO should be externalised in input class

    public bool IsStopped { get { return m_rigidBody.velocity.sqrMagnitude < 1f; } }
    public bool IsMovingForward { get { return Vector3.Angle(transform.forward, m_rigidBody.velocity) < 90f; } }
    public float Speed { get { return m_rigidBody.velocity.magnitude; } }

    private void Awake()
    {
        m_axles.Add(m_front);
        m_axles.Add(m_rear);
		//Init(vehicleSetupData);

        //m_wheels = new List<Wheel>();
        //m_wheels.AddRange(m_frontWheels);
        //m_wheels.AddRange(m_rearWheels);


        //m_front = new Axle(m_frontWheels);
        //m_rear = new Axle(m_rearWheels);
    }

	public void Init(VehicleSetUpData setupData, VehicleInput vehicleInput, List<Axle> axles)
	{
		m_vehicleInput = vehicleInput;
		//m_maxWheelTorque = setupData.MaxEngineTorque;
        m_rigidBody = GetComponent<Rigidbody>();
        //m_rigidBody.centerOfMass = setupData.
        
        //TODO null check, should also be a list
        m_front = axles[0];
        m_rear = axles[1];
	}

    private void Update()
    {
        /*m_steeringInput = Input.GetAxis("Steering");
        m_accelerationInput = Input.GetAxis("Acceleration");
        m_deccelerationInput = Input.GetAxis("Decceleration");
        m_handbrake = Input.GetButton("Handbrake");*/

        if (!m_flip)
            m_flip = Input.GetButtonDown("Flip");

		//STEERING
		m_front.Steer(m_vehicleInput.steering);
		m_rear.Steer(m_vehicleInput.steering, true);

		//DRIVE
		m_front.Drive(m_vehicleInput.acceleration);
		m_rear.Drive(m_vehicleInput.acceleration);

		//BRAKING
		m_front.Brake(m_vehicleInput.braking, m_vehicleInput.handBrake);
		m_rear.Brake(m_vehicleInput.braking, m_vehicleInput.handBrake);				
	}

    private void FixedUpdate()
    {
        if (m_flip)
        {
            m_flip = false;
            m_rigidBody.AddForce(Vector3.up * 7f, ForceMode.VelocityChange);
            m_rigidBody.AddRelativeTorque(Vector3.forward * 2f, ForceMode.VelocityChange);
        }


		
		//m_front.OnUpdate(m_vehicleInput);
		//m_rear.OnUpdate (m_vehicleInput);
    }


	public void SetVehicleInput(VehicleInput vehicleInput)
	{
		m_vehicleInput = vehicleInput;
	}
}
