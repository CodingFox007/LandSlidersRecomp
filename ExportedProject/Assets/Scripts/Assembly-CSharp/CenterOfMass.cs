using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
	[SerializeField]
	private Vector3 m_position;

	private void Start()
	{
		base.gameObject.GetComponent<Rigidbody>().centerOfMass = m_position;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawSphere(m_position, 0.1f);
	}

	private void OnEnable()
	{
		base.gameObject.GetComponent<Rigidbody>().centerOfMass = m_position;
	}
}
