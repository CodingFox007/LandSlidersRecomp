using System;
using System.Collections;
using I2.Loc;
using Unibill.Impl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UICharacterSelect : MonoBehaviour, IEventSystemHandler
{
	[Serializable]
	public class CrossPromoCharacter
	{
		public string m_characterId;

		public string m_text;

		[FormerlySerializedAs("m_url")]
		public string m_urlIOS;

		public string m_urlAndroid;
	}

	[SerializeField]
	private UIRenderCharacter m_characterRenderer;

	[SerializeField]
	private UIRenderPickup m_selectedPickup;

	[SerializeField]
	private UIRenderPickup m_pickupRendererPrefab;

	[SerializeField]
	private ScrollRect m_scrollRect;

	[SerializeField]
	private RectTransform m_scrollListContainer;

	[SerializeField]
	private RectTransform m_scrollBounds;

	[SerializeField]
	private Text m_nameText;

	[SerializeField]
	private Image[] m_stars;

	[SerializeField]
	private Button m_selectButton;

	[SerializeField]
	private Button m_buyButton;

	[SerializeField]
	private Graphic m_specialUnlock;

	[SerializeField]
	private Text m_iapText;

	[SerializeField]
	private Text m_unlockText;

	[SerializeField]
	private GameObject m_statsContainer;

	[SerializeField]
	private Text m_unlockedCountText;

	[SerializeField]
	private Text m_unlockedTotalCountText;

	[SerializeField]
	private Image m_loadingRing;

	[SerializeField]
	private CrossPromoCharacter[] m_crossPromoCharacters;

	[SerializeField]
	private Graphic m_crossPromoButton;

	[SerializeField]
	private Text m_categoryText;

	[SerializeField]
	private Text m_crossPromoText;

	[SerializeField]
	public UnityEvent m_onCharacterChange = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onCharacterSelected = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onCharacterUnlocked = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onClose = new UnityEvent();

	[SerializeField]
	private UIPerkController m_perkController;

	private bool m_needsSave;

	private int m_currentFilter;

	private float m_ohlordVert;

	private void AddCharacter(CharacterInfo character)
	{
		GameObject replacementPickup = character.ReplacementPickup;
		Pickup component = replacementPickup.GetComponent<Pickup>();
		UIRenderPickup uIRenderPickup = Utils.CreateFromPrefab(m_pickupRendererPrefab, character.Id);
		uIRenderPickup.IsGolden = character.IsGolden;
		uIRenderPickup.SetPickup(component);
		uIRenderPickup.IsVisible = false;
		uIRenderPickup.transform.SetParent(m_scrollListContainer, false);
		UIScrollListScale component2 = uIRenderPickup.GetComponent<UIScrollListScale>();
		component2.SetBounds(m_scrollBounds);
	}

	private void Start()
	{
		AddCharacter(Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter());
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			if (!availableCharacter.IsGolden || !availableCharacter.Locked)
			{
				AddCharacter(availableCharacter);
			}
		}
	}

	public void SnapToPickup()
	{
	}

	public void SetFilter(int filter)
	{
		m_currentFilter = filter;
		for (int i = 0; i < m_scrollListContainer.transform.childCount; i++)
		{
			UIRenderPickup component = m_scrollListContainer.GetChild(i).GetComponent<UIRenderPickup>();
			if (component.name == Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter().Id)
			{
				component.gameObject.SetActive(true);
			}
			else
			{
				CharacterInfo characterInfo = Singleton<Game>.Instance.Player.GetCharacterInfo(component.name);
				if (characterInfo.m_group == (CharacterInfo.Group)filter)
				{
					component.gameObject.SetActive(true);
				}
				else
				{
					component.gameObject.SetActive(false);
				}
			}
			component.transform.GetChild(0).gameObject.SetActive(false);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(m_scrollListContainer.transform.GetComponent<RectTransform>());
		m_characterRenderer.GetComponent<RawImage>().enabled = false;
		switch (filter)
		{
		case 0:
			m_categoryText.gameObject.GetComponent<Localize>().SetTerm("People");
			break;
		case 1:
			m_categoryText.gameObject.GetComponent<Localize>().SetTerm("Animals");
			break;
		case 2:
			m_categoryText.gameObject.GetComponent<Localize>().SetTerm("Things");
			break;
		case 3:
			m_categoryText.gameObject.GetComponent<Localize>().SetTerm("Seasonal");
			break;
		}
		if (Singleton<Game>.Instance.Player.IsRandomCharacter)
		{
			GameObject replacementPickup = Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter().ReplacementPickup;
			Pickup component2 = replacementPickup.GetComponent<Pickup>();
			SnapToPickup(component2, Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter().Id);
			SetCharacter(Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter());
		}
		else if (Singleton<Game>.Instance.Player.CharacterInfo.m_group == (CharacterInfo.Group)filter)
		{
			GameObject replacementPickup2 = Singleton<Game>.Instance.Player.CharacterInfo.ReplacementPickup;
			Pickup component3 = replacementPickup2.GetComponent<Pickup>();
			SnapToPickup(component3, Singleton<Game>.Instance.Player.CharacterId);
			SetCharacter(Singleton<Game>.Instance.Player.CharacterInfo);
		}
		else
		{
			for (int j = 1; j < m_scrollListContainer.transform.childCount; j++)
			{
				if (m_scrollListContainer.GetChild(j).gameObject.activeSelf)
				{
					UIRenderPickup component4 = m_scrollListContainer.GetChild(j).GetComponent<UIRenderPickup>();
					CharacterInfo characterInfo2 = Singleton<Game>.Instance.Player.GetCharacterInfo(component4.name);
					SnapToPickup(component4.Prefab, characterInfo2.Id);
					SetCharacter(characterInfo2);
					break;
				}
			}
		}
		SetUnlockCountText();
	}

	public void SnapToPickup(Pickup pickupPrefab, string name)
	{
		int num = -1;
		int num2 = 0;
		for (int i = 0; i < m_scrollListContainer.childCount; i++)
		{
			UIRenderPickup component = m_scrollListContainer.GetChild(i).GetComponent<UIRenderPickup>();
			if (component != null)
			{
				if (component.Prefab == pickupPrefab && name == component.name)
				{
					num = num2;
				}
				if (component.Prefab != null && m_scrollListContainer.GetChild(i).gameObject.activeSelf)
				{
					num2++;
				}
			}
		}
		if (num != -1)
		{
			float num3 = (float)num / (float)(num2 - 1);
			m_scrollRect.verticalNormalizedPosition = 1f - num3;
			StartCoroutine(FocusOnCoroutine(1f - num3));
		}
	}

	private IEnumerator FocusOnCoroutine(float x)
	{
		m_scrollRect.verticalNormalizedPosition = x;
		yield return new WaitForEndOfFrame();
		m_scrollRect.verticalNormalizedPosition = x;
		yield return new WaitForEndOfFrame();
		for (int i = 0; i < m_scrollListContainer.transform.childCount; i++)
		{
			m_scrollListContainer.transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(true);
		}
		m_characterRenderer.GetComponent<RawImage>().enabled = true;
		m_scrollRect.verticalNormalizedPosition = x;
		m_ohlordVert = x;
	}

	private void Update()
	{
		if (m_characterRenderer.Prefab == null)
		{
			m_loadingRing.enabled = true;
		}
		else
		{
			m_loadingRing.enabled = false;
		}
		VerticalLayoutGroup component = m_scrollListContainer.GetComponent<VerticalLayoutGroup>();
		if (Screen.height < Screen.width)
		{
			float num = (float)Screen.height / (float)Screen.width;
			component.padding.top = (int)(num * 400f);
			component.padding.bottom = (int)(num * 400f);
		}
		else
		{
			float num2 = (float)Screen.width / (float)Screen.height;
			component.padding.top = (int)(num2 * 400f);
			component.padding.bottom = (int)(num2 * 400f);
		}
	}

	public void Load()
	{
		if (Singleton<Game>.Instance.IsLoaded && !(Singleton<Game>.Instance.Player == null) && !(Singleton<Game>.Instance.Player.Character == null))
		{
			RefreshPickups();
			GameObject replacementPickup = Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter().ReplacementPickup;
			Pickup component = replacementPickup.GetComponent<Pickup>();
			SnapToPickup(component, Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter().Id);
			m_characterRenderer.Load();
			m_characterRenderer.AddRandomForce();
			SetCharacter(Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter());
			SetUnlockCountText();
		}
	}

	private void RefreshPickup(CharacterInfo character, ref int childIdx)
	{
		if (!character.Locked)
		{
			if (m_scrollListContainer.Find(character.Id) == null)
			{
				GameObject replacementPickup = character.ReplacementPickup;
				Pickup component = replacementPickup.GetComponent<Pickup>();
				UIRenderPickup uIRenderPickup = Utils.CreateFromPrefab(m_pickupRendererPrefab, character.Id);
				uIRenderPickup.IsGolden = character.IsGolden;
				uIRenderPickup.SetPickup(component);
				uIRenderPickup.transform.SetParent(m_scrollListContainer, false);
				uIRenderPickup.transform.SetSiblingIndex(childIdx);
				UIScrollListScale component2 = uIRenderPickup.GetComponent<UIScrollListScale>();
				component2.SetBounds(m_scrollBounds);
			}
			else
			{
				m_scrollListContainer.Find(character.Id).transform.SetSiblingIndex(childIdx);
			}
			childIdx++;
		}
	}

	private void RefreshPickups()
	{
		int childIdx = 0;
		RefreshPickup(Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter(), ref childIdx);
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			RefreshPickup(availableCharacter, ref childIdx);
		}
		for (int i = 0; i < m_scrollListContainer.childCount; i++)
		{
			UIRenderPickup component = m_scrollListContainer.GetChild(i).GetComponent<UIRenderPickup>();
			if (component != null)
			{
				component.SetPickup(component.Prefab, false, true);
				component.IsVisible = false;
			}
		}
		m_selectedPickup.SetPickup(m_selectedPickup.Prefab, true, true);
	}

	public void Unload()
	{
		m_characterRenderer.Unload();
	}

	private void SetUnlockCountText()
	{
		int num = 0;
		int num2 = 0;
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			if (availableCharacter.m_group != (CharacterInfo.Group)m_currentFilter)
			{
				continue;
			}
			if (availableCharacter.IsGolden || !availableCharacter.m_countsForTotal)
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
		SetUnlockCountTextTotal();
	}

	private void SetUnlockCountTextTotal()
	{
		int num = 0;
		int num2 = 0;
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			if (availableCharacter.IsGolden || !availableCharacter.m_countsForTotal)
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
		m_unlockedTotalCountText.text = num2 + "/" + num;
	}

	public void OnChangePickup(Pickup pickupPrefab, string rendererName)
	{
		if (pickupPrefab == null)
		{
			m_selectedPickup.SetPickup(pickupPrefab);
			return;
		}
		CharacterInfo characterInfo = null;
		characterInfo = ((!(rendererName == Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter().Id)) ? Singleton<Game>.Instance.Player.GetCharacterInfo(rendererName) : Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter());
		if (characterInfo != null && pickupPrefab.gameObject == characterInfo.ReplacementPickup && !(m_characterRenderer.CharacterId == characterInfo.Id))
		{
			m_selectedPickup.IsGolden = characterInfo.IsGolden;
			m_selectedPickup.SetPickup(pickupPrefab, false, true);
			m_characterRenderer.IsGolden = characterInfo.IsGolden;
			m_characterRenderer.SetCharacter(characterInfo.Id, true);
			SetCharacter(characterInfo);
			m_onCharacterChange.Invoke();
		}
	}

	private void SetCharacter(CharacterInfo character)
	{
		if (m_perkController != null)
		{
			m_perkController.SetOverride(character.Id);
			m_perkController.UpdatePerk();
		}
		if (character.Locked)
		{
			if (character.Unlockable || character.PurchaseableOnly)
			{
				PurchasableItem iAP = Singleton<Game>.Instance.InventoryManager.GetIAP("com.prettygreat.landsliders." + character.Id);
				if (iAP != null)
				{
					m_iapText.text = iAP.localizedPriceString;
				}
				m_specialUnlock.gameObject.SetActive(false);
				m_selectButton.gameObject.SetActive(false);
				m_buyButton.gameObject.SetActive(true);
			}
			else
			{
				m_unlockText.text = character.UnlockText;
				m_specialUnlock.gameObject.SetActive(true);
				m_selectButton.gameObject.SetActive(false);
				m_buyButton.gameObject.SetActive(false);
			}
			m_nameText.text = character.Name;
			for (int i = 0; i < m_stars.Length; i++)
			{
				m_stars[i].enabled = false;
			}
		}
		else
		{
			m_specialUnlock.gameObject.SetActive(false);
			m_selectButton.gameObject.SetActive(true);
			m_buyButton.gameObject.SetActive(false);
			m_nameText.text = character.Name;
			object obj = Utils.Parse("Stats.collectible.Character" + character.Id + ".Current");
			if (obj == null || obj.GetType() != typeof(float))
			{
				return;
			}
			float f = (float)obj;
			int num = 0;
			for (int j = 0; j < m_stars.Length; j++)
			{
				num += Singleton<Game>.Instance.Player.XPTiers[j].m_target;
				if (Mathf.FloorToInt(f) >= num)
				{
					m_stars[j].enabled = true;
				}
				else
				{
					m_stars[j].enabled = false;
				}
			}
		}
		CrossPromoCharacter crossPromoCharacter = null;
		CrossPromoCharacter[] crossPromoCharacters = m_crossPromoCharacters;
		foreach (CrossPromoCharacter crossPromoCharacter2 in crossPromoCharacters)
		{
			if (crossPromoCharacter2.m_characterId == character.Id)
			{
				crossPromoCharacter = crossPromoCharacter2;
				break;
			}
		}
		if (crossPromoCharacter != null)
		{
			m_crossPromoText.text = crossPromoCharacter.m_text;
			m_crossPromoButton.gameObject.SetActive(true);
		}
		else
		{
			m_crossPromoButton.gameObject.SetActive(false);
		}
		if (m_statsContainer != null)
		{
			m_statsContainer.SetActive(character.Id != Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter().Id);
		}
	}

	public void OnSelectCharacter()
	{
		if (m_characterRenderer.CharacterId == Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter().Id)
		{
			Singleton<Game>.Instance.Player.IsRandomCharacter = true;
			Singleton<Game>.Instance.Player.PickRandomCharacter();
			Singleton<Game>.Instance.Player.JustPickedRandomCharacter();
		}
		else
		{
			Singleton<Game>.Instance.Player.IsRandomCharacter = false;
			Singleton<Game>.Instance.Player.SetCharacter(m_characterRenderer.CharacterId);
		}
		m_needsSave = true;
		m_onCharacterSelected.Invoke();
	}

	public void OnBuyCharacter()
	{
		Singleton<Game>.Instance.InventoryManager.PurchaseIAP("com.prettygreat.landsliders." + m_characterRenderer.CharacterId, PurchaseCallback);
	}

	public void PurchaseCallback(bool success, string id)
	{
		if (success)
		{
			if (Singleton<Game>.Instance.Player.GetCharacterInfo(id) == null)
			{
				PurchasableItem[] allPurchasableItems = Unibiller.AllPurchasableItems;
				foreach (PurchasableItem purchasableItem in allPurchasableItems)
				{
					if (purchasableItem.LocalIds[BillingPlatform.GooglePlay] == id)
					{
						id = purchasableItem.Id;
						break;
					}
				}
			}
			OnUnlockCharacter(id);
		}
		else
		{
			string empty = string.Empty;
			string msg = ScriptLocalization.Get("Purchase Failed", true);
			string ok = ScriptLocalization.Get("OK", true);
			NativeDialog.ShowDialog(empty, msg, ok);
		}
	}

	public void OnUnlockCharacter(string ID)
	{
		int num = ID.LastIndexOf('.');
		if (num != -1)
		{
			ID = ID.Substring(num + 1);
		}
		if (Singleton<Game>.Instance.Player.GetCharacterInfo(ID) == null)
		{
			return;
		}
		Singleton<Game>.Instance.InventoryManager.AddCurrency(ID, 1);
		SetCharacter(Singleton<Game>.Instance.Player.GetCharacterInfo(ID));
		for (int i = 0; i < m_scrollListContainer.childCount; i++)
		{
			UIRenderPickup component = m_scrollListContainer.GetChild(i).GetComponent<UIRenderPickup>();
			if (component != null)
			{
				GameObject replacement = m_characterRenderer.Prefab.GetReplacement(Singleton<Game>.Instance.DefaultPickup);
				if (component.Prefab == replacement.GetComponent<Pickup>())
				{
					component.SetPickup(component.Prefab, false, true);
					break;
				}
			}
		}
		m_selectedPickup.SetPickup(m_selectedPickup.Prefab, true, true);
		m_characterRenderer.SetCharacter(m_characterRenderer.CharacterId, true);
		Singleton<Game>.Instance.Player.SetCharacter(m_characterRenderer.CharacterId);
		m_onCharacterUnlocked.Invoke();
		m_needsSave = true;
		SetUnlockCountText();
	}

	public void OnClose()
	{
		m_onClose.Invoke();
	}

	public void OnCrossPromoClicked()
	{
		CrossPromoCharacter[] crossPromoCharacters = m_crossPromoCharacters;
		foreach (CrossPromoCharacter crossPromoCharacter in crossPromoCharacters)
		{
			if (crossPromoCharacter.m_characterId == m_characterRenderer.CharacterId)
			{
				Application.OpenURL(crossPromoCharacter.m_urlAndroid);
			}
		}
	}

	public void OnDisableScreen()
	{
		if (m_needsSave)
		{
			SaveManager.Save();
			m_needsSave = false;
		}
		Singleton<Game>.Instance.UIController.HideCharacterSelect();
	}
}
