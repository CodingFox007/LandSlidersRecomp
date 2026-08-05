using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIChooseCharacter : MonoBehaviour, IEventSystemHandler
{
	[Serializable]
	public class CharacterChoice
	{
		public UIRenderCharacter m_characterRenderer;

		public Text m_characterName;

		public string[] m_choices;

		[HideInInspector]
		public int m_selectedIdx;
	}

	[SerializeField]
	private CharacterChoice[] m_characterChoices;

	[SerializeField]
	public UnityEvent m_onShow = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onCharacterSelected = new UnityEvent();

	public void OnEnable()
	{
		if (Singleton<Game>.Instance.IsLoaded && !(Singleton<Game>.Instance.Player == null) && !(Singleton<Game>.Instance.Player.Character == null))
		{
			m_onShow.Invoke();
		}
	}

	public void LoadCharacters()
	{
		CharacterChoice[] characterChoices = m_characterChoices;
		foreach (CharacterChoice characterChoice in characterChoices)
		{
			characterChoice.m_selectedIdx = UnityEngine.Random.Range(0, characterChoice.m_choices.Length);
			CharacterInfo characterInfo = Singleton<Game>.Instance.Player.GetCharacterInfo(characterChoice.m_choices[characterChoice.m_selectedIdx]);
			if (characterChoice.m_characterRenderer != null && characterInfo != null)
			{
				characterChoice.m_characterRenderer.CharacterId = characterInfo.Id;
				characterChoice.m_characterRenderer.Load();
			}
			if (characterChoice.m_characterName != null)
			{
				characterChoice.m_characterName.text = characterInfo.Name;
			}
		}
	}

	public void OnDisable()
	{
		CharacterChoice[] characterChoices = m_characterChoices;
		foreach (CharacterChoice characterChoice in characterChoices)
		{
			characterChoice.m_characterRenderer.Unload();
		}
	}

	public void OnSelectCharacter(int idx)
	{
		string text = m_characterChoices[idx].m_choices[m_characterChoices[idx].m_selectedIdx];
		Singleton<Game>.Instance.InventoryManager.AddCurrency(text, 1);
		Singleton<Game>.Instance.Player.SetCharacter(text);
		SaveManager.Save();
		m_onCharacterSelected.Invoke();
	}

	public void OnDisableScreen()
	{
		Singleton<Game>.Instance.UIController.HideChooseCharacter();
	}
}
