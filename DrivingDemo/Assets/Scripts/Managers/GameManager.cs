using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
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