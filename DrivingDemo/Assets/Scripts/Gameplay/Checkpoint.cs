using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

public class Checkpoint : TriggerZone
{
    //protected Color m_gizmoColor = Color.yellow;

    private void OnTriggerEnter(Collider collider)
    {
        Vehicle tempVehicle = null;
        if (!UtilsGameplay.IsVehicle(collider, out tempVehicle))
            return;

        Unibus.Dispatch<TriggerZoneVehiclePair>(EventTags.TriggerEn_CheckpointReached, new TriggerZoneVehiclePair(this, tempVehicle));
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_gizmoColor = Color.yellow;
    }
#endif
}
