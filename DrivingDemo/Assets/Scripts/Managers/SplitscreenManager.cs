using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitscreenManager : Singleton<SplitscreenManager>
{
    public List<Camera> m_cameras;

    public void AddScreen(Camera camera)
    {
        m_cameras.Add(camera);

        RefreshScreen();
    }

    private void RefreshScreen()
    {
        switch (m_cameras.Count)
        {
            case 0:
                Debug.LogError("no cameras");
                break;
            case 1:
                {
                    Rect bounds0 = new Rect(0f, 0f, 1f, 1f);
                    m_cameras[0].rect = bounds0;
                }

                break;
            case 2:
                {
                    Rect bounds0 = new Rect(0f, 0.5f, 1f, 0.5f);
                    m_cameras[0].rect = bounds0;

                    Rect bounds1 = new Rect(0f, 0f, 1f, 0.5f);
                    m_cameras[1].rect = bounds1;
                }
                break;
            case 3:
                {
                    Rect bounds0 = new Rect(0f, 0.5f, 0.5f, 0.5f);
                    m_cameras[0].rect = bounds0;

                    Rect bounds1 = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    m_cameras[1].rect = bounds1;

                    Rect bounds2 = new Rect(0f, 0f, 1f, 0.5f);
                    m_cameras[2].rect = bounds2;
                }
                break;
            case 4:
                {
                    Rect bounds0 = new Rect(0f, 0.5f, 0.5f, 0.5f);
                    m_cameras[0].rect = bounds0;

                    Rect bounds1 = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    m_cameras[1].rect = bounds1;

                    Rect bounds2 = new Rect(0f, 0f, 0.5f, 0.5f);
                    m_cameras[2].rect = bounds2;

                    Rect bounds3 = new Rect(0.5f, 0f, 0.5f, 0.5f);
                    m_cameras[3].rect = bounds3;
                }
                break;
        }
    }
}
