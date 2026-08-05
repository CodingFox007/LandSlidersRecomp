using UnityEngine;

public class UIChristmasDisplay : MonoBehaviour
{
	[SerializeField]
	private GameObject m_found;

	[SerializeField]
	private GameObject m_notFound;

	[SerializeField]
	private GameObject m_notAvailable;

	[SerializeField]
	private string m_characterID;

	public void OnEnable()
	{
		m_found.SetActive(false);
		m_notFound.SetActive(false);
		m_notAvailable.SetActive(false);
		if (Singleton<Game>.Instance.Player.GetCharacterInfo(m_characterID) != null)
		{
			if (!Singleton<Game>.Instance.Player.GetCharacterInfo(m_characterID).Locked)
			{
				m_found.SetActive(true);
			}
			else if (Singleton<Game>.Instance.SpecialEventManager.IsAvailable(m_characterID))
			{
				m_notFound.SetActive(true);
			}
			else
			{
				m_notAvailable.SetActive(true);
			}
		}
	}

	public void Update()
	{
		if (Singleton<Game>.Instance.UIController.HUD.m_allSpecialNeedsUpdate > 0)
		{
			OnEnable();
		}
	}
}
