using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Wheel : MonoBehaviour
{
    [SerializeField]
    private Vehicle m_owner;

    [SerializeField] private WheelColliderSource m_collider;
    //[SerializeField] private WheelCollider m_sphereCollider;
    public WheelColliderSource Collider { get { return m_collider; } }
    [SerializeField]
    private float m_radius;
    public float Radius { get { return m_radius; } }
    [SerializeField]
    private float m_suspensionDistance;
    public float SuspensionDistance { get { return m_suspensionDistance; } }

    public float Rpm { get { return 0f; } }

    [SerializeField, Range(0, 1)]
    private float m_wheelBounciness = 0.5f;


    [SerializeField]
    private float m_wheelAcceleration = 1f;
    [SerializeField]
    private float m_springAcceleration = 5f;



    [SerializeField]
    private LayerMask m_wheelLayerMask;

    //private WheelHit m_wheelHit;

    [SerializeField] private GameObject m_model;

    [SerializeField] private float m_maxWheelAngle = 0;
    [SerializeField] private float m_maxBrakingTorque = 0;
    [SerializeField] private float m_handBrakeTorque = 0;
    [SerializeField] private float m_maxEngineTorque = 0;//TODO shouldnt be down here, should be sent via engine/drive
    public float MaxEngineTorque { get { return m_maxEngineTorque; } }

    private float m_accelerationInput = 0f;
    private float m_differentialForce = 0f;

    //Debug
    [SerializeField]
    private AnimationCurve m_forwardSlipCurve;
    [SerializeField]
    private AnimationCurve m_lateralSlipCurve;
    private float m_slipIntervals = 0.05f;

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
        m_collider.Mass = axleData.WheelData.WheelMass;
        m_collider.WheelRadius = axleData.WheelData.WheelRadius;
        m_collider.SuspensionDistance = axleData.WheelData.SuspensionDistance;
        //m_collider.forceAppPointDistance = axleData.WheelData.ForceAppPointDistance;
        //m_collider.Center = axleData.WheelData.Center;

        //Spring
        JointSpringSource tempSpring = m_collider.SuspensionSpring;
        tempSpring.Spring = axleData.WheelData.Spring;
        tempSpring.Damper = axleData.WheelData.Damper;
        tempSpring.TargetPosition = axleData.WheelData.TargetPosition;
        m_collider.SuspensionSpring = tempSpring;

        m_collider.UseSimpleFrictionCurve = axleData.WheelData.UseSimpleFrictionCurve;

        //forward
        m_collider.SimpleForwardFriction = axleData.WheelData.ForwardFrictionCurve;
        WheelFrictionCurveSource forwardFriction = m_collider.ForwardFriction;
        forwardFriction.ExtremumSlip = axleData.WheelData.ExtremiumSlipForward;
        forwardFriction.ExtremumValue = axleData.WheelData.ExtremiumValueForward;
        forwardFriction.AsymptoteSlip = axleData.WheelData.AsymptoteSlipForward;
        forwardFriction.AsymptoteValue = axleData.WheelData.AsymptoteValueForward;
        m_collider.ForwardFriction = forwardFriction;

        //Sideways
        m_collider.SimpleLateralFriction = axleData.WheelData.LateralFrictionCurve;
        WheelFrictionCurveSource sidewaysFriction = m_collider.SidewaysFriction;
        sidewaysFriction.ExtremumSlip = axleData.WheelData.ExtremiumSlipSideways;
        sidewaysFriction.ExtremumValue = axleData.WheelData.ExtremiumValueSideways;
        sidewaysFriction.AsymptoteSlip = axleData.WheelData.AsymptoteSlipSideways;
        sidewaysFriction.AsymptoteValue = axleData.WheelData.AsymptoteValueSideways;
        m_collider.SidewaysFriction = sidewaysFriction;

        #endregion

        #region RaycastSetup
        m_radius = axleData.WheelData.WheelRadius;
        #endregion
        //VIEW
        m_model.transform.localScale = Vector3.one * axleData.WheelData.WheelRadius;
        UpdateModel();

        PopulateFrictionCurves();
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
        //m_collider.MotorTorque = (m_maxEngineTorque * m_accelerationInput) + m_differentialForce;
        /*Ray floorCast = new Ray(transform.position, -transform.up);
        RaycastHit raycastHit;

        Debug.Log("relativeVelocity" + m_owner.Rigidbody.GetRelativePointVelocity(transform.localPosition).magnitude);
        
        if (Physics.Raycast(floorCast, out raycastHit, m_radius, m_wheelLayerMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log("FLOOR HIT");

            m_wheelHit.collider = raycastHit.collider;
            m_wheelHit.normal = raycastHit.normal;

            float normalisedHitDistance = (raycastHit.distance / m_radius);

            //m_owner.Rigidbody.AddForceAtPosition(transform.up * (1-normalisedHitDistance) * m_springAcceleration, transform.position, ForceMode.Acceleration);
            //m_owner.Rigidbody.AddForceAtPosition(raycastHit.normal.normalized * m_owner.Rigidbody.GetRelativePointVelocity(transform.localPosition).magnitude , transform.position, ForceMode.Acceleration);
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


        float wheelForce = m_maxEngineTorque * m_accelerationInput;
        m_owner.Rigidbody.AddForceAtPosition(transform.forward * wheelForce, transform.position, ForceMode.Force);*/
    }

    public void Steer(float steeringInput)
    {
        m_collider.SteerAngle = steeringInput * m_maxWheelAngle;
        //transform.localRotation = Quaternion.identity;
        //transform.Rotate(new Vector3(0f, steeringInput * m_maxWheelAngle),Space.Self);
    }

    public void Brake(float brakeInput, bool eBrakeInput)
    {
        //Setup
        m_collider.BrakeTorque = 0;

        //Braking
        m_collider.BrakeTorque += m_maxBrakingTorque * brakeInput;

        if (eBrakeInput)
        {
            m_collider.BrakeTorque += m_handBrakeTorque;
        }
    }

    public void Drive(float accelerationInput)
    {
        //Acceleration
        m_accelerationInput = accelerationInput;
        m_collider.MotorTorque = (m_maxEngineTorque * m_accelerationInput) + m_differentialForce;
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

    /*public bool GetGroundHit(out WheelHit wheelHit)
    {
        wheelHit = m_wheelHit;
        return m_wheelHit.collider != null;
    }*/

    private void PopulateFrictionCurves()
    {
        m_forwardSlipCurve = new AnimationCurve();
        float fwdLimit = m_collider.ForwardFriction.AsymptoteSlip;
        for (float i = -fwdLimit; i < fwdLimit; i += m_slipIntervals)
        {
            m_forwardSlipCurve.AddKey(i, m_collider.ForwardFriction.Evaluate(i));
        }

        m_lateralSlipCurve = new AnimationCurve();
        float latLimit = m_collider.SidewaysFriction.AsymptoteSlip;
        for (float i = -latLimit; i < latLimit; i += m_slipIntervals)
        {
            m_lateralSlipCurve.AddKey(i, m_collider.SidewaysFriction.Evaluate(i));
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //UnityEditor.Handles.color = Color.cyan;
        //UnityEditor.Handles.DrawWireDisc(transform.position, transform.right, m_radius);
    }
#endif
}
