using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CodeBreaker : MonoBehaviour, IEventSystemHandler
{
	[Serializable]
	public class SymbolSet
	{
		public Mesh[] m_meshes;
	}

	[SerializeField]
	private int[] m_pin = new int[5];

	private List<int> m_currentCode = new List<int>();

	[SerializeField]
	private Text m_hintText;

	[SerializeField]
	private MeshFilter[] m_meshes;

	[SerializeField]
	private SymbolSet[] m_symbolsSets;

	[SerializeField]
	private UnityEvent m_onSuccess = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onFail = new UnityEvent();

	private void OnEnable()
	{
		m_currentCode.Clear();
		Utils.ShuffleArray(m_pin);
		int[] array = new int[5];
		for (int i = 0; i < 5; i++)
		{
			array[i] = i;
		}
		Utils.ShuffleArray(array);
		string text = string.Empty;
		int[] pin = m_pin;
		foreach (int num in pin)
		{
			text = text + (array[num] + 1) + ",";
		}
		text = text.Remove(text.Length - 1);
		m_hintText.text = text;
		int num2 = UnityEngine.Random.Range(0, m_symbolsSets.Length);
		for (int k = 0; k < 5; k++)
		{
			num2 = UnityEngine.Random.Range(0, m_symbolsSets.Length);
			int num3 = array[k];
			m_meshes[k].mesh = m_symbolsSets[num2].m_meshes[num3];
		}
	}

	public void EnterCode(int idx)
	{
		if (!m_currentCode.Contains(idx))
		{
			m_currentCode.Add(idx);
		}
	}

	public void CheckCode()
	{
		if (m_pin.Length != m_currentCode.Count)
		{
			m_onFail.Invoke();
			m_currentCode.Clear();
			return;
		}
		bool flag = true;
		for (int i = 0; i < m_pin.Length; i++)
		{
			if (m_pin[i] != m_currentCode[i])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			m_onSuccess.Invoke();
		}
		else
		{
			m_onFail.Invoke();
		}
		m_currentCode.Clear();
	}
}
