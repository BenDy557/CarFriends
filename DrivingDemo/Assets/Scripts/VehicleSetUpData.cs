using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

[CreateAssetMenu(fileName = "VehicleData", menuName = "Vehicle", order = 1)]
public class VehicleSetUpData : ScriptableObject
{
	[SerializeField]
	private Vector3 m_spawnPosition;

	[SerializeField]
	private string m_presetName;
	public string PresetName { get { return m_presetName; } }

	[SerializeField,ShowAssetPreview]
	private GameObject m_chassisPrefab;

	[SerializeField] private float m_mass;
	public float Mass { get { return m_mass; } }

	[Header("EngineProperties")]
	[SerializeField] private float m_maxBrakingTorque;
	public float MaxBrakingTorque{ get { return m_maxBrakingTorque; } }
	[SerializeField] private float m_HandBrakeTorque;
	public float HandBrakeTorque{ get { return m_HandBrakeTorque; } }
	[SerializeField] private float m_maxEngineTorque;
	public float MaxEngineTorque{ get { return m_maxEngineTorque; } }


	[Button]
	private void UpdateChassis()
	{
		if (m_chassisPrefab != null)
		{
			//TODO retrieve chassis data
			//chassis data determines how many axles a vehicle has, where the wheels are placed and probably some other things
			//m_axles.Clear();
			//for (int i = 0; i < m_chassis.axleCount; i++)
			//{
			//m_axles.Add(new AxleData);
			//}
			Debug.Log("You've got chassis");
		}
	}

	[SerializeField, NaughtyAttributes.BoxGroup("Axles")]
	private List<AxleData> m_axles;
	public IList<AxleData> Axles { get { return m_axles.AsReadOnly(); } }



	[NaughtyAttributes.Button]
	private void GenerateVehicle()
	{
		//VehicleSpawner.SpawnVehicle (this);//TODO
		GameObject vehicleObject = new GameObject("Vehicle "+m_presetName);

		if(Camera.current != null)
			vehicleObject.transform.position = (Camera.current.transform.position + (Camera.current.transform.forward * 10f));
		else
			vehicleObject.transform.position = Vector3.zero;

		//RIGID_BODY/////////////////////////////////////////////////////////////////
		Rigidbody rigidBody = vehicleObject.AddComponent<Rigidbody> ();//TODO make this data driven
		rigidBody.mass = m_mass;
		rigidBody.drag = 0f;
		rigidBody.angularDrag = 0f;
		rigidBody.useGravity = true;
		rigidBody.isKinematic = false;
		rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
		rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		rigidBody.constraints = RigidbodyConstraints.None;

		//VEHICLE/////////////////////////////////////////////////////////////////
		//ENGINE/////////////////////////////////////////////////////////////////
		Vehicle vehicle = vehicleObject.AddComponent<Vehicle>();
		Drive engine = vehicleObject.AddComponent<Drive>();
		
		vehicle.Init (this);
		
		

		//VIEW/////////////////////////////////////////////////////////////////
		GameObject viewObject = new GameObject("View");
		viewObject.transform.SetParent (vehicleObject.transform, false);
		View view = viewObject.AddComponent<View> ();

		GameObject chassisObject = Instantiate(m_chassisPrefab, view.transform);
		Chassis chassis = chassisObject.GetComponent<Chassis>();
		if(chassis == null)
		Debug.LogError("No chassis component found");


		//AXLES/////////////////////////////////////////////////////////////////
		GameObject axlesObject = new GameObject("Axles");
		axlesObject.transform.SetParent (vehicleObject.transform, false);

		List<Axle> axles = new List<Axle>();

		//foreach (AxleData axleData in m_axles)
		for(int i = 0 ; i<m_axles.Count ; i++)
		{
			GameObject axleObject = new GameObject (m_axles[i].Name);
			axleObject.transform.SetParent (axlesObject.transform, false);
			Axle axle = axleObject.AddComponent<Axle> ();
			

			axles.Add(axle);

			List<Wheel> wheels = new List<Wheel>();

			//WHEELS/////////////////////////////////////////////////////////////////
			for(int j = 0 ; j<m_axles[i].WheelCount;j++)
			{
				GameObject wheelObject = new GameObject (m_axles[i].Name + j);
				wheelObject.transform.SetParent (axleObject.transform, false);
				
				if (i == 0)//front
				{
					wheelObject.transform.position = chassis.ForwardAxleOffset + (Vector3.right * (-(m_axles[i].AxleWidth*0.5f) + (j *m_axles[i].AxleWidth)));
				}
				else if (i == 1)//rear //TODO make more robust to work with chassis
				{
					wheelObject.transform.position = chassis.RearAxleOffset + (Vector3.right * (-(m_axles[i].AxleWidth*0.5f) + (j *m_axles[i].AxleWidth)));
				}

				Wheel wheel = wheelObject.AddComponent<Wheel> ();
				wheel.Init(this,m_axles[i]);

				wheels.Add(wheel);
			}

			axle.Init(this,m_axles[i],wheels);
		}

		engine.Init (this,vehicle.VehicleInput,axles);

		
		vehicleObject.transform.position = m_spawnPosition;
	}

}

