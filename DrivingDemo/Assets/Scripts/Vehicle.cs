using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
	//[SerializeField]
	//private VehicleSetUpData vehicleSetupData;
    //WheelCollider m_frontWheelPrefab;
    //WheelCollider m_rearWheelPrefab;

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
    private bool m_flip = false;
    private bool m_handbrake = false;

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
	}

	// Update is called once per frame
	private void Update ()
    {
		//TODO externalise this into input class
		m_vehicleInput.steering = Input.GetAxis("Steering");
		m_vehicleInput.acceleration = Input.GetAxis("Acceleration");
		m_vehicleInput.braking = Input.GetAxis("Decceleration");
		m_vehicleInput.handBrake = Input.GetButton("Handbrake");
    }

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.matrix = transform.localToWorldMatrix;//worldToLocalMatrix;

		Gizmos.DrawWireSphere(m_engine.RigidBody.centerOfMass,0.3f);
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


