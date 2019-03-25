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
    [SerializeField]
    private float m_duration;
    [SerializeField]
    private float m_timer;

    public enum ScaleType
    {
        MULTIPLY,
        ADD,
        SET,
    }
    [SerializeField]
    private ScaleType m_scaleType;
    [SerializeField]
    private float m_engineModifier;
    private bool m_isFinished;
    public bool IsFinished { get {return m_isFinished;}}

    #region GeneralMethods
    public void Start(Vehicle vehicle)
    {
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
