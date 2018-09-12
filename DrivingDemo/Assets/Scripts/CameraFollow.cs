using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public GameObject mObjectToFollow;
    public GameObject mObjectToLookAt;

    void Update ()
	{
		transform.position = mObjectToFollow.transform.position;
        transform.LookAt(mObjectToLookAt.transform);
	}
}
