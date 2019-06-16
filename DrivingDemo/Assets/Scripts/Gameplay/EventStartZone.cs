using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;
using NaughtyAttributes;

public class EventStartZone : TriggerZone
{
    [SerializeField]
    private ActivityType m_activityType;

    #region customInspector
    private bool IsRace() { return m_activityType == ActivityType.RACE; } 
    private bool IsHighJump(){ return m_activityType == ActivityType.HIGH_JUMP; } 
    #endregion

    [SerializeField,ShowIf("IsRace")]
    private Course m_courseToRace = null;
    [SerializeField, ShowIf("IsRace")]
    private int m_laps = 2;
    [SerializeField, ShowIf("IsHighJump")]
    private float m_duration = -1f;


    private Activity m_activity;
    public Activity Activity { get { return m_activity; } }

    private HashSet<Vehicle> m_entrants = new HashSet<Vehicle>();

    [SerializeField]
    private ParticleSystem m_particleSystem;

    protected override void Awake()
    {
        base.Awake();

        SetParticleRadius(m_radius);

        if (m_courseToRace == null)
            Debug.LogError("no course added");

        switch (m_activityType)
        {
            case ActivityType.RACE:
                m_activity = new Race(m_courseToRace);
                break;
            case ActivityType.HIGH_JUMP:
                m_activity = new ActivityHighJump();
                break;
        }

    }

    private void OnEnable()
    {
        this.BindUntilDisable<Vehicle>(EventTags.Activity_RequestStart, RequestStart);
        this.BindUntilDestroy<Activity>(EventTags.Activity_OnFinish, ActivityFinished);
    }

    private void RequestStart(Vehicle vehicle)
    {
        if (!m_entrants.Contains(vehicle))
            return;

        if (m_activity == null || !m_activity.InProgress)
        {
            StartActivity();

            ToggleParticles(false);
        }
    }

    private void StartActivity()
    {
        switch (m_activityType)
        {
            case ActivityType.RACE:
                ((Race)m_activity).Start(m_entrants, m_laps);
                break;
            case ActivityType.HIGH_JUMP:
                ((ActivityHighJump)m_activity).Start(m_entrants, m_duration);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Vehicle tempVehicle = null;
        if (!UtilsGameplay.IsVehicle(other, out tempVehicle))
            return;

        m_entrants.Add(tempVehicle);

        Unibus.Dispatch<TriggerZoneVehiclePair>(EventTags.TriggerEn_ActivityStartZone, new TriggerZoneVehiclePair(this, tempVehicle));
    }

    private void OnTriggerExit(Collider other)
    {
        Vehicle tempVehicle = null;
        if (!UtilsGameplay.IsVehicle(other, out tempVehicle))
            return;

        m_entrants.Remove(tempVehicle);

        Unibus.Dispatch<TriggerZoneVehiclePair>(EventTags.TriggerEx_ActivityStartZone, new TriggerZoneVehiclePair(this, tempVehicle));
    }

    #region VIEW
    private void ActivityFinished(Activity activity)
    {
        if (m_activity != activity)
            return;

        ToggleParticles(true);
    }

    private void SetParticleRadius(float radius)
    {
        ParticleSystem.EmissionModule emissionModule = m_particleSystem.emission;
        float modifiedRate = 1.4f * (2 * Mathf.PI * m_radius);
        emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(modifiedRate);
        ParticleSystem.ShapeModule shapeModule = m_particleSystem.shape;
        shapeModule.radius = radius;
    }

    private void ToggleParticles(bool toggle)
    {
        ParticleSystem.EmissionModule emissionModule = m_particleSystem.emission;
        emissionModule.enabled = toggle;
    }
    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_gizmoColor = Color.cyan;
    }
#endif
}
