﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventTags
{
    #region NetworkEvents
    public static string NetDataReceived = "NetworkDataReceived";
    public static string NetDataReceived_Message = "NetDataReceived_Message";
    public static string NetDataReceived_Join = "NetDataReceived_Join";
    public static string NetDataReceived_Locomotion= "NetDataReceived_Locomotion";
    public static string NetDataReceived_Input = "NetDataReceived_Input";
    #endregion

    #region RaceEvents
    public static string CheckpointReached = "CheckpointReached";
    #endregion
}
