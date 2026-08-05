using UnityEngine;

public class MotorOscillator : MonoBehaviour
{
	[SerializeField]
	private HingeJoint m_hingeJoint;

	[SerializeField]
	private float m_minRotation;

	[SerializeField]
	private float m_maxRotation;

	[SerializeField]
	private float m_force;

	[SerializeField]
	private GameObject m_body;

	[SerializeField]
	private float m_targetVelocity;

	private JointMotor motor;

	private Vector3 axis;

	private void Start()
	{
		motor = m_hingeJoint.motor;
		motor.force = m_force;
		motor.targetVelocity = m_targetVelocity;
		m_hingeJoint.motor = motor;
		axis = m_hingeJoint.axis;
	}

	private void Update()
	{
		if (m_body == null || m_hingeJoint == null)
		{
			return;
		}
		if (axis.y == 1f)
		{
			float num = m_body.transform.localRotation.eulerAngles.y;
			if (num > 180f)
			{
				num -= 360f;
			}
			if (num < m_minRotation)
			{
				motor.targetVelocity = m_targetVelocity;
				m_hingeJoint.motor = motor;
			}
			if (num > m_maxRotation)
			{
				motor.targetVelocity = 0f - m_targetVelocity;
				m_hingeJoint.motor = motor;
			}
		}
		if (axis.z == 1f)
		{
			float num2 = m_body.transform.localRotation.eulerAngles.z;
			if (num2 > 180f)
			{
				num2 -= 360f;
			}
			if (num2 < m_minRotation)
			{
				motor.targetVelocity = m_targetVelocity;
				m_hingeJoint.motor = motor;
			}
			if (num2 > m_maxRotation)
			{
				motor.targetVelocity = 0f - m_targetVelocity;
				m_hingeJoint.motor = motor;
			}
		}
	}
}
