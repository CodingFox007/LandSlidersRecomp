using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

public class SpecialEventManager : MonoBehaviour, Savable
{
	private class AvailableData
	{
		private enum CheckType
		{
			Simple = 0,
			Timed = 1
		}

		private CheckType m_checkType;

		private bool m_simpleValue;

		private int m_startHour;

		private int m_endHour;

		public AvailableData(string available)
		{
			if (available.ToUpper() == "AVAILABLE")
			{
				m_simpleValue = true;
				return;
			}
			if (available.ToUpper() == "UNAVAILABLE")
			{
				m_simpleValue = false;
				return;
			}
			m_checkType = CheckType.Timed;
			bool flag = available.EndsWith("am");
			available = available.Substring(0, available.Length - 2);
			string s = available.Substring(0, available.IndexOf('-'));
			string s2 = available.Substring(available.IndexOf('-') + 1);
			m_startHour = int.Parse(s);
			if (!flag)
			{
				m_startHour += 12;
			}
			m_endHour = int.Parse(s2);
			if (!flag)
			{
				m_endHour += 12;
			}
		}

		public bool IsAvailable()
		{
			if (m_checkType == CheckType.Simple)
			{
				return m_simpleValue;
			}
			int hour = DateTime.Now.Hour;
			if (hour >= m_startHour && hour < m_endHour)
			{
				return true;
			}
			return false;
		}
	}

	[SerializeField]
	private string m_ID;

	[SerializeField]
	private string m_scheduleURL;

	[SerializeField]
	private string m_currentSelectorURL;

	[SerializeField]
	private string m_currentSelectorOverride;

	[SerializeField]
	private int m_startingChanceOneIn = 99;

	[SerializeField]
	private Level m_spawnSetPiece;

	[SerializeField]
	private float m_syncMinutes = 30f;

	[SerializeField]
	private List<string> m_buyAllIAP;

	private float m_timeAtLastSync;

	private string m_chosenCharacter;

	private bool m_takenAny;

