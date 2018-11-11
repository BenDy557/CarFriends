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
}
