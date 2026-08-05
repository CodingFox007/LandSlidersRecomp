using System;
using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;

[Serializable]
public class LevelTileSet : MonoBehaviour, ISerializationCallbackReceiver
{
	[Serializable]
	public class WallTileSet
	{
		public HexType type;

		public GameObject[] walls;
	}

	[Serializable]
	public class WaterTileSet
	{
		public GameObject waterTile;

		public GameObject waterQuater;

		public GameObject waterHalf;

		public GameObject wakeIn;

		public GameObject wakeOut;

		public GameObject wakeQuater;

		public GameObject wakeHalf;

		public GameObject wakeQuaterLeft;

		public GameObject wakeQuaterRight;

		public GameObject wakeQuaterHalfLeft;

		public GameObject wakeQuaterHalfRight;

		public GameObject wakeHalfHalf;

		public GameObject wakeHalfCorner;
	}

	[Dictionary("m_tilesLowValues")]
	public List<HexType> m_tilesLow;

	[HideInInspector]
	public List<GameObject> m_tilesLowValues;

	private Dictionary<HexType, GameObject> m_tileLowGeometry;

	[Dictionary("m_tilesMidValues")]
	public List<HexType> m_tilesMid;

	[HideInInspector]
	public List<GameObject> m_tilesMidValues;

	private Dictionary<HexType, GameObject> m_tileMidGeometry;

	[Dictionary("m_tilesHighValues")]
	public List<HexType> m_tilesHigh;

	[HideInInspector]
	public List<GameObject> m_tilesHighValues;

	private Dictionary<HexType, GameObject> m_tileHighGeometry;

	[SerializeField]
	private WallTileSet[] m_walls;

	[SerializeField]
	private GameObject m_dynamicTileWall;

	[SerializeField]
	private WaterTileSet m_water;

	[Dictionary("m_scenerySpawnChanceValues")]
	public List<GameObject> m_scenerySpawnChance;

	[HideInInspector]
	public List<float> m_scenerySpawnChanceValues;

	private Dictionary<GameObject, float> m_scenerySpawnChances;

	[Dictionary("m_waterScenerySpawnChanceValues")]
	public List<GameObject> m_waterScenerySpawnChance;

	[HideInInspector]
	public List<float> m_waterScenerySpawnChanceValues;

	private Dictionary<GameObject, float> m_waterScenerySpawnChances;

	public Dictionary<GameObject, float> ScenerySpawnChances
	{
		get
		{
			return m_scenerySpawnChances;
		}
	}

	public Dictionary<GameObject, float> WaterScenerySpawnChances
	{
		get
		{
			return m_waterScenerySpawnChances;
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_tilesLow, m_tilesLowValues, out m_tileLowGeometry, "tileLowGeometry");
		Utils.InitDictionary(m_tilesMid, m_tilesMidValues, out m_tileMidGeometry, "tileMidGeometry");
		Utils.InitDictionary(m_tilesHigh, m_tilesHighValues, out m_tileHighGeometry, "tileHighGeometry");
		Utils.InitDictionary(m_scenerySpawnChance, m_scenerySpawnChanceValues, out m_scenerySpawnChances, "scenerySpawnChances");
		Utils.InitDictionary(m_waterScenerySpawnChance, m_waterScenerySpawnChanceValues, out m_waterScenerySpawnChances, "waterScenerySpawnChances");
	}

	public GameObject Tile(HexType type, int height)
	{
		if (height < 0 || height > 2)
		{
			return null;
		}
		Dictionary<HexType, GameObject> dictionary = null;
		switch (height)
		{
		case 0:
			dictionary = m_tileLowGeometry;
			break;
		case 1:
			dictionary = m_tileMidGeometry;
			break;
		case 2:
			dictionary = m_tileHighGeometry;
			break;
		}
		if (!dictionary.ContainsKey(type))
		{
			Debug.LogError("Unable to find tile: " + type.ToString() + " in tileset: " + base.gameObject.name);
		}
		return dictionary[type];
	}

	public GameObject Wall(Level level, HexType type)
	{
		WallTileSet[] walls = m_walls;
		foreach (WallTileSet wallTileSet in walls)
		{
			if (wallTileSet.type == type)
			{
				int nextRandomRange = level.GetNextRandomRange(0, wallTileSet.walls.Length);
				return wallTileSet.walls[nextRandomRange];
			}
		}
		Debug.LogError("Unable to find wall: " + type.ToString() + " in tileset: " + base.gameObject.name);
		return null;
	}

	public GameObject DynamicTileWall()
	{
		return m_dynamicTileWall;
	}

	public GameObject Water(HexType type)
	{
		switch (type)
		{
		case HexType.Empty:
			return m_water.waterTile;
		case HexType.ThreeQuater:
			return m_water.waterQuater;
		case HexType.Half:
			return m_water.waterHalf;
		default:
			return null;
		}
	}

	public GameObject WaterQuater()
	{
		return m_water.waterQuater;
	}

	public GameObject WaterWakeIn()
	{
		return m_water.wakeIn;
	}

	public GameObject WaterWakeOut()
	{
		return m_water.wakeOut;
	}

	public GameObject WaterWakeQuater()
	{
		return m_water.wakeQuater;
	}

	public GameObject WaterWakeHalf()
	{
		return m_water.wakeHalf;
	}

	public GameObject WaterWakeQuaterHalfLeft()
	{
		return m_water.wakeQuaterHalfLeft;
	}

	public GameObject WaterWakeQuaterHalfRight()
	{
		return m_water.wakeQuaterHalfRight;
	}

	public GameObject WaterWakeQuaterLeft()
	{
		return m_water.wakeQuaterLeft;
	}

	public GameObject WaterWakeQuaterRight()
	{
		return m_water.wakeQuaterRight;
	}

	public GameObject WaterWakeHalfHalf()
	{
		return m_water.wakeHalfHalf;
	}

	public GameObject WaterWakeHalfCorner()
	{
		return m_water.wakeHalfCorner;
	}

	public GameObject GetRandomScenery(Level level)
	{
		if (m_scenerySpawnChances.Count <= 0)
		{
			return null;
		}
		float num = 0f;
		foreach (float value in m_scenerySpawnChances.Values)
		{
			float num2 = value;
			num += num2;
		}
		float nextRandomRange = level.GetNextRandomRange(0f, num);
		num = 0f;
		foreach (KeyValuePair<GameObject, float> scenerySpawnChance in m_scenerySpawnChances)
		{
			num += scenerySpawnChance.Value;
			if (num >= nextRandomRange)
			{
				return scenerySpawnChance.Key;
			}
		}
		return null;
	}

	public GameObject GetRandomWaterScenery(Level level)
	{
		if (m_waterScenerySpawnChances.Count <= 0)
		{
			return null;
		}
		float num = 0f;
		foreach (float value in m_waterScenerySpawnChances.Values)
		{
			float num2 = value;
			num += num2;
		}
		float nextRandomRange = level.GetNextRandomRange(0f, num);
		num = 0f;
		foreach (KeyValuePair<GameObject, float> waterScenerySpawnChance in m_waterScenerySpawnChances)
		{
			num += waterScenerySpawnChance.Value;
			if (num >= nextRandomRange)
			{
				return waterScenerySpawnChance.Key;
			}
		}
		return null;
	}
}
