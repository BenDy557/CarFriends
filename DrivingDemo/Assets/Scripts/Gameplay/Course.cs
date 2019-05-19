using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A course is a collection of checkpoints, it can be created manually by placing checkpoints in 
/// </summary>
public class Course : MonoBehaviour
{
    [SerializeField,ReorderableList]
    private List<Checkpoint> m_checkpoints;
    public IList<Checkpoint> Checkpoints { get { return m_checkpoints.AsReadOnly(); } }

    [SerializeField]
    private bool m_loop = false;

    public Checkpoint GetFirstCheckpoint()
    {
        if (m_checkpoints.IsNullOrEmpty())
        {
            Debug.LogError("Could not find first checkpoint");
            return null;
        }

        return m_checkpoints[0];
    }

    public Checkpoint GetNextCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint == null)
            return null;

        if (m_checkpoints.IsNullOrEmpty() || !m_checkpoints.Contains(checkpoint))
        {
            Debug.LogError("Could not find next checkpoint");
            return null;
        }

        //increment index and loop round
        int index = m_checkpoints.IndexOf(checkpoint);
        index = index >= m_checkpoints.Count - 1 ? 0 : index + 1;
        return m_checkpoints[index];
    }

    public void AddCheckpoint(Checkpoint checkpoint)
    {
        m_checkpoints.Add(checkpoint);
    }

#if UNITY_EDITOR
    [Button]
    private void GetCheckpoints()
    {
        m_checkpoints.Clear();
        m_checkpoints.AddRange(GetComponentsInChildren<Checkpoint>());
    }

    [Button]
    private void AddCheckpoint()
    {
        Checkpoint tempCheckPoint = (PrefabUtility.InstantiatePrefab(Resources.Load(PrefabLibrary.CheckpointDefault), this.transform) as GameObject).GetComponent<Checkpoint>();
        AddCheckpoint(tempCheckPoint);

        if (m_checkpoints.IsNullOrEmpty() || m_checkpoints[0] == null)
            tempCheckPoint.transform.localPosition = Vector3.zero;
        else
            tempCheckPoint.transform.localPosition = m_checkpoints[0].transform.localPosition + Vector3.right;

    }

    private void OnDrawGizmosSelected()
    {
        if (m_checkpoints == null || m_checkpoints.Count == 0)
            return;

        Handles.color = new Color(1f, 0.96f, 0.016f, 0.4f);

        if (m_checkpoints[0] != null)
        {
            Vector3 pos0 = m_checkpoints[0].transform.position;
            Handles.DrawLine(pos0, pos0 + (Vector3.up*50f));
        }

        for (int i = 0; i < m_checkpoints.Count -1; i++)
        {
            if (m_checkpoints[i] == null)
                return;

            Vector3 pos0 = m_checkpoints[i].transform.position;
            Vector3 pos1 = m_checkpoints[i+1].transform.position;
            //Vector3 midPos = (pos0 + pos1) * 0.5f;
            Handles.DrawLine(pos0, pos1);
        }    

        if (m_loop)
        {
            Vector3 pos0 = m_checkpoints[m_checkpoints.Count - 1].transform.position;
            Vector3 pos1 = m_checkpoints[0].transform.position;
            //Vector3 midPos = (pos0 + pos1) * 0.5f;
            Handles.DrawLine(pos0, pos1);
        }

    }
#endif

}
