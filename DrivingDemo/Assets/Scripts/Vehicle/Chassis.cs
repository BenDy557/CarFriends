using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chassis : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_forwardAxleOffset = Vector3.forward;
	public Vector3 ForwardAxleOffset { get { return m_forwardAxleOffset; } }
	[SerializeField]
	private Vector3 m_rearAxleOffset = Vector3.back;
	public Vector3 RearAxleOffset { get { return m_rearAxleOffset; } }

    [SerializeField]
    private Vector3 m_centerOfMass = Vector3.zero;
    public Vector3 CentreOfMass { get { return m_centerOfMass; } }

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.matrix = transform.localToWorldMatrix;//worldToLocalMatrix;

		Gizmos.DrawLine(m_forwardAxleOffset + Vector3.left, m_forwardAxleOffset + Vector3.right);
		Gizmos.DrawWireSphere(m_forwardAxleOffset + Vector3.left,0.3f);
		Gizmos.DrawWireSphere(m_forwardAxleOffset + Vector3.right,0.3f);

		Gizmos.DrawLine(m_rearAxleOffset + Vector3.left, m_rearAxleOffset + Vector3.right);
		Gizmos.DrawWireSphere(m_rearAxleOffset + Vector3.left,0.3f);
		Gizmos.DrawWireSphere(m_rearAxleOffset + Vector3.right,0.3f);


        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;//worldToLocalMatrix;

        Gizmos.DrawWireSphere(m_centerOfMass, 0.3f);
        Debug.DrawLine(transform.position + m_centerOfMass - Vector3.up, transform.position + m_centerOfMass + Vector3.up,Color.red);
        Debug.DrawLine(transform.position + m_centerOfMass - Vector3.left, transform.position + m_centerOfMass + Vector3.left, Color.red);
        Debug.DrawLine(transform.position + m_centerOfMass - Vector3.forward, transform.position + m_centerOfMass + Vector3.forward, Color.red);
    }
}
