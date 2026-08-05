using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
	[SerializeField]
	private Toggle m_music;

	[SerializeField]
	private Toggle m_sfx;

	[SerializeField]
	private Toggle m_inverted;

	[SerializeField]
	private Text m_debugText;

	public void TimeStop()
	{
		Time.timeScale = 0f;
		m_music.isOn = Singleton<Game>.Instance.IsMusicEnabled;
		m_sfx.isOn = Singleton<Game>.Instance.IsSFXEnabled;
		m_inverted.isOn = Singleton<Game>.Instance.IsInverted;
		if (Singleton<Game>.Instance.MissionManager.ActiveMission)
		{
			Singleton<Game>.Instance.UIController.HUD.MissionShow();
		}
	}

	public void TimeStart()
	{
		Time.timeScale = 1f;
		if (Singleton<Game>.Instance.MissionManager.ActiveMission)
		{
			Singleton<Game>.Instance.UIController.HUD.MissionHide();
		}
	}

	public void Update()
	{
		if (m_debugText != null && Singleton<Game>.Instance.IsShippingBuild)
		{
			m_debugText.gameObject.SetActive(false);
		}
	}

	public void OnToggleSFX(bool value)
	{
		Singleton<Game>.Instance.IsSFXEnabled = value;
	}

	public void OnToggleMusic(bool value)
	{
		Singleton<Game>.Instance.IsMusicEnabled = value;
	}

	public void OnKillSelf()
	{
		Singleton<Game>.Instance.Player.Kill(null, true);
		Singleton<Game>.Instance.UIController.HidePause();
	}

	public void OnToggleInverted(bool value)
	{
		Singleton<Game>.Instance.IsInverted = value;
	}

	public void OnResumeClick()
	{
		SaveManager.Save(Singleton<Game>.Instance.Player);
		Singleton<Game>.Instance.UIController.HidePause();
	}

	public void OnReplayKitClick()
	{
		ReplayKit.Instance.StartRecording();
	}
}
