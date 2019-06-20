/******************************************
 * WheelColliderSource
 *  
 * This class was created by:
 * 
 * Nic Tolentino.
 * rotatingcube@gmail.com
 * 
 * I take no liability for it's use and is provided as is.
 * 
 * The classes are based off the original code developed by Unity Technologies.
 * 
 * You are free to use, modify or distribute these classes however you wish, 
 * I only ask that you make mention of their use in your project credits.
*/
using UnityEngine;
using System.Collections;

public class WheelColliderSource : MonoBehaviour
{
    [SerializeField]
    private Transform m_wheelParent;
    [SerializeField]
    private Rigidbody m_rigidbody;

    //private Vector3 m_wheelPosition;
    //private Vector3 m_wheelRotation;


    private SphereCollider m_collider;

    [SerializeField]
    private AnimationCurve m_simpleForwardFriction;
    [SerializeField]
    private AnimationCurve m_simpleLateralFriction;

    [SerializeField]
    private bool m_useForceLimit = false;
    
    [SerializeField, NaughtyAttributes.ShowIf("UsingForceLimit")]
    private AnimationCurve m_wheelForceLimitCurve;

    //private float m_forwardSlip;
    private float m_sidewaysSlip;
    private Vector3 m_springForce;
    private Vector3 m_wheelForce;
    private Vector3 m_center; //The center of the wheel, measured in the object's local space.
    private Vector3 m_prevPosition;
    private bool m_isGrounded; //Indicates whether the wheel currently collides with something (Read Only).

    private float m_wheelMotorTorque; //Motor torque on the wheel axle. Positive or negative depending on direction.
    private float m_wheelBrakeTorque; //Brake torque. Must be positive.
    private float m_wheelSteerAngle; //Steering angle in degrees, always around the local y-axis.
    private float m_wheelAngularVelocity; //Current wheel axle rotation speed, in rotations per minute (Read Only).
    private float m_wheelRotationAngle;
    [SerializeField]
    private float m_radius; //The radius of the wheel, measured in local space.
    [SerializeField]
    private float m_mass; //The mass of the wheel. Must be larger than zero.

    private RaycastHit m_raycastHit;
    [SerializeField]
    private float m_suspensionDistance; //Maximum extension distance of wheel suspension, measured in local space.
    private float m_suspensionCompression;
    private float m_suspensionCompressionPrev;
    [SerializeField]
    private JointSpringSource m_suspensionSpring; //The parameters of wheel's suspension. The suspension attempts to reach a target position

    private WheelHitSource m_wheelHit;

    //Debugging 
    private Color GizmoColor = Color.cyan;

    //Standard accessor and mutator properties
    public Vector3 Center
    {
        set
        {
            m_center = value;
            m_wheelParent.localPosition = transform.localPosition + m_center;
            m_prevPosition = m_wheelParent.localPosition;
        }
        get
        {
            return m_center;
        }
    }
    public float WheelRadius
    {
        set
        {
            m_radius = value;

            if(m_collider !=null)
                m_collider.center = new Vector3(0, m_radius, 0);
        }
        get
        {
            return m_radius;
        }
    }
    public float SuspensionDistance
    {
        set
        {
            m_suspensionDistance = value;
        }
        get
        {
            return m_suspensionDistance;
        }
    }
    public JointSpringSource SuspensionSpring
    {
        set
        {
            m_suspensionSpring = value;
        }
        get
        {
            return m_suspensionSpring;
        }
    }
    public float Mass
    {
        set
        {
            m_mass = Mathf.Max(value, 0.0001f);
        }
        get
        {
            return m_mass;
        }
    }
    
    public AnimationCurve SimpleForwardFriction
    {
        set
        {
            m_simpleForwardFriction = value;
        }
        get
        {
            return m_simpleForwardFriction;
        }
    }
    public AnimationCurve SimpleLateralFriction
    {
        set
        {
            m_simpleLateralFriction = value;
        }
        get
        {
            return m_simpleLateralFriction;
        }
    }

    public bool UseForceLimitCurve
    {
        set
        {
            m_useForceLimit = value;
        }
        get
        {
            return m_useForceLimit;
        }
    }

