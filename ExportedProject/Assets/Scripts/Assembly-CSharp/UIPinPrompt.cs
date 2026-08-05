using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIPinPrompt : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private int[] m_pin;

	[SerializeField]
	private int[] m_pin2;

	private List<int> m_currentCode = new List<int>();

	[SerializeField]
	private UnityEvent m_onSuccess = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onSuccess2 = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onFail = new UnityEvent();

	private void OnEnable()
	{
		m_currentCode.Clear();
	}

	public void EnterCode(int idx)
	{
		m_currentCode.Add(idx);
		if (m_pin.Length != m_currentCode.Count)
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < m_pin.Length; i++)
		{
			if (m_pin[i] != m_currentCode[i] && m_pin2[i] != m_currentCode[i])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			MonoBehaviour.print(m_pin[1] + " " + m_currentCode[1]);
			if (m_pin[1] == m_currentCode[1])
			{
				m_onSuccess.Invoke();
			}
			else
			{
				m_onSuccess2.Invoke();
			}
		}
		else
		{
			m_onFail.Invoke();
		}
		m_currentCode.Clear();
	}
}
