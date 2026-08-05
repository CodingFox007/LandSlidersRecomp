using UnityEngine;
using UnityEngine.UI;

public class UIReplaceCollectible : MonoBehaviour
{
	[SerializeField]
	private Image m_image;

	[SerializeField]
	private bool m_useLastPlayedCharacter;

	[SerializeField]
	private bool m_objectOfDesire;

	[SerializeField]
	private Sprite m_coinImage;

	public CharacterInfo Info
	{
		get
		{
			if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.Player != null)
			{
				if (!m_useLastPlayedCharacter)
				{
					return Singleton<Game>.Instance.Player.CharacterInfo;
				}
				return Singleton<Game>.Instance.Player.LastPlayedCharacter;
			}
			return null;
		}
	}

	private void Start()
	{
		Singleton<Game>.Instance.Player.OnCharacterChanged += CharacterChanged;
		if (m_objectOfDesire)
		{
			Singleton<Game>.Instance.LevelManager.OnLevelLoadComplete += CharacterChanged;
		}
	}

	private void OnDestroy()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.Player != null)
		{
			Singleton<Game>.Instance.Player.OnCharacterChanged -= CharacterChanged;
		}
		if (m_objectOfDesire && Singleton<Game>.Instance != null && Singleton<Game>.Instance.LevelManager != null)
		{
			Singleton<Game>.Instance.LevelManager.OnLevelLoadComplete -= CharacterChanged;
		}
	}

	private void OnEnable()
	{
		CharacterChanged();
	}

	public void CharacterChanged()
	{
		if (Info != null)
		{
			if (m_objectOfDesire && Singleton<Game>.Instance.LevelManager != null && Singleton<Game>.Instance.LevelManager.CurrentLevel != null && Singleton<Game>.Instance.LevelManager.CurrentLevel.m_objectOfDesire == "coin")
			{
				m_image.sprite = m_coinImage;
				return;
			}
			GameObject replacementPickup = Info.ReplacementPickup;
			m_image.sprite = replacementPickup.GetComponent<Pickup>().Icon;
		}
	}
}
