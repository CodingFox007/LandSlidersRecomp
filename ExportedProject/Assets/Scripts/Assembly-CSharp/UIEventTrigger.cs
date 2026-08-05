using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIEventTrigger : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private UnityEvent m_onTrigger = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onUntrigger = new UnityEvent();

	public void Trigger()
	{
		m_onTrigger.Invoke();
	}

	public void Untrigger()
	{
		m_onUntrigger.Invoke();
	}
}
