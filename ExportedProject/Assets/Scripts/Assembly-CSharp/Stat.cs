using System;
using System.Collections.Generic;

public class Stat
{
	protected bool m_active;

	protected StatValue m_level;

	protected StatValue m_life;

	protected Dictionary<string, StatValue> m_character = new Dictionary<string, StatValue>();

	protected StatValue m_characterCurrent;

	protected StatValue m_total;

	public StatValue Level
	{
		get
		{
			return m_level;
		}
	}

	public StatValue Life
	{
		get
		{
			return m_life;
		}
	}

	public StatValue Total
	{
		get
		{
			return m_total;
		}
	}

	public StatValue Character
	{
		get
		{
			return m_characterCurrent;
		}
	}

	public bool IsActive
	{
		get
		{
			return m_active;
		}
		set
		{
			m_active = value;
			m_level.IsActive = m_active;
			m_life.IsActive = m_active;
			Character.IsActive = m_active;
			m_total.IsActive = m_active;
		}
	}

	public void SetValue(float value)
	{
		m_level.Current = value;
		m_life.Current = value;
		m_total.Current = value;
		Character.Current = value;
	}

	public void IncreaseStat(float value)
	{
		m_level.Current += value;
		m_life.Current += value;
		m_total.Current += value;
		if (Character != null)
		{
			Character.Current += value;
		}
	}

	public void DecreaseStat(float value)
	{
		m_level.Current -= value;
		m_life.Current -= value;
		m_total.Current -= value;
		Character.Current -= value;
	}

	public void Update()
	{
		m_level.Update();
		m_life.Update();
		m_total.Update();
		Character.Update();
	}

	public virtual void Init()
	{
		m_level = new StatValue();
		m_life = new StatValue();
		m_character = new Dictionary<string, StatValue>();
		if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
		{
			SetCurrentCharacter();
		}
		m_total = new StatValue();
	}

	public virtual StatValue GetCharacter(string name)
	{
		if (!m_character.ContainsKey(name))
		{
			StatValue value = new StatValue();
			m_character.Add(name, value);
		}
		return m_character[name];
	}

	public virtual void SetCurrentCharacter()
	{
		if (!m_character.ContainsKey(Singleton<Game>.Instance.Player.CharacterId))
		{
			StatValue value = new StatValue();
			m_character.Add(Singleton<Game>.Instance.Player.CharacterId, value);
		}
		m_characterCurrent = m_character[Singleton<Game>.Instance.Player.CharacterId];
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("lev", m_level.Save());
		jSONObject.AddField("life", m_life.Save());
		JSONObject jSONObject2 = new JSONObject();
		foreach (KeyValuePair<string, StatValue> item in m_character)
		{
			jSONObject2.AddField(item.Value.GetType().ToString() + item.Key, item.Value.Save());
		}
		jSONObject.AddField("char", jSONObject2);
		jSONObject.AddField("tot", m_total.Save());
		return jSONObject;
	}

	public void Load(JSONObject data)
	{
		m_level.Load(data.GetField("lev"));
		m_life.Load(data.GetField("life"));
		JSONObject field = data.GetField("char");
		if (field != null && field.keys != null)
		{
			foreach (string key in field.keys)
			{
				if (key.StartsWith(typeof(StatValue).ToString(), StringComparison.Ordinal))
				{
					StatValue statValue = new StatValue();
					statValue.Load(field[key]);
					m_character.Add(key.Replace(typeof(StatValue).ToString(), string.Empty), statValue);
				}
				else if (key.StartsWith(typeof(StatTimerValue).ToString(), StringComparison.Ordinal))
				{
					StatTimerValue statTimerValue = new StatTimerValue();
					statTimerValue.Load(field[key]);
					m_character.Add(key.Replace(typeof(StatTimerValue).ToString(), string.Empty), statTimerValue);
				}
			}
		}
		m_total.Load(data.GetField("tot"));
	}

