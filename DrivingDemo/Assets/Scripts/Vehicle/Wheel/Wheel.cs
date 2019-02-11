using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Wheel : MonoBehaviour
{
    [SerializeField]
    private Vehicle m_owner;

    //[SerializeField] private WheelCollider m_collider;
    //[SerializeField] private WheelCollider m_sphereCollider;
    //public WheelCollider Collider { get { return m_collider; } }
    [SerializeField]
    private float m_radius;
    public float Radius { get { return m_radius; } }
    [SerializeField]
    private float m_suspensionDistance;
    public float SuspensionDistance {  get { return m_suspensionDistance; } }

    public float Rpm { get { return 0f; } }

    [SerializeField]
    private float m_wheelAcceleration = 1f;
    [SerializeField]
    private float m_springAcceleration = 5f;



    [SerializeField]
    private LayerMask m_wheelLayerMask;

    private WheelHit m_wheelHit;

    [SerializeField] private GameObject m_model;

	[SerializeField] private float m_maxWheelAngle = 0;
	[SerializeField] private float m_maxBrakingTorque = 0;
	[SerializeField] private float m_handBrakeTorque = 0;
	[SerializeField] private float m_maxEngineTorque = 0;//TODO shouldnt be down here, should be sent via engine/drive
    public float MaxEngineTorque { get { return m_maxEngineTorque; } }

    private float m_accelerationInput = 0f;
    private float m_differentialForce = 0f;

    


    public void ApplySetupData(VehicleSetUpData vehicleSetUpData, AxleData axleData)
	{
		//m_vehicleInput = vehicleInput;

		//VehicleData
		m_maxBrakingTorque = vehicleSetUpData.MaxBrakingTorque;
		m_handBrakeTorque = vehicleSetUpData.HandBrakeTorque;
		m_maxEngineTorque = vehicleSetUpData.MaxEngineTorque;

		//WheelData

		m_maxWheelAngle = axleData.MaxSteerinAngle;

        #region WheelColliderSetup
        //Properties
        /*m_collider.mass = axleData.WheelData.WheelMass;
		m_collider.radius = axleData.WheelData.WheelRadius;
		m_collider.wheelDampingRate = axleData.WheelData.WheelDampingRate;
		m_collider.suspensionDistance = axleData.WheelData.SuspensionDistance;
		m_collider.forceAppPointDistance = axleData.WheelData.ForceAppPointDistance;
		m_collider.center = axleData.WheelData.Center;*/

        //Spring
        /*JointSpring tempSpring = m_collider.suspensionSpring;
		tempSpring.spring = axleData.WheelData.Spring;
		tempSpring.damper = axleData.WheelData.Damper;
		tempSpring.targetPosition = axleData.WheelData.TargetPosition;
		m_collider.suspensionSpring = tempSpring;*/

        //forward
        /*WheelFrictionCurve forwardFriction = m_collider.forwardFriction;
		forwardFriction.extremumSlip = axleData.WheelData.ExtremiumSlipForward;
		forwardFriction.extremumValue = axleData.WheelData.ExtremiumValueForward;
		forwardFriction.asymptoteSlip = axleData.WheelData.AsymptoteSlipForward;
		forwardFriction.asymptoteValue = axleData.WheelData.AsymptoteValueForward;
		m_collider.forwardFriction = forwardFriction;*/

        //Sideways
        /*WheelFrictionCurve sidewaysFriction = m_collider.sidewaysFriction;
		sidewaysFriction.extremumSlip = axleData.WheelData.ExtremiumSlipSideways;
		sidewaysFriction.extremumValue = axleData.WheelData.ExtremiumValueSideways;
		sidewaysFriction.asymptoteSlip = axleData.WheelData.AsymptoteSlipSideways;
		sidewaysFriction.asymptoteValue = axleData.WheelData.AsymptoteValueSideways;
		m_collider.sidewaysFriction = sidewaysFriction;*/
        #endregion

        #region RaycastSetup
        m_radius = axleData.WheelData.WheelRadius;
        #endregion
        //VIEW
        m_model.transform.localScale = Vector3.one * axleData.WheelData.WheelRadius;
		UpdateModel();
	}

	public void Update()
	{
		/*WheelFrictionCurve forwardFriction = m_collider.forwardFriction;
		//forwardFriction.stiffness = 0.8f;
		m_collider.forwardFriction = forwardFriction;*/
		
		/*WheelFrictionCurve sidewaysFriction = m_collider.sidewaysFriction;
		//sidewaysFriction.stiffness = 0.8f;
		m_collider.sidewaysFriction = sidewaysFriction;*/
		
		UpdateModel();
	}

    public void FixedUpdate()
    {
        //m_collider.motorTorque = (m_maxEngineTorque * m_accelerationInput) + m_differentialForce;

        Ray floorCast = new Ray(transform.position, -transform.up);
        RaycastHit raycastHit;

        
        if (Physics.Raycast(floorCast, out raycastHit, m_radius, m_wheelLayerMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log("FLOOR HIT");

            m_wheelHit.collider = raycastHit.collider;
            m_wheelHit.normal = raycastHit.normal;

            float normalisedHitDistance = (raycastHit.distance / m_radius);

            m_owner.Rigidbody.AddForceAtPosition(transform.up * (1-normalisedHitDistance) * m_springAcceleration, transform.position, ForceMode.Acceleration);
        }
        else
        {
            m_wheelHit.collider = null;
            m_wheelHit.normal = -transform.up;
        }

        m_wheelHit.point = raycastHit.point;
        m_wheelHit.forwardDir = transform.forward;
        m_wheelHit.sidewaysDir = transform.right;
        m_wheelHit.sidewaysSlip = 0f;
        m_wheelHit.forwardSlip = 0f;
        m_wheelHit.force = 0f;

        m_owner.Rigidbody.AddForceAtPosition(transform.forward * m_wheelAcceleration, transform.position, ForceMode.Acceleration);
    }

    public void Steer(float steeringInput)
	{
        //m_collider.steerAngle = steeringInput * m_maxWheelAngle;
        transform.localRotation = Quaternion.identity;
        transform.Rotate(new Vector3(0f, steeringInput * m_maxWheelAngle),Space.Self);
	}

	public void Brake(float brakeInput, bool eBrakeInput)
	{
		//Setup
		//m_collider.brakeTorque = 0;

		//Braking
		//m_collider.brakeTorque += m_maxBrakingTorque * brakeInput;

		if (eBrakeInput)
		{
			//m_collider.brakeTorque += m_handBrakeTorque;
		}
	}

	public void Drive(float accelerationInput)
	{
        //Acceleration
        m_accelerationInput = accelerationInput;
    }

    public void SetDifferentialForce(float diffForce)
    {
        m_differentialForce = diffForce;
    }

	private void UpdateModel()
	{
		//Positioning
		/*Vector3 tWheelPos;
		Quaternion tWheelRotation;
		m_collider.GetWorldPose(out tWheelPos, out tWheelRotation);
		m_model.transform.position = tWheelPos;
		m_model.transform.rotation = tWheelRotation;*///TODO should be replaced in the view class
	}

    public bool GetGroundHit(out WheelHit wheelHit)
    {
        wheelHit = m_wheelHit;
        return m_wheelHit.collider != null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.right, m_radius);
    }
#endif
}
