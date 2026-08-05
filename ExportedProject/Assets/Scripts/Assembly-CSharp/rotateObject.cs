using UnityEngine;

public class rotateObject : MonoBehaviour
{
	[SerializeField]
	private float m_speed;

	[SerializeField]
	private Vector3 m_rotationAxis;

	[SerializeField]
	private bool m_offset;

	private Vector3 oldParPos = Vector3.zero;

	private Vector3 newParPos = Vector3.zero;

	private Vector3 offset = Vector3.zero;

	private void Update()
	{
		newParPos = base.transform.parent.localEulerAngles;
		offset = newParPos - oldParPos;
		if (m_offset)
		{
			base.transform.Rotate(m_rotationAxis * m_speed - offset);
		}
		else
		{
			base.transform.Rotate(m_rotationAxis * m_speed);
		}
		oldParPos = base.transform.parent.localEulerAngles;
	}
}
