using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleInfoFinder : MonoBehaviour
{
    [SerializeField]
    private Vehicle m_vehicle;

    private List<float> m_speeds;
    private List<float> m_accelerations;


    private float m_averageSpeed;
    private float m_averageAcceleration;

    private float m_prevSpeed;

    void FixedUpdate ()
    {
        float speedDiff = m_vehicle.Rigidbody.velocity.magnitude - m_prevSpeed;


        //Debug.Log("KPH: " + Mathf.RoundToInt((m_vehicle.Engine.Speed * 60f * 60f) * 0.001f) + " M/s^2: " + Mathf.RoundToInt((speedDiff/Time.fixedDeltaTime)*100)/100f);

        m_prevSpeed = m_vehicle.Rigidbody.velocity.magnitude;
    }
}
