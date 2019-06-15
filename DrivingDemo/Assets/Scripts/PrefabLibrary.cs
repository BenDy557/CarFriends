using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefabLibrary
{
    public static GameObject Get(string location)
    {
        return Resources.Load<GameObject>(location);
    }

    public static string CheckpointDefault = "Prefabs/CheckPoint";
    public static string VehicleDefault = "Prefabs/Buggy";
    public static string WaypointDefault = "Prefabs/UI/Waypoint";
    public static string CameraDefault = "Prefabs/Camera";
    public static string HUDDefault = "Prefabs/UI/HUD";
}
