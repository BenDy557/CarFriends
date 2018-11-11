using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelView : View
{
    private Wheel m_wheel;

    [SerializeField]
    private ParticleSystem m_fwdSlipEmitterPrefab;
    [SerializeField]
    private float m_fwdSlipMaxEmissionRate = 50;
    [SerializeField]
    private float m_fwdSlipMaxSpeed = 10;

    private ParticleSystem m_fwdSlipEmitter;

    public void Init(VehicleSetUpData setupData, AxleData axleData)
    {
        m_fwdSlipEmitterPrefab = axleData.WheelData.FwdSlipEmitterPrefab;
    }

    private void Start()
    {
        m_fwdSlipEmitter = Instantiate(m_fwdSlipEmitterPrefab, transform);

        m_wheel = GetComponent<Wheel>();
    }

    private void Update()
    {
        WheelHit tempWheelHit;
        if (m_wheel.Collider.GetGroundHit(out tempWheelHit))
        {
            m_fwdSlipEmitter.transform.position = tempWheelHit.point + (Vector3.up * 0.3f);//TODO//magic numbers
            m_fwdSlipEmitter.transform.forward = tempWheelHit.forwardDir * (tempWheelHit.forwardSlip < 0 ? 1 : -1);

            ParticleSystem.MainModule tempMainModule = m_fwdSlipEmitter.main;
            tempMainModule.startSpeed = Mathf.Abs(tempWheelHit.forwardSlip) * m_fwdSlipMaxSpeed;

            ParticleSystem.EmissionModule tempEmissionModule = m_fwdSlipEmitter.emission;
            tempEmissionModule.enabled = true;
            tempEmissionModule.rateOverTime = Mathf.Abs(tempWheelHit.forwardSlip) * m_fwdSlipMaxEmissionRate;

            //tempEmissionModule.rateOverTime = tempWheelHit.sidewaysSlip;
        }
        else
        {
            ParticleSystem.EmissionModule tempEmissionModule = m_fwdSlipEmitter.emission;
            tempEmissionModule.enabled = false;
        }

    }
}
