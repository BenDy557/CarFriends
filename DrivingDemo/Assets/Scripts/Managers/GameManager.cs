using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public List<Vehicle> participants;
    public Race firstRace;

    public Course firstCourse;
    public int laps = 1;

    private void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("StartRace");

            firstRace = new Race(participants, firstCourse, laps);
            firstRace.Start();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Plus))
        {
            if (Time.timeScale < 2f)
            {
                Debug.Log("speed up");
                Time.timeScale += 0.05f;
            }
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (Time.timeScale > 0f)
            {
                Debug.Log("slow down");
                Time.timeScale -= 0.05f;
            }
        }
    }
}