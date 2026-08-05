using System;
using System.Collections;
using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Level : MonoBehaviour, ISerializationCallbackReceiver, IEventSystemHandler
{
	public delegate void LoadComplete();

	[Serializable]
	public class DebugOptions
	{
		public bool layer0Visible = true;

		public bool layer1Visible = true;

		public bool layer2Visible = true;

		public bool showGridLayer0 = true;

		public bool showGridLayer1;

		public bool showGridLayer2;

		[Button("Update", "DebugUpdate", true)]
		public bool updateHidden = true;

		[Button("Reload Seed", "DebugReloadSeed", true)]
		public bool reloadHidden = true;

		[Button("Next Seed", "DebugNextSeed", true)]
		public bool nextHidden = true;

		[Button("Prev Seed", "DebugPrevSeed", true)]
		public bool prevHidden = true;

		[HideInInspector]
		public List<int> m_usedSeeds = new List<int>();

		[HideInInspector]
		public int m_seedIdx;
	}

	public int seed;

	public List<HexMapInfo> m_mapsResources;

	private HexMap m_hexMap;

	private HexMap m_hexMapPrefab;

	private string m_hexMapPath = string.Empty;

	private System.Random m_randomGenerator;

	[Dictionary("m_dynamicTilesValues")]
	public List<DynamicTile> m_dynamicTiles;

	[HideInInspector]
	public List<float> m_dynamicTilesValues;

	private Dictionary<DynamicTile, float> m_dynamicTile;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_sceneryDensity;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_waterSceneryDensity;

	[SerializeField]
	private GameObject m_cloudsPrefab;

	[SerializeField]
	private Vector3 m_spawnPos = Vector3.zero;

	[SerializeField]
	private LevelTileSetGroup m_overrideTilesetGroup;

	[SerializeField]
	private UnityEvent m_onLoadComplete = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onUnloadComplete = new UnityEvent();

	private List<HexTile> m_availableLandTiles = new List<HexTile>();

	private List<HexTile> m_availableWaterTiles = new List<HexTile>();

	private float m_progress;

	public string m_objectOfDesire = "collectible";

	public int m_bonusObjectsOnComplete = 5;

	private Rect m_worldBounds;

	[SerializeField]
	private DebugOptions m_debugOptions;

	private List<List<HexTile>> m_debugSelectedList = new List<List<HexTile>>();

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

	public HexMap HexMap
	{
		get
		{
			return m_hexMap;
		}
	}

	public HexMap HexMapPrefab
	{
		get
		{
			return m_hexMapPrefab;
		}
	}

	public string HexMapPath
	{
		get
		{
			return m_hexMapPath;
		}
	}

	public Dictionary<DynamicTile, float> DynamicTiles
	{
		get
		{
			return m_dynamicTile;
		}
	}

	public Vector3 SpawnPos
	{
		get
		{
			return m_spawnPos;
		}
		set
		{
			m_spawnPos = value;
		}
	}

	public Rect WorldBounds
	{
		get
		{
			return m_worldBounds;
		}
	}

	public HexMapInfo[] Maps
	{
		get
		{
			return m_mapsResources.ToArray();
		}
	}

	public LevelTileSetGroup OverrideTileSetGroup
	{
		get
		{
			return m_overrideTilesetGroup;
		}
	}

	public event LoadComplete OnLoadPreComplete;

	public event LoadComplete OnLoadComplete;

	public float GetNextRandomValue()
	{
		int num = m_randomGenerator.Next();
		return (float)num / 2.1474836E+09f;
	}

	public float GetNextRandomRange(float min, float max)
	{
		float num = max - min;
		return min + GetNextRandomValue() * num;
	}

	public int GetNextRandomRange(int min, int max)
	{
		return m_randomGenerator.Next(min, max);
	}

	public Transform GetTransform(string name)
	{
		return HexMap.GetCategoryTransform(base.transform, name);
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_dynamicTiles, m_dynamicTilesValues, out m_dynamicTile, "dynamicTile");
	}

	public string GetRandomMap()
	{
		if (m_mapsResources.Count <= 0)
		{
			return null;
		}
		float num = 0f;
		foreach (HexMapInfo mapsResource in m_mapsResources)
		{
			num += mapsResource.m_chance;
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		num = 0f;
		foreach (HexMapInfo mapsResource2 in m_mapsResources)
		{
			num += mapsResource2.m_chance;
			if (num >= num2)
			{
				return mapsResource2.m_path;
			}
		}
		return null;
	}

	private void OnDestroy()
	{
		m_onUnloadComplete.Invoke();
	}

	public void LoadLevel(string mapPath = null)
	{
		StartCoroutine(LoadLevelAsync(mapPath));
	}

	public IEnumerator LoadLevelAsync(string mapPath = null, bool isSetPiece = false)
	{
		if (Singleton<Game>.Instance.LevelManager.CurrentLevel.gameObject == base.gameObject)
		{
			UnityEngine.Random.seed = seed;
			m_randomGenerator = new System.Random(seed);
		}
		else
		{
			m_randomGenerator = Singleton<Game>.Instance.LevelManager.CurrentLevel.m_randomGenerator;
		}
		m_progress = 0f;
		if (m_hexMap != null)
		{
			UnityEngine.Object.Destroy(m_hexMap.gameObject);
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			UnityEngine.Object.Destroy(base.transform.GetChild(i).gameObject);
		}
		if (m_hexMapPrefab != null)
		{
			Resources.UnloadAsset(m_hexMapPrefab);
		}
		m_hexMapPath = string.Empty;
		if (mapPath == null || mapPath == string.Empty)
		{
			mapPath = GetRandomMap();
		}
		else
		{
			GetRandomMap();
		}
		ResourceRequest request = Resources.LoadAsync<HexMap>(mapPath);
		while (!request.isDone)
		{
			yield return null;
		}
		m_hexMapPrefab = request.asset as HexMap;
		m_hexMapPath = mapPath;
		m_hexMap = Utils.CreateFromPrefab(m_hexMapPrefab, m_hexMapPrefab.name);
		m_hexMap.transform.parent = base.transform;
		m_hexMap.transform.localPosition = Vector3.zero;
		yield return null;
		yield return StartCoroutine(m_hexMap.GenerateLevel(this));
		if (isSetPiece)
		{
			yield break;
		}
		if (!m_hexMap.OverridesGeometry)
		{
			GetAvailableTiles();
			yield return StartCoroutine(MergeMeshes());
			yield return StartCoroutine(CreateScenery());
			yield return StartCoroutine(CreateWaterScenery());
			yield return StartCoroutine(CreateOtherSpawnValues());
		}
		yield return StartCoroutine(CreateClouds());
		m_worldBounds = new Rect(2.1474836E+09f, 2.1474836E+09f, -2.1474836E+09f, -2.1474836E+09f);
		HexGrid[] hexGrids = HexMap.HexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			HexTile[,] tiles = hexGrid.Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int k = 0; k < length; k++)
			{
				for (int l = 0; l < length2; l++)
				{
					HexTile hexTile = tiles[k, l];
					if (hexTile.Occupied)
					{
						m_worldBounds.xMin = Mathf.Min(m_worldBounds.xMin, hexTile.X);
						m_worldBounds.xMax = Mathf.Max(m_worldBounds.xMax, hexTile.X);
						m_worldBounds.yMin = Mathf.Min(m_worldBounds.yMin, hexTile.Y);
						m_worldBounds.yMax = Mathf.Max(m_worldBounds.yMax, hexTile.Y);
					}
				}
			}
		}
		Vector3 min = HexMap.HexWorldPosition(Mathf.FloorToInt(m_worldBounds.xMin), Mathf.FloorToInt(m_worldBounds.yMin), 0);
		Vector3 max = HexMap.HexWorldPosition(Mathf.FloorToInt(m_worldBounds.xMax), Mathf.FloorToInt(m_worldBounds.yMax), 0);
		m_worldBounds.xMin = min.x;
		m_worldBounds.yMin = min.z;
		m_worldBounds.xMax = max.x;
		m_worldBounds.yMax = max.z;
		if (this.OnLoadPreComplete != null)
		{
			this.OnLoadPreComplete();
		}
		m_progress = 1f;
		m_onLoadComplete.Invoke();
		if (this.OnLoadComplete != null)
		{
			this.OnLoadComplete();
		}
	}

	public Level[] GetRandomSetPieces()
	{
		Level[] array = null;
		SpawnValues[] components = base.gameObject.GetComponents<SpawnValues>();
		SpawnValues[] array2 = components;
		foreach (SpawnValues spawnValues in array2)
		{
			if (!(spawnValues.GroupName == "SetPieces"))
			{
				continue;
			}
			int num = 1;
			if (spawnValues.SpawnCounts.Count > 0)
			{
				num = spawnValues.GetRandomCount(this);
			}
			array = new Level[num];
			for (int j = 0; j < num; j++)
			{
				if (!(spawnValues.Density < GetNextRandomRange(0f, 1f)))
				{
					GameObject randomPrefab = spawnValues.GetRandomPrefab(this);
					array[j] = randomPrefab.GetComponent<Level>();
				}
			}
		}
		return array;
	}

	private void GetAvailableTiles()
	{
		m_availableWaterTiles.Clear();
		m_availableLandTiles.Clear();
		int width = HexMap.HexGrids[0].Width;
		int height = HexMap.HexGrids[0].Height;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				HexTile highestOccupiedTile = HexMap.GetHighestOccupiedTile(j, i);
				HexTile highestWaterTile = HexMap.GetHighestWaterTile(j, i);
				HexTile hexTile = null;
				hexTile = ((highestOccupiedTile != null && highestWaterTile != null) ? ((highestOccupiedTile.Layer <= highestWaterTile.Layer) ? highestWaterTile : highestOccupiedTile) : ((highestOccupiedTile == null) ? highestWaterTile : highestOccupiedTile));
				if (hexTile != null)
				{
					if (hexTile.IsWater)
					{
						m_availableWaterTiles.Add(hexTile);
					}
					else
					{
						m_availableLandTiles.Add(hexTile);
					}
				}
			}
		}
	}

	private IEnumerator CreateScenery()
	{
		if (m_sceneryDensity == 0f)
		{
			yield break;
		}
		Transform categoryTransform = HexMap.GetCategoryTransform(base.transform, "Scenery", true);
		int count = m_availableLandTiles.Count;
		for (int i = 0; i < count && m_availableLandTiles.Count > 0; i++)
		{
			if (m_sceneryDensity < GetNextRandomRange(0f, 1f))
			{
				continue;
			}
			HexTile hexTile = m_availableLandTiles[GetNextRandomRange(0, m_availableLandTiles.Count)];
			GameObject gameObject = Singleton<Game>.Instance.LevelManager.GetTileSet(hexTile.TileSet).GetRandomScenery(this);
			if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
			{
				gameObject = Singleton<Game>.Instance.Player.Character.GetReplacement(gameObject);
			}
			if (gameObject != null)
			{
				SpawnRulesBase component = gameObject.GetComponent<SpawnRules>();
				if (component != null && !component.CheckTile(hexTile))
				{
					continue;
				}
				GameObject gameObject2 = Utils.CreateFromPrefab(gameObject, gameObject.name);
				Transform categoryTransform2 = HexMap.GetCategoryTransform(categoryTransform, "Layer" + hexTile.Layer);
				gameObject2.transform.parent = categoryTransform2;
				gameObject2.transform.position = HexMap.HexWorldPosition(hexTile);
				Renderer component2 = gameObject2.GetComponent<Renderer>();
				if (component2 != null)
				{
					ReplaceMaterials(component2);
				}
				GameObject gameObject3 = null;
				if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
				{
					gameObject3 = Singleton<Game>.Instance.Player.Character.GetAdditions(gameObject);
				}
				if (gameObject3 != null)
				{
					GameObject gameObject4 = Utils.CreateFromPrefab(gameObject3, gameObject3.name);
					gameObject4.transform.parent = gameObject2.transform;
					gameObject4.transform.localPosition = gameObject3.transform.localPosition;
					gameObject4.transform.localRotation = gameObject3.transform.localRotation;
					gameObject4.transform.localScale = gameObject3.transform.localScale;
				}
			}
			m_availableLandTiles.Remove(hexTile);
		}
	}

	private IEnumerator CreateWaterScenery()
	{
		if (m_sceneryDensity == 0f)
		{
			yield break;
		}
		Transform categoryTransform = HexMap.GetCategoryTransform(base.transform, "WaterScenery", true);
		int count = m_availableWaterTiles.Count;
		for (int i = 0; i < count && m_availableWaterTiles.Count > 0; i++)
		{
			if (m_waterSceneryDensity < GetNextRandomRange(0f, 1f))
			{
				continue;
			}
			HexTile hexTile = m_availableWaterTiles[GetNextRandomRange(0, m_availableWaterTiles.Count)];
			GameObject gameObject = Singleton<Game>.Instance.LevelManager.GetTileSet(hexTile.TileSet).GetRandomWaterScenery(this);
			if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
			{
				gameObject = Singleton<Game>.Instance.Player.Character.GetReplacement(gameObject);
			}
			if (gameObject != null)
			{
				SpawnRulesBase component = gameObject.GetComponent<SpawnRulesBase>();
				if (component != null && !component.CheckTile(hexTile))
				{
					continue;
				}
				GameObject gameObject2 = Utils.CreateFromPrefab(gameObject, gameObject.name);
				Transform categoryTransform2 = HexMap.GetCategoryTransform(categoryTransform, "Layer" + hexTile.Layer);
				gameObject2.transform.parent = categoryTransform2;
				gameObject2.transform.position = HexMap.HexWorldPosition(hexTile);
				GameObject gameObject3 = null;
				if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
				{
					gameObject3 = Singleton<Game>.Instance.Player.Character.GetAdditions(gameObject);
				}
				if (gameObject3 != null)
				{
					GameObject gameObject4 = Utils.CreateFromPrefab(gameObject3, gameObject3.name);
					gameObject4.transform.parent = gameObject2.transform;
					gameObject4.transform.localPosition = gameObject3.transform.localPosition;
					gameObject4.transform.localRotation = gameObject3.transform.localRotation;
					gameObject4.transform.localScale = gameObject3.transform.localScale;
				}
			}
			m_availableWaterTiles.Remove(hexTile);
		}
	}

	private IEnumerator CreateOtherSpawnValues()
	{
		SpawnValues[] spawnValues = base.gameObject.GetComponents<SpawnValues>();
		Utils.ShuffleList(m_availableLandTiles, GetNextRandomRange(0, int.MaxValue));
		Utils.ShuffleList(m_availableWaterTiles, GetNextRandomRange(0, int.MaxValue));
		int createCount = 0;
		SpawnValues[] array = spawnValues;
		foreach (SpawnValues spawn in array)
		{
			if (spawn.GroupName == "SetPieces")
			{
				continue;
			}
			List<HexTile> availableTiles = m_availableLandTiles;
			if (!spawn.IsLandBased)
			{
				availableTiles = m_availableWaterTiles;
			}
			int count = availableTiles.Count;
			if (spawn.SpawnCounts.Count > 0)
			{
				count = spawn.GetRandomCount(this);
			}
			for (int j = 0; j < count; j++)
			{
				if (!(spawn.Density < GetNextRandomRange(0f, 1f)))
				{
					Spawn(spawn.GetRandomPrefab(this), spawn.GroupName, spawn.IsLandBased, spawn.IsRandomRotated, spawn.RotateWithTile);
					createCount++;
					if (createCount > 16)
					{
						createCount = 0;
						yield return null;
					}
				}
			}
		}
	}

	public void Spawn(GameObject prefab, string groupName, bool onLand, bool randomRotate = true, bool rotateWithTile = false)
	{
		Material material = null;
		if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
		{
			prefab = Singleton<Game>.Instance.Player.Character.GetReplacement(prefab);
			material = Singleton<Game>.Instance.Player.Character.GetMaterialReplacement(prefab);
		}
		Transform categoryTransform = HexMap.GetCategoryTransform(base.transform, groupName);
		List<HexTile> list = null;
		list = ((!onLand) ? m_availableWaterTiles : m_availableLandTiles);
		if (list.Count <= 0)
		{
			return;
		}
		HexTile hexTile = list[GetNextRandomRange(0, list.Count)];
		SpawnRulesBase component = prefab.GetComponent<SpawnRulesBase>();
		if (component != null)
		{
			hexTile = component.PickRandomTile(list);
		}
		if (hexTile == null)
		{
			return;
		}
		if (prefab != null)
		{
			prefab = Singleton<Game>.Instance.Player.Character.GetReplacement(prefab);
			GameObject gameObject = Utils.CreateFromPrefab(prefab, prefab.name);
			gameObject.transform.parent = categoryTransform;
			gameObject.transform.position += HexMap.HexWorldPosition(hexTile);
			if (material != null)
			{
				SkinnedMeshRenderer[] componentsInChildren = gameObject.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
				{
					skinnedMeshRenderer.sharedMaterial = material;
				}
			}
			if (randomRotate)
			{
				gameObject.transform.rotation = Quaternion.Euler(0f, GetNextRandomRange(0f, 360f), 0f);
			}
			if (rotateWithTile)
			{
				gameObject.transform.rotation = Quaternion.Euler(0f, 0f - hexTile.Rotation, 0f);
			}
			GameObject gameObject2 = null;
			if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
			{
				gameObject2 = Singleton<Game>.Instance.Player.Character.GetAdditions(prefab);
			}
			if (gameObject2 != null)
			{
				GameObject gameObject3 = Utils.CreateFromPrefab(gameObject2, gameObject2.name);
				gameObject3.transform.parent = gameObject.transform;
				gameObject3.transform.localPosition = gameObject2.transform.localPosition;
				gameObject3.transform.localRotation = gameObject2.transform.localRotation;
				gameObject3.transform.localScale = gameObject2.transform.localScale;
			}
		}
		list.Remove(hexTile);
		if (component != null)
		{
			component.SpawnPartners();
		}
	}

	private IEnumerator CreateClouds()
	{
		LevelClouds levelClouds = base.gameObject.GetComponent<LevelClouds>();
		if (levelClouds == null || levelClouds.CloudRings == null || levelClouds.CloudRings.Length == 0)
		{
			yield break;
		}
		Transform categoryTransform = HexMap.GetCategoryTransform(base.transform, "Clouds");
		List<HexTile> outterTiles = HexMap.GetOutterTiles();
		Vector3 mapCenter = Vector3.zero;
		for (int i = 0; i < outterTiles.Count; i++)
		{
			mapCenter += HexMap.HexWorldPosition(outterTiles[i], 0);
		}
		mapCenter /= (float)outterTiles.Count;
		int createCount = 0;
		LevelClouds.CloudRing[] cloudRings = levelClouds.CloudRings;
		foreach (LevelClouds.CloudRing ring in cloudRings)
		{
			Utils.ShuffleList(outterTiles, GetNextRandomRange(0, int.MaxValue));
			List<float> prevAngles = new List<float>();
			foreach (HexTile tile in outterTiles)
			{
				ring.m_distance.Randomize();
				ring.m_height.Randomize();
				ring.m_angleSpacing.Randomize();
				Vector3 pos = HexMap.HexWorldPosition(tile, 0);
				Vector3 dir = pos - mapCenter;
				pos += dir.normalized * ring.m_distance.Value;
				pos.y = ring.m_height.Value;
				Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
				float yAngle = rot.eulerAngles.y;
				bool tooClose = false;
				if (yAngle > 90f && yAngle < 270f && ring.m_removeFront)
				{
					tooClose = true;
				}
				foreach (float item in prevAngles)
				{
					float num = item;
					if (Mathf.Abs(yAngle - num) < ring.m_angleSpacing.Value)
					{
						tooClose = true;
						break;
					}
				}
				Cloud prefab = ring.m_clouds[GetNextRandomRange(0, ring.m_clouds.Length)];
				if (prefab == null)
				{
					continue;
				}
				rot *= Quaternion.AngleAxis(90f, Vector3.up);
				Vector3 bounds = prefab.m_bounds;
				Transform checkTransform = prefab.transform;
				bounds.x *= checkTransform.localScale.x;
				bounds.y *= checkTransform.localScale.y;
				bounds.z *= checkTransform.localScale.z;
				Rect cloudRect = new Rect(pos.x - bounds.x * 0.5f, pos.z - bounds.z * 0.5f, bounds.x, bounds.z);
				HexMap hexMap = Singleton<Game>.Instance.LevelManager.CurrentLevel.HexMap;
				foreach (HexTile item2 in outterTiles)
				{
					if (item2.Occupied)
					{
						Vector3 vector = hexMap.HexWorldPosition(item2);
						Rect other = new Rect(vector.x - 5f, vector.z - 4.33f, 10f, 8.66f);
						if (cloudRect.Overlaps(other))
						{
							tooClose = true;
							break;
						}
					}
				}
				if (!tooClose)
				{
					Cloud cloud = Utils.CreateFromPrefab(prefab, prefab.name);
					cloud.transform.position = pos;
					cloud.transform.rotation = rot;
					cloud.transform.parent = categoryTransform;
					createCount++;
					if (createCount > 10)
					{
						createCount = 0;
						yield return null;
					}
					prevAngles.Add(yAngle);
				}
			}
		}
	}

	public static void ReplaceMaterials(Renderer rend)
	{
		Material obj = rend.sharedMaterial;
		if (Singleton<Game>.Instance.Player.Character.GetHexMaterialReplacement(ref obj))
		{
			rend.sharedMaterial = obj;
		}
	}

	public IEnumerator MergeMeshes()
	{
		yield return StartCoroutine(MergeTiles());
		yield return StartCoroutine(MergeWalls());
		yield return StartCoroutine(MergeWater());
		yield return StartCoroutine(MergeFences());
	}

	public IEnumerator MergeTiles()
	{
		proMeshCombineUtility meshCombine = Singleton<Game>.Instance.LevelManager.GetComponent<proMeshCombineUtility>();
		if (meshCombine == null)
		{
			yield break;
		}
		Transform geometry = HexMap.GetCategoryTransform(HexMap.transform, "Geometry");
		Transform tiles = HexMap.GetCategoryTransform(geometry, "Tiles");
		List<GameObject> meshes = new List<GameObject>();
		PhysicMaterial pMaterial = null;
		Dictionary<Material, string> m_tags = new Dictionary<Material, string>();
		HexGrid[] hexGrids = HexMap.HexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			Transform categoryTransform = HexMap.GetCategoryTransform(tiles, "Layer" + hexGrid.Layer);
			meshes.Add(categoryTransform.gameObject);
			Collider componentInChildren = categoryTransform.gameObject.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				pMaterial = componentInChildren.material;
			}
			for (int j = 0; j < categoryTransform.childCount; j++)
			{
				Renderer component = categoryTransform.GetChild(j).gameObject.GetComponent<Renderer>();
				if (component != null && !m_tags.ContainsKey(component.sharedMaterial))
				{
					m_tags.Add(component.sharedMaterial, component.gameObject.tag);
				}
			}
		}
		yield return StartCoroutine(meshCombine.Batch(meshes, true, true, false, meshCombine.mapOptimizationLevel[meshCombine.currentOptimizationLevelIndex]));
		HexGrid[] hexGrids2 = HexMap.HexGrids;
		foreach (HexGrid hexGrid2 in hexGrids2)
		{
			Transform categoryTransform2 = HexMap.GetCategoryTransform(tiles, "Layer" + hexGrid2.Layer);
			for (int l = 0; l < categoryTransform2.childCount; l++)
			{
				categoryTransform2.GetChild(l).gameObject.layer = LayerMask.NameToLayer("Ground");
				categoryTransform2.GetChild(l).gameObject.GetComponent<Collider>().sharedMaterial = pMaterial;
				Renderer component2 = categoryTransform2.GetChild(l).gameObject.GetComponent<Renderer>();
				if (component2 != null)
				{
					if (m_tags.ContainsKey(component2.sharedMaterial))
					{
						categoryTransform2.GetChild(l).gameObject.tag = m_tags[component2.sharedMaterial];
					}
					else
					{
						categoryTransform2.GetChild(l).gameObject.tag = "Ground";
					}
					ReplaceMaterials(component2);
				}
			}
		}
	}

	public IEnumerator MergeWalls()
	{
		proMeshCombineUtility meshCombine = Singleton<Game>.Instance.LevelManager.GetComponent<proMeshCombineUtility>();
		if (meshCombine == null)
		{
			yield break;
		}
		Transform geometry = HexMap.GetCategoryTransform(HexMap.transform, "Geometry");
		Transform walls = HexMap.GetCategoryTransform(geometry, "Walls");
		List<GameObject> meshes = new List<GameObject>();
		Transform wallFX = HexMap.GetCategoryTransform(geometry, "WallFX", true);
		HexGrid[] hexGrids = HexMap.HexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			Transform categoryTransform = HexMap.GetCategoryTransform(walls, "Layer" + hexGrid.Layer);
			for (int j = 0; j < categoryTransform.childCount; j++)
			{
				if (categoryTransform.GetChild(j).childCount > 0)
				{
					for (int k = 0; k < categoryTransform.GetChild(j).childCount; k++)
					{
						categoryTransform.GetChild(j).GetChild(k).parent = wallFX;
					}
				}
			}
		}
		HexGrid[] hexGrids2 = HexMap.HexGrids;
		foreach (HexGrid hexGrid2 in hexGrids2)
		{
			Transform categoryTransform2 = HexMap.GetCategoryTransform(walls, "Layer" + hexGrid2.Layer);
			meshes.Add(categoryTransform2.gameObject);
		}
		yield return StartCoroutine(meshCombine.BatchCollision(meshes, false, meshCombine.mapOptimizationLevel[meshCombine.currentOptimizationLevelIndex]));
		Transform wallCollision = HexMap.GetCategoryTransform(geometry, "WallCollision", true);
		HexGrid[] hexGrids3 = HexMap.HexGrids;
		foreach (HexGrid hexGrid3 in hexGrids3)
		{
			Transform categoryTransform3 = HexMap.GetCategoryTransform(walls, "Layer" + hexGrid3.Layer);
			List<Transform> list = new List<Transform>();
			for (int n = 0; n < categoryTransform3.childCount; n++)
			{
				if (categoryTransform3.GetChild(n).name == "COMBINED_MESH_DATA")
				{
					list.Add(categoryTransform3.GetChild(n));
				}
			}
			foreach (Transform item in list)
			{
				item.parent = wallCollision;
				item.gameObject.layer = LayerMask.NameToLayer("Walls");
				item.gameObject.tag = "Walls";
			}
		}
		yield return StartCoroutine(meshCombine.Batch(meshes, false, true, false, meshCombine.mapOptimizationLevel[meshCombine.currentOptimizationLevelIndex]));
		HexGrid[] hexGrids4 = HexMap.HexGrids;
		foreach (HexGrid hexGrid4 in hexGrids4)
		{
			Transform categoryTransform4 = HexMap.GetCategoryTransform(walls, "Layer" + hexGrid4.Layer);
			for (int num2 = 0; num2 < categoryTransform4.childCount; num2++)
			{
				categoryTransform4.GetChild(num2).gameObject.layer = LayerMask.NameToLayer("Walls");
				categoryTransform4.GetChild(num2).gameObject.tag = "Walls";
				Renderer component = categoryTransform4.GetChild(num2).gameObject.GetComponent<Renderer>();
				if (component != null)
				{
					ReplaceMaterials(component);
				}
			}
		}
	}

	public IEnumerator MergeWater()
	{
		proMeshCombineUtility meshCombine = Singleton<Game>.Instance.LevelManager.GetComponent<proMeshCombineUtility>();
		if (meshCombine == null)
		{
			yield break;
		}
		Transform geometry = HexMap.GetCategoryTransform(HexMap.transform, "Geometry");
		Transform water = HexMap.GetCategoryTransform(geometry, "Water");
		List<GameObject> meshes = new List<GameObject>();
		PhysicMaterial pMaterial = null;
		Dictionary<Material, string> m_tags = new Dictionary<Material, string>();
		HexGrid[] hexGrids = HexMap.HexGrids;
		foreach (HexGrid hexGrid in hexGrids)
		{
			Transform categoryTransform = HexMap.GetCategoryTransform(water, "Layer" + hexGrid.Layer);
			meshes.Add(categoryTransform.gameObject);
			Collider componentInChildren = categoryTransform.gameObject.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				pMaterial = componentInChildren.material;
			}
			for (int j = 0; j < categoryTransform.childCount; j++)
			{
				Renderer component = categoryTransform.GetChild(j).gameObject.GetComponent<Renderer>();
				if (component != null && !m_tags.ContainsKey(component.sharedMaterial))
				{
					m_tags.Add(component.sharedMaterial, component.gameObject.tag);
				}
			}
		}
		yield return StartCoroutine(meshCombine.Batch(meshes, true, true, false, meshCombine.mapOptimizationLevel[meshCombine.currentOptimizationLevelIndex]));
		HexGrid[] hexGrids2 = HexMap.HexGrids;
		foreach (HexGrid hexGrid2 in hexGrids2)
		{
			Transform categoryTransform2 = HexMap.GetCategoryTransform(water, "Layer" + hexGrid2.Layer);
			for (int l = 0; l < categoryTransform2.childCount; l++)
			{
				categoryTransform2.GetChild(l).gameObject.layer = LayerMask.NameToLayer("Water");
				categoryTransform2.GetChild(l).gameObject.GetComponent<Collider>().sharedMaterial = pMaterial;
				Renderer component2 = categoryTransform2.GetChild(l).gameObject.GetComponent<Renderer>();
				if (component2 != null)
				{
					if (m_tags.ContainsKey(component2.sharedMaterial))
					{
						categoryTransform2.GetChild(l).gameObject.tag = m_tags[component2.sharedMaterial];
					}
					else
					{
						categoryTransform2.GetChild(l).gameObject.tag = "Water";
					}
					ReplaceMaterials(component2);
				}
			}
		}
	}

	public IEnumerator MergeFences()
	{
		proMeshCombineUtility meshCombine = Singleton<Game>.Instance.LevelManager.GetComponent<proMeshCombineUtility>();
		if (meshCombine == null)
		{
			yield break;
		}
		Transform fences = HexMap.GetCategoryTransform(base.transform, "Fences");
		List<GameObject> meshes = new List<GameObject> { fences.gameObject };
		yield return StartCoroutine(meshCombine.BatchCollision(meshes, false, meshCombine.mapOptimizationLevel[meshCombine.currentOptimizationLevelIndex]));
		Transform wallCollision = HexMap.GetCategoryTransform(base.transform, "FencesCollision", true);
		List<Transform> merged = new List<Transform>();
		for (int i = 0; i < fences.childCount; i++)
		{
			if (fences.GetChild(i).name == "COMBINED_MESH_DATA")
			{
				merged.Add(fences.GetChild(i));
			}
		}
		foreach (Transform item in merged)
		{
			item.parent = wallCollision;
			item.gameObject.layer = LayerMask.NameToLayer("Walls");
			item.gameObject.tag = "Walls";
		}
		yield return StartCoroutine(meshCombine.Batch(meshes, false, true, false, meshCombine.mapOptimizationLevel[meshCombine.currentOptimizationLevelIndex]));
		for (int j = 0; j < fences.childCount; j++)
		{
			fences.GetChild(j).gameObject.layer = LayerMask.NameToLayer("Walls");
			fences.GetChild(j).gameObject.tag = "Walls";
		}
	}

	private void Update()
	{
	}

	private void OnDrawGizmosSelected()
	{
		if (!IsComplete)
		{
			return;
		}
		if (m_debugSelectedList.Count > 0)
		{
			Color[] array = new Color[7]
			{
				Color.red,
				Color.blue,
				Color.white,
				Color.magenta,
				Color.cyan,
				Color.green,
				Color.yellow
			};
			int num = 0;
			foreach (List<HexTile> debugSelected in m_debugSelectedList)
			{
				Gizmos.color = array[(int)Mathf.Repeat(num, array.Length)];
				foreach (HexTile item in debugSelected)
				{
					Vector3 vector = HexMap.HexWorldPosition(item);
					Vector3 vector2 = new Vector3(HexMap.TileWidth / 4f, 0f, 0f);
					Vector3 vector3 = new Vector3(HexMap.TileWidth / 2f, 0f, 0f);
					Vector3 vector4 = new Vector3(0f, 0f, HexMap.TileHeight / 2f);
					Gizmos.DrawLine(vector - vector2 - vector4, vector + vector2 - vector4);
					Gizmos.DrawLine(vector + vector2 - vector4, vector + vector3);
					Gizmos.DrawLine(vector + vector2 + vector4, vector + vector3);
					Gizmos.DrawLine(vector - vector2 - vector4, vector - vector3);
					Gizmos.DrawLine(vector - vector2 + vector4, vector - vector3);
					Gizmos.DrawLine(vector - vector2 + vector4, vector + vector2 + vector4);
				}
				num++;
			}
		}
		if (m_debugOptions.showGridLayer0 && HexMap.HexGrids[0] != null)
		{
			HexTile[,] tiles = HexMap.HexGrids[0].Tiles;
			int length = tiles.GetLength(0);
			int length2 = tiles.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					HexTile tile = tiles[i, j];
					GizmoDrawGridTile(tile);
				}
			}
		}
		if (m_debugOptions.showGridLayer1 && HexMap.HexGrids[1] != null)
		{
			HexTile[,] tiles2 = HexMap.HexGrids[1].Tiles;
			int length3 = tiles2.GetLength(0);
			int length4 = tiles2.GetLength(1);
			for (int k = 0; k < length3; k++)
			{
				for (int l = 0; l < length4; l++)
				{
					HexTile tile2 = tiles2[k, l];
					GizmoDrawGridTile(tile2);
				}
			}
		}
		if (!m_debugOptions.showGridLayer2 || HexMap.HexGrids[2] == null)
		{
			return;
		}
		HexTile[,] tiles3 = HexMap.HexGrids[2].Tiles;
		int length5 = tiles3.GetLength(0);
		int length6 = tiles3.GetLength(1);
		for (int m = 0; m < length5; m++)
		{
			for (int n = 0; n < length6; n++)
			{
				HexTile tile3 = tiles3[m, n];
				GizmoDrawGridTile(tile3);
			}
		}
	}

	private void GizmoDrawGridTile(HexTile tile)
	{
		Vector3 vector = HexMap.HexWorldPosition(tile);
		Vector3 vector2 = new Vector3(HexMap.TileWidth / 4f, 0f, 0f);
		Vector3 vector3 = new Vector3(HexMap.TileWidth / 2f, 0f, 0f);
		Vector3 vector4 = new Vector3(0f, 0f, HexMap.TileHeight / 2f);
		Gizmos.color = Color.black;
		Gizmos.DrawLine(vector - vector2 - vector4, vector + vector2 - vector4);
		Gizmos.DrawLine(vector + vector2 - vector4, vector + vector3);
		Gizmos.DrawLine(vector + vector2 + vector4, vector + vector3);
		Gizmos.DrawLine(vector - vector2 - vector4, vector - vector3);
		Gizmos.DrawLine(vector - vector2 + vector4, vector - vector3);
		Gizmos.DrawLine(vector - vector2 + vector4, vector + vector2 + vector4);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(vector, vector + vector4);
	}

	private void DebugUpdate()
	{
		Debug.Log("Debug Update");
		for (int i = 0; i < HexMap.HexGrids.Length; i++)
		{
			bool active = true;
			switch (i)
			{
			case 0:
				active = m_debugOptions.layer0Visible;
				break;
			case 1:
				active = m_debugOptions.layer1Visible;
				break;
			case 2:
				active = m_debugOptions.layer2Visible;
				break;
			}
			Transform categoryTransform = HexMap.GetCategoryTransform(HexMap.transform, "Geometry");
			Transform categoryTransform2 = HexMap.GetCategoryTransform(categoryTransform, "Tiles");
			Transform categoryTransform3 = HexMap.GetCategoryTransform(categoryTransform2, "Layer" + i);
			Transform categoryTransform4 = HexMap.GetCategoryTransform(categoryTransform, "Walls");
			Transform categoryTransform5 = HexMap.GetCategoryTransform(categoryTransform4, "Layer" + i);
			Transform categoryTransform6 = HexMap.GetCategoryTransform(categoryTransform, "Water");
			Transform categoryTransform7 = HexMap.GetCategoryTransform(categoryTransform6, "Layer" + i);
			Transform categoryTransform8 = HexMap.GetCategoryTransform(base.transform, "SetPiece");
			Transform categoryTransform9 = HexMap.GetCategoryTransform(categoryTransform8, "Layer" + i);
			Transform categoryTransform10 = HexMap.GetCategoryTransform(base.transform, "Scenery");
			Transform categoryTransform11 = HexMap.GetCategoryTransform(categoryTransform10, "Layer" + i);
			Transform categoryTransform12 = HexMap.GetCategoryTransform(base.transform, "WaterScenery");
			Transform categoryTransform13 = HexMap.GetCategoryTransform(categoryTransform12, "Layer" + i);
			categoryTransform3.gameObject.SetActive(active);
			categoryTransform5.gameObject.SetActive(active);
			categoryTransform7.gameObject.SetActive(active);
			categoryTransform9.gameObject.SetActive(active);
			categoryTransform11.gameObject.SetActive(active);
			categoryTransform13.gameObject.SetActive(active);
		}
	}

	public void DebugReloadSeed()
	{
		LoadLevel();
	}

	public void DebugNextSeed()
	{
		m_debugOptions.m_seedIdx++;
		if (m_debugOptions.m_seedIdx >= m_debugOptions.m_usedSeeds.Count)
		{
			seed = Mathf.FloorToInt(Time.realtimeSinceStartup * 100f);
			m_debugOptions.m_usedSeeds.Add(seed);
		}
		else
		{
			seed = m_debugOptions.m_usedSeeds[m_debugOptions.m_seedIdx];
		}
		LoadLevel();
	}

	public void DebugPrevSeed()
	{
		m_debugOptions.m_seedIdx--;
		m_debugOptions.m_seedIdx = Mathf.Max(m_debugOptions.m_seedIdx, 0);
		if (m_debugOptions.m_seedIdx >= m_debugOptions.m_usedSeeds.Count)
		{
			seed = 0;
		}
		else
		{
			seed = m_debugOptions.m_usedSeeds[m_debugOptions.m_seedIdx];
		}
		LoadLevel();
	}
}
