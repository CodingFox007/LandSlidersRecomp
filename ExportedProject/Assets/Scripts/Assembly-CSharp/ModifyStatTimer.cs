using UnityEngine;

public class ModifyStatTimer : MonoBehaviour
{
	[SerializeField]
	private string m_statTimer;

	public void Trigger()
	{
		Stat statTimer = Singleton<Game>.Instance.StatsManager.GetStatTimer(m_statTimer);
		if (statTimer != null)
		{
			statTimer.IsActive = true;
		}
	}

	public void UnTrigger()
	{
		Stat statTimer = Singleton<Game>.Instance.StatsManager.GetStatTimer(m_statTimer);
		if (statTimer != null)
		{
			statTimer.IsActive = false;
		}
	}
}