[System.Serializable]
public class AxleData
{
	[SerializeField]
	private string m_name;
	public string Name { get { return m_name; } }

	[SerializeField]
	private float m_axleWidth = 3f;
	public float AxleWidth { get { return m_axleWidth; } }
	[SerializeField]
	private float m_axleOffset = 0f;//TODO should be replaced by chassis driving this info
	public float AxleOffset { get { return m_axleOffset; } }

	[SerializeField]
	private bool m_steeringEnabled = false;
	public bool SteeringEnabled {get{return m_steeringEnabled;}}
	[SerializeField]
	private bool m_brakingEnabled = false;
	public bool BrakingEnabled {get{return m_brakingEnabled;}}
	[SerializeField]
	private bool m_eBrakingEnabled = false;
	public bool EBrakingEnabled {get{return m_eBrakingEnabled;}}
	[SerializeField]
	private bool m_driveEnabled = false;
	public bool DriveEnabled {get{return m_driveEnabled;}}

	[SerializeField, NaughtyAttributes.BoxGroup("Wheels")]
	private WheelData m_wheelData;
	public WheelData WheelData { get { return m_wheelData; } }

	[SerializeField]
	private int m_wheelCount = 2;
	public int WheelCount { get { return m_wheelCount; } }

	[SerializeField]
	private float m_maxSteerinAngle = 35f;
	public float MaxSteerinAngle { get { return m_maxSteerinAngle; } }
}

[System.Serializable]
public class WheelData
{
	//[SerializeField] private float m_maxWheelAngle = 0;
	//public float MaxWheelAngle {get{return m_maxWheelAngle;}}
	[SerializeField]
	private float m_wheelMass;
	public float WheelMass { get { return m_wheelMass; } }
	
	[SerializeField]
	private float m_wheelRadius;
	public float WheelRadius { get { return m_wheelRadius; } }

	[SerializeField]
	private float m_wheelDampingRate;
	public float WheelDampingRate { get { return m_wheelDampingRate; } }

	[SerializeField]
	private float m_suspensionDistance;
	public float SuspensionDistance { get { return m_suspensionDistance; } }

	[SerializeField]
	private float m_forceAppPointDistance;
	public float ForceAppPointDistance { get { return m_forceAppPointDistance; } }

	[SerializeField]
	private Vector3 m_center;
	public Vector3 Center { get { return m_center; } }

	[SerializeField]	
	private float m_spring;
	public float Spring { get { return m_spring; } }

	[SerializeField]	
	private float m_damper;
	public float Damper { get { return m_damper; } }

	[SerializeField]	
	private float m_targetPosition;
	public float TargetPosition { get { return m_targetPosition; } }


	//Forward
	[SerializeField,Header("Forward")]
	private float m_extremiumSlipForward;
	public float ExtremiumSlipForward { get { return m_extremiumSlipForward; } set { m_extremiumSlipForward = value; } }

	[SerializeField]
	private float m_extremiumValueForward;
	public float ExtremiumValueForward { get { return m_extremiumValueForward; } set { m_extremiumValueForward = value; } }

	[SerializeField]
	private float m_asymptoteSlipForward;
	public float AsymptoteSlipForward { get { return m_asymptoteSlipForward; } set { m_asymptoteSlipForward = value; } }

	[SerializeField]
	private float m_asymptoteValueForward;
	public float AsymptoteValueForward { get { return m_asymptoteValueForward; } set { m_asymptoteValueForward = value; } }

	//Sideways
	[SerializeField,Header("Sideways")]
	private float m_extremiumSlipSideways;
	public float ExtremiumSlipSideways { get { return m_extremiumSlipSideways; } set { m_extremiumSlipSideways = value; } }

	[SerializeField]
	private float m_extremiumValueSideways;
	public float ExtremiumValueSideways { get { return m_extremiumValueSideways; } set { m_extremiumValueSideways = value; } }

	[SerializeField]
	private float m_asymptoteSlipSideways;
	public float AsymptoteSlipSideways { get { return m_asymptoteSlipSideways; } set { m_asymptoteSlipSideways = value; } }

	[SerializeField]
	private float m_asymptoteValueSideways;
	public float AsymptoteValueSideways { get { return m_asymptoteValueSideways; } set { m_asymptoteValueSideways = value; } }



	[SerializeField] 
	private GameObject m_model;
	public GameObject Model { get { return m_model; } }
}