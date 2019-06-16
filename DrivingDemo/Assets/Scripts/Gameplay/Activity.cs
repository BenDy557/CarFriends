using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

public abstract class Activity
{
    protected Dictionary<Vehicle, ActivityParticipentStats> m_participants;
    protected List<ActivityParticipentStats> m_particpantStats;

    protected bool m_inProgress = false;
    public bool InProgress { get { return m_inProgress; } }

    //creates the data structures but the inherited class chooses what to put in them
    public Activity()
    {
        m_participants = new Dictionary<Vehicle, ActivityParticipentStats>();
        m_particpantStats = new List<ActivityParticipentStats>();
    }

    ~Activity()
    {
        //Unibus.Unsubscribe<Checkpoint.CheckpointVehiclePair>(EventTags.CheckpointReached, OnCheckpointArrival);
    }

    protected virtual bool Start(HashSet<Vehicle> participants)
    {
        if (participants == null)
            return false;

        AddParticipants(participants);

        if (m_inProgress)
            return false;

        m_inProgress = true;

        ActivityManager.RegisterActivity(this);
        Unibus.Dispatch<Activity>(EventTags.Activity_OnStart, this);
        return true;
    }

    public virtual void Stop()
    {
        if (!m_inProgress)
            return;

        ActivityManager.DeregisterActivity(this);
        Unibus.Dispatch<Activity>(EventTags.Activity_OnFinish, this);

        m_participants.Clear();
        m_particpantStats.Clear();
        
        m_inProgress = false;
    }

    /// <summary>
    /// Add a participant to the activity
    /// If you want to make this function public you should override it in a derived class
    /// This is to to stop adding participants to an activity that you do not know the type of
    /// </summary>
    /// <param name="vehicle"></param>
    /// <param name="activityStats"></param>
    protected virtual void AddParticipant(Vehicle vehicle, ActivityParticipentStats activityStats)
    {
        m_participants.Add(vehicle, activityStats);
        m_particpantStats.Add(activityStats);
    }

    //
    public abstract void AddParticipants(ICollection<Vehicle> vehicle);

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

    public bool CanJoin(Vehicle vehicle)
    {
        if (ActivityManager.HasActivity(vehicle))
            return false;

        if (m_inProgress)
            return false;

        return true;
    }
}

public class ActivityParticipentStats
{
    //private Activity m_activity;
    public Activity Activity { get; private set; }
    public Vehicle Vehicle { get; private set; }
    public bool finishedActivity;

    public ActivityParticipentStats(Activity activity, Vehicle vehicle)
    {
        Activity = activity;
        Vehicle = vehicle;
    }
}
