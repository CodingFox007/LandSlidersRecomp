using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextCounter : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private Text m_text;

	[SerializeField]
	private float m_timePerUnit;

	[SerializeField]
	private float m_minTime;

	[SerializeField]
	private float m_maxTime;

	[SerializeField]
	private bool m_countActive = true;

	[SerializeField]
	private UnityEvent m_onChange = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onReachedTarget = new UnityEvent();

	private float m_timer;

	private float m_time;

	private int m_current;

	private int m_target;

	public bool CountActive
	{
		get
		{
			return m_countActive;
		}
	}

	public Text Text
	{
		get
		{
			return m_text;
		}
	}

	private void Awake()
	{
		m_timer = 0f;
		m_time = 0f;
	}

	public void SetValue(int value)
	{
		m_current = value;
		m_target = value;
		m_text.text = value.ToString();
	}

	public void SetTarget(int target)
	{
		int.TryParse(m_text.text, out m_current);
		m_target = target;
		int num = Mathf.Abs(m_target - m_current);
		m_time = (float)num * m_timePerUnit;
		m_time = Mathf.Clamp(m_time, m_minTime, m_maxTime);
		m_timer = 0f;
	}

	public int GetValue()
	{
		return int.Parse(m_text.text);
	}

	public int GetTarget()
	{
		return m_target;
	}

	private void Update()
	{
		if (m_countActive && m_current != m_target)
		{
			m_timer += Time.deltaTime;
			string text = Mathf.FloorToInt(Mathf.Lerp(m_current, m_target, m_timer / m_time)).ToString();
			if (m_text.text != text)
			{
				m_text.text = text;
				m_onChange.Invoke();
			}
			if (m_timer >= m_time)
			{
				m_current = m_target;
				m_onReachedTarget.Invoke();
			}
		}
	}

	public void StartCount()
	{
		m_countActive = true;
	}

	public void StopCount()
	{
		m_countActive = false;
	}
}
