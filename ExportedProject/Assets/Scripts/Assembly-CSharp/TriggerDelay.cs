using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TriggerDelay : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private UnityEvent m_onTimeReached = new UnityEvent();

	[SerializeField]
	private float m_triggerDelay;

	private float m_timer;

	private bool m_active;

	[SerializeField]
	private bool m_waitForPlayerToStopMoving;

	private void Update()
	{
		if (m_active)
		{
			if (!m_waitForPlayerToStopMoving || Singleton<Game>.Instance.Player.Rigidbody.velocity.magnitude < 3f)
			{
				m_timer += Time.deltaTime;
			}
			else
			{
				m_timer = 0f;
			}
			if (m_timer > m_triggerDelay)
			{
				m_timer = 0f;
				m_active = false;
				m_onTimeReached.Invoke();
			}
		}
	}

	public void Trigger()
	{
		if (!m_active)
		{
			m_timer = 0f;
		}
		m_active = true;
	}

	public void UnTrigger()
	{
		m_active = false;
		m_timer = 0f;
	}
}
