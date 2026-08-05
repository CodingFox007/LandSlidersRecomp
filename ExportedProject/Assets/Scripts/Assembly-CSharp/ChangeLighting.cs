using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChangeLighting : MonoBehaviour
{
	public enum Priority
	{
		Default = 0,
		Level = 1,
		Character = 2
	}

	public class LightOptions
	{
		public Color m_color;

		public float m_intensity;
	}

	[SerializeField]
	private Priority m_priority;

	[SerializeField]
	private Color m_color;

	[SerializeField]
	[Range(0f, 8f)]
	private float m_intensity;

	[SerializeField]
	private float m_tweenTime;

	private float m_timer;

	private bool m_active;

	[SerializeField]
	private AnimationCurve m_curve;

	[SerializeField]
	private bool m_pingpong;

	[SerializeField]
	private float m_pongMultiplier = 1f;

	[SerializeField]
	private bool m_hasSecondaryLighting;

	[SerializeField]
	private float m_secondaryMinDelayTime = 5f;

	[SerializeField]
	private float m_secondaryMaxDelayTime = 10f;

	[SerializeField]
	private float m_secondaryMinHoldTime = 0.7f;

	[SerializeField]
	private float m_secondaryMaxHoldTime = 1f;

	[SerializeField]
	private Color m_secondaryColor = Color.white;

	[SerializeField]
	[Range(0f, 8f)]
	private float m_secondaryIntensity;

	[SerializeField]
	private UnityEvent m_onTriggerSecondary = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onUntriggerSecondary = new UnityEvent();

	[SerializeField]
	private AnimationCurve m_secondaryIntensityCurve;

	[SerializeField]
	private AnimationCurve m_secondaryColorCurve;

	private float m_secondaryDelayTime;

	private float m_secondaryHoldTime;

	private float m_secondaryTotalHoldTime;

	private bool m_backwards;

	private static Dictionary<Priority, LightOptions> m_lightOptions = new Dictionary<Priority, LightOptions>();

	public void Awake()
	{
		SetLightOptions(Priority.Default, Singleton<Game>.Instance.LightDefaultColor, Singleton<Game>.Instance.LightDefaultIntensity);
		if (m_hasSecondaryLighting)
		{
			m_secondaryDelayTime = UnityEngine.Random.Range(m_secondaryMinDelayTime, m_secondaryMaxDelayTime);
		}
	}

	public void Trigger()
	{
		m_active = true;
		if (m_pingpong)
		{
			if (m_timer != 0f)
			{
				m_backwards = true;
			}
			else
			{
				m_backwards = false;
			}
		}
		else
		{
			m_timer = 0f;
		}
	}

	public void Untrigger()
	{
		m_active = false;
		m_timer = 0f;
		RemoveLightOptions(m_priority);
	}

	public float GetSecondaryIntensityScale()
	{
		if (!m_hasSecondaryLighting)
		{
			return 0f;
		}
		if (m_secondaryHoldTime <= 0f)
		{
			return 0f;
		}
		float b = 1f;
		if (m_secondaryTotalHoldTime != 0f)
		{
			b = 1f - m_secondaryHoldTime / m_secondaryTotalHoldTime;
		}
		b = Mathf.Max(0f, Mathf.Min(1f, b));
		float result = b;
		if (m_secondaryIntensityCurve != null)
		{
			result = m_secondaryIntensityCurve.Evaluate(b);
		}
		return result;
	}

	private void Update()
	{
		if (!m_active)
		{
			return;
		}
		bool flag = false;
		if (m_hasSecondaryLighting)
		{
			if (m_secondaryHoldTime > 0f)
			{
				flag = true;
				m_secondaryHoldTime -= Time.deltaTime;
				if (m_secondaryHoldTime <= 0f)
				{
					m_secondaryHoldTime = 0f;
					m_secondaryDelayTime = UnityEngine.Random.Range(m_secondaryMinDelayTime, m_secondaryMaxDelayTime);
					m_onUntriggerSecondary.Invoke();
				}
				else
				{
					Color a = Color.Lerp(Singleton<Game>.Instance.LightDefaultColor, m_color, GetLightScale());
					float a2 = Mathf.Lerp(Singleton<Game>.Instance.LightDefaultIntensity, m_intensity, GetLightScale());
					float b = 1f;
					if (m_secondaryTotalHoldTime != 0f)
					{
						b = 1f - m_secondaryHoldTime / m_secondaryTotalHoldTime;
					}
					b = Mathf.Max(0f, Mathf.Min(1f, b));
					float t = b;
					float t2 = b;
					if (m_secondaryColorCurve != null)
					{
						t = m_secondaryColorCurve.Evaluate(b);
					}
					if (m_secondaryIntensityCurve != null)
					{
						t2 = m_secondaryIntensityCurve.Evaluate(b);
					}
					SetLightOptions(m_priority, Color.Lerp(a, m_secondaryColor, t), Mathf.Lerp(a2, m_secondaryIntensity, t2));
				}
			}
			else
			{
				m_secondaryDelayTime -= Time.deltaTime;
				if (m_secondaryDelayTime <= 0f)
				{
					m_secondaryHoldTime = UnityEngine.Random.Range(m_secondaryMinHoldTime, m_secondaryMaxHoldTime);
					m_secondaryTotalHoldTime = m_secondaryHoldTime;
					m_onTriggerSecondary.Invoke();
					flag = true;
				}
			}
		}
		if (flag)
		{
			return;
		}
		if (!m_backwards)
		{
			m_timer += Time.deltaTime;
		}
		else
		{
			m_timer -= Time.deltaTime * m_pongMultiplier;
			if (m_timer < 0f)
			{
				m_timer = 0f;
				m_backwards = false;
			}
		}
		float lightScale = GetLightScale();
		Color lightDefaultColor = Singleton<Game>.Instance.LightDefaultColor;
		Color color = m_color;
		float lightDefaultIntensity = Singleton<Game>.Instance.LightDefaultIntensity;
		float intensity = m_intensity;
		Color color2 = Color.Lerp(lightDefaultColor, color, lightScale);
		float intensity2 = Mathf.Lerp(lightDefaultIntensity, intensity, lightScale);
		SetLightOptions(m_priority, color2, intensity2);
		if (m_timer >= m_tweenTime)
		{
			if (!m_pingpong)
			{
				m_timer = 0f;
			}
			if (!m_hasSecondaryLighting)
			{
				m_active = false;
			}
		}
	}

	private float GetLightScale()
	{
		float b = 1f;
		if (m_tweenTime != 0f)
		{
			b = m_timer / m_tweenTime;
		}
		b = Mathf.Max(0f, Mathf.Min(1f, b));
		if (m_curve != null)
		{
			b = m_curve.Evaluate(b);
		}
		return b;
	}

	private static void SetLightOptions(Priority priority, Color color, float intensity)
	{
		LightOptions lightOptions = new LightOptions();
		lightOptions.m_color = color;
		lightOptions.m_intensity = intensity;
		if (m_lightOptions.ContainsKey(priority))
		{
			m_lightOptions[priority] = lightOptions;
		}
		else
		{
			m_lightOptions.Add(priority, lightOptions);
		}
	}

	private static void RemoveLightOptions(Priority priority)
	{
		m_lightOptions.Remove(priority);
	}

	private static bool IsHighestPriority(Priority priority)
	{
		Array values = Enum.GetValues(typeof(Priority));
		foreach (Priority item in values)
		{
			int num = (int)item;
			if (num > (int)priority && m_lightOptions.ContainsKey(item))
			{
				return false;
			}
		}
		return true;
	}

	public static void GetHighestPriority(out Color color, out float intensity)
	{
		if (m_lightOptions == null || m_lightOptions.Count <= 0)
		{
			color = Singleton<Game>.Instance.LightDefaultColor;
			intensity = Singleton<Game>.Instance.LightDefaultIntensity;
			return;
		}
		Priority key = Priority.Default;
		if (m_lightOptions.ContainsKey(Priority.Character))
		{
			key = Priority.Character;
		}
		else if (m_lightOptions.ContainsKey(Priority.Level))
		{
			key = Priority.Level;
		}
		color = m_lightOptions[key].m_color;
		intensity = m_lightOptions[key].m_intensity;
	}
}
