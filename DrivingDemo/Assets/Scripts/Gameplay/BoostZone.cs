using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostZone : TriggerZone
{
    [Header("BoostZone")]
    //private VehicleEffectData m_data;
    [SerializeField]
    private VehicleEffect m_effect;

    private List<Vehicle> m_ignoreList = new List<Vehicle>();

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
            Vehicle tempVehicle = other.GetComponent<Hull>().Owner;

            if (m_ignoreList.Contains(tempVehicle))
                return;

            tempVehicle.ApplyEffect(new VehicleEffect(m_effect));
        }
    }

    public void AddToIgnoreList(Vehicle vehicle)
    {
        m_ignoreList.Add(vehicle);
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
