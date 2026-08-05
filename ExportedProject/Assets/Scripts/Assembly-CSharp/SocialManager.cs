using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SocialManager : MonoBehaviour, Savable
{
	[Serializable]
	public class Leaderboard
	{
		public string m_id;

		public string m_androidId;

		public string m_stat;

		public ILeaderboard m_Leaderboard;
	}

	[SerializeField]
	private Leaderboard[] m_leaderboards;

	[SerializeField]
	private Achievement[] m_achievements;

	private Achievement.Location m_location = Achievement.Location.LevelStart;

	private bool m_loggingEnabled;

	[SerializeField]
	private int m_framesBetweenChecks = 10;

	private int m_currentFrame;

	private int m_lastCheckFrame;

	private int m_currentAchievementToCheckIdx;

	private Achievement achievementToCheck
	{
		get
		{
			return m_achievements[m_currentAchievementToCheckIdx % m_achievements.Length];
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

	public int AchievementCount
	{
		get
		{
			return m_achievements.Length;
		}
	}

	public Achievement.Location CurrentLocation
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

	public Achievement GetAchievementAt(int idx)
	{
		if (idx < 0 || idx >= m_achievements.Length)
		{
			return null;
		}
		return m_achievements[idx];
	}

	private void Start()
	{
		PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
		PlayGamesPlatform.InitializeInstance(configuration);
		PlayGamesPlatform.Activate();
		Singleton<Game>.Instance.LevelManager.OnLevelLoadComplete += LevelLoadComplete;
		Singleton<Game>.Instance.LevelManager.OnStartNewLife += StartNewLife;
		if (!PlayerPrefs.HasKey("cancelledAuthentication"))
		{
			Social.localUser.Authenticate(ProcessAuthentication);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && !PlayerPrefs.HasKey("cancelledAuthentication"))
		{
			Social.localUser.Authenticate(ProcessAuthentication);
		}
	}

	public void ShowLeaderboards()
	{
		if (!Social.Active.localUser.authenticated)
		{
			Social.localUser.Authenticate(ShowLeaderboardsAfterSignin);
		}
		else
		{
			Social.ShowLeaderboardUI();
		}
	}

	public void ShowAchievements()
	{
		if (!Social.Active.localUser.authenticated)
		{
			Social.localUser.Authenticate(ShowAchievementsAfterSignin);
		}
		else
		{
			Social.ShowAchievementsUI();
		}
	}

	public void ShowAchievementsAfterSignin(bool success)
	{
		if (success)
		{
			CreateLeaderboards();
			CreateAchievements();
			Social.ShowAchievementsUI();
			PlayerPrefs.DeleteKey("cancelledAuthentication");
		}
	}

	public void ShowLeaderboardsAfterSignin(bool success)
	{
		if (success)
		{
			CreateLeaderboards();
			CreateAchievements();
			Social.ShowLeaderboardUI();
			PlayerPrefs.DeleteKey("cancelledAuthentication");
		}
	}

	public void CheckLeaderboards()
	{
		Leaderboard[] leaderboards = m_leaderboards;
		foreach (Leaderboard leaderboard in leaderboards)
		{
			if (leaderboard.m_Leaderboard != null)
			{
				object obj = Utils.Parse(leaderboard.m_stat);
				if (obj == null || obj.GetType() != typeof(float))
				{
					Debug.LogError("Parsing Error " + leaderboard.m_stat);
				}
				float num = (float)obj;
				ReportScore((long)num, leaderboard.m_Leaderboard.id);
			}
		}
	}

	public void CheckAchievementProgress()
	{
		Achievement[] achievements = m_achievements;
		foreach (Achievement achievement in achievements)
		{
			if (achievement.m_achievement != null)
			{
				ReportAchievementProgress(achievement, (double)achievement.Progress * 100.0);
			}
		}
	}

	public void DebugLogAchievementProgress()
	{
		Achievement[] achievements = m_achievements;
		foreach (Achievement achievement in achievements)
		{
			double num = (double)achievement.Progress * 100.0;
			Debug.Log(achievement.m_id + " progress = " + num);
		}
	}

	private void LevelLoadComplete()
	{
		Achievement[] achievements = m_achievements;
		foreach (Achievement achievement in achievements)
		{
			achievement.LevelLoadComplete();
		}
	}

	private void StartNewLife()
	{
		Achievement[] achievements = m_achievements;
		foreach (Achievement achievement in achievements)
		{
			achievement.StartNewLife();
		}
	}

	public void ResetAllAchievements()
	{
		Reset();
		SaveManager.Save();
	}

	public void ReportAchievementProgress(Achievement ach, double progress, bool onlyIfDifferent = true)
	{
		if (ach.m_achievement == null || (onlyIfDifferent && ach.m_achievement.percentCompleted == progress))
		{
			return;
		}
		ach.m_achievement.percentCompleted = progress;
		ach.m_achievement.ReportProgress(delegate(bool result)
		{
			if (result)
			{
				Debug.Log("Achievement " + ach.m_id + ": Successfully reported progress - Update");
			}
			else
			{
				Debug.Log("Achievement " + ach.m_id + ": Failed to report progress - Update");
			}
		});
	}

	private void Update()
	{
		m_currentFrame++;
		if (m_currentFrame > m_lastCheckFrame + m_framesBetweenChecks)
		{
			m_lastCheckFrame = m_currentFrame;
			Achievement achievement = achievementToCheck;
			m_currentAchievementToCheckIdx++;
			while (achievementToCheck.Passed && achievementToCheck != achievement)
			{
				m_currentAchievementToCheckIdx++;
			}
			achievementToCheck.Check();
			if (achievementToCheck.Passed)
			{
				Debug.Log("Achievement " + achievementToCheck.m_id + " Passed!");
				ReportAchievementProgress(achievementToCheck, 100.0);
			}
		}
		if (m_location != Achievement.Location.InGame)
		{
			m_location = Achievement.Location.Loading;
		}
	}

	private int GetNextUnPassedIdx(int idx)
	{
		for (int i = idx; i < m_achievements.Length; i++)
		{
			int num = (int)Mathf.Repeat(i, m_achievements.Length);
			if (!m_achievements[num].Passed)
			{
				return num;
			}
		}
		return -1;
	}

	private void ProcessAuthentication(bool success)
	{
		if (!success)
		{
			PlayerPrefs.SetInt("cancelledAuthentication", 1);
			Debug.Log("Failed to authenticate with Game Center.");
			return;
		}
		Debug.Log("Authenticated");
		CloudAndroid.LoadFromCloud();
		CreateLeaderboards();
		CreateAchievements();
		LoadProgressFromDevice();
	}

	private void ReportScore(long score, string leaderboardID)
	{
		if (m_loggingEnabled)
		{
			Debug.Log("Reporting score " + score + " on leaderboard " + leaderboardID);
		}
		Social.ReportScore(score, leaderboardID, delegate(bool success)
		{
			Debug.Log((!success) ? "Failed to report score" : "Reported score to leaderboard successfully");
		});
	}

	private void CreateLeaderboards()
	{
		Leaderboard[] leaderboards = m_leaderboards;
		foreach (Leaderboard leaderboard in leaderboards)
		{
			if (leaderboard.m_Leaderboard == null)
			{
				leaderboard.m_Leaderboard = Social.CreateLeaderboard();
				leaderboard.m_Leaderboard.id = leaderboard.m_androidId;
				leaderboard.m_Leaderboard.LoadScores(delegate
				{
				});
			}
		}
	}

	private void CreateAchievements()
	{
		Achievement[] achievements = m_achievements;
		foreach (Achievement achievement in achievements)
		{
			if (achievement.m_achievement == null)
			{
				achievement.m_achievement = Social.CreateAchievement();
				achievement.m_achievement.id = achievement.m_androidId;
				ReportAchievementProgress(achievement, 0.0, false);
			}
		}
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		JSONObject jSONObject2 = new JSONObject();
		Achievement[] achievements = m_achievements;
		foreach (Achievement achievement in achievements)
		{
			jSONObject2.AddField(achievement.m_id, achievement.Save());
		}
		jSONObject.AddField("achievements", jSONObject2);
		return jSONObject;
	}

	public bool Verify(JSONObject data)
	{
		return true;
	}

	public void Load(JSONObject data)
	{
		if (!data.HasField("achievements"))
		{
			return;
		}
		JSONObject field = data.GetField("achievements");
		foreach (string key in field.keys)
		{
			Achievement achievement = null;
			Achievement[] achievements = m_achievements;
			foreach (Achievement achievement2 in achievements)
			{
				if (achievement2.m_id == key)
				{
					achievement = achievement2;
				}
			}
			if (achievement == null)
			{
				Debug.LogError("Achievement not found: " + key);
				continue;
			}
			achievement.Init();
			achievement.Load(field[key]);
		}
	}

	public void LoadProgressFromDevice()
	{
		Social.LoadAchievements(delegate(IAchievement[] achievements)
		{
			if (achievements.Length > 0)
			{
				Achievement[] achievements2 = m_achievements;
				foreach (Achievement achievement in achievements2)
				{
					bool flag = false;
					foreach (IAchievement achievement2 in achievements)
					{
						if (achievement.m_androidId.ToLower() == achievement2.id.ToLower())
						{
							flag = true;
							if (achievement.Passed && achievement2.percentCompleted != 100.0)
							{
								ReportAchievementProgress(achievement, 100.0, false);
							}
							break;
						}
					}
					if (!flag && achievement.Passed)
					{
						ReportAchievementProgress(achievement, 100.0, false);
					}
				}
			}
		});
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
		Achievement[] achievements = m_achievements;
		foreach (Achievement achievement in achievements)
		{
			achievement.Init();
		}
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
		return "Social";
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
