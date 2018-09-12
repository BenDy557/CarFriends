using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axle : MonoBehaviour
{
	//private VehicleInput m_vehicleInput;
	[SerializeField] private bool m_steeringEnabled = false;
	public bool SteeringEnabled {get{return m_steeringEnabled;}}
	[SerializeField] private bool m_brakingEnabled = false;
	public bool BrakingEnabled {get{return m_brakingEnabled;}}
	[SerializeField] private bool m_eBrakingEnabled = false;
	public bool EBrakingEnabled {get{return m_eBrakingEnabled;}}
	[SerializeField] private bool m_driveEnabled = false;
	public bool DriveEnabled {get{return m_driveEnabled;}}

	[SerializeField] private List<Wheel> m_wheels;
	public List<Wheel> Wheels {get{return m_wheels;}}

	public void Init(VehicleSetUpData vehicleSetUpData, AxleData axleData, List<Wheel> wheels)
	{
		m_steeringEnabled = axleData.SteeringEnabled;
		m_brakingEnabled = axleData.BrakingEnabled;
		m_eBrakingEnabled = axleData.EBrakingEnabled;
		m_driveEnabled = axleData.DriveEnabled;

		m_wheels = wheels;

		//m_vehicleInput = vehicleInput;
		/*foreach (WheelData wheelData in axleData.Wheels)
		{
			wheelData.Init(vehicleSetUpData,wheelData);
		}*/
	}

	private void Update()
	{
		UpdateWheels();
		UpdateDifferential();
		UpdateAntiRollBar();
	}


	private void UpdateWheels()
	{
		foreach (Wheel tempWheel in m_wheels)
		{

		}
	}

	private void UpdateDifferential()
	{
		/*foreach (Wheel tempWheel in m_wheels)
		{
			tempWheel.OnUpdate();
		}*/
	}

	private void UpdateAntiRollBar()
	{

	}

	public void Drive(float accelerationAmount)
	{
		if (!m_driveEnabled)
			return;

		foreach (Wheel wheel in m_wheels)
		{
			wheel.Drive(accelerationAmount);
		}
	}

	public void Steer(float steeringAmount, bool inverse = false)
	{
		if (!m_steeringEnabled)
			return;

		foreach (Wheel wheel in m_wheels)
		{
			wheel.Steer(inverse ? -steeringAmount : steeringAmount);
		}
	}

	public void Brake(float brakeAmount,bool eBrakeInput)
	{
		if (!m_brakingEnabled)
			return;

		foreach (Wheel wheel in m_wheels)
		{
			wheel.Brake(brakeAmount,m_eBrakingEnabled && eBrakeInput);
		}
	}

}
