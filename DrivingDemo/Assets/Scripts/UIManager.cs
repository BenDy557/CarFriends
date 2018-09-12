using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas m_canvas;

    [SerializeField]
    private Text m_speedText;

    [SerializeField]
    private WheelDisplay m_wheelDisplayPrefab;


    [SerializeField]
	private Vehicle m_vehicle;


    private void Start()
    {
        /*
        //front left
        WheelDisplay tempWheelDisplay = Instantiate(m_wheelDisplayPrefab, m_canvas.transform);
        tempWheelDisplay.transform.localPosition = tempWheelDisplay.transform.localPosition + new Vector3(0f, 0f);
        tempWheelDisplay.Wheel = m_vehicle.Engine.Wheels[0];
        //front right
        tempWheelDisplay = Instantiate(m_wheelDisplayPrefab, m_canvas.transform);
        tempWheelDisplay.transform.localPosition = tempWheelDisplay.transform.localPosition + new Vector3(100f, 0f);
		tempWheelDisplay.Wheel = m_vehicle.Engine.Wheels[1];

        //back left
        tempWheelDisplay = Instantiate(m_wheelDisplayPrefab, m_canvas.transform);
        tempWheelDisplay.transform.localPosition = tempWheelDisplay.transform.localPosition + new Vector3(0f, -200f);
		tempWheelDisplay.Wheel = m_vehicle.Engine.Wheels[2];
        //back right
        tempWheelDisplay = Instantiate(m_wheelDisplayPrefab, m_canvas.transform);
        tempWheelDisplay.transform.localPosition = tempWheelDisplay.transform.localPosition + new Vector3(100f, -200f);
		tempWheelDisplay.Wheel = m_vehicle.Engine.Wheels[3];
        */

        //Spawn wheel displays for each wheel
    }

    private void Update()
    {
        m_speedText.text = "KPH: " + Mathf.RoundToInt((m_vehicle.Engine.Speed * 60f * 60f) * 0.001f);
    }
}
