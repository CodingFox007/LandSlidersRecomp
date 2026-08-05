using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
public class Achievement
{
	public enum Condition
	{
		EQUALS = 0,
		GREATER_THAN = 1,
		LESS_THAN = 2,
		GREATER_THAN_OR_EQUAL = 3,
		LESS_THAN_OR_EQUAL = 4
	}

	public enum Location
	{
		InGame = 0,
		LevelPass = 1,
		LevelStart = 2,
		Loading = 3
	}

	public enum ResetLocation
	{
		None = 0,
		LevelLoadComplete = 1,
		StartNewLife = 2
	}

	[Serializable]
	public class AchievementCheck
	{
		public string m_paramater;

		public Condition m_condition;

		public float m_target;

		[HideInInspector]
		public float m_startValue;

		public Location m_location;

		public ResetLocation m_resetLocation;

		private bool m_passed;

		public bool Passed
		{
			get
			{
				return m_passed;
			}
		}

		private float RawValue
		{
			get
			{
				object obj = Utils.Parse(m_paramater);
				if (obj == null)
				{
					Debug.LogError("Achievement parsing error: " + m_paramater);
					return 0f;
				}
				if (obj.GetType() != typeof(float))
				{
					Debug.LogError("Achievement parsing error: " + m_paramater);
					return 0f;
				}
				return (float)obj;
			}
		}

		public float Target
		{
			get
			{
				return m_target;
			}
		}

		public float Current
		{
			get
			{
				return RawValue - m_startValue;
			}
		}

		public float Remaining
		{
			get
			{
				return Target - Current;
			}
		}

		public float RemainingAboveZero
		{
			get
			{
				return Mathf.Max(0f, Target - Current);
			}
		}

		public void Init()
		{
			m_passed = false;
			m_startValue = RawValue;
			if (Singleton<Game>.Instance.SocialManager.LoggingEnabled)
			{
				Debug.Log("Check Init = " + m_paramater + ": " + Current + " -> " + Target + " (" + m_startValue + ")");
			}
		}

		public void Check()
		{
			switch (m_condition)
			{
			case Condition.EQUALS:
				m_passed = Current == Target;
				break;
			case Condition.GREATER_THAN:
				m_passed = Current > Target;
				break;
			case Condition.GREATER_THAN_OR_EQUAL:
				m_passed = Current >= Target;
				break;
			case Condition.LESS_THAN:
				m_passed = Current < Target;
				break;
			case Condition.LESS_THAN_OR_EQUAL:
				m_passed = Current <= Target;
				break;
			}
			if (Singleton<Game>.Instance.SocialManager.LoggingEnabled)
			{
				Debug.Log("Check " + m_paramater + " = " + m_passed + "  (" + Current + " -> " + Target + ")");
			}
		}
	}

	[Serializable]
	public class AchievementProgress
	{
		public string m_paramater;

		public float m_target;

		[HideInInspector]
		public float m_startValue;

		public float Progress
		{
			get
			{
				if (m_paramater == null || m_paramater == string.Empty)
				{
					return 0f;
				}
				return (RawValue - m_startValue) / m_target;
			}
		}

		public float RawValue
		{
			get
			{
				if (m_paramater == null || m_paramater == string.Empty)
				{
					return 0f;
				}
				object obj = Utils.Parse(m_paramater);
				if (obj == null)
				{
					Debug.LogError("Achievement parsing error: " + m_paramater);
					return 0f;
				}
				if (obj.GetType() != typeof(float))
				{
					Debug.LogError("Achievement parsing error: " + m_paramater);
					return 0f;
				}
				return (float)obj;
			}
		}

		public void Init()
		{
			m_startValue = RawValue;
		}
	}

	public string m_id;

	public string m_gameCenterId;

	public string m_androidId;

	public List<AchievementCheck> m_checks;

	public AchievementProgress m_progress;

	private bool m_passed;

	public IAchievement m_achievement;

	public bool Passed
	{
		get
		{
			return m_passed;
		}
	}

	public float Progress
	{
		get
		{
			return m_progress.Progress;
		}
	}

	public void Init()
	{
		if (Singleton<Game>.Instance.SocialManager.LoggingEnabled)
		{
			Debug.Log("Achievement Init = " + m_id);
		}
		m_passed = false;
		foreach (AchievementCheck check in m_checks)
		{
			check.Init();
		}
		m_progress.Init();
	}

	public void Check()
	{
		if (m_passed)
		{
			return;
		}
		bool flag = true;
		foreach (AchievementCheck check in m_checks)
		{
			check.Check();
			if (!check.Passed)
			{
				flag = false;
			}
		}
		if (flag)
		{
			if (Singleton<Game>.Instance.SocialManager.LoggingEnabled)
			{
				Debug.Log("Achievement Passed");
			}
			m_passed = true;
		}
	}

	public void LevelLoadComplete()
	{
		foreach (AchievementCheck check in m_checks)
		{
			if (check.m_resetLocation == ResetLocation.LevelLoadComplete)
			{
				check.Init();
			}
		}
	}

	public void StartNewLife()
	{
		foreach (AchievementCheck check in m_checks)
		{
			if (check.m_resetLocation == ResetLocation.StartNewLife)
			{
				check.Init();
			}
		}
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("passed", m_passed);
		jSONObject.AddField("progress", m_progress.m_startValue);
		JSONObject jSONObject2 = new JSONObject();
		int num = 0;
		foreach (AchievementCheck check in m_checks)
		{
			jSONObject2.AddField(num.ToString(), check.m_startValue);
			num++;
		}
		jSONObject.AddField("checks", jSONObject2);
		return jSONObject;
	}

	public void Load(JSONObject data)
	{
		m_passed = data.GetField("passed").AsBool;
		if (data.HasField("progress"))
		{
			m_progress.Init();
			m_progress.m_startValue = data.GetField("progress").AsFloat;
		}
		JSONObject field = data.GetField("checks");
		int num = 0;
		foreach (AchievementCheck check in m_checks)
		{
			check.Init();
			if (field.HasField(num.ToString()))
			{
				check.m_startValue = field.GetField(num.ToString()).AsFloat;
			}
			num++;
		}
	}
}
