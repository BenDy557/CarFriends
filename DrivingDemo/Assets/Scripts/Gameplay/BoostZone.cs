using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostZone : TriggerZone
{
    [Header("BoostZone")]
    //private VehicleEffectData m_data;
    [SerializeField]
    private VehicleEffect m_effect;

	protected override void Awake()
    {
        base.Awake();
        m_gizmoColor = Color.magenta;		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hull"))
        {
            //other.GetComponent<Hull>().Owner.ApplyEffect(m_data == null ? m_effect : m_data.Effect);
            other.GetComponent<Hull>().Owner.ApplyEffect(m_effect);
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hull"))
        {
            //other.GetComponent<Hull>().Owner.ApplyEffect(m_data == null ? m_effect : m_data.Effect);
            other.GetComponent<Hull>().Owner.RemoveEffect(m_effect);
        }
    }*/
}
