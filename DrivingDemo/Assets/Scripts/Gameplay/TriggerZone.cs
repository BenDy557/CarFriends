using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class TriggerZone : MonoBehaviour
{
    [SerializeField]
    protected float m_radius = 5f;

    [SerializeField]
    protected float m_height = 5f;

    private static float m_underHeight = 3f;

    protected Color m_gizmoColor = Color.magenta;

    protected CapsuleCollider m_collider;

    protected virtual void Awake()
    {
        m_collider = gameObject.AddComponent<CapsuleCollider>();
        m_collider.isTrigger = true;
        m_collider.radius = m_radius;
        m_collider.height = m_height + m_underHeight + (m_radius * 2.0f);
        m_collider.center = Vector3.up * ((-m_underHeight) + (m_height * 0.5f));
    }

    

#if UNITY_EDITOR
    [NaughtyAttributes.Button]
    protected void SnapToFloor()
    {
        Ray tempRay = new Ray(transform.position + (Vector3.up * 500f), Vector3.down);
        RaycastHit raycastHit;
        Physics.Raycast(tempRay, out raycastHit, 1000f);

        if (raycastHit.collider == null)
        {
            Debug.LogError("Could not find surface to snap to");
        }
        else
        {
            transform.position = raycastHit.point;
        }
    }

    private void OnDrawGizmos()
    {
        Color tempColor = m_gizmoColor;
        tempColor.a = 0.4f;
        Handles.color = tempColor;
        Handles.DrawWireDisc(transform.position, Vector3.up, m_radius);
    }

    private void OnDrawGizmosSelected()
    {
        Color tempColor = m_gizmoColor;
        tempColor.a = 1f;
        Handles.color = tempColor;
        Handles.DrawWireDisc(transform.position, Vector3.up, m_radius);

        Ray tempRay = new Ray(transform.position + (Vector3.up * 500f), Vector3.down);
        RaycastHit raycastHit;
        Physics.Raycast(tempRay, out raycastHit, 1000f);

        if (raycastHit.collider != null)
        {
            Handles.DrawDottedLine(transform.position, raycastHit.point, 3f);

            tempColor.a = 0.5f;
            Handles.color = tempColor;
            Handles.DrawWireDisc(raycastHit.point, Vector3.up, m_radius);
        }

        tempColor.a = 0.3f;
        Handles.color = tempColor;
        Handles.DrawWireDisc(transform.position + (Vector3.up * m_height), Vector3.up, m_radius);
        //Handles.color = new Color(1f, 0.96f, 0.016f, 0.2f);
        //Handles.DrawWireDisc(transform.position - (Vector3.up * m_underHeight), Vector3.up, m_radius);
    }
#endif
}

public struct TriggerZoneVehiclePair
{
    public TriggerZone TriggerZone;
    public Vehicle Vehicle;

    public TriggerZoneVehiclePair(TriggerZone checkpointIn, Vehicle vehicleIn)
    {
        TriggerZone = checkpointIn;
        Vehicle = vehicleIn;
    }
}
