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

    public Race(HashSet<Vehicle> participants, Course course, int laps) : base(participants)
    {
        foreach (Vehicle vehicle in participants)
        {
            m_participants.Add(vehicle, new RaceParticipentStats(this, vehicle, course.GetFirstCheckpoint()));
        }

        m_course = course;
        m_laps = laps;

        Unibus.Subscribe<TriggerZoneVehiclePair>(EventTags.Trigger_CheckpointReached, OnCheckpointArrival);
    }

    ~Race()
    {
        Unibus.Unsubscribe<TriggerZoneVehiclePair>(EventTags.Trigger_CheckpointReached, OnCheckpointArrival);
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
        if (checkpointIn == m_course.GetNextCheckpoint(tempStats.currentCheckpoint))
        {
            //the the current checkpoint is incremented
            tempStats.currentCheckpoint = checkpointIn;

            //Checkpoint nextCheckpoint = m_course.GetNextCheckpoint(checkpointIn);
        }
        else
        {
            Debug.Log("Wrong Checkpoint: " + checkpointIn.name + " Current Checkpoint: " + tempStats.currentCheckpoint + " Correct Checkpoint: " + m_course.GetNextCheckpoint(tempStats.currentCheckpoint), tempStats.m_vehicle.gameObject);
            return;
        }


        Debug.Log("CheckpointReached: " + tempStats.m_vehicle.name, tempStats.m_vehicle.gameObject);

        if (tempStats.currentCheckpoint == m_course.GetFirstCheckpoint())
        {
            Debug.Log("LapCompleted: " + tempStats.m_vehicle.name, tempStats.m_vehicle.gameObject);
            tempStats.lapsCompleted++;

            if (tempStats.lapsCompleted >= m_laps && !tempStats.finishedRace)
            {
                Debug.Log("RaceFinished: " + tempStats.m_vehicle.name, tempStats.m_vehicle.gameObject);
                tempStats.finishedRace = true;
                Stop();
            }
        }

    }
}

public class RaceParticipentStats : ActivityParticipentStats
{
    public Checkpoint currentCheckpoint;
    public int lapsCompleted;

    public bool finishedRace;

    public RaceParticipentStats(Activity activity, Vehicle vehicle, Checkpoint checkpoint) : base(activity, vehicle)
    {
        currentCheckpoint = checkpoint;
        lapsCompleted = 0;
    }
}