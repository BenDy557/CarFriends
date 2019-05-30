using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

/// <summary>
/// A race contains a course and any details about the status of a race
/// Has it been started how many participants are there, how many laps, how long has it been running
/// how many laps are left etc
/// the race is a thing that can be won and lost, it's not a physical thing
/// </summary>
public class Race : Activity
{
    private Course m_course = null;
    private int m_laps = 0;
    public int Laps { get { return m_laps; } }

    public Race(Course course)
    {
        m_course = course;
    }

    ~Race()
    {
        Unibus.Unsubscribe<TriggerZoneVehiclePair>(EventTags.TriggerEn_CheckpointReached, OnCheckpointArrival);
    }

    public void Start(HashSet<Vehicle> participants, int laps)
    {
        m_laps = laps;
        Unibus.Subscribe<TriggerZoneVehiclePair>(EventTags.TriggerEn_CheckpointReached, OnCheckpointArrival);
        SolveParticipantPositions();

        Start(participants);
    }

    private void OnCheckpointArrival(TriggerZoneVehiclePair triggerZoneVehiclePairIn)
    {
        if (!m_inProgress)
            return;

        Checkpoint checkpointIn = triggerZoneVehiclePairIn.TriggerZone as Checkpoint;
        Vehicle vehicleIn = triggerZoneVehiclePairIn.Vehicle;

        if (!m_participants.ContainsKey(vehicleIn) || !m_course.Checkpoints.Contains(checkpointIn))
            return;

        RaceParticipentStats tempStats = m_participants[vehicleIn] as RaceParticipentStats;

        //if the checkpoint being hit is the next in line for the current vehicle
        if (checkpointIn == m_course.GetNextCheckpoint(tempStats.CurrentCheckpoint))
        {
            //the the current checkpoint is incremented
            tempStats.IncrementCheckpoint(checkpointIn);

            SolveParticipantPositions();
            
            //tempStats.CurrentCheckpoint = checkpointIn;
        }
        else
        {
            Debug.Log("Wrong Checkpoint: " + checkpointIn.name + " Current Checkpoint: " + tempStats.CurrentCheckpoint + " Correct Checkpoint: " + m_course.GetNextCheckpoint(tempStats.CurrentCheckpoint), tempStats.Vehicle.gameObject);
            return;
        }

        Debug.Log("CheckpointReached: " + tempStats.Vehicle.name, tempStats.Vehicle.gameObject);

        if (tempStats.CurrentCheckpoint == m_course.GetFirstCheckpoint())
        {
            Debug.Log("LapCompleted: " + tempStats.Vehicle.name, tempStats.Vehicle.gameObject);
            tempStats.IncrementLap(1);
            
            if (tempStats.LapsCompleted >= m_laps && !tempStats.RaceFinished)
            {
                Debug.Log("RaceFinished: " + tempStats.Vehicle.name, tempStats.Vehicle.gameObject);
                tempStats.FinishRace();
                Stop();
            }
        }

    }

    //TODO incorrectly sorts, as checkpoints arent the best way to work out race position
    private void SolveParticipantPositions()
    {
        RaceComparer raceComparer = new RaceComparer();
        m_particpantStats.Sort(raceComparer);

        foreach (ActivityParticipentStats activityParticipentStats in m_particpantStats)
        {
            (activityParticipentStats as RaceParticipentStats).Position = m_particpantStats.IndexOf(activityParticipentStats) + 1;
        }
    }

    public void AddParticipant(Vehicle vehicle)
    {
        AddParticipant(vehicle, new RaceParticipentStats(this, vehicle, m_course.GetFirstCheckpoint()));
    }

    public override void AddParticipants(ICollection<Vehicle> vehicles)
    {
        foreach (Vehicle vehicle in vehicles)
        {
            AddParticipant(vehicle);
        }
    }
}

public class RaceParticipentStats : ActivityParticipentStats
{
    public Checkpoint CurrentCheckpoint { get; private set; }
    public int LapsCompleted { get; private set; }
    public int CurrentLap { get; private set; }
    public int CheckpointsCompleted { get; private set; }
    public int Position { get; set; }

    public bool RaceFinished { get; private set; }

    public RaceParticipentStats(Activity activity, Vehicle vehicle, Checkpoint checkpoint) : base(activity, vehicle)
    {
        CurrentCheckpoint = checkpoint;
        LapsCompleted = 0;
        RefreshCurrentLap();
    }

    public void IncrementLap(int count)
    {
        LapsCompleted += count;
        RefreshCurrentLap();
    }

    private void RefreshCurrentLap()
    {
        CurrentLap = Mathf.Clamp(LapsCompleted + 1, 0, ((Race)Activity).Laps);
    }

    public void IncrementCheckpoint(Checkpoint checkpointIn)
    {
        CurrentCheckpoint = checkpointIn;
        CheckpointsCompleted++;
    }

    public void FinishRace()
    {
        RaceFinished = true;
    }
}

public class RaceComparer : IComparer<ActivityParticipentStats>
{
    public int Compare(ActivityParticipentStats x, ActivityParticipentStats y)
    {
        RaceParticipentStats xRace = (x as RaceParticipentStats);
        RaceParticipentStats yRace = (y as RaceParticipentStats);

        if (xRace == null || yRace == null)
            return 0;

        return yRace.CheckpointsCompleted.CompareTo(xRace.CheckpointsCompleted);
    }
}