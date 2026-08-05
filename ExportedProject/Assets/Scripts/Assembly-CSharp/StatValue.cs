using UnityEngine;

public class StatValue
{
	protected bool m_active;

	protected float m_current;

	protected float m_highest;

	protected float m_lowest;

	public float Current
	{
		get
		{
			return m_current;
		}
		set
		{
			m_current = value;
			m_highest = Mathf.Max(m_highest, m_current);
			m_lowest = Mathf.Min(m_lowest, m_current);
		}
	}

	public float Highest
	{
		get
		{
			return m_highest;
		}
	}

	public float Lowest
	{
		get
		{
			return m_lowest;
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
		}
	}

	public StatValue()
	{
		m_active = true;
	}

	public virtual void Reset()
	{
		m_current = 0f;
		m_active = true;
	}

	public virtual void Update()
	{
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("curr", m_current);
		jSONObject.AddField("high", m_highest);
		jSONObject.AddField("low", m_lowest);
		return jSONObject;
	}

	public void Load(JSONObject data)
	{
		m_current = data.GetField("curr").AsFloat;
		m_highest = data.GetField("high").AsFloat;
		m_lowest = data.GetField("low").AsFloat;
	}

	public void Merge(JSONObject dataA, JSONObject dataB)
	{
		float maxFloat = GetMaxFloat("curr", dataA, dataB);
		m_current = m_current;
		float maxFloat2 = GetMaxFloat("high", dataA, dataB);
		m_highest = Mathf.Max(m_highest, maxFloat2);
		float minFloat = GetMinFloat("low", dataA, dataB);
		m_lowest = Mathf.Min(m_lowest, minFloat);
	}

	public static JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB)
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("curr", GetMaxFloat("curr", dataA, dataB));
		jSONObject.AddField("high", GetMaxFloat("high", dataA, dataB));
		jSONObject.AddField("low", GetMinFloat("low", dataA, dataB));
		return jSONObject;
	}

	private static float GetMaxFloat(string key, JSONObject dataA, JSONObject dataB)
	{
		float a = 0f;
		float b = 0f;
		if (dataA != null && dataA.HasField(key))
		{
			a = dataA.GetField(key).AsFloat;
		}
		if (dataB != null && dataB.HasField(key))
		{
			b = dataB.GetField(key).AsFloat;
		}
		return Mathf.Max(a, b);
	}

	private static float GetMinFloat(string key, JSONObject dataA, JSONObject dataB)
	{
		float a = 0f;
		float b = 0f;
		if (dataA != null && dataA.HasField(key))
		{
			a = dataA.GetField(key).AsFloat;
		}
		if (dataB != null && dataB.HasField(key))
		{
			b = dataB.GetField(key).AsFloat;
		}
		return Mathf.Min(a, b);
	}
}
