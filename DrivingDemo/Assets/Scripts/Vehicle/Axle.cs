using System.Collections;
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

    public void ApplySetupData(VehicleSetUpData setupData, AxleData axleData)
    {
		m_steeringEnabled = axleData.SteeringEnabled;
		m_brakingEnabled = axleData.BrakingEnabled;
		m_eBrakingEnabled = axleData.EBrakingEnabled;
		m_driveEnabled = axleData.DriveEnabled;

		//m_vehicleInput = vehicleInput;
		foreach (Wheel wheel in m_wheels)
		{
            wheel.ApplySetupData(setupData, axleData);
		}
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

        Wheel wheelL = m_wheels[0];
        Wheel wheelR = m_wheels[1];

        float rpmAvg = (wheelL.Rpm + wheelR.Rpm) * 0.5f;


        float wheelLRPMDiff = wheelL.Rpm - rpmAvg;
        float torqueL = (wheelLRPMDiff / rpmDiffHigh) * -m_wheels[0].MaxEngineTorque;
        m_wheels[0].SetDifferentialForce(torqueL);

        float wheelRRPMDiff = wheelR.Rpm - rpmAvg;
        float torqueR = (wheelRRPMDiff / rpmDiffHigh) * -m_wheels[1].MaxEngineTorque;
        m_wheels[1].SetDifferentialForce(torqueR);
    }

	private void UpdateAntiRollBar()
	{
        WheelHit hit = new WheelHit();
        float travelL = 1.0f;
        float travelR = 1.0f;

        Wheel wheelL = m_wheels[0];
        Wheel wheelR = m_wheels[1];

        bool groundedL = wheelL.GetGroundHit(out hit);

        if (groundedL)
        {
            travelL = (-wheelL.transform.InverseTransformPoint(hit.point).y - wheelL.Radius) / wheelL.SuspensionDistance;
        }

        bool groundedR = wheelR.GetGroundHit(out hit);

        if (groundedR)
        {
            travelR = (-wheelR.transform.InverseTransformPoint(hit.point).y - wheelR.Radius) / wheelR.SuspensionDistance;
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
