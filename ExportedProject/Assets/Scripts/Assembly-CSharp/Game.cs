using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

public class Game : Singleton<Game>
{
	public delegate void AspectRatioChanged();

	[SerializeField]
	private Player m_playerPrefab;

	private Player m_player;

	[SerializeField]
	private LevelManager m_levelManagerPrefab;

	private LevelManager m_levelManager;

	[SerializeField]
	private GameCamera m_visualCameraPrefab;

	private GameCamera m_visualCamera;

	[SerializeField]
	private GameObject m_rayCastCameraPrefab;

	public GameObject m_rayCastCamera;

	[SerializeField]
	private GameObject m_lightPrefab;

	private GameObject m_light;

	[SerializeField]
	private EffectManager m_effectManagerPrefab;

	private EffectManager m_effectManager;

	[SerializeField]
	private GameObject m_cursorPrefab;

	private GameObject m_cursor;

	[SerializeField]
	private UIController m_uiControllerPrefab;

	private UIController m_uiController;

	[SerializeField]
	private DevGiftManager m_devGiftManagerPrefab;

	private DevGiftManager m_devGiftManager;

	public Font m_chineseSimplifiedFont;

	[SerializeField]
	private InventoryManager m_inventoryManagerPrefab;

	private InventoryManager m_inventoryManager;

	[SerializeField]
	private StatsManager m_statsManagerPrefab;

	private StatsManager m_statsManager;

	[SerializeField]
	private MissionManager m_missionManagerPrefab;

	private MissionManager m_missionManager;

	[SerializeField]
	private GameObject m_defaultPickup;

	[SerializeField]
	private GameObject m_debuggerPrefab;

	private GameObject m_debugger;

	[SerializeField]
	private bool m_isShippingBuild;

	[SerializeField]
	private EventManager m_eventManagerPrefab;

	private EventManager m_eventManager;

	[SerializeField]
	private SocialManager m_socialManagerPrefab;

	private SocialManager m_socialManager;

	[SerializeField]
	private ShareManager m_shareManagerPrefab;

	private ShareManager m_shareManager;

	[SerializeField]
	private AdManager m_adManagerPrefab;

	private AdManager m_adManager;

	[SerializeField]
	private PerkManager m_perkManagerPrefab;

	private PerkManager m_perkManager;

	[SerializeField]
	private SpecialEventManager m_specialEventManagerPrefab;

	private SpecialEventManager m_specialEventManager;

	[SerializeField]
	private SpawnPickups.SpawnOptions m_defaultSpawnPickupsOptions;

	[SerializeField]
	public Font m_font;

	[SerializeField]
	private FastPoolManager m_fastPoolManager;

	[SerializeField]
	private string m_storeURL;

	[SerializeField]
	private string m_storeURLAndroid;

	private bool m_isLoaded;

	private bool m_loading;

	private float m_aspectRatio;

	private ScreenOrientation m_orientation;

	private Color m_lightDefaultColor;

	private float m_lightDefaultIntensity;

	private int m_leftAppCount;

	private static readonly string glyphs = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!$?-+.,";

	public bool IsLoaded
	{
		get
		{
			return m_isLoaded;
		}
	}

	public Player Player
	{
		get
		{
			return m_player;
		}
	}

	public LevelManager LevelManager
	{
		get
		{
			return m_levelManager;
		}
	}

	public Camera RayCastCamera
	{
		get
		{
			return m_rayCastCamera.GetComponent<Camera>();
		}
	}

	public GameCamera VisualCamera
	{
		get
		{
			return m_visualCamera;
		}
	}

	public bool IsShippingBuild
	{
		get
		{
			return m_isShippingBuild;
		}
	}

	public bool IsTouchDown
	{
		get
		{
			return m_visualCamera.Target.IsTouchDown;
		}
	}

	public Vector3 TouchDownPos
	{
		get
		{
			return m_visualCamera.Target.TouchDownPos;
		}
	}

	public Vector3 TouchDownCurrentPos
	{
		get
		{
			return m_visualCamera.Target.TouchDownCurrentPos;
		}
	}

	public EffectManager EffectManager
	{
		get
		{
			return m_effectManager;
		}
	}

	public UIController UIController
	{
		get
		{
			return m_uiController;
		}
	}

	public InventoryManager InventoryManager
	{
		get
		{
			return m_inventoryManager;
		}
	}

	public StatsManager StatsManager
	{
		get
		{
			return m_statsManager;
		}
	}

	public MissionManager MissionManager
	{
		get
		{
			return m_missionManager;
		}
	}

	public Light Light
	{
		get
		{
			if (m_light == null)
			{
				return null;
			}
			return m_light.GetComponent<Light>();
		}
	}

	public GameObject DefaultPickup
	{
		get
		{
			return m_defaultPickup;
		}
	}

