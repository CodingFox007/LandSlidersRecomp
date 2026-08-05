using UnityEngine;

public class RotateWithSpeed : MonoBehaviour
{
	[SerializeField]
	private HingeJoint m_hingeJoint;

	[SerializeField]
	private float m_minPlayerSpeed;

	[SerializeField]
	private float m_maxPlayerSpeed;

	[SerializeField]
	private float m_minMotorSpeed;

	[SerializeField]
	private float m_maxMotorSpeed;

	private JointMotor motor;

	private Vector3 axis;

	private Vector3 oldPos;

	private Vector3 newPos;

	private void Start()
	{
		if (m_hingeJoint == null)
		{
			base.enabled = false;
		}
		motor = m_hingeJoint.motor;
		motor.targetVelocity = m_minMotorSpeed;
		m_hingeJoint.motor = motor;
		oldPos = base.transform.position;
		newPos = oldPos;
	}

	private void Update()
	{
		if (!(m_hingeJoint == null))
		{
			newPos = base.transform.position;
			float magnitude = (oldPos - newPos).magnitude;
			float t = 0f;
			if (magnitude > m_minPlayerSpeed)
			{
				t = Mathf.Min(1f, (magnitude - m_minPlayerSpeed) / m_maxPlayerSpeed);
			}
			motor.targetVelocity = Mathf.Lerp(m_minMotorSpeed, m_maxMotorSpeed, t);
			m_hingeJoint.motor = motor;
			oldPos = base.transform.position;
		}
	}
}
