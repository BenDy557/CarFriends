using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetObject : MonoBehaviour
{
    [SerializeField]
    private int m_ID = -1;
    public int ID { get { return m_ID; } }

    private static HashSet<int> m_networkIDs = new HashSet<int>();

    [SerializeField]
    private bool m_isNetworkControlled = false;
    public bool IsNetworkControlled { get { return m_isNetworkControlled; } }

    /// <summary>
    /// initialises the networkObject, allows you to specify an ID or have one generated
    /// the funtion returns the new id of the networkObject
    /// </summary>
    /// <returns>The init.</returns>
    /// <param name="isNetworkControlled">If set to <c>true</c> is network controlled.</param>
    /// <param name="id">Identifier.</param>
    public int Init(bool isNetworkControlled, int id = -1)
    {
        int tempID = -1;

        if (id == -1)
            tempID = GetNewID();
        else
            tempID = id;

        if (m_networkIDs.Contains(tempID))
        {
            Debug.LogError("network ID already exists");
            tempID = -1;
        }
        else
            m_networkIDs.Add(tempID);


        m_ID = tempID;
        m_isNetworkControlled = isNetworkControlled;

        return m_ID;
    }

    //Should always be private
    private int GetNewID()
    {
        while (true)
        {
            int tempValue = Random.Range(int.MinValue, int.MaxValue);
           
            if (m_networkIDs.Contains(tempValue) || tempValue == -1)
                continue;

            return tempValue;
        }
    }
}
