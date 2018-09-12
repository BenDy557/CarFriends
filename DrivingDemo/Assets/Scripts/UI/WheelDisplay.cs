using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelDisplay : MonoBehaviour
{
    private Wheel m_wheel;
    public Wheel Wheel { set { m_wheel = value; } }

    [SerializeField]
    private Text m_nameText;
    [SerializeField]
    private Text m_RPMText;
    [SerializeField]
    private Image m_slipIndicator;

    void Start()
    {
		m_nameText.text = m_wheel.Collider.name;
    }

    // Update is called once per frame
    void Update()
    {
		m_RPMText.text = "FL " + Mathf.RoundToInt(m_wheel.Collider.rpm);

        Vector3 slipOffset = new Vector3();

        WheelHit tempWheelHit;
		if (m_wheel.Collider.GetGroundHit(out tempWheelHit))
        {
            slipOffset.x = tempWheelHit.sidewaysSlip;
            slipOffset.y = tempWheelHit.forwardSlip;
        }

        m_slipIndicator.rectTransform.localPosition = slipOffset * 50f;
    }
}
