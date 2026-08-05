using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICollectionBonus : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private int m_bonusAmount;

	[SerializeField]
	private Text m_bonusText;

	[SerializeField]
	private Image m_bonusIcon;

	[SerializeField]
	private Image m_coinIcon;

	[SerializeField]
	private Sprite m_coinImage;

	[SerializeField]
	private UIPopup m_popup;

	[SerializeField]
	private Text m_remaining;

	[SerializeField]
	private UnityEvent m_onCollectAll = new UnityEvent();

	private bool m_rewardCollected;

	private List<Pickup> m_pickups = new List<Pickup>();

	private void Start()
	{
		Singleton<Game>.Instance.LevelManager.OnStartLevelLoadComplete += StartLevelLoadComplete;
		Singleton<Game>.Instance.LevelManager.OnLevelLoadComplete += LevelLoadComplete;
		Singleton<Game>.Instance.InventoryManager.OnCurrencyChanged += CurrencyChanged;
	}

	private void StartLevelLoadComplete()
	{
		LevelLoadComplete();
	}

	private void LevelLoadComplete()
	{
		m_pickups.Clear();
		m_rewardCollected = false;
		Transform transform = Singleton<Game>.Instance.LevelManager.CurrentLevel.transform;
		Pickup[] componentsInChildren = transform.GetComponentsInChildren<Pickup>();
		Pickup[] array = componentsInChildren;
		foreach (Pickup pickup in array)
		{
			if (pickup.CurrencyId == Singleton<Game>.Instance.LevelManager.CurrentLevel.m_objectOfDesire && !(pickup.transform.position.y > 30f))
			{
				m_pickups.Add(pickup);
			}
		}
		m_bonusText.text = "+" + Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Bonus Maps On Collect All", m_bonusAmount);
		if (Singleton<Game>.Instance.LevelManager.CurrentLevel.m_objectOfDesire == "coin")
		{
			m_bonusIcon.sprite = m_coinImage;
			m_coinIcon.sprite = m_coinImage;
		}
		else
		{
			GameObject replacementPickup = Singleton<Game>.Instance.Player.CharacterInfo.ReplacementPickup;
			m_bonusIcon.sprite = replacementPickup.GetComponent<Pickup>().Icon;
			m_coinIcon.sprite = replacementPickup.GetComponent<Pickup>().Icon;
		}
		if (m_remaining != null)
		{
			m_remaining.text = m_pickups.Count.ToString();
		}
	}

	public void CurrencyChanged(string id, int prev, int curr)
	{
		if (!Singleton<Game>.Instance.LevelManager.IsNormalLevel || !(id == Singleton<Game>.Instance.LevelManager.CurrentLevel.m_objectOfDesire) || m_rewardCollected)
		{
			return;
		}
		int num = 0;
		foreach (Pickup pickup in m_pickups)
		{
			if (pickup != null && !pickup.IsCollected)
			{
				num++;
			}
		}
		if (num <= 0 && !Singleton<Game>.Instance.Player.BlockInput)
		{
			m_onCollectAll.Invoke();
			m_rewardCollected = true;
			Singleton<Game>.Instance.UIController.HUD.AllPickupsCollected();
			Singleton<Game>.Instance.UIController.HUD.AddPopup(null, null, m_popup);
		}
		if (m_remaining != null)
		{
			int num2 = Mathf.Max(num, 0);
			m_remaining.text = num2.ToString();
		}
	}

	public void RewardBonus()
	{
		Singleton<Game>.Instance.InventoryManager.AddCurrency(Singleton<Game>.Instance.LevelManager.CurrentLevel.m_objectOfDesire, Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Bonus Maps On Collect All", Singleton<Game>.Instance.LevelManager.CurrentLevel.m_bonusObjectsOnComplete));
	}
}
