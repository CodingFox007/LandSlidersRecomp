public class HexTile
{
	private HexType m_tileType;

	private float m_rotation;

	private int m_tileSetIdx;

	private HexGrid m_grid;

	private int m_x;

	private int m_y;

	private bool m_locked;

	private int m_setPieceIdx = -1;

	private int m_dynamicTileIdx = -1;

	private bool m_isWater;

	public int m_metaData;

	public HexType Type
	{
		get
		{
			return m_tileType;
		}
		set
		{
			m_tileType = value;
		}
	}

	public HexTypeExtended TypeExtended
	{
		get
		{
			if (IsWater)
			{
				return HexTypeExtended.Water;
			}
			if (IsSetPiece)
			{
				return HexTypeExtended.SetPiece;
			}
			if (IsDynamicTile)
			{
				return HexTypeExtended.DynamicTile;
			}
			return HexTypeExtended.None;
		}
	}

	public int X
	{
		get
		{
			return m_x;
		}
	}

	public int Y
	{
		get
		{
			return m_y;
		}
	}

	public int Layer
	{
		get
		{
			return m_grid.Layer;
		}
	}

	public bool Occupied
	{
		get
		{
			return m_tileType != HexType.Empty;
		}
	}

	public bool IsSmoothType
	{
		get
		{
			return m_tileType != HexType.Hex && m_tileType != HexType.Empty;
		}
	}

	public float Rotation
	{
		get
		{
			return m_rotation;
		}
		set
		{
			m_rotation = value;
		}
	}

	public int TileSet
	{
		get
		{
			return m_tileSetIdx;
		}
		set
		{
			m_tileSetIdx = value;
		}
	}

	public int SetPiece
	{
		get
		{
			return m_setPieceIdx;
		}
		set
		{
			m_setPieceIdx = value;
		}
	}

	public bool IsSetPiece
	{
		get
		{
			return m_setPieceIdx != -1;
		}
	}

	public int DynamicTile
	{
		get
		{
			return m_dynamicTileIdx;
		}
		set
		{
			m_dynamicTileIdx = value;
		}
	}

	public bool IsDynamicTile
	{
		get
		{
			return m_dynamicTileIdx != -1;
		}
	}

	public bool IsSpecialTile
	{
		get
		{
			return IsDynamicTile || IsSetPiece;
		}
	}

	public bool Locked
	{
		get
		{
			return m_locked;
		}
		set
		{
			m_locked = value;
		}
	}

	public bool IsWater
	{
		get
		{
			return m_isWater;
		}
		set
		{
			m_isWater = value;
		}
	}

	public HexTile[] Neighbours
	{
		get
		{
			return new HexTile[6] { Top, TopRight, BottomRight, Bottom, BottomLeft, TopLeft };
		}
	}

	public bool MultipleNeighbourGroups
	{
		get
		{
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			HexTile[] neighbours = Neighbours;
			for (int i = 0; i < neighbours.Length; i++)
			{
				if (i == 0)
				{
					if (neighbours[i] == null)
					{
						flag2 = false;
						flag = false;
					}
					else
					{
						flag2 = neighbours[i].Occupied;
						flag = neighbours[i].Occupied;
					}
				}
				else if (neighbours[i] == null)
				{
					if (flag)
					{
						flag = false;
						num++;
					}
				}
				else if (neighbours[i].Occupied != flag)
				{
					flag = neighbours[i].Occupied;
					num++;
				}
			}
			if (flag != flag2)
			{
				num++;
			}
			return num > 2;
		}
	}

	public int NeighbouringTileSet
	{
		get
		{
			HexTile[] neighbours = Neighbours;
			HexTile[] array = neighbours;
			foreach (HexTile hexTile in array)
			{
				if (hexTile != null && hexTile.TileSet != TileSet)
				{
					return hexTile.TileSet;
				}
			}
			return TileSet;
		}
	}

	public bool NeighbouringWater
	{
		get
		{
			HexTile[] neighbours = Neighbours;
			HexTile[] array = neighbours;
			foreach (HexTile hexTile in array)
			{
				if (hexTile != null && hexTile.IsWater)
				{
					return true;
				}
			}
			return false;
		}
	}

	public HexTile Top
	{
		get
		{
			return m_grid.GetTile(m_x, m_y - 2);
		}
	}

	public HexTile Bottom
	{
		get
		{
			return m_grid.GetTile(m_x, m_y + 2);
		}
	}

	public HexTile TopLeft
	{
		get
		{
			if (m_y % 2 == 0)
			{
				return m_grid.GetTile(m_x - 1, m_y - 1);
			}
			return m_grid.GetTile(m_x, m_y - 1);
		}
	}

	public HexTile TopRight
	{
		get
		{
			if (m_y % 2 == 0)
			{
				return m_grid.GetTile(m_x, m_y - 1);
			}
			return m_grid.GetTile(m_x + 1, m_y - 1);
		}
	}

	public HexTile BottomLeft
	{
		get
		{
			if (m_y % 2 == 0)
			{
				return m_grid.GetTile(m_x - 1, m_y + 1);
			}
			return m_grid.GetTile(m_x, m_y + 1);
		}
	}

	public HexTile BottomRight
	{
		get
		{
			if (m_y % 2 == 0)
			{
				return m_grid.GetTile(m_x, m_y + 1);
			}
			return m_grid.GetTile(m_x + 1, m_y + 1);
		}
	}

	public HexTile(HexGrid grid, int x, int y)
	{
		m_grid = grid;
		m_x = x;
		m_y = y;
	}

	public bool Needwall(int index, HexTile neighbour)
	{
		if (neighbour.Type == HexType.Hex)
		{
			return false;
		}
		if (neighbour.Type == HexType.Empty || neighbour.Type == HexType.Island)
		{
			return true;
		}
		int num = (index + 1) % 6;
		int num2 = (int)(neighbour.Rotation / 60f);
		if (num2 < 0)
		{
			num2 += 6;
		}
		switch (neighbour.Type)
		{
		case HexType.Crescent:
			if (index == (1 + num2) % 6 && num == (2 + num2) % 6)
			{
				return false;
			}
			if (index == (2 + num2) % 6 && num == (3 + num2) % 6)
			{
				return false;
			}
			if (index == (3 + num2) % 6 && num == (4 + num2) % 6)
			{
				return false;
			}
			if (index == (4 + num2) % 6 && num == (5 + num2) % 6)
			{
				return false;
			}
			break;
		case HexType.CurvedHalf:
			if (index == (3 + num2) % 6 && num == (4 + num2) % 6)
			{
				return false;
			}
			if (index == (4 + num2) % 6 && num == (5 + num2) % 6)
			{
				return false;
			}
			if (index == (5 + num2) % 6 && num == num2 % 6)
			{
				return false;
			}
			break;
		case HexType.Half:
			if (index == num2 % 6 && num == (1 + num2) % 6)
			{
				return false;
			}
			if (index == (1 + num2) % 6 && num == (2 + num2) % 6)
			{
				return false;
			}
			if (index == (5 + num2) % 6 && num == num2 % 6)
			{
				return false;
			}
			break;
		case HexType.Quarter:
			if (index == num2 % 6 && num == (1 + num2) % 6)
			{
				return false;
			}
			if (index == (1 + num2) % 6 && num == (2 + num2) % 6)
			{
				return false;
			}
			break;
		}
		return true;
	}

	public void LargestConsecutiveNeighbours(HexType type, out int tileCount, out int tileStartIdx, bool tileSetCheck = false)
	{
		HexTile[] neighbours = Neighbours;
		int num = 0;
		int num2 = -1;
		int num3 = 0;
		int num4 = 0;
		int num5 = -1;
		int num6 = 0;
		HexTile[] array = neighbours;
		foreach (HexTile hexTile in array)
		{
			bool flag = true;
			if (hexTile == null)
			{
				flag = false;
			}
			if (hexTile != null && type != HexType.Any && hexTile.Type != type)
			{
				flag = false;
			}
			if (hexTile != null && tileSetCheck && hexTile.TileSet != TileSet)
			{
				flag = false;
			}
			if (!flag)
			{
				if (num5 == -1)
				{
					num5 = num;
				}
				if (num >= num3)
				{
					num3 = num;
					num4 = num2;
				}
				num = 0;
				num2 = num6;
			}
			else
			{
				num++;
			}
			num6++;
		}
		if (num != 0)
		{
			if (num5 != -1)
			{
				num += num5;
			}
			if (num > num3)
			{
				num3 = num;
				num4 = num2;
			}
		}
		tileCount = num3;
		tileStartIdx = num4;
	}

	public void LargestConsecutiveWaterNeighbours(out int tileCount, out int tileStartIdx, bool tileSetCheck = false)
	{
		HexTile[] neighbours = Neighbours;
		int num = 0;
		int num2 = -1;
		int num3 = 0;
		int num4 = 0;
		int num5 = -1;
		int num6 = 0;
		HexTile[] array = neighbours;
		foreach (HexTile hexTile in array)
		{
			bool flag = true;
			if (hexTile == null)
			{
				flag = false;
			}
			if (hexTile != null && !hexTile.IsWater)
			{
				flag = false;
			}
			if (hexTile != null && tileSetCheck && hexTile.TileSet != TileSet)
			{
				flag = false;
			}
			if (!flag)
			{
				if (num5 == -1)
				{
					num5 = num;
				}
				if (num >= num3)
				{
					num3 = num;
					num4 = num2;
				}
				num = 0;
				num2 = num6;
			}
			else
			{
				num++;
			}
			num6++;
		}
		if (num != 0)
		{
			if (num5 != -1)
			{
				num += num5;
			}
			if (num > num3)
			{
				num3 = num;
				num4 = num2;
			}
		}
		tileCount = num3;
		tileStartIdx = num4;
	}
}
