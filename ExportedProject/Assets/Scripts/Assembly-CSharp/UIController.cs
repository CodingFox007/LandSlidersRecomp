using System.Collections;
using DarkTonic.MasterAudio;
using UnityEngine;

public class UIController : MonoBehaviour
{
	public delegate void StartLevelUp();

	public delegate void SetLevelUp();

	public enum State
	{
		StartLevel = 0,
		StartNewLife = 1,
		InGame = 2,
		LevelChange = 3,
		Retry = 4,
		Shop = 5,
		Social = 6,
		Achievements = 7,
		Leaderboard = 8,
		CharacterSelect = 9,
		CharacterUnlock = 10,
		PrizeSelect = 11,
		TutorialLevel = 12,
		Settings = 13,
		Pause = 14,
		NewBest = 15,
		SpecialEvent = 16,
		StarterPack = 17,
		GoldenPack = 18
	}

	public delegate void StateChanged(State prev, State cur);

	[SerializeField]
	private Camera m_uiCameraPrefab;

	private Camera m_uiCamera;

	[SerializeField]
	private Camera m_uiParticleCameraPrefab;

	private Camera m_uiParticleCamera;

	[SerializeField]
	private UIHud m_hudPrefab;

	private UIHud m_hud;

	[SerializeField]
	private UIEffects m_effectsBackPrefab;

	private UIEffects m_effectsBack;

	[SerializeField]
	private UIEffects m_effectsTopPrefab;

	private UIEffects m_effectsTop;

	[SerializeField]
	private UICharacterSelect m_characterSelectPrefab;

	private UICharacterSelect m_characterSelect;

	[SerializeField]
	private UICharacterUnlock m_characterUnlockPrefab;

	private UICharacterUnlock m_characterUnlock;

	[SerializeField]
	private UIPrizeSelect m_prizeSelectPrefab;

	private UIPrizeSelect m_prizeSelect;

	[SerializeField]
	private UISettings m_settingsPrefab;

	private UISettings m_settings;

	[SerializeField]
	private GameObject m_waitingBlockerPrefab;

	private GameObject m_waitingBlocker;

	[SerializeField]
	private GameObject m_pinPromptPrefab;

	private GameObject m_pinPrompt;

	[SerializeField]
	private GameObject m_starterPackPrefab;

	private GameObject m_starterPack;

	[SerializeField]
	private GameObject m_goldenPackPrefab;

	private GameObject m_goldenPack;

	[SerializeField]
	private UIChooseCharacter m_chooseCharacterPrefab;

	private UIChooseCharacter m_chooseCharacter;

	[SerializeField]
	private UIChooseControls m_chooseControlsPrefab;

	private UIChooseControls m_chooseControls;

	private State m_state;

	private State m_prevState;

	public Camera UICamera
	{
		get
		{
			return m_uiCamera;
		}
	}

	public Camera UIParticleCamera
	{
		get
		{
			return m_uiParticleCamera;
		}
	}

	public UIHud HUD
	{
		get
		{
			return m_hud;
		}
	}

	public UIEffects EffectsTop
	{
		get
		{
			return m_effectsTop;
		}
	}

	public UIEffects EffectsBack
	{
		get
		{
			return m_effectsBack;
		}
	}

	public UICharacterSelect CharacterSelect
	{
		get
		{
			return m_characterSelect;
		}
	}

	public UICharacterUnlock CharacterUnlock
	{
		get
		{
			return m_characterUnlock;
		}
	}

	public UIPrizeSelect PrizeSelect
	{
		get
		{
			return m_prizeSelect;
		}
	}

	public UISettings Settings
	{
		get
		{
			return m_settings;
		}
	}

	public State CurrentState
	{
		get
		{
			return m_state;
		}
	}

	public event StartLevelUp OnStartLevelUp;

	public event SetLevelUp OnSetLevelUp;

	public event StateChanged OnStateChanged;

	private void Start()
	{
	}

