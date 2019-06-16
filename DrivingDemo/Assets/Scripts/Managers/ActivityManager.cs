using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

public class ActivityManager : Singleton<ActivityManager>
{

    [SerializeField]
    private static List<Activity> m_currentActivities;


    protected override void Awake()
    {
        base.Awake();

        m_currentActivities = new List<Activity>();
    }

    private void Update()
    {
        foreach (Activity activity in m_currentActivities)
            activity.Update();
    }

    public static void RegisterActivity(Activity activity)
    {
        m_currentActivities.Add(activity);
    }

    public static void DeregisterActivity(Activity activity)
    {
        m_currentActivities.Remove(activity);
    }

    public static bool HasActivity(Vehicle vehicle)
    {
        foreach (Activity activity in m_currentActivities)
        {
            if (activity.ContainsParticipant(vehicle))
                return true;
        }
        
        return false;
    }
}

public enum ActivityType
{
    NONE,
    RACE,
    HIGH_JUMP,
}