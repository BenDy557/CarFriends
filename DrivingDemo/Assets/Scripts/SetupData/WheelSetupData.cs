using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WheelData", menuName = "Wheel", order = 2)]
public class WheelSetupData : ScriptableObject
{
    [SerializeField, NaughtyAttributes.BoxGroup("Wheels")]
    private WheelData m_wheelData;
    public WheelData WheelData { get { return m_wheelData; } }
}
