using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;

public class DynamicTile : MonoBehaviour, ISerializationCallbackReceiver
{
	[SerializeField]
	private string m_id;

	[SerializeField]
	private RandomRange m_count;

	[SerializeField]
	private bool m_castShadows;

	[Dictionary("m_tilesetValuesLayer0")]
	public List<LevelTileSet> m_tilesetEdgesLayer0;

	[HideInInspector]
	public List<GameObject> m_tilesetValuesLayer0;

	private Dictionary<LevelTileSet, GameObject> m_tilesetsLayer0;

	[Dictionary("m_tilesetValuesLayer1")]
	public List<LevelTileSet> m_tilesetEdgesLayer1;

	[HideInInspector]
	public List<GameObject> m_tilesetValuesLayer1;

	private Dictionary<LevelTileSet, GameObject> m_tilesetsLayer1;

	[Dictionary("m_tilesetValuesLayer2")]
	public List<LevelTileSet> m_tilesetEdgesLayer2;

	[HideInInspector]
	public List<GameObject> m_tilesetValuesLayer2;

	private Dictionary<LevelTileSet, GameObject> m_tilesetsLayer2;

	public string ID
	{
		get
		{
			return m_id;
		}
	}

	public RandomRange Count
	{
		get
		{
			return m_count;
		}
	}

	public bool CastShadows
	{
		get
		{
			return m_castShadows;
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_tilesetEdgesLayer0, m_tilesetValuesLayer0, out m_tilesetsLayer0, "tilesetsLayer0");
		Utils.InitDictionary(m_tilesetEdgesLayer1, m_tilesetValuesLayer1, out m_tilesetsLayer1, "tilesetsLayer1");
		Utils.InitDictionary(m_tilesetEdgesLayer2, m_tilesetValuesLayer2, out m_tilesetsLayer2, "tilesetsLayer2");
	}

	public GameObject GetEdgePrefab(LevelTileSet tileset, int layer)
	{
		switch (layer)
		{
		case 0:
			if (!m_tilesetsLayer0.ContainsKey(tileset))
			{
				return null;
			}
			return m_tilesetsLayer0[tileset];
		case 1:
			if (!m_tilesetsLayer1.ContainsKey(tileset))
			{
				return null;
			}
			return m_tilesetsLayer1[tileset];
		case 2:
			if (!m_tilesetsLayer2.ContainsKey(tileset))
			{
				return null;
			}
			return m_tilesetsLayer2[tileset];
		default:
			return null;
		}
	}
}
