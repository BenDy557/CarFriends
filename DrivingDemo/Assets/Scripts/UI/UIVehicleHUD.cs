using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIVehicleHUD : MonoBehaviour
{
    //[SerializeField]
    private Vehicle m_vehicle;//TODO// should this be some sort of player class as opposed to referencing a vehicle? it technically is associated with the camera more than the vehicle
    private Camera m_camera;

    private Activity m_currentActivity;
    private EventStartZone m_currentActivityZone;

    [SerializeField]
    private Canvas m_canvas;


    [Header("Vehicle"),SerializeField]
    private TextMeshProUGUI m_speedometer;
    [SerializeField]
    private Utils.Units m_units = Utils.Units.MS;

    [Header("Activity"), SerializeField]
    private TextMeshProUGUI m_activityProgress;
    [SerializeField]
    private TextMeshProUGUI m_activityPosition;

    [SerializeField]
    private TextMeshProUGUI m_activityPrompt;



    private void Start()
    {
        Debug.LogWarning("BAD CODE");
        m_canvas.worldCamera = m_camera;//Why is this bad?

        this.BindUntilDestroy<Activity>(EventTags.Activity_OnStart, OnActivityStart);
        this.BindUntilDestroy<Activity>(EventTags.Activity_OnFinish, OnActivityFinish);
        this.BindUntilDestroy<TriggerZoneVehiclePair>(EventTags.TriggerEn_ActivityStartZone, OnStartZoneEntered);
        this.BindUntilDestroy<TriggerZoneVehiclePair>(EventTags.TriggerEx_ActivityStartZone, OnStartZoneExit);
    }

    private void Update()
    {
        float speed = Utils.UnitConverter(Utils.Units.MS, m_vehicle.Engine.Speed, m_units);
        m_speedometer.text = Mathf.RoundToInt(speed).ToString() + m_units.ToString();


        if (m_currentActivity != null)
        {
            Race currentRace = m_currentActivity as Race;
            if (currentRace != null && currentRace.InProgress)
            {
                RaceParticipentStats stats = (currentRace.GetParticipantStat(m_vehicle) as RaceParticipentStats);
                m_activityProgress.text = (stats.CurrentLap).ToString() + "/" + currentRace.Laps.ToString();
                m_activityPosition.text = stats.Position.ToString();
            }
        }
    }

    public void Initialise(Vehicle vehichle, Camera camera)
    {
        Debug.Log("Camera" + camera.gameObject.name, camera.gameObject);

        m_vehicle = vehichle;
        m_camera = camera;
    }

    private void RefreshActivityPrompt()
    {
        if (m_currentActivityZone == null)
        {
            m_activityPrompt.enabled = false;
            return;
        }

        Activity activity = m_currentActivityZone.Activity;

        if (activity.CanJoin(m_vehicle))
        {
            //display join request prompt
            m_activityPrompt.text = "Join " + activity.GetType().ToString() + " Activity";
            m_activityPrompt.enabled = true;
        }
        else if (activity.InProgress && !activity.ContainsParticipant(m_vehicle))
        {
            m_activityPrompt.text = activity.GetType().ToString() + " Activity in progress";
            m_activityPrompt.enabled = true;
        }
        else
            m_activityPrompt.enabled = false;
    }

    private void OnActivityStart(Activity activity)
    {
        if (activity.ContainsParticipant(m_vehicle))
        {
            m_currentActivity = activity;

            m_activityProgress.enabled = true;
            m_activityPosition.enabled = true;
            /*Race currentRace = m_currentActivity as Race;
            if (currentRace != null)
            {
                this.BindUntilDestroy<Activity>(EventTags.Trigger_CheckpointReached, OnCheckpointReached);
            }*/
        }

        RefreshActivityPrompt();
    }

    private void OnActivityFinish(Activity activity)
    {
        if (activity.ContainsParticipant(m_vehicle))
        {
            m_activityProgress.enabled = false;
            m_activityPosition.enabled = false;

            m_currentActivity = null;
        }

        RefreshActivityPrompt();
    }

    private void OnStartZoneEntered(TriggerZoneVehiclePair triggerVehiclePair)
    {
        if (triggerVehiclePair.Vehicle != m_vehicle)
            return;

        m_currentActivityZone = triggerVehiclePair.TriggerZone as EventStartZone;
        RefreshActivityPrompt();
    }

    private void OnStartZoneExit(TriggerZoneVehiclePair triggerVehiclePair)
    {
        if (triggerVehiclePair.Vehicle != m_vehicle)
            return;

        m_currentActivityZone = null;
        RefreshActivityPrompt();
    }
}
