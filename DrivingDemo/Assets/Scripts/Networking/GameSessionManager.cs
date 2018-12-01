using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class GameSessionManager : Singleton<GameSessionManager>
{
    private Host m_host;
    private Client m_client;
    private bool SessionStarted { get { return !(m_host == null && m_client == null); } }

    [Button]
    private void StartServer()
    {
        if (SessionStarted)
            return;

        m_host = gameObject.AddComponent<Host>();
    }

    [Button]
    private void StartClient()
    {
        if (SessionStarted)
            return;

        m_client = gameObject.AddComponent<Client>();
    }


    [Button]
    private void StopSession()
    {
        if (m_host != null)
        {
            m_host.Close();
        }

        if (m_client != null)
        {
            m_client.Close();
        }
    }
}
