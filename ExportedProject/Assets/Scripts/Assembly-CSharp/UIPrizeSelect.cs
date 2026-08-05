using System;
using System.Collections.Generic;
using HeavyDutyInspector;
using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPrizeSelect : MonoBehaviour, IEventSystemHandler, ISerializationCallbackReceiver
{
	[SerializeField]
	private UIPrizeSelectItem[] m_prizeItems;

	private UIPrizeSelectItem m_selectedItem;

	[SerializeField]
	private Text m_rewardText;

	[SerializeField]
	private Graphic m_coinSource;

	[SerializeField]
	private Text m_timerText;

	[Multiline]
	[SerializeField]
	private string m_notificationBody;

	[Multiline]
	[SerializeField]
	private string m_notificationReminderBody;

	[SerializeField]
	private string m_notificationAction;

	[SerializeField]
	private int[] m_remindsInHours;

	[Dictionary("m_prizesValues")]
	public List<int> m_prizes;

	[HideInInspector]
	public List<float> m_prizesValues;

	private Dictionary<int, float> m_prize;

	[SerializeField]
	public UnityEvent m_onReset = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onClose = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onPlay = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onShowPrize = new UnityEvent();

	private int m_reward;

	private bool m_needsSave;

	public UIPrizeSelectItem SelectedItem
	{
		get
		{
			return m_selectedItem;
		}
	}

	public bool PrizeAvailable
	{
		get
		{
			if (Singleton<Game>.Instance.EventManager.IsEventGroupActive("Prizes"))
			{
				return true;
			}
			return false;
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_prizes, m_prizesValues, out m_prize, "prizes");
	}

	private void OnEnable()
	{
		m_onReset.Invoke();
		m_selectedItem = null;
		m_reward = 0;
	}

	private void Update()
	{
		TimeSpan timeSpan = Singleton<Game>.Instance.EventManager.EventGroupAvailableIn("Prizes");
		m_timerText.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public void SetSelectedItem(UIPrizeSelectItem item)
	{
		m_selectedItem = item;
		UIPrizeSelectItem[] prizeItems = m_prizeItems;
		foreach (UIPrizeSelectItem uIPrizeSelectItem in prizeItems)
		{
			if (uIPrizeSelectItem != m_selectedItem)
			{
				uIPrizeSelectItem.UnselectItem();
			}
			else
			{
				uIPrizeSelectItem.UnlockItem();
			}
		}
	}

	public void OnPlayClick()
	{
		m_onPlay.Invoke();
	}

	public void OnCloseClick()
	{
		m_onClose.Invoke();
	}

	public void OnDisableScreen()
	{
		if (m_needsSave)
		{
			SaveManager.Save();
			m_needsSave = false;
		}
		Singleton<Game>.Instance.UIController.HidePrizeSelect();
	}

	public void OnSetNotification()
	{
		TimeSpan timeSpan = Singleton<Game>.Instance.EventManager.EventGroupAvailableIn("Prizes");
		DateTime time = DateTime.Now + timeSpan;
		Singleton<NotificationManager>.Instance.EnableNotifications();
		Singleton<NotificationManager>.Instance.SetNotification(ScriptLocalization.Get(m_notificationBody, true), ScriptLocalization.Get(m_notificationAction, true), time);
		m_onClose.Invoke();
	}

	public void ShowPrize()
	{
		Singleton<NotificationManager>.Instance.ClearNotifications();
		m_reward = GetRandomReward();
		m_rewardText.text = m_reward.ToString();
		m_onShowPrize.Invoke();
		EventManager.Event activeEventInGroup = Singleton<Game>.Instance.EventManager.GetActiveEventInGroup("Prizes");
		if (activeEventInGroup != null)
		{
			Singleton<Game>.Instance.EventManager.UseEvent(activeEventInGroup);
		}
		TimeSpan timeSpan = Singleton<Game>.Instance.EventManager.EventGroupAvailableIn("Prizes");
		m_timerText.text = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		m_needsSave = true;
	}

	public void SpawnCoins()
	{
		Singleton<Game>.Instance.UIController.HUD.SpawnCoinTravellers(m_coinSource, m_reward);
	}

	private int GetRandomReward()
	{
		if (m_prize.Count <= 0)
		{
			return 0;
		}
		float num = 0f;
		foreach (float value in m_prize.Values)
		{
			float num2 = value;
			num += num2;
		}
		float num3 = UnityEngine.Random.Range(0f, num);
		num = 0f;
		foreach (KeyValuePair<int, float> item in m_prize)
		{
			num += item.Value;
			if (num >= num3)
			{
				return item.Key;
			}
		}
		return 0;
	}
}
