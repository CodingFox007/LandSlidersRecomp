using UnityEngine;

public class Tree : MonoBehaviour
{
	[SerializeField]
	private float m_minSpeedBeforeTorque = 15f;

	[SerializeField]
	private float m_minTorque = 100f;

	[SerializeField]
	private float m_maxTorque = 1000f;

	[SerializeField]
	private float m_maxSpeedForTorque = 50f;

	[SerializeField]
	private float m_driveMaxVel = 10f;

	[SerializeField]
	private float m_driveMinForce = 5f;

	[SerializeField]
	private float m_driveMaxForce = 500f;

	[SerializeField]
	private bool m_reverse;

	[SerializeField]
	private RandomRange m_mass;

	private ConstantForce m_constantForce;

	private ConfigurableJoint m_joint;

	private void Awake()
	{
		m_joint = GetComponent<ConfigurableJoint>();
		m_constantForce = GetComponent<ConstantForce>();
		m_mass.Randomize();
		GetComponent<Rigidbody>().mass = Mathf.Max(0.1f, m_mass.Value);
	}

	private void FixedUpdate()
	{
		Vector3 velocity = Singleton<Game>.Instance.Player.Rigidbody.velocity;
		velocity.y = 0f;
		float magnitude = velocity.magnitude;
		if (magnitude > m_minSpeedBeforeTorque)
		{
			Vector3 torque = m_constantForce.torque;
			torque = velocity;
			torque.y = 0f;
			float x = torque.x;
			torque.x = 0f - torque.z;
			torque.z = x;
			torque = torque.normalized * Mathf.Lerp(m_minTorque, m_maxTorque, magnitude / m_maxSpeedForTorque);
			if (m_reverse)
			{
				m_constantForce.torque = -torque;
			}
			else
			{
				m_constantForce.torque = torque;
			}
		}
		else
		{
			m_constantForce.torque = Vector3.zero;
		}
		JointDrive slerpDrive = m_joint.slerpDrive;
		if (magnitude > m_driveMaxVel)
		{
			slerpDrive.maximumForce = m_driveMinForce;
		}
		else
		{
			slerpDrive.maximumForce = Mathf.Lerp(m_driveMaxForce, m_driveMinForce, magnitude / m_driveMaxVel);
		}
		m_joint.slerpDrive = slerpDrive;
	}

	private void OnEnable()
	{
		ObjectRegistry.AddObject("tree", base.gameObject);
	}

	private void OnDisable()
	{
		ObjectRegistry.RemoveObject("tree", base.gameObject);
	}
}
