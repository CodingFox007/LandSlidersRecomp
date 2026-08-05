using System;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using HeavyDutyInspector;
using UnityEngine;

public class LevelManager : MonoBehaviour, ISerializationCallbackReceiver
{
	public delegate void LevelLoadComplete();

	public delegate void StartNewLife();

	[SerializeField]
	private Level m_testLevel;

	[SerializeField]
	private Level[] m_levels;

	[SerializeField]
	private Level m_startLevel;

	[SerializeField]
	private Level m_retryLevel;

	[SerializeField]
	private Level m_tutorialLevel;

	[Dictionary("m_tilesetsValues")]
	public List<LevelTileSetGroup> m_tilesetGroups;

	[HideInInspector]
	public List<float> m_tilesetsValues;

	private Dictionary<LevelTileSetGroup, float> m_tilesets;

	[SerializeField]
	private RandomRange m_tilesetHoldCount;

	private LevelTileSetGroup m_currentTilesetGroup;

	private int m_nextTilesetDue;

	[SerializeField]
	private LevelTileSetGroup m_blankTilesetGroup;

	private Level m_currentLevel;

	private Level m_currentLevelPrefab;

	private int m_currentLevelIdx;

	private int m_prevLevelIdx;

	private bool m_isLoaded;

	public Material[] m_wakeMaterials;

	public AnimationCurve m_waterWakeAnimation;

	public float m_waterWakeAnimationSpeed;

	public Level CurrentLevel
	{
		get
		{
			return m_currentLevel;
		}
	}

	public Level CurrentLevelPrefab
	{
		get
		{
			return m_currentLevelPrefab;
		}
	}

	public Level[] Levels
	{
		get
		{
			return m_levels;
		}
	}

	public bool IsStartLevel
	{
		get
		{
			return m_currentLevelPrefab == m_startLevel || IsTutorialLevel;
		}
	}

	public bool IsRetryLevel
	{
		get
		{
			return m_currentLevelPrefab == m_retryLevel;
		}
	}

	public bool IsTutorialLevel
	{
		get
		{
			return m_currentLevelPrefab == m_tutorialLevel;
		}
	}

	public bool IsNormalLevel
	{
		get
		{
			return !IsStartLevel && !IsRetryLevel && !IsTutorialLevel;
		}
	}

	public int CurrentLevelIndex
	{
		get
		{
			return m_currentLevelIdx;
		}
	}

	public int PreviousLevelIndex
	{
		get
		{
			return m_prevLevelIdx;
		}
	}

	public LevelTileSetGroup TileSetGroup
	{
		get
		{
			return m_currentTilesetGroup;
		}
	}

	public LevelTileSetGroup[] TileSetGroups
	{
		get
		{
			LevelTileSetGroup[] array = new LevelTileSetGroup[m_tilesets.Keys.Count];
			m_tilesets.Keys.CopyTo(array, 0);
			return array;
		}
	}

	public int TileSetCount
	{
		get
		{
			return TileSetGroup.TileSetCount;
		}
	}

	public bool IsLoaded
	{
		get
		{
			return m_isLoaded;
		}
	}

	public event LevelLoadComplete OnLevelLoadComplete;

	public event LevelLoadComplete OnStartLevelLoadComplete;

	public event LevelLoadComplete OnRetryLevelLoadComplete;

	public event StartNewLife OnStartNewLife;

	public LevelTileSet GetTileSet(int idx)
	{
		return TileSetGroup.GetTileSet(idx);
	}

