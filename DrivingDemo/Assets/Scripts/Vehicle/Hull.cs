using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hull : MonoBehaviour
{
    [SerializeField]
    private Vehicle m_owner;
    public Vehicle Owner { get { return m_owner; } }
    
    public void Start()
    {
        if (m_owner == null)
        {
            Debug.LogError("Hull has no owner", gameObject);
        }
    }

    [NaughtyAttributes.Button]
    private void GetOwner()
    {
        Transform parentCheck = transform.parent;
        while (transform.parent != null && m_owner == null)
        {
            Vehicle tempVehicle = parentCheck.gameObject.GetComponent<Vehicle>();
            m_owner = tempVehicle;
            parentCheck = parentCheck.parent;
        }
    }
}
