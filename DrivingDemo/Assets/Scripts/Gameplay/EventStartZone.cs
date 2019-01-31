using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStartZone : TriggerZone
{
    [SerializeField]
    private Course m_courseToRace = null;
    [SerializeField]
    private int m_laps = 2;

    private Race m_event;


    private new void Awake()
    {
        base.Awake();

        if (m_courseToRace == null)
            Debug.LogError("no course added");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Hull")
            return;

        Hull tempHull = other.GetComponent<Hull>();
        if (tempHull == null)
        {
            Debug.LogError("hull not attached to tagged hull" + other.name, other.gameObject);
            return;
        }


        List<Vehicle> tempParticipants = new List<Vehicle>();
        tempParticipants.Add(tempHull.Owner);

        if (m_event == null)
            m_event = new Race(tempParticipants, m_courseToRace, m_laps);

        if (!m_event.InProgress)
            m_event.Start();
    }



#if UNITY_EDITOR
    private void OnValidate()
    {
        m_gizmoColor = Color.cyan;
    }
#endif
}
