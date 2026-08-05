using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour, Savable
{
	private Dictionary<string, Stat> m_stats = new Dictionary<string, Stat>();

	private bool m_hasCharacter;

	public void StartLevel()
	{
		foreach (Stat value in m_stats.Values)
		{
			value.Level.Reset();
		}
	}

	public void PlayerStartLevel()
	{
		Singleton<Game>.Instance.StatsManager.GetStatTimer("PlayTime").Level.Reset();
	}

	public void StartLife()
	{
		Singleton<Game>.Instance.StatsManager.GetStatTimer("AliveTime").Level.Reset();
		foreach (Stat value in m_stats.Values)
		{
			value.Life.Reset();
		}
	}

	public void StartGame()
	{
		foreach (Stat value in m_stats.Values)
		{
			value.Life.Reset();
		}
	}

	private void PreCharacterChanged()
	{
	}

	private void CharacterChanged()
	{
		foreach (Stat value in m_stats.Values)
		{
			value.SetCurrentCharacter();
		}
		m_hasCharacter = true;
	}

	public void Init()
	{
		Singleton<Game>.Instance.LevelManager.OnStartNewLife += StartLife;
		Singleton<Game>.Instance.LevelManager.OnLevelLoadComplete += StartLevel;
		Singleton<Game>.Instance.LevelManager.OnStartLevelLoadComplete += StartGame;
		Singleton<Game>.Instance.Player.OnPreCharacterChanged += PreCharacterChanged;
		Singleton<Game>.Instance.Player.OnCharacterChanged += CharacterChanged;
		Singleton<Game>.Instance.Player.OnPlayerStartLevel += PlayerStartLevel;
	}

	private void Update()
	{
		if (!Singleton<Game>.Instance.IsLoaded || !m_hasCharacter)
		{
			return;
		}
		foreach (Stat value in m_stats.Values)
		{
			value.Update();
		}
	}

	public bool HasStat(string id)
	{
		return m_stats.ContainsKey(id);
	}

	public Stat GetStat(string id)
	{
		if (!m_stats.ContainsKey(id))
		{
			Stat stat = new Stat();
			stat.Init();
			m_stats.Add(id, stat);
		}
		return m_stats[id];
	}

	public Stat GetStatTimer(string id)
	{
		if (!m_stats.ContainsKey(id))
		{
			StatTimer statTimer = new StatTimer();
			statTimer.Init();
			m_stats.Add(id, statTimer);
		}
		if (m_stats[id].GetType() != typeof(StatTimer))
		{
			Debug.Log("Timer Invalid");
		}
		return m_stats[id];
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		JSONObject jSONObject2 = new JSONObject();
		foreach (KeyValuePair<string, Stat> stat in m_stats)
		{
			jSONObject2.AddField(stat.Value.GetType().ToString() + stat.Key, stat.Value.Save());
		}
		jSONObject.AddField("stats", jSONObject2);
		return jSONObject;
	}

	public bool Verify(JSONObject data)
	{
		return true;
	}

	public void Load(JSONObject data)
	{
		m_stats.Clear();
		if (!data.HasField("stats"))
		{
			return;
		}
		JSONObject field = data.GetField("stats");
		if (field == null || field.ToString() == "null")
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (string key in field.keys)
		{
			string text = key.Substring(4);
			if (Singleton<Game>.Instance.Player.GetCharacterInfo(text) != null && text != "phil" && text != "hugh" && text != "baz" && text != "luke" && Singleton<Game>.Instance.InventoryManager.GetCurrency(text) == 0)
			{
				list.Add(text);
			}
			if (key.StartsWith(typeof(StatTimer).ToString(), StringComparison.Ordinal))
			{
				if (m_stats.ContainsKey(key.Replace(typeof(StatTimer).ToString(), string.Empty)))
				{
					Debug.LogError("Duplicate Stat Found: " + key);
				}
				StatTimer statTimer = new StatTimer();
				statTimer.Init();
				statTimer.Load(field[key]);
				m_stats.Add(key.Replace(typeof(StatTimer).ToString(), string.Empty), statTimer);
			}
			else if (key.StartsWith(typeof(Stat).ToString(), StringComparison.Ordinal))
			{
				if (m_stats.ContainsKey(key.Replace(typeof(Stat).ToString(), string.Empty)))
				{
					Debug.LogError("Duplicate Stat Found: " + key);
				}
				Stat stat = new Stat();
				stat.Init();
				stat.Load(field[key]);
				m_stats.Add(key.Replace(typeof(Stat).ToString(), string.Empty), stat);
			}
		}
		foreach (string item in list)
		{
			Singleton<Game>.Instance.InventoryManager.AddCurrency(item, 1);
		}
	}

	public JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB)
	{
		JSONObject jSONObject = null;
		JSONObject jSONObject2 = null;
		JSONObject jSONObject3 = dataA;
		JSONObject jSONObject4 = null;
		if (dataA != null && dataA.HasField("stats") && dataA.GetField("stats").ToString() != "null")
		{
			jSONObject = dataA.GetField("stats");
			jSONObject3 = new JSONObject(dataA);
			jSONObject4 = jSONObject3.GetField("stats");
		}
		if (dataB != null && dataB.HasField("stats") && dataB.GetField("stats").ToString() != "null")
		{
			jSONObject2 = dataB.GetField("stats");
			jSONObject3 = new JSONObject(dataB);
			jSONObject4 = jSONObject3.GetField("stats");
		}
		JSONObject dataA2 = null;
		JSONObject dataB2 = null;
		JSONObject dataA3 = null;
		JSONObject dataB3 = null;
		string text = typeof(Stat).ToString() + "collectible";
		string text2 = typeof(StatTimer).ToString() + "collectible";
		if (jSONObject != null && jSONObject.keys.Contains(text))
		{
			dataA2 = jSONObject[text];
		}
		if (jSONObject2 != null && jSONObject2.keys.Contains(text))
		{
			dataB2 = jSONObject2[text];
		}
		if (jSONObject != null && jSONObject.keys.Contains(text2))
		{
			dataA3 = jSONObject[text2];
		}
		if (jSONObject2 != null && jSONObject2.keys.Contains(text2))
		{
			dataB3 = jSONObject2[text2];
		}
		JSONObject obj = Stat.MergeToJSON(dataA2, dataB2);
		JSONObject obj2 = Stat.MergeToJSON(dataA3, dataB3);
		if (jSONObject4.HasField(text))
		{
			jSONObject4.SetField(text, obj);
		}
		else
		{
			jSONObject4.AddField(text, obj);
		}
		if (jSONObject4.HasField(text2))
		{
			jSONObject4.SetField(text2, obj2);
		}
		else
		{
			jSONObject4.AddField(text2, obj2);
		}
		return jSONObject3;
	}

	public void Merge(JSONObject dataA, JSONObject dataB)
	{
		JSONObject jSONObject = null;
		JSONObject jSONObject2 = null;
		if (dataA != null && dataA.HasField("stats"))
		{
			jSONObject = dataA.GetField("stats");
		}
		if (dataB != null && dataB.HasField("stats"))
		{
			jSONObject2 = dataB.GetField("stats");
		}
		if (jSONObject != null && jSONObject.ToString() == "null")
		{
			jSONObject = null;
		}
		if (jSONObject2 != null && jSONObject2.ToString() == "null")
		{
			jSONObject2 = null;
		}
		JSONObject jSONObject3 = null;
		JSONObject jSONObject4 = null;
		JSONObject jSONObject5 = null;
		JSONObject jSONObject6 = null;
		string text = typeof(Stat).ToString() + "collectible";
		string text2 = typeof(StatTimer).ToString() + "collectible";
		if (jSONObject != null && jSONObject.keys.Contains(text))
		{
			jSONObject3 = jSONObject[text];
		}
		if (jSONObject2 != null && jSONObject2.keys.Contains(text))
		{
			jSONObject4 = jSONObject2[text];
		}
		if (jSONObject != null && jSONObject.keys.Contains(text2))
		{
			jSONObject5 = jSONObject[text2];
		}
		if (jSONObject2 != null && jSONObject2.keys.Contains(text2))
		{
			jSONObject6 = jSONObject2[text2];
		}
		if (jSONObject3 != null)
		{
			Debug.Log("StatsManager Merge Save statA: " + jSONObject3.ToString());
		}
		else
		{
			Debug.Log("StatsManager Merge Save statA: null");
		}
		if (jSONObject4 != null)
		{
			Debug.Log("StatsManager Merge Save statB: " + jSONObject4.ToString());
		}
		else
		{
			Debug.Log("StatsManager Merge Save statB: null");
		}
		if (jSONObject5 != null)
		{
			Debug.Log("StatsManager Merge Save statTimerA: " + jSONObject5.ToString());
		}
		else
		{
			Debug.Log("StatsManager Merge Save statTimerA: null");
		}
		if (jSONObject6 != null)
		{
			Debug.Log("StatsManager Merge Save statTimerB: " + jSONObject6.ToString());
		}
		else
		{
			Debug.Log("StatsManager Merge Save statTimerB: null");
		}
		Stat stat = null;
		Stat stat2 = null;
		if (!m_stats.ContainsKey(text.Replace(typeof(Stat).ToString(), string.Empty)))
		{
			stat = new Stat();
			stat.Init();
			m_stats.Add(text.Replace(typeof(Stat).ToString(), string.Empty), stat);
		}
		else
		{
			stat = m_stats[text.Replace(typeof(Stat).ToString(), string.Empty)];
		}
		if (!m_stats.ContainsKey(text2.Replace(typeof(StatTimer).ToString(), string.Empty)))
		{
			stat2 = new StatTimer();
			stat2.Init();
			m_stats.Add(text2.Replace(typeof(StatTimer).ToString(), string.Empty), stat2);
		}
		else
		{
			stat2 = m_stats[text2.Replace(typeof(StatTimer).ToString(), string.Empty)];
		}
		stat.Merge(jSONObject3, jSONObject4);
		stat2.Merge(jSONObject5, jSONObject6);
	}

	public JSONObject Reset()
	{
		m_stats.Clear();
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
		return "Stats";
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

	public object Parse()
	{
		object result = null;
		if (Utils.ParseTokenCount() < 4)
		{
			return result;
		}
		Stat stat = null;
		stat = ((Utils.ParseTokenHash(0) != "StatsTimer".GetHashCode()) ? GetStat(Utils.ParseToken(1)) : GetStatTimer(Utils.ParseToken(1)));
		if (stat == null)
		{
			return result;
		}
		StatValue statValue = null;
		if (Utils.ParseTokenHash(2) == "Level".GetHashCode())
		{
			statValue = stat.Level;
		}
		else if (Utils.ParseTokenHash(2) == "Life".GetHashCode())
		{
			statValue = stat.Life;
		}
		else if (Utils.ParseTokenHash(2) == "Character".GetHashCode())
		{
			statValue = stat.Character;
		}
		else if (Utils.ParseTokenHash(2) == "Total".GetHashCode())
		{
			statValue = stat.Total;
		}
		if (statValue == null && Utils.ParseToken(2).StartsWith("Character", StringComparison.Ordinal))
		{
			string text = Utils.ParseToken(2).Replace("Character", string.Empty);
			statValue = stat.GetCharacter(text);
		}
		if (statValue == null)
		{
			return result;
		}
		if (Utils.ParseTokenHash(3) == "Current".GetHashCode())
		{
			result = statValue.Current;
		}
		else if (Utils.ParseTokenHash(3) == "Highest".GetHashCode())
		{
			result = statValue.Highest;
		}
		else if (Utils.ParseTokenHash(3) == "Lowest".GetHashCode())
		{
			result = statValue.Lowest;
		}
		return result;
	}
}
