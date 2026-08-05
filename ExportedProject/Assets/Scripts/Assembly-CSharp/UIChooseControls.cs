using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIChooseControls : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	public UnityEvent m_onShow = new UnityEvent();

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnEnable()
	{
		if (Singleton<Game>.Instance.IsLoaded && !(Singleton<Game>.Instance.Player == null) && !(Singleton<Game>.Instance.Player.Character == null))
		{
			m_onShow.Invoke();
		}
	}

	public void LoadCharacters()
	{
	}

	public void OnDisableScreen()
	{
		Singleton<Game>.Instance.UIController.HideChooseControls();
	}

	public void OnInvertedSelected()
	{
		Singleton<Game>.Instance.IsInverted = true;
	}

	public void OnNormalSelected()
	{
		Singleton<Game>.Instance.IsInverted = false;
	}
}
