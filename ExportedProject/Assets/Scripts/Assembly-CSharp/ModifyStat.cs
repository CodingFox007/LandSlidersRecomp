using UnityEngine;

public class ModifyStat : MonoBehaviour
{
	private enum ModifyType
	{
		Increase = 0,
		Decrease = 1,
		SetValue = 2
	}

	[SerializeField]
	private string m_stat;

	[SerializeField]
	private ModifyType m_modifyType;

	[SerializeField]
	private float m_value;

	[SerializeField]
	private bool m_triggerOnce = true;

	private int m_triggerCount;

	[SerializeField]
	private bool m_saveImmediately;

	public void Trigger()
	{
		if (m_triggerOnce && m_triggerCount > 0)
		{
			return;
		}
		m_triggerCount++;
		Stat stat = Singleton<Game>.Instance.StatsManager.GetStat(m_stat);
		if (stat != null)
		{
			switch (m_modifyType)
			{
			case ModifyType.Increase:
				stat.IncreaseStat(m_value);
				break;
			case ModifyType.Decrease:
				stat.DecreaseStat(m_value);
				break;
			case ModifyType.SetValue:
				stat.SetValue(m_value);
				break;
			}
			if (m_saveImmediately)
			{
				SaveManager.Save();
			}
		}
	}
}
