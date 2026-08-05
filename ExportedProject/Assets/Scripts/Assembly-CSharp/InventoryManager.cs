using System.Collections.Generic;
using I2.Loc;
using NativeAlert;
using Unibill;
using UnityEngine;

public class InventoryManager : MonoBehaviour, Savable
{
	public delegate void PurchaseCallback(bool success, string id);

	public delegate void CurrencyChange(string id, int prev, int curr);

	private Dictionary<string, int> m_inventory = new Dictionary<string, int>();

	private Dictionary<string, PurchaseCallback> m_purchaseCallbacks = new Dictionary<string, PurchaseCallback>();

	public event CurrencyChange OnCurrencyChanged;

	public void Start()
	{
		if (Resources.Load("unibillInventory.json") == null)
		{
			Debug.LogError("You must define your purchasable inventory within the inventory editor!");
			base.gameObject.SetActive(false);
			return;
		}
		Unibiller.onBillerReady += OnBillerReady;
		Unibiller.onTransactionsRestored += OnTransactionsRestored;
		Unibiller.onPurchaseFailed += OnFailed;
		Unibiller.onPurchaseCompleteEvent += OnPurchased;
		Unibiller.onPurchaseDeferred += onDeferred;
		Unibiller.Initialise();
	}

	public void AddCurrency(string id, int value)
	{
		int prev = 0;
		if (!m_inventory.ContainsKey(id))
		{
			m_inventory.Add(id, 0);
		}
		else
		{
			prev = m_inventory[id];
		}
		m_inventory[id] += value;
		Singleton<Game>.Instance.StatsManager.GetStat(id).IncreaseStat(value);
		if (this.OnCurrencyChanged != null)
		{
			this.OnCurrencyChanged(id, prev, m_inventory[id]);
		}
	}

	public void RemoveCurrency(string id, int value)
	{
		if (m_inventory.ContainsKey(id))
		{
			int prev = m_inventory[id];
			m_inventory[id] -= value;
			Singleton<Game>.Instance.StatsManager.GetStat(id).DecreaseStat(value);
			if (this.OnCurrencyChanged != null)
			{
				this.OnCurrencyChanged(id, prev, m_inventory[id]);
			}
		}
	}

	public int GetCurrency(string id)
	{
		if (!m_inventory.ContainsKey(id))
		{
			return 0;
		}
		return m_inventory[id];
	}

	public object Parse()
	{
		if (Utils.ParseTokenCount() < 2)
		{
			return null;
		}
		return GetCurrency(Utils.ParseToken(1));
	}

	public bool SpendCurrency(string id, int value)
	{
		if (GetCurrency(id) >= value)
		{
			RemoveCurrency(id, value);
			return true;
		}
		return false;
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		JSONObject jSONObject2 = new JSONObject();
		foreach (KeyValuePair<string, int> item in m_inventory)
		{
			jSONObject2.AddField(item.Key, item.Value);
		}
		jSONObject.AddField("items", jSONObject2);
		return jSONObject;
	}

	public bool Verify(JSONObject data)
	{
		if (data.HasField("items"))
		{
			JSONObject field = data.GetField("items");
			if (field.keys == null)
			{
				return false;
			}
		}
		return true;
	}

	public void Load(JSONObject data)
	{
		m_inventory.Clear();
		if (data.HasField("items"))
		{
			JSONObject field = data.GetField("items");
			if (field.keys != null)
			{
				foreach (string key in field.keys)
				{
					m_inventory.Add(key, field[key].AsInt);
				}
			}
			else
			{
				m_inventory.Add("turny", 1);
			}
		}
		UpdateInventoryFromUnibill();
	}

