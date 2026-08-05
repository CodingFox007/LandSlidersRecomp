using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIPrizeSelectItem : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	public UnityEvent m_onSelect = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onUnselect = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onUnlock = new UnityEvent();

	public void OnMouseUpAsButton()
	{
		if (Singleton<Game>.Instance.UIController.PrizeSelect.SelectedItem == null)
		{
			SelectItem();
		}
	}

	public void SelectItem()
	{
		m_onSelect.Invoke();
		Singleton<Game>.Instance.UIController.PrizeSelect.SetSelectedItem(this);
	}

	public void UnselectItem()
	{
		m_onUnselect.Invoke();
	}

	public void UnlockItem()
	{
		m_onUnlock.Invoke();
	}

	public void ShowPrize()
	{
		Singleton<Game>.Instance.UIController.PrizeSelect.ShowPrize();
	}
}
