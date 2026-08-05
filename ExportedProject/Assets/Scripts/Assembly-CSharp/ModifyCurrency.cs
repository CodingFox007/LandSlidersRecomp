using UnityEngine;
using UnityEngine.Events;

public class ModifyCurrency : MonoBehaviour
{
	[SerializeField]
	private string m_currencyId;

	[SerializeField]
	private int m_currencyValue;

	[SerializeField]
	private UnityEvent m_onFailure = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onSuccess = new UnityEvent();

	public void Add()
	{
		Singleton<Game>.Instance.InventoryManager.AddCurrency(m_currencyId, m_currencyValue);
		m_onSuccess.Invoke();
	}

	public void Spend()
	{
		if (!(Singleton<Game>.Instance == null) && !(Singleton<Game>.Instance.InventoryManager == null))
		{
			if (Singleton<Game>.Instance.InventoryManager.SpendCurrency(m_currencyId, m_currencyValue))
			{
				m_onSuccess.Invoke();
			}
			else
			{
				m_onFailure.Invoke();
			}
		}
	}
}
