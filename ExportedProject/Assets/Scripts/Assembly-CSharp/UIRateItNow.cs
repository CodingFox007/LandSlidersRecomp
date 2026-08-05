using System;
using I2.Loc;
using NativeAlert;
using UnityEngine;

public class UIRateItNow : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void AttemptRateItNow()
	{
		if (DateTime.UtcNow.Ticks <= RateItNowSaveData.Instance.m_timeToRemind)
		{
			return;
		}
		int num = 0;
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			if (!availableCharacter.Locked)
			{
				num++;
			}
		}
		Stat stat = Singleton<Game>.Instance.StatsManager.GetStat("collectible");
		float current2 = stat.Life.Current;
		float highest = stat.Life.Highest;
		if (num >= 2 && (double)current2 > (double)highest * 0.75 && Singleton<Game>.Instance.UIController.CurrentState != UIController.State.Social)
		{
			RateItNow();
		}
	}

	public void OnDialogButtonPressed(string message)
	{
		int num = int.Parse(message);
		if (num == 0)
		{
			string text = "1022667752";
			Singleton<Game>.Instance.OpenSpecialURL("itms-apps://itunes.apple.com/app/id" + text, "https://itunes.apple.com/" + text + "/lookup");
			RateItNowSaveData.Instance.SetRemindTime(long.MaxValue);
		}
		if (num == 1)
		{
			RateItNowSaveData.Instance.SetRemindTime(long.MaxValue);
		}
		if (num == 2)
		{
		}
		SaveManager.Save();
	}

	public void RateItNow()
	{
		RateItNowSaveData.Instance.SetRemindTime(DateTime.Now.AddDays(2.0).Ticks);
		RateItNowSaveData.Instance.m_timesPresented++;
		string title = ScriptLocalization.Get("Rate It Now Title", true);
		string message = ScriptLocalization.Get("Rate It Now Message", true);
		string btn = ScriptLocalization.Get("Rate It Now Ok Android", true);
		string btn2 = ScriptLocalization.Get("Rate It Now Later", true);
		string btn3 = ScriptLocalization.Get("Rate It Now Never", true);
		AndroidNativeAlert.ShowAlert(title, message, btn, btn2, btn3);
	}
}