	[TextArea(3, 10)]
	public string m_defaultSchedule = "[\n    {\n        \"id\": \"rat\",\n        \"4th\": \"Unavailable\",\n        \"5th\": \"Available\",\n        \"6th\": \"Unavailable\",\n        \"7th\": \"Unavailable\",\n        \"8th\": \"Unavailable\",\n        \"9th\": \"Unavailable\",\n        \"10th\": \"Unavailable\",\n        \"11th\": \"Available\",\n        \"12th\": \"Unavailable\",\n        \"13th\": \"Unavailable\",\n        \"14th\": \"Unavailable\",\n        \"15th\": \"Unavailable\",\n        \"16th\": \"Unavailable\",\n        \"17th\": \"Available\",\n        \"18th\": \"Available\",\n        \"19th\": \"Available\",\n        \"20th\": \"Available\",\n        \"21st\": \"Available\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"ox\",\n        \"4th\": \"Unavailable\",\n        \"5th\": \"Unavailable\",\n        \"6th\": \"Available\",\n        \"7th\": \"Unavailable\",\n        \"8th\": \"Unavailable\",\n        \"9th\": \"Unavailable\",\n        \"10th\": \"Unavailable\",\n        \"11th\": \"Unavailable\",\n        \"12th\": \"Available\",\n        \"13th\": \"Unavailable\",\n        \"14th\": \"Unavailable\",\n        \"15th\": \"Unavailable\",\n        \"16th\": \"Unavailable\",\n        \"17th\": \"Unavailable\",\n        \"18th\": \"Available\",\n        \"19th\": \"Available\",\n        \"20th\": \"Available\",\n        \"21st\": \"Available\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"tiger\",\n        \"4th\": \"Unavailable\",\n        \"5th\": \"Unavailable\",\n        \"6th\": \"Unavailable\",\n        \"7th\": \"Available\",\n        \"8th\": \"Unavailable\",\n        \"9th\": \"Unavailable\",\n        \"10th\": \"Unavailable\",\n        \"11th\": \"Unavailable\",\n        \"12th\": \"Unavailable\",\n        \"13th\": \"Available\",\n        \"14th\": \"Unavailable\",\n        \"15th\": \"Unavailable\",\n        \"16th\": \"Unavailable\",\n        \"17th\": \"Unavailable\",\n        \"18th\": \"Unavailable\",\n        \"19th\": \"Available\",\n        \"20th\": \"Available\",\n        \"21st\": \"Available\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"rabbit\",\n        \"4th\": \"Unavailable\",\n        \"5th\": \"Unavailable\",\n        \"6th\": \"Unavailable\",\n        \"7th\": \"Unavailable\",\n        \"8th\": \"Available\",\n        \"9th\": \"Unavailable\",\n        \"10th\": \"Unavailable\",\n        \"11th\": \"Unavailable\",\n        \"12th\": \"Unavailable\",\n        \"13th\": \"Unavailable\",\n        \"14th\": \"Available\",\n        \"15th\": \"Unavailable\",\n        \"16th\": \"Unavailable\",\n        \"17th\": \"Unavailable\",\n        \"18th\": \"Unavailable\",\n        \"19th\": \"Unavailable\",\n        \"20th\": \"Available\",\n        \"21st\": \"Available\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"dragon\",\n        \"4th\": \"Unavailable\",\n        \"5th\": \"Unavailable\",\n        \"6th\": \"Unavailable\",\n        \"7th\": \"Unavailable\",\n        \"8th\": \"Unavailable\",\n        \"9th\": \"Available\",\n        \"10th\": \"Unavailable\",\n        \"11th\": \"Unavailable\",\n        \"12th\": \"Unavailable\",\n        \"13th\": \"Unavailable\",\n        \"14th\": \"Unavailable\",\n        \"15th\": \"Available\",\n        \"16th\": \"Unavailable\",\n        \"17th\": \"Unavailable\",\n        \"18th\": \"Unavailable\",\n        \"19th\": \"Unavailable\",\n        \"20th\": \"Unavailable\",\n        \"21st\": \"Available\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"snake\",\n        \"4th\": \"10-11pm\",\n        \"5th\": \"10-11pm\",\n        \"6th\": \"10-11pm\",\n        \"7th\": \"10-11pm\",\n        \"8th\": \"10-11pm\",\n        \"9th\": \"10-11pm\",\n        \"10th\": \"Available\",\n        \"11th\": \"10-11pm\",\n        \"12th\": \"10-11pm\",\n        \"13th\": \"10-11pm\",\n        \"14th\": \"10-11pm\",\n        \"15th\": \"10-11pm\",\n        \"16th\": \"Available\",\n        \"17th\": \"10-11pm\",\n        \"18th\": \"10-11pm\",\n        \"19th\": \"10-11pm\",\n        \"20th\": \"10-11pm\",\n        \"21st\": \"10-11pm\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"horse\",\n        \"4th\": \"Unavailable\",\n        \"5th\": \"Unavailable\",\n        \"6th\": \"Unavailable\",\n        \"7th\": \"Unavailable\",\n        \"8th\": \"Unavailable\",\n        \"9th\": \"Unavailable\",\n        \"10th\": \"Unavailable\",\n        \"11th\": \"Available\",\n        \"12th\": \"Unavailable\",\n        \"13th\": \"Unavailable\",\n        \"14th\": \"Unavailable\",\n        \"15th\": \"Unavailable\",\n        \"16th\": \"Unavailable\",\n        \"17th\": \"Available\",\n        \"18th\": \"Unavailable\",\n        \"19th\": \"Unavailable\",\n        \"20th\": \"Unavailable\",\n        \"21st\": \"Unavailable\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"goat\",\n        \"4th\": \"Unavailable\",\n        \"5th\": \"Unavailable\",\n        \"6th\": \"Unavailable\",\n        \"7th\": \"Unavailable\",\n        \"8th\": \"Unavailable\",\n        \"9th\": \"Unavailable\",\n        \"10th\": \"Unavailable\",\n        \"11th\": \"Unavailable\",\n        \"12th\": \"Available\",\n        \"13th\": \"Unavailable\",\n        \"14th\": \"Unavailable\",\n        \"15th\": \"Unavailable\",\n        \"16th\": \"Unavailable\",\n        \"17th\": \"Unavailable\",\n        \"18th\": \"Available\",\n        \"19th\": \"Unavailable\",\n        \"20th\": \"Unavailable\",\n        \"21st\": \"Unavailable\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"rooster\",\n        \"4th\": \"5-6am\",\n        \"5th\": \"5-6am\",\n        \"6th\": \"5-6am\",\n        \"7th\": \"5-6am\",\n        \"8th\": \"5-6am\",\n        \"9th\": \"5-6am\",\n        \"10th\": \"5-6am\",\n        \"11th\": \"5-6am\",\n        \"12th\": \"5-6am\",\n        \"13th\": \"Available\",\n        \"14th\": \"5-6am\",\n        \"15th\": \"5-6am\",\n        \"16th\": \"5-6am\",\n        \"17th\": \"5-6am\",\n        \"18th\": \"5-6am\",\n        \"19th\": \"Available\",\n        \"20th\": \"5-6am\",\n        \"21st\": \"5-6am\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"dog\",\n        \"4th\": \"Unavailable\",\n        \"5th\": \"Unavailable\",\n        \"6th\": \"Unavailable\",\n        \"7th\": \"Unavailable\",\n        \"8th\": \"Unavailable\",\n        \"9th\": \"Unavailable\",\n        \"10th\": \"Unavailable\",\n        \"11th\": \"Unavailable\",\n        \"12th\": \"Unavailable\",\n        \"13th\": \"Unavailable\",\n        \"14th\": \"Available\",\n        \"15th\": \"Unavailable\",\n        \"16th\": \"Unavailable\",\n        \"17th\": \"Unavailable\",\n        \"18th\": \"Unavailable\",\n        \"19th\": \"Unavailable\",\n        \"20th\": \"Available\",\n        \"21st\": \"Unavailable\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"pig\",\n        \"4th\": \"Available\",\n        \"5th\": \"Unavailable\",\n        \"6th\": \"Unavailable\",\n        \"7th\": \"Unavailable\",\n        \"8th\": \"Unavailable\",\n        \"9th\": \"Unavailable\",\n        \"10th\": \"Unavailable\",\n        \"11th\": \"Unavailable\",\n        \"12th\": \"Unavailable\",\n        \"13th\": \"Unavailable\",\n        \"14th\": \"Unavailable\",\n        \"15th\": \"Available\",\n        \"16th\": \"Unavailable\",\n        \"17th\": \"Unavailable\",\n        \"18th\": \"Unavailable\",\n        \"19th\": \"Unavailable\",\n        \"20th\": \"Unavailable\",\n        \"21st\": \"Available\",\n        \"22nd\": \"Available\"\n    },\n    {\n        \"id\": \"monkey\",\n        \"4th\": \"Unavailable\",\n        \"5th\": \"Unavailable\",\n        \"6th\": \"Unavailable\",\n        \"7th\": \"Unavailable\",\n        \"8th\": \"Available\",\n        \"9th\": \"Unavailable\",\n        \"10th\": \"Unavailable\",\n        \"11th\": \"Unavailable\",\n        \"12th\": \"Unavailable\",\n        \"13th\": \"Unavailable\",\n        \"14th\": \"Unavailable\",\n        \"15th\": \"Unavailable\",\n        \"16th\": \"Unavailable\",\n        \"17th\": \"Unavailable\",\n        \"18th\": \"Unavailable\",\n        \"19th\": \"Unavailable\",\n        \"20th\": \"Unavailable\",\n        \"21st\": \"Unavailable\",\n        \"22nd\": \"Available\"\n    }\n]\n";

