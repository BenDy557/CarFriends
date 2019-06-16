using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityHighJump : Activity
{
    private float m_duration = -1f;

    public ActivityHighJump() : base()
    {
        Type = ActivityType.HIGH_JUMP;
    }

    public override void Update()
    {
        base.Update();

        bool refreshPositions = false;

        foreach (ActivityParticipentStats stats in m_particpantStats)
        {
            HighJumpParticipantStats highJumpStats = stats as HighJumpParticipantStats;
            if (highJumpStats == null)
                continue;

            float currentHeight = stats.Vehicle.transform.position.y;
            if (currentHeight > highJumpStats.HighestPoint)
            {
                highJumpStats.HighestPoint = currentHeight;
                refreshPositions = true;
            }
        }

        if (refreshPositions)
            SolveParticipantPositions();
    }

    public void Start(HashSet<Vehicle> participants, float duration)
    {
        Start(participants);

        m_duration = duration;
        SolveParticipantPositions();
    }

    protected override void SolveParticipantPositions()
    {
        HighJumpComparer highJumpComparer = new HighJumpComparer();
        m_particpantStats.Sort(highJumpComparer);

        foreach (ActivityParticipentStats activityParticipentStats in m_particpantStats)
        {
            (activityParticipentStats as HighJumpParticipantStats).Position = m_particpantStats.IndexOf(activityParticipentStats) + 1;
        }
    }

    public override void AddParticipant(Vehicle vehicle)
    {
        AddParticipant(vehicle, new HighJumpParticipantStats(this, vehicle));
    }
}

public class HighJumpParticipantStats : ActivityParticipentStats
{
    public float HighestPoint = 0;

    public HighJumpParticipantStats(Activity activity, Vehicle vehicle) : base(activity,vehicle)
    {
        Type = typeof(HighJumpParticipantStats);
    }
}

public class HighJumpComparer : IComparer<ActivityParticipentStats>
{
    public int Compare(ActivityParticipentStats firstIn, ActivityParticipentStats secondIn)
    {
        HighJumpParticipantStats first = (firstIn as HighJumpParticipantStats);
        HighJumpParticipantStats second = (secondIn as HighJumpParticipantStats);

        if (first == null || second == null)
            return 0;

        return second.HighestPoint.CompareTo(first.HighestPoint);
    }
}