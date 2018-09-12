using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It is used to rotate an object on Z axis
/// </summary>

public class RotateAround : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, Time.deltaTime * 300);
    }
}
