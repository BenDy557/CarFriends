using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelView : View
{
    [SerializeField]
    private Wheel m_wheel;
    [SerializeField]
    private WheelColliderSource m_wheelCollider;
    [SerializeField]
    private ParticleSystem m_fwdSlipEmitterPrefab;
    [SerializeField]
    private float m_fwdSlipMaxEmissionRate = 50;
    [SerializeField]
    private float m_fwdSlipMaxSpeed = 10;

    private ParticleSystem m_fwdSlipEmitter;


    private float m_wheelRotation = 0f;

    public void Init(VehicleSetUpData setupData, AxleData axleData)
    {
        m_fwdSlipEmitterPrefab = axleData.WheelData.FwdSlipEmitterPrefab;
    }

    private void Start()
    {
        m_fwdSlipEmitter = Instantiate(m_fwdSlipEmitterPrefab, transform);
    }

    private void Update()
    {
        WheelHitSource tempWheelHit;
        if (m_wheel.Collider.GetGroundHit(out tempWheelHit))
        {
            m_fwdSlipEmitter.transform.position = tempWheelHit.Point + (Vector3.up * 0.3f);//TODO//magic numbers
            m_fwdSlipEmitter.transform.forward = tempWheelHit.ForwardDir * (tempWheelHit.ForwardSlip < 0 ? 1 : -1);

            ParticleSystem.MainModule tempMainModule = m_fwdSlipEmitter.main;
            tempMainModule.startSpeed = Mathf.Abs(tempWheelHit.ForwardSlip) * m_fwdSlipMaxSpeed;

            ParticleSystem.EmissionModule tempEmissionModule = m_fwdSlipEmitter.emission;
            tempEmissionModule.enabled = true;
            tempEmissionModule.rateOverTime = Mathf.Abs(tempWheelHit.ForwardSlip) * m_fwdSlipMaxEmissionRate;

            //tempEmissionModule.rateOverTime = tempWheelHit.sidewaysSlip;
        }
        else
        {
            ParticleSystem.EmissionModule tempEmissionModule = m_fwdSlipEmitter.emission;
            tempEmissionModule.enabled = false;
        }


        //transform.localEulerAngles = m_wheelCollider.SteerAngle
        m_wheelRotation += m_wheelCollider.RPM * Time.deltaTime;

        transform.localRotation = Quaternion.AngleAxis(m_wheelCollider.SteerAngle, Vector3.up);
        transform.Rotate(Vector3.right, m_wheelRotation);

    }
}
