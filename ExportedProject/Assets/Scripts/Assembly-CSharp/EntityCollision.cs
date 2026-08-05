using UnityEngine;

public class EntityCollision : MonoBehaviour
{
	[SerializeField]
	private Entity m_parent;

	private void OnTriggerEnter(Collider other)
	{
		if (m_parent != null)
		{
			m_parent.OnTriggerChildEnter(base.gameObject, other);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (m_parent != null)
		{
			m_parent.OnCollisionChildEnter(base.gameObject, collision);
		}
	}
}
