using UnityEngine;

public class SpringSwitch : MonoBehaviour
{
	[SerializeField]
	private HingeJoint[] m_hingeJoints;

	[SerializeField]
	private float m_onMin;

	[SerializeField]
	private float m_onMax;

	[SerializeField]
	private float m_offMin;

	[SerializeField]
	private float m_offMax;

	private float onTimer;

	private float offTimer;

	private bool on = true;

	private void Start()
	{
		onTimer = Random.Range(m_onMin, m_onMax);
		offTimer = Random.Range(m_offMin, m_offMax);
	}

	private void Update()
	{
		HingeJoint[] hingeJoints = m_hingeJoints;
		foreach (HingeJoint hingeJoint in hingeJoints)
		{
			if (hingeJoint == null)
			{
				return;
			}
		}
		if (on)
		{
			onTimer -= Time.deltaTime;
			if (onTimer < 0f)
			{
				on = false;
				onTimer = Random.Range(m_onMin, m_onMax);
			}
			HingeJoint[] hingeJoints2 = m_hingeJoints;
			foreach (HingeJoint hingeJoint2 in hingeJoints2)
			{
				hingeJoint2.useSpring = true;
			}
		}
		else
		{
			offTimer -= Time.deltaTime;
			if (offTimer < 0f)
			{
				on = true;
				offTimer = Random.Range(m_offMin, m_offMax);
			}
			HingeJoint[] hingeJoints3 = m_hingeJoints;
			foreach (HingeJoint hingeJoint3 in hingeJoints3)
			{
				hingeJoint3.useSpring = false;
			}
		}
	}
}
