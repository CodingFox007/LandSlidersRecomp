using System;
using System.Collections.Generic;
using ArabicSupport;
using I2.Loc;
using NativeAlert;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIHud : MonoBehaviour
{
	public enum State
	{
		Normal = 0,
		Results = 1
	}

	[Serializable]
	public class ScoreLookup
	{
		public int score;

		public float percentage;
	}

	[Serializable]
	public class CoinTravellers
	{
		public UICoinTraveller m_travellerPrefab;

		public GameObject m_parent;

		public int m_maxCount = 15;

		public RandomRange m_explodeRange;

		public float m_explodeSpeed = 2f;

		public float m_travelSpeed = 2f;

		public Graphic m_target;

		public LeanTweenType m_explodeTween;

		public LeanTweenType m_travelTween;

		[HideInInspector]
		public List<UICoinTraveller> m_travellers;
	}

	[Serializable]
	public class ScrollBars
	{
		public CanvasGroup m_scrollGroup;

		public float m_alphaUpSpeed;

		public float m_alphaDownSpeed;

		public Scrollbar m_scrollX;

		public float minSizeX;

		public float maxSizeX;

		public float minLevelSizeX;

		public float maxLevelSizeX;

		public Scrollbar m_scrollY;

		public float minSizeY;

		public float maxSizeY;

		public float minLevelSizeY;

		public float maxLevelSizeY;
	}

	public class PopupInfo
	{
		public string[] m_text;

		public Sprite[] m_icons;

		public UIPopup m_uiPopup;

		public int m_id;
	}

	[SerializeField]
	private TextCounter m_scoreCount;

	[SerializeField]
	private TextCounter m_best;

	[SerializeField]
	private Image m_scoreIcon;

	[SerializeField]
	private Animator m_scoreAnimator;

	[SerializeField]
	private Image m_collectIcon;

	[SerializeField]
	private TextCounter m_coinCount;

	[SerializeField]
	private Animator m_coinAnimator;

	[SerializeField]
	private UIPopup m_missionPopup;

	[SerializeField]
	private Animator m_missionAnimator;

	[SerializeField]
	private UIPopup m_missionAcceptPopup;

	[SerializeField]
	private UIPopup m_missionPassedPopup;

	[SerializeField]
	private Animator m_missionAcceptAnimator;

	[SerializeField]
	private Graphic m_missionRewardIcon;

	[SerializeField]
	private GameObject m_reviveButton;

	[SerializeField]
	private Text m_bestScorePlayersText;

	[SerializeField]
	private string m_bestScorePlayersString;

	[SerializeField]
	private Text m_bestScorePercentageText;

	[SerializeField]
	private string m_bestScorePercentageString;

	[SerializeField]
	public UnityEvent m_onBuyAllSpecialSuccess = new UnityEvent();

	public int m_allSpecialNeedsUpdate;

	[SerializeField]
	private int m_scoreLookupWorldPlayers = 1000000;

	public GameObject m_newBestCheck;

	public ScoreLookup[] m_scorelookups;

	[SerializeField]
	private ScrollBars m_scrollBars;

	[SerializeField]
	private int m_starterPackMinCharacters = 4;

	[SerializeField]
	private int m_goldenPackMinCharacters = 4;

	[SerializeField]
	private CoinTravellers m_coinTravellers;

	[SerializeField]
	private UIStateTransition m_prizeButton;

	[SerializeField]
	private Image m_powerupFill;

	[SerializeField]
	private Animator m_powerupAnimator;

	[SerializeField]
	private UIPopup m_genericPopup;

	[SerializeField]
	private Animator m_iCloudPromptAnimator;

	[SerializeField]
	private UIPopup m_preExitPopup;

	[SerializeField]
	private UnityEvent m_isBestScore = new UnityEvent();

	private int m_nextMissionIdx;

	private int m_passedMissionIdx;

	private bool m_missionSelectedThisLevel;

	private bool m_allPickupsCollected;

	private Stat m_scoreStat;

	private List<PopupInfo> m_popupQueue = new List<PopupInfo>();

	public static bool m_waitAFrameBestScore;

	public static int m_suppressPause;

	public static bool m_quitTest;

	private int waitingForAdTimer;

	public int MissionPassedIdx
	{
		get
		{
			return m_passedMissionIdx;
		}
	}

	public Graphic MissionRewardIcon
	{
		get
		{
			return m_missionRewardIcon;
		}
	}

	public void Start()
	{
		Singleton<Game>.Instance.InventoryManager.OnCurrencyChanged += CurrencyChanged;
		Singleton<Game>.Instance.Player.OnCharacterChanged += CharacterChanged;
		Singleton<Game>.Instance.Player.OnPlayerFinishLevel += FinishLevel;
		Singleton<Game>.Instance.LevelManager.OnStartNewLife += StartLife;
		Singleton<Game>.Instance.LevelManager.OnStartLevelLoadComplete += StartLevelLoadComplete;
		Singleton<Game>.Instance.LevelManager.OnRetryLevelLoadComplete += RetryLevelLoadComplete;
		Singleton<Game>.Instance.LevelManager.OnLevelLoadComplete += LevelLoadComplete;
		Singleton<Game>.Instance.EventManager.OnEventTriggered += EventActive;
		m_scoreCount.SetValue(Mathf.FloorToInt(Singleton<Game>.Instance.StatsManager.GetStat("collectible").Life.Current));
		m_best.SetValue(Mathf.FloorToInt(Singleton<Game>.Instance.StatsManager.GetStat("collectible").Life.Highest));
		m_coinCount.SetValue(Singleton<Game>.Instance.InventoryManager.GetCurrency("coin"));
		m_missionAcceptAnimator.gameObject.SetActive(false);
		m_coinTravellers.m_travellers = new List<UICoinTraveller>();
		for (int i = 0; i < m_coinTravellers.m_maxCount; i++)
		{
			UICoinTraveller uICoinTraveller = Utils.CreateFromPrefab(m_coinTravellers.m_travellerPrefab, "CoinTraveller" + i);
			uICoinTraveller.transform.SetParent(m_coinTravellers.m_parent.transform, false);
			uICoinTraveller.IsActive = false;
			m_coinTravellers.m_travellers.Add(uICoinTraveller);
		}
		Singleton<Game>.Instance.UIController.OnStateChanged += OnUIStateChange;
		m_scoreStat = Singleton<Game>.Instance.StatsManager.GetStat("collectible");
	}

	private void OnUIStateChange(UIController.State prev, UIController.State cur)
	{
		if (prev == UIController.State.LevelChange && cur == UIController.State.Retry)
		{
			m_waitAFrameBestScore = true;
		}
	}

	private void StartLife()
	{
		Singleton<Game>.Instance.ShareManager.ScreenshotAvailable = false;
	}

	private void StartLevelLoadComplete()
	{
	}

	public void GoToInGameState()
	{
		Time.timeScale = 1f;
		Singleton<Game>.Instance.UIController.GoToInGameState();
	}

	public void GoToRetryState()
	{
		Singleton<Game>.Instance.UIController.GoToRetryState();
	}

	private void FinishLevel()
	{
		if (m_popupQueue.Count > 0 && m_popupQueue[0].m_uiPopup == m_missionAcceptPopup && m_popupQueue[0].m_uiPopup.IsShowing)
		{
			MissionDecline();
		}
	}

	private void RetryLevelLoadComplete()
	{
		if (SaveManager.CheckICloud())
		{
			ShowICloudPrompt();
		}
		SaveManager.CheckGoogleCloud();
		Singleton<Game>.Instance.UIController.DoSetLevelUp();
	}

	private void LevelLoadComplete()
	{
		ClearPopupQueue();
		if (Singleton<Game>.Instance.LevelManager.CurrentLevelIndex == 0 && Singleton<Game>.Instance.MissionManager.ActiveMission)
		{
			MissionShow();
		}
		m_missionSelectedThisLevel = false;
		m_allPickupsCollected = false;
	}

	private void EventActive(string id)
	{
		EventManager.Event obj = Singleton<Game>.Instance.EventManager.GetEvent(id);
		if (obj != null && obj.m_group == "Prizes" && Singleton<Game>.Instance.LevelManager.IsRetryLevel && m_prizeButton != null && !m_prizeButton.Active)
		{
			m_prizeButton.Activate();
		}
	}

	public void Reset()
	{
		SpawnTile spawnTile = UnityEngine.Object.FindObjectOfType<SpawnTile>();
		if (spawnTile != null)
		{
			Singleton<Game>.Instance.Player.RespawnPlayer(spawnTile.transform.position);
		}
	}

	private void OnEnable()
	{
	}

	public void BuyAllSpecial()
	{
		Singleton<Game>.Instance.SpecialEventManager.BuyRemaining();
	}

	public void BuyAllSpecialSuccess()
	{
		m_allSpecialNeedsUpdate = 2;
		m_onBuyAllSpecialSuccess.Invoke();
	}

	private void OnDisable()
	{
	}

	private void OnAlertFinish(string clickedBtn)
	{
		if (m_quitTest)
		{
			m_quitTest = false;
			if (clickedBtn == ScriptLocalization.Get("Yes", true))
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					androidJavaObject.Call<bool>("moveTaskToBack", new object[1] { true });
					return;
				}
			}
			return;
		}
		string text = ScriptLocalization.Get("Rate It Now Ok Android", true);
		string text2 = ScriptLocalization.Get("Rate It Now Never", true);
		if (clickedBtn == text)
		{
			Application.OpenURL("market://details?id=com.prettygreat.landsliders");
			RateItNowSaveData.Instance.SetRemindTime(long.MaxValue);
		}
		if (clickedBtn == text2)
		{
			RateItNowSaveData.Instance.SetRemindTime(long.MaxValue);
		}
	}

	public void AndroidQuit()
	{
		if (Singleton<Game>.Instance.LevelManager.IsNormalLevel)
		{
			ShowPause();
		}
		else if ((Singleton<Game>.Instance.LevelManager.IsStartLevel || Singleton<Game>.Instance.LevelManager.IsTutorialLevel) && Singleton<Game>.Instance.UIController.CurrentState == UIController.State.Pause)
		{
			Singleton<Game>.Instance.UIController.HidePause();
		}
		else if ((Singleton<Game>.Instance.LevelManager.IsRetryLevel || Singleton<Game>.Instance.LevelManager.IsStartLevel || Singleton<Game>.Instance.LevelManager.IsTutorialLevel) && (!Singleton<Game>.Instance.LevelManager.IsRetryLevel || Singleton<Game>.Instance.UIController.CurrentState == UIController.State.Retry))
		{
			m_suppressPause = 2;
			m_quitTest = true;
			AndroidNativeAlert.ShowAlert(ScriptLocalization.Get("Exit", true), ScriptLocalization.Get("Are you sure you want to quit the game?", true), ScriptLocalization.Get("Yes", true), ScriptLocalization.Get("No", true));
		}
	}

	private bool isiPhoneX()
	{
		return false;
	}

	private void Update()
	{
		if (!Singleton<Game>.Instance.IsLoaded)
		{
			return;
		}
		if (m_waitAFrameBestScore)
		{
			m_waitAFrameBestScore = false;
			DoNewBestScoreCheck();
		}
		if (waitingForAdTimer > 0)
		{
			waitingForAdTimer--;
			if (waitingForAdTimer == 0)
			{
				GoToWipe();
			}
		}
		if (m_allSpecialNeedsUpdate > 0)
		{
			m_allSpecialNeedsUpdate--;
		}
		float value = Mathf.InverseLerp(Singleton<Game>.Instance.LevelManager.CurrentLevel.WorldBounds.xMin, Singleton<Game>.Instance.LevelManager.CurrentLevel.WorldBounds.xMax, Singleton<Game>.Instance.Player.transform.position.x);
		float value2 = Mathf.InverseLerp(Singleton<Game>.Instance.LevelManager.CurrentLevel.WorldBounds.yMin, Singleton<Game>.Instance.LevelManager.CurrentLevel.WorldBounds.yMax, Singleton<Game>.Instance.Player.transform.position.z);
		float t = Mathf.InverseLerp(m_scrollBars.minLevelSizeX, m_scrollBars.maxLevelSizeX, Singleton<Game>.Instance.LevelManager.CurrentLevel.WorldBounds.width);
		float t2 = Mathf.InverseLerp(m_scrollBars.minLevelSizeY, m_scrollBars.maxLevelSizeY, Singleton<Game>.Instance.LevelManager.CurrentLevel.WorldBounds.height);
		if (Singleton<Game>.Instance.IsTouchDown && !isiPhoneX())
		{
			m_scrollBars.m_scrollGroup.alpha += m_scrollBars.m_alphaUpSpeed * Time.deltaTime;
			m_scrollBars.m_scrollGroup.alpha = Mathf.Min(m_scrollBars.m_scrollGroup.alpha, 1f);
		}
		else
		{
			m_scrollBars.m_scrollGroup.alpha -= m_scrollBars.m_alphaDownSpeed * Time.deltaTime;
			m_scrollBars.m_scrollGroup.alpha = Mathf.Max(m_scrollBars.m_scrollGroup.alpha, 0f);
		}
		m_scrollBars.m_scrollX.size = Mathf.Lerp(m_scrollBars.minSizeX, m_scrollBars.maxSizeX, t);
		m_scrollBars.m_scrollY.size = Mathf.Lerp(m_scrollBars.minSizeY, m_scrollBars.maxSizeY, t2);
		m_scrollBars.m_scrollX.value = value;
		m_scrollBars.m_scrollY.value = value2;
		int num = Mathf.FloorToInt(m_scoreStat.Life.Current);
		if (m_scoreCount.GetTarget() != num)
		{
			m_scoreAnimator.SetTrigger("Collected");
			m_scoreCount.SetTarget(num);
		}
		int num2 = Mathf.FloorToInt(m_scoreStat.Life.Highest);
		if (m_best.GetTarget() != num2)
		{
			m_best.SetTarget(num2);
		}
	}

	public string GetLocalisedBestString<T>(string format, T value)
	{
		string empty = string.Empty;
		if (LocalizationManager.IsRight2Left)
		{
			LocalizationManager.IsRight2Left = false;
			empty = ArabicFixer.Fix(string.Format(ScriptLocalization.Get(format), value));
			LocalizationManager.IsRight2Left = true;
		}
		else
		{
			empty = string.Format(ScriptLocalization.Get(format, true), value);
		}
		return empty;
	}

	public void DoNewBestScoreCheck()
	{
		int num = Mathf.FloorToInt(Singleton<Game>.Instance.StatsManager.GetStat("PlayerDeaths").Total.Current);
		int num2 = Mathf.FloorToInt(m_scoreStat.Life.Current);
		int num3 = Mathf.FloorToInt(m_scoreStat.Life.Highest);
		int i = 0;
		float num4 = 0f;
		int num5 = 0;
		if (m_scorelookups != null)
		{
			for (; i < m_scorelookups.Length - 1 && num3 >= m_scorelookups[i + 1].score; i++)
			{
			}
			if (i >= m_scorelookups.Length - 1)
			{
				i = m_scorelookups.Length - 2;
			}
			if (i < m_scorelookups.Length)
			{
				int score = m_scorelookups[i].score;
				int score2 = m_scorelookups[i + 1].score;
				float percentage = m_scorelookups[i].percentage;
				float percentage2 = m_scorelookups[i + 1].percentage;
				if (score2 == -1)
				{
					num4 = percentage2;
					num5 = (int)((float)m_scoreLookupWorldPlayers * (num4 / 100f));
				}
				else
				{
					float num6 = (float)(num3 - score) / (float)(score2 - score);
					num4 = percentage + num6 * (percentage2 - percentage);
					num5 = (int)((float)m_scoreLookupWorldPlayers * (num4 / 100f));
				}
				if (m_bestScorePercentageText != null)
				{
					m_bestScorePercentageText.text = GetLocalisedBestString(m_bestScorePercentageString, num4);
				}
				if (m_bestScorePlayersText != null)
				{
					m_bestScorePlayersText.text = GetLocalisedBestString(m_bestScorePlayersString, m_scoreLookupWorldPlayers - num5);
				}
			}
		}
		if (num2 == num3 && num > 1)
		{
			m_isBestScore.Invoke();
			Singleton<Game>.Instance.UIController.ShowNewBest();
		}
		else if (InvokeStarterPack())
		{
			Singleton<Game>.Instance.StatsManager.GetStat("starterPack").SetValue((Singleton<Game>.Instance.Player.UnlockedCharacterCount() - m_starterPackMinCharacters) / 10 + 1);
			Singleton<Game>.Instance.UIController.ShowStarterPack();
		}
		else if (InvokeGoldenPack())
		{
			Singleton<Game>.Instance.UIController.ShowGoldenPack();
		}
		else
		{
			Singleton<Game>.Instance.UIController.DoStartLevelUp();
		}
	}

	public bool InvokeStarterPack()
	{
		if (Singleton<Game>.Instance.Player.UnlockedCharacterCount() < m_starterPackMinCharacters)
		{
			return false;
		}
		if (Singleton<Game>.Instance.StatsManager.GetStat("starterPack").Total.Current >= 3f)
		{
			return false;
		}
		if (Singleton<Game>.Instance.StatsManager.GetStat("starterPack").Total.Current >= (float)((Singleton<Game>.Instance.Player.UnlockedCharacterCount() - m_starterPackMinCharacters) / 10 + 1))
		{
			return false;
		}
		if (!Unibiller.GetPurchasableItemById("com.prettygreat.landsliders.starterPack").AvailableToPurchase)
		{
			return false;
		}
		return true;
	}

	public bool InvokeGoldenPack()
	{
		string[] array = new string[12]
		{
			"rooster", "ox", "rabbit", "snake", "goat", "horse", "monkey", "rat", "tiger", "dragon",
			"cnydog", "pig"
		};
		int num = 0;
		string[] array2 = array;
		foreach (string id in array2)
		{
			if (Singleton<Game>.Instance.Player.GetCharacterInfo(id) != null && !Singleton<Game>.Instance.Player.GetCharacterInfo(id).Locked)
			{
				num++;
			}
		}
		if (num >= 12 && Singleton<Game>.Instance.StatsManager.GetStat("goldenPack12").Total.Current == 0f)
		{
			Singleton<Game>.Instance.StatsManager.GetStat("goldenPack12").IncreaseStat(1f);
			Singleton<Game>.Instance.StatsManager.GetStat("goldenPack8").IncreaseStat(1f);
			Singleton<Game>.Instance.StatsManager.GetStat("goldenPack4").IncreaseStat(1f);
			SaveManager.Save();
			return true;
		}
		if (num >= 8 && Singleton<Game>.Instance.StatsManager.GetStat("goldenPack8").Total.Current == 0f)
		{
			Singleton<Game>.Instance.StatsManager.GetStat("goldenPack8").IncreaseStat(1f);
			Singleton<Game>.Instance.StatsManager.GetStat("goldenPack4").IncreaseStat(1f);
			SaveManager.Save();
			return true;
		}
		if (num >= 4 && Singleton<Game>.Instance.StatsManager.GetStat("goldenPack4").Total.Current == 0f)
		{
			Singleton<Game>.Instance.StatsManager.GetStat("goldenPack4").IncreaseStat(1f);
			SaveManager.Save();
			return true;
		}
		return false;
	}

	public void SetState(State state)
	{
		if (state == State.Results)
		{
			base.gameObject.GetComponent<Animator>().SetBool("Results", true);
		}
		else
		{
			base.gameObject.GetComponent<Animator>().SetBool("Results", false);
		}
	}

	public void ShowResults()
	{
		m_scoreAnimator.SetTrigger("Transition");
		MissionShow();
	}

	public void HideResults()
	{
		m_scoreAnimator.SetTrigger("TransitionOut");
	}

	public void HideSpawnHUD()
	{
		MissionHide();
	}

	public void SetupMissionPopup(string[] text, Sprite[] icons, bool passed)
	{
		m_missionPopup.Create(text, icons);
	}

	public void MissionShow()
	{
		if (Singleton<Game>.Instance.MissionManager.ActiveMission)
		{
			m_missionAnimator.SetBool("ShowingMission", true);
		}
	}

	public void MissionHide()
	{
		m_missionAnimator.SetBool("ShowingMission", false);
	}

	public void MissionAccept()
	{
		Singleton<Game>.Instance.MissionManager.SetMision(m_nextMissionIdx);
		m_missionAcceptAnimator.SetBool("AcceptedMission", true);
		MissionAcceptHide();
		m_missionAcceptPopup.Hide();
		m_missionAnimator.SetBool("MissionCancelled", false);
		m_missionAnimator.SetTrigger("AcceptedMission");
	}

	public void MissionDecline()
	{
		m_missionAcceptAnimator.SetBool("AcceptedMission", false);
		MissionAcceptHide();
	}

	public void MissionAcceptShow()
	{
		if (!Singleton<Game>.Instance.Player.IsDead)
		{
			if (!m_missionSelectedThisLevel)
			{
				m_nextMissionIdx = Singleton<Game>.Instance.MissionManager.SelectRandomMissionIdx();
				m_missionSelectedThisLevel = true;
			}
			string[] popupText;
			Sprite[] icons;
			Singleton<Game>.Instance.MissionManager.GetMissionDescription(m_nextMissionIdx, out popupText, out icons);
			List<string> list = new List<string>();
			list.AddRange(popupText);
			list.AddRange(popupText);
			List<Sprite> list2 = new List<Sprite>();
			list2.AddRange(icons);
			list2.AddRange(icons);
			m_missionAcceptAnimator.gameObject.SetActive(true);
			AddPopup(list.ToArray(), list2.ToArray(), m_missionAcceptPopup);
			m_missionAcceptAnimator.SetBool("ShowMissionAccept", true);
			Time.timeScale = 0f;
		}
	}

	public void MissionAcceptHide()
	{
		m_missionAcceptPopup.Hide();
		Time.timeScale = 1f;
	}

	public void MissionPassed()
	{
		string[] popupText;
		Sprite[] icons;
		Singleton<Game>.Instance.MissionManager.GetMissionDescription(m_nextMissionIdx, out popupText, out icons);
		List<string> list = new List<string>();
		list.AddRange(popupText);
		list.AddRange(popupText);
		List<Sprite> list2 = new List<Sprite>();
		list2.AddRange(icons);
		list2.AddRange(icons);
		m_missionAcceptAnimator.gameObject.SetActive(true);
		AddPopup(list.ToArray(), list2.ToArray(), m_missionPassedPopup);
		m_passedMissionIdx = Singleton<Game>.Instance.MissionManager.CurrentMissionIdx;
	}

	public void MissionCollectReward()
	{
		Singleton<Game>.Instance.MissionManager.CollectReward(m_passedMissionIdx);
	}

	public void MissionCancel()
	{
		Singleton<Game>.Instance.MissionManager.CancelMission();
		m_missionAnimator.SetBool("MissionCancelled", true);
		MissionHide();
	}

	public void ShowPowerup()
	{
		m_powerupAnimator.SetTrigger("ShowBottle");
	}

	public void HidePowerup()
	{
		m_powerupAnimator.SetTrigger("HideBottle");
	}

	public void SetPowerupProgress(float progress)
	{
		m_powerupFill.fillAmount = progress;
	}

	public void ReportIssue()
	{
		string text = "baz@prettygreat.com";
		string text2 = MyEscapeURL("Level Issue Report");
		string text3 = MyEscapeURL("Level Issue\r\nSeed: " + Singleton<Game>.Instance.LevelManager.CurrentLevel.seed + "\r\n" + Singleton<Game>.Instance.LevelManager.CurrentLevel.name);
		Application.OpenURL("mailto:" + text + "?subject=" + text2 + "&body=" + text3);
	}

	private string MyEscapeURL(string url)
	{
		return WWW.EscapeURL(url).Replace("+", "%20");
	}

	public void CurrencyChanged(string id, int prev, int curr)
	{
		if (id == "coin")
		{
			m_coinCount.SetTarget(curr);
			if (!m_coinCount.CountActive)
			{
				m_coinAnimator.SetTrigger("Collected");
			}
		}
	}

	public void GoToReviveAd()
	{
		if (!Singleton<Game>.Instance.Player.IsWiping)
		{
			waitingForAdTimer = 60;
			Singleton<Game>.Instance.AdManager.ShowRewardedVideo(AdCallback, "Revive");
			m_reviveButton.GetComponent<Animator>().SetTrigger("buttonOut");
			m_reviveButton.GetComponent<UIReviveButton>().m_running = false;
		}
	}

	private void AdCallback(bool result)
	{
		waitingForAdTimer = 0;
		if (result)
		{
			Singleton<Game>.Instance.Player.WaitHack();
			Time.timeScale = 1f;
		}
		else
		{
			Singleton<Game>.Instance.Player.GoToWipe();
		}
	}

	public void GoToWipe()
	{
		Singleton<Game>.Instance.Player.GoToWipe();
	}

	public void StartRevive()
	{
		Time.timeScale = 0f;
		m_reviveButton.SetActive(true);
		m_reviveButton.GetComponent<Animator>().SetTrigger("buttonIn");
		Singleton<Game>.Instance.Player.StopDeathNow();
	}

	public void CharacterChanged()
	{
		GameObject replacementPickup = Singleton<Game>.Instance.Player.CharacterInfo.ReplacementPickup;
		m_scoreIcon.sprite = replacementPickup.GetComponent<Pickup>().Icon;
		m_collectIcon.sprite = replacementPickup.GetComponent<Pickup>().Icon;
		if (Singleton<Game>.Instance.MissionManager.ActiveMission)
		{
			string[] popupText;
			Sprite[] icons;
			Singleton<Game>.Instance.MissionManager.GetPopupText(out popupText, out icons);
			SetupMissionPopup(popupText, icons, false);
		}
	}

	public void SwitchCharacter()
	{
		Singleton<Game>.Instance.UIController.ShowCharacterSelect();
	}

	public void SelectPrize()
	{
		Singleton<Game>.Instance.UIController.ShowPrizeSelect();
	}

	public void ShowPause()
	{
		Singleton<Game>.Instance.UIController.ShowPause();
	}

	public void SpawnCoinTravellers(Graphic source, int amount)
	{
		Singleton<Game>.Instance.InventoryManager.AddCurrency("coin", amount);
		int num = 0;
		foreach (UICoinTraveller traveller in m_coinTravellers.m_travellers)
		{
			if (!traveller.IsActive)
			{
				traveller.IsActive = true;
				m_coinTravellers.m_explodeRange.Randomize();
				traveller.Spawn(source, m_coinTravellers.m_target, m_coinTravellers.m_explodeRange.Value, m_coinTravellers.m_explodeSpeed, m_coinTravellers.m_travelSpeed, m_coinTravellers.m_explodeTween, m_coinTravellers.m_travelTween);
				num++;
			}
			if (num >= amount)
			{
				break;
			}
		}
	}

	public void AddPopup(string[] text, Sprite[] icons, UIPopup popup = null)
	{
		if (popup == null)
		{
			popup = m_genericPopup;
		}
		PopupInfo popupInfo = new PopupInfo();
		popupInfo.m_text = text;
		popupInfo.m_icons = icons;
		popupInfo.m_uiPopup = popup;
		popupInfo.m_id = Mathf.FloorToInt(Time.realtimeSinceStartup * 100f);
		m_popupQueue.Add(popupInfo);
		if (m_popupQueue.Count == 1)
		{
			m_popupQueue[0].m_uiPopup.Create(m_popupQueue[0].m_text, m_popupQueue[0].m_icons);
			m_popupQueue[0].m_uiPopup.Show(m_popupQueue[0].m_id);
		}
	}

	public void RemovePopup(UIPopup popup)
	{
		if (m_popupQueue.Count > 0 && (!(m_popupQueue[0].m_uiPopup != popup) || m_popupQueue[0].m_id == popup.Id))
		{
			m_popupQueue.RemoveAt(0);
			if (m_popupQueue.Count > 0)
			{
				m_popupQueue[0].m_uiPopup.Create(m_popupQueue[0].m_text, m_popupQueue[0].m_icons);
				m_popupQueue[0].m_uiPopup.Show(m_popupQueue[0].m_id);
			}
		}
	}

	public void ClearPopupQueue()
	{
		foreach (PopupInfo item in m_popupQueue)
		{
			if (item.m_uiPopup.IsShowing)
			{
				item.m_uiPopup.Hide();
			}
		}
		m_popupQueue.Clear();
	}

	public void DisableMisionAccept()
	{
		UIRenderCharacter component = m_missionAcceptAnimator.GetComponent<UIRenderCharacter>();
		if (component != null)
		{
			component.Unload();
		}
		m_missionAcceptAnimator.gameObject.SetActive(false);
	}

	public void CreateMissionAcceptPopup(UIPopup popup)
	{
		string[] popupText;
		Sprite[] icons;
		Singleton<Game>.Instance.MissionManager.GetMissionDescription(m_nextMissionIdx, out popupText, out icons);
		List<string> list = new List<string>();
		list.AddRange(popupText);
		list.AddRange(popupText);
		List<Sprite> list2 = new List<Sprite>();
		list2.AddRange(icons);
		list2.AddRange(icons);
		popup.Create(list.ToArray(), list2.ToArray());
		UIRenderCharacter component = m_missionAcceptAnimator.GetComponent<UIRenderCharacter>();
		if (component != null)
		{
			component.Load();
		}
	}

	public void ClearAllPopups()
	{
		m_popupQueue.Clear();
	}

	public bool IsPopupQueued(UIPopup prefab)
	{
		foreach (PopupInfo item in m_popupQueue)
		{
			if (item.m_uiPopup == prefab)
			{
				return true;
			}
		}
		return false;
	}

	public void ShowICloudPrompt()
	{
		m_iCloudPromptAnimator.SetTrigger("show");
	}

	public void ShowPreExitPopup()
	{
		if (!m_allPickupsCollected && !IsPopupQueued(m_preExitPopup))
		{
			AddPopup(null, null, m_preExitPopup);
		}
	}

	public void AllPickupsCollected()
	{
		m_allPickupsCollected = true;
	}
}
