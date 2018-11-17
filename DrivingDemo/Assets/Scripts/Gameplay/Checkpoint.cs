using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

public class Checkpoint : TriggerZone
{
    //protected Color m_gizmoColor = Color.yellow;

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

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_gizmoColor = Color.yellow;
    }
#endif
}