	public SpawnPickups.SpawnOptions DefaultSpawnPickupsOptions
	{
		get
		{
			return m_defaultSpawnPickupsOptions;
		}
	}

	public EventManager EventManager
	{
		get
		{
			return m_eventManager;
		}
	}

	public SocialManager SocialManager
	{
		get
		{
			return m_socialManager;
		}
	}

	public ShareManager ShareManager
	{
		get
		{
			return m_shareManager;
		}
	}

	public DevGiftManager DevGiftManager
	{
		get
		{
			return m_devGiftManager;
		}
	}

	public AdManager AdManager
	{
		get
		{
			return m_adManager;
		}
	}

	public PerkManager PerkManager
	{
		get
		{
			return m_perkManager;
		}
	}

	public SpecialEventManager SpecialEventManager
	{
		get
		{
			return m_specialEventManager;
		}
	}

	public Color LightDefaultColor
	{
		get
		{
			return m_lightDefaultColor;
		}
	}

	public float LightDefaultIntensity
	{
		get
		{
			return m_lightDefaultIntensity;
		}
	}

	public bool IsSFXEnabled
	{
		get
		{
			return Singleton<Game>.Instance.Player.IsSFXEnabled;
		}
		set
		{
			Singleton<Game>.Instance.Player.IsSFXEnabled = value;
		}
	}

	public bool IsMusicEnabled
	{
		get
		{
			return Singleton<Game>.Instance.Player.IsMusicEnabled;
		}
		set
		{
			Singleton<Game>.Instance.Player.IsMusicEnabled = value;
		}
	}

	public bool IsPerformanceModeEnabled
	{
		get
		{
			return Singleton<Game>.Instance.Player.IsPerformanceModeEnabled;
		}
		set
		{
			Singleton<Game>.Instance.Player.IsPerformanceModeEnabled = value;
		}
	}

	public string LocalisationLanguage
	{
		get
		{
			return Singleton<Game>.Instance.Player.LocalisationLanguage;
		}
		set
		{
			Singleton<Game>.Instance.Player.LocalisationLanguage = value;
		}
	}

	public bool IsInverted
	{
		get
		{
			return Singleton<Game>.Instance.Player.IsInverted;
		}
		set
		{
			Singleton<Game>.Instance.Player.IsInverted = value;
		}
	}

	public string StoreURL
	{
		get
		{
			return m_storeURLAndroid;
		}
	}

	public event AspectRatioChanged AspectRatioChangedEvent;

	private void Awake()
	{
		Application.targetFrameRate = 60;
		m_isLoaded = false;
		m_loading = false;
		m_aspectRatio = (float)Screen.width / (float)Screen.height;
		m_orientation = Screen.orientation;
	}

	public IEnumerator PrecacheFontGlyphs(Font theFont, int fontSize, FontStyle style, string glyphs)
	{
		for (int index = 0; index < glyphs.Length; index++)
		{
			theFont.RequestCharactersInTexture(glyphs[index].ToString(), fontSize, style);
			yield return null;
		}
	}