    public AnimationCurve WheelForceLimitCurve
    {
        set
        {
            m_wheelForceLimitCurve = value;
        }
        get
        {
            return m_wheelForceLimitCurve;
        }
    }

    public float MotorTorque
    {
        set
        {
            m_wheelMotorTorque = value;
        }
        get
        {
            return m_wheelMotorTorque;
        }
    }
    public float BrakeTorque
    {
        set
        {
            m_wheelBrakeTorque = value;
        }
        get
        {
            return m_wheelBrakeTorque;
        }
    }
    public float SteerAngle
    {
        set
        {
            m_wheelSteerAngle = value;
        }
        get
        {
            return m_wheelSteerAngle;
        }
    }
    public bool IsGrounded
    {
        get
        {
            return m_isGrounded;
        }
    }
    public float RPM
    {
        get
        {
            return m_wheelAngularVelocity;
        }
    }

    // Use this for initialization
    public void Awake()
    {
        m_wheelParent = new GameObject("ParentWheel").transform;
        m_wheelParent.transform.parent = this.transform.parent;
        Center = Vector3.zero;

        m_radius = 0.5f;
        m_suspensionDistance = 0.5f;
        m_suspensionCompression = 0.0f;
        Mass = 1.0f;

        m_suspensionSpring = new JointSpringSource();
        m_suspensionSpring.Spring = 15000.0f;
        m_suspensionSpring.Damper = 1000.0f;
        m_suspensionSpring.TargetPosition = 0.0f;

        if (m_rigidbody == null)
            Debug.LogError("No rigidbody set up",gameObject);
    }

    public void Start()
    {
        //Add a SphereCollider as a work around to the Collider implementation issue.
        //See wiki for more details.
        //http://www.unifycommunity.com/wiki/index.php?title=WheelColliderSource
        m_collider = gameObject.AddComponent<SphereCollider>();
        m_collider.center = new Vector3(0, m_radius, 0);
        m_collider.radius = 0.0f;
    }

    // Called once per physics update
    public void FixedUpdate()
    {
        UpdateSuspension();

        UpdateWheel();

        GetGroundHit(out m_wheelHit);

        if (m_isGrounded)
        {
            CalculateSlips();

            CalculateForcesFromSlips();

            //m_rigidbody.AddForceAtPosition(m_springForce, transform.position);
            m_rigidbody.AddForceAtPosition(m_springForce, m_wheelHit.Point);
            m_rigidbody.AddForceAtPosition(m_wheelForce, m_wheelHit.Point);

        }

        /*if (m_suspensionCompression > 0.8f)
        {
            WheelHitSource wheelHit;
            GetGroundHit(out wheelHit);
            //UnityEditor.EditorApplication.Beep();
        }*/
    }

    public bool GetGroundHit(out WheelHitSource wheelHit)
    {
        wheelHit = new WheelHitSource();
        if (m_isGrounded)
        {
            wheelHit.Collider = m_raycastHit.collider;
            wheelHit.Point = m_raycastHit.point;
            wheelHit.ForwardDir = m_wheelParent.forward;
            wheelHit.SidewaysDir = m_wheelParent.right;
            wheelHit.Normal = m_raycastHit.normal;
            wheelHit.NormalRight = Vector3.Cross(m_raycastHit.normal,m_wheelParent.forward);
            wheelHit.NormalForward = Vector3.Cross(m_raycastHit.normal, wheelHit.NormalRight);
            //wheelHit.Force = m_totalForce;
            //wheelHit.ForwardSlip = m_forwardSlip;
            wheelHit.SidewaysSlip = m_sidewaysSlip;
        }

        return m_isGrounded;
    }

