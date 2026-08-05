using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Mission
{
	public enum MissionTag
	{
		Collect = 0,
		Kill = 1,
		Chase = 2,
		Perform = 3
	}

	public enum Difficulty
	{
		Easy = 0,
		Medium = 1,
		Hard = 2,
		Extreme = 3,
		Tutorial = 4
	}

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
	public class MissionCheck
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
					Debug.LogError("Mission parsing error: " + m_paramater);
					return 0f;
				}
				if (obj.GetType() != typeof(float))
				{
					Debug.LogError("Mission parsing error: " + m_paramater);
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
			if (Singleton<Game>.Instance.MissionManager.LoggingEnabled)
			{
				Debug.Log("Check Init = " + m_paramater + ": " + Current + " -> " + Target + " (" + m_startValue + ")");
			}
		}

		public void Check()
		{
			if (Singleton<Game>.Instance.MissionManager.CurrentLocation == m_location)
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
				if (Singleton<Game>.Instance.MissionManager.LoggingEnabled)
				{
					Debug.Log("Check " + m_paramater + " = " + m_passed + "  (" + Current + " -> " + Target + ")");
				}
			}
		}
	}

	[Serializable]
	public class Description
	{
		[Multiline]
		public string m_text;

		[SerializeField]
		public string[] m_textInputs;
	}

	[Serializable]
	public class HUDHelper
	{
		public int m_checkIdx;
	}

	public string m_id;

	public Difficulty m_difficulty;

	public MissionTag m_tag;

	[Range(0f, 1f)]
	public float m_chance;

	public List<MissionCheck> m_checks;

	public Description m_description;

	public HUDHelper m_hudHelper;

	private bool m_passed;

	private int m_passCount;

	public bool Passed
	{
		get
		{
			return m_passed;
		}
	}

	public void Init()
	{
		if (Singleton<Game>.Instance.MissionManager.LoggingEnabled)
		{
			Debug.Log("Mission Init = " + m_id);
		}
		m_passed = false;
		foreach (MissionCheck check in m_checks)
		{
			check.Init();
		}
	}

	public void Check()
	{
		if (m_passed)
		{
			return;
		}
		foreach (MissionCheck check in m_checks)
		{
			check.Check();
		}
		bool flag = true;
		foreach (MissionCheck check2 in m_checks)
		{
			if (!check2.Passed)
			{
				flag = false;
			}
		}
		if (flag)
		{
			if (Singleton<Game>.Instance.MissionManager.LoggingEnabled)
			{
				Debug.Log("Mission Passed");
			}
			m_passed = true;
			m_passCount++;
		}
	}

	public void LevelLoadComplete()
	{
		foreach (MissionCheck check in m_checks)
		{
			if (check.m_resetLocation == ResetLocation.LevelLoadComplete)
			{
				check.Init();
			}
		}
	}

	public void StartNewLife()
	{
		foreach (MissionCheck check in m_checks)
		{
			if (check.m_resetLocation == ResetLocation.StartNewLife)
			{
				check.Init();
			}
		}
	}
}
