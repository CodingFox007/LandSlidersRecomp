using UnityEngine;
using UnityEngine.Events;

public class OnParseCheck : MonoBehaviour
{
	[SerializeField]
	private string m_parse;

	[SerializeField]
	private bool m_checkEachUpdate;

	[SerializeField]
	private UnityEvent m_onActive = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onInactive = new UnityEvent();

	private bool m_active;

	private void OnEnable()
	{
		ParseCheck(true);
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		if (m_checkEachUpdate)
		{
			ParseCheck();
		}
	}

	public void ParseCheck()
	{
		ParseCheck(false);
	}

	public void ParseCheck(bool force)
	{
		bool flag = false;
		object obj = Utils.Parse(m_parse);
		if (obj == null || obj.GetType() != typeof(bool))
		{
			Debug.LogError("Parse error: " + m_parse);
		}
		if ((bool)obj)
		{
			if (!m_active || force)
			{
				m_onActive.Invoke();
				m_active = true;
			}
		}
		else if (m_active || force)
		{
			m_onInactive.Invoke();
			m_active = false;
		}
	}
}
