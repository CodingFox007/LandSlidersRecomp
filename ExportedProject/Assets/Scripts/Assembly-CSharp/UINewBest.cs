using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UINewBest : MonoBehaviour, IEventSystemHandler
{
	public UnityEvent m_onClose = new UnityEvent();

	public void OnCloseClick()
	{
		m_onClose.Invoke();
	}

	public void OnDisableScreen()
	{
		Singleton<Game>.Instance.UIController.HideNewBest();
		Singleton<Game>.Instance.UIController.DoStartLevelUp();
	}
}
