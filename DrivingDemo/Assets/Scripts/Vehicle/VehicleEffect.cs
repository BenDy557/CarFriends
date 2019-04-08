using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public class VehicleEffect
{
    public enum EffectType
    {
        NONE,
        ENGINE_POWER,
        GRIP_POWER,
        GRAVITY_STRENGTH,
    }

    private Vehicle m_owner = null;
    [SerializeField]
    private EffectType m_type = EffectType.NONE;
    public EffectType Type { get { return m_type; } }
    [SerializeField]
    private float m_duration;
    public float Duration { get { return m_duration; } }
    private float m_timer = 0f;

    public enum ScaleType
    {
        MULTIPLY,
        ADD,
        SET,
    }
    [SerializeField]
    private ScaleType m_scaleType;
    public ScaleType ScaleModifierType { get { return m_scaleType; } }
    [SerializeField]
    private float m_engineModifier;
    public float EngineModifier { get { return m_engineModifier; } }


    private bool m_isFinished = false;
    public bool IsFinished { get {return m_isFinished;}}


    #region GeneralMethods
    public VehicleEffect(VehicleEffect effectIn)
    {
        m_type = effectIn.Type;
        m_duration = effectIn.Duration;
        m_scaleType = effectIn.ScaleModifierType;
        m_engineModifier = effectIn.m_engineModifier;
    }

    public void Start(Vehicle vehicle)
    {
        m_isFinished = false;
        m_owner = vehicle;

        m_timer = m_duration;
        switch (m_type)
        {
            case EffectType.ENGINE_POWER:
                OnStartEnginePower();
                break;
            default:
                Debug.LogError("Incorrect Effect setup");
                break;
        }
    }

    public void Update()
    {
        m_timer -= Time.deltaTime;

        if (m_timer <= 0)
        {
            End();
            m_isFinished = true;
        }
    }

    public void End()
    {
        switch (m_type)
        {
            case EffectType.ENGINE_POWER:
                OnEndEnginePower();
                break;
            default:
                Debug.LogError("Incorrect Effect setup");
                break;
        }
    }
    #endregion

    #region EffectSpecific
    private void OnStartEnginePower()
    {
        switch (m_scaleType)
        {
            case ScaleType.MULTIPLY:
                throw new System.NotImplementedException();
                break;
            case ScaleType.ADD:
                Debug.LogWarning("BAD CODE");
                m_owner.MaxEngineTorque += m_engineModifier;
                break;
            case ScaleType.SET:
                throw new System.NotImplementedException();
                break;
        }
    }

    private void OnEndEnginePower()
    {
        switch (m_scaleType)
        {
            case ScaleType.MULTIPLY:
                throw new System.NotImplementedException();
                break;
            case ScaleType.ADD:
                Debug.LogWarning("BAD CODE");
                m_owner.MaxEngineTorque -= m_engineModifier;
                break;
            case ScaleType.SET:
                throw new System.NotImplementedException();
                break;
        }
    }
    #endregion
}
