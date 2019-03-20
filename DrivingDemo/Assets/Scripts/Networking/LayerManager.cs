using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : Singleton<LayerManager>
{
    [SerializeField]
    private LayerMask m_drivable;
    public LayerMask Drivable { get { return m_drivable; } }

}
