using UnityEngine;
using UnityEngine.Events;

public class OnMissionActive : MonoBehaviour
{
	[SerializeField]
	private string[] m_missionIds;

	[SerializeField]
	private UnityEvent m_onActive = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onInactive = new UnityEvent();

	private bool m_active;

	private void OnEnable()
	{
		bool flag = false;
		if (m_missionIds == null || m_missionIds.Length == 0)
		{
			flag = Singleton<Game>.Instance.MissionManager.ActiveMission;
		}
		else
		{
			string[] missionIds = m_missionIds;
			foreach (string text in missionIds)
			{
				if (Singleton<Game>.Instance.MissionManager.ActiveMissionId == text)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			m_onActive.Invoke();
			m_active = true;
		}
		else
		{
			m_onInactive.Invoke();
			m_active = false;
		}
		Singleton<Game>.Instance.MissionManager.OnMissionChanged += MissionChanged;
	}

	private void OnDisable()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.MissionManager != null)
		{
			Singleton<Game>.Instance.MissionManager.OnMissionChanged -= MissionChanged;
		}
	}

	private void MissionChanged(Mission mission)
	{
		bool flag = false;
		if (m_missionIds == null || m_missionIds.Length == 0)
		{
			flag = Singleton<Game>.Instance.MissionManager.ActiveMission;
		}
		else
		{
			string[] missionIds = m_missionIds;
			foreach (string text in missionIds)
			{
				if (Singleton<Game>.Instance.MissionManager.ActiveMissionId == text)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			if (!m_active)
			{
				m_onActive.Invoke();
				m_active = true;
			}
		}
		else if (m_active)
		{
			m_onInactive.Invoke();
			m_active = false;
		}
	}
}
