using UnityEngine;

public class AttractEntity : MonoBehaviour
{
	[SerializeField]
	private string[] m_entityIds;

	[SerializeField]
	private bool m_triggerOnce = true;

	private int m_triggerCount;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag != "Entity" || (m_triggerOnce && m_triggerCount > 0))
		{
			return;
		}
		Debug.Log("OnTriggerEnter");
		Entity component = other.gameObject.GetComponent<Entity>();
		if (component == null && other.gameObject.transform.parent != null)
		{
			component = other.gameObject.transform.parent.gameObject.GetComponent<Entity>();
		}
		if (component == null)
		{
			return;
		}
		if (m_entityIds.Length > 0)
		{
			bool flag = false;
			string[] entityIds = m_entityIds;
			foreach (string text in entityIds)
			{
				if (text == component.Id)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
		}
		component.SetPreferredTarget(base.gameObject);
		m_triggerCount++;
	}
}
