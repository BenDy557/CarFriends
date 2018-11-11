using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public GameObject mObjectToFollow;
    public GameObject mObjectToLookAt;

    void LateUpdate ()
	{
		transform.position = mObjectToFollow.transform.position;
        transform.LookAt(mObjectToLookAt.transform);
	}
}
