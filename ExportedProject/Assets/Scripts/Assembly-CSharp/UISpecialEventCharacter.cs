using UnityEngine;
using UnityEngine.UI;

public class UISpecialEventCharacter : MonoBehaviour
{
	[SerializeField]
	private UIRenderCharacter m_characterRenderer;

	[SerializeField]
	private Text m_characterName;

	private void OnEnable()
	{
		if (Singleton<Game>.Instance.IsLoaded)
		{
			m_characterRenderer.CharacterId = Singleton<Game>.Instance.SpecialEventManager.ChosenCharacter;
			m_characterRenderer.Load();
			m_characterName.text = Singleton<Game>.Instance.Player.GetCharacterInfo(Singleton<Game>.Instance.SpecialEventManager.ChosenCharacter).Name;
		}
	}

	public void OnDisable()
	{
		m_characterRenderer.Unload();
	}
}