	public float GetTileSetChance(int idx)
	{
		return TileSetGroup.GetTileSetChance(idx);
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_tilesetGroups, m_tilesetsValues, out m_tilesets, "tilesets");
	}

	private void Start()
	{
	}

	public void Init()
	{
		if (Singleton<Game>.Instance.StatsManager.GetStat("TutorialLevel").Total.Current <= 0f)
		{
			LoadTutorialLevel();
		}
		else
		{
			LoadStartLevel();
		}
	}

	private void Update()
	{
		float num = Time.time * m_waterWakeAnimationSpeed;
		num -= (float)(int)num;
		num = m_waterWakeAnimation.Evaluate(num);
		Material[] wakeMaterials = m_wakeMaterials;
		foreach (Material material in wakeMaterials)
		{
			material.SetTextureOffset("_MainTex", new Vector2(0f, num));
		}
	}

	public void LoadTutorialLevel()
	{
		m_isLoaded = false;
		PickNewTilesetGroup(0);
		m_currentTilesetGroup = m_blankTilesetGroup;
		if (m_currentLevel != null)
		{
			UnityEngine.Object.Destroy(m_currentLevel.gameObject);
		}
		m_currentLevelPrefab = m_tutorialLevel;
		m_currentLevel = Utils.CreateFromPrefab(m_currentLevelPrefab, m_currentLevelPrefab.name);
		m_currentLevel.transform.parent = base.transform;
		m_currentLevel.OnLoadComplete += LoadComplete;
		m_currentLevel.seed = GetRandomSeed();
		m_currentLevel.LoadLevel();
		m_currentLevelIdx = -1;
	}

	public void LoadStartLevel()
	{
		m_isLoaded = false;
		PickNewTilesetGroup(0);
		m_currentTilesetGroup = m_blankTilesetGroup;
		if (m_currentLevel != null)
		{
			UnityEngine.Object.Destroy(m_currentLevel.gameObject);
		}
		m_currentLevelPrefab = m_startLevel;
		m_currentLevel = Utils.CreateFromPrefab(m_currentLevelPrefab, m_currentLevelPrefab.name);
		m_currentLevel.transform.parent = base.transform;
		m_currentLevel.OnLoadComplete += LoadComplete;
		m_currentLevel.seed = GetRandomSeed();
		m_currentLevel.LoadLevel();
		m_currentLevelIdx = -1;
	}

	public void LoadRetryLevel()
	{
		m_isLoaded = false;
		PickNewTilesetGroup(0);
		m_currentTilesetGroup = m_blankTilesetGroup;
		SaveManager.Save();
		if (m_currentLevel != null)
		{
			UnityEngine.Object.Destroy(m_currentLevel.gameObject);
		}
		m_currentLevelPrefab = m_retryLevel;
		m_currentLevel = Utils.CreateFromPrefab(m_currentLevelPrefab, m_currentLevelPrefab.name);
		m_currentLevel.transform.parent = base.transform;
		m_currentLevel.OnLoadComplete += LoadComplete;
		m_currentLevel.seed = GetRandomSeed();
		m_currentLevel.LoadLevel();
		m_currentLevelIdx = -1;
	}

	public void LoadNextLevel(Level levelPrefab = null, int levelIdx = -1, int mapIdx = -1, int seed = -1, LevelTileSetGroup tileSetGroup = null)
	{
		m_isLoaded = false;
		if (m_currentLevel != null)
		{
			UnityEngine.Object.Destroy(m_currentLevel.gameObject);
		}
		m_prevLevelIdx = m_currentLevelIdx;
		if (levelIdx == -1)
		{
			m_currentLevelIdx++;
		}
		else
		{
			m_currentLevelIdx = levelIdx;
		}
		if (m_currentLevelIdx == 0 || m_currentLevelIdx >= m_nextTilesetDue)
		{
			PickNewTilesetGroup(m_currentLevelIdx);
		}
		if (tileSetGroup != null)
		{
			m_currentTilesetGroup = tileSetGroup;
		}
		int num = Mathf.Min(m_currentLevelIdx, m_levels.Length - 1);
		if (levelPrefab == null)
		{
			m_currentLevelPrefab = m_levels[num];
		}
		else
		{
			m_currentLevelPrefab = levelPrefab;
		}
		if (m_currentLevelPrefab.OverrideTileSetGroup != null)
		{
			m_currentTilesetGroup = m_currentLevelPrefab.OverrideTileSetGroup;
			m_nextTilesetDue = m_currentLevelIdx + 1;
		}
		if (seed == -1)
		{
			seed = GetRandomSeed();
		}
		m_currentLevel = Utils.CreateFromPrefab(m_currentLevelPrefab, m_currentLevelPrefab.name);
		m_currentLevel.transform.parent = base.transform;
		m_currentLevel.OnLoadComplete += LoadComplete;
		m_currentLevel.seed = seed;
		string mapPath = null;
		if (mapIdx >= 0 && mapIdx < m_currentLevel.Maps.Length)
		{
			mapPath = m_currentLevel.Maps[mapIdx].m_path;
		}
		m_currentLevel.LoadLevel(mapPath);
		if (!IsTutorialLevel)
		{
			Singleton<Game>.Instance.StatsManager.GetStat("TutorialLevel").IncreaseStat(1f);
		}
	}

	public void Restart()
	{
		m_prevLevelIdx = m_currentLevelIdx;
		m_currentLevelIdx = -1;
		LoadRetryLevel();
	}

	private void LoadComplete()
	{
		if (IsStartLevel)
		{
			SpawnTile spawnTile = UnityEngine.Object.FindObjectOfType<SpawnTile>();
			if (spawnTile != null)
			{
				bool flag = Singleton<Game>.Instance.StatsManager.HasStat("IsRandom") && Singleton<Game>.Instance.StatsManager.GetStat("IsRandom").Total.Current == 1f;
				if (flag)
				{
					Singleton<Game>.Instance.Player.PickRandomCharacter();
					Singleton<Game>.Instance.Player.IsRandomCharacter = flag;
				}
				Singleton<Game>.Instance.Player.RespawnPlayer(spawnTile.transform.position);
			}
			MasterAudio.TriggerPlaylistClip("Main");
			if (this.OnStartLevelLoadComplete != null)
			{
				this.OnStartLevelLoadComplete();
			}
		}
		else if (IsRetryLevel)
		{
			SpawnTile spawnTile2 = UnityEngine.Object.FindObjectOfType<SpawnTile>();
			if (spawnTile2 != null)
			{
			}
			if (this.OnRetryLevelLoadComplete != null)
			{
				this.OnRetryLevelLoadComplete();
			}
			MasterAudio.TriggerPlaylistClip("Heaven");
			if (Singleton<Game>.Instance.AdManager.IsInterstitialReady())
			{
				Singleton<Game>.Instance.AdManager.ShowInterstitial();
			}
		}
		else
		{
			if (m_currentLevelIdx == 0 && this.OnStartNewLife != null)
			{
				this.OnStartNewLife();
			}
			if (this.OnLevelLoadComplete != null)
			{
				this.OnLevelLoadComplete();
			}
		}
		m_isLoaded = true;
	}

	public void PickNewTilesetGroup(int currentIdx)
	{
		m_currentTilesetGroup = GetRandomTileset();
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
		{
			m_currentTilesetGroup = Singleton<Game>.Instance.Player.Character.OverrideTileset(m_currentTilesetGroup);
		}
		m_tilesetHoldCount.Randomize();
		m_nextTilesetDue = currentIdx + Mathf.FloorToInt(m_tilesetHoldCount.Value);
	}

	private LevelTileSetGroup GetRandomTileset()
	{
		if (m_tilesets.Count <= 0)
		{
			return null;
		}
		float num = 0f;
		foreach (float value in m_tilesets.Values)
		{
			float num2 = value;
			num += num2;
		}
		float num3 = UnityEngine.Random.Range(0f, num);
		num = 0f;
		foreach (KeyValuePair<LevelTileSetGroup, float> tileset in m_tilesets)
		{
			num += tileset.Value;
			if (num >= num3)
			{
				return tileset.Key;
			}
		}
		return null;
	}

	public object Parse()
	{
		object result = null;
		if (Utils.ParseTokenCount() < 2)
		{
			return result;
		}
		if (Utils.ParseTokenHash(1) == "CurrentIndex".GetHashCode())
		{
			result = (float)CurrentLevelIndex;
		}
		else if (Utils.ParseTokenHash(1) == "PreviousIndex".GetHashCode())
		{
			result = (float)PreviousLevelIndex;
		}
		else if (Utils.ParseTokenHash(1) == "CurrentNumber".GetHashCode())
		{
			result = (float)CurrentLevelIndex + 1f;
		}
		else if (Utils.ParseTokenHash(1) == "PreviousNumber".GetHashCode())
		{
			result = (float)PreviousLevelIndex + 1f;
		}
		else if (Utils.ParseTokenHash(1) == "IsRetryLevel".GetHashCode())
		{
			result = IsRetryLevel;
		}
		else if (Utils.ParseTokenHash(1) == "IsStartLevel".GetHashCode())
		{
			result = IsStartLevel;
		}
		else if (Utils.ParseTokenHash(1) == "IsNormalLevel".GetHashCode())
		{
			result = IsNormalLevel;
		}
		return result;
	}

	private int GetRandomSeed()
	{
		return Mathf.FloorToInt(DateTime.Now.Millisecond);
	}
}
