using UnityEngine;
using UnityEngine.Events;

public class OnPlayerStartLevel : MonoBehaviour
{
	[SerializeField]
	private UnityEvent m_onTrigger = new UnityEvent();

	private void OnEnable()
	{
		Singleton<Game>.Instance.Player.OnPlayerStartLevel += PlayerStartLevel;
	}

	private void OnDisable()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.Player != null)
		{
			Singleton<Game>.Instance.Player.OnPlayerStartLevel -= PlayerStartLevel;
		}
	}

	private void PlayerStartLevel()
	{
		m_onTrigger.Invoke();
	}
}