	public IEnumerator LoadScene()
	{
		Screen.fullScreen = true;
		if (Application.isEditor)
		{
			Profiler.maxNumberOfSamplesPerFrame = 32768;
		}
		m_loading = true;
		m_statsManager = Utils.CreateFromPrefab(m_statsManagerPrefab, string.Empty);
		yield return null;
		m_missionManager = Utils.CreateFromPrefab(m_missionManagerPrefab, string.Empty);
		yield return null;
		m_levelManager = Utils.CreateFromPrefab(m_levelManagerPrefab, string.Empty);
		yield return null;
		m_fastPoolManager = Utils.CreateFromPrefab(m_fastPoolManager, m_fastPoolManager.name);
		yield return null;
		m_effectManager = Utils.CreateFromPrefab(m_effectManagerPrefab, string.Empty);
		yield return null;
		m_light = Utils.CreateFromPrefab(m_lightPrefab, "Light");
		m_lightDefaultColor = Light.color;
		m_lightDefaultIntensity = Light.intensity;
		yield return null;
		m_player = Utils.CreateFromPrefab(m_playerPrefab, string.Empty);
		yield return null;
		m_inventoryManager = Utils.CreateFromPrefab(m_inventoryManagerPrefab, string.Empty);
		yield return null;
		m_devGiftManager = Utils.CreateFromPrefab(m_devGiftManagerPrefab, string.Empty);
		m_visualCamera = Utils.CreateFromPrefab(m_visualCameraPrefab, "VisualCamera");
		m_visualCamera.transform.parent = m_player.transform;
		m_rayCastCamera = Utils.CreateFromPrefab(m_rayCastCameraPrefab, "RayCastCamera");
		m_rayCastCamera.transform.parent = m_player.transform;
		m_rayCastCamera.transform.localPosition = m_visualCamera.transform.localPosition;
		m_rayCastCamera.transform.localRotation = m_visualCamera.transform.localRotation;
		yield return null;
		m_statsManager.Init();
		m_missionManager.Init();
		m_cursor = Utils.CreateFromPrefab(m_cursorPrefab, "Cursor");
		m_uiController = Utils.CreateFromPrefab(m_uiControllerPrefab, "UI");
		if (m_debuggerPrefab != null && !m_isShippingBuild)
		{
			m_debugger = Utils.CreateFromPrefab(m_debuggerPrefab, "Debugger");
			yield return null;
		}
		m_eventManager = Utils.CreateFromPrefab(m_eventManagerPrefab, "EventManager");
		yield return null;
		m_socialManager = Utils.CreateFromPrefab(m_socialManagerPrefab, "SocialManager");
		yield return null;
		m_shareManager = Utils.CreateFromPrefab(m_shareManagerPrefab, m_shareManagerPrefab.name);
		yield return null;
		m_adManager = Utils.CreateFromPrefab(m_adManagerPrefab, "AdManager");
		yield return null;
		m_perkManager = Utils.CreateFromPrefab(m_perkManagerPrefab, "PerkManager");
		yield return null;
		m_specialEventManager = Utils.CreateFromPrefab(m_specialEventManagerPrefab, "SpecialEventManager");
		yield return null;
		SaveManager.RegisterSaveType<bool>(new SaveTypeBool());
		SaveManager.RegisterSaveType<Vector2>(new SaveTypeVector2());
		SaveManager.RegisterSaveType<Vector3>(new SaveTypeVector3());
		SaveManager.RegisterSaveType<Color>(new SaveTypeColor());
		SaveManager.RegisterSaveType<Rect>(new SaveTypeRect());
		SaveManager.RegisterSaveType<JSONObject>(new SaveTypeJSON());
		SaveManager.RegisterSavable(m_inventoryManager);
		SaveManager.RegisterSavable(m_statsManager);
		SaveManager.RegisterSavable(m_player);
		SaveManager.RegisterSavable(Singleton<NotificationManager>.Instance);
		SaveManager.RegisterSavable(m_eventManager);
		SaveManager.RegisterSavable(m_missionManager);
		SaveManager.RegisterSavable(m_socialManager);
		SaveManager.RegisterSavable(m_adManager);
		SaveManager.RegisterSavable(RateItNowSaveData.Instance);
		SaveManager.RegisterSavable(CharacterUnlockCaseSaveData.Instance);
		SaveManager.RegisterSavable(m_devGiftManager);
		SaveManager.RegisterSavable(m_specialEventManager);
		yield return StartCoroutine(SaveManager.Load());
		SaveManager.InitCloud();
		yield return StartCoroutine(m_uiController.Load());
		LevelManager.Init();
		while (!LevelManager.IsLoaded)
		{
			yield return null;
		}
		m_isLoaded = true;
		Shader.SetGlobalFloat("_FogInversion", 1f);
	}

	private void Update()
	{
		float num = (float)Screen.width / (float)Screen.height;
		if (m_aspectRatio != num)
		{
			m_aspectRatio = num;
			if (this.AspectRatioChangedEvent != null)
			{
				this.AspectRatioChangedEvent();
			}
		}
		if (m_orientation != Screen.orientation)
		{
			m_orientation = Screen.orientation;
			if (this.AspectRatioChangedEvent != null)
			{
				this.AspectRatioChangedEvent();
			}
		}
		if (Light != null)
		{
			Color color = LightDefaultColor;
			float intensity = LightDefaultIntensity;
			ChangeLighting.GetHighestPriority(out color, out intensity);
			if (Light.color != color || Light.intensity != intensity)
			{
				Light.color = color;
				Light.intensity = intensity;
			}
		}
	}

	public void OpenSpecialURL(string specialUrl, string backupURL)
	{
		StartCoroutine(OpenSpecialURLInternal(specialUrl, backupURL));
	}

	private void OnApplicationPause()
	{
		if (UIHud.m_suppressPause > 0)
		{
			UIHud.m_suppressPause--;
		}
		else if (Singleton<Game>.Instance.UIController != null && Singleton<Game>.Instance.IsLoaded)
		{
			Singleton<Game>.Instance.UIController.ShowPause();
		}
		m_leftAppCount++;
	}

	private void OnApplicationQuit()
	{
		SaveManager.Save();
	}

	private IEnumerator OpenSpecialURLInternal(string specialUrl, string backupURL)
	{
		int oldLeftAppCount = m_leftAppCount;
		Application.OpenURL(specialUrl);
		yield return new WaitForSeconds(1f);
		if (m_leftAppCount <= oldLeftAppCount)
		{
			Application.OpenURL(backupURL);
		}
	}
}
