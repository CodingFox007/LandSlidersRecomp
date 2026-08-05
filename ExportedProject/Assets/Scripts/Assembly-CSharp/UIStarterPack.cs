using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIStarterPack : MonoBehaviour
{
	[SerializeField]
	private string m_purchaseID = "com.prettygreat.landsliders.starterPack";

	[SerializeField]
	private int m_coinsToGive = 1000;

	[SerializeField]
	private Graphic m_coinTravellerSourceUpsell;

	[SerializeField]
	public UnityEvent m_onPurchaseSuccess = new UnityEvent();

	[TextArea(3, 10)]
	[SerializeField]
	public string m_itemsToGive = string.Empty;

	[SerializeField]
	private List<string> m_prefsToSet = new List<string>();

	public void TryBuy()
	{
		if (m_purchaseID == string.Empty)
		{
			PurchaseStarterPackCallback(true, string.Empty);
		}
		else
		{
			Singleton<Game>.Instance.InventoryManager.PurchaseIAP(m_purchaseID, PurchaseStarterPackCallback);
		}
	}

	public void PurchaseStarterPackCallback(bool success, string id)
	{
		if (success)
		{
			if (m_coinsToGive != 0)
			{
				Singleton<Game>.Instance.UIController.HUD.SpawnCoinTravellers(m_coinTravellerSourceUpsell, m_coinsToGive);
			}
			if (m_itemsToGive != string.Empty)
			{
				string[] array = m_itemsToGive.Split(',');
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (Singleton<Game>.Instance.InventoryManager.GetCurrency(text.Trim()) == 0)
					{
						Singleton<Game>.Instance.InventoryManager.AddCurrency(text.Trim(), 1);
					}
				}
				foreach (string item in m_prefsToSet)
				{
					Singleton<Game>.Instance.StatsManager.GetStat(item).IncreaseStat(1f);
				}
			}
			m_onPurchaseSuccess.Invoke();
			SaveManager.Save();
		}
		else
		{
			string empty = string.Empty;
			string msg = ScriptLocalization.Get("Purchase Failed", true);
			string ok = ScriptLocalization.Get("OK", true);
			NativeDialog.ShowDialog(empty, msg, ok);
		}
	}
}
