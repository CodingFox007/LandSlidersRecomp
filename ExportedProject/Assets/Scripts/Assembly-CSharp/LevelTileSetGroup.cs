using System.Collections.Generic;
using System.Linq;
using HeavyDutyInspector;
using UnityEngine;

public class LevelTileSetGroup : MonoBehaviour, ISerializationCallbackReceiver
{
	[Dictionary("m_tilesetsValues")]
	public List<LevelTileSet> m_tilesetsChances;

	[HideInInspector]
	public List<float> m_tilesetsValues;

	private Dictionary<LevelTileSet, float> m_tilesets;

	public int TileSetCount
	{
		get
		{
			return m_tilesets.Count;
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_tilesetsChances, m_tilesetsValues, out m_tilesets, "tilesets");
	}

	public LevelTileSet GetTileSet(int idx)
	{
		if (idx < 0 || idx >= TileSetCount)
		{
			return null;
		}
		return m_tilesets.Keys.ElementAt(idx);
	}

	public float GetTileSetChance(int idx)
	{
		if (idx < 0 || idx >= TileSetCount)
		{
			return 0f;
		}
		return m_tilesets.Values.ElementAt(idx);
	}
}
