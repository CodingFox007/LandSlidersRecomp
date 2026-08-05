using System;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using HeavyDutyInspector;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityStandardAssets.Utility;

public class Player : MonoBehaviour, Savable, ISerializationCallbackReceiver
{
	[Serializable]
	public class XPTier
	{
		public int m_target;
	}

	public delegate void CharacterChange();

	public delegate void PlayerStartLevel();

	public delegate void PlayerFinishLevel();

	public delegate void LanguageChanged();

	private enum State
	{
		Idle = 0,
		Die = 1,
		ChangeCamera = 2,
		Transition = 3,
		Revive = 4,
		Wipe = 5,
		LoadLevel = 6,
		Raise = 7
	}

	[SerializeField]
	private string m_defaultCharacterId;

	private Character m_character;

	private string m_characterId = string.Empty;

	private CharacterInfo m_characterInfo;

	private Character m_prefab;

	private CharacterInfo m_lastPlayedCharacter;

	[SerializeField]
	private SurfaceEffect[] m_surfaceEffects;

	[SerializeField]
	private SurfaceEffect[] m_surfaceDeathEffects;

	[SerializeField]
	private Animator m_dangerWarningPrefab;

	private Animator m_dangerWarning;

	[SerializeField]
	private Vector3 m_dangerWarningOffset;

	[SerializeField]
	[ReorderableList(true)]
	private List<CharacterInfo> m_characters;

	private List<CharacterInfo> m_unlockableCharacters;

	[SerializeField]
	private CharacterInfo m_randomCharacter;

	[SerializeField]
	private List<Powerup> m_powerups;

	private Powerup m_currentPowerup;

	private int m_currentPowerupIdx = -1;

	[SerializeField]
	private Collider m_pickupMagnet;

	[SerializeField]
	private Material m_goldMaterial;

	[SerializeField]
	private Material m_freezeMaterial;

	[SerializeField]
	private int m_minReviveLevel = 2;

	[SerializeField]
	private int m_maxReviveLevel = 7;

	[SerializeField]
	private int m_minReviveGames = 3;

	[SoundGroup]
	[SerializeField]
	private string m_movingSFX;

	public GameObject m_freezeObject;

	[SerializeField]
	private AnimationCurve m_movingSFXVolume;

	[SerializeField]
	private float m_movingVolumeSpeedMin = 10f;

	[SerializeField]
	private float m_movingVolumeSpeedMax = 30f;

	[SerializeField]
	private float m_movingVolumeSmooth = 0.2f;

	private SoundGroupVariation m_movingSoundVariation;

	private bool m_isBeingKnocked;

	private int m_knockCounter;

	private Vector3 m_knockbackForce = Vector3.zero;

	private float m_invincibleWait;

	private bool m_isCinematicMove;

	[SerializeField]
	private XPTier[] m_xpTiers;

	private int m_currentXPTierIdx;

	private float m_previousXPCurrent;

	private float m_previousXPTarget;

	private GameObject m_killedBy;

	private float m_wallCollisionTimer;

	private int m_wallCollisionCounter;

	private float m_freezeTimer;

	private float m_postFreezeTimer;

	private bool m_godMode;

	[SerializeField]
	private Material m_whiteMaterial;

	private float m_postReviveBlink;

	[SerializeField]
	private float m_reviveBlinkInitial = 4f;

	[SerializeField]
	private float m_reviveBlinkInitialOn = 0.6f;

	[SerializeField]
	private float m_reviveBlinkInitialOff = 0.15f;

	[SerializeField]
	private float m_reviveBlinkFast = 1f;

	[SerializeField]
	private float m_reviveBlinkFastOn = 0.1f;

	[SerializeField]
	private float m_reviveBlinkFastOff = 0.1f;

	private bool m_dead;

	private bool m_invincible;

	private float m_invincibleGracePeriod;

	private bool m_hasFreezePower;

	private float m_hasFreezePowerGracePeriod;

	private bool m_inputBlocked;

	private bool m_respawning;

	private bool m_betweenLevels;

	private bool m_justPickedRandomCharacter;

	[SerializeField]
	public UnityEvent m_onFreeze = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onUnfreeze = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onReviveStart = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onReviveEnd = new UnityEvent();

	[SerializeField]
	private float m_freezeTime = 2f;

	private bool m_isMutedFromReplayKit;

	private bool m_isMutedFromAdShowing;

	private bool m_isVolumeDownFromScreenCaptureWithMicrophone;

	private bool m_onGround;

	private float m_oldOnGroundHeight;

	private string m_groundSurfaceType = string.Empty;

	private bool m_sfxEnabled = true;

	private bool m_musicEnabled = true;

	private bool m_systemMusicEnabled;

	private bool m_performanceEnabled;

	private bool m_reduceMotionEnabled;

	private string m_language = string.Empty;

	private bool m_inverted;

	private bool m_isRandomCharacter;

	private Rigidbody m_rigidBody;

	private float m_magnetScale = 1f;

	[SerializeField]
	private float m_cinematicMoveTopSpeed = 20f;

	[SerializeField]
	private float m_cinematicMoveAcceleration = 1f;

	private float m_cinematicVelocity;

	[SerializeField]
	private float m_reviveHeight = 15f;

	public Vector3 m_defaultTransitionOffset = new Vector3(0f, 3f, 0f);

	private float m_waitHack;

	private string m_currentCustomShaderKeyword;

	private SurfaceEffect m_currentEffect;

	public bool ForceKill;

	[SerializeField]
	private GameObject m_transitionCameraPrefab;

	private GameObject m_transitionCamera;

	[SerializeField]
	private float m_fovTarget = 10f;

	[SerializeField]
	private LeanTweenType m_fovTweenIn = LeanTweenType.easeOutBack;

	[SerializeField]
	private float m_fovTweenTimeIn = 0.5f;

	[SerializeField]
	private float m_fovTweenDelayIn;

	private float m_timeScaleLerpTimer;

	private bool m_timeLerping;

	private RenderTexture m_renderTexture;

	[Dictionary("m_deathsValues")]
	public List<PlayerDeath> m_deathChances;

	[HideInInspector]
	public List<float> m_deathsValues;

	private Dictionary<PlayerDeath, float> m_deaths;

	private PlayerDeath m_currentDeath;

	private string m_testDeathName = string.Empty;

	private StateMachine<State> m_deathState = new StateMachine<State>();

	private float m_deathStateTimer;

	private bool m_showPlayer = true;

	public bool IsCinematicMove
	{
		get
		{
			return m_isCinematicMove || m_waitHack > 0f;
		}
		set
		{
			m_isCinematicMove = value;
		}
	}

	public bool GodMode
	{
		get
		{
			return m_godMode;
		}
		set
		{
			m_godMode = value;
		}
	}

	public Character Character
	{
		get
		{
			return m_character;
		}
	}

	public CharacterInfo LastPlayedCharacter
	{
		get
		{
			if (m_lastPlayedCharacter == null)
			{
				return CharacterInfo;
			}
			return m_lastPlayedCharacter;
		}
	}

	public bool IsBeingKnocked
	{
		get
		{
			return m_isBeingKnocked;
		}
		set
		{
			if (value)
			{
				m_knockCounter = 30;
			}
			m_isBeingKnocked = value;
		}
	}

	public float InvincibleGracePeriod
	{
		set
		{
			m_invincibleGracePeriod = value;
		}
	}

	public float HasFreezePowerGracePeriod
	{
		set
		{
			m_hasFreezePowerGracePeriod = value;
		}
	}

	public CharacterInfo CharacterInfo
	{
		get
		{
			return m_characterInfo;
		}
	}

	public string CharacterId
	{
		get
		{
			return m_characterId;
		}
	}

	public string DefaultCharacterId
	{
		get
		{
			return m_defaultCharacterId;
		}
	}

	public List<CharacterInfo> AvailableCharacters
	{
		get
		{
			return m_characters;
		}
	}

	public List<CharacterInfo> UnlockableCharacters
	{
		get
		{
			return m_unlockableCharacters;
		}
	}

