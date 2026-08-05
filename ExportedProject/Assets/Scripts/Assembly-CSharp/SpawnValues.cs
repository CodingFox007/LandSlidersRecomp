using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnValues : MonoBehaviour, ISerializationCallbackReceiver
{
	[SerializeField]
	private string m_group;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_density;

	[SerializeField]
	private string m_densityPerkId;

	[SerializeField]
	private bool m_landBased = true;

	[SerializeField]
	private bool m_randomRotated = true;

	[SerializeField]
	private bool m_rotateWithTile;

	[SerializeField]
	private string m_maxCurrencyCheck;

	[SerializeField]
	private int m_maxCurrencyAmount;

	[FormerlySerializedAs("m_perkID")]
	[SerializeField]
	private string m_spawnCountOverridePerkID;

	[Dictionary("m_spawnChanceValues")]
	public List<GameObject> m_spawnChance;

	[HideInInspector]
	public List<float> m_spawnChanceValues;

	private Dictionary<GameObject, float> m_spawnChances;

	[Dictionary("m_spawnCountValues")]
	public List<int> m_spawnCountOverride;

	[HideInInspector]
	public List<float> m_spawnCountValues;

	private Dictionary<int, float> m_spawnCounts;

	public Dictionary<GameObject, float> SpawnChances
	{
		get
		{
			return m_spawnChances;
		}
	}

	public Dictionary<int, float> SpawnCounts
	{
		get
		{
			return m_spawnCounts;
		}
	}

	public string GroupName
	{
		get
		{
			return m_group;
		}
	}

	public bool IsLandBased
	{
		get
		{
			return m_landBased;
		}
	}

	public bool IsRandomRotated
	{
		get
		{
			return m_randomRotated;
		}
	}

	public bool RotateWithTile
	{
		get
		{
			return m_rotateWithTile;
		}
	}

	public float Density
	{
		get
		{
			if (m_maxCurrencyCheck != string.Empty && Singleton<Game>.Instance.InventoryManager.GetCurrency(m_maxCurrencyCheck) >= m_maxCurrencyAmount)
			{
				return 0f;
			}
			return Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue(m_densityPerkId, m_density);
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_spawnChance, m_spawnChanceValues, out m_spawnChances, "spawnChances");
		Utils.InitDictionary(m_spawnCountOverride, m_spawnCountValues, out m_spawnCounts, "spawnCounts");
	}

	public GameObject GetRandomPrefab(Level level)
	{
		if (m_spawnChances.Count <= 0)
		{
			return null;
		}
		float num = 0f;
		foreach (float value in m_spawnChances.Values)
		{
			float num2 = value;
			num += num2;
		}
		float nextRandomRange = level.GetNextRandomRange(0f, num);
		num = 0f;
		foreach (KeyValuePair<GameObject, float> spawnChance in m_spawnChances)
		{
			num += spawnChance.Value;
			if (num >= nextRandomRange)
			{
				return spawnChance.Key;
			}
		}
		return null;
	}

	public int GetRandomCount(Level level)
	{
		if (m_spawnCounts.Count <= 0)
		{
			return 0;
		}
		float num = 0f;
		foreach (float value in m_spawnCounts.Values)
		{
			float num2 = value;
			num += num2;
		}
		float num3 = 0f;
		foreach (KeyValuePair<int, float> spawnCount in m_spawnCounts)
		{
			num3 = ((spawnCount.Key != 0) ? (num3 + Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue(m_spawnCountOverridePerkID, spawnCount.Value)) : (num3 + spawnCount.Value));
		}
		float nextRandomRange = level.GetNextRandomRange(0f, num3);
		num3 = 0f;
		foreach (KeyValuePair<int, float> spawnCount2 in m_spawnCounts)
		{
			num3 = ((spawnCount2.Key != 0) ? (num3 + Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue(m_spawnCountOverridePerkID, spawnCount2.Value)) : (num3 + spawnCount2.Value));
			if (num3 >= nextRandomRange)
			{
				return spawnCount2.Key;
			}
		}
		return 0;
	}
}