	public static JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB)
	{
		JSONObject obj = StatValue.MergeToJSON(GetFieldSafe(dataA, "lev"), GetFieldSafe(dataB, "lev"));
		JSONObject obj2 = StatValue.MergeToJSON(GetFieldSafe(dataA, "life"), GetFieldSafe(dataB, "life"));
		JSONObject obj3 = StatValue.MergeToJSON(GetFieldSafe(dataA, "tot"), GetFieldSafe(dataB, "tot"));
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("lev", obj);
		jSONObject.AddField("life", obj2);
		JSONObject fieldSafe = GetFieldSafe(dataA, "char");
		JSONObject fieldSafe2 = GetFieldSafe(dataB, "char");
		JSONObject jSONObject2 = new JSONObject();
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			string text = typeof(StatValue).ToString() + availableCharacter.Id;
			string text2 = typeof(StatTimerValue).ToString() + availableCharacter.Id;
			JSONObject fieldSafe3 = GetFieldSafe(fieldSafe, text);
			JSONObject fieldSafe4 = GetFieldSafe(fieldSafe2, text);
			JSONObject fieldSafe5 = GetFieldSafe(fieldSafe, text2);
			JSONObject fieldSafe6 = GetFieldSafe(fieldSafe2, text2);
			if (fieldSafe3 != null || fieldSafe4 != null || fieldSafe5 != null || fieldSafe6 != null)
			{
				jSONObject2.AddField(text, StatValue.MergeToJSON(fieldSafe3, fieldSafe4));
				jSONObject2.AddField(text2, StatValue.MergeToJSON(fieldSafe5, fieldSafe6));
			}
		}
		jSONObject.AddField("char", jSONObject2);
		jSONObject.AddField("tot", obj3);
		return jSONObject;
	}

	public void Merge(JSONObject dataA, JSONObject dataB)
	{
		m_level.Merge(GetFieldSafe(dataA, "lev"), GetFieldSafe(dataB, "lev"));
		m_life.Merge(GetFieldSafe(dataA, "life"), GetFieldSafe(dataB, "life"));
		m_total.Merge(GetFieldSafe(dataA, "tot"), GetFieldSafe(dataB, "tot"));
		JSONObject fieldSafe = GetFieldSafe(dataA, "char");
		JSONObject fieldSafe2 = GetFieldSafe(dataB, "char");
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			string text = typeof(StatValue).ToString() + availableCharacter.Id;
			string text2 = typeof(StatTimerValue).ToString() + availableCharacter.Id;
			JSONObject fieldSafe3 = GetFieldSafe(fieldSafe, text);
			JSONObject fieldSafe4 = GetFieldSafe(fieldSafe2, text);
			JSONObject fieldSafe5 = GetFieldSafe(fieldSafe, text2);
			JSONObject fieldSafe6 = GetFieldSafe(fieldSafe2, text2);
			if (fieldSafe3 != null || fieldSafe4 != null || fieldSafe5 != null || fieldSafe6 != null)
			{
				StatValue statValue = null;
				StatValue statValue2 = null;
				if (!m_character.ContainsKey(text.Replace(typeof(StatValue).ToString(), string.Empty)))
				{
					statValue = new StatValue();
					m_character.Add(text.Replace(typeof(StatValue).ToString(), string.Empty), statValue);
				}
				else
				{
					statValue = m_character[text.Replace(typeof(StatValue).ToString(), string.Empty)];
				}
				if (!m_character.ContainsKey(text2.Replace(typeof(StatTimerValue).ToString(), string.Empty)))
				{
					statValue2 = new StatTimerValue();
					m_character.Add(text2.Replace(typeof(StatTimerValue).ToString(), string.Empty), statValue2);
				}
				else
				{
					statValue2 = m_character[text2.Replace(typeof(StatTimerValue).ToString(), string.Empty)];
				}
				statValue.Merge(fieldSafe3, fieldSafe4);
				statValue2.Merge(fieldSafe5, fieldSafe6);
			}
		}
	}

	private static JSONObject GetFieldSafe(JSONObject data, string key)
	{
		if (data == null)
		{
			return null;
		}
		if (data.HasField(key))
		{
			return data.GetField(key);
		}
		return null;
	}
}
