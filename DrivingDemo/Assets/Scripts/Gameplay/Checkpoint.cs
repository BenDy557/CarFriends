using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

public class Checkpoint : TriggerZone, IWaypointTarget
{
    //protected Color m_gizmoColor = Color.yellow;

    private void OnTriggerEnter(Collider collider)
    {
        Vehicle tempVehicle = null;
        if (!UtilsGameplay.IsVehicle(collider, out tempVehicle))
            return;

        Unibus.Dispatch<TriggerZoneVehiclePair>(EventTags.TriggerEn_CheckpointReached, new TriggerZoneVehiclePair(this, tempVehicle));
    }

    #region IWaypointTarget
    public Vector3 WaypointPosition { get { return transform.position; } }
    public Quaternion WaypointRotation { get { return transform.rotation; } }
    public Vector3 WaypointScale { get { return new Vector3(m_radius * 2f, m_height, 1); } }
    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_gizmoColor = Color.yellow;
    }
#endif
}