	public bool IsCollidingWall
	{
		get
		{
			return m_wallCollisionCounter > 0 && m_wallCollisionTimer > 0f;
		}
	}

	public bool UseGravity
	{
		get
		{
			return m_rigidBody.useGravity;
		}
		set
		{
			m_rigidBody.useGravity = value;
		}
	}

	public Rigidbody Rigidbody
	{
		get
		{
			return m_rigidBody;
		}
	}

	public bool IsDead
	{
		get
		{
			return m_dead;
		}
	}

	public bool BlockInput
	{
		get
		{
			return m_inputBlocked;
		}
		set
		{
			m_inputBlocked = value;
		}
	}

	public bool Invincible
	{
		get
		{
			return m_invincible || m_invincibleGracePeriod > 0f;
		}
		set
		{
			m_invincible = value;
		}
	}

	public bool HasFeezePower
	{
		get
		{
			return m_hasFreezePower || m_hasFreezePowerGracePeriod > 0f;
		}
		set
		{
			m_hasFreezePower = value;
		}
	}

	public Powerup PowerupActive
	{
		get
		{
			return m_currentPowerup;
		}
	}

	public string PowerupActiveId
	{
		get
		{
			if (m_currentPowerup == null)
			{
				return string.Empty;
			}
			return m_currentPowerup.Id;
		}
	}

	public int PowerupActiveIdx
	{
		get
		{
			if (m_currentPowerup == null)
			{
				return -1;
			}
			return m_currentPowerupIdx;
		}
	}

	public GameObject KilledBy
	{
		get
		{
			return m_killedBy;
		}
	}

	public bool IsSFXEnabled
	{
		get
		{
			return m_sfxEnabled;
		}
		set
		{
			m_sfxEnabled = value;
			UpdateToDesiredSFXVolume();
		}
	}

	public bool IsMutedFromReplayKit
	{
		get
		{
			return m_isMutedFromReplayKit;
		}
		set
		{
			if (m_isMutedFromReplayKit != value)
			{
				m_isMutedFromReplayKit = value;
				UpdateToDesiredMusicVolume();
				UpdateToDesiredSFXVolume();
			}
		}
	}

	public bool IsMutedFromAdShowing
	{
		get
		{
			return m_isMutedFromAdShowing;
		}
		set
		{
			if (m_isMutedFromAdShowing != value)
			{
				m_isMutedFromAdShowing = value;
				UpdateToDesiredMusicVolume();
				UpdateToDesiredSFXVolume();
			}
		}
	}

	public bool IsMusicEnabled
	{
		get
		{
			return m_musicEnabled;
		}
		set
		{
			m_musicEnabled = value;
			UpdateToDesiredMusicVolume();
		}
	}

	public bool IsSystemMusicEnabled
	{
		get
		{
			return m_systemMusicEnabled;
		}
		set
		{
			m_systemMusicEnabled = value;
			UpdateToDesiredMusicVolume();
		}
	}

	public bool IsVolumeDownFromScreenCaptureWithMicrophone
	{
		get
		{
			return m_isVolumeDownFromScreenCaptureWithMicrophone;
		}
		set
		{
			if (m_isVolumeDownFromScreenCaptureWithMicrophone != value)
			{
				m_isVolumeDownFromScreenCaptureWithMicrophone = value;
				UpdateToDesiredMusicVolume();
				UpdateToDesiredSFXVolume();
			}
		}
	}

	private float ExpectedGlobalVolume
	{
		get
		{
			float result = 0f;
			if (!m_isMutedFromAdShowing && !m_isMutedFromReplayKit)
			{
				result = 1f;
				if (m_isVolumeDownFromScreenCaptureWithMicrophone)
				{
					result = 0.2f;
				}
			}
			return result;
		}
	}

	public bool IsBetweenLevels
	{
		get
		{
			return m_betweenLevels;
		}
	}

	public bool IsReduceMotionEnabled
	{
		get
		{
			return m_reduceMotionEnabled;
		}
		set
		{
			m_reduceMotionEnabled = value;
		}
	}

	public bool IsPerformanceModeEnabled
	{
		get
		{
			return m_performanceEnabled;
		}
		set
		{
			m_performanceEnabled = value;
			if (m_performanceEnabled)
			{
				QualitySettings.SetQualityLevel(1, true);
				Singleton<Game>.Instance.Light.shadows = LightShadows.None;
			}
			else
			{
				QualitySettings.SetQualityLevel(0, true);
				Singleton<Game>.Instance.Light.shadows = LightShadows.Hard;
			}
		}
	}

	public string LocalisationLanguage
	{
		get
		{
			return m_language;
		}
		set
		{
			m_language = value;
			if (m_language != string.Empty && LocalizationManager.HasLanguage(m_language))
			{
				LocalizationManager.CurrentLanguage = m_language;
			}
			else if (m_language != string.Empty)
			{
				Debug.LogError("Unable to find language: " + m_language);
			}
			if (this.OnLanguageChanged != null)
			{
				this.OnLanguageChanged();
			}
		}
	}

	public bool IsInverted
	{
		get
		{
			return m_inverted;
		}
		set
		{
			m_inverted = value;
		}
	}

	public XPTier[] XPTiers
	{
		get
		{
			return m_xpTiers;
		}
	}

	public XPTier CurrentXPTier
	{
		get
		{
			if (m_currentXPTierIdx >= XPTiers.Length)
			{
				return XPTiers[XPTiers.Length - 1];
			}
			return XPTiers[m_currentXPTierIdx];
		}
	}

	public int CurrentXPTierIdx
	{
		get
		{
			return m_currentXPTierIdx;
		}
		set
		{
			m_currentXPTierIdx = value;
		}
	}

	public float CurrentXP
	{
		get
		{
			XPTier currentXPTier = CurrentXPTier;
			if (currentXPTier == null)
			{
				return 0f;
			}
			object obj = Utils.Parse("Stats.collectible.Character" + m_characterId + ".Current");
			if (obj == null || obj.GetType() != typeof(float))
			{
				Debug.LogError("XP parsing error");
				return 0f;
			}
			int num = 0;
			for (int i = 0; i < m_currentXPTierIdx; i++)
			{
				num = ((i < XPTiers.Length) ? (num + XPTiers[i].m_target) : (num + XPTiers[XPTiers.Length - 1].m_target));
			}
			return (float)obj - (float)num;
		}
	}

	public float TargetXP
	{
		get
		{
			XPTier currentXPTier = CurrentXPTier;
			if (currentXPTier == null)
			{
				return 0f;
			}
			return currentXPTier.m_target;
		}
	}

	public int PreviousXP
	{
		get
		{
			return Mathf.FloorToInt(m_previousXPCurrent);
		}
	}

	public int PreviousXPTarget
	{
		get
		{
			return Mathf.FloorToInt(m_previousXPTarget);
		}
	}

	public bool IsFrozen
	{
		get
		{
			return m_freezeTimer > 0f;
		}
	}

	public float GetPostFreezeTimer
	{
		get
		{
			return m_postFreezeTimer;
		}
	}

	public float PickupMagnetScale
	{
		get
		{
			return m_magnetScale;
		}
		set
		{
			m_magnetScale = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Magnetic Range", value);
			m_pickupMagnet.gameObject.transform.localScale = new Vector3(m_magnetScale, m_magnetScale, m_magnetScale);
		}
	}

	public Material GoldMaterial
	{
		get
		{
			return m_goldMaterial;
		}
	}

	public Vector3 TransitionOffset
	{
		get
		{
			if (m_characterInfo != null && m_characterInfo.m_useCustomTransitionOffset)
			{
				return m_characterInfo.m_customOffsetTransitionOffset;
			}
			return m_defaultTransitionOffset;
		}
	}

	public bool IsRandomCharacter
	{
		get
		{
			return m_isRandomCharacter;
		}
		set
		{
			m_isRandomCharacter = value;
			Singleton<Game>.Instance.StatsManager.GetStat("IsRandom").SetValue(value ? 1 : 0);
		}
	}

	public string currentCustomShaderKeyword
	{
		get
		{
			return m_currentCustomShaderKeyword;
		}
		set
		{
			if (value != m_currentCustomShaderKeyword)
			{
				if (!string.IsNullOrEmpty(m_currentCustomShaderKeyword))
				{
					Shader.DisableKeyword(m_currentCustomShaderKeyword);
				}
				m_currentCustomShaderKeyword = value;
				if (!string.IsNullOrEmpty(m_currentCustomShaderKeyword))
				{
					Shader.EnableKeyword(m_currentCustomShaderKeyword);
				}
			}
		}
	}

	public bool IsWiping
	{
		get
		{
			return m_deathState.CurrentState == State.Wipe;
		}
	}

	public string TestDeathName
	{
		get
		{
			return m_testDeathName;
		}
		set
		{
			m_testDeathName = value;
		}
	}

	public int DeathTypeCount
	{
		get
		{
			return m_deaths.Count;
		}
	}

	public RenderTexture RenderTexture
	{
		get
		{
			return m_renderTexture;
		}
		set
		{
			m_renderTexture = value;
		}
	}

	public event CharacterChange OnCharacterChanged;

	public event CharacterChange OnPreCharacterChanged;

	public event PlayerStartLevel OnPlayerStartLevel;

	public event PlayerFinishLevel OnPlayerFinishLevel;

	public event LanguageChanged OnLanguageChanged;

	public int UnlockedCharacterCount()
	{
		int num = 0;
		foreach (CharacterInfo availableCharacter in AvailableCharacters)
		{
			if (!availableCharacter.Locked)
			{
				num++;
			}
		}
		return num;
	}

	private void UpdateToDesiredSFXVolume()
	{
		float masterVolumeLevel = 0f;
		if (m_sfxEnabled)
		{
			masterVolumeLevel = ExpectedGlobalVolume;
		}
		MasterAudio.MasterVolumeLevel = masterVolumeLevel;
	}

	private void UpdateToDesiredMusicVolume()
	{
		float playlistMasterVolume = 0f;
		if (m_musicEnabled && !m_systemMusicEnabled)
		{
			playlistMasterVolume = ExpectedGlobalVolume;
		}
		MasterAudio.PlaylistMasterVolume = playlistMasterVolume;
	}

	public void Freeze(Vector3 knockback)
	{
		if (m_freezeTimer <= 0f && !Invincible && !IsCinematicMove)
		{
			SetMaterials(m_freezeMaterial);
			Character.Freeze();
			m_freezeTimer = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Freeze Time", m_freezeTime);
			AddKnockbackForce(knockback);
			m_onFreeze.Invoke();
		}
	}

	public void UpdateXP()
	{
		m_previousXPCurrent = CurrentXP;
		m_previousXPTarget = TargetXP;
	}

	private void Start()
	{
		if (m_characterId == string.Empty)
		{
			m_characterId = m_defaultCharacterId;
			m_characterInfo = GetCharacterInfo(m_characterId);
		}
		int num = Mathf.Max(Screen.width, Screen.height);
		IsPerformanceModeEnabled = num <= 800;
		m_rigidBody = base.gameObject.GetComponent<Rigidbody>();
		m_dangerWarning = UnityEngine.Object.Instantiate(m_dangerWarningPrefab);
		m_dangerWarning.transform.parent = base.transform;
		m_dangerWarning.transform.localPosition = m_dangerWarningOffset;
		m_wallCollisionCounter = 0;
		m_wallCollisionTimer = 0f;
		m_dead = false;
		m_respawning = true;
		BuildUnlockableList();
		List<CharacterInfo> list = new List<CharacterInfo>();
		foreach (CharacterInfo character in m_characters)
		{
			CharacterInfo characterInfo = character.Clone() as CharacterInfo;
			characterInfo.IsGolden = true;
			list.Add(characterInfo);
		}
		m_characters.AddRange(list);
		SetupDeath();
		Shader.DisableKeyword("_MONOCHROME");
		Shader.DisableKeyword("_BLACK_HOLE");
		Shader.DisableKeyword("_GLITCH");
		Shader.DisableKeyword("_UNDERWATER");
		Shader.SetGlobalColor("_MonochromeBlack", Color.white);
		Shader.SetGlobalColor("_MonochromeWhite", Color.black);
		SaveManager.OnCloudChanged += CloudChanged;
		IsInverted = m_inverted;
	}

	public void WaitHack()
	{
		m_waitHack = 0.8f;
		m_character.gameObject.SetActive(false);
	}

	public void StopDeathNow()
	{
		if (m_currentDeath != null)
		{
			m_currentDeath.gameObject.SetActive(false);
		}
	}

	public void BuildUnlockableList()
	{
		m_unlockableCharacters = new List<CharacterInfo>();
		foreach (CharacterInfo character in m_characters)
		{
			if (character.Unlockable && !character.IsGolden)
			{
				m_unlockableCharacters.Add(character);
			}
		}
	}

	public void DoRevive()
	{
		Singleton<Game>.Instance.StatsManager.GetStat("Revived").IncreaseStat(1f);
		Singleton<Game>.Instance.UIController.HUD.SetState(UIHud.State.Normal);
		Singleton<Game>.Instance.UIController.GoToInGameState();
		m_deathState.CurrentState = State.Idle;
		Vector3 position = base.transform.position;
		position.y = Mathf.Max(position.y, 0f);
		RespawnPlayer(position);
		Singleton<Game>.Instance.VisualCamera.CanZoom = true;
		Singleton<Game>.Instance.Player.Rigidbody.velocity = new Vector3(0f, 0f, 0f);
		if (m_currentDeath != null)
		{
			UnityEngine.Object.Destroy(m_currentDeath.gameObject);
			m_currentDeath = null;
		}
		BlockInput = true;
		IsCinematicMove = true;
		m_cinematicVelocity = 0f;
		m_onReviveStart.Invoke();
	}

