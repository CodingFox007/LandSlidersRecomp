using System;
using UnityEngine;

[Serializable]
public class RandomRange
{
	public float m_min;

	public float m_max;

	private float m_value = float.NaN;

	public float Value
	{
		get
		{
			if (m_value == float.NaN)
			{
				Randomize();
			}
			return m_value;
		}
	}

	public float RandomValue
	{
		get
		{
			return UnityEngine.Random.Range(m_min, m_max);
		}
	}

	public int RandomInt
	{
		get
		{
			return Mathf.FloorToInt(UnityEngine.Random.Range(m_min, m_max));
		}
	}

	public RandomRange(float min, float max)
	{
		m_min = min;
		m_max = max;
	}

	public void Randomize()
	{
		m_value = UnityEngine.Random.Range(m_min, m_max);
	}
}