	public JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB)
	{
		JSONObject jSONObject = dataA;
		JSONObject jSONObject2 = null;
		JSONObject jSONObject3 = null;
		JSONObject jSONObject4 = null;
		Debug.Log("A");
		if (dataA != null && dataA.HasField("items"))
		{
			Debug.Log("Setting for a");
			jSONObject2 = dataA.GetField("items");
			jSONObject = new JSONObject(dataA);
			jSONObject4 = jSONObject.GetField("items");
		}
		Debug.Log("B");
		if (dataB != null && dataB.HasField("items"))
		{
			Debug.Log("Setting for b");
			jSONObject3 = dataB.GetField("items");
			jSONObject = new JSONObject(dataB);
			jSONObject4 = jSONObject.GetField("items");
		}
		Debug.Log("C");
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			int a = 0;
			int b = 0;
			Debug.Log(availableCharacter.Id);
			if (jSONObject2 != null)
			{
				Debug.Log("Part a0" + jSONObject2.keys);
			}
			if (jSONObject2 != null && jSONObject2.keys.Contains(availableCharacter.Id))
			{
				Debug.Log("Part a");
				a = jSONObject2[availableCharacter.Id].AsInt;
			}
			if (jSONObject3 != null)
			{
				Debug.Log("Part b0" + jSONObject3.keys);
			}
			if (jSONObject3 != null && jSONObject3.keys.Contains(availableCharacter.Id))
			{
				Debug.Log("Part b");
				b = jSONObject3[availableCharacter.Id].AsInt;
			}
			int num = Mathf.Max(a, b);
			if (num > 0)
			{
				if (!jSONObject4.HasField(availableCharacter.Id))
				{
					jSONObject4.AddField(availableCharacter.Id, num);
				}
				else
				{
					jSONObject4.SetField(availableCharacter.Id, num);
				}
			}
			Debug.Log(availableCharacter.Id + " end");
		}
		return jSONObject;
	}

	public void Merge(JSONObject dataA, JSONObject dataB)
	{
		JSONObject jSONObject = null;
		JSONObject jSONObject2 = null;
		if (dataA != null && dataA.type != JSONObject.Type.NULL && dataA.HasField("items"))
		{
			jSONObject = dataA.GetField("items");
		}
		if (dataB != null && dataB.type != JSONObject.Type.NULL && dataB.HasField("items"))
		{
			jSONObject2 = dataB.GetField("items");
		}
		if (jSONObject != null)
		{
			Debug.Log("Inventory Merge Save dataA: " + jSONObject.ToString());
		}
		else
		{
			Debug.Log("Inventory Merge Save dataA: null");
		}
		if (jSONObject2 != null)
		{
			Debug.Log("Inventory Merge Save dataB: " + jSONObject2.ToString());
		}
		else
		{
			Debug.Log("Inventory Merge Save dataB: null");
		}
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			int a = 0;
			int b = 0;
			if (jSONObject != null && jSONObject.keys != null && jSONObject.keys.Contains(availableCharacter.Id))
			{
				a = jSONObject[availableCharacter.Id].AsInt;
			}
			if (jSONObject2 != null && jSONObject2.keys != null && jSONObject2.keys.Contains(availableCharacter.Id))
			{
				b = jSONObject2[availableCharacter.Id].AsInt;
			}
			int num = Mathf.Max(a, b);
			if (num > 0)
			{
				if (!m_inventory.ContainsKey(availableCharacter.Id))
				{
					m_inventory.Add(availableCharacter.Id, num);
				}
				else if (num > m_inventory[availableCharacter.Id])
				{
					m_inventory[availableCharacter.Id] = num;
				}
			}
		}
	}

	public JSONObject Reset()
	{
		Debug.Log("Reset");
		m_inventory.Clear();
		return Save();
	}

	public JSONObject UpdateVersion(JSONObject data)
	{
		JSONObject result = data;
		string version = GetVersion(data);
		if (!(version == string.Empty))
		{
			switch (version)
			{
			case "0.0.1":
			case "0.0.2":
			case "0.0.3":
				break;
			default:
				goto IL_0051;
			}
		}
		result = Reset();
		goto IL_0051;
		IL_0051:
		return result;
	}

	public string SaveId()
	{
		return "Inventory";
	}

	public string Version()
	{
		return "0.0.4";
	}

	public string GetVersion(JSONObject data)
	{
		if (!data.HasField("version"))
		{
			return string.Empty;
		}
		return data.GetField("version").AsString;
	}

	public bool UseCloud()
	{
		return true;
	}

	public PurchasableItem GetIAP(string id)
	{
		if (!Unibiller.Initialised)
		{
			return null;
		}
		PurchasableItem result = null;
		PurchasableItem[] allPurchasableItems = Unibiller.AllPurchasableItems;
		foreach (PurchasableItem purchasableItem in allPurchasableItems)
		{
			if (purchasableItem.Id == id)
			{
				result = purchasableItem;
				break;
			}
		}
		return result;
	}

	public void PurchaseIAP(string id, PurchaseCallback callback)
	{
		PurchasableItem iAP = GetIAP(id);
		if (iAP == null)
		{
			Debug.LogError("Unable to find IAP: " + id);
			callback(false, id);
			return;
		}
		if (!m_purchaseCallbacks.ContainsKey(id))
		{
			m_purchaseCallbacks.Add(id, callback);
		}
		else
		{
			m_purchaseCallbacks[id] = callback;
		}
		Singleton<Game>.Instance.UIController.ShowWaitingBlocker();
		Unibiller.initiatePurchase(iAP, string.Empty);
	}

	public void RestorePurchases()
	{
		if (Unibiller.Initialised)
		{
			Singleton<Game>.Instance.UIController.ShowWaitingBlocker();
			Unibiller.restoreTransactions();
		}
		else
		{
			OnTransactionsRestored(false);
		}
	}

	private void OnBillerReady(UnibillState state)
	{
		if (state != UnibillState.CRITICAL_ERROR)
		{
			UpdateInventoryFromUnibill();
		}
	}

	private void OnTransactionsRestored(bool success)
	{
		Singleton<Game>.Instance.UIController.HideWaitingBlocker();
		UpdateInventoryFromUnibill();
		Debug.Log("Transactions restored.");
		if (success)
		{
			AndroidNativeAlert.ShowAlert(ScriptLocalization.Get("Land Sliders", true), ScriptLocalization.Get("Purchases Restored", true), ScriptLocalization.Get("OK", true));
		}
		else
		{
			AndroidNativeAlert.ShowAlert(ScriptLocalization.Get("Land Sliders", true), ScriptLocalization.Get("Restore Purchases Failed", true), ScriptLocalization.Get("OK", true));
		}
	}

	private void OnPurchased(PurchaseEvent e)
	{
		Debug.Log("Purchase OK: " + e.PurchasedItem.Id);
		Debug.Log("Receipt: " + e.Receipt);
		Debug.Log(string.Format("{0} has now been purchased {1} times.", e.PurchasedItem.name, Unibiller.GetPurchaseCount(e.PurchasedItem)));
		Singleton<Game>.Instance.UIController.HideWaitingBlocker();
		if (m_purchaseCallbacks.ContainsKey(e.PurchasedItem.Id))
		{
			m_purchaseCallbacks[e.PurchasedItem.Id](true, e.PurchasedItem.Id);
			m_purchaseCallbacks.Remove(e.PurchasedItem.Id);
		}
	}

	private void onDeferred(PurchasableItem item)
	{
		Debug.Log("Purchase deferred blud: " + item.Id);
	}

	private void OnFailed(PurchaseFailedEvent e)
	{
		Debug.Log("Purchase failed: " + e.PurchasedItem.Id);
		Debug.Log(e.Reason);
		Singleton<Game>.Instance.UIController.HideWaitingBlocker();
		if (m_purchaseCallbacks.ContainsKey(e.PurchasedItem.Id))
		{
			m_purchaseCallbacks[e.PurchasedItem.Id](false, e.PurchasedItem.Id);
			m_purchaseCallbacks.Remove(e.PurchasedItem.Id);
		}
	}

	private void UpdateInventoryFromUnibill()
	{
		if (!Unibiller.Initialised)
		{
			return;
		}
		PurchasableItem[] allPurchasableItems = Unibiller.AllPurchasableItems;
		foreach (PurchasableItem purchasableItem in allPurchasableItems)
		{
			int purchaseCount = Unibiller.GetPurchaseCount(purchasableItem);
			if (purchaseCount <= 0)
			{
				continue;
			}
			string text = purchasableItem.Id.Replace("com.prettygreat.landsliders.", string.Empty);
			switch (text)
			{
			case "goldpack":
			{
				string text2 = "roosterGold, oxGold, rabbitGold, snakeGold, goatGold, horseGold, monkeyGold, ratGold, tigerGold, dragonGold, cnydogGold, pigGold";
				string[] array = text2.Split(',');
				string[] array2 = array;
				foreach (string text3 in array2)
				{
					if (Singleton<Game>.Instance.InventoryManager.GetCurrency(text3.Trim()) == 0)
					{
						Singleton<Game>.Instance.InventoryManager.AddCurrency(text3.Trim(), 1);
					}
				}
				Singleton<Game>.Instance.StatsManager.GetStat("goldenPack4").IncreaseStat(1f);
				Singleton<Game>.Instance.StatsManager.GetStat("goldenPack8").IncreaseStat(1f);
				Singleton<Game>.Instance.StatsManager.GetStat("goldenPack12").IncreaseStat(1f);
				break;
			}
			case "shrekpack1":
			{
				string text4 = "shrek_donkey,shrek_dragon,shrek_fiona,shrek_farquaad";
				string[] array3 = text4.Split(',');
				string[] array4 = array3;
				foreach (string text5 in array4)
				{
					if (Singleton<Game>.Instance.InventoryManager.GetCurrency(text5.Trim()) == 0)
					{
						Singleton<Game>.Instance.InventoryManager.AddCurrency(text5.Trim(), 1);
					}
				}
				break;
			}
			case "shrekpack2":
			{
				string text6 = "shrek_pussinboots,shrek_gingy,shrek_pinocchio,shrek_threeblindmice";
				string[] array5 = text6.Split(',');
				string[] array6 = array5;
				foreach (string text7 in array6)
				{
					if (Singleton<Game>.Instance.InventoryManager.GetCurrency(text7.Trim()) == 0)
					{
						Singleton<Game>.Instance.InventoryManager.AddCurrency(text7.Trim(), 1);
					}
				}
				break;
			}
			default:
				if (!m_inventory.ContainsKey(text))
				{
					m_inventory.Add(text, purchaseCount);
				}
				else
				{
					m_inventory[text] = purchaseCount;
				}
				break;
			}
		}
	}
}
