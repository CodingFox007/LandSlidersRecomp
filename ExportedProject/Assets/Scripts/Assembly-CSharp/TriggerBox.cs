using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TriggerBox : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private UnityEvent m_onTrigger = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onUntrigger = new UnityEvent();

	[SerializeField]
	private bool m_triggerOnce = true;

	private int m_triggerCount;

	[SerializeField]
	private bool m_untriggerOnce;

	private int m_untriggerCount;

	[SerializeField]
	private bool m_killTriggerer;

	private void OnTriggerEnter(Collider other)
	{
		if (Singleton<Game>.Instance.LevelManager.CurrentLevel.IsComplete && (!(other.transform.parent != null) || !(other.transform.parent.GetComponent<AcidBall>() != null)) && (!(other.transform.parent != null) || !(other.transform.parent.GetComponent<Snowball>() != null)) && (!m_triggerOnce || m_triggerCount <= 0))
		{
			if (m_killTriggerer)
			{
				KillTriggerer(other.gameObject);
			}
			m_triggerCount++;
			m_onTrigger.Invoke();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((!(other.transform.parent != null) || !(other.transform.parent.GetComponent<AcidBall>() != null)) && (!(other.transform.parent != null) || !(other.transform.parent.GetComponent<Snowball>() != null)) && (!m_untriggerOnce || m_untriggerCount <= 0))
		{
			if (m_killTriggerer)
			{
				KillTriggerer(other.gameObject);
			}
			m_untriggerCount++;
			m_onUntrigger.Invoke();
		}
	}

	private void KillTriggerer(GameObject triggerer)
	{
		Entity entity = triggerer.GetComponent<Entity>();
		if (entity == null)
		{
			entity = triggerer.GetComponentInParent<Entity>();
		}
		if (entity != null)
		{
			entity.Kill(base.gameObject);
		}
	}
}
