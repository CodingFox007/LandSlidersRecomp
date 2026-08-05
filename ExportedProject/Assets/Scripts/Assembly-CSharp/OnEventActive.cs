using UnityEngine;
using UnityEngine.Events;

public class OnEventActive : MonoBehaviour
{
	[SerializeField]
	private string m_eventId;

	[SerializeField]
	private string m_eventGroupId;

	[SerializeField]
	private UnityEvent m_onActive = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onInactive = new UnityEvent();

	private void OnEnable()
	{
		EventCheck();
		Singleton<Game>.Instance.EventManager.OnEventTriggered += EventActive;
		Singleton<Game>.Instance.EventManager.OnEventAvailable += EventActive;
	}

	private void OnDisable()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.EventManager != null)
		{
			Singleton<Game>.Instance.EventManager.OnEventTriggered -= EventActive;
			Singleton<Game>.Instance.EventManager.OnEventAvailable -= EventActive;
		}
	}

	private void EventActive(string id)
	{
		EventCheck();
	}

	public void EventCheck()
	{
		bool flag = false;
		if (m_eventGroupId != null && m_eventGroupId != string.Empty)
		{
			if (Singleton<Game>.Instance.EventManager.IsEventGroupActive(m_eventGroupId))
			{
				flag = true;
			}
		}
		else if (Singleton<Game>.Instance.EventManager.IsEventActive(m_eventId))
		{
			flag = true;
		}
		if (flag)
		{
			m_onActive.Invoke();
		}
		else
		{
			m_onInactive.Invoke();
		}
	}
}
