﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axle : MonoBehaviour
{
    [SerializeField]
    private Vehicle m_owner;
	//private VehicleInput m_vehicleInput;
	[SerializeField] private bool m_steeringEnabled = false;
	public bool SteeringEnabled {get{return m_steeringEnabled;}}
	[SerializeField] private bool m_brakingEnabled = false;
	public bool BrakingEnabled {get{return m_brakingEnabled;}}
	[SerializeField] private bool m_eBrakingEnabled = false;
	public bool EBrakingEnabled {get{return m_eBrakingEnabled;}}
	[SerializeField] private bool m_driveEnabled = false;
	public bool DriveEnabled {get{return m_driveEnabled;}}

    [SerializeField]
    private float m_antiSwayBarForce = 5000f;

	[SerializeField] private List<Wheel> m_wheels;
	public List<Wheel> Wheels {get{return m_wheels;}}

    public void Init(VehicleSetUpData vehicleSetUpData, AxleData axleData, List<Wheel> wheels, Vehicle owner)
	{
        m_owner = owner;
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

	private void FixedUpdate()
	{
		UpdateDifferential();
		UpdateAntiRollBar();
	}

	private void UpdateDifferential()
	{
        /*foreach (Wheel tempWheel in m_wheels)
		{
			tempWheel.OnUpdate();
		}*/
        float rpmDiffHigh = 200f;//rpm difference that causes to fully load one wheel over other

        WheelCollider wheelL = m_wheels[0].Collider;
        WheelCollider wheelR = m_wheels[1].Collider;

        float rpmAvg = (wheelL.rpm + wheelR.rpm) * 0.5f;


        float wheelLRPMDiff = wheelL.rpm - rpmAvg;
        float torqueL = (wheelLRPMDiff / rpmDiffHigh) * -m_wheels[0].MaxEngineTorque;
        m_wheels[0].SetDifferentialForce(torqueL);

        float wheelRRPMDiff = wheelR.rpm - rpmAvg;
        float torqueR = (wheelRRPMDiff / rpmDiffHigh) * -m_wheels[1].MaxEngineTorque;
        m_wheels[1].SetDifferentialForce(torqueR);
    }

	private void UpdateAntiRollBar()
	{
        WheelHit hit = new WheelHit();
        float travelL = 1.0f;
        float travelR = 1.0f;

        WheelCollider wheelL = m_wheels[0].Collider;
        WheelCollider wheelR = m_wheels[1].Collider;

        bool groundedL = wheelL.GetGroundHit(out hit);

        if (groundedL)
        {
            travelL = (-wheelL.transform.InverseTransformPoint(hit.point).y - wheelL.radius) / wheelL.suspensionDistance;
        }

        bool groundedR = wheelR.GetGroundHit(out hit);

        if (groundedR)
        {
            travelR = (-wheelR.transform.InverseTransformPoint(hit.point).y - wheelR.radius) / wheelR.suspensionDistance;
        }

        float antiRollForce = (travelL - travelR) * m_antiSwayBarForce;

        if (groundedL)
            m_owner.Rigidbody.AddForceAtPosition(wheelL.transform.up * -antiRollForce,
                wheelL.transform.position);
        if (groundedR)
            m_owner.Rigidbody.AddForceAtPosition(wheelR.transform.up * antiRollForce,
                wheelR.transform.position);
                                            
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
