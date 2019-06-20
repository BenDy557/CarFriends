using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventTags
{
    #region NetworkEvents
    public static string NetDataReceived = "NetworkDataReceived";
    public static string NetDataReceived_Network_Message = "NetDataReceived_Network_Message";
    //public static string NetDataReceived_Join = "NetDataReceived_Join";
    public static string NetDataReceived_Locomotion= "NetDataReceived_Locomotion";
    public static string NetDataReceived_Input = "NetDataReceived_Input";
    public static string NetDataReceived_Server_Brodacast = "NetDataReceived_Server_Brodacast";

    public static string OnServerStart = "OnServerStart";
    public static string OnClientStart = "OnClientStart";
    #endregion

    #region TriggerEvents
    public static string TriggerEn_ActivityStartZone = "TriggerEn_ActivityStartZone";
    public static string TriggerEx_ActivityStartZone = "TriggerEx_ActivityStartZone";
    public static string TriggerEn_CheckpointReached = "TriggerEn_CheckpointReached";

    #endregion

    #region ActivityEvents
    public static string Activity_RequestStart = "Activity_RequestStart";

    public static string Activity_OnStart = "Activity_OnStart";
    public static string Activity_OnFinish = "Activity_OnFinish";
    #endregion

    #region RaceEvents
    public static string ActivityRace_OnFinish = "ActivityRace_OnFinish";
    #endregion
}
