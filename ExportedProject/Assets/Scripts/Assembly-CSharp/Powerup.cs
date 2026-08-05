using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Powerup : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private string m_id;

	[SerializeField]
	private UnityEvent m_onActivated = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onDeactivated = new UnityEvent();

	private bool m_active;

	protected bool m_isSilent;

	public string Id
	{
		get
		{
			return m_id;
		}
	}

	public bool IsSilent
	{
		set
		{
			m_isSilent = value;
		}
	}

	public bool IsActive
	{
		get
		{
			return m_active;
		}
		set
		{
			if (m_active != value)
			{
				if (value)
				{
					Activate();
				}
				else
				{
					Deactivate();
				}
			}
		}
	}

	public virtual void Activate()
	{
		if (!m_active)
		{
			m_active = true;
			if (!m_isSilent)
			{
				m_onActivated.Invoke();
			}
		}
	}

	public virtual void Deactivate()
	{
		if (m_active)
		{
			m_active = false;
			if (!m_isSilent)
			{
				m_onDeactivated.Invoke();
			}
		}
	}

	public virtual void Update()
	{
	}
}
