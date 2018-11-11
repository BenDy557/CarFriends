using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private float m_radius = 5f;

    [SerializeField]
    private float m_height = 5f;

    private static float m_underHeight = 3f;
    private static LayerMask m_terrainLayers;

    private CapsuleCollider m_collider;

    private void Awake()
    {
        m_collider = gameObject.AddComponent<CapsuleCollider>();
        m_collider.isTrigger = true;
        m_collider.radius = m_radius;
        m_collider.height = m_height + m_underHeight + (m_radius*2.0f);
        m_collider.center = Vector3.up * ((-m_underHeight) + (m_height * 0.5f));
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "Hull")
            return;

        Hull tempHull = collider.GetComponent<Hull>();
        if (tempHull == null)
        {
            Debug.LogError("hull not attached to tagged hull" + collider.name, collider.gameObject);
            return;
        }

        Unibus.Dispatch<CheckpointVehiclePair>(EventTags.CheckpointReached, new CheckpointVehiclePair(this, tempHull.Owner));
    }


#if UNITY_EDITOR
    [NaughtyAttributes.Button]
    private void SnapToFloor()
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
        Handles.color = new Color(1f, 0.96f, 0.016f, 0.4f);
        Handles.DrawWireDisc(transform.position, Vector3.up, m_radius);
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = new Color(1f, 0.96f, 0.016f, 1f);
        Handles.DrawWireDisc(transform.position, Vector3.up, m_radius);

        Ray tempRay = new Ray(transform.position + (Vector3.up * 500f), Vector3.down);
        RaycastHit raycastHit;
        Physics.Raycast(tempRay, out raycastHit, 1000f);

        if (raycastHit.collider != null)
        {
            Handles.DrawDottedLine(transform.position, raycastHit.point, 3f);

            Handles.color = new Color(1f, 0.96f, 0.016f, 0.5f);
            Handles.DrawWireDisc(raycastHit.point, Vector3.up, m_radius);
        }


        Handles.color = new Color(1f, 0.96f, 0.016f, 0.3f);
        Handles.DrawWireDisc(transform.position + (Vector3.up * m_height), Vector3.up, m_radius);
        //Handles.color = new Color(1f, 0.96f, 0.016f, 0.2f);
        //Handles.DrawWireDisc(transform.position - (Vector3.up * m_underHeight), Vector3.up, m_radius);
    }
#endif  

    public struct CheckpointVehiclePair
    {
        public Checkpoint Checkpoint;
        public Vehicle Vehicle;

        public CheckpointVehiclePair(Checkpoint checkpointIn, Vehicle vehicleIn)
        {
            Checkpoint = checkpointIn;
            Vehicle = vehicleIn;
        }
    }
}
