using UnityEngine;

public class AttachToPlayer : MonoBehaviour
{
	[SerializeField]
	private float m_minDist;

	[SerializeField]
	private float m_spring = 100f;

	[SerializeField]
	private float m_damper = 1f;

	private void Awake()
	{
	}

	public void Attach()
	{
		SpringJoint springJoint = base.gameObject.AddComponent<SpringJoint>();
		springJoint.connectedBody = Singleton<Game>.Instance.Player.Rigidbody;
		springJoint.spring = m_spring;
		springJoint.damper = m_damper;
		springJoint.minDistance = m_minDist;
		springJoint.connectedAnchor = Vector3.zero;
	}
}
