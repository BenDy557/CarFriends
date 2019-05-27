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

    public enum Units
    {
        MS,
        KPH,
        MPH,
    }

    public static float UnitConverter(Units unitIn, float value, Units unitOut)
    {
        if (unitIn == unitOut)
            return value;

        switch (unitIn)
        {
            case Units.MS:
                switch (unitOut)
                {
                    case Units.KPH:
                        return (value * 60f * 60f) * 0.001f;
                        break;
                    case Units.MPH:
                        throw new System.NotImplementedException();
                        break;
                }
                break;

            case Units.KPH:
                switch (unitOut)
                {
                    case Units.MS:
                        throw new System.NotImplementedException();
                        break;
                    case Units.MPH:
                        throw new System.NotImplementedException();
                        break;
                }
                break;

            case Units.MPH:
                switch (unitOut)
                {
                    case Units.MS:
                        throw new System.NotImplementedException();
                        break;
                    case Units.KPH:
                        throw new System.NotImplementedException();
                        break;
                }
                break;
        }


        return 0f;
    }

}

public static class UtilsGameplay
{
    //used in collision detection to check if collider belongs to a vehicle and also returns any vehicle found
    public static bool IsVehicle(Collider collider, out Vehicle vehicle)
    {
        vehicle = null;

        if (collider.tag != "Hull")
            return false;

        Hull tempHull = collider.GetComponent<Hull>();
        if (tempHull == null)
        {
            Debug.LogError("hull not attached to tagged hull" + collider.name, collider.gameObject);
            return false;
        }

        vehicle = tempHull.Owner;
        return true;
    }
}