	public IEnumerator Load()
	{
		m_uiCamera = Object.Instantiate(m_uiCameraPrefab);
		m_uiCamera.name = "UICamera";
		base.gameObject.GetComponent<Canvas>().worldCamera = m_uiCamera;
		m_uiParticleCamera = Object.Instantiate(m_uiParticleCameraPrefab);
		m_uiParticleCamera.name = "UIParticleCamera";
		m_effectsBack = Utils.CreateFromPrefab(m_effectsBackPrefab, "EffectsBack");
		m_effectsBack.transform.SetParent(base.transform, false);
		m_characterSelect = Utils.CreateFromPrefab(m_characterSelectPrefab, "CharacterSelect");
		m_characterSelect.transform.SetParent(base.transform, false);
		m_starterPack = Utils.CreateFromPrefab(m_starterPackPrefab, "StarterPack");
		m_starterPack.transform.SetParent(base.transform, false);
		m_starterPack.SetActive(true);
		m_goldenPack = Utils.CreateFromPrefab(m_goldenPackPrefab, "GoldenPack");
		m_goldenPack.transform.SetParent(base.transform, false);
		m_goldenPack.SetActive(true);
		yield return null;
		m_characterUnlock = Utils.CreateFromPrefab(m_characterUnlockPrefab, "CharacterUnlock");
		m_characterUnlock.transform.SetParent(base.transform, false);
		m_prizeSelect = Utils.CreateFromPrefab(m_prizeSelectPrefab, "PrizeSelect");
		m_prizeSelect.transform.SetParent(base.transform, false);
		m_hud = Utils.CreateFromPrefab(m_hudPrefab, "HUD");
		m_hud.transform.SetParent(base.transform, false);
		yield return null;
		m_settings = Utils.CreateFromPrefab(m_settingsPrefab, "Settings");
		m_settings.transform.SetParent(base.transform, false);
		yield return null;
		m_pinPrompt = Utils.CreateFromPrefab(m_pinPromptPrefab, "PinPrompt");
		m_pinPrompt.transform.SetParent(base.transform, false);
		m_pinPrompt.SetActive(false);
		if (m_chooseCharacterPrefab != null)
		{
			m_chooseCharacter = Utils.CreateFromPrefab(m_chooseCharacterPrefab, "ChooseCharacter");
			m_chooseCharacter.transform.SetParent(base.transform, false);
			m_chooseCharacter.gameObject.SetActive(false);
		}
		if (m_chooseControlsPrefab != null)
		{
			m_chooseControls = Utils.CreateFromPrefab(m_chooseControlsPrefab, "ChooseControls");
			m_chooseControls.transform.SetParent(base.transform, false);
			m_chooseControls.gameObject.SetActive(false);
		}
		m_effectsTop = Utils.CreateFromPrefab(m_effectsTopPrefab, "EffectsTop");
		m_effectsTop.transform.SetParent(base.transform, false);
		m_waitingBlocker = Utils.CreateFromPrefab(m_waitingBlockerPrefab, "WaitingBlocker");
		m_waitingBlocker.transform.SetParent(base.transform, false);
		m_waitingBlocker.SetActive(false);
		Singleton<Game>.Instance.LevelManager.OnStartLevelLoadComplete += StartLevel;
		Singleton<Game>.Instance.LevelManager.OnStartNewLife += StartNewLife;
		Singleton<Game>.Instance.LevelManager.OnRetryLevelLoadComplete += RetryLevelLoadComplete;
		Singleton<Game>.Instance.LevelManager.OnLevelLoadComplete += LevelLoadComplete;
		Singleton<Game>.Instance.Player.OnPlayerStartLevel += PlayerStartLevel;
		Singleton<Game>.Instance.Player.OnPlayerFinishLevel += PlayerFinishLevel;
	}

	public void DoSetLevelUp()
	{
		if (this.OnSetLevelUp != null)
		{
			this.OnSetLevelUp();
		}
	}

	public void DoStartLevelUp()
	{
		if (this.OnStartLevelUp != null)
		{
			this.OnStartLevelUp();
		}
	}

