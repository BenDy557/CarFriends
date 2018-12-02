using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetObject : MonoBehaviour
{
    [SerializeField]
    private int m_ID = -1;
    public int ID { get { return m_ID; } }

    [SerializeField]
    private bool m_isNetworkControlled = false;
    public bool IsNetworkControlled { get { return m_isNetworkControlled; } }

    public void Init(int id, bool isNetworkControlled)
    {
        m_ID = id;
        m_isNetworkControlled = isNetworkControlled;
    }
}
