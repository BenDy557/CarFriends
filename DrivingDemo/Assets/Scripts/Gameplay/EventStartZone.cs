using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnibusEvent;

public class EventStartZone : TriggerZone
{
    [SerializeField]
    private Course m_courseToRace = null;
    [SerializeField]
    private int m_laps = 2;

    private Race m_activity;
    public Race Activity { get { return m_activity; } }

    private HashSet<Vehicle> m_entrants = new HashSet<Vehicle>();

    [SerializeField]
    private ParticleSystem m_particleSystem;

    protected override void Awake()
    {
        base.Awake();

        SetParticleRadius(m_radius);

        if (m_courseToRace == null)
            Debug.LogError("no course added");

        m_activity = new Race(m_courseToRace);
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
            m_activity.Start(m_entrants,m_laps);

            ToggleParticles(false);
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
