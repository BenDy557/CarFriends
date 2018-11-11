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
public class Race
{
    private Course m_course;
    private int m_laps;
    private bool m_inProgress = false;
    private Dictionary<Vehicle, RaceParticipentStats> m_participants = null;

    public Race(List<Vehicle> participants, Course course, int laps)
    {
        m_participants = new Dictionary<Vehicle, RaceParticipentStats>();

        foreach (Vehicle vehicle in participants)
        {
            m_participants.Add(vehicle, new RaceParticipentStats(vehicle,course.GetFirstCheckpoint()));
        }

        m_course = course;
        m_laps = laps;

    }

    public void Start()
    {
        m_inProgress = true;

        Unibus.Subscribe<Checkpoint.CheckpointVehiclePair>(EventTags.CheckpointReached, OnCheckpointArrival);
    }

    public void Stop()
    {
        m_inProgress = false;
        Unibus.Unsubscribe<Checkpoint.CheckpointVehiclePair>(EventTags.CheckpointReached, OnCheckpointArrival);
    }

    private void OnCheckpointArrival(Checkpoint.CheckpointVehiclePair checkpointVehiclePairIn)
    {
        if (!m_inProgress)
            return;

        Checkpoint checkpointIn = checkpointVehiclePairIn.Checkpoint;
        Vehicle vehicleIn = checkpointVehiclePairIn.Vehicle;

        if (!m_course.Checkpoints.Contains(checkpointIn) || !m_participants.ContainsKey(vehicleIn))
            return;

        RaceParticipentStats tempStats = m_participants[vehicleIn];

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

public class RaceParticipentStats
{
    public Vehicle m_vehicle;
    public Checkpoint currentCheckpoint;
    public int lapsCompleted;

    public bool finishedRace;

    public RaceParticipentStats(Vehicle vehicle, Checkpoint checkpoint)
    {
        m_vehicle = vehicle;
        currentCheckpoint = checkpoint;
        lapsCompleted = 0;
    }
}