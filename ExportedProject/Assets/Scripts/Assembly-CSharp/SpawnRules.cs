using System;
using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;

[Serializable]
public class SpawnRules : MonoBehaviour, SpawnRulesBase
{
	public enum CheckType
	{
		MustContain = 0,
		CantContain = 1
	}

	public enum CheckLayerDiff
	{
		Any = 0,
		Same = 1,
		Higher = 2,
		Lower = 3,
		OneHigher = 4,
		OneLower = 5
	}

	[Serializable]
	public class NeighbourCheck
	{
		public CheckType m_checkType;

		public int m_radius;

		public int m_countNeeded;

		public HexType m_type;

		public HexTypeExtended m_typeExtended;

		public string m_dynamicTileId;

		public CheckLayerDiff m_layerDiff;
	}

	public HexType m_type;

	public HexTypeExtended m_typeExtended;

	[HideConditional("m_typeExtended", new int[] { 3 })]
	public string m_dynamicTileId;

	public HexLayer m_layer;

	[SerializeField]
	private NeighbourCheck[] m_neighbourChecks;

	[SerializeField]
	private GameObject[] m_spawnPartners;

	public virtual bool CheckTile(HexTile tile)
	{
		if (tile == null)
		{
			return false;
		}
		HexMap hexMap = Singleton<Game>.Instance.LevelManager.CurrentLevel.HexMap;
		HexTile highestOccupiedTile = hexMap.GetHighestOccupiedTile(tile.X, tile.Y);
		if (highestOccupiedTile == null || tile != highestOccupiedTile)
		{
			return false;
		}
		if (m_type != HexType.Any && m_type != tile.Type)
		{
			return false;
		}
		if (m_typeExtended != HexTypeExtended.Any && m_typeExtended != tile.TypeExtended)
		{
			return false;
		}
		if (m_dynamicTileId != string.Empty && m_dynamicTileId.GetHashCode() != tile.DynamicTile)
		{
			return false;
		}
		if (m_layer != HexLayer.Any && m_layer != (HexLayer)tile.Layer)
		{
			return false;
		}
		NeighbourCheck[] neighbourChecks = m_neighbourChecks;
		foreach (NeighbourCheck neighbourCheck in neighbourChecks)
		{
			int num = 0;
			List<HexTile> tilesInRadius = hexMap.GetTilesInRadius(tile, neighbourCheck.m_radius);
			foreach (HexTile item in tilesInRadius)
			{
				if (item == null)
				{
					if (neighbourCheck.m_type == HexType.Empty)
					{
						num++;
					}
					continue;
				}
				HexTile highestOccupiedTile2 = hexMap.GetHighestOccupiedTile(item.X, item.Y);
				if (highestOccupiedTile2 == null)
				{
					if (neighbourCheck.m_type == HexType.Empty)
					{
						num++;
					}
				}
				else
				{
					if ((neighbourCheck.m_type != HexType.Any && highestOccupiedTile2.Type != neighbourCheck.m_type) || (neighbourCheck.m_typeExtended != HexTypeExtended.Any && highestOccupiedTile2.TypeExtended != neighbourCheck.m_typeExtended) || (neighbourCheck.m_dynamicTileId != string.Empty && highestOccupiedTile2.DynamicTile != neighbourCheck.m_dynamicTileId.GetHashCode()))
					{
						continue;
					}
					if (neighbourCheck.m_layerDiff != CheckLayerDiff.Any)
					{
						int num2 = highestOccupiedTile2.Layer - tile.Layer;
						if ((neighbourCheck.m_layerDiff == CheckLayerDiff.Same && num2 != 0) || (neighbourCheck.m_layerDiff == CheckLayerDiff.Higher && num2 <= 0) || (neighbourCheck.m_layerDiff == CheckLayerDiff.Lower && num2 >= 0) || (neighbourCheck.m_layerDiff == CheckLayerDiff.OneHigher && num2 != 1) || (neighbourCheck.m_layerDiff == CheckLayerDiff.OneLower && num2 != -1))
						{
							continue;
						}
					}
					num++;
				}
			}
			if (neighbourCheck.m_checkType == CheckType.MustContain)
			{
				if (num < neighbourCheck.m_countNeeded)
				{
					return false;
				}
			}
			else if (neighbourCheck.m_checkType == CheckType.CantContain && num >= neighbourCheck.m_countNeeded)
			{
				return false;
			}
		}
		return true;
	}

	public HexTile PickRandomTile(List<HexTile> tiles)
	{
		if (tiles.Count <= 0)
		{
			return null;
		}
		HexTile hexTile = null;
		foreach (HexTile tile in tiles)
		{
			if (CheckTile(tile))
			{
				hexTile = tile;
				break;
			}
		}
		if (hexTile == null)
		{
			return null;
		}
		return hexTile;
	}

	public void SpawnPartners()
	{
		GameObject[] spawnPartners = m_spawnPartners;
		foreach (GameObject gameObject in spawnPartners)
		{
			Singleton<Game>.Instance.LevelManager.CurrentLevel.Spawn(gameObject, gameObject.name, true);
		}
	}

	public int CheckCount()
	{
		int num = 1;
		NeighbourCheck[] neighbourChecks = m_neighbourChecks;
		foreach (NeighbourCheck neighbourCheck in neighbourChecks)
		{
			num += neighbourCheck.m_radius * neighbourCheck.m_radius * 6;
		}
		return num;
	}
}
