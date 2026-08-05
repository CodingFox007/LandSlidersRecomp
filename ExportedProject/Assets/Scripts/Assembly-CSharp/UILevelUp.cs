using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UILevelUp : MonoBehaviour, IEventSystemHandler
{
	[Serializable]
	public class Tier : IEventSystemHandler
	{
		public int m_reward;

		public string m_rewardCurrencyId;

		public string m_levelUpString;

		[SerializeField]
		public UnityEvent m_onStart = new UnityEvent();

		[SerializeField]
		public UnityEvent m_onLeveup = new UnityEvent();
	}

	[SerializeField]
	private Tier[] m_tiers;

	[SerializeField]
	private Image m_progressBar;

	[SerializeField]
	private TextCounter m_currentText;

	[SerializeField]
	private Text m_targetText;

	[SerializeField]
	private Text m_rewardText;

	[SerializeField]
	private Text m_nameText;

	[SerializeField]
	private Graphic m_rewardIcon;

	[SerializeField]
	private Text m_levelUpText;

	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private SpawnPickups m_spawnPickups;

	[SerializeField]
	public UnityEvent m_onEnable = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onStart = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onLevelUp = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onComplete = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onClose = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onLevelUpHasStoppedAnimating = new UnityEvent();

	private float m_cloudBackup;

	private bool m_hasFinishedCountingXP;

	private bool m_hasStartedCountingXP;

	public Tier CurrentRewardTier
	{
		get
		{
			if (Singleton<Game>.Instance.Player.CurrentXPTierIdx >= m_tiers.Length)
			{
				return m_tiers[m_tiers.Length - 1];
			}
			return m_tiers[Singleton<Game>.Instance.Player.CurrentXPTierIdx];
		}
	}

	private void Start()
	{
		Singleton<Game>.Instance.Player.OnCharacterChanged += CharacterChanged;
		Singleton<Game>.Instance.Player.OnLanguageChanged += LanguageChanged;
		Singleton<Game>.Instance.InventoryManager.OnCurrencyChanged += CurrencyChanged;
		SaveManager.OnCloudReceived += CloudReceived;
		SaveManager.OnCloudChanged += CloudChanged;
		Singleton<Game>.Instance.UIController.OnStartLevelUp += StartCheckActions;
		Singleton<Game>.Instance.UIController.OnSetLevelUp += SetCurrentValues;
		m_hasFinishedCountingXP = false;
		m_hasStartedCountingXP = false;
	}

	private void OnDestroy()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.Player != null)
		{
			Singleton<Game>.Instance.Player.OnCharacterChanged -= CharacterChanged;
			Singleton<Game>.Instance.Player.OnLanguageChanged -= LanguageChanged;
		}
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.InventoryManager != null)
		{
			Singleton<Game>.Instance.InventoryManager.OnCurrencyChanged -= CurrencyChanged;
		}
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.UIController != null)
		{
			Singleton<Game>.Instance.UIController.OnStartLevelUp -= StartCheckActions;
			Singleton<Game>.Instance.UIController.OnSetLevelUp -= SetCurrentValues;
		}
		SaveManager.OnCloudReceived -= CloudReceived;
		SaveManager.OnCloudChanged -= CloudChanged;
	}

	private void OnEnable()
	{
	}

	public void StartCheckActions()
	{
		m_hasFinishedCountingXP = false;
		m_hasStartedCountingXP = false;
		CharacterChanged();
		m_onEnable.Invoke();
	}

	private void OnDisable()
	{
		StopCount();
	}

	public void CheckLevelUp()
	{
		m_currentText.SetTarget((int)Mathf.Min(Singleton<Game>.Instance.Player.CurrentXP, Singleton<Game>.Instance.Player.TargetXP));
		if (Singleton<Game>.Instance.Player.CurrentXP > (float)Singleton<Game>.Instance.Player.PreviousXPTarget)
		{
			Singleton<Game>.Instance.UIController.EffectsTop.StartEffect("FocusWipe", EffectComplete);
		}
	}

	public void StartCount()
	{
		m_currentText.SetTarget((int)Mathf.Min(Singleton<Game>.Instance.Player.CurrentXP, Singleton<Game>.Instance.Player.TargetXP));
		m_hasStartedCountingXP = true;
		if (!Singleton<Game>.Instance.UIController.EffectsTop.IsActive)
		{
			m_currentText.StartCount();
		}
	}

	private void EffectComplete()
	{
		m_currentText.StartCount();
	}

	public void StopCount()
	{
		m_currentText.StopCount();
	}

	public void CurrencyChanged(string id, int prev, int curr)
	{
	}

	public void Hide()
	{
	}

	private void LanguageChanged()
	{
		m_nameText.text = Singleton<Game>.Instance.Player.CharacterInfo.Name;
	}

	public void Show()
	{
		m_nameText.text = Singleton<Game>.Instance.Player.CharacterInfo.Name;
		if (Singleton<Game>.Instance.Player.CurrentXPTierIdx >= m_tiers.Length)
		{
			m_onComplete.Invoke();
		}
		else
		{
			m_onStart.Invoke();
		}
		if (CurrentRewardTier != null)
		{
			CurrentRewardTier.m_onStart.Invoke();
			m_levelUpText.text = ScriptLocalization.Get(CurrentRewardTier.m_levelUpString, true);
			m_currentText.SetValue(Singleton<Game>.Instance.Player.PreviousXP);
			UpdateProgressBar(Singleton<Game>.Instance.Player.PreviousXPTarget);
			m_targetText.text = "/" + Singleton<Game>.Instance.Player.PreviousXPTarget;
			m_rewardText.text = CurrentRewardTier.m_reward.ToString();
			Singleton<Game>.Instance.Player.UpdateXP();
		}
	}

	public void CloudReceived()
	{
		object obj = Utils.Parse("Stats.collectible.Character.Current");
		if (obj == null || obj.GetType() != typeof(float))
		{
			Debug.LogError("Level Up parsing error: Stats.collectible.Character.Current");
		}
		else
		{
			m_cloudBackup = (float)obj;
		}
	}

	public void CloudChanged()
	{
		object obj = Utils.Parse("Stats.collectible.Character.Current");
		if (obj == null || obj.GetType() != typeof(float))
		{
			Debug.LogError("Level Up parsing error: Stats.collectible.Character.Current");
			return;
		}
		float num = (float)obj;
		if (num > m_cloudBackup)
		{
			CharacterChanged();
		}
	}

	public void SetCurrentValues()
	{
		if (CurrentRewardTier != null)
		{
			CurrentRewardTier.m_onStart.Invoke();
			m_levelUpText.text = ScriptLocalization.Get(CurrentRewardTier.m_levelUpString, true);
			m_currentText.SetValue(Singleton<Game>.Instance.Player.PreviousXP);
			UpdateProgressBar(Singleton<Game>.Instance.Player.PreviousXPTarget);
			m_targetText.text = "/" + Singleton<Game>.Instance.Player.PreviousXPTarget;
			m_rewardText.text = CurrentRewardTier.m_reward.ToString();
			m_nameText.text = Singleton<Game>.Instance.Player.CharacterInfo.Name;
		}
	}

	public void CharacterChanged()
	{
		if (CurrentRewardTier != null)
		{
			CurrentRewardTier.m_onStart.Invoke();
			m_levelUpText.text = ScriptLocalization.Get(CurrentRewardTier.m_levelUpString, true);
			m_currentText.SetValue(Singleton<Game>.Instance.Player.PreviousXP);
			UpdateProgressBar(Singleton<Game>.Instance.Player.PreviousXPTarget);
			m_targetText.text = "/" + Singleton<Game>.Instance.Player.PreviousXPTarget;
			m_rewardText.text = CurrentRewardTier.m_reward.ToString();
		}
		m_nameText.text = Singleton<Game>.Instance.Player.CharacterInfo.Name;
		if (Singleton<Game>.Instance.Player.CurrentXPTierIdx >= m_tiers.Length)
		{
			m_onComplete.Invoke();
		}
		else
		{
			m_onStart.Invoke();
		}
	}

	public void UpdateProgressBar()
	{
		UpdateProgressBar(Singleton<Game>.Instance.Player.TargetXP);
	}

	private void UpdateProgressBar(float target)
	{
		Player.XPTier currentXPTier = Singleton<Game>.Instance.Player.CurrentXPTier;
		if (currentXPTier != null)
		{
			float num = m_currentText.GetValue();
			float num2 = 0f;
			num2 = num / target;
			float fillAmount = m_progressBar.fillAmount;
			m_progressBar.fillAmount = num2;
			if (m_progressBar.fillAmount >= 1f && fillAmount < 1f)
			{
				StopCount();
				Levelup();
			}
		}
	}

	public void ClaimReward()
	{
		if (Singleton<Game>.Instance.Player.CurrentXPTier != null)
		{
			m_spawnPickups.Spawn(CurrentRewardTier.m_reward);
		}
	}

	public void Levelup()
	{
		m_onLevelUp.Invoke();
	}

	public void AddStar()
	{
		if (CurrentRewardTier != null)
		{
			CurrentRewardTier.m_onLeveup.Invoke();
		}
	}

	public void Update()
	{
		if (m_hasStartedCountingXP && !m_hasFinishedCountingXP && m_currentText.GetTarget() == m_currentText.GetValue() && (float)m_currentText.GetValue() == Singleton<Game>.Instance.Player.CurrentXP)
		{
			m_hasFinishedCountingXP = true;
			m_onLevelUpHasStoppedAnimating.Invoke();
		}
	}

	public void LevelUpComplete()
	{
		Singleton<Game>.Instance.Player.CurrentXPTierIdx++;
		if (Singleton<Game>.Instance.Player.CurrentXPTierIdx >= m_tiers.Length)
		{
			m_onComplete.Invoke();
		}
		if (Singleton<Game>.Instance.Player.CurrentXPTierIdx >= m_tiers.Length - 1)
		{
			Singleton<Game>.Instance.StatsManager.GetStat("CharactersMaxed").IncreaseStat(1f);
		}
		Singleton<Game>.Instance.Player.UpdateXP();
		if (Singleton<Game>.Instance.UIController.EffectsTop.IsActive && Singleton<Game>.Instance.Player.CurrentXP <= Singleton<Game>.Instance.Player.TargetXP)
		{
			Singleton<Game>.Instance.UIController.EffectsTop.EndEffect();
		}
		if (CurrentRewardTier != null)
		{
			CurrentRewardTier.m_onStart.Invoke();
			m_levelUpText.text = ScriptLocalization.Get(CurrentRewardTier.m_levelUpString, true);
			m_currentText.SetValue(0);
			m_currentText.SetTarget((int)Mathf.Min(Singleton<Game>.Instance.Player.CurrentXP, Singleton<Game>.Instance.Player.TargetXP));
			m_currentText.StartCount();
			UpdateProgressBar(Singleton<Game>.Instance.Player.TargetXP);
			m_targetText.text = "/" + (int)Singleton<Game>.Instance.Player.TargetXP;
			m_rewardText.text = CurrentRewardTier.m_reward.ToString();
		}
	}
}
