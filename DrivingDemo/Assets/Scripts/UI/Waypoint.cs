using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private IWaypointTarget m_target;
    [SerializeField]
    private Transform m_modelTransform;

    public void SetTarget(IWaypointTarget target)
    {
        m_target = target;

        transform.position = m_target.WaypointPosition;
        transform.rotation = m_target.WaypointRotation;
        m_modelTransform.localScale = m_target.WaypointScale;
    }

    public void ToggleActive(bool toggle)
    {
        gameObject.SetActive(toggle);
    }
}

public interface IWaypointTarget
{
    Vector3 WaypointPosition { get; }
    Quaternion WaypointRotation { get; }
    Vector3 WaypointScale { get; }
}