	public void RespawnPlayer(Vector3 pos)
	{
		m_rigidBody.velocity = Vector3.zero;
		m_dead = false;
		base.transform.position = pos;
		Singleton<Game>.Instance.VisualCamera.ResetTargets(pos);
		Singleton<Game>.Instance.VisualCamera.ConnectToPlayer();
		if (m_deathState.CurrentState == State.Idle)
		{
			Singleton<Game>.Instance.VisualCamera.transform.parent = base.transform;
			Singleton<Game>.Instance.VisualCamera.Reset();
		}
		if (Singleton<Game>.Instance.LevelManager.IsNormalLevel && Singleton<Game>.Instance.Player.RenderTexture != null)
		{
			Singleton<Game>.Instance.Player.RenderTexture.Release();
			UnityEngine.Object.Destroy(Singleton<Game>.Instance.Player.RenderTexture);
			Singleton<Game>.Instance.Player.RenderTexture = null;
		}
		m_dangerWarning.transform.parent = base.transform;
		m_dangerWarning.transform.localPosition = m_dangerWarningOffset;
		m_dangerWarning.SetBool("InDanger", false);
		m_respawning = true;
		BlockInput = false;
		Singleton<Game>.Instance.UIController.HUD.SetState(UIHud.State.Normal);
		m_killedBy = null;
		PickupMagnetScale = 1f;
		SetCharacter(m_characterId);
		Collider component = base.gameObject.GetComponent<Collider>();
		if (component != null)
		{
			component.enabled = true;
		}
		Collider[] componentsInChildren = m_character.GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			collider.enabled = true;
		}
	}

	public CharacterInfo GetCharacterInfo(string id)
	{
		if (id == m_randomCharacter.Id)
		{
			return m_randomCharacter;
		}
		foreach (CharacterInfo character in m_characters)
		{
			if (character.Id == id)
			{
				return character;
			}
		}
		return null;
	}

	public CharacterInfo GetCharacterInfoForRandomCharacter()
	{
		return m_randomCharacter;
	}

	public void SetCharacter(string id)
	{
		SetCharacter(GetCharacterInfo(id));
	}

	public void JustPickedRandomCharacter()
	{
		m_justPickedRandomCharacter = true;
	}

	public void PickRandomCharacter()
	{
		if (m_justPickedRandomCharacter)
		{
			m_justPickedRandomCharacter = false;
			return;
		}
		CharacterInfo characterInfo = null;
		int num = 0;
		foreach (CharacterInfo availableCharacter in AvailableCharacters)
		{
			if (!availableCharacter.Locked)
			{
				num++;
			}
		}
		do
		{
			characterInfo = m_characters[UnityEngine.Random.Range(0, m_characters.Count - 1)];
		}
		while (characterInfo.Locked || (num != 1 && !(characterInfo.Id != CharacterId)));
		SetCharacter(characterInfo);
	}

	public void SetCharacter(CharacterInfo character)
	{
		if (character == null)
		{
			m_characterId = m_defaultCharacterId;
			character = GetCharacterInfo(m_defaultCharacterId);
		}
		string text = ((!(m_character == null)) ? m_characterId : string.Empty);
		if (character.Id != text && this.OnPreCharacterChanged != null)
		{
			this.OnPreCharacterChanged();
		}
		m_characterId = character.Id;
		m_characterInfo = GetCharacterInfo(m_characterId);
		if (m_character != null)
		{
			UnityEngine.Object.DestroyImmediate(m_character.gameObject);
		}
		if (m_prefab != null)
		{
			m_prefab = null;
		}
		m_prefab = Resources.Load<Character>(character.ResourcePath);
		m_character = Utils.CreateFromPrefab(m_prefab, "Character");
		m_character.transform.parent = base.transform;
		m_character.transform.localPosition = Vector3.zero;
		m_character.transform.localScale = new Vector3(1f, 1f, 1f);
		m_character.Load();
		if (m_characterInfo.IsGolden)
		{
			m_character.SetMaterials(m_character.GoldMaterial);
		}
		m_character.SetupPhysics(m_rigidBody);
		FastPool pool = FastPoolManager.GetPool(m_characterInfo.ReplacementPickup.GetComponent<Pickup>().PickupEffect.m_particlePrefab.gameObject, false);
		foreach (GameObject item in pool.cache)
		{
			ParticleSystemPoolDestroyer component = item.GetComponent<ParticleSystemPoolDestroyer>();
			if (component != null)
			{
				component.m_prefab = m_characterInfo.ReplacementPickup.GetComponent<Pickup>().PickupEffect.m_particlePrefab.gameObject;
			}
		}
		if (m_characterId != text)
		{
			m_currentXPTierIdx = 0;
			while (!(CurrentXP < TargetXP))
			{
				m_currentXPTierIdx++;
			}
			UpdateXP();
		}
		if (m_characterId != text && this.OnCharacterChanged != null)
		{
			this.OnCharacterChanged();
		}
	}

	public void CloudChanged()
	{
		m_currentXPTierIdx = 0;
		while (!(CurrentXP < TargetXP))
		{
			m_currentXPTierIdx++;
		}
		UpdateXP();
	}

	public void StartLevel()
	{
		m_betweenLevels = false;
		BlockInput = false;
		Invincible = false;
		PickupMagnetScale = 1f;
		if (Singleton<Game>.Instance.LevelManager.IsRetryLevel)
		{
			PickupMagnetScale = 0.5f;
		}
		if (this.OnPlayerStartLevel != null)
		{
			this.OnPlayerStartLevel();
		}
		Singleton<Game>.Instance.StatsManager.GetStatTimer("PlayTime").IsActive = true;
		Singleton<Game>.Instance.StatsManager.GetStatTimer("AliveTime").IsActive = true;
		Singleton<Game>.Instance.MissionManager.CurrentLocation = Mission.Location.InGame;
		Singleton<Game>.Instance.SocialManager.CurrentLocation = Achievement.Location.InGame;
		if (Singleton<Game>.Instance.LevelManager.IsNormalLevel)
		{
			string text = "Level " + Singleton<Game>.Instance.LevelManager.CurrentLevelIndex;
			int num = Mathf.FloorToInt(Singleton<Game>.Instance.StatsManager.GetStat("collectible").Life.Current);
		}
		StartCoroutine(Utils.ClearUnusedAssets());
	}

	public void FinishLevel()
	{
		m_betweenLevels = true;
		BlockInput = true;
		if (!IsDead)
		{
			Invincible = true;
		}
		if (this.OnPlayerFinishLevel != null)
		{
			this.OnPlayerFinishLevel();
		}
		if (m_currentPowerup != null)
		{
			m_currentPowerup.Deactivate();
		}
		Singleton<Game>.Instance.StatsManager.GetStatTimer("PlayTime").IsActive = false;
		Singleton<Game>.Instance.StatsManager.GetStatTimer("AliveTime").IsActive = false;
		if (!IsDead && !Singleton<Game>.Instance.LevelManager.IsRetryLevel && !Singleton<Game>.Instance.LevelManager.IsStartLevel)
		{
			Singleton<Game>.Instance.StatsManager.GetStat("LevelsPassed").IncreaseStat(1f);
			Singleton<Game>.Instance.MissionManager.CurrentLocation = Mission.Location.LevelPass;
			Singleton<Game>.Instance.SocialManager.CurrentLocation = Achievement.Location.LevelPass;
		}
		if (Singleton<Game>.Instance.LevelManager.IsNormalLevel)
		{
			string text = "Level " + Singleton<Game>.Instance.LevelManager.CurrentLevelIndex;
			int num = Mathf.FloorToInt(Singleton<Game>.Instance.StatsManager.GetStat("collectible").Life.Current);
		}
	}

	private void Update()
	{
		if (m_waitHack > 0f)
		{
			m_waitHack = Mathf.Max(0f, m_waitHack - Time.deltaTime);
			if (m_waitHack == 0f)
			{
				DoRevive();
			}
			return;
		}
		if (m_postReviveBlink > 0f)
		{
			bool flag = false;
			if (m_postReviveBlink <= m_reviveBlinkFast)
			{
				float num = Mathf.Repeat(m_postReviveBlink, m_reviveBlinkFastOff + m_reviveBlinkFastOn);
				flag = num < m_reviveBlinkFastOff;
			}
			else
			{
				float num2 = Mathf.Repeat(m_postReviveBlink - m_reviveBlinkFast, m_reviveBlinkInitialOff + m_reviveBlinkInitialOn);
				flag = num2 < m_reviveBlinkInitialOff;
			}
			m_postReviveBlink = Mathf.Max(0f, m_postReviveBlink - Time.deltaTime);
			if (m_postReviveBlink == 0f)
			{
				flag = false;
			}
			if (flag)
			{
				SetMaterials(m_whiteMaterial);
			}
			else
			{
				Character.UnsetMaterials();
			}
		}
		if (m_wallCollisionTimer > 0f)
		{
			m_wallCollisionTimer -= Time.deltaTime;
		}
		else
		{
			m_wallCollisionCounter = 0;
		}
		if (m_freezeTimer > 0f)
		{
			m_postFreezeTimer = 0f;
			m_freezeTimer = Mathf.Max(0f, m_freezeTimer - Time.deltaTime);
			if (m_freezeTimer == 0f)
			{
				if (!m_dead)
				{
					Singleton<Game>.Instance.StatsManager.GetStat("Unfreeze").IncreaseStat(1f);
				}
				m_onUnfreeze.Invoke();
				m_character.Unfreeze();
			}
		}
		else
		{
			m_postFreezeTimer += Time.deltaTime;
		}
		if (m_freezeObject != null && Character != null)
		{
			m_freezeObject.transform.localRotation = Character.transform.localRotation;
		}
		if (IsBeingKnocked && m_onGround)
		{
			m_knockCounter--;
			if (m_knockCounter <= 0)
			{
				IsBeingKnocked = false;
			}
		}
		m_invincibleGracePeriod = Mathf.Max(0f, m_invincibleGracePeriod - Time.deltaTime);
		m_hasFreezePowerGracePeriod = Mathf.Max(0f, m_hasFreezePowerGracePeriod - Time.deltaTime);
		if (m_invincibleWait > 0f)
		{
			m_invincibleWait = Mathf.Max(0f, m_invincibleWait - Time.deltaTime);
			if (m_invincibleWait != 0f)
			{
			}
		}
		ReplayKit instance = ReplayKit.Instance;
		if (instance != null)
		{
			IsMutedFromReplayKit = instance.isSharing;
			IsVolumeDownFromScreenCaptureWithMicrophone = instance.isRecording && instance.wasMicrophoneOnWhenRecordingStarted;
		}
		if (!Singleton<Game>.Instance.IsLoaded)
		{
			return;
		}
		if (m_movingSoundVariation != null)
		{
			float num3 = 0f;
			Vector3 velocity = Rigidbody.velocity;
			velocity.y = 0f;
			float time = Mathf.InverseLerp(m_movingVolumeSpeedMin, m_movingVolumeSpeedMax, velocity.magnitude);
			num3 = m_movingSFXVolume.Evaluate(time);
			m_movingSoundVariation.FadeToVolume(num3, m_movingVolumeSmooth);
		}
		else
		{
			PlaySoundResult playSoundResult = MasterAudio.PlaySound(m_movingSFX, 0f);
			if (playSoundResult != null)
			{
				m_movingSoundVariation = playSoundResult.ActingVariation;
			}
			else
			{
				Debug.LogError("Unable to play sound: " + m_movingSFX);
			}
		}
		currentCustomShaderKeyword = ((!(m_character != null)) ? string.Empty : m_character.m_customShaderKeyword);
		if (!string.IsNullOrEmpty(currentCustomShaderKeyword))
		{
			Shader.SetGlobalFloat("_Timing", Time.time);
			Shader.SetGlobalVector("_PlayerPos", new Vector4(base.transform.position.x, base.transform.position.y, base.transform.position.z, 0f));
			Shader.SetGlobalColor("_MonochromeBlack", m_character.m_monochromeBlack);
			Shader.SetGlobalColor("_MonochromeWhite", m_character.m_monochromeWhite);
			Shader.SetGlobalFloat("_MonochromeScale", m_character.m_monochromeScale);
		}
		if (IsCinematicMove)
		{
			UseGravity = false;
			m_character.gameObject.SetActive(false);
			Collider component = base.gameObject.GetComponent<Collider>();
			if (component != null)
			{
				component.enabled = false;
			}
			Singleton<Game>.Instance.Player.Rigidbody.velocity = Vector3.zero;
			m_rigidBody.velocity = Vector3.zero;
			m_rigidBody.isKinematic = true;
			SpawnTile spawnTile = UnityEngine.Object.FindObjectOfType<SpawnTile>();
			if (spawnTile != null)
			{
				Vector3 position = spawnTile.transform.position;
				position.y += m_reviveHeight;
				float magnitude = (position - base.transform.position).magnitude;
				Vector3 normalized = (position - base.transform.position).normalized;
				m_cinematicVelocity = Mathf.Min(m_cinematicVelocity + Time.deltaTime * m_cinematicMoveAcceleration, m_cinematicMoveTopSpeed);
				if (magnitude <= Time.deltaTime * m_cinematicVelocity)
				{
					if (component != null)
					{
						component.enabled = true;
					}
					IsCinematicMove = false;
					m_postReviveBlink = m_reviveBlinkInitial + m_reviveBlinkFast;
					base.transform.position = position;
					BlockInput = false;
					UseGravity = true;
					m_rigidBody.isKinematic = false;
					m_character.gameObject.SetActive(true);
					Singleton<Game>.Instance.VisualCamera.Target.ResetAirWalkingToAir();
					m_invincibleWait = 0.5f;
					m_onReviveEnd.Invoke();
				}
				else
				{
					base.transform.position = base.transform.position + normalized * Time.deltaTime * m_cinematicVelocity;
				}
			}
		}
		if (Singleton<Game>.Instance.LevelManager.CurrentLevel.IsComplete && !(m_character == null))
		{
			UpdateSurfaceEffects();
			if (m_currentPowerup != null && !m_currentPowerup.IsActive)
			{
				DeactivatePowerup();
			}
			UpdateDeath();
			if (!IsDead && m_respawning)
			{
				m_respawning = false;
			}
		}
	}

	private void UpdateSurfaceEffects()
	{
		bool onGround = m_onGround;
		m_onGround = Singleton<Game>.Instance.VisualCamera.Target.IsOnGround;
		m_groundSurfaceType = Singleton<Game>.Instance.VisualCamera.Target.GroundSurfaceType;
		string groundSurfaceType = m_groundSurfaceType;
		if (Singleton<Game>.Instance.VisualCamera.Target.DistanceOffGroundActual > 1f)
		{
			m_onGround = false;
			m_groundSurfaceType = "Air";
		}
		else if (Singleton<Game>.Instance.VisualCamera.Target.DistanceOffGround < 0.5f)
		{
			m_onGround = true;
		}
		if (m_onGround)
		{
			if (!onGround && Mathf.Abs(m_oldOnGroundHeight - Singleton<Game>.Instance.VisualCamera.Target.GroundHeight) >= 4f && !BlockInput)
			{
				Singleton<Game>.Instance.VisualCamera.Shake("Fall Impact");
			}
			m_oldOnGroundHeight = Singleton<Game>.Instance.VisualCamera.Target.GroundHeight;
		}
		SurfaceEffect surfaceEffect = GetSurfaceEffect(m_groundSurfaceType);
		if (IsDead)
		{
			surfaceEffect = null;
		}
		if (!m_onGround && surfaceEffect != null && surfaceEffect.m_onlyWhenGrounded)
		{
			surfaceEffect = null;
		}
		if (surfaceEffect != m_currentEffect)
		{
			if (m_currentEffect != null)
			{
				Singleton<Game>.Instance.EffectManager.StopEffect(m_currentEffect);
				Singleton<Game>.Instance.StatsManager.GetStatTimer("PlayerOver" + m_currentEffect.m_surfaceTag).IsActive = false;
			}
			m_currentEffect = surfaceEffect;
			if (m_currentEffect != null)
			{
				Singleton<Game>.Instance.EffectManager.StartEffect(m_currentEffect, m_rigidBody, m_characterInfo.ParticleColor);
				Singleton<Game>.Instance.StatsManager.GetStatTimer("PlayerOver" + m_currentEffect.m_surfaceTag).IsActive = true;
			}
		}
		if (!(m_dangerWarning != null))
		{
			return;
		}
		bool value = false;
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.InGame || IsDead)
		{
			float distanceOffGround = Singleton<Game>.Instance.VisualCamera.Target.DistanceOffGround;
			value = groundSurfaceType == "Air" || groundSurfaceType.StartsWith("Water", StringComparison.Ordinal) || distanceOffGround > 1f;
			if (m_currentDeath != null && m_currentDeath.m_turnOffExclamation)
			{
				value = false;
			}
			if (IsCinematicMove)
			{
				value = false;
			}
		}
		m_dangerWarning.SetBool("InDanger", value);
	}

	private SurfaceEffect GetSurfaceEffect(string surfaceType)
	{
		SurfaceEffect[] surfaceEffects = m_characterInfo.m_surfaceEffects;
		foreach (SurfaceEffect surfaceEffect in surfaceEffects)
		{
			if (surfaceEffect.m_surfaceTag == surfaceType)
			{
				return surfaceEffect;
			}
		}
		SurfaceEffect[] surfaceEffects2 = m_surfaceEffects;
		foreach (SurfaceEffect surfaceEffect2 in surfaceEffects2)
		{
			if (surfaceEffect2.m_surfaceTag == surfaceType)
			{
				return surfaceEffect2;
			}
		}
		return null;
	}

	private SurfaceEffect GetSurfaceDeathEffect(string surfaceType)
	{
		SurfaceEffect[] surfaceDeathEffects = m_characterInfo.m_surfaceDeathEffects;
		foreach (SurfaceEffect surfaceEffect in surfaceDeathEffects)
		{
			if (surfaceEffect.m_surfaceTag == surfaceType)
			{
				return surfaceEffect;
			}
		}
		SurfaceEffect[] surfaceDeathEffects2 = m_surfaceDeathEffects;
		foreach (SurfaceEffect surfaceEffect2 in surfaceDeathEffects2)
		{
			if (surfaceEffect2.m_surfaceTag == surfaceType)
			{
				return surfaceEffect2;
			}
		}
		return null;
	}

	public void Kill(GameObject killer, bool forceKill = false)
	{
		if (m_dead || IsCinematicMove || (m_postReviveBlink > 0f && killer != null) || !Singleton<Game>.Instance.LevelManager.IsLoaded)
		{
			return;
		}
		ForceKill = forceKill;
		if (killer != null && (m_invincible || GodMode || HasFeezePower))
		{
			return;
		}
		m_dead = true;
		m_lastPlayedCharacter = m_characterInfo;
		m_dangerWarning.transform.parent = null;
		BlockInput = true;
		Singleton<Game>.Instance.UIController.HUD.SetState(UIHud.State.Results);
		Collider component = base.gameObject.GetComponent<Collider>();
		if (component != null)
		{
			component.enabled = false;
		}
		Collider[] componentsInChildren = m_character.GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			collider.enabled = false;
		}
		m_onGround = Singleton<Game>.Instance.VisualCamera.Target.IsOnGround;
		m_groundSurfaceType = Singleton<Game>.Instance.VisualCamera.Target.GroundSurfaceType;
		if (m_onGround && !m_groundSurfaceType.StartsWith("Ground"))
		{
			m_groundSurfaceType = "Ground";
		}
		SurfaceEffect surfaceDeathEffect = GetSurfaceDeathEffect(m_groundSurfaceType);
		if (surfaceDeathEffect != null && (!surfaceDeathEffect.m_onlyWhenGrounded || m_onGround))
		{
			Vector3 position = base.transform.position;
			if (m_groundSurfaceType.StartsWith("Water"))
			{
				int layerMask = 1 << LayerMask.NameToLayer("Water");
				RaycastHit hitInfo;
				if (Physics.Raycast(new Ray(position + new Vector3(0f, 10f, 0f), -base.transform.up.normalized), out hitInfo, 50f, layerMask))
				{
					position.y = hitInfo.point.y;
				}
			}
			Singleton<Game>.Instance.EffectManager.StartEffect(surfaceDeathEffect, position, m_characterInfo.ParticleColor);
		}
		Singleton<Game>.Instance.VisualCamera.DisconnectFromPlayer();
		Rigidbody.velocity = Vector3.zero;
		if (m_currentPowerup != null)
		{
			m_currentPowerup.Deactivate();
		}
		m_killedBy = killer;
		SetRandomDeath();
		Unfreeze();
	}

	private void SetMaterials(Material mat)
	{
		m_character.SetMaterials(mat);
	}

	public void ActivatePowerup(string id, bool silent = false)
	{
		if (PowerupActiveId == id)
		{
			return;
		}
		for (int i = 0; i < m_powerups.Count; i++)
		{
			if (m_powerups[i].Id == id)
			{
				ActivatePowerup(i, silent);
				break;
			}
		}
	}

	public void ActivatePowerup(int idx, bool silent = false)
	{
		if (idx < 0 || idx >= m_powerups.Count)
		{
			return;
		}
		Powerup powerup = null;
		powerup = m_powerups[idx];
		if (!(powerup == null))
		{
			if (m_currentPowerup != null)
			{
				UnityEngine.Object.Destroy(m_currentPowerup.gameObject);
				m_currentPowerup = null;
			}
			Unfreeze();
			m_currentPowerup = Utils.CreateFromPrefab(powerup, powerup.name);
			m_currentPowerup.transform.SetParent(base.transform, false);
			m_currentPowerup.IsSilent = silent;
			m_currentPowerup.IsActive = true;
			m_currentPowerupIdx = idx;
		}
	}

	public void DeactivatePowerup()
	{
		if (m_currentPowerup != null)
		{
			UnityEngine.Object.Destroy(m_currentPowerup.gameObject);
			m_currentPowerup = null;
		}
		m_currentPowerupIdx = -1;
	}

	public void Unfreeze()
	{
		if (m_freezeTimer > 0f)
		{
			m_freezeTimer = 0f;
			m_onUnfreeze.Invoke();
			m_character.Unfreeze();
		}
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		jSONObject.AddField("char", m_characterId);
		jSONObject.AddField("sfx", m_sfxEnabled);
		jSONObject.AddField("music", m_musicEnabled);
		jSONObject.AddField("performance", m_performanceEnabled);
		jSONObject.AddField("reducemotion", m_reduceMotionEnabled);
		jSONObject.AddField("language", m_language);
		jSONObject.AddField("inverted", m_inverted);
		return jSONObject;
	}

	private bool IsReduceMotionSetSystemWide()
	{
		return false;
	}

	public bool Verify(JSONObject data)
	{
		return true;
	}

	public void Load(JSONObject data)
	{
		m_characterId = data.GetField("char").AsString;
		if (data.HasField("sfx"))
		{
			IsSFXEnabled = data.GetField("sfx").AsBool;
		}
		if (data.HasField("music"))
		{
			IsMusicEnabled = data.GetField("music").AsBool;
		}
		if (data.HasField("performance"))
		{
			IsPerformanceModeEnabled = data.GetField("performance").AsBool;
		}
		if (data.HasField("language"))
		{
			LocalisationLanguage = data.GetField("language").AsString;
		}
		if (data.HasField("inverted"))
		{
			IsInverted = data.GetField("inverted").AsBool;
		}
		bool isReduceMotionEnabled = IsReduceMotionSetSystemWide();
		if (data.HasField("reduceMotion"))
		{
			isReduceMotionEnabled = data.GetField("reduceMotion").AsBool;
		}
		IsReduceMotionEnabled = isReduceMotionEnabled;
	}

	public void Merge(JSONObject dataA, JSONObject dataB)
	{
	}

	public JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB)
	{
		return dataA;
	}

	public JSONObject Reset()
	{
		m_characterId = m_defaultCharacterId;
		m_sfxEnabled = true;
		m_musicEnabled = true;
		m_performanceEnabled = false;
		m_reduceMotionEnabled = false;
		m_language = string.Empty;
		m_inverted = false;
		return Save();
	}

	public JSONObject UpdateVersion(JSONObject data)
	{
		JSONObject result = data;
		string version = GetVersion(data);
		if (!(version == string.Empty))
		{
			switch (version)
			{
			case "0.0.1":
			case "0.0.2":
			case "0.0.3":
				break;
			default:
				goto IL_0051;
			}
		}
		result = Reset();
		goto IL_0051;
		IL_0051:
		return result;
	}

	public string SaveId()
	{
		return "Player";
	}

	public string Version()
	{
		return "0.0.4";
	}

	public string GetVersion(JSONObject data)
	{
		if (!data.HasField("version"))
		{
			return string.Empty;
		}
		return data.GetField("version").AsString;
	}

	public bool UseCloud()
	{
		return false;
	}

	private void OnCollisionEnter(Collision collisionInfo)
	{
		if (collisionInfo.collider.tag == "Walls")
		{
			m_wallCollisionCounter++;
			m_wallCollisionTimer = 0.15f;
		}
		else if (collisionInfo.collider.tag == "Death")
		{
			Kill(collisionInfo.collider.gameObject);
		}
		else
		{
			if (!(collisionInfo.collider.tag == "Entity"))
			{
				return;
			}
			Entity entity = collisionInfo.collider.gameObject.GetComponent<Entity>();
			if (entity == null)
			{
				entity = collisionInfo.collider.gameObject.GetComponentInParent<Entity>();
			}
			if (!(entity != null))
			{
				return;
			}
			if (Invincible || entity.KillableByPlayer)
			{
				entity.Kill(base.gameObject);
				if (Invincible)
				{
					Singleton<Game>.Instance.StatsManager.GetStat("InvincibleKill").IncreaseStat(1f);
					Singleton<Game>.Instance.VisualCamera.Shake("Invincible Kill");
					MasterAudio.PlaySound("NP-EnemyDeath");
				}
			}
			Singleton<Game>.Instance.StatsManager.GetStat(entity.Id + "BumpedPlayer").IncreaseStat(1f);
		}
	}

	private void OnCollisionStay(Collision collisionInfo)
	{
		if (collisionInfo.collider.tag == "Walls")
		{
			m_wallCollisionTimer = 0.15f;
		}
	}

	private void OnCollisionExit(Collision collisionInfo)
	{
		if (collisionInfo.collider.tag == "Walls")
		{
			m_wallCollisionCounter = Mathf.Max(m_wallCollisionCounter - 1, 0);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!Invincible && other.tag == "Knockback")
		{
			Entity entity = other.GetComponent<Entity>();
			if (entity == null)
			{
				entity = other.GetComponentInParent<Entity>();
			}
			if (entity != null)
			{
				entity.Knockback(this);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (Invincible)
		{
			if (other.tag == "Entity" || other.tag == "Death")
			{
				Entity entity = other.gameObject.GetComponent<Entity>();
				if (entity == null)
				{
					entity = other.gameObject.GetComponentInParent<Entity>();
				}
				if (entity != null)
				{
					entity.Kill(base.gameObject);
					Singleton<Game>.Instance.StatsManager.GetStat("InvincibleKill").IncreaseStat(1f);
					Singleton<Game>.Instance.VisualCamera.Shake("Invincible Kill");
					MasterAudio.PlaySound("NP-EnemyDeath");
				}
				else if (other.tag == "Death")
				{
					Kill(other.gameObject);
				}
			}
		}
		else if (other.tag == "Death")
		{
			Kill(other.gameObject);
		}
	}

	public void AddKnockbackForce(Vector3 force)
	{
		m_knockbackForce = force;
	}

	private void FixedUpdate()
	{
		if (m_knockbackForce != Vector3.zero)
		{
			Rigidbody.velocity += m_knockbackForce;
			m_knockbackForce = Vector3.zero;
		}
	}

	public object Parse()
	{
		object result = null;
		if (Utils.ParseTokenCount() < 2)
		{
			return result;
		}
		if (Utils.ParseTokenHash(1) == "UnlockedCount".GetHashCode())
		{
			float num = 0f;
			foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
			{
				if (!availableCharacter.Locked)
				{
					num += 1f;
				}
			}
			result = num;
		}
		return result;
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_deathChances, m_deathsValues, out m_deaths, "deaths");
	}

	public string GetDeathIdFromIdx(int idx)
	{
		if (idx < 0 || idx >= m_deaths.Count)
		{
			return string.Empty;
		}
		int num = 0;
		foreach (KeyValuePair<PlayerDeath, float> death in m_deaths)
		{
			if (num == idx)
			{
				return death.Key.m_id;
			}
			num++;
		}
		return string.Empty;
	}

	public void SetDeathType(string id)
	{
		if (m_currentDeath != null)
		{
			UnityEngine.Object.Destroy(m_currentDeath.gameObject);
			m_currentDeath = null;
		}
		foreach (KeyValuePair<PlayerDeath, float> death in m_deaths)
		{
			if (death.Key.m_id == id)
			{
				m_currentDeath = Utils.CreateFromPrefab(death.Key, "PlayerDeath");
				return;
			}
		}
		SetRandomDeath();
	}

	public float GetChanceOfPickingDeath(PlayerDeath death)
	{
		float value = 0f;
		if (death != null)
		{
			if (!string.IsNullOrEmpty(m_testDeathName) && death.m_id == m_testDeathName)
			{
				return 1000f;
			}
			string groundSurfaceType = m_groundSurfaceType;
			bool isOnGround = Singleton<Game>.Instance.VisualCamera.Target.IsOnGround;
			bool flag = false;
			if (death.m_allowIfKilledByEntityEvenOffGround && m_killedBy != null)
			{
				flag = true;
			}
			if (groundSurfaceType.StartsWith("Air") && !isOnGround)
			{
				if (death.m_location == PlayerDeath.Location.Air)
				{
					flag = true;
				}
			}
			else if (groundSurfaceType.StartsWith("Water") && !isOnGround)
			{
				if (death.m_location == PlayerDeath.Location.Water)
				{
					flag = true;
				}
			}
			else if (death.m_location == PlayerDeath.Location.Ground)
			{
				flag = true;
			}
			if (m_killedBy != null && m_killedBy.transform.parent != null && death.m_disallowObjectNames.Contains(m_killedBy.transform.parent.gameObject.name))
			{
				flag = false;
			}
			if (death.m_disallowCharacterIDs.Contains(CharacterId))
			{
				flag = false;
			}
			if (death.m_restrictToCharacterIDs.Count != 0 && !death.m_restrictToCharacterIDs.Contains(CharacterId))
			{
				flag = false;
			}
			if (!death.m_allowIfFrozen && IsFrozen)
			{
				flag = false;
			}
			if (flag)
			{
				m_deaths.TryGetValue(death, out value);
			}
		}
		return Mathf.Max(0f, value);
	}

	public void SetRandomDeath()
	{
		if (m_currentDeath != null)
		{
			UnityEngine.Object.Destroy(m_currentDeath.gameObject);
			m_currentDeath = null;
		}
		float num = 0f;
		PlayerDeath prefab = null;
		foreach (KeyValuePair<PlayerDeath, float> death in m_deaths)
		{
			num += GetChanceOfPickingDeath(death.Key);
			prefab = death.Key;
		}
		float num2 = UnityEngine.Random.Range(0f, Mathf.Max(0f, num - 0.001f));
		foreach (KeyValuePair<PlayerDeath, float> death2 in m_deaths)
		{
			num2 -= GetChanceOfPickingDeath(death2.Key);
			if (num2 <= 0f)
			{
				prefab = death2.Key;
				break;
			}
		}
		m_currentDeath = Utils.CreateFromPrefab(prefab, "PlayerDeath");
	}

	public void GoToWipe()
	{
		Singleton<Game>.Instance.SocialManager.CheckLeaderboards();
		Singleton<Game>.Instance.SocialManager.CheckAchievementProgress();
		IsCinematicMove = false;
		Singleton<Game>.Instance.Player.FinishLevel();
		if (m_killedBy == null)
		{
			string empty = string.Empty;
			empty = "Air";
			int num = 1 << LayerMask.NameToLayer("Ground");
			num |= 1 << LayerMask.NameToLayer("Water");
			RaycastHit hitInfo;
			if (Physics.Raycast(new Ray(base.transform.position + new Vector3(0f, 50f, 0f), -base.transform.up.normalized), out hitInfo, 100f, num))
			{
				empty = hitInfo.collider.tag;
			}
			Singleton<Game>.Instance.StatsManager.GetStat("PlayerKilledBy" + empty).IncreaseStat(1f);
		}
		else
		{
			Entity entity = m_killedBy.GetComponent<Entity>();
			if (entity == null)
			{
				entity = m_killedBy.GetComponentInParent<Entity>();
			}
			if (entity != null)
			{
				Singleton<Game>.Instance.StatsManager.GetStat("PlayerKilledBy" + entity.Id).IncreaseStat(1f);
			}
			else
			{
				Singleton<Game>.Instance.StatsManager.GetStat("PlayerKilledBy" + m_killedBy.name).IncreaseStat(1f);
			}
		}
		Singleton<Game>.Instance.StatsManager.GetStat("PlayerDeaths").IncreaseStat(1f);
		m_deathState.CurrentState = State.Wipe;
	}

	private void SetupDeath()
	{
		m_deathState.AddState(State.Idle, IdleEnter, Idle, IdleExit);
		m_deathState.AddState(State.Die, DieEnter, Die, DieExit);
		m_deathState.AddState(State.ChangeCamera, ChangeCameraEnter, ChangeCamera, ChangeCameraExit);
		m_deathState.AddState(State.Transition, TransitionEnter, Transition, TransitionExit);
		m_deathState.AddState(State.Wipe, WipeEnter, Wipe, WipeExit);
		m_deathState.AddState(State.Revive, ReviveEnter, Revive, ReviveExit);
		m_deathState.AddState(State.LoadLevel, LoadLevelEnter, LoadLevel, LoadLevelExit);
		m_deathState.AddState(State.Raise, RaiseEnter, Raise, RaiseExit);
		m_deathState.CurrentState = State.Idle;
	}

	private void IdleEnter()
	{
		m_deathStateTimer = 0f;
		m_timeLerping = false;
	}

	private void Idle()
	{
		if (Singleton<Game>.Instance.Player.IsDead)
		{
			m_deathState.CurrentState = State.Die;
			if (Singleton<Game>.Instance.VisualCamera.Target.GroundHeight == float.MaxValue)
			{
				m_showPlayer = false;
			}
			else
			{
				m_showPlayer = true;
			}
		}
	}

	private void IdleExit()
	{
	}

	private void DieEnter()
	{
		m_deathStateTimer = 0f;
		m_timeLerping = true;
		if (Singleton<Game>.Instance.VisualCamera.Target.GroundHeight != float.MaxValue)
		{
			m_currentDeath.Die();
		}
		else
		{
			m_currentDeath.Die();
		}
	}

	private void Die()
	{
		m_deathState.CurrentState = State.ChangeCamera;
	}

	private void DieExit()
	{
	}

	private void ChangeCameraEnter()
	{
		m_deathStateTimer = 0f;
		if (m_showPlayer)
		{
			Singleton<Game>.Instance.Player.Rigidbody.velocity = Vector3.zero;
		}
	}

	private void ChangeCamera()
	{
		m_transitionCamera = Singleton<Game>.Instance.VisualCamera.gameObject;
		m_deathState.CurrentState = State.Transition;
	}

	private void ChangeCameraExit()
	{
	}

	private void TransitionEnter()
	{
		m_deathStateTimer = 0f;
		Singleton<Game>.Instance.UIController.HUD.SetState(UIHud.State.Results);
		if (m_showPlayer)
		{
			LTDescr lTDescr = LeanTween.value(m_transitionCamera, m_transitionCamera.GetComponent<Camera>().fieldOfView, m_fovTarget, m_fovTweenTimeIn);
			lTDescr.setEase(m_fovTweenIn);
			lTDescr.setOnUpdate(TweenFovUpdate);
			lTDescr.setDelay(m_fovTweenDelayIn);
			Singleton<Game>.Instance.VisualCamera.CanZoom = false;
		}
	}

	private void Transition()
	{
		if (!LeanTween.isTweening(m_transitionCamera) && m_deathStateTimer >= m_currentDeath.m_wipeDelay)
		{
			bool flag = true;
			bool revived = false;
			if (Singleton<Game>.Instance.Player.ForceKill)
			{
				Singleton<Game>.Instance.Player.ForceKill = false;
				flag = false;
			}
			if (Singleton<Game>.Instance.LevelManager.CurrentLevelIndex + 1 < m_minReviveLevel || Singleton<Game>.Instance.LevelManager.CurrentLevelIndex + 1 > m_maxReviveLevel)
			{
				flag = false;
			}
			if (Singleton<Game>.Instance.StatsManager.GetStat("PlayerDeaths").Total.Current < (float)m_minReviveGames)
			{
				flag = false;
			}
			if (Singleton<Game>.Instance.StatsManager.GetStat("Revived").Life.Current != 0f)
			{
				flag = false;
				revived = true;
			}
			if (!Singleton<Game>.Instance.AdManager.IsRewardedVideoReady(true))
			{
				flag = false;
			}
			if (flag)
			{
				m_deathState.CurrentState = State.Revive;
				return;
			}
			GoToWipe();
			Singleton<Game>.Instance.AdManager.EndOfGame(revived);
		}
	}

	private void TransitionExit()
	{
	}

	private void ReviveEnter()
	{
		Singleton<Game>.Instance.UIController.HUD.StartRevive();
	}

	private void Revive()
	{
	}

	private void ReviveExit()
	{
	}

	private void WipeEnter()
	{
		m_deathStateTimer = 0f;
		Singleton<Game>.Instance.UIController.EffectsBack.StartEffect("SpotlightWipe");
		Animator effectAnimator = Singleton<Game>.Instance.UIController.EffectsBack.GetEffectAnimator("SpotlightWipe");
		RawImage componentInChildren = effectAnimator.gameObject.GetComponentInChildren<RawImage>();
		if (componentInChildren != null)
		{
			m_renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
			m_transitionCamera.GetComponent<Camera>().targetTexture = m_renderTexture;
			m_transitionCamera.GetComponent<Camera>().Render();
			m_transitionCamera.GetComponent<Camera>().targetTexture = null;
			componentInChildren.texture = m_renderTexture;
			AspectRatioFitter componentInChildren2 = effectAnimator.GetComponentInChildren<AspectRatioFitter>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.aspectRatio = (float)m_renderTexture.width / (float)m_renderTexture.height;
			}
		}
	}

	private void Wipe()
	{
		if (!LeanTween.isTweening(m_transitionCamera))
		{
			m_deathState.CurrentState = State.LoadLevel;
		}
	}

	private void WipeExit()
	{
	}

	private void LoadLevelEnter()
	{
		m_deathStateTimer = 0f;
		Singleton<Game>.Instance.LevelManager.Restart();
	}

	private void LoadLevel()
	{
		if (Singleton<Game>.Instance.LevelManager.CurrentLevel.IsComplete)
		{
			m_deathState.CurrentState = State.Raise;
		}
	}

	private void LoadLevelExit()
	{
	}

	private void RaiseEnter()
	{
		m_deathStateTimer = 0f;
		SpawnTile spawnTile = UnityEngine.Object.FindObjectOfType<SpawnTile>();
		if (spawnTile != null)
		{
			spawnTile.SetVisible(true);
			Singleton<Game>.Instance.Player.RespawnPlayer(spawnTile.transform.position);
			Singleton<Game>.Instance.VisualCamera.CanZoom = true;
			Singleton<Game>.Instance.VisualCamera.transform.parent = base.transform;
			Singleton<Game>.Instance.VisualCamera.Reset();
		}
		UnityEngine.Object.Destroy(m_currentDeath.gameObject);
		m_currentDeath = null;
		Time.timeScale = 1f;
		m_timeLerping = false;
		Singleton<Game>.Instance.UIController.EffectsBack.EndEffect();
		if (!m_showPlayer)
		{
		}
	}

	private void Raise()
	{
		if (Singleton<Game>.Instance.UIController.EffectsBack.IsComplete && !LeanTween.isTweening(m_transitionCamera))
		{
			Singleton<Game>.Instance.VisualCamera.Camera.cullingMask |= 1 << LayerMask.NameToLayer("Player");
			Singleton<Game>.Instance.Player.StartLevel();
			m_deathState.CurrentState = State.Idle;
		}
	}

	private void RaiseExit()
	{
	}

	private void TweenFovUpdate(float value)
	{
		m_transitionCamera.GetComponent<Camera>().fieldOfView = value;
	}

	private void UpdateDeath()
	{
		m_deathStateTimer += Time.unscaledDeltaTime;
		m_deathState.Update();
		if (m_timeLerping && m_deathState.CurrentState != State.Revive && m_currentDeath != null)
		{
			m_timeScaleLerpTimer += Time.unscaledDeltaTime;
			Time.timeScale = Mathf.Lerp(1f, m_currentDeath.m_timeScale, m_timeScaleLerpTimer / m_currentDeath.m_timeScaleLerp);
		}
		else
		{
			m_timeScaleLerpTimer = 0f;
		}
	}
}
