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


    private HashSet<Vehicle> m_entrants = new HashSet<Vehicle>();
    private HashSet<Vehicle> m_participants = new HashSet<Vehicle>();

    private new void Awake()
    {
        base.Awake();

        if (m_courseToRace == null)
            Debug.LogError("no course added");

    }

    private void OnEnable()
    {
        this.BindUntilDisable<Vehicle>(EventTags.Activity_RequestStart, RequestStart);
    }

    [NaughtyAttributes.Button]
    private void RequestStart(Vehicle vehicle)
    {
        if (!m_entrants.Contains(vehicle))
            return;

        if (m_activity == null || !m_activity.InProgress)
        {
            m_participants.Clear();
            foreach (Vehicle entrant in m_entrants)
            {
                m_participants.Add(entrant);
            }
            m_activity = new Race(m_participants, m_courseToRace, m_laps);
            m_activity.Start();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Vehicle tempVehicle = null;
        if (!UtilsGameplay.IsVehicle(other, out tempVehicle))
            return;

        Debug.Log("VEHICLE" + tempVehicle,tempVehicle);
        m_entrants.Add(tempVehicle);


        /*if (m_event == null)
            m_event = new Race(tempParticipants, m_courseToRace, m_laps);

        if (!m_event.InProgress)
            m_event.Start();*/
    }

    private void OnTriggerExit(Collider other)
    {
        Vehicle tempVehicle = null;
        if (!UtilsGameplay.IsVehicle(other, out tempVehicle))
            return;

        m_entrants.Remove(tempVehicle);
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        m_gizmoColor = Color.cyan;
    }
#endif
}
