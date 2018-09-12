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
	}
}
