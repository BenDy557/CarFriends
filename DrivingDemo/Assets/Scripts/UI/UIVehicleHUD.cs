using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIVehicleHUD : MonoBehaviour
{
    private enum Units
    {
        MS,
        KPH,
        MPH,
    }

    //[SerializeField]
    private Vehicle m_vehicle;//TODO// should this be some sort of player class as opposed to referencing a vehicle? it technically is associated with the camera more than the vehicle
    private Camera m_camera;

    [SerializeField]
    private Canvas m_canvas;


    [SerializeField]
    private TextMeshProUGUI m_speedometer;
    [SerializeField]
    private Units m_units = Units.MS;

    private void Start()
    {
        Debug.LogWarning("BAD CODE");
        m_canvas.worldCamera = m_camera;
    }

    private void Update()
    {
        float speed = UnitConverter(Units.MS, m_vehicle.Engine.Speed, m_units);
        m_speedometer.text = Mathf.RoundToInt(speed).ToString() + m_units.ToString();
    }

    public void Initialise(Vehicle vehichle, Camera camera)
    {
        Debug.Log("Camera" + camera.gameObject.name, camera.gameObject);

        m_vehicle = vehichle;
        m_camera = camera;
    }


    private float UnitConverter(Units unitIn, float value, Units unitOut)
    {
        if (unitIn == unitOut)
            return value;

        switch (unitIn)
        {
            case Units.MS:
                switch (unitOut)
                {
                    case Units.KPH:
                        return (value * 60f * 60f) *0.001f;
                        break;
                    case Units.MPH:
                        throw new System.NotImplementedException();
                        break;
                }
                break;
            
            case Units.KPH:
                switch (unitOut)
                {
                    case Units.MS:
                        throw new System.NotImplementedException();
                        break;
                    case Units.MPH:
                        throw new System.NotImplementedException();
                        break;
                }
                break;
            
            case Units.MPH:
                switch (unitOut)
                {
                    case Units.MS:
                        throw new System.NotImplementedException();
                        break;
                    case Units.KPH:
                        throw new System.NotImplementedException();
                        break;
                }
                break;
        }


        return 0f;
    }
}
