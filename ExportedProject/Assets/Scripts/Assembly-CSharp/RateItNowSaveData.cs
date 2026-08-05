using System;

public class RateItNowSaveData : Savable
{
	public long m_timeToRemind;

	public int m_timesPresented;

	private static RateItNowSaveData _instance;

	public static RateItNowSaveData Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new RateItNowSaveData();
			}
			return _instance;
		}
	}

	public void SetRemindTime(long value)
	{
		m_timeToRemind = Max(value, m_timeToRemind);
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		if (m_timeToRemind > 0)
		{
			string val = m_timeToRemind.ToString();
			if (m_timeToRemind >= DateTime.MaxValue.Ticks)
			{
				val = "never-v1.2";
			}
			jSONObject.AddField("timeToRemind", val);
		}
		if (m_timesPresented > 0)
		{
			jSONObject.AddField("timesPresented", m_timesPresented.ToString());
		}
		return jSONObject;
	}

	public void Load(JSONObject data)
	{
		m_timeToRemind = GetTimeToRemindValue(data);
	}

	public bool Verify(JSONObject data)
	{
		return true;
	}

	public long GetTimeToRemindValue(JSONObject data)
	{
		if (data != null && (bool)data.GetField("timeToRemind"))
		{
			string asString = data.GetField("timeToRemind").AsString;
			if (asString == "never")
			{
				return 0L;
			}
			if (asString == "never-v1.2")
			{
				return long.MaxValue;
			}
			if (long.Parse(asString) == long.MaxValue)
			{
				return 0L;
			}
			return long.Parse(asString);
		}
		return 0L;
	}

	public int GetTimesPresented(JSONObject data)
	{
		if (data != null && (bool)data.GetField("timesPresented"))
		{
			return int.Parse(data.GetField("timesPresented").AsString);
		}
		return 0;
	}

	private static T Max<T>(T a, T b) where T : IComparable<T>
	{
		return (a.CompareTo(b) <= 0) ? b : a;
	}

	public JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB)
	{
		return dataA;
	}

	public void Merge(JSONObject dataA, JSONObject dataB)
	{
		m_timeToRemind = Max(GetTimeToRemindValue(dataA), GetTimeToRemindValue(dataB));
		m_timesPresented = Max(GetTimesPresented(dataA), GetTimesPresented(dataB));
	}

	public JSONObject Reset()
	{
		JSONObject result = new JSONObject();
		m_timeToRemind = 0L;
		return result;
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
		return "RateItNow";
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
}
