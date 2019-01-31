using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

[CreateAssetMenu(fileName = "VehicleData", menuName = "Vehicle", order = 1)]
public class VehicleSetUpData : ScriptableObject
{
	[SerializeField]
	private string m_presetName;
	public string PresetName { get { return m_presetName; } }

	[SerializeField] private float m_mass;
	public float Mass { get { return m_mass; } }

	[Header("EngineProperties")]
	[SerializeField] private float m_maxBrakingTorque;
	public float MaxBrakingTorque{ get { return m_maxBrakingTorque; } }
	[SerializeField] private float m_HandBrakeTorque;
	public float HandBrakeTorque{ get { return m_HandBrakeTorque; } }
	[SerializeField] private float m_maxEngineTorque;
	public float MaxEngineTorque{ get { return m_maxEngineTorque; } }

    /*[SerializeField, NaughtyAttributes.BoxGroup("Axles")]
	private List<AxleData> m_axles;
	public IList<AxleData> Axles { get { return m_axles.AsReadOnly(); } }*/

    [SerializeField, NaughtyAttributes.BoxGroup("Axles")]
    private AxleData m_frontAxle;
    public AxleData FrontAxle { get { return m_frontAxle; } }
    [SerializeField, NaughtyAttributes.BoxGroup("Axles")]
    private AxleData m_rearAxle;
    public AxleData RearAxle { get { return m_rearAxle; } }
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

    [SerializeField]
    private WheelSetupData m_wheelSetupData;
    private bool HasSetupData()
    {
        return m_wheelSetupData != null;
    }

    //[SerializeField, NaughtyAttributes.BoxGroup("Wheels"), NaughtyAttributes.HideIfAttribute("HasSetupData")]
	//private WheelData m_wheelData;
	public WheelData WheelData
    {
        get
        {
            //return HasSetupData() ? m_wheelSetupData.WheelData : m_wheelData;
            return m_wheelSetupData.WheelData;
        }
    }

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

    [SerializeField]
    private ParticleSystem m_fwdSlipEmitterPrefab;
    public ParticleSystem FwdSlipEmitterPrefab { get { return m_fwdSlipEmitterPrefab; } }
    [SerializeField]
    private ParticleSystem m_sideSlipEmitterPrefab;
    public ParticleSystem SideSlipEmitterPrefab { get { return m_sideSlipEmitterPrefab; } }
}