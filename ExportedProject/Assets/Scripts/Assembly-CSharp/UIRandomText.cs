using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class UIRandomText : MonoBehaviour
{
	[SerializeField]
	private Text m_textField;

	[SerializeField]
	private bool m_localized = true;

	[Multiline]
	[SerializeField]
	private string[] m_text;

	private void Start()
	{
		RandomizeText();
	}

	public void RandomizeText()
	{
		if (!m_localized)
		{
			m_textField.text = m_text[Random.Range(0, m_text.Length)];
		}
		else
		{
			m_textField.text = ScriptLocalization.Get(m_text[Random.Range(0, m_text.Length)], true);
		}
	}
}
