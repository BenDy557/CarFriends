using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockAbsorberFollow : MonoBehaviour
{
	//public GameObject mShockAbsorber;
	public WheelCollider mWheel;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (mWheel == null)
			return;

		transform.up = transform.parent.transform.up;
		Vector3 tWheelPos;
		Quaternion tWheelRotation;
		mWheel.GetWorldPose (out tWheelPos,out tWheelRotation);
		transform.LookAt (tWheelPos);
	}
}
