using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TriggerRandom : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private List<UnityEvent> m_events = new List<UnityEvent>();

	public void Trigger()
	{
		Vector3 position = base.transform.position;
		int index = Random.Range(0, m_events.Count);
		m_events[index].Invoke();
		base.transform.position = position;
	}
}
