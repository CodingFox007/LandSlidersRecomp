using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIStateTransition : MonoBehaviour, IEventSystemHandler
{
	[Serializable]
	public class SpecialTransition
	{
		public string m_name;

		public UIController.State m_prevState;

		public UIController.State m_state;

		public bool m_overrideNormal;

		public UnityEvent m_onEnter = new UnityEvent();
	}

	[SerializeField]
	private bool m_assetActive = true;

	private bool m_active;

	[SerializeField]
	private List<UIController.State> m_activeStates = new List<UIController.State>();

	[SerializeField]
	private List<UIController.State> m_inactiveStates = new List<UIController.State>();

	[SerializeField]
	private UnityEvent m_toActive = new UnityEvent();

	[SerializeField]
	private UnityEvent m_toInactive = new UnityEvent();

	[SerializeField]
	private List<SpecialTransition> m_specialTransitions = new List<SpecialTransition>();

	[SerializeField]
	private string[] m_conditions;

	public bool Active
	{
		get
		{
			return m_active;
		}
	}

	private void Start()
	{
		m_active = m_assetActive;
		Singleton<Game>.Instance.UIController.OnStateChanged += OnUIStateChange;
	}

	public void OnUIStateChange(UIController.State prev, UIController.State curr)
	{
		bool active = m_active;
		if (HasSpecialState(prev, curr))
		{
			EnterSpecialState(prev, curr);
			if (SpecialStateOverride(prev, curr))
			{
				return;
			}
		}
		if (m_activeStates.Contains(curr) && !active)
		{
			string[] conditions = m_conditions;
			foreach (string text in conditions)
			{
				object obj = Utils.Parse(text);
				if (obj == null || obj.GetType() != typeof(bool))
				{
					Debug.LogError("Condition parsing error: " + text);
				}
				else if (!(bool)obj)
				{
					return;
				}
			}
			Activate();
		}
		else if (m_inactiveStates.Contains(curr) && active)
		{
			Deactivate();
		}
	}

	public void Activate()
	{
		m_active = true;
		m_toActive.Invoke();
	}

	public void Deactivate()
	{
		m_active = false;
		m_toInactive.Invoke();
	}

	public bool HasSpecialState(UIController.State prev, UIController.State state)
	{
		foreach (SpecialTransition specialTransition in m_specialTransitions)
		{
			if (specialTransition.m_prevState == prev && specialTransition.m_state == state)
			{
				return true;
			}
		}
		return false;
	}

	public bool SpecialStateOverride(UIController.State prev, UIController.State state)
	{
		foreach (SpecialTransition specialTransition in m_specialTransitions)
		{
			if (specialTransition.m_prevState == prev && specialTransition.m_state == state && specialTransition.m_overrideNormal)
			{
				return true;
			}
		}
		return false;
	}

	private void EnterSpecialState(UIController.State prev, UIController.State state)
	{
		foreach (SpecialTransition specialTransition in m_specialTransitions)
		{
			if (specialTransition.m_prevState == prev && specialTransition.m_state == state)
			{
				specialTransition.m_onEnter.Invoke();
				break;
			}
		}
	}
}
