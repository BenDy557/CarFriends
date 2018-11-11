using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class AITarget : MonoBehaviour
{
    [SerializeField]
    private float m_activationDistance = 20f;

    [SerializeField]
    private Transform m_agent;

    [SerializeField]
    private SplineFollower m_follower;

    [SerializeField]
    private float m_minLaneDuration = 3f;
    [SerializeField]
    private float m_maxLaneDuration = 12f;

    private float m_laneSwitchTimer = 0f;
    private float m_laneSwitchDuration = 3f;

    [SerializeField]
    private float m_laneWidth = 10f;
    [SerializeField]
    private float m_maxLaneChangeDifference = 0.5f;

    private Vector2 m_currentLaneOffset = Vector2.zero;

    [SerializeField]
    private GameObject m_actualTarget;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        double currentProgress = m_follower.clampedPercent;
        m_laneSwitchTimer += Time.deltaTime;

        if (m_laneSwitchTimer >= m_laneSwitchDuration)
        {
            m_laneSwitchTimer = 0;
            m_laneSwitchDuration = Random.Range(m_minLaneDuration, m_maxLaneDuration);

            m_currentLaneOffset.x = Mathf.Clamp(m_currentLaneOffset.x + Random.Range(-m_maxLaneChangeDifference, m_maxLaneChangeDifference), -m_laneWidth / 2f, m_laneWidth / 2f);
            m_currentLaneOffset.y = Mathf.Clamp(m_currentLaneOffset.y + Random.Range(-m_maxLaneChangeDifference, m_maxLaneChangeDifference), -m_laneWidth / 2f, m_laneWidth / 2f);
            m_actualTarget.transform.localPosition = new Vector3(m_currentLaneOffset.x, 0, m_currentLaneOffset.y);
            //m_follower.motion.offset = new Vector2(m_currentLaneOffset, 0f);
            //m_follower.motion.offset = new Vector2(m_follower.motion.offset.x, m_follower.motion.offset.y);
            //m_follower.motion = new Vector2(m_currentLaneOffset, 0f);
            //m_follower.motion.offset.Set(m_currentLaneOffset, 0);
            //m_follower.Move(currentProgress);
        }

        if (Vector3.Distance(m_agent.transform.position, transform.position) < m_activationDistance)
        {
            m_follower.Move(m_follower.followSpeed * Time.deltaTime);
        }
	}
}
