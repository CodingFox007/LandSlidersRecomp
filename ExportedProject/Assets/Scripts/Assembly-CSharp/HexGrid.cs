using System.Collections;
using System.Collections.Generic;

public class HexGrid
{
	private int m_width;

	private int m_height;

	private int m_layer;

	private HexTile[,] m_tiles;

	public int Width
	{
		get
		{
			return m_width;
		}
	}

	public int Height
	{
		get
		{
			return m_height;
		}
	}

	public int Layer
	{
		get
		{
			return m_layer;
		}
	}

	public HexTile[,] Tiles
	{
		get
		{
			return m_tiles;
		}
	}

	public void CreateGrid(int width, int height, int layer)
	{
		m_width = width;
		m_height = height;
		m_layer = layer;
		m_tiles = new HexTile[width, height];
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				m_tiles[j, i] = new HexTile(this, j, i);
			}
		}
	}

	public void AddTile(int x, int y)
	{
		if (x >= 0 && x < Width && y >= 0 && y < Height)
		{
			m_tiles[x, y].Type = HexType.Hex;
		}
	}

	public void RemoveTile(int x, int y)
	{
		if (x >= 0 && x < Width && y >= 0 && y < Height)
		{
			m_tiles[x, y].Type = HexType.Empty;
		}
	}

	public HexTile GetTile(int x, int y)
	{
		if (x < 0 || x >= Width)
		{
			return null;
		}
		if (y < 0 || y >= Height)
		{
			return null;
		}
		return m_tiles[x, y];
	}

	public HexTile[] GetAttachedTiles(HexTile tile)
	{
		return null;
	}

	public IEnumerator SmoothGrid(Level level, Dictionary<HexType, float> tileChances)
	{
		HexTile[,] tiles = m_tiles;
		int length = tiles.GetLength(0);
		int length2 = tiles.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				HexTile hexTile = tiles[i, j];
				if (hexTile.Type == HexType.Hex && !hexTile.Locked)
				{
					ModifyTileBasedOnWaterNeighbours(level, hexTile, tileChances);
				}
			}
		}
		HexTile[,] tiles2 = m_tiles;
		int length3 = tiles2.GetLength(0);
		int length4 = tiles2.GetLength(1);
		for (int k = 0; k < length3; k++)
		{
			for (int l = 0; l < length4; l++)
			{
				HexTile hexTile2 = tiles2[k, l];
				if (hexTile2.Type == HexType.Empty && !hexTile2.Locked)
				{
					ModifyTileBasedOnOccupiedNeighbours(level, hexTile2, tileChances);
				}
			}
		}
		HexTile[,] tiles3 = m_tiles;
		int length5 = tiles3.GetLength(0);
		int length6 = tiles3.GetLength(1);
		for (int m = 0; m < length5; m++)
		{
			for (int n = 0; n < length6; n++)
			{
				HexTile hexTile3 = tiles3[m, n];
				if (hexTile3.Type == HexType.Hex && !hexTile3.Locked)
				{
					ModifyTileBasedOnUnoccupiedNeighbours(level, hexTile3, tileChances);
				}
			}
		}
		yield break;
	}

	private void ModifyTileBasedOnOccupiedNeighbours(Level level, HexTile tile, Dictionary<HexType, float> tileChances)
	{
		int tileCount = 0;
		int tileStartIdx = 0;
		float nextRandomRange = level.GetNextRandomRange(0f, 1f);
		tile.LargestConsecutiveNeighbours(HexType.Hex, out tileCount, out tileStartIdx);
		switch (tileCount)
		{
		case 2:
			if (nextRandomRange > tileChances[HexType.Quarter])
			{
				return;
			}
			break;
		case 3:
			if (nextRandomRange > tileChances[HexType.Half])
			{
				return;
			}
			break;
		case 4:
			if (nextRandomRange > tileChances[HexType.Crescent])
			{
				return;
			}
			break;
		case 5:
			if (nextRandomRange > tileChances[HexType.Crescent])
			{
				return;
			}
			break;
		}
		switch (tileCount)
		{
		case 2:
			tile.Type = HexType.Quarter;
			tile.Rotation = 60f * (float)(tileStartIdx - 2);
			break;
		case 3:
			tile.Type = HexType.Half;
			tile.Rotation = 60f * (float)(tileStartIdx - 1);
			break;
		case 4:
			tile.Type = HexType.Crescent;
			tile.Rotation = 60f * (float)(tileStartIdx + 3);
			break;
		case 5:
			tile.Type = HexType.Crescent;
			tile.Rotation = 60f * (float)(tileStartIdx - 3);
			break;
		}
	}

	private void ModifyTileBasedOnUnoccupiedNeighbours(Level level, HexTile tile, Dictionary<HexType, float> tileChances)
	{
		int tileCount = 0;
		int tileStartIdx = 0;
		float nextRandomRange = level.GetNextRandomRange(0f, 1f);
		tile.LargestConsecutiveNeighbours(HexType.Empty, out tileCount, out tileStartIdx);
		if (tile.NeighbouringWater)
		{
			return;
		}
		switch (tileCount)
		{
		case 3:
			if (nextRandomRange > tileChances[HexType.CurvedHalf])
			{
				return;
			}
			break;
		case 4:
			if (nextRandomRange > tileChances[HexType.CurvedHalf])
			{
				return;
			}
			break;
		case 5:
			if (nextRandomRange > tileChances[HexType.CurvedHalf])
			{
				return;
			}
			break;
		case 6:
			if (nextRandomRange > tileChances[HexType.Island])
			{
				return;
			}
			break;
		}
		switch (tileCount)
		{
		case 3:
			tile.Type = HexType.CurvedHalf;
			tile.Rotation = 60f * (float)(tileStartIdx - 1);
			break;
		case 4:
			tile.Type = HexType.CurvedHalf;
			tile.Rotation = 60f * (float)(tileStartIdx - 1);
			break;
		case 5:
			tile.Type = HexType.CurvedHalf;
			tile.Rotation = 60f * (float)(tileStartIdx - 1);
			break;
		case 6:
			tile.Type = HexType.Island;
			tile.Rotation = 60f * (float)(tileStartIdx - 1);
			break;
		}
	}

	private void ModifyTileBasedOnWaterNeighbours(Level level, HexTile tile, Dictionary<HexType, float> tileChances)
	{
		int tileCount = 0;
		int tileStartIdx = 0;
		float nextRandomRange = level.GetNextRandomRange(0f, 1f);
		tile.LargestConsecutiveWaterNeighbours(out tileCount, out tileStartIdx);
		if (tile.MultipleNeighbourGroups)
		{
			return;
		}
		switch (tileCount)
		{
		case 2:
			if (nextRandomRange > tileChances[HexType.ThreeQuater])
			{
				return;
			}
			break;
		case 3:
			if (nextRandomRange > tileChances[HexType.Half])
			{
				return;
			}
			break;
		}
		switch (tileCount)
		{
		case 2:
			tile.Type = HexType.ThreeQuater;
			tile.Rotation = 60f * (float)(tileStartIdx - 2);
			break;
		case 3:
			tile.Type = HexType.Half;
			tile.Rotation = 60f * (float)(tileStartIdx - 4);
			break;
		}
	}
}
