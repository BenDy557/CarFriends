using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField]
    private Drive m_vehicle;

    List<ParticleSystem> m_fwdSlipEmitters = new List<ParticleSystem>();

    
    [SerializeField]
    private ParticleSystem m_fwdSlipEmitterPrefab;
    [SerializeField]
    private float m_fwdSlipMaxEmissionRate = 50;
    [SerializeField]
    private float m_fwdSlipMaxSpeed = 10;


    /*[SerializeField]
    private ParticleSystem SidewaysSlipParticles;
    [SerializeField]
    private TrailRenderer WheelTrail;*/

    // Use this for initialization
    void Start ()
    {
        foreach (Wheel tempWheel in m_vehicle.Wheels as List<Wheel>)
        {
            m_fwdSlipEmitters.Add(Instantiate(m_fwdSlipEmitterPrefab, transform));
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        foreach(Wheel wheel in m_vehicle.Wheels)
        {
            WheelHit tempWheelHit;
			if (wheel.Collider.GetGroundHit(out tempWheelHit))
            {
                m_fwdSlipEmitters[i].transform.position = tempWheelHit.point + (Vector3.up * 0.3f);//TODO//magic numbers
                m_fwdSlipEmitters[i].transform.forward = tempWheelHit.forwardDir * (tempWheelHit.forwardSlip < 0 ? 1 : -1);

                ParticleSystem.MainModule tempMainModule = m_fwdSlipEmitters[i].main;
                tempMainModule.startSpeed = Mathf.Abs(tempWheelHit.forwardSlip) * m_fwdSlipMaxSpeed;

                ParticleSystem.EmissionModule tempEmissionModule = m_fwdSlipEmitters[i].emission;
                tempEmissionModule.enabled = true;
                tempEmissionModule.rateOverTime = Mathf.Abs(tempWheelHit.forwardSlip) * m_fwdSlipMaxEmissionRate;

                //tempEmissionModule.rateOverTime = tempWheelHit.sidewaysSlip;
				Debug.Log(m_wheels[i].Collider.name + "RearSlip " + tempWheelHit.forwardSlip);
            }
            else
            {
                ParticleSystem.EmissionModule tempEmissionModule = m_fwdSlipEmitters[i].emission;
                tempEmissionModule.enabled = false;

				Debug.Log(m_wheels[i].Collider.name + "Off");
            }

        }


        //BLWheel.forceAppPointDistance
        /*Debug.Log("forwardslip" + tempWheelHit.forwardSlip);
        Debug.Log("sideslip" + tempWheelHit.sidewaysSlip);
        Debug.Log("rpm" + tempWheelHit.rpm);*/
    }
}
