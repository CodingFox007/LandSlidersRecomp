using UnityEngine;
using UnityEngine.Events;

public class OnCharacterCheck : MonoBehaviour
{
	[SerializeField]
	private string m_id;

	[SerializeField]
	private UnityEvent m_onActive = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onInactive = new UnityEvent();

	private void OnEnable()
	{
		CharacterCheck();
		Singleton<Game>.Instance.Player.OnCharacterChanged += CharacterCheck;
	}

	private void OnDisable()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.Player != null)
		{
			Singleton<Game>.Instance.Player.OnCharacterChanged -= CharacterCheck;
		}
	}

	public void CharacterCheck()
	{
		if (Singleton<Game>.Instance.Player.CharacterId == m_id)
		{
			m_onActive.Invoke();
		}
		else
		{
			m_onInactive.Invoke();
		}
	}
}
