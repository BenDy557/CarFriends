using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;
public class EventStartZone : TriggerZone
{
    [SerializeField]
    private Course m_courseToRace = null;
    [SerializeField]
    private int m_laps = 2;

    private Race m_activity;
    public Race Activity { get { return m_activity; } }

    private HashSet<Vehicle> m_entrants = new HashSet<Vehicle>();

    protected override void Awake()
    {
        base.Awake();

        if (m_courseToRace == null)
            Debug.LogError("no course added");

        m_activity = new Race(m_courseToRace);
    }

    private void OnEnable()
    {
        this.BindUntilDisable<Vehicle>(EventTags.Activity_RequestStart, RequestStart);
    }

    private void RequestStart(Vehicle vehicle)
    {
        if (!m_entrants.Contains(vehicle))
            return;

        if (m_activity == null || !m_activity.InProgress)
        {
            m_activity.Start(m_entrants,m_laps);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Vehicle tempVehicle = null;
        if (!UtilsGameplay.IsVehicle(other, out tempVehicle))
            return;

        m_entrants.Add(tempVehicle);

        Unibus.Dispatch<TriggerZoneVehiclePair>(EventTags.TriggerEn_ActivityStartZone, new TriggerZoneVehiclePair(this, tempVehicle));
    }

    private void OnTriggerExit(Collider other)
    {
        Vehicle tempVehicle = null;
        if (!UtilsGameplay.IsVehicle(other, out tempVehicle))
            return;

        m_entrants.Remove(tempVehicle);

        Unibus.Dispatch<TriggerZoneVehiclePair>(EventTags.TriggerEx_ActivityStartZone, new TriggerZoneVehiclePair(this, tempVehicle));
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_gizmoColor = Color.cyan;
    }
#endif
}
