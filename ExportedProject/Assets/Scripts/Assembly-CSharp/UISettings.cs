using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISettings : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private Toggle m_music;

	[SerializeField]
	private Toggle m_sfx;

	[SerializeField]
	private Toggle m_performance;

	[SerializeField]
	private Toggle m_reduceMotion;

	[SerializeField]
	private Toggle m_inverted;

	[SerializeField]
	public UnityEvent m_onClose = new UnityEvent();

	[SerializeField]
	private Text m_versionText;

	private void OnEnable()
	{
		SetupButtons();
		m_versionText.text = CurrentBundleVersion.version + " - " + Singleton<Game>.Instance.DevGiftManager.ID;
	}

	public void SetupButtons()
	{
		m_music.isOn = Singleton<Game>.Instance.IsMusicEnabled;
		m_sfx.isOn = Singleton<Game>.Instance.IsSFXEnabled;
		m_performance.isOn = !Singleton<Game>.Instance.IsPerformanceModeEnabled;
		m_inverted.isOn = Singleton<Game>.Instance.IsInverted;
		m_reduceMotion.isOn = Singleton<Game>.Instance.Player.IsReduceMotionEnabled;
	}

	public void OnToggleSFX(bool value)
	{
		Singleton<Game>.Instance.IsSFXEnabled = value;
	}

	public void OnToggleMusic(bool value)
	{
		Singleton<Game>.Instance.IsMusicEnabled = value;
	}

	public void OnPrivacyPolicyClicked()
	{
		Application.OpenURL("http://www.prettygreat.com/privacy");
	}

	public void OnFacebookButtonClicked()
	{
		Application.OpenURL("https://www.facebook.com/landslidersgame");
	}

	public void OnTwitterButtonClicked()
	{
		Application.OpenURL("https://www.twitter.com/landslidersgame");
	}

	public void OnRestorePurchasesClicked()
	{
		Singleton<Game>.Instance.InventoryManager.RestorePurchases();
	}

	public void SetLanguage(string language)
	{
		Singleton<Game>.Instance.LocalisationLanguage = language;
	}

	public void OnTogglePerformance(bool value)
	{
		Singleton<Game>.Instance.IsPerformanceModeEnabled = !value;
	}

	public void OnToggleReduceMotion(bool value)
	{
		Singleton<Game>.Instance.Player.IsReduceMotionEnabled = m_reduceMotion.isOn;
	}

	public void OnToggleInverted(bool value)
	{
		Singleton<Game>.Instance.IsInverted = value;
	}

	public void OnSupportClicked()
	{
		Application.OpenURL("mailto:support@prettygreat.com");
	}

	public void OpenURL(string url)
	{
		Application.OpenURL(url);
	}

	public void OpenTwitter(string username)
	{
		Singleton<Game>.Instance.OpenSpecialURL("twitter:///user?screen_name=" + username, "http://www.twitter.com/" + username);
	}

	public void OnCloseClick()
	{
		m_onClose.Invoke();
		SaveManager.Save();
	}

	public void OnDisableScreen()
	{
		Singleton<Game>.Instance.UIController.HideSettings();
	}
}
