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

    [SerializeField]
    private Canvas m_canvas;




    
    [Header("Vehicle"),SerializeField]
    private TextMeshProUGUI m_speedometer;
    [SerializeField]
    private Utils.Units m_units = Utils.Units.MS;

    [Header("Activity"), SerializeField]
    private TextMeshProUGUI m_activityProgress;
    private TextMeshProUGUI m_activityPosition;



    private void Start()
    {
        Debug.LogWarning("BAD CODE");
        m_canvas.worldCamera = m_camera;

        this.BindUntilDestroy<Activity>(EventTags.Activity_OnStart, OnActivityStart);
    }

    private void Update()
    {
        float speed = Utils.UnitConverter(Utils.Units.MS, m_vehicle.Engine.Speed, m_units);
        m_speedometer.text = Mathf.RoundToInt(speed).ToString() + m_units.ToString();


        if (m_currentActivity != null)
        {
            Race currentRace = m_currentActivity as Race;
            if (currentRace != null)
            {
                RaceParticipentStats stats = (currentRace.GetParticipantStat(m_vehicle) as RaceParticipentStats);
                m_activityProgress.text = stats.lapsCompleted.ToString() + "/" + currentRace.Laps.ToString();
            }
        }
    }

    public void Initialise(Vehicle vehichle, Camera camera)
    {
        Debug.Log("Camera" + camera.gameObject.name, camera.gameObject);

        m_vehicle = vehichle;
        m_camera = camera;
    }

    private void OnActivityStart(Activity activity)
    {
        if (activity.ContainsParticipant(m_vehicle))
        {
            m_currentActivity = activity;

            /*Race currentRace = m_currentActivity as Race;
            if (currentRace != null)
            {
                this.BindUntilDestroy<Activity>(EventTags.Trigger_CheckpointReached, OnCheckpointReached);
            }*/
        }
    }
}
