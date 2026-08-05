using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;

public class UIEffects : MonoBehaviour, ISerializationCallbackReceiver
{
	public delegate void CompleteCallback();

	[Dictionary("m_effectsValues")]
	public List<string> m_effects;

	[HideInInspector]
	public List<Animator> m_effectsValues;

	private Dictionary<string, Animator> m_effectAnimators;

	private CompleteCallback m_callback;

	private Animator m_currentEffect;

	private Animator m_finishingEffect;

	private bool m_complete;

	public bool IsComplete
	{
		get
		{
			return m_complete;
		}
	}

	public bool IsActive
	{
		get
		{
			return m_currentEffect != null;
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_effects, m_effectsValues, out m_effectAnimators, "effectAnimators");
	}

	private void Awake()
	{
		foreach (KeyValuePair<string, Animator> effectAnimator in m_effectAnimators)
		{
			effectAnimator.Value.gameObject.SetActive(false);
		}
	}

	public void StartEffect(string id, CompleteCallback callback = null)
	{
		if (m_effectAnimators.ContainsKey(id))
		{
			m_effectAnimators[id].gameObject.SetActive(true);
			m_complete = false;
			m_currentEffect = m_effectAnimators[id];
			m_callback = callback;
			m_currentEffect.SetTrigger("start");
		}
	}

	public void EndEffect()
	{
		m_complete = false;
		m_currentEffect.SetTrigger("finish");
		m_finishingEffect = m_currentEffect;
		m_currentEffect = null;
	}

	public void AnimComplete()
	{
		m_complete = true;
		if (m_callback != null)
		{
			m_callback();
		}
		if (m_finishingEffect != null)
		{
			m_finishingEffect.gameObject.SetActive(false);
			m_finishingEffect = null;
		}
	}

	public Animator GetEffectAnimator(string id)
	{
		if (!m_effectAnimators.ContainsKey(id))
		{
			return null;
		}
		return m_effectAnimators[id];
	}
}
