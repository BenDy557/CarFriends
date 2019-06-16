using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

public abstract class Activity
{
    public ActivityType Type { get; set; }

    protected Dictionary<Vehicle, ActivityParticipentStats> m_participants;
    protected List<ActivityParticipentStats> m_particpantStats;

    protected bool m_inProgress = false;
    public bool InProgress { get { return m_inProgress; } }

    protected float m_startTime;

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

    /// <summary>
    /// Starts the activity. Is at protected access level to make sure an activity is started with the correct parameters for the given activity type
    /// </summary>
    /// <returns>The start.</returns>
    /// <param name="participants">Participants.</param>
    protected virtual bool Start(HashSet<Vehicle> participants)
    {
        if (participants == null)
            return false;

        AddParticipants(participants);

        if (m_inProgress)
            return false;

        m_startTime = Time.time;
        m_inProgress = true;

        SolveParticipantPositions();

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

    public virtual void Update()
    {

    }

    /// <summary>
    /// sorts participants list in competition order
    /// </summary>
    protected abstract void SolveParticipantPositions();

    /// <summary>
    /// needs to be implemented by inherited classes so they can call their own version of :AddParticipant(Vehicle, ActivityParticipentStats)
    /// this is to allow inherited activities to add their own custom ActivityParticipentStats type
    /// </summary>
    /// <param name="vehicle">Vehicle.</param>
    public abstract void AddParticipant(Vehicle vehicle);

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
    public void AddParticipants(ICollection<Vehicle> vehicles)
    {
        foreach (Vehicle vehicle in vehicles)
        {
            AddParticipant(vehicle);
        }
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
    public System.Type Type { get; protected set; }
    public Activity Activity { get; protected set; }
    public Vehicle Vehicle { get; protected set; }
    public bool FinishedActivity { get; protected set; }
    public int Position { get; set; }

    public ActivityParticipentStats(Activity activity, Vehicle vehicle)
    {
        Activity = activity;
        Vehicle = vehicle;
    }
}
