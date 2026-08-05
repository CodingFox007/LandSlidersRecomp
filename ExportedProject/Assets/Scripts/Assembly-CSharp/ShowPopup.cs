using ArabicSupport;
using HeavyDutyInspector;
using I2.Loc;
using UnityEngine;

public class ShowPopup : MonoBehaviour
{
	public enum PopupType
	{
		Generic = 0,
		PreExit = 1
	}

	[SerializeField]
	private PopupType m_popupType;

	[SerializeField]
	[Multiline]
	private string[] m_text;

	[Comment("TextInput count must match Text count", CommentType.Warning, 0)]
	[SerializeField]
	private string[] m_textInput;

	[SerializeField]
	private Sprite[] m_icons;

	private void Start()
	{
	}

	public void Trigger()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (m_popupType == PopupType.Generic)
		{
			string[] array = new string[m_text.Length];
			for (int i = 0; i < m_text.Length; i++)
			{
				if (LocalizationManager.IsRight2Left)
				{
					LocalizationManager.IsRight2Left = false;
					array[i] = ArabicFixer.Fix(string.Format(ScriptLocalization.Get(m_text[i]), Utils.Parse(m_textInput[i])));
					LocalizationManager.IsRight2Left = true;
				}
				else
				{
					array[i] = string.Format(ScriptLocalization.Get(m_text[i], true), Utils.Parse(m_textInput[i]));
				}
			}
			Singleton<Game>.Instance.UIController.HUD.AddPopup(array, m_icons);
		}
		else if (m_popupType == PopupType.PreExit)
		{
			Singleton<Game>.Instance.UIController.HUD.ShowPreExitPopup();
		}
	}
}
