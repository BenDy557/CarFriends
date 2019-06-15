using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : Singleton<WaypointManager>
{
    private static Dictionary<Vehicle, Waypoint> m_waypointSets;

    protected override void Awake()
    {
        base.Awake();

        m_waypointSets = new Dictionary<Vehicle, Waypoint>();
    }

    public static void SetWaypoint(Vehicle vehicle, IWaypointTarget target, bool activate = false)
    {
        if (!m_waypointSets.ContainsKey(vehicle))
            return;

        Waypoint tempWaypoint = GetWaypoint(vehicle);

        if(activate)
            tempWaypoint.ToggleActive(true);

        tempWaypoint.SetTarget(target);
    }

    public static void ToggleWaypoint(Vehicle vehicle, bool toggle)
    {
        if (!m_waypointSets.ContainsKey(vehicle))
            return;

        m_waypointSets[vehicle].ToggleActive(toggle);
    }

    private static Waypoint GetWaypoint(Vehicle vehicle)
    {
        if (!m_waypointSets.ContainsKey(vehicle))
            return null;

        return m_waypointSets[vehicle];
    }

    public static void AddLocalPlayer(Vehicle vehicle)
    {
        Waypoint tempWaypoint = Instantiate(PrefabLibrary.Get(PrefabLibrary.WaypointDefault)).GetComponent<Waypoint>();
        //tempWaypoint.Initialise(vehicle);

        m_waypointSets.Add(vehicle, tempWaypoint);
    }
}
