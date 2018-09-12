using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Wheel : MonoBehaviour
{
	//private VehicleInput m_vehicleInput;

	[SerializeField] private WheelCollider m_collider;
	public WheelCollider Collider { get { return m_collider; } }

	[SerializeField] private GameObject m_model;

	[SerializeField] private float m_maxWheelAngle = 0;
	[SerializeField] private float m_maxBrakingTorque = 0;
	[SerializeField] private float m_handBrakeTorque = 0;
	[SerializeField] private float m_maxEngineTorque = 0;

	public void Init(VehicleSetUpData vehicleSetUpData, AxleData axleData)
	{
		//m_vehicleInput = vehicleInput;

		//VehicleData
		m_maxBrakingTorque = vehicleSetUpData.MaxBrakingTorque;
		m_handBrakeTorque = vehicleSetUpData.HandBrakeTorque;
		m_maxEngineTorque = vehicleSetUpData.MaxEngineTorque;

		//WheelData

		m_maxWheelAngle = axleData.MaxSteerinAngle;

		m_collider = gameObject.AddComponent<WheelCollider>();
		
		//Protperties
		m_collider.mass = axleData.WheelData.WheelMass;
		m_collider.radius = axleData.WheelData.WheelRadius;
		m_collider.wheelDampingRate = axleData.WheelData.WheelDampingRate;
		m_collider.suspensionDistance = axleData.WheelData.SuspensionDistance;
		m_collider.forceAppPointDistance = axleData.WheelData.ForceAppPointDistance;
		m_collider.center = axleData.WheelData.Center;

		//Spring
		JointSpring tempSpring = m_collider.suspensionSpring;
		tempSpring.spring = axleData.WheelData.Spring;
		tempSpring.damper = axleData.WheelData.Damper;
		tempSpring.targetPosition = axleData.WheelData.TargetPosition;
		m_collider.suspensionSpring = tempSpring;

		//forward
		WheelFrictionCurve forwardFriction = m_collider.forwardFriction;
		forwardFriction.extremumSlip = axleData.WheelData.ExtremiumSlipForward;
		forwardFriction.extremumValue = axleData.WheelData.ExtremiumValueForward;
		forwardFriction.asymptoteSlip = axleData.WheelData.AsymptoteSlipForward;
		forwardFriction.asymptoteValue = axleData.WheelData.AsymptoteValueForward;
		m_collider.forwardFriction = forwardFriction;
		
		//Sideways
		WheelFrictionCurve sidewaysFriction = m_collider.sidewaysFriction;
		sidewaysFriction.extremumSlip = axleData.WheelData.ExtremiumSlipSideways;
		sidewaysFriction.extremumValue = axleData.WheelData.ExtremiumValueSideways;
		sidewaysFriction.asymptoteSlip = axleData.WheelData.AsymptoteSlipSideways;
		sidewaysFriction.asymptoteValue = axleData.WheelData.AsymptoteValueSideways;
		m_collider.sidewaysFriction = sidewaysFriction;

		//VIEW
		m_model = Instantiate<GameObject>(axleData.WheelData.Model,Vector3.zero,Quaternion.identity,transform);
		m_model.transform.localScale = Vector3.one * axleData.WheelData.WheelRadius;
		UpdateModel();
	}

	public void Update()
	{
		WheelFrictionCurve forwardFriction = m_collider.forwardFriction;
		forwardFriction.stiffness = 0.8f;
		m_collider.forwardFriction = forwardFriction;
		
		WheelFrictionCurve sidewaysFriction = m_collider.sidewaysFriction;
		sidewaysFriction.stiffness = 0.8f;
		m_collider.sidewaysFriction = sidewaysFriction;
		
		UpdateModel();
	}

	public void Steer(float steeringInput)
	{
		m_collider.steerAngle = steeringInput * m_maxWheelAngle;
	}

	public void Brake(float brakeInput, bool eBrakeInput)
	{
		//Setup
		m_collider.brakeTorque = 0;

		//Braking
		m_collider.brakeTorque += m_maxBrakingTorque * brakeInput;

		if (eBrakeInput)
		{
			m_collider.brakeTorque += m_handBrakeTorque;
		}
	}

	public void Drive(float accelerationInput)
	{
		

		//Acceleration
		m_collider.motorTorque = m_maxEngineTorque * accelerationInput;
		
		//Decceleration/Reverse
		//mCollider.motorTorque -= mMaxWheelTorque * mDeccelerationInput;
	}

	private void UpdateModel()
	{
		//Positioning
		Vector3 tWheelPos;
		Quaternion tWheelRotation;
		m_collider.GetWorldPose(out tWheelPos, out tWheelRotation);
		m_model.transform.position = tWheelPos;
		m_model.transform.rotation = tWheelRotation;//TODO should be replaced in the view class
	}

}