    private void UpdateSuspension()
    {
        //Raycast down along the suspension to find out how far the ground is to the wheel
        bool result = Physics.Raycast(new Ray(m_wheelParent.position, -m_wheelParent.up), out m_raycastHit, m_radius + m_suspensionDistance, LayerManager.Instance.Drivable,QueryTriggerInteraction.Ignore);

        if (result) //The wheel is in contact with the ground
        {
            if (!m_isGrounded) //If not previously grounded, set the prevPosition value to the wheel's current position.
            {
                m_prevPosition = m_wheelParent.position;
            }
            GizmoColor = Color.green;
            m_isGrounded = true;

            //Store the previous suspension compression for the damping calculation
            m_suspensionCompressionPrev = m_suspensionCompression;

            //Update the suspension compression
            m_suspensionCompression = m_suspensionDistance + m_radius - (m_raycastHit.point - m_wheelParent.position).magnitude;

            if (m_suspensionCompression > m_suspensionDistance)
            {
                GizmoColor = Color.red;
            }

            Utils.DrawCross(m_raycastHit.point, Color.magenta);
        }
        else //The wheel is in the air
        {
            m_suspensionCompression = 0;
            GizmoColor = Color.blue;
            m_isGrounded = false;
        }
    }

    private void UpdateWheel()
    {
        //Set steering angle of the wheel dummy
        m_wheelParent.localEulerAngles = new Vector3(0, m_wheelSteerAngle, 0);

        float wheelCircumference = 2f * Mathf.PI * m_radius;
        m_wheelAngularVelocity = (Vector3.Dot(m_rigidbody.velocity, m_wheelParent.forward) * 360f) / wheelCircumference;
        //Debug.Log("VehicleVelocity" + m_rigidbody.velocity);
        //Debug.Log("WheelAngularVelocity" + m_wheelAngularVelocity);
        //Calculate the wheel's rotation given it's angular velocity
        //m_wheelAngularVelocity = 0f;
        //m_wheelRotationAngle += m_wheelAngularVelocity * Time.deltaTime;

        //Set the rotation and steer angle of the wheel model
        //this.transform.localEulerAngles = new Vector3(m_wheelRotationAngle, m_wheelSteerAngle, 0);
        //this.transform.localEulerAngles = new Vector3(0f, m_wheelSteerAngle, 0f);

        //Set the wheel's position given the current suspension compression
        transform.localPosition = m_wheelParent.localPosition - Vector3.up * (m_suspensionDistance - m_suspensionCompression);

        //Debug.Log("angularVelocity1: " + m_wheelAngularVelocity);
        //Apply motor torque
        //float motorVelocityChange = m_wheelMotorTorque / m_radius / m_mass * Time.deltaTime;
        //Debug.Log("angularVelocity2: " + m_wheelAngularVelocity);
        //Apply brake torque
        /*float brakeVelocity = -Mathf.Sign(m_wheelAngularVelocity) * Mathf.Min(Mathf.Abs(m_wheelAngularVelocity), m_wheelBrakeTorque * m_radius / m_mass * Time.deltaTime);
        m_wheelAngularVelocity += brakeVelocity;*/

        //m_wheelAngularVelocity = 0f;
        //m_wheelAngularVelocity = m_wheelRotationAngle + freeRollingVelocityChange + motorVelocityChange + brakeVelocity;
        //Debug.Log("angularVelocity: " + m_wheelAngularVelocity + " " + name);
        //Debug.Log("freeRollingVelocityChange" + freeRollingVelocityChange);
        //Debug.Log("motorVelocityChange" + motorVelocityChange);
        //Debug.Log("brakeVelocity" + brakeVelocity);
    }

    private void CalculateSlips()
    {
        //Calculate the wheel's linear velocity
        //Vector3 velocity = (m_wheelParent.position - m_prevPosition) / Time.deltaTime;
        //m_prevPosition = m_wheelParent.position;

        Vector3 velocity = m_rigidbody.GetPointVelocity(m_wheelParent.position);

        //Store the forward and sideways direction to improve performance
        Vector3 forward = m_wheelParent.forward;
        Vector3 sideways = -m_wheelParent.right;

        //Calculate the forward and sideways velocity components relative to the wheel
        Vector3 forwardVelocity = Vector3.Dot(velocity, forward) * forward;
        Vector3 sidewaysVelocity = Vector3.Dot(velocity, sideways) * sideways;

        //Calculate the slip velocities. 
        //Note that these values are different from the standard slip calculation.
        //m_forwardSlip = -Mathf.Sign(Vector3.Dot(forward, forwardVelocity)) * forwardVelocity.magnitude + (m_wheelAngularVelocity * Mathf.PI / 180.0f * m_radius);
        m_sidewaysSlip = -Mathf.Sign(Vector3.Dot(sideways, sidewaysVelocity)) * sidewaysVelocity.magnitude;
        //Debug.Log("Sideways slip: " + gameObject.name + " " + (Mathf.Round(m_sidewaysSlip * 100)/100f), gameObject);


        //m_forwardSlip = 0f;
        //m_sidewaysSlip = 0f;

        //Debug.Log("ForwardSlips: " + m_forwardSlip);
        //Debug.Log("ForwardFriction: " + m_forwardFriction.Evaluate(m_forwardSlip));

        //Debug.Log("SidewaySlips: " + m_sidewaysSlip);
        //Debug.Log("SidewaysFriction: " + m_sidewaysFriction.Evaluate(m_sidewaysSlip));

    }

