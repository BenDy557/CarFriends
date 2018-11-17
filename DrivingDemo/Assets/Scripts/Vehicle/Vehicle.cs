using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    //[SerializeField]
    //private VehicleSetUpData vehicleSetupData;
    //WheelCollider m_frontWheelPrefab;
    //WheelCollider m_rearWheelPrefab;

    [SerializeField]
    private Rigidbody m_rigidBody;
    public Rigidbody Rigidbody { get { return m_rigidBody; } }

    //Axle m_frontAxle;
    [SerializeField]
    private Drive m_engine;
	public Drive Engine{ get { return m_engine; } }

    //Input
	VehicleInput m_vehicleInput;
	public VehicleInput VehicleInput { get { return m_vehicleInput; } }

    private float m_accelerationInput;
    private float m_deccelerationInput;
    private float m_steeringInput;
    //private bool m_flip = false;
    //private bool m_handbrake = false;

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
		m_vehicleInput = new VehicleInput ();
	}

	private void Start()
	{
		m_engine.SetVehicleInput (m_vehicleInput);
	}

	public void Init(VehicleSetUpData setupData)
	{
		m_engine = GetComponent<Drive>();
        m_rigidBody = GetComponent<Rigidbody>();
	}	
}

public class VehicleInput
{
    public float steering;
    public float acceleration;
    public float braking;
    public bool handBrake;

    //public bool flip;
}


