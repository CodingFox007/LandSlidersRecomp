using System;
using System.Collections.Generic;
using ArabicSupport;
using I2.Loc;
using UnityEngine;

public class MissionManager : MonoBehaviour, ISerializationCallbackReceiver, Savable
{
	[Serializable]
	public class MissionReward
	{
		public Mission.Difficulty m_difficulty;

		public string m_currencyId;

		public int m_currencyAmount;

		public Sprite m_icon;

		public string m_flavourText;
	}

	public delegate void MissionEvent(Mission mission);

	[SerializeField]
	private List<Mission> m_missions;

	[SerializeField]
	private MissionReward[] m_rewards;

	[SerializeField]
	private int m_tutorialMissionsRequired = 3;

	private int m_tutorialMissionsCompleted;

	private Mission m_activeMission;

	private Mission m_previousMission;

	private Mission.Location m_location = Mission.Location.LevelStart;

	private bool m_loggingEnabled;

	public bool ActiveMission
	{
		get
		{
			return m_activeMission != null;
		}
	}

	public string ActiveMissionId
	{
		get
		{
			if (m_activeMission == null)
			{
				return string.Empty;
			}
			return m_activeMission.m_id;
		}
	}

	public Mission.Location CurrentLocation
	{
		get
		{
			return m_location;
		}
		set
		{
			m_location = value;
		}
	}

	public List<Mission> Missions
	{
		get
		{
			return m_missions;
		}
	}

	public int MissionCount
	{
		get
		{
			return m_missions.Count;
		}
	}

	public int CurrentMissionIdx
	{
		get
		{
			if (m_activeMission == null)
			{
				return -1;
			}
			return m_missions.IndexOf(m_activeMission);
		}
	}

	public string CurrentMissionId
	{
		get
		{
			if (m_activeMission == null)
			{
				return null;
			}
			return m_activeMission.m_id;
		}
	}

	public MissionReward CurrentMissionReward
	{
		get
		{
			if (m_activeMission == null)
			{
				return null;
			}
			return GetMissionReward(m_activeMission.m_difficulty);
		}
	}

	public bool LoggingEnabled
	{
		get
		{
			return m_loggingEnabled;
		}
		set
		{
			m_loggingEnabled = value;
		}
	}

	public event MissionEvent OnMissionChanged;

	public event MissionEvent OnMissionComplete;

