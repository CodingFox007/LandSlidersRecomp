using UnityEngine;
using UnityEngine.Events;

public class OnCurrencyCheck : MonoBehaviour
{
	[SerializeField]
	private string m_currencyId;

	[SerializeField]
	private int m_currencyAmount;

	[SerializeField]
	private string m_currencyId2;

	[SerializeField]
	private int m_currencyAmount2;

	[SerializeField]
	private UnityEvent m_onEnoughCurrency = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onNotEnoughCurrency = new UnityEvent();

	private bool m_enough;

	private bool HasEnough()
	{
		if (Singleton<Game>.Instance.InventoryManager.GetCurrency(m_currencyId) >= m_currencyAmount)
		{
			return true;
		}
		if (m_currencyId2 != string.Empty && Singleton<Game>.Instance.InventoryManager.GetCurrency(m_currencyId2) >= m_currencyAmount2)
		{
			return true;
		}
		return false;
	}

	private void OnEnable()
	{
		if (HasEnough())
		{
			m_onEnoughCurrency.Invoke();
			m_enough = true;
		}
		else
		{
			m_onNotEnoughCurrency.Invoke();
			m_enough = false;
		}
		Singleton<Game>.Instance.InventoryManager.OnCurrencyChanged += CurrencyChange;
	}

	private void OnDisable()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.InventoryManager != null)
		{
			Singleton<Game>.Instance.InventoryManager.OnCurrencyChanged -= CurrencyChange;
		}
	}

	private void CurrencyChange(string id, int prev, int curr)
	{
		if (id == m_currencyId || id == m_currencyId2)
		{
			if (HasEnough() && !m_enough)
			{
				m_onEnoughCurrency.Invoke();
				m_enough = true;
			}
			else if (!HasEnough() && m_enough)
			{
				m_onNotEnoughCurrency.Invoke();
				m_enough = false;
			}
		}
	}
}
