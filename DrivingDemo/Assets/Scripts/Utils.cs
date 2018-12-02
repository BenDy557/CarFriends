using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 Flatten(this Vector3 vector)
    {
        Vector3 tempVector = vector;
        tempVector.y = 0;
        return tempVector;
    }

    public static bool IsNullOrEmpty(this IList list)
    {
        return list == null || list.Count == 0;
    }

    internal static void DrawCross(Vector3 m_spawnPosition,Color colorIn)
    {
        Debug.DrawLine(m_spawnPosition + (Vector3.left * 0.5f), m_spawnPosition + (Vector3.right * 0.5f), colorIn);
        Debug.DrawLine(m_spawnPosition + (Vector3.back * 0.5f), m_spawnPosition + (Vector3.forward * 0.5f), colorIn);
    }
}
