using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICharacterUnlock : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private int m_unlockCost = 100;

	[SerializeField]
	private string m_unlockCurrency = "coin";

	[SerializeField]
	private UICharacterUnlockItem[] m_unlockItems;

	private UICharacterUnlockItem m_itemSelected;

	private UICharacterUnlockItem m_unlockDuplicate;

	[SerializeField]
	private UIRenderCharacter m_characterRenderer;

	[SerializeField]
	private Transform m_case;

	[SerializeField]
	private Text m_casePrice;

	[SerializeField]
	private Transform m_stand;

	[SerializeField]
	private Text m_nameText;

	[SerializeField]
	private Image m_pickupIcon;

	[SerializeField]
	private GameObject m_buyCaseButton;

	[SerializeField]
	private GameObject m_coinPurchaseButton;

	[SerializeField]
	private GameObject m_tokenPurchaseButton;

	[SerializeField]
	private GameObject m_playButton;

	[SerializeField]
	private GameObject m_buyAgainButton;

	[SerializeField]
	private GameObject m_nextButton;

	[SerializeField]
	private GameObject m_backButton;

	[SerializeField]
	public UnityEvent m_onReset = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onClose = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onPlay = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onBuyAgain = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onShare = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onFocusItem = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onShowPrize = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onShowPrizeDuplicate = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onBuySuccess = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onBuyFail = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onNotEnoughCash = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onitemSelected = new UnityEvent();

	public float m_shimmerAnimationSpeed = 15f;

	public Material[] m_shimmerMaterials;

	public float[] m_shimmerOffsets;

	public float m_shimmerWait = 3f;

	[SerializeField]
	public UnityEvent m_triggerFullscreenSparkle = new UnityEvent();

	[SerializeField]
	public UnityEvent m_untriggerFullscreenSparkle = new UnityEvent();

	[SerializeField]
	private int m_unquieCount = 2;

	[SerializeField]
	private int m_unquieCountStart = 4;

	private int m_unquieCounter;

	[SerializeField]
	private int m_startRandomCount = 14;

	[SerializeField]
	private float m_unquieRandomChance = 0.2f;

	[SerializeField]
	private Text m_unlockedCountText;

	[Tooltip("How many of the force override characters *must* be won by the player before resuming normal lottery?")]
	public int m_forceOverrideCount;

	[Tooltip("Force Override Count worth of these characters *must* be won by the player before normal lottery resumes.  Fields are uniqueID from characterInfo.")]
	public string[] m_forceOverrideCharacterLottery;

	[SerializeField]
	private float m_goldChance = 0.1f;

	private bool m_needsSave;

	private bool m_forceGolden;

	private bool m_forceUnique;

	public UICharacterUnlockItem SelectedItem
	{
		get
		{
			return m_itemSelected;
		}
	}

	public void Update()
	{
		if (Singleton<Game>.Instance.InventoryManager.GetCurrency("boxToken") > 0)
		{
			float t = Time.time * m_shimmerAnimationSpeed;
			t = Mathf.Repeat(t, m_shimmerWait + 1f);
			for (int i = 0; i < m_shimmerMaterials.Length; i++)
			{
				Material material = m_shimmerMaterials[i];
				if (t + m_shimmerOffsets[i] < 0f)
				{
					material.SetTextureOffset("_MainTex", new Vector2(Mathf.Min(1f, 1f), 0f));
				}
				else
				{
					material.SetTextureOffset("_MainTex", new Vector2(Mathf.Min(1f, t + m_shimmerOffsets[i]), 0f));
				}
			}
		}
		else
		{
			Material[] shimmerMaterials = m_shimmerMaterials;
			foreach (Material material2 in shimmerMaterials)
			{
				material2.SetTextureOffset("_MainTex", new Vector2(1f, 0f));
			}
		}
	}

	private void OnEnable()
	{
		if (!Singleton<Game>.Instance.IsLoaded)
		{
			return;
		}
		m_characterRenderer.Load();
		bool flag = Singleton<Game>.Instance.InventoryManager.GetCurrency("boxToken") > 0;
		for (int i = 0; i < 9; i++)
		{
			UICharacterUnlockItem uICharacterUnlockItem = m_unlockItems[i];
			bool flag2 = true;
			if (flag)
			{
				int openedBoxes = CharacterUnlockCaseSaveData.Instance.m_openedBoxes;
				if ((openedBoxes & (1 << i)) != 0)
				{
					flag2 = false;
				}
			}
			uICharacterUnlockItem.gameObject.SetActive(flag2);
			uICharacterUnlockItem.SetAvailable(flag2);
		}
		m_buyCaseButton.SetActive(!flag);
		m_coinPurchaseButton.SetActive(!flag);
		m_tokenPurchaseButton.SetActive(flag);
		m_backButton.SetActive(!flag);
		m_onReset.Invoke();
		if (flag)
		{
			m_triggerFullscreenSparkle.Invoke();
		}
		m_itemSelected = null;
		JSONObject jSONObject = SaveManager.LoadJSON("CharacterUnlock");
		if (jSONObject != null && jSONObject.HasField("unquieCounter"))
		{
			m_unquieCounter = jSONObject.GetField("unquieCounter").AsInt;
		}
		int num = 0;
		foreach (CharacterInfo unlockableCharacter in Singleton<Game>.Instance.Player.UnlockableCharacters)
		{
			if (!unlockableCharacter.Locked)
			{
				num++;
			}
		}
		if (num <= 1)
		{
			m_unquieCounter = m_unquieCountStart;
		}
		if (Singleton<Game>.Instance.InventoryManager.GetCurrency(m_unlockCurrency) < m_unlockCost)
		{
			m_onNotEnoughCash.Invoke();
		}
		SetUnlockCountText();
		PurchasableItem iAP = Singleton<Game>.Instance.InventoryManager.GetIAP("com.prettygreat.landsliders.characterCase");
		if (iAP != null)
		{
			m_casePrice.text = iAP.localizedPriceString;
		}
	}

	private void OnDisable()
	{
		m_characterRenderer.Unload();
	}

	public void SetSelectedItem(UICharacterUnlockItem item)
	{
		if (m_itemSelected != null)
		{
			m_itemSelected.UnselectItem();
		}
		m_itemSelected = item;
		m_onitemSelected.Invoke();
	}

	public bool CanAfford()
	{
		return Singleton<Game>.Instance.InventoryManager.GetCurrency(m_unlockCurrency) >= m_unlockCost;
	}

	public void SetUnlockCountText()
	{
		int num = 0;
		int num2 = 0;
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			if (availableCharacter.IsGolden)
			{
				if (!availableCharacter.Locked)
				{
					num2++;
					num++;
				}
			}
			else
			{
				if (!availableCharacter.Locked)
				{
					num2++;
				}
				num++;
			}
		}
		m_unlockedCountText.text = num2 + "/" + num;
	}

	public void OnBuyCase()
	{
		Singleton<Game>.Instance.InventoryManager.PurchaseIAP("com.prettygreat.landsliders.characterCase", PurchaseCaseCallback);
	}

	public void PurchaseCaseCallback(bool success, string id)
	{
		if (success)
		{
			Singleton<Game>.Instance.InventoryManager.AddCurrency("boxToken", 9);
			UICharacterUnlockItem[] unlockItems = m_unlockItems;
			foreach (UICharacterUnlockItem uICharacterUnlockItem in unlockItems)
			{
				uICharacterUnlockItem.SetAvailable(true);
			}
			if (m_buyCaseButton != null)
			{
				m_buyCaseButton.SetActive(false);
			}
			m_triggerFullscreenSparkle.Invoke();
			m_tokenPurchaseButton.SetActive(true);
			m_tokenPurchaseButton.GetComponent<Animator>().SetTrigger("ShowButton");
			m_coinPurchaseButton.GetComponent<Animator>().SetTrigger("HideButton");
			m_backButton.SetActive(false);
			CharacterUnlockCaseSaveData.Instance.m_goldUnlock = Random.Range(0, 9);
			CharacterUnlockCaseSaveData.Instance.m_openedBoxes = 0;
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

	public void OnBuyClick()
	{
		m_untriggerFullscreenSparkle.Invoke();
		bool flag = false;
		if (Singleton<Game>.Instance.InventoryManager.GetCurrency("boxToken") == 0)
		{
			if (!CanAfford())
			{
				m_onBuyFail.Invoke();
				return;
			}
			Singleton<Game>.Instance.InventoryManager.SpendCurrency(m_unlockCurrency, m_unlockCost);
		}
		else
		{
			Debug.Log("Had " + Singleton<Game>.Instance.InventoryManager.GetCurrency("boxToken") + " tokens");
			flag = true;
			Singleton<Game>.Instance.InventoryManager.SpendCurrency("boxToken", 1);
		}
		if (m_itemSelected == null)
		{
			UICharacterUnlockItem randomItem = GetRandomItem();
			SetSelectedItem(randomItem);
		}
		int num = -1;
		for (int i = 0; i < m_unlockItems.Length; i++)
		{
			if (m_unlockItems[i] == m_itemSelected)
			{
				num = i;
			}
		}
		m_forceGolden = false;
		m_forceUnique = false;
		if (flag)
		{
			m_itemSelected.SetAvailable(false);
			int openedBoxes = CharacterUnlockCaseSaveData.Instance.m_openedBoxes;
			openedBoxes |= 1 << num;
			CharacterUnlockCaseSaveData.Instance.m_openedBoxes = openedBoxes;
			m_forceGolden = num == CharacterUnlockCaseSaveData.Instance.m_goldUnlock;
			m_forceUnique = num != CharacterUnlockCaseSaveData.Instance.m_goldUnlock;
		}
		m_unlockDuplicate = Object.Instantiate(m_itemSelected);
		m_unlockDuplicate.transform.SetParent(m_stand);
		m_unlockDuplicate.transform.localPosition = m_itemSelected.transform.localPosition;
		m_unlockDuplicate.transform.localRotation = m_itemSelected.transform.localRotation;
		m_unlockDuplicate.transform.localScale = m_itemSelected.transform.localScale;
		m_unlockDuplicate.UnlockItem();
		m_itemSelected.UnselectItem();
		m_itemSelected.gameObject.SetActive(false);
		m_itemSelected = null;
		bool flag2 = Singleton<Game>.Instance.InventoryManager.GetCurrency("boxToken") > 0;
		m_playButton.SetActive(!flag2);
		m_buyAgainButton.SetActive(!flag2);
		m_nextButton.SetActive(flag2);
		m_backButton.SetActive(!flag2);
		m_onBuySuccess.Invoke();
	}

	public void OnBuyAgain()
	{
		if (m_unlockDuplicate != null)
		{
			Object.Destroy(m_unlockDuplicate.gameObject);
			m_unlockDuplicate = null;
		}
		if (m_itemSelected != null)
		{
			m_itemSelected.gameObject.SetActive(m_itemSelected.IsAvailable());
			m_itemSelected.UnselectItem();
		}
		m_itemSelected = null;
		bool flag = Singleton<Game>.Instance.InventoryManager.GetCurrency("boxToken") > 0;
		m_buyCaseButton.SetActive(!flag);
		m_coinPurchaseButton.SetActive(!flag);
		m_tokenPurchaseButton.SetActive(flag);
		if (flag)
		{
			m_triggerFullscreenSparkle.Invoke();
		}
		if (!flag)
		{
			UICharacterUnlockItem[] unlockItems = m_unlockItems;
			foreach (UICharacterUnlockItem uICharacterUnlockItem in unlockItems)
			{
				uICharacterUnlockItem.gameObject.SetActive(true);
				uICharacterUnlockItem.SetAvailable(true);
			}
		}
		SetUnlockCountText();
		m_onBuyAgain.Invoke();
	}

	public void OnShareClick()
	{
		m_onShare.Invoke();
	}

	public void OnPlayClick()
	{
		Singleton<Game>.Instance.Player.IsRandomCharacter = false;
		Singleton<Game>.Instance.Player.SetCharacter(m_characterRenderer.CharacterId);
		m_onPlay.Invoke();
		if (m_unlockDuplicate != null)
		{
			Object.Destroy(m_unlockDuplicate.gameObject);
		}
		if (m_itemSelected != null)
		{
			Renderer[] componentsInChildren = m_itemSelected.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = true;
			}
		}
		m_needsSave = true;
	}

	public void OnCloseClick()
	{
		if (Singleton<Game>.Instance.InventoryManager.GetCurrency("boxToken") > 0)
		{
			return;
		}
		m_untriggerFullscreenSparkle.Invoke();
		m_onClose.Invoke();
		if (m_unlockDuplicate != null)
		{
			Object.Destroy(m_unlockDuplicate.gameObject);
		}
		if (m_itemSelected != null)
		{
			Renderer[] componentsInChildren = m_itemSelected.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = true;
			}
		}
	}

	public void ShowPrize()
	{
		CharacterInfo characterInfo = null;
		bool flag = false;
		int num = 0;
		foreach (CharacterInfo unlockableCharacter in Singleton<Game>.Instance.Player.UnlockableCharacters)
		{
			if (!unlockableCharacter.Locked)
			{
				num++;
			}
		}
		if (num >= m_startRandomCount)
		{
			flag = Random.Range(0f, 1f) < m_unquieRandomChance;
		}
		else if (m_unquieCounter > 0)
		{
			flag = true;
			m_unquieCounter--;
		}
		else
		{
			flag = false;
		}
		if (m_forceUnique)
		{
			flag = true;
			m_forceUnique = false;
		}
		characterInfo = SelectRandomCharacter(flag);
		if (!characterInfo.Locked)
		{
			m_unquieCounter = m_unquieCount;
		}
		bool flag2 = !characterInfo.Locked;
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("unquieCounter", m_unquieCounter);
		SaveManager.SaveJSON("CharacterUnlock", jSONObject);
		Singleton<Game>.Instance.InventoryManager.AddCurrency(characterInfo.Id, 1);
		m_characterRenderer.SetCharacter(characterInfo.Id, true);
		m_nameText.text = characterInfo.Name;
		GameObject replacementPickup = characterInfo.ReplacementPickup;
		m_pickupIcon.sprite = replacementPickup.GetComponent<Pickup>().Icon;
		if (flag2)
		{
			m_onShowPrizeDuplicate.Invoke();
		}
		else
		{
			m_onShowPrize.Invoke();
		}
		SaveManager.Save();
		m_needsSave = true;
	}

	public void FocusItem()
	{
		m_onFocusItem.Invoke();
	}

	private UICharacterUnlockItem GetRandomItem()
	{
		List<UICharacterUnlockItem> list = new List<UICharacterUnlockItem>();
		UICharacterUnlockItem[] unlockItems = m_unlockItems;
		foreach (UICharacterUnlockItem uICharacterUnlockItem in unlockItems)
		{
			if (uICharacterUnlockItem.IsAvailable())
			{
				list.Add(uICharacterUnlockItem);
			}
		}
		if (list.Count == 0)
		{
			Debug.LogWarning("No boxes to choose from, reset not working after buying all boxes");
			return m_unlockItems[Random.Range(0, m_unlockItems.Length)];
		}
		return list[Random.Range(0, list.Count)];
	}

	private CharacterInfo SelectRandomCharacter(bool mustBeLocked = false)
	{
		List<CharacterInfo> list = new List<CharacterInfo>();
		int forceOverrideCount = m_forceOverrideCount;
		int num = 0;
		if (m_forceOverrideCount > m_forceOverrideCharacterLottery.Length)
		{
			Debug.LogError("Want player to have " + m_forceOverrideCount + " characters from a list that only has " + m_forceOverrideCharacterLottery.Length + " items...not using overrides");
		}
		else
		{
			string[] forceOverrideCharacterLottery = m_forceOverrideCharacterLottery;
			foreach (string text in forceOverrideCharacterLottery)
			{
				CharacterInfo characterInfo = Singleton<Game>.Instance.Player.GetCharacterInfo(text);
				if (characterInfo == null)
				{
					Debug.LogError("Could not find character " + text);
				}
				else if (characterInfo.Locked)
				{
					list.Add(characterInfo);
				}
				else
				{
					num++;
				}
			}
		}
		List<CharacterInfo> list2 = new List<CharacterInfo>();
		if (m_forceGolden)
		{
			foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
			{
				if (availableCharacter.IsGolden && !(availableCharacter.Id == Singleton<Game>.Instance.Player.CharacterId) && availableCharacter.Unlockable && availableCharacter.Locked)
				{
					list2.Add(availableCharacter);
				}
			}
			if (list2.Count == 0)
			{
				foreach (CharacterInfo availableCharacter2 in Singleton<Game>.Instance.Player.AvailableCharacters)
				{
					if (availableCharacter2.IsGolden)
					{
						list2.Add(availableCharacter2);
					}
				}
			}
			m_forceGolden = false;
		}
		else if (num >= forceOverrideCount)
		{
			if (Random.Range(0f, 1f) < m_goldChance)
			{
				foreach (CharacterInfo availableCharacter3 in Singleton<Game>.Instance.Player.AvailableCharacters)
				{
					if (availableCharacter3.IsGolden && !(availableCharacter3.Id == Singleton<Game>.Instance.Player.CharacterId) && availableCharacter3.Unlockable && (!mustBeLocked || availableCharacter3.Locked))
					{
						list2.Add(availableCharacter3);
					}
				}
			}
			if (list2.Count <= 0)
			{
				foreach (CharacterInfo unlockableCharacter in Singleton<Game>.Instance.Player.UnlockableCharacters)
				{
					if (!(unlockableCharacter.Id == Singleton<Game>.Instance.Player.CharacterId) && (!mustBeLocked || unlockableCharacter.Locked))
					{
						list2.Add(unlockableCharacter);
					}
				}
			}
		}
		else
		{
			list2 = list;
		}
		if (list2.Count <= 0)
		{
			return Singleton<Game>.Instance.Player.UnlockableCharacters[Random.Range(0, Singleton<Game>.Instance.Player.UnlockableCharacters.Count)];
		}
		return list2[Random.Range(0, list2.Count)];
	}

	public void OnDisableScreen()
	{
		if (m_needsSave)
		{
			SaveManager.Save();
			m_needsSave = false;
		}
		Singleton<Game>.Instance.UIController.HideCharacterUnlock();
	}
}