	public event MissionEvent OnMissionCancelled;

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
	}

	public void Init()
	{
		Singleton<Game>.Instance.LevelManager.OnLevelLoadComplete += LevelLoadComplete;
		Singleton<Game>.Instance.LevelManager.OnStartNewLife += StartNewLife;
	}

	private void LevelLoadComplete()
	{
		if (m_activeMission != null)
		{
			m_activeMission.LevelLoadComplete();
		}
	}

	private void StartNewLife()
	{
		if (m_activeMission != null)
		{
			m_activeMission.StartNewLife();
		}
	}

	public int SelectRandomMissionIdx()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < m_missions.Count; i++)
		{
			if (m_tutorialMissionsCompleted < m_tutorialMissionsRequired)
			{
				if (m_missions[i].m_difficulty == Mission.Difficulty.Tutorial)
				{
					list.Add(i);
				}
			}
			else if (m_previousMission == null || m_previousMission.m_tag != m_missions[i].m_tag)
			{
				list.Add(i);
			}
		}
		if (list.Count <= 0)
		{
			return 0;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public void SelectRandomMission()
	{
		int idx = SelectRandomMissionIdx();
		SetMision(idx);
	}

	public void SetMision(int idx, bool fireEvents = true)
	{
		if (idx < 0 || idx >= m_missions.Count)
		{
			return;
		}
		m_activeMission = m_missions[idx];
		if (m_activeMission != null)
		{
			m_activeMission.Init();
			if (this.OnMissionChanged != null && fireEvents)
			{
				this.OnMissionChanged(m_activeMission);
			}
		}
	}

	public void CancelMission()
	{
		if (this.OnMissionCancelled != null)
		{
			this.OnMissionCancelled(m_activeMission);
		}
		if (this.OnMissionChanged != null)
		{
			this.OnMissionChanged(null);
		}
		m_previousMission = m_activeMission;
		m_activeMission = null;
	}

	public void ForceComplete()
	{
		if (m_activeMission != null)
		{
			if (m_activeMission.m_difficulty == Mission.Difficulty.Tutorial)
			{
				m_tutorialMissionsCompleted++;
			}
			Singleton<Game>.Instance.UIController.HUD.MissionPassed();
			if (this.OnMissionComplete != null)
			{
				this.OnMissionComplete(m_activeMission);
			}
			if (this.OnMissionChanged != null)
			{
				this.OnMissionChanged(null);
			}
			Singleton<Game>.Instance.StatsManager.GetStat("MissionsPassed").IncreaseStat(1f);
			m_previousMission = m_activeMission;
			m_activeMission = null;
		}
	}

	private void Update()
	{
		if (m_activeMission == null)
		{
			return;
		}
		bool passed = m_activeMission.Passed;
		m_activeMission.Check();
		UpdateDescription();
		if (!passed && m_activeMission.Passed)
		{
			Singleton<Game>.Instance.UIController.HUD.MissionPassed();
			if (m_activeMission.m_difficulty == Mission.Difficulty.Tutorial)
			{
				m_tutorialMissionsCompleted++;
			}
			if (this.OnMissionComplete != null)
			{
				this.OnMissionComplete(m_activeMission);
			}
			if (this.OnMissionChanged != null)
			{
				this.OnMissionChanged(null);
			}
			Singleton<Game>.Instance.StatsManager.GetStat("MissionsPassed").IncreaseStat(1f);
			m_previousMission = m_activeMission;
			m_activeMission = null;
		}
		if (m_location != Mission.Location.InGame)
		{
			m_location = Mission.Location.Loading;
		}
	}

	private void UpdateDescription()
	{
		string[] popupText;
		Sprite[] icons;
		GetPopupText(out popupText, out icons);
		Singleton<Game>.Instance.UIController.HUD.SetupMissionPopup(popupText, icons, m_activeMission.Passed);
	}

	public void GetPopupText(out string[] popupText, out Sprite[] icons)
	{
		MissionReward currentMissionReward = CurrentMissionReward;
		popupText = new string[3];
		icons = new Sprite[1];
		string format = ScriptLocalization.Get("Missions/" + m_activeMission.m_id);
		if (LocalizationManager.IsRight2Left)
		{
			LocalizationManager.IsRight2Left = false;
			popupText[0] = ArabicFixer.Fix(string.Format(format, Utils.Parse(m_activeMission.m_description.m_textInputs)));
			LocalizationManager.IsRight2Left = true;
		}
		else
		{
			popupText[0] = string.Format(format, Utils.Parse(m_activeMission.m_description.m_textInputs));
		}
		popupText[1] = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Coins Quests", currentMissionReward.m_currencyAmount).ToString();
		popupText[2] = ScriptLocalization.Get(currentMissionReward.m_flavourText, true);
		icons[0] = currentMissionReward.m_icon;
	}

	public void GetMissionDescription(int missionIdx, out string[] popupText, out Sprite[] icons)
	{
		Mission activeMission = m_activeMission;
		SetMision(missionIdx, false);
		GetPopupText(out popupText, out icons);
		m_activeMission = activeMission;
	}

	public MissionReward GetMissionReward(Mission.Difficulty difficulty)
	{
		MissionReward[] rewards = m_rewards;
		foreach (MissionReward missionReward in rewards)
		{
			if (missionReward.m_difficulty == difficulty)
			{
				return missionReward;
			}
		}
		return null;
	}

	public void CollectReward(int missionIdx)
	{
		if (missionIdx >= 0 && missionIdx < m_missions.Count)
		{
			Mission mission = m_missions[missionIdx];
			MissionReward missionReward = GetMissionReward(mission.m_difficulty);
			if (missionReward.m_currencyId == "coin")
			{
				Singleton<Game>.Instance.UIController.HUD.SpawnCoinTravellers(Singleton<Game>.Instance.UIController.HUD.MissionRewardIcon, Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Coins Quests", missionReward.m_currencyAmount));
			}
			else
			{
				Singleton<Game>.Instance.InventoryManager.AddCurrency(missionReward.m_currencyId, missionReward.m_currencyAmount);
			}
		}
	}

	public object Parse()
	{
		object result = null;
		if (Utils.ParseTokenCount() < 3)
		{
			return result;
		}
		if (Utils.ParseToken(1).StartsWith("Check", StringComparison.Ordinal))
		{
			Mission.MissionCheck missionCheck = null;
			int num = int.Parse(Utils.ParseToken(1).Replace("Check", string.Empty));
			if (num >= 0 && num < m_activeMission.m_checks.Count)
			{
				missionCheck = m_activeMission.m_checks[num];
			}
			if (missionCheck == null)
			{
				return result;
			}
			switch (Utils.ParseToken(2))
			{
			case "Target":
				result = missionCheck.Target;
				break;
			case "Remaining":
				result = missionCheck.Remaining;
				break;
			case "RemainingAboveZero":
				result = missionCheck.RemainingAboveZero;
				break;
			case "Current":
				result = missionCheck.Current;
				break;
			}
		}
		return result;
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		jSONObject.AddField("tutorials", m_tutorialMissionsCompleted);
		return jSONObject;
	}

	public void Load(JSONObject data)
	{
		if (data.HasField("tutorials"))
		{
			m_tutorialMissionsCompleted = data.GetField("tutorials").AsInt;
		}
	}

	public bool Verify(JSONObject data)
	{
		return true;
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
		m_tutorialMissionsCompleted = 0;
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
		return "Missions";
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
}
