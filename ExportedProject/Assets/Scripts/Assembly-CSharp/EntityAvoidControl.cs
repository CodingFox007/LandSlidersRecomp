using UnityEngine;

public class EntityAvoidControl : MonoBehaviour
{
	[SerializeField]
	private bool m_triggerOnce;

	private int m_triggerCount;

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.tag != "Entity") && (!m_triggerOnce || m_triggerCount <= 0))
		{
			Entity component = other.gameObject.GetComponent<Entity>();
			if (component == null && other.gameObject.transform.parent != null)
			{
				component = other.gameObject.transform.parent.gameObject.GetComponent<Entity>();
			}
			if (!(component == null))
			{
				component.AvoidBlockDisabled = true;
				m_triggerCount++;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!m_triggerOnce || m_triggerCount <= 0)
		{
			Entity component = other.gameObject.GetComponent<Entity>();
			if (component == null && other.gameObject.transform.parent != null)
			{
				component = other.gameObject.transform.parent.gameObject.GetComponent<Entity>();
			}
			if (!(component == null))
			{
				component.AvoidBlockDisabled = false;
			}
		}
	}
}
