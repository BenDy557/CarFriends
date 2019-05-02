using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas m_canvas;
    [SerializeField]
    private Button m_defaultButton;
    


    private void Start()
    {
        m_defaultButton.Select();
    }

    
}
