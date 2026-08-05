using UnityEngine;
using UnityEngine.Events;

public class OnCurrencyChanged : MonoBehaviour
{
	[SerializeField]
	private string m_currencyId;

	[SerializeField]
	private UnityEvent m_onIncrease = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onDecrease = new UnityEvent();

	private void OnEnable()
	{
		Singleton<Game>.Instance.InventoryManager.OnCurrencyChanged += CurrencyChange;
	}

	private void OnDisable()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.InventoryManager != null)
		{
			Singleton<Game>.Instance.InventoryManager.OnCurrencyChanged -= CurrencyChange;
		}
	}

	private void Destroy()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.InventoryManager != null)
		{
			Singleton<Game>.Instance.InventoryManager.OnCurrencyChanged -= CurrencyChange;
		}
	}

	private void CurrencyChange(string id, int prev, int curr)
	{
		if (id == m_currencyId)
		{
			if (curr > prev)
			{
				m_onIncrease.Invoke();
			}
			else
			{
				m_onDecrease.Invoke();
			}
		}
	}
}
