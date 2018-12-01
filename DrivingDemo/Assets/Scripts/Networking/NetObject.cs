using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetObject : MonoBehaviour
{
    [SerializeField]
    private int m_ID = -1;
    public int ID { get { return m_ID; } }
}
