using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIAdController : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private Graphic m_coinTravellerSource;

	[SerializeField]
	private Graphic m_coinTravellerSourceUpsell;

	[SerializeField]
	private int m_coinAmount;

	[SerializeField]
	public UnityEvent m_onAvailable = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onCharacterUpsell = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onUnavailable = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onSuccess = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onFail = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onPurchaseCharacterSuccess = new UnityEvent();

	[SerializeField]
	private GameObject m_adContent;

	[SerializeField]
	private UIRenderCharacter m_upsellRenderer;

	[SerializeField]
	private GameObject m_characterUpsellContent;

	[SerializeField]
	private bool m_debugForceUpsell;

	[SerializeField]
	private Text m_characterNameForUpsell;

	[SerializeField]
	private int m_minGamesForCharacterUpsell = 2;

	[SerializeField]
	private int m_maxGamesForCharacterUpsell = 8;

	[SerializeField]
	private int m_purchaseCharacterCoinBonus = 100;

	[Tooltip("If player has ignored video for x games, show upsell instead")]
	[SerializeField]
	private int m_upsellIfVideoIgnored = 4;

	private CharacterInfo m_upsellCharacter;

	private void OnEnable()
	{
		Check();
	}

	public void Unload()
	{
		m_upsellRenderer.Unload();
	}

	private void Start()
	{
		Singleton<Game>.Instance.Player.OnLanguageChanged += LanguageChanged;
	}

	private void OnDestroy()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.Player != null)
		{
			Singleton<Game>.Instance.Player.OnLanguageChanged -= LanguageChanged;
		}
	}

	private void LanguageChanged()
	{
		if (m_upsellCharacter != null)
		{
			string text = m_upsellCharacter.Name + " - ";
			PurchasableItem iAP = Singleton<Game>.Instance.InventoryManager.GetIAP("com.prettygreat.landsliders." + m_upsellCharacter.Id);
			if (iAP != null)
			{
				text += iAP.localizedPriceString;
			}
			m_characterNameForUpsell.text = text;
		}
	}

	public void Check()
	{
		bool flag = false;
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.UIController != null && Singleton<Game>.Instance.UIController.HUD != null)
		{
			flag = Singleton<Game>.Instance.UIController.HUD.InvokeStarterPack();
		}
		if (m_debugForceUpsell)
		{
			SetupCharacterUpsell();
		}
		else if (!flag)
		{
			if (Singleton<Game>.Instance.AdManager.TestUnavailableAdUpsell(m_minGamesForCharacterUpsell, m_maxGamesForCharacterUpsell))
			{
				SetupCharacterUpsell();
			}
			else if (Singleton<Game>.Instance.AdManager.AdsIgnored() >= m_upsellIfVideoIgnored)
			{
				SetupCharacterUpsell();
				Singleton<Game>.Instance.AdManager.WatchedAd();
			}
			else if (Singleton<Game>.Instance.AdManager.IsRewardedVideoReady())
			{
				m_onAvailable.Invoke();
			}
			else
			{
				m_onUnavailable.Invoke();
			}
		}
	}

	public void SetupCharacterUpsell()
	{
		CharacterInfo characterInfo = Singleton<Game>.Instance.Player.GetCharacterInfo(Singleton<Game>.Instance.AdManager.GetPriorityUpsell());
		if (characterInfo == null || !characterInfo.Locked || characterInfo.IsGolden || (!characterInfo.Unlockable && !characterInfo.PurchaseableOnly))
		{
			List<CharacterInfo> list = new List<CharacterInfo>();
			foreach (CharacterInfo unlockableCharacter in Singleton<Game>.Instance.Player.UnlockableCharacters)
			{
				if (unlockableCharacter.Locked && !unlockableCharacter.IsGolden && (unlockableCharacter.Unlockable || unlockableCharacter.PurchaseableOnly))
				{
					list.Add(unlockableCharacter);
				}
			}
			if (list.Count == 0)
			{
				Debug.Log("Nothing to buy!");
				return;
			}
			m_upsellCharacter = list[Random.Range(0, list.Count)];
		}
		else
		{
			m_upsellCharacter = characterInfo;
		}
		m_onCharacterUpsell.Invoke();
		m_upsellRenderer.CharacterId = m_upsellCharacter.Id;
		m_upsellRenderer.Load();
		string text = m_upsellCharacter.Name + " - ";
		PurchasableItem iAP = Singleton<Game>.Instance.InventoryManager.GetIAP("com.prettygreat.landsliders." + m_upsellCharacter.Id);
		if (iAP != null)
		{
			text += iAP.localizedPriceString;
		}
		m_characterNameForUpsell.text = text;
	}

	public void OnBuyCharacter()
	{
		Singleton<Game>.Instance.InventoryManager.PurchaseIAP("com.prettygreat.landsliders." + m_upsellCharacter.Id, PurchaseCharacterCallback);
	}

	public void TestAdIgnored()
	{
		if (m_adContent.activeInHierarchy)
		{
			Singleton<Game>.Instance.AdManager.IgnoredAd();
		}
	}

	public void PurchaseCharacterCallback(bool success, string id)
	{
		if (success)
		{
			Singleton<Game>.Instance.InventoryManager.AddCurrency(m_upsellCharacter.Id, 1);
			Singleton<Game>.Instance.UIController.HUD.SpawnCoinTravellers(m_coinTravellerSourceUpsell, m_purchaseCharacterCoinBonus);
			Singleton<Game>.Instance.Player.SetCharacter(m_upsellCharacter.Id);
			m_onPurchaseCharacterSuccess.Invoke();
			SaveManager.Save();
		}
		else
		{
			string empty = string.Empty;
			string msg = ScriptLocalization.Get("Purchase Failed", true);
			string ok = ScriptLocalization.Get("OK", true);
			NativeDialog.ShowDialog(empty, msg, ok);
		}
	}

	public void ShowAd()
	{
		Singleton<Game>.Instance.AdManager.ShowRewardedVideo(RewardedAdCallback);
	}

	private void RewardedAdCallback(bool result)
	{
		if (result)
		{
			Singleton<Game>.Instance.UIController.HUD.SpawnCoinTravellers(m_coinTravellerSource, m_coinAmount);
			m_onSuccess.Invoke();
		}
		else
		{
			m_onFail.Invoke();
		}
	}
}
