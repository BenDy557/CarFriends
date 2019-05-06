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

    #region Color
    public static Color SetR(this Color color, float value)
    {
        Color tempColor = color;
        tempColor.r = value;
        return tempColor;
    }

    public static Color SetG(this Color color, float value)
    {
        Color tempColor = color;
        tempColor.g = value;
        return tempColor;
    }

    public static Color SetB(this Color color, float value)
    {
        Color tempColor = color;
        tempColor.b = value;
        return tempColor;
    }

    public static Color SetA(this Color color, float value)
    {
        Color tempColor = color;
        tempColor.a = value;
        return tempColor;
    }
    #endregion

    public static bool IsNullOrEmpty(this IList list)
    {
        return list == null || list.Count == 0;
    }

    #region Debug

    internal static void DrawCross(Vector3 m_spawnPosition, Color colorIn)
    {
        Debug.DrawLine(m_spawnPosition + (Vector3.left * 0.5f), m_spawnPosition + (Vector3.right * 0.5f), colorIn);
        Debug.DrawLine(m_spawnPosition + (Vector3.back * 0.5f), m_spawnPosition + (Vector3.forward * 0.5f), colorIn);
    }

    #endregion

    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    }
}
