using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

public abstract class Activity
{
    protected Dictionary<Vehicle, ActivityParticipentStats> m_participants;



    protected bool m_inProgress = false;
    public bool InProgress { get { return m_inProgress; } }

    public virtual void Start()
    {
        if (m_inProgress)
            return;

        m_inProgress = true;

        Unibus.Dispatch<Activity>(EventTags.Activity_OnStart, this);
    }

    public virtual void Stop()
    {
        if (!m_inProgress)
            return;

        m_inProgress = false;
        //Unibus.Unsubscribe<Checkpoint.CheckpointVehiclePair>(EventTags.CheckpointReached, OnCheckpointArrival);
        Unibus.Dispatch<Activity>(EventTags.Activity_OnFinish, this);
    }

    public Activity(HashSet<Vehicle> participants)
    {
        m_participants = new Dictionary<Vehicle, ActivityParticipentStats>();
    }
    //{
    //    m_participants = new Dictionary<Vehicle, ActivityParticipentStats>();

    //    foreach (Vehicle vehicle in participants)
    //    {
    //        m_participants.Add(vehicle, new ActivityParticipentStats(vehicle));
    //    }

    //    //Unibus.Subscribe<Checkpoint.CheckpointVehiclePair>(EventTags.CheckpointReached, OnCheckpointArrival);
    //}

    ~Activity()
    {
        //Unibus.Unsubscribe<Checkpoint.CheckpointVehiclePair>(EventTags.CheckpointReached, OnCheckpointArrival);
    }

    public bool ContainsParticipant(Vehicle vehicle)
    {
        return m_participants.ContainsKey(vehicle);
    }

    public ActivityParticipentStats GetParticipantStat(Vehicle vehicle)
    {
        if (m_participants.ContainsKey(vehicle))
            return m_participants[vehicle];

        return null;
    }
}

public class ActivityParticipentStats
{
    public Activity m_activity;
    public Vehicle m_vehicle;
    public bool finishedActivity;

    public ActivityParticipentStats(Activity activity, Vehicle vehicle)
    {
        m_activity = activity;
        m_vehicle = vehicle;
    }
}