	private int m_currentChanceOneIn = 99;

	private string m_currentSelector = "Finished";

	private Dictionary<string, Dictionary<string, AvailableData>> m_characterAvailableData = new Dictionary<string, Dictionary<string, AvailableData>>();

	public string ChosenCharacter
	{
		get
		{
			return m_chosenCharacter;
		}
	}

	public Level SpawnSetPiece
	{
		get
		{
			return m_spawnSetPiece;
		}
	}

	public string Selector
	{
		get
		{
			if (m_currentSelectorOverride != string.Empty)
			{
				return m_currentSelectorOverride;
			}
			return m_currentSelector;
		}
	}

	private void LoadFromSchedule(JSONObject obj)
	{
		m_characterAvailableData.Clear();
		foreach (JSONObject item in obj.list)
		{
			string key = string.Empty;
			for (int i = 0; i < item.keys.Count; i++)
			{
				if (item.keys[i] == "id")
				{
					key = item.list[i].AsString;
					m_characterAvailableData.Add(key, new Dictionary<string, AvailableData>());
				}
				else
				{
					AvailableData value = new AvailableData(item.list[i].AsString);
					m_characterAvailableData[key].Add(item.keys[i], value);
				}
			}
		}
	}

	private void Start()
	{
		LoadFromSchedule(new JSONObject(m_defaultSchedule));
		m_currentChanceOneIn = m_startingChanceOneIn;
	}

