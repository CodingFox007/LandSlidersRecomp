using UnityEngine;
using UnityEngine.Events;

public class TriggerOnStat : MonoBehaviour
{
	public enum ConditionType
	{
		LessThan = 0,
		LessThanEquals = 1,
		Equals = 2,
		DoesntEqual = 3,
		GreaterThan = 4,
		GreaterThanEquals = 5
	}

	[SerializeField]
	private string m_stat;

	[SerializeField]
	private ConditionType m_conditionType = ConditionType.Equals;

	[SerializeField]
	private float m_value;

	[SerializeField]
	private bool m_triggerIfNotSet;

	[SerializeField]
	private bool m_triggerOnce = true;

	private int m_triggerCount;

	[SerializeField]
	private bool m_checkOnce;

	private int m_checkCount;

	[SerializeField]
	private UnityEvent m_onTrigger = new UnityEvent();

	private void Update()
	{
		if (m_checkOnce && m_checkCount > 0)
		{
			return;
		}
		m_checkCount++;
		if (m_triggerOnce && m_triggerCount > 0)
		{
			return;
		}
		bool flag = false;
		if (!Singleton<Game>.Instance.StatsManager.HasStat(m_stat))
		{
			if (m_triggerIfNotSet)
			{
				flag = true;
			}
		}
		else
		{
			Stat stat = Singleton<Game>.Instance.StatsManager.GetStat(m_stat);
			if (stat != null)
			{
				switch (m_conditionType)
				{
				case ConditionType.DoesntEqual:
					if (stat.Total.Current != m_value)
					{
						flag = true;
					}
					break;
				case ConditionType.Equals:
					if (stat.Total.Current == m_value)
					{
						flag = true;
					}
					break;
				case ConditionType.GreaterThan:
					if (stat.Total.Current > m_value)
					{
						flag = true;
					}
					break;
				case ConditionType.GreaterThanEquals:
					if (stat.Total.Current >= m_value)
					{
						flag = true;
					}
					break;
				case ConditionType.LessThan:
					if (stat.Total.Current < m_value)
					{
						flag = true;
					}
					break;
				case ConditionType.LessThanEquals:
					if (stat.Total.Current <= m_value)
					{
						flag = true;
					}
					break;
				}
			}
		}
		if (flag)
		{
			m_triggerCount++;
			m_onTrigger.Invoke();
		}
	}
}
