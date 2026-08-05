using UnityEngine;

public class UITileButton : MonoBehaviour
{
	public void OnLeaderboardClick()
	{
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.Retry)
		{
			Singleton<Game>.Instance.SocialManager.ShowLeaderboards();
		}
	}

	public void OnAchievementClick()
	{
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.Retry)
		{
			Singleton<Game>.Instance.SocialManager.ShowAchievements();
		}
	}

	public void OnSocialClick()
	{
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.Retry)
		{
			Singleton<Game>.Instance.UIController.ShowSharing();
		}
	}

	public void OnSpecialEventClick()
	{
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.InGame)
		{
			Singleton<Game>.Instance.SpecialEventManager.GiveCharacter();
			Singleton<Game>.Instance.UIController.ShowSpecialEvent();
			Time.timeScale = 0f;
		}
	}

	public void OnSpecialEventHeavenClick()
	{
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.Retry)
		{
			Singleton<Game>.Instance.UIController.ShowSpecialEvent();
		}
	}

	public void OnShopClick()
	{
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.Retry)
		{
			Singleton<Game>.Instance.UIController.ShowCharacterUnlock();
		}
	}

	public void OnMissionClick()
	{
		if (!Singleton<Game>.Instance.MissionManager.ActiveMission)
		{
			Singleton<Game>.Instance.UIController.HUD.MissionAcceptShow();
		}
	}

	public void OnMissionUnclick()
	{
		Singleton<Game>.Instance.UIController.HUD.MissionAcceptHide();
	}

	public void OnSettingsClick()
	{
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.Retry)
		{
			Singleton<Game>.Instance.UIController.ShowSettings();
		}
	}

	public void OnCharacterSelect()
	{
		Singleton<Game>.Instance.UIController.ShowCharacterSelect();
	}

	public void OnShowPrizeSelect()
	{
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.Retry)
		{
			Singleton<Game>.Instance.UIController.ShowPrizeSelect();
		}
	}
}