	public void GiveCharacter()
	{
		if (Singleton<Game>.Instance.InventoryManager.GetCurrency(ChosenCharacter) == 0)
		{
			Singleton<Game>.Instance.InventoryManager.AddCurrency(ChosenCharacter, 1);
		}
		ResetChance();
		m_takenAny = true;
		SaveManager.Save();
	}

	public void LockChoice()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Dictionary<string, AvailableData>> characterAvailableDatum in m_characterAvailableData)
		{
			if (characterAvailableDatum.Value.ContainsKey(Selector) && characterAvailableDatum.Value[Selector].IsAvailable() && Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key).Locked)
			{
				list.Add(characterAvailableDatum.Key);
			}
		}
		m_chosenCharacter = list[UnityEngine.Random.Range(0, list.Count)];
	}

	public void ResetChance()
	{
		m_currentChanceOneIn = m_startingChanceOneIn;
	}

	public bool RequestChance()
	{
		if (!AnythingAvailable())
		{
			return false;
		}
		if (Singleton<Game>.Instance.LevelManager.CurrentLevelIndex + 1 >= 2 && !m_takenAny)
		{
			return true;
		}
		if (m_currentChanceOneIn > m_startingChanceOneIn)
		{
			m_currentChanceOneIn = m_startingChanceOneIn;
		}
		bool flag = UnityEngine.Random.Range(0f, 1f) <= 1f / (float)m_currentChanceOneIn;
		if (!flag)
		{
			m_currentChanceOneIn--;
		}
		return flag;
	}

	public string GetID()
	{
		return m_ID;
	}

	private void RequestSchedule()
	{
		string scheduleURL = m_scheduleURL;
		scheduleURL = scheduleURL + "?p=" + DateTime.Now.Ticks;
		WWW www = new WWW(scheduleURL);
		StartCoroutine(WaitForRequestSchedule(www));
	}

	private void RequestCurrentDay()
	{
		string currentSelectorURL = m_currentSelectorURL;
		currentSelectorURL = currentSelectorURL + "?p=" + DateTime.Now.Ticks;
		WWW www = new WWW(currentSelectorURL);
		StartCoroutine(WaitForRequestSelector(www));
	}

	private IEnumerator WaitForRequestSchedule(WWW www)
	{
		yield return www;
		if (www.error != null)
		{
			Debug.Log("Error getting " + www.url);
			yield break;
		}
		string result = www.text;
		LoadFromSchedule(new JSONObject(result));
	}

	public string CurrentDay()
	{
		return Selector;
	}

	private IEnumerator WaitForRequestSelector(WWW www)
	{
		yield return www;
		if (www.error != null)
		{
			Debug.Log("Error getting " + www.url);
			yield break;
		}
		string oldSelector = m_currentSelector;
		m_currentSelector = www.text.Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
		if (oldSelector != m_currentSelector && Singleton<Game>.Instance != null && Singleton<Game>.Instance.Player != null)
		{
			Singleton<Game>.Instance.Player.BuildUnlockableList();
		}
	}

	public bool AnythingAvailable()
	{
		foreach (KeyValuePair<string, Dictionary<string, AvailableData>> characterAvailableDatum in m_characterAvailableData)
		{
			if (characterAvailableDatum.Value.ContainsKey(Selector) && characterAvailableDatum.Value[Selector].IsAvailable() && Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key) != null && Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key).Locked)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAvailable(string ID)
	{
		if (!m_characterAvailableData.ContainsKey(ID))
		{
			return false;
		}
		if (!m_characterAvailableData[ID].ContainsKey(Selector))
		{
			return false;
		}
		return m_characterAvailableData[ID][Selector].IsAvailable();
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		jSONObject.AddField("chance", m_currentChanceOneIn);
		jSONObject.AddField("takenAny", m_takenAny);
		jSONObject.AddField("selector", m_currentSelector);
		return jSONObject;
	}

	public bool Verify(JSONObject data)
	{
		return true;
	}

	public void Load(JSONObject data)
	{
		if (data != null && (bool)data.GetField("chance"))
		{
			m_currentChanceOneIn = data.GetField("chance").AsInt;
		}
		if (data != null && (bool)data.GetField("takenAny"))
		{
			m_takenAny = data.GetField("takenAny").AsBool;
		}
		if (data != null && (bool)data.GetField("selector") && m_currentSelector == "10th")
		{
			m_currentSelector = data.GetField("selector").AsString;
		}
	}

	public void Merge(JSONObject dataA, JSONObject dataB)
	{
	}

	public JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB)
	{
		return dataA;
	}

	public JSONObject Reset()
	{
		JSONObject result = new JSONObject();
		m_currentChanceOneIn = m_startingChanceOneIn;
		return result;
	}

	public void Update()
	{
		if (Time.realtimeSinceStartup > 600f && Time.realtimeSinceStartup - m_timeAtLastSync > m_syncMinutes * 60f)
		{
			m_timeAtLastSync = Time.realtimeSinceStartup;
			RequestSchedule();
			RequestCurrentDay();
		}
	}

	public bool AnyLocked()
	{
		foreach (KeyValuePair<string, Dictionary<string, AvailableData>> characterAvailableDatum in m_characterAvailableData)
		{
			if (Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key) != null && Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key).Locked)
			{
				return true;
			}
		}
		return false;
	}

	public void BuyRemaining()
	{
		int num = 0;
		foreach (KeyValuePair<string, Dictionary<string, AvailableData>> characterAvailableDatum in m_characterAvailableData)
		{
			if (Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key) != null && Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key).Locked)
			{
				num++;
			}
		}
		string text = m_buyAllIAP[num];
		Singleton<Game>.Instance.InventoryManager.PurchaseIAP("com.prettygreat.landsliders." + text, PurchaseAllCallback);
	}

	public string GetBuyIAP()
	{
		int num = 0;
		foreach (KeyValuePair<string, Dictionary<string, AvailableData>> characterAvailableDatum in m_characterAvailableData)
		{
			if (Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key) != null && Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key).Locked)
			{
				num++;
			}
		}
		string text = m_buyAllIAP[num];
		return "com.prettygreat.landsliders." + text;
	}

	public void PurchaseAllCallback(bool success, string id)
	{
		if (success)
		{
			foreach (KeyValuePair<string, Dictionary<string, AvailableData>> characterAvailableDatum in m_characterAvailableData)
			{
				if (Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key) != null && Singleton<Game>.Instance.Player.GetCharacterInfo(characterAvailableDatum.Key).Locked)
				{
					Singleton<Game>.Instance.InventoryManager.AddCurrency(characterAvailableDatum.Key, 1);
				}
			}
			Singleton<Game>.Instance.StatsManager.GetStat("goldenPack4").IncreaseStat(1f);
			Singleton<Game>.Instance.StatsManager.GetStat("goldenPack8").IncreaseStat(1f);
			Singleton<Game>.Instance.UIController.HUD.BuyAllSpecialSuccess();
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
		return "SpecialEventCNY";
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
		return false;
	}
}