	private void StartLevel()
	{
		m_prevState = m_state;
		if (Singleton<Game>.Instance.LevelManager.IsTutorialLevel)
		{
			m_state = State.TutorialLevel;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
		else
		{
			m_state = State.StartLevel;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	private void PlayerStartLevel()
	{
		if (Singleton<Game>.Instance.LevelManager.IsRetryLevel)
		{
			m_prevState = m_state;
			m_state = State.Retry;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	private void PlayerFinishLevel()
	{
		m_prevState = m_state;
		m_state = State.LevelChange;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	private void StartNewLife()
	{
		m_prevState = m_state;
		m_state = State.StartNewLife;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	private void RetryLevelLoadComplete()
	{
	}

	public void GoToRetryState()
	{
		State state = m_state;
		m_state = m_prevState;
		m_prevState = state;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	public void GoToInGameState()
	{
		LevelLoadComplete();
	}

	private void LevelLoadComplete()
	{
		m_prevState = m_state;
		m_state = State.InGame;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	private void Update()
	{
	}

	public void ShowCharacterSelect()
	{
		if (m_state == State.StartLevel || m_state == State.Retry)
		{
			m_prevState = m_state;
			m_state = State.CharacterSelect;
			MasterAudio.TriggerPlaylistClip("Shop");
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	public void HideCharacterSelect()
	{
		State state = m_state;
		m_state = m_prevState;
		m_prevState = state;
		if (Singleton<Game>.Instance.LevelManager.IsRetryLevel)
		{
			MasterAudio.TriggerPlaylistClip("Heaven");
		}
		else
		{
			MasterAudio.TriggerPlaylistClip("Main");
		}
		StartCoroutine(Utils.ClearUnusedAssets());
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	public void ShowCharacterUnlock()
	{
		if (m_state == State.Retry)
		{
			m_prevState = m_state;
			m_state = State.CharacterUnlock;
			MasterAudio.TriggerPlaylistClip("Shop");
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	public void HideCharacterUnlock()
	{
		State state = m_state;
		m_state = m_prevState;
		m_prevState = state;
		if (Singleton<Game>.Instance.LevelManager.IsRetryLevel)
		{
			MasterAudio.TriggerPlaylistClip("Heaven");
		}
		else
		{
			MasterAudio.TriggerPlaylistClip("Main");
		}
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	public void ShowPrizeSelect()
	{
		if (m_state == State.Retry)
		{
			m_prevState = m_state;
			m_state = State.PrizeSelect;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	public void HidePrizeSelect()
	{
		State state = m_state;
		m_state = m_prevState;
		m_prevState = state;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	public void ShowSettings()
	{
		if (m_state == State.Retry)
		{
			m_prevState = m_state;
			m_state = State.Settings;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	public void HideSettings()
	{
		State state = m_state;
		m_state = m_prevState;
		m_prevState = state;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	public void ShowGoldenPack()
	{
		if (m_state == State.Retry)
		{
			m_prevState = m_state;
			m_state = State.GoldenPack;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	public void HideGoldenPack()
	{
		State state = m_state;
		m_state = m_prevState;
		m_prevState = state;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	public void ShowStarterPack()
	{
		if (m_state == State.Retry)
		{
			m_prevState = m_state;
			m_state = State.StarterPack;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	public void HideStarterPack()
	{
		State state = m_state;
		m_state = m_prevState;
		m_prevState = state;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	public void ShowNewBest()
	{
		if (m_state == State.Retry)
		{
			m_prevState = m_state;
			m_state = State.NewBest;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	public void HideNewBest()
	{
		State state = m_state;
		m_state = m_prevState;
		m_prevState = state;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	public void ShowPause()
	{
		if (!Singleton<Game>.Instance.Player.BlockInput && (m_state == State.InGame || m_state == State.StartLevel || m_state == State.TutorialLevel))
		{
			m_prevState = m_state;
			m_state = State.Pause;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	public void HidePause()
	{
		if (m_state == State.Pause)
		{
			State state = m_state;
			m_state = m_prevState;
			m_prevState = state;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	public void ShowWaitingBlocker()
	{
		m_waitingBlocker.SetActive(true);
	}

	public void HideWaitingBlocker()
	{
		if (m_waitingBlocker != null)
		{
			m_waitingBlocker.SetActive(false);
		}
	}

	public void ShowPinPrompt()
	{
		m_pinPrompt.SetActive(true);
	}

	public void HidePinPrompt()
	{
		m_pinPrompt.SetActive(false);
	}

	public void ShowSharing()
	{
		if (m_state == State.Retry)
		{
			m_prevState = m_state;
			m_state = State.Social;
			if (this.OnStateChanged != null)
			{
				this.OnStateChanged(m_prevState, m_state);
			}
		}
	}

	public void ShowSpecialEvent()
	{
		m_prevState = m_state;
		m_state = State.SpecialEvent;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	public void HideSharing()
	{
		State state = m_state;
		m_state = m_prevState;
		m_prevState = state;
		if (this.OnStateChanged != null)
		{
			this.OnStateChanged(m_prevState, m_state);
		}
	}

	public void ShowChooseCharacter()
	{
		if (m_chooseCharacter != null)
		{
			Vector3 velocity = Singleton<Game>.Instance.Player.Rigidbody.velocity;
			velocity.x = 0f;
			velocity.z = 0f;
			Singleton<Game>.Instance.Player.Rigidbody.velocity = velocity;
			Singleton<Game>.Instance.Player.BlockInput = true;
			m_chooseCharacter.gameObject.SetActive(true);
		}
	}

	public void HideChooseCharacter()
	{
		if (m_chooseCharacter != null)
		{
			Singleton<Game>.Instance.Player.BlockInput = false;
			m_chooseCharacter.gameObject.SetActive(false);
		}
	}

	public bool IsChooseCharacterActive()
	{
		if (m_chooseCharacter != null)
		{
			return m_chooseCharacter.gameObject.activeSelf;
		}
		return false;
	}

	public void ShowChooseControls()
	{
		if (m_chooseControls != null)
		{
			Vector3 velocity = Singleton<Game>.Instance.Player.Rigidbody.velocity;
			velocity.x = 0f;
			velocity.z = 0f;
			Singleton<Game>.Instance.Player.Rigidbody.velocity = velocity;
			Singleton<Game>.Instance.Player.BlockInput = true;
			m_chooseControls.gameObject.SetActive(true);
		}
	}

	public void HideChooseControls()
	{
		if (m_chooseControls != null)
		{
			Singleton<Game>.Instance.Player.BlockInput = false;
			m_chooseControls.gameObject.SetActive(false);
		}
	}

	public bool IsChooseControlsActive()
	{
		if (m_chooseControls != null)
		{
			return m_chooseControls.gameObject.activeSelf;
		}
		return false;
	}

	public bool CanChangeMenu()
	{
		return !IsChooseCharacterActive() && !IsChooseControlsActive();
	}
}
