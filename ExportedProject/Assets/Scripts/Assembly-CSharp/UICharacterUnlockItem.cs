using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UICharacterUnlockItem : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	public UnityEvent m_onSelect = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onUnselect = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onUnlock = new UnityEvent();

	private bool m_isBought;

	private bool m_isAvailable = true;

	public void OnMouseUpAsButton()
	{
		if (!m_isBought)
		{
			if (Singleton<Game>.Instance.UIController.CharacterUnlock.SelectedItem != this)
			{
				SelectItem();
			}
			else
			{
				Singleton<Game>.Instance.UIController.CharacterUnlock.OnBuyClick();
			}
		}
	}

	public bool IsAvailable()
	{
		return m_isAvailable;
	}

	public void SetAvailable(bool available)
	{
		m_isAvailable = available;
	}

	public void SelectItem()
	{
		m_onSelect.Invoke();
		Singleton<Game>.Instance.UIController.CharacterUnlock.SetSelectedItem(this);
	}

	public void UnselectItem()
	{
		m_onUnselect.Invoke();
	}

	public void UnlockItem()
	{
		m_isBought = true;
		m_onUnlock.Invoke();
	}

	public void FocusItem()
	{
		Singleton<Game>.Instance.UIController.CharacterUnlock.FocusItem();
	}

	public void ShowPrize()
	{
		Singleton<Game>.Instance.UIController.CharacterUnlock.ShowPrize();
	}
}
