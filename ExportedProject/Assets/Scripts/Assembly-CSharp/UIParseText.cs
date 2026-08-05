using UnityEngine;
using UnityEngine.UI;

public class UIParseText : MonoBehaviour
{
	[SerializeField]
	private Text m_text;

	[SerializeField]
	private string m_format;

	[SerializeField]
	private string m_parse;

	[SerializeField]
	private bool m_update;

	private void OnEnable()
	{
		SetText();
	}

	private void Update()
	{
		if (m_update)
		{
			SetText();
		}
	}

	private void SetText()
	{
		m_text.text = string.Format(m_format, Utils.Parse(m_parse));
	}
}
