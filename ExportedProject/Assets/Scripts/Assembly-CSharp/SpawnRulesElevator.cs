using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnRulesElevator : SpawnRules
{
	[SerializeField]
	private int m_neighbourNeighbours = 3;

	public override bool CheckTile(HexTile tile)
	{
		if (!base.CheckTile(tile))
		{
			return false;
		}
		HexMap hexMap = Singleton<Game>.Instance.LevelManager.CurrentLevel.HexMap;
		HexTile tileAbove = hexMap.GetTileAbove(tile);
		if (tileAbove.MultipleNeighbourGroups)
		{
			return false;
		}
		int num = 0;
		List<HexTile> tilesInRadius = hexMap.GetTilesInRadius(tile, 1);
		foreach (HexTile item in tilesInRadius)
		{
			if (item != null)
			{
				HexTile highestOccupiedTile = hexMap.GetHighestOccupiedTile(item.X, item.Y);
				if (highestOccupiedTile != null && highestOccupiedTile.Type != HexType.Empty && highestOccupiedTile.Layer == tile.Layer + 1)
				{
					int tileCount = 0;
					int tileStartIdx = 0;
					highestOccupiedTile.LargestConsecutiveNeighbours(HexType.Hex, out tileCount, out tileStartIdx);
					num = Mathf.Max(num, tileCount);
				}
			}
		}
		if (num < m_neighbourNeighbours)
		{
			return false;
		}
		return true;
	}
}
