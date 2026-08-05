using System.Collections;
using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class HexMap : MonoBehaviour, ISerializationCallbackReceiver
{
	public delegate void LoadComplete();

	public class HexIsland
	{
		public List<HexTile> m_tiles = new List<HexTile>();

		public float m_distanceToMain;
	}

	[Dictionary("m_tileSpawnValues")]
	public List<HexType> m_tileSpawnChance;

	[HideInInspector]
	public List<float> m_tileSpawnValues;

	private Dictionary<HexType, float> m_tileSpawnOptions;

	[SerializeField]
	private float m_maxTilesBetweenIslands = 3f;

	[SerializeField]
	private Texture2D m_map;

	[SerializeField]
	private Texture2D m_groupingMap;

	[SerializeField]
	private GameObject m_overrideGeometry;

	[SerializeField]
	public bool m_removeGeometryUnderneath = true;

	private HexGrid[] m_hexGrids = new HexGrid[3];

	private float[] m_levelHeights = new float[3] { 0f, 7.5f, 15f };

	private float m_hexWidth;

	private float m_hexHeight;

	private Transform m_geometryTransform;

	private float m_progress;

	private List<HexTile> m_mainIsland;

	private static int m_prefabCreateLimitPerFrame = 100;

	public bool IsComplete
	{
		get
		{
			return m_progress == 1f;
		}
	}

	public float Progress
	{
		get
		{
			return m_progress;
		}
	}

	public HexGrid[] HexGrids
	{
		get
		{
			return m_hexGrids;
		}
	}

	public float TileWidth
	{
		get
		{
			return m_hexWidth;
		}
	}

	public float TileHeight
	{
		get
		{
			return m_hexHeight;
		}
	}

	public bool OverridesGeometry
	{
		get
		{
			return m_overrideGeometry != null;
		}
	}

	public event LoadComplete OnLoadComplete;

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_tileSpawnChance, m_tileSpawnValues, out m_tileSpawnOptions, "tileSpawnOptions");
	}

	public IEnumerator GenerateLevel(Level level)
	{
		m_progress = 0f;
		m_geometryTransform = GetCategoryTransform(base.transform, "Geometry", true);
		LevelTileSet tileSet = GetTileSet(0);
		m_hexHeight = tileSet.Tile(HexType.Hex, 0).GetComponent<Renderer>().bounds.size.z;
		m_hexWidth = tileSet.Tile(HexType.Hex, 0).GetComponent<Renderer>().bounds.size.x;
		CreateGrids(level);
		yield return null;
		if (m_overrideGeometry != null)
		{
			RepositionLevelAroundPlayer();
			m_progress = 1f;
			if (this.OnLoadComplete != null)
			{
				this.OnLoadComplete();
			}
			yield break;
		}
		yield return StartCoroutine(CreateDifferentSurfaceTypes(level));
		yield return StartCoroutine(AddSetPieces());
		yield return StartCoroutine(RemoveFarIslands());
		yield return StartCoroutine(RemoveVisibiltyIssues());
		yield return StartCoroutine(CreateWater());
		yield return StartCoroutine(CreateDynamicTiles());
		RepositionLevelAroundPlayer();
		yield return null;
		yield return StartCoroutine(SmoothGrids(level));
		yield return StartCoroutine(CreateGeometry(level));
		yield return StartCoroutine(CreateWalls(level));
		yield return StartCoroutine(CreateWaterGeometry());
		m_progress = 1f;
		if (this.OnLoadComplete != null)
		{
			this.OnLoadComplete();
		}
	}

	private void CreateGrids(Level level)
	{
		int num = m_map.width / 4;
		int height = m_map.height;
		for (int i = 0; i < m_hexGrids.Length; i++)
		{
			m_hexGrids[i] = new HexGrid();
			m_hexGrids[i].CreateGrid(num, height, i);
		}
		for (int j = 0; j < height; j++)
		{
			for (int k = 0; k < num; k++)
			{
				Color pixel = m_map.GetPixel(k * 4 + j % 2 * 2, j);
				if (pixel.r >= level.GetNextRandomRange(0f, 1f))
				{
					m_hexGrids[0].AddTile(k, j);
				}
				if (pixel.g >= level.GetNextRandomRange(0f, 1f))
				{
					m_hexGrids[1].AddTile(k, j);
				}
				if (pixel.b >= level.GetNextRandomRange(0f, 1f))
				{
					m_hexGrids[2].AddTile(k, j);
				}
			}
		}
		if (!(m_groupingMap != null))
		{
			return;
		}
		for (int l = 0; l < m_hexGrids.Length; l++)
		{
			Dictionary<Color, bool> dictionary = new Dictionary<Color, bool>();
			for (int m = 0; m < height; m++)
			{
				for (int n = 0; n < num; n++)
				{
					Color pixel2 = m_groupingMap.GetPixel(n * 4 + m % 2 * 2, m);
					float num2 = 0f;
					if (l == 0)
					{
						num2 = pixel2.r;
					}
					if (l == 1)
					{
						num2 = pixel2.g;
					}
					if (l == 2)
					{
						num2 = pixel2.b;
					}
					if (!dictionary.ContainsKey(pixel2))
					{
						dictionary.Add(pixel2, num2 > level.GetNextRandomRange(0f, 1f));
					}
					if (num2 != 0f)
					{
						if (dictionary[pixel2])
						{
							m_hexGrids[l].AddTile(n, m);
						}
						else
						{
							m_hexGrids[l].RemoveTile(n, m);
						}
					}
				}
			}
		}
	}

	private IEnumerator SmoothGrids(Level level)
	{
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid grid in hexGrids)
		{
			yield return StartCoroutine(grid.SmoothGrid(level, m_tileSpawnOptions));
		}
		for (int j = 0; j < m_hexGrids.Length; j++)
		{
			HexTile[,] tiles = m_hexGrids[j].Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int k = 0; k < length; k++)
			{
				for (int l = 0; l < length2; l++)
				{
					HexTile hexTile = tiles[k, l];
					if (!hexTile.Occupied || !hexTile.IsSmoothType)
					{
						continue;
					}
					for (int m = j + 1; m < m_hexGrids.Length; m++)
					{
						HexTile tile = m_hexGrids[m].GetTile(hexTile.X, hexTile.Y);
						if (tile != null && tile.Occupied && tile.IsSmoothType && tile.Type != hexTile.Type)
						{
							hexTile.Type = HexType.Hex;
						}
					}
				}
			}
		}
	}

	private IEnumerator CreateGeometry(Level level)
	{
		Transform categoryTransform = GetCategoryTransform(m_geometryTransform, "Tiles", true);
		int createCount = 0;
		int gridIdx = 0;
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid grid in hexGrids)
		{
			Transform layerTransform = GetCategoryTransform(categoryTransform, "Layer" + gridIdx);
			HexTile[,] tiles = grid.Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					HexTile tile = tiles[j, k];
					if (!tile.Occupied || (tile.IsSpecialTile && tile.Locked))
					{
						continue;
					}
					if (createCount > m_prefabCreateLimitPerFrame)
					{
						createCount = 0;
						yield return null;
					}
					if (tile.Type == HexType.Hex)
					{
						HexTile tileAbove = GetTileAbove(tile);
						if (tileAbove != null && tileAbove.Type == HexType.CurvedHalf)
						{
							tile.Type = HexType.CurvedHalf;
							tile.Rotation = tileAbove.Rotation;
						}
						tileAbove = GetTileAbove(tile, tile.Layer + 2);
						if (tileAbove != null && tileAbove.Type == HexType.CurvedHalf)
						{
							tile.Type = HexType.CurvedHalf;
							tile.Rotation = tileAbove.Rotation;
						}
					}
					if (tile.Type == HexType.CurvedHalf)
					{
						HexTile tileAbove2 = GetTileAbove(tile);
						if (tileAbove2 != null && tileAbove2.Type == HexType.Hex)
						{
							tile.Type = HexType.Hex;
							tile.Rotation = tileAbove2.Rotation;
						}
						tileAbove2 = GetTileAbove(tile, tile.Layer + 2);
						if (tileAbove2 != null && tileAbove2.Type == HexType.Hex)
						{
							tile.Type = HexType.Hex;
							tile.Rotation = tileAbove2.Rotation;
						}
					}
					LevelTileSet tileSet = GetTileSet(tile.TileSet);
					if (tileSet == null)
					{
						continue;
					}
					GameObject prefab = tileSet.Tile(tile.Type, gridIdx);
					if (prefab == null)
					{
						continue;
					}
					float rotation = tile.Rotation;
					if (tile.Type == HexType.Hex && tile.TileSet == 0)
					{
						int tileCount = 0;
						int tileStartIdx = 0;
						float nextRandomRange = level.GetNextRandomRange(0f, 1f);
						tile.LargestConsecutiveNeighbours(HexType.Any, out tileCount, out tileStartIdx, true);
						if (tileCount == 4 && nextRandomRange <= m_tileSpawnOptions[HexType.ThreeQuater])
						{
							rotation = 60f * (float)(tileStartIdx - 4);
							prefab = tileSet.Tile(HexType.ThreeQuater, gridIdx);
							LevelTileSet tileSet2 = GetTileSet(tile.NeighbouringTileSet);
							GameObject original = tileSet2.Tile(HexType.Quarter, gridIdx);
							float num = 60f * (float)(tileStartIdx - 4);
							Vector3 position = HexWorldPosition(tile, gridIdx);
							GameObject gameObject = Object.Instantiate(original, position, Quaternion.Euler(0f, 0f - num, 0f));
							gameObject.transform.parent = layerTransform;
							createCount++;
						}
						else if (tileCount == 3 && nextRandomRange <= m_tileSpawnOptions[HexType.Half])
						{
							rotation = 60f * (float)(tileStartIdx - 1);
							prefab = tileSet.Tile(HexType.Half, gridIdx);
							LevelTileSet tileSet3 = GetTileSet(tile.NeighbouringTileSet);
							GameObject original2 = tileSet3.Tile(HexType.Half, gridIdx);
							float num2 = 60f * (float)(tileStartIdx - 4);
							Vector3 position2 = HexWorldPosition(tile, gridIdx);
							GameObject gameObject2 = Object.Instantiate(original2, position2, Quaternion.Euler(0f, 0f - num2, 0f));
							gameObject2.transform.parent = layerTransform;
							createCount++;
						}
					}
					Vector3 pos = HexWorldPosition(tile, gridIdx);
					GameObject obj = Object.Instantiate(prefab, pos, Quaternion.Euler(0f, 0f - rotation, 0f));
					obj.transform.parent = layerTransform;
					createCount++;
				}
			}
			gridIdx++;
		}
	}

	private IEnumerator CreateWalls(Level level)
	{
		Transform categoryTransform = GetCategoryTransform(m_geometryTransform, "Walls", true);
		int createCount = 0;
		int gridIdx = 0;
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid grid in hexGrids)
		{
			Transform layerTransform = GetCategoryTransform(categoryTransform, "Layer" + gridIdx);
			HexTile[,] tiles = grid.Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					HexTile tile = tiles[j, k];
					if (createCount > m_prefabCreateLimitPerFrame)
					{
						createCount = 0;
						yield return null;
					}
					if (!tile.Occupied)
					{
						continue;
					}
					LevelTileSet tileSet = GetTileSet(tile.TileSet);
					if (tileSet == null)
					{
						continue;
					}
					GameObject prefab = tileSet.Wall(level, tile.Type);
					if (prefab == null)
					{
						continue;
					}
					float rotation = 0f - tile.Rotation;
					int occupiedNeighbours = 0;
					int startIdx = 0;
					tile.LargestConsecutiveNeighbours(HexType.Hex, out occupiedNeighbours, out startIdx);
					if (tile.IsDynamicTile)
					{
						GameObject gameObject = tileSet.DynamicTileWall();
						if (gameObject != null)
						{
							Vector3 position = HexWorldPosition(tile);
							GameObject gameObject2 = Object.Instantiate(gameObject, position, Quaternion.Euler(0f, rotation, 0f));
							gameObject2.transform.parent = layerTransform;
							createCount++;
						}
					}
					if (occupiedNeighbours == 6)
					{
						continue;
					}
					HexTile tileAbove = GetTileAbove(tile);
					if (tileAbove != null && tileAbove.Type == tile.Type && (tileAbove.Type != HexType.Quarter || tileAbove.Rotation == tile.Rotation))
					{
						continue;
					}
					tileAbove = GetTileAbove(tile, tile.Layer + 2);
					if (tileAbove != null && tileAbove.Type == tile.Type && (tileAbove.Type != HexType.Quarter || tileAbove.Rotation == tile.Rotation))
					{
						continue;
					}
					if (tile.Type == HexType.Hex)
					{
						HexTile[] neighbours = tile.Neighbours;
						startIdx = 0;
						HexTile[] array = neighbours;
						foreach (HexTile hexTile in array)
						{
							if (hexTile == null || tile.Needwall(startIdx, hexTile))
							{
								rotation = -60f * (float)(startIdx + 3);
								Vector3 position2 = HexWorldPosition(tile);
								GameObject gameObject3 = Object.Instantiate(prefab, position2, Quaternion.Euler(0f, rotation, 0f));
								gameObject3.transform.parent = layerTransform;
								createCount++;
								if (startIdx == 3 && gameObject3.GetComponent<MeshRenderer>() != null)
								{
									Object.Destroy(gameObject3.GetComponent<MeshRenderer>());
								}
							}
							startIdx++;
						}
						continue;
					}
					Vector3 position3 = HexWorldPosition(tile);
					GameObject gameObject4 = Object.Instantiate(prefab, position3, Quaternion.Euler(0f, rotation, 0f));
					gameObject4.transform.parent = layerTransform;
					createCount++;
					if (tile.Type == HexType.Half && Mathf.Abs(tile.Rotation) == 180f)
					{
						if (gameObject4.GetComponent<MeshRenderer>() != null)
						{
							Object.Destroy(gameObject4.GetComponent<MeshRenderer>());
						}
						if (gameObject4.transform.childCount == 1)
						{
							Object.Destroy(gameObject4.transform.GetChild(0));
						}
					}
				}
			}
			gridIdx++;
		}
	}

	private IEnumerator CreateWater()
	{
		List<HexTile> waterTiles = new List<HexTile>();
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid grid in hexGrids)
		{
			HexTile[] holes = FindHoles(grid);
			HexTile[] array = holes;
			foreach (HexTile hexTile in array)
			{
				for (int k = 0; k < m_hexGrids.Length; k++)
				{
					HexTile tile = m_hexGrids[k].GetTile(hexTile.X, hexTile.Y);
					if (tile != null)
					{
						m_hexGrids[k].RemoveTile(hexTile.X, hexTile.Y);
						waterTiles.Add(tile);
					}
				}
			}
			HexTile[] array2 = holes;
			foreach (HexTile hexTile2 in array2)
			{
				waterTiles.Add(hexTile2);
				hexTile2.Type = HexType.Empty;
				hexTile2.IsWater = true;
			}
			yield return null;
		}
		foreach (HexTile item in waterTiles)
		{
			item.Locked = true;
		}
	}

	private IEnumerator CreateWaterGeometry()
	{
		Transform categoryTransform = GetCategoryTransform(m_geometryTransform, "Water", true);
		int createCount = 0;
		List<HexTile> waterTiles = new List<HexTile>();
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile[,] tiles = hexGrid.Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					HexTile hexTile = tiles[j, k];
					if (hexTile.IsWater)
					{
						waterTiles.Add(hexTile);
					}
					if (hexTile.Type == HexType.ThreeQuater)
					{
						waterTiles.Add(hexTile);
					}
					if (hexTile.Type == HexType.Half && hexTile.NeighbouringWater)
					{
						waterTiles.Add(hexTile);
					}
				}
			}
		}
		foreach (HexTile tile in waterTiles)
		{
			Transform layerTransform = GetCategoryTransform(categoryTransform, "Layer" + tile.Layer);
			if (createCount > m_prefabCreateLimitPerFrame)
			{
				createCount = 0;
				yield return null;
			}
			LevelTileSet tileSet = GetTileSet(tile.TileSet);
			if (tileSet == null)
			{
				continue;
			}
			GameObject prefab = tileSet.Water(tile.Type);
			if (!(prefab == null))
			{
				float rotation = 0f - tile.Rotation;
				if (tile.Type == HexType.Half)
				{
					rotation -= 180f;
				}
				Vector3 pos = HexWorldPosition(tile);
				GameObject obj = Object.Instantiate(prefab, pos, Quaternion.Euler(0f, rotation, 0f));
				obj.transform.parent = layerTransform;
				createCount++;
			}
		}
		Transform wakeCategoryTransform = GetCategoryTransform(m_geometryTransform, "WaterWake", true);
		foreach (HexTile tile2 in waterTiles)
		{
			Transform layerTransform2 = GetCategoryTransform(wakeCategoryTransform, "Layer" + tile2.Layer);
			if (createCount > m_prefabCreateLimitPerFrame)
			{
				createCount = 0;
				yield return null;
			}
			LevelTileSet tileSet2 = GetTileSet(tile2.TileSet);
			if (tileSet2 == null)
			{
				continue;
			}
			Vector3 pos2 = HexWorldPosition(tile2);
			float rotation2 = 0f - tile2.Rotation;
			int neighbouringLand = 0;
			HexTile[] neighbours = tile2.Neighbours;
			HexTile[] array = neighbours;
			foreach (HexTile hexTile2 in array)
			{
				if (hexTile2 != null && hexTile2.Occupied && !waterTiles.Contains(hexTile2))
				{
					neighbouringLand++;
				}
			}
			if (neighbouringLand <= 0)
			{
				continue;
			}
			if (tile2.Type == HexType.Empty)
			{
				if (!waterTiles.Contains(tile2.BottomLeft) && !waterTiles.Contains(tile2.TopLeft))
				{
					rotation2 = -60f;
					GameObject gameObject = Object.Instantiate(tileSet2.WaterWakeIn(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject.transform.parent = layerTransform2;
					createCount++;
				}
				if (!waterTiles.Contains(tile2.TopLeft) && !waterTiles.Contains(tile2.Top))
				{
					rotation2 = -120f;
					GameObject gameObject2 = Object.Instantiate(tileSet2.WaterWakeIn(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject2.transform.parent = layerTransform2;
					createCount++;
				}
				if (!waterTiles.Contains(tile2.Top) && !waterTiles.Contains(tile2.TopRight))
				{
					rotation2 = -180f;
					GameObject gameObject3 = Object.Instantiate(tileSet2.WaterWakeIn(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject3.transform.parent = layerTransform2;
					createCount++;
				}
				if (!waterTiles.Contains(tile2.TopRight) && !waterTiles.Contains(tile2.BottomRight))
				{
					rotation2 = -240f;
					GameObject gameObject4 = Object.Instantiate(tileSet2.WaterWakeIn(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject4.transform.parent = layerTransform2;
					createCount++;
				}
				if (!waterTiles.Contains(tile2.BottomRight) && !waterTiles.Contains(tile2.Bottom))
				{
					rotation2 = -300f;
					GameObject gameObject5 = Object.Instantiate(tileSet2.WaterWakeIn(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject5.transform.parent = layerTransform2;
					createCount++;
				}
				if (!waterTiles.Contains(tile2.Bottom) && !waterTiles.Contains(tile2.BottomLeft))
				{
					rotation2 = -360f;
					GameObject gameObject6 = Object.Instantiate(tileSet2.WaterWakeIn(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject6.transform.parent = layerTransform2;
					createCount++;
				}
			}
			if (tile2.Type == HexType.Empty)
			{
				if (waterTiles.Contains(tile2.TopLeft) && !waterTiles.Contains(tile2.BottomLeft) && tile2.TopLeft.Type == HexType.Empty)
				{
					rotation2 = -60f;
					GameObject gameObject7 = Object.Instantiate(tileSet2.WaterWakeOut(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject7.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Top) && !waterTiles.Contains(tile2.TopLeft) && tile2.Top.Type == HexType.Empty)
				{
					rotation2 = -120f;
					GameObject gameObject8 = Object.Instantiate(tileSet2.WaterWakeOut(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject8.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopRight) && !waterTiles.Contains(tile2.Top) && tile2.TopRight.Type == HexType.Empty)
				{
					rotation2 = -180f;
					GameObject gameObject9 = Object.Instantiate(tileSet2.WaterWakeOut(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject9.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomRight) && !waterTiles.Contains(tile2.TopRight) && tile2.BottomRight.Type == HexType.Empty)
				{
					rotation2 = -240f;
					GameObject gameObject10 = Object.Instantiate(tileSet2.WaterWakeOut(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject10.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Bottom) && !waterTiles.Contains(tile2.BottomRight) && tile2.Bottom.Type == HexType.Empty)
				{
					rotation2 = -300f;
					GameObject gameObject11 = Object.Instantiate(tileSet2.WaterWakeOut(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject11.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomLeft) && !waterTiles.Contains(tile2.Bottom) && tile2.BottomLeft.Type == HexType.Empty)
				{
					rotation2 = -360f;
					GameObject gameObject12 = Object.Instantiate(tileSet2.WaterWakeOut(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject12.transform.parent = layerTransform2;
					createCount++;
				}
			}
			if (tile2.Type == HexType.Empty)
			{
				if (waterTiles.Contains(tile2.TopLeft) && !waterTiles.Contains(tile2.BottomLeft) && tile2.TopLeft.Type == HexType.ThreeQuater)
				{
					rotation2 = -60f;
					GameObject gameObject13 = Object.Instantiate(tileSet2.WaterWakeQuaterRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject13.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Top) && !waterTiles.Contains(tile2.TopLeft) && tile2.Top.Type == HexType.ThreeQuater)
				{
					rotation2 = -120f;
					GameObject gameObject14 = Object.Instantiate(tileSet2.WaterWakeQuaterRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject14.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopRight) && !waterTiles.Contains(tile2.Top) && tile2.TopRight.Type == HexType.ThreeQuater)
				{
					rotation2 = -180f;
					GameObject gameObject15 = Object.Instantiate(tileSet2.WaterWakeQuaterRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject15.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomRight) && !waterTiles.Contains(tile2.TopRight) && tile2.BottomRight.Type == HexType.ThreeQuater)
				{
					rotation2 = -240f;
					GameObject gameObject16 = Object.Instantiate(tileSet2.WaterWakeQuaterRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject16.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Bottom) && !waterTiles.Contains(tile2.BottomRight) && tile2.Bottom.Type == HexType.ThreeQuater)
				{
					rotation2 = -300f;
					GameObject gameObject17 = Object.Instantiate(tileSet2.WaterWakeQuaterRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject17.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomLeft) && !waterTiles.Contains(tile2.Bottom) && tile2.BottomLeft.Type == HexType.ThreeQuater)
				{
					rotation2 = -360f;
					GameObject gameObject18 = Object.Instantiate(tileSet2.WaterWakeQuaterRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject18.transform.parent = layerTransform2;
					createCount++;
				}
			}
			if (tile2.Type == HexType.Empty)
			{
				if (waterTiles.Contains(tile2.BottomLeft) && !waterTiles.Contains(tile2.TopLeft) && tile2.BottomLeft.Type == HexType.ThreeQuater)
				{
					rotation2 = -60f;
					GameObject gameObject19 = Object.Instantiate(tileSet2.WaterWakeQuaterLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject19.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopLeft) && !waterTiles.Contains(tile2.Top) && tile2.TopLeft.Type == HexType.ThreeQuater)
				{
					rotation2 = -120f;
					GameObject gameObject20 = Object.Instantiate(tileSet2.WaterWakeQuaterLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject20.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Top) && !waterTiles.Contains(tile2.TopRight) && tile2.Top.Type == HexType.ThreeQuater)
				{
					rotation2 = -180f;
					GameObject gameObject21 = Object.Instantiate(tileSet2.WaterWakeQuaterLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject21.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopRight) && !waterTiles.Contains(tile2.BottomRight) && tile2.TopRight.Type == HexType.ThreeQuater)
				{
					rotation2 = -240f;
					GameObject gameObject22 = Object.Instantiate(tileSet2.WaterWakeQuaterLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject22.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomRight) && !waterTiles.Contains(tile2.Bottom) && tile2.BottomRight.Type == HexType.ThreeQuater)
				{
					rotation2 = -300f;
					GameObject gameObject23 = Object.Instantiate(tileSet2.WaterWakeQuaterLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject23.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Bottom) && !waterTiles.Contains(tile2.BottomLeft) && tile2.Bottom.Type == HexType.ThreeQuater)
				{
					rotation2 = -360f;
					GameObject gameObject24 = Object.Instantiate(tileSet2.WaterWakeQuaterLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject24.transform.parent = layerTransform2;
					createCount++;
				}
			}
			if (tile2.Type == HexType.ThreeQuater)
			{
				if (waterTiles.Contains(tile2.BottomLeft) && waterTiles.Contains(tile2.TopLeft) && tile2.BottomLeft.Type == HexType.ThreeQuater && tile2.TopLeft.Type == HexType.Empty)
				{
					rotation2 = -60f;
					GameObject gameObject25 = Object.Instantiate(tileSet2.WaterWakeQuater(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject25.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopLeft) && waterTiles.Contains(tile2.Top) && tile2.TopLeft.Type == HexType.ThreeQuater && tile2.Top.Type == HexType.Empty)
				{
					rotation2 = -120f;
					GameObject gameObject26 = Object.Instantiate(tileSet2.WaterWakeQuater(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject26.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Top) && waterTiles.Contains(tile2.TopRight) && tile2.Top.Type == HexType.ThreeQuater && tile2.TopRight.Type == HexType.Empty)
				{
					rotation2 = -180f;
					GameObject gameObject27 = Object.Instantiate(tileSet2.WaterWakeQuater(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject27.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopRight) && waterTiles.Contains(tile2.BottomRight) && tile2.TopRight.Type == HexType.ThreeQuater && tile2.BottomRight.Type == HexType.Empty)
				{
					rotation2 = -240f;
					GameObject gameObject28 = Object.Instantiate(tileSet2.WaterWakeQuater(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject28.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomRight) && waterTiles.Contains(tile2.Bottom) && tile2.BottomRight.Type == HexType.ThreeQuater && tile2.Bottom.Type == HexType.Empty)
				{
					rotation2 = -300f;
					GameObject gameObject29 = Object.Instantiate(tileSet2.WaterWakeQuater(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject29.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Bottom) && waterTiles.Contains(tile2.BottomLeft) && tile2.Bottom.Type == HexType.ThreeQuater && tile2.BottomLeft.Type == HexType.Empty)
				{
					rotation2 = -360f;
					GameObject gameObject30 = Object.Instantiate(tileSet2.WaterWakeQuater(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject30.transform.parent = layerTransform2;
					createCount++;
				}
			}
			if (tile2.Type == HexType.Empty)
			{
				if (waterTiles.Contains(tile2.BottomRight) && !waterTiles.Contains(tile2.Bottom) && tile2.BottomRight.Type == HexType.Half)
				{
					rotation2 = -60f;
					GameObject gameObject31 = Object.Instantiate(tileSet2.WaterWakeHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject31.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Bottom) && !waterTiles.Contains(tile2.BottomLeft) && tile2.Bottom.Type == HexType.Half)
				{
					rotation2 = -120f;
					GameObject gameObject32 = Object.Instantiate(tileSet2.WaterWakeHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject32.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomLeft) && !waterTiles.Contains(tile2.TopLeft) && tile2.BottomLeft.Type == HexType.Half)
				{
					rotation2 = -180f;
					GameObject gameObject33 = Object.Instantiate(tileSet2.WaterWakeHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject33.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopLeft) && !waterTiles.Contains(tile2.Top) && tile2.TopLeft.Type == HexType.Half)
				{
					rotation2 = -240f;
					GameObject gameObject34 = Object.Instantiate(tileSet2.WaterWakeHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject34.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Top) && !waterTiles.Contains(tile2.TopRight) && tile2.Top.Type == HexType.Half)
				{
					rotation2 = -300f;
					GameObject gameObject35 = Object.Instantiate(tileSet2.WaterWakeHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject35.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopRight) && !waterTiles.Contains(tile2.BottomRight) && tile2.TopRight.Type == HexType.Half)
				{
					rotation2 = -360f;
					GameObject gameObject36 = Object.Instantiate(tileSet2.WaterWakeHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject36.transform.parent = layerTransform2;
					createCount++;
				}
			}
			if (tile2.Type == HexType.Half)
			{
				if (waterTiles.Contains(tile2.TopRight) && !waterTiles.Contains(tile2.BottomRight) && tile2.TopRight.Type == HexType.Empty)
				{
					rotation2 = -60f;
					GameObject gameObject37 = Object.Instantiate(tileSet2.WaterWakeHalfHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject37.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomRight) && !waterTiles.Contains(tile2.Bottom) && tile2.BottomRight.Type == HexType.Empty)
				{
					rotation2 = -120f;
					GameObject gameObject38 = Object.Instantiate(tileSet2.WaterWakeHalfHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject38.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Bottom) && !waterTiles.Contains(tile2.BottomLeft) && tile2.Bottom.Type == HexType.Empty)
				{
					rotation2 = -180f;
					GameObject gameObject39 = Object.Instantiate(tileSet2.WaterWakeHalfHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject39.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomLeft) && !waterTiles.Contains(tile2.TopLeft) && tile2.BottomLeft.Type == HexType.Empty)
				{
					rotation2 = -240f;
					GameObject gameObject40 = Object.Instantiate(tileSet2.WaterWakeHalfHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject40.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopLeft) && !waterTiles.Contains(tile2.Top) && tile2.TopLeft.Type == HexType.Empty)
				{
					rotation2 = -300f;
					GameObject gameObject41 = Object.Instantiate(tileSet2.WaterWakeHalfHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject41.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Top) && !waterTiles.Contains(tile2.TopRight) && tile2.Top.Type == HexType.Empty)
				{
					rotation2 = -360f;
					GameObject gameObject42 = Object.Instantiate(tileSet2.WaterWakeHalfHalf(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject42.transform.parent = layerTransform2;
					createCount++;
				}
			}
			if (tile2.Type == HexType.Half)
			{
				if (waterTiles.Contains(tile2.TopLeft) && waterTiles.Contains(tile2.Top) && tile2.TopLeft.Type == HexType.ThreeQuater && tile2.Top.Type == HexType.Empty)
				{
					rotation2 = -60f;
					GameObject gameObject43 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject43.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Top) && waterTiles.Contains(tile2.TopRight) && tile2.Top.Type == HexType.ThreeQuater && tile2.TopRight.Type == HexType.Empty)
				{
					rotation2 = -120f;
					GameObject gameObject44 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject44.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopRight) && waterTiles.Contains(tile2.BottomRight) && tile2.TopRight.Type == HexType.ThreeQuater && tile2.BottomRight.Type == HexType.Empty)
				{
					rotation2 = -180f;
					GameObject gameObject45 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject45.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomRight) && waterTiles.Contains(tile2.Bottom) && tile2.BottomRight.Type == HexType.ThreeQuater && tile2.Bottom.Type == HexType.Empty)
				{
					rotation2 = -240f;
					GameObject gameObject46 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject46.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Bottom) && waterTiles.Contains(tile2.BottomLeft) && tile2.Bottom.Type == HexType.ThreeQuater && tile2.BottomLeft.Type == HexType.Empty)
				{
					rotation2 = -300f;
					GameObject gameObject47 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject47.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomLeft) && waterTiles.Contains(tile2.TopLeft) && tile2.BottomLeft.Type == HexType.ThreeQuater && tile2.TopLeft.Type == HexType.Empty)
				{
					rotation2 = -360f;
					GameObject gameObject48 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfLeft(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject48.transform.parent = layerTransform2;
					createCount++;
				}
			}
			if (tile2.Type == HexType.Half)
			{
				if (waterTiles.Contains(tile2.Bottom) && waterTiles.Contains(tile2.BottomRight) && tile2.Bottom.Type == HexType.ThreeQuater && tile2.BottomRight.Type == HexType.Empty)
				{
					rotation2 = -60f;
					GameObject gameObject49 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject49.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomLeft) && waterTiles.Contains(tile2.Bottom) && tile2.BottomLeft.Type == HexType.ThreeQuater && tile2.Bottom.Type == HexType.Empty)
				{
					rotation2 = -120f;
					GameObject gameObject50 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject50.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopLeft) && waterTiles.Contains(tile2.Bottom) && tile2.TopLeft.Type == HexType.ThreeQuater && tile2.Bottom.Type == HexType.Empty)
				{
					rotation2 = -180f;
					GameObject gameObject51 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject51.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Top) && waterTiles.Contains(tile2.TopLeft) && tile2.Top.Type == HexType.ThreeQuater && tile2.TopLeft.Type == HexType.Empty)
				{
					rotation2 = -240f;
					GameObject gameObject52 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject52.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopRight) && waterTiles.Contains(tile2.Top) && tile2.TopRight.Type == HexType.ThreeQuater && tile2.Top.Type == HexType.Empty)
				{
					rotation2 = -300f;
					GameObject gameObject53 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject53.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomRight) && waterTiles.Contains(tile2.TopRight) && tile2.BottomRight.Type == HexType.ThreeQuater && tile2.TopRight.Type == HexType.Empty)
				{
					rotation2 = -360f;
					GameObject gameObject54 = Object.Instantiate(tileSet2.WaterWakeQuaterHalfRight(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject54.transform.parent = layerTransform2;
					createCount++;
				}
			}
			if (tile2.Type == HexType.Half)
			{
				if (waterTiles.Contains(tile2.TopLeft) && waterTiles.Contains(tile2.BottomLeft) && tile2.TopLeft.Type == HexType.Half && tile2.BottomLeft.Type == HexType.Empty)
				{
					rotation2 = -60f;
					GameObject gameObject55 = Object.Instantiate(tileSet2.WaterWakeHalfCorner(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject55.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Top) && waterTiles.Contains(tile2.TopLeft) && tile2.Top.Type == HexType.Half && tile2.TopLeft.Type == HexType.Empty)
				{
					rotation2 = -120f;
					GameObject gameObject56 = Object.Instantiate(tileSet2.WaterWakeHalfCorner(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject56.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.TopRight) && waterTiles.Contains(tile2.Top) && tile2.TopRight.Type == HexType.Half && tile2.Top.Type == HexType.Empty)
				{
					rotation2 = -180f;
					GameObject gameObject57 = Object.Instantiate(tileSet2.WaterWakeHalfCorner(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject57.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomRight) && waterTiles.Contains(tile2.TopRight) && tile2.BottomRight.Type == HexType.Half && tile2.TopRight.Type == HexType.Empty)
				{
					rotation2 = -240f;
					GameObject gameObject58 = Object.Instantiate(tileSet2.WaterWakeHalfCorner(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject58.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.Bottom) && waterTiles.Contains(tile2.BottomRight) && tile2.Bottom.Type == HexType.Half && tile2.BottomRight.Type == HexType.Empty)
				{
					rotation2 = -300f;
					GameObject gameObject59 = Object.Instantiate(tileSet2.WaterWakeHalfCorner(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject59.transform.parent = layerTransform2;
					createCount++;
				}
				if (waterTiles.Contains(tile2.BottomLeft) && waterTiles.Contains(tile2.Bottom) && tile2.BottomLeft.Type == HexType.Half && tile2.Bottom.Type == HexType.Empty)
				{
					rotation2 = -360f;
					GameObject gameObject60 = Object.Instantiate(tileSet2.WaterWakeHalfCorner(), pos2, Quaternion.Euler(0f, rotation2, 0f));
					gameObject60.transform.parent = layerTransform2;
					createCount++;
				}
			}
		}
	}

	private HexTile[] FindHoles(HexGrid grid)
	{
		List<HexTile> list = new List<HexTile>();
		List<HexTile> list2 = new List<HexTile>();
		HexTile[,] tiles = grid.Tiles;
		int length = tiles.GetLength(0);
		int length2 = tiles.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				HexTile hexTile = tiles[i, j];
				hexTile.m_metaData = 0;
			}
		}
		HexTile[,] tiles2 = grid.Tiles;
		int length3 = tiles2.GetLength(0);
		int length4 = tiles2.GetLength(1);
		for (int k = 0; k < length3; k++)
		{
			for (int l = 0; l < length4; l++)
			{
				HexTile hexTile2 = tiles2[k, l];
				if (hexTile2.Type == HexType.Empty && !hexTile2.IsSetPiece)
				{
					list.Add(hexTile2);
					if (hexTile2.X == 0 || hexTile2.X == grid.Width - 1 || hexTile2.Y == 0 || hexTile2.Y == grid.Height - 1)
					{
						list2.Add(hexTile2);
					}
				}
			}
		}
		while (list2.Count > 0)
		{
			HexTile hexTile3 = list2[0];
			List<HexTile> tiles3 = new List<HexTile>();
			tiles3.Add(hexTile3);
			GetConnectedNeighbours(hexTile3, HexType.Empty, ref tiles3);
			foreach (HexTile item in tiles3)
			{
				list2.Remove(item);
				list.Remove(item);
			}
		}
		return list.ToArray();
	}

	private List<List<HexTile>> FindIslands(int grixIdx)
	{
		List<HexTile> list = new List<HexTile>();
		List<List<HexTile>> list2 = new List<List<HexTile>>();
		HexGrid hexGrid = m_hexGrids[grixIdx];
		HexTile[,] tiles = hexGrid.Tiles;
		int length = tiles.GetLength(0);
		int length2 = tiles.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j++)
			{
				HexTile hexTile = tiles[i, j];
				if (hexTile.Type == HexType.Hex)
				{
					list.Add(hexTile);
				}
			}
		}
		while (list.Count > 0)
		{
			HexTile hexTile2 = list[0];
			List<HexTile> tiles2 = new List<HexTile>();
			tiles2.Add(hexTile2);
			GetConnectedNeighbours(hexTile2, HexType.Hex, ref tiles2);
			list2.Add(tiles2);
			foreach (HexTile item in tiles2)
			{
				list.Remove(item);
			}
		}
		return list2;
	}

	private float MinDistanceBetweenIslands(List<HexTile> islandA, List<HexTile> islandB)
	{
		float num = float.MaxValue;
		foreach (HexTile islandum in islandA)
		{
			foreach (HexTile item in islandB)
			{
				float sqrMagnitude = new Vector3(islandum.X - item.X, 0f, islandum.Y - item.Y).sqrMagnitude;
				num = Mathf.Min(num, sqrMagnitude);
			}
		}
		return num;
	}

	private int CompareIslandsByDistanceToMainIsland(HexIsland islandA, HexIsland islandB)
	{
		if (islandA == null)
		{
			if (islandB == null)
			{
				return 0;
			}
			return -1;
		}
		if (islandB == null)
		{
			return 1;
		}
		float distanceToMain = islandA.m_distanceToMain;
		float distanceToMain2 = islandB.m_distanceToMain;
		return distanceToMain.CompareTo(distanceToMain2);
	}

	private IEnumerator RemoveFarIslands()
	{
		for (int gridIdx = 0; gridIdx < m_hexGrids.Length; gridIdx++)
		{
			List<List<HexTile>> islandTiles = FindIslands(gridIdx);
			if (islandTiles.Count <= 0)
			{
				continue;
			}
			yield return null;
			int mainIslandIdx = -1;
			float maxTiles = 0f;
			for (int i = 0; i < islandTiles.Count; i++)
			{
				if ((float)islandTiles[i].Count > maxTiles)
				{
					maxTiles = islandTiles[i].Count;
					mainIslandIdx = i;
				}
			}
			m_mainIsland = new List<HexTile>(islandTiles[mainIslandIdx]);
			int checkCount = 0;
			List<HexIsland> hexIslands = new List<HexIsland>();
			for (int j = 0; j < islandTiles.Count; j++)
			{
				HexIsland island = new HexIsland
				{
					m_tiles = islandTiles[j]
				};
				if (mainIslandIdx != j)
				{
					island.m_distanceToMain = MinDistanceBetweenIslands(island.m_tiles, m_mainIsland);
				}
				hexIslands.Add(island);
				if (checkCount > 8)
				{
					checkCount = 0;
					yield return null;
				}
			}
			hexIslands.Sort(CompareIslandsByDistanceToMainIsland);
			yield return null;
			List<int> islandsToDelete = new List<int>();
			for (int k = 1; k < hexIslands.Count; k++)
			{
				islandsToDelete.Add(k);
			}
			for (int l = 0; l < hexIslands.Count; l++)
			{
				float num = float.MaxValue;
				for (int m = 0; m < hexIslands.Count; m++)
				{
					if (l != m && !islandsToDelete.Contains(m))
					{
						float num2 = MinDistanceBetweenIslands(hexIslands[l].m_tiles, hexIslands[m].m_tiles);
						if (num2 < num)
						{
							num = num2;
						}
					}
				}
				if (num != float.MaxValue && num <= m_maxTilesBetweenIslands)
				{
					islandsToDelete.Remove(l);
				}
			}
			foreach (int item in islandsToDelete)
			{
				foreach (HexTile tile in hexIslands[item].m_tiles)
				{
					if (tile.IsSetPiece)
					{
						RemoveSetPiece(tile.SetPiece);
					}
					tile.Type = HexType.Empty;
				}
			}
		}
	}

	private IEnumerator RemoveVisibiltyIssues()
	{
		List<HexTile> obstructedTiles = new List<HexTile>();
		List<HexTile> flattenedTiles = new List<HexTile>();
		int checkCount = 0;
		int gridIdx = 0;
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid grid in hexGrids)
		{
			for (int y = grid.Height - 1; y >= 0; y--)
			{
				for (int x = 0; x < grid.Width; x++)
				{
					if (checkCount > 500)
					{
						checkCount = 0;
						yield return null;
					}
					HexTile tile = grid.GetTile(x, y);
					checkCount++;
					if (!IsTileVisibiltyObstructed(tile, gridIdx))
					{
						continue;
					}
					obstructedTiles.Add(tile);
					HexTile top = tile.Top;
					if (top != null)
					{
						if (top.IsSetPiece)
						{
							FlattenSetPiece(top.SetPiece, gridIdx);
						}
						FlattenTile(top, gridIdx);
						flattenedTiles.Add(top);
					}
				}
			}
			gridIdx++;
		}
	}

	private LevelTileSet GetTileSet(int tileSetIdx)
	{
		if (tileSetIdx < 0 || tileSetIdx >= Singleton<Game>.Instance.LevelManager.TileSetCount)
		{
			return null;
		}
		return Singleton<Game>.Instance.LevelManager.GetTileSet(tileSetIdx);
	}

	private IEnumerator CreateDifferentSurfaceTypes(Level level)
	{
		for (int optionIdx = 0; optionIdx < Singleton<Game>.Instance.LevelManager.TileSetCount; optionIdx++)
		{
			HexGrid[] hexGrids = m_hexGrids;
			foreach (HexGrid hexGrid in hexGrids)
			{
				int num = Mathf.FloorToInt((float)hexGrid.Tiles.Length * Singleton<Game>.Instance.LevelManager.GetTileSetChance(optionIdx));
				int nextRandomRange = level.GetNextRandomRange(4, 8);
				for (int j = 0; j < nextRandomRange; j++)
				{
					if (num <= 0)
					{
						break;
					}
					HexTile tile = hexGrid.GetTile(level.GetNextRandomRange(0, hexGrid.Width - 1), level.GetNextRandomRange(0, hexGrid.Height - 1));
					tile.TileSet = optionIdx;
					List<HexTile> tiles = new List<HexTile>();
					GetTileSplat(level, tile, Mathf.FloorToInt((float)num * level.GetNextRandomRange(0.25f, 1f)), ref tiles);
					foreach (HexTile item in tiles)
					{
						if (item != null)
						{
							item.TileSet = optionIdx;
						}
					}
				}
			}
			yield return null;
		}
	}

	private IEnumerator AddSetPieces()
	{
		List<HexTile> removedTiles = new List<HexTile>();
		Transform categoryTransform = GetCategoryTransform(base.transform, "SetPiece", true);
		int setPieceIdx = 0;
		Level[] setPieces = null;
		if (Singleton<Game>.Instance.SpecialEventManager.RequestChance())
		{
			Singleton<Game>.Instance.SpecialEventManager.LockChoice();
			setPieces = new Level[1] { Singleton<Game>.Instance.SpecialEventManager.SpawnSetPiece };
		}
		else
		{
			setPieces = Singleton<Game>.Instance.LevelManager.CurrentLevel.GetRandomSetPieces();
		}
		if (setPieces == null)
		{
			yield break;
		}
		Level[] array = setPieces;
		foreach (Level prefab in array)
		{
			Level setPiece = Utils.CreateFromPrefab(Singleton<Game>.Instance.Player.Character.GetSetPieceReplacement(prefab), "SetPiece");
			yield return StartCoroutine(setPiece.LoadLevelAsync(null, true));
			int setWidth = setPiece.HexMap.m_hexGrids[0].Width;
			int setHeight = setPiece.HexMap.m_hexGrids[0].Height;
			int xPos = Singleton<Game>.Instance.LevelManager.CurrentLevel.GetNextRandomRange(0, m_hexGrids[0].Width - setWidth);
			int yPos = Singleton<Game>.Instance.LevelManager.CurrentLevel.GetNextRandomRange(0, m_hexGrids[0].Height - setHeight);
			if (xPos % 2 != 0)
			{
				xPos++;
			}
			if (yPos % 2 != 0)
			{
				yPos++;
			}
			int layerIdx = 0;
			for (int j = 0; j < setHeight; j++)
			{
				for (int k = 0; k < setWidth; k++)
				{
					HexTile tile = setPiece.HexMap.m_hexGrids[0].GetTile(k, j);
					if (tile.Occupied)
					{
						HexTile highestOccupiedTile = GetHighestOccupiedTile(xPos + k, yPos + j);
						if (highestOccupiedTile != null)
						{
							layerIdx = Mathf.Max(layerIdx, highestOccupiedTile.Layer);
						}
					}
				}
			}
			Transform layerTransform = GetCategoryTransform(categoryTransform, "Layer" + layerIdx);
			setPiece.HexMap.m_overrideGeometry.transform.parent = layerTransform;
			layerTransform.position += HexWorldPosition(m_hexGrids[layerIdx].GetTile(xPos, yPos), layerIdx);
			for (int l = 0; l < setHeight; l++)
			{
				for (int m = 0; m < setWidth; m++)
				{
					HexTile tile2 = setPiece.HexMap.m_hexGrids[0].GetTile(m, l);
					if (!tile2.Occupied)
					{
						continue;
					}
					HexGrid[] hexGrids = m_hexGrids;
					foreach (HexGrid hexGrid in hexGrids)
					{
						HexTile tile3 = hexGrid.GetTile(xPos + m, yPos + l);
						if (tile3 != null)
						{
							if (hexGrid.Layer == layerIdx)
							{
								tile3.Type = HexType.Hex;
							}
							else
							{
								tile3.Type = HexType.Empty;
							}
							tile3.SetPiece = setPieceIdx;
							if (setPiece.HexMap.m_removeGeometryUnderneath)
							{
								tile3.Locked = true;
							}
							removedTiles.Add(tile3);
						}
					}
				}
			}
			Object.DestroyImmediate(setPiece.gameObject);
			setPieceIdx++;
		}
	}

	private void RemoveSetPiece(int setPieceIdx)
	{
		Transform transform = base.transform.Find("SetPiece");
		if (transform != null && transform.gameObject != null)
		{
			Object.DestroyImmediate(transform.gameObject);
		}
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile[,] tiles = hexGrid.Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					HexTile hexTile = tiles[j, k];
					if (hexTile.SetPiece == setPieceIdx)
					{
						hexTile.SetPiece = -1;
					}
				}
			}
		}
	}

	private void FlattenSetPiece(int setPieceIdx, int gridIdx)
	{
		Transform transform = base.transform.Find("SetPiece");
		Transform transform2 = transform.GetChild(0).transform;
		if (transform2 != null && transform2.gameObject != null)
		{
			Vector3 position = transform2.position;
			position.y = m_levelHeights[gridIdx];
			transform2.position = position;
		}
		int num = 0;
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile[,] tiles = hexGrid.Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					HexTile hexTile = tiles[j, k];
					if (hexTile.SetPiece == setPieceIdx)
					{
						hexTile.SetPiece = -1;
						FlattenTile(hexTile, num);
						HexTile tile = m_hexGrids[gridIdx].GetTile(hexTile.X, hexTile.Y);
						if (tile != null)
						{
							tile.SetPiece = setPieceIdx;
						}
					}
				}
			}
			num++;
		}
	}

	private IEnumerator CreateDynamicTiles()
	{
		Transform categoryTransform = GetCategoryTransform(base.transform, "DynamicTiles", true);
		int checkCount = 0;
		int createCount = 0;
		List<HexTile> checkTiles = new List<HexTile>();
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile[,] tiles = hexGrid.Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					HexTile item = tiles[j, k];
					checkTiles.Add(item);
				}
			}
		}
		Utils.ShuffleList(checkTiles, Singleton<Game>.Instance.LevelManager.CurrentLevel.GetNextRandomRange(0, int.MaxValue));
		foreach (KeyValuePair<DynamicTile, float> pair in Singleton<Game>.Instance.LevelManager.CurrentLevel.DynamicTiles)
		{
			if (pair.Value < Singleton<Game>.Instance.LevelManager.CurrentLevel.GetNextRandomRange(0f, 1f))
			{
				continue;
			}
			DynamicTile dynamicTile = pair.Key;
			SpawnRulesBase[] spawnRules = dynamicTile.GetComponents<SpawnRulesBase>();
			dynamicTile.Count.Randomize();
			List<HexTile> availableTiles = new List<HexTile>();
			SpawnRulesBase[] array = spawnRules;
			foreach (SpawnRulesBase spawnRule in array)
			{
				int spawnChecks = spawnRule.CheckCount();
				foreach (HexTile tile in checkTiles)
				{
					if (spawnRules == null || spawnRule.CheckTile(tile))
					{
						availableTiles.Add(tile);
					}
					if ((float)availableTiles.Count >= dynamicTile.Count.Value)
					{
						break;
					}
					checkCount += spawnChecks;
					if (checkCount > 5000)
					{
						checkCount = 0;
						yield return null;
					}
				}
				if ((float)availableTiles.Count >= dynamicTile.Count.Value)
				{
					break;
				}
			}
			for (int m = 0; (float)m < dynamicTile.Count.Value; m++)
			{
				if (availableTiles.Count <= 0)
				{
					break;
				}
				if (createCount > m_prefabCreateLimitPerFrame)
				{
					createCount = 0;
					yield return null;
				}
				HexTile choosenTile = availableTiles[m];
				Transform layerTransform = GetCategoryTransform(categoryTransform, "Layer" + choosenTile.Layer);
				Vector3 pos = HexWorldPosition(choosenTile, choosenTile.Layer);
				DynamicTile obj = Object.Instantiate(dynamicTile, pos, Quaternion.Euler(0f, 0f - choosenTile.Rotation, 0f));
				obj.tag = GetTileSet(choosenTile.TileSet).m_tilesLowValues[0].gameObject.tag;
				obj.gameObject.transform.parent = layerTransform;
				createCount++;
				GameObject edgePrefab = obj.GetEdgePrefab(GetTileSet(choosenTile.TileSet), choosenTile.Layer);
				if (edgePrefab != null)
				{
					GameObject gameObject = Object.Instantiate(edgePrefab, pos, Quaternion.Euler(0f, 0f - choosenTile.Rotation, 0f));
					gameObject.gameObject.transform.parent = obj.gameObject.transform;
					Renderer component = gameObject.gameObject.GetComponent<Renderer>();
					if (component != null)
					{
						Level.ReplaceMaterials(component);
					}
					if (obj.CastShadows)
					{
						gameObject.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.On;
					}
				}
				HexGrid[] hexGrids2 = m_hexGrids;
				foreach (HexGrid hexGrid2 in hexGrids2)
				{
					HexTile tile2 = hexGrid2.GetTile(choosenTile.X, choosenTile.Y);
					if (tile2 != null)
					{
						tile2.Locked = true;
						hexGrid2.RemoveTile(tile2.X, tile2.Y);
					}
				}
				choosenTile.Type = HexType.Hex;
				choosenTile.DynamicTile = dynamicTile.ID.GetHashCode();
				choosenTile.Locked = true;
				checkTiles.Remove(choosenTile);
			}
		}
	}

	public void RepositionLevelAroundPlayer()
	{
		Vector3 vector = Vector3.zero;
		if (Singleton<Game>.Instance.LevelManager.IsRetryLevel)
		{
			vector = Vector3.zero;
		}
		else
		{
			SpawnTile spawnTile = Object.FindObjectOfType<SpawnTile>();
			if (spawnTile != null)
			{
				vector = Singleton<Game>.Instance.VisualCamera.Target.transform.position - spawnTile.transform.position;
			}
		}
		vector.y = 0f;
		Singleton<Game>.Instance.LevelManager.CurrentLevel.SpawnPos = vector;
		GetCategoryTransform(base.transform, "SetPiece").position += vector;
		GetCategoryTransform(base.transform, "DynamicTiles").position += vector;
		Singleton<Game>.Instance.Light.transform.position = new Vector3(vector.x, Singleton<Game>.Instance.Light.transform.position.y, vector.z);
	}

	public int GetMaxTileHeight(int x, int y)
	{
		int result = 0;
		for (int i = 0; i < m_hexGrids.Length; i++)
		{
			HexTile tile = m_hexGrids[i].GetTile(x, y);
			if (tile != null && tile.Occupied)
			{
				result = i;
			}
		}
		return result;
	}

	public HexTile GetTileAbove(HexTile tile, int layer = -1)
	{
		if (layer == -1)
		{
			layer = tile.Layer + 1;
		}
		if (layer >= m_hexGrids.Length)
		{
			return null;
		}
		return m_hexGrids[layer].GetTile(tile.X, tile.Y);
	}

	public HexTile GetTileBelow(HexTile tile, int layer = -1)
	{
		if (layer == -1)
		{
			layer = tile.Layer - 1;
		}
		if (layer < 0)
		{
			return null;
		}
		return m_hexGrids[layer].GetTile(tile.X, tile.Y);
	}

	public HexTile GetHighestOccupiedTile(int x, int y)
	{
		HexTile result = null;
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile tile = hexGrid.GetTile(x, y);
			if (tile != null && tile.Occupied)
			{
				result = tile;
			}
		}
		return result;
	}

	public HexTile GetHighestWaterTile(int x, int y)
	{
		HexTile result = null;
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile tile = hexGrid.GetTile(x, y);
			if (tile != null && tile.IsWater)
			{
				result = tile;
			}
		}
		return result;
	}

	public HexTile GetHighestNonEmptyTile(int x, int y)
	{
		HexTile result = null;
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile tile = hexGrid.GetTile(x, y);
			if (tile != null && !tile.IsWater)
			{
				result = tile;
			}
		}
		return result;
	}

	public HexTile[] GetHighestNonEmptyNeighbours(HexTile tile)
	{
		List<HexTile> list = new List<HexTile>();
		if (tile == null)
		{
			return list.ToArray();
		}
		HexTile[] neighbours = tile.Neighbours;
		HexTile[] array = neighbours;
		foreach (HexTile hexTile in array)
		{
			if (hexTile != null)
			{
				HexTile highestNonEmptyTile = GetHighestNonEmptyTile(hexTile.X, hexTile.Y);
				if (highestNonEmptyTile != null)
				{
					list.Add(highestNonEmptyTile);
				}
			}
		}
		return list.ToArray();
	}

	public List<HexTile> GetTilesInRadius(HexTile tile, int tileRadius)
	{
		List<HexTile> tiles = new List<HexTile>();
		HexGrid[] hexGrids = HexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile[,] tiles2 = hexGrid.Tiles;
			int length = tiles2.GetLength(0);
			int length2 = tiles2.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					HexTile hexTile = tiles2[j, k];
					hexTile.m_metaData = 0;
				}
			}
		}
		GetTilesInRadiusInternal(tile, tileRadius, ref tiles);
		return tiles;
	}

	private void GetTilesInRadiusInternal(HexTile tile, int tileRadius, ref List<HexTile> tiles)
	{
		if (tile == null || tileRadius <= 0)
		{
			return;
		}
		HexTile[] neighbours = tile.Neighbours;
		foreach (HexTile hexTile in neighbours)
		{
			if (hexTile != null)
			{
				if (hexTile.m_metaData != 10)
				{
					hexTile.m_metaData = 10;
					tiles.Add(hexTile);
				}
				if (tileRadius - 1 > 0)
				{
					GetTilesInRadiusInternal(hexTile, tileRadius - 1, ref tiles);
				}
			}
		}
	}

	public void GetTileSplat(Level level, HexTile tile, int tileCount, ref List<HexTile> tiles)
	{
		if (tile == null || tiles.Count >= tileCount)
		{
			return;
		}
		HexTile hexTile = tile;
		switch (level.GetNextRandomRange(0, 6))
		{
		case 0:
			hexTile = tile.Top;
			break;
		case 1:
			hexTile = tile.TopLeft;
			break;
		case 2:
			hexTile = tile.TopRight;
			break;
		case 3:
			hexTile = tile.Bottom;
			break;
		case 4:
			hexTile = tile.BottomLeft;
			break;
		case 5:
			hexTile = tile.BottomRight;
			break;
		}
		tiles.Add(hexTile);
		if (tiles.Count < tileCount)
		{
			GetTileSplat(level, hexTile, tileCount, ref tiles);
			if (tiles.Count < tileCount)
			{
			}
		}
	}

	private void FlattenTile(HexTile tile, int gridIdx)
	{
		tile.Type = HexType.Hex;
		for (int i = gridIdx + 1; i < m_hexGrids.Length; i++)
		{
			HexTile tile2 = m_hexGrids[i].GetTile(tile.X, tile.Y);
			if (tile2 != null && tile2.Occupied)
			{
				m_hexGrids[i].RemoveTile(tile2.X, tile2.Y);
			}
		}
	}

	public bool IsTileVisibiltyObstructed(HexTile tile, int gridIdx)
	{
		if (tile.Occupied)
		{
			for (int i = gridIdx + 1; i < m_hexGrids.Length; i++)
			{
				HexTile tile2 = m_hexGrids[i].GetTile(tile.X, tile.Y);
				if (tile2 != null && tile2.Occupied)
				{
					return false;
				}
			}
			HexTile top = tile.Top;
			if (top == null)
			{
				return false;
			}
			for (int j = gridIdx + 1; j < m_hexGrids.Length; j++)
			{
				HexTile tile3 = m_hexGrids[j].GetTile(top.X, top.Y);
				if (tile3 != null && tile3.Occupied)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void GetConnectedNeighbours(HexTile tile, HexType type, ref List<HexTile> tiles)
	{
		HexTile[] neighbours = tile.Neighbours;
		HexTile[] array = neighbours;
		foreach (HexTile hexTile in array)
		{
			if (hexTile != null && hexTile.Type == type && hexTile.m_metaData != 20)
			{
				hexTile.m_metaData = 20;
				tiles.Add(hexTile);
				GetConnectedNeighbours(hexTile, type, ref tiles);
			}
		}
	}

	public bool IsTileConnectedToTile(HexTile tile, HexTile target, ref List<HexTile> checkedTiles)
	{
		checkedTiles.Add(tile);
		HexTile[] neighbours = tile.Neighbours;
		HexTile[] array = neighbours;
		foreach (HexTile hexTile in array)
		{
			if (hexTile != null)
			{
				if (hexTile == target)
				{
					return true;
				}
				if (hexTile.Type == HexType.Hex && !checkedTiles.Contains(hexTile) && IsTileConnectedToTile(hexTile, target, ref checkedTiles))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsTileConnectToMapBounds(HexTile tile, ref List<HexTile> checkedTiles)
	{
		checkedTiles.Add(tile);
		HexTile[] neighbours = tile.Neighbours;
		HexTile[] array = neighbours;
		foreach (HexTile hexTile in array)
		{
			if (hexTile == null)
			{
				return true;
			}
			if (hexTile.Type != HexType.Hex && !checkedTiles.Contains(hexTile) && IsTileConnectToMapBounds(hexTile, ref checkedTiles))
			{
				return true;
			}
		}
		return false;
	}

	public Vector3 HexWorldPosition(HexTile tile, int layer = -1)
	{
		if (layer == -1)
		{
			layer = tile.Layer;
		}
		return HexWorldPosition(tile.X, tile.Y, layer);
	}

	public Vector3 HexWorldPosition(int x, int y, int layer)
	{
		float num = (float)x * m_hexWidth * 1.5f;
		num += (float)(y % 2) * (m_hexWidth * 1.5f) / 2f;
		float z = (float)y * m_hexHeight / 2f;
		return new Vector3(num, m_levelHeights[layer], z) + Singleton<Game>.Instance.LevelManager.CurrentLevel.SpawnPos;
	}

	public Transform GetCategoryTransform(Transform parent, string category, bool destroy = false)
	{
		if (parent.Find(category) != null)
		{
			if (!destroy)
			{
				return parent.Find(category);
			}
			Object.DestroyImmediate(parent.Find(category).gameObject);
		}
		GameObject gameObject = new GameObject();
		gameObject.name = category;
		gameObject.transform.parent = parent;
		return gameObject.transform;
	}

	public List<HexTile> GetOutterTiles()
	{
		List<HexTile> list = new List<HexTile>();
		HexGrid[] hexGrids = m_hexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile[,] tiles = hexGrid.Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length2; k++)
				{
					HexTile hexTile = tiles[j, k];
					if (hexTile.Type == HexType.Empty || hexTile.NeighbouringWater)
					{
						continue;
					}
					if (hexTile.Type != HexType.Hex && hexTile.Type != HexType.Empty)
					{
						bool flag = true;
						HexGrid[] hexGrids2 = m_hexGrids;
						foreach (HexGrid hexGrid2 in hexGrids2)
						{
							HexTile tile = hexGrid2.GetTile(hexTile.X, hexTile.Y);
							if (tile != null && tile.Type == HexType.Hex)
							{
								flag = false;
								break;
							}
							if (tile != null && tile.IsWater)
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							list.Add(hexTile);
						}
					}
					else
					{
						HexTile[] highestNonEmptyNeighbours = GetHighestNonEmptyNeighbours(hexTile);
						if (highestNonEmptyNeighbours.Length != 6)
						{
							list.Add(hexTile);
						}
					}
				}
			}
		}
		return list;
	}
}
