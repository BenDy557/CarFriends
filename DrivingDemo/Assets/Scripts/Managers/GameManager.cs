﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Vehicle> participants;
    public Race firstRace;

    public Course firstCourse;
    public int laps = 1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("StartRace");

            firstRace = new Race(participants, firstCourse, laps);
            firstRace.Start();
        }
    }

}