    private void CalculateForcesFromSlips()
    {

        //Lateral slip force
        float lateralFrictionValue = m_simpleLateralFriction.Evaluate(m_sidewaysSlip);
        //Vector3 lateralSlipForce = -m_wheelParent.right * Mathf.Sign(m_sidewaysSlip) * lateralFrictionValue;
        Vector3 lateralSlipForce = -m_wheelHit.NormalRight * Mathf.Sign(m_sidewaysSlip) * lateralFrictionValue;
        //lateralSlipForce = Vector3.zero;
        Vector3 fakeForce = m_wheelParent.forward * m_wheelMotorTorque;

        m_wheelForce = fakeForce + lateralSlipForce;

        if (m_useForceLimit)
            m_wheelForce *= m_wheelForceLimitCurve.Evaluate(m_wheelForce.magnitude);
            

        //Spring force
        //Vector3 springForce = m_wheelParent.up * (m_suspensionCompression - m_suspensionDistance * (m_suspensionSpring.TargetPosition)) * m_suspensionSpring.Spring;
        float springMagnitude = (m_suspensionCompression - (m_suspensionDistance * m_suspensionSpring.TargetPosition)) * m_suspensionSpring.Spring;
        float springDampingMagnitude = (m_suspensionCompression - m_suspensionCompressionPrev) / Time.deltaTime * m_suspensionSpring.Damper;

        Vector3 resultantSpringForce = m_wheelHit.Normal * Mathf.Clamp(springMagnitude + springDampingMagnitude, 0f, float.PositiveInfinity);


        m_springForce = /*forwardSlipForce + */ resultantSpringForce;
        
    }

    //naughty attributes
    private bool UsingForceLimit()
    {
        return m_useForceLimit;
    }

#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        //Wheel contact point
        if (m_isGrounded)
        {
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(m_wheelHit.Point, m_wheelHit.Normal, 0.5f);
            Gizmos.color = Color.red.SetA(0.5f);
            Gizmos.DrawLine(m_wheelHit.Point, m_wheelHit.Point + m_wheelHit.NormalRight);
            Gizmos.color = Color.green.SetA(0.5f);
            Gizmos.DrawLine(m_wheelHit.Point, m_wheelHit.Point + m_wheelHit.Normal);
            Gizmos.color = Color.blue.SetA(0.5f);
            Gizmos.DrawLine(m_wheelHit.Point, m_wheelHit.Point + m_wheelHit.NormalForward);
        }

        Gizmos.color = GizmoColor;

        Transform parentTransform;

        if (m_wheelParent == null)
            parentTransform = transform;
        else
            parentTransform = m_wheelParent;

        //Draw the suspension
        Gizmos.DrawLine(transform.position - parentTransform.up * m_radius, transform.position + (parentTransform.up * (m_suspensionDistance - m_suspensionCompression)));

        //Draw the wheel
        Vector3 point1;
        Vector3 point0 = transform.TransformPoint(m_radius * new Vector3(0, Mathf.Sin(0), Mathf.Cos(0)));
        for (int i = 1; i <= 20; ++i)
        {
            point1 = transform.TransformPoint(m_radius * new Vector3(0, Mathf.Sin(i / 20.0f * Mathf.PI * 2.0f), Mathf.Cos(i / 20.0f * Mathf.PI * 2.0f)));
            Gizmos.DrawLine(point0, point1);
            point0 = point1;

        }
        Gizmos.color = Color.white;
    }
#endif
}