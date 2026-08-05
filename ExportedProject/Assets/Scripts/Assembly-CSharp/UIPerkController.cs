using ArabicSupport;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class UIPerkController : MonoBehaviour
{
	[SerializeField]
	private Text m_textField1;

	[SerializeField]
	private Text m_textField2;

	[SerializeField]
	private Image m_image;

	[SerializeField]
	private string m_needLevelUpString = "LEVEL UP TO GAIN PERK TEXT";

	[SerializeField]
	private bool m_enableLocalisation;

	private string m_overrideID = string.Empty;

	public void SetOverride(string ID)
	{
		m_overrideID = ID;
	}

	private string LocaliseValue(string key, float val)
	{
		string result;
		if (m_enableLocalisation)
		{
			if (LocalizationManager.IsRight2Left)
			{
				LocalizationManager.IsRight2Left = false;
				result = ArabicFixer.Fix(string.Format(ScriptLocalization.Get(key), val));
				LocalizationManager.IsRight2Left = true;
			}
			else
			{
				result = string.Format(ScriptLocalization.Get(key, true), val);
			}
		}
		else
		{
			result = string.Format(key, val);
		}
		return result;
	}

	private void OnEnable()
	{
		UpdatePerk();
	}

	public void UpdatePerk()
	{
		m_textField1.gameObject.SetActive(true);
		m_textField2.gameObject.SetActive(true);
		m_image.gameObject.SetActive(true);
		string text = ((!(m_overrideID == string.Empty)) ? m_overrideID : Singleton<Game>.Instance.Player.CharacterInfo.Id);
		PerkManager.Perk perkForCharacter = Singleton<Game>.Instance.PerkManager.GetPerkForCharacter(text);
		if (perkForCharacter != null)
		{
			object obj = Utils.Parse("Stats.collectible.Character" + text + ".Current");
			if (obj == null || obj.GetType() != typeof(float))
			{
				return;
			}
			float f = (float)obj;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 5; i++)
			{
				num += Singleton<Game>.Instance.Player.XPTiers[i].m_target;
				if (Mathf.FloorToInt(f) >= num)
				{
					num2++;
				}
			}
			if (num2 > 5)
			{
				num2 = 5;
			}
			string lockedSelectionPerkText = Singleton<Game>.Instance.Player.GetCharacterInfo(text).m_lockedSelectionPerkText;
			if (Singleton<Game>.Instance.Player.GetCharacterInfo(text).Locked && Singleton<Game>.Instance.Player.GetCharacterInfo(text).m_useCustomLockedSelectionPerkText)
			{
				if (lockedSelectionPerkText == string.Empty)
				{
					m_textField1.gameObject.SetActive(false);
					m_textField2.gameObject.SetActive(false);
					m_image.gameObject.SetActive(false);
					return;
				}
				if (m_enableLocalisation)
				{
					m_textField1.text = ScriptLocalization.Get(lockedSelectionPerkText, true);
				}
				else
				{
					m_textField1.text = lockedSelectionPerkText;
				}
				m_textField2.text = string.Empty;
				m_image.gameObject.SetActive(false);
				return;
			}
			if (num2 == 0)
			{
				if (m_enableLocalisation)
				{
					m_textField1.text = ScriptLocalization.Get(m_needLevelUpString, true);
				}
				else
				{
					m_textField1.text = m_needLevelUpString;
				}
				m_textField2.text = string.Empty;
				m_image.gameObject.SetActive(false);
				return;
			}
			float num3 = 0f;
			num3 = ((perkForCharacter.m_visualAmountPerLevel != 0f) ? Mathf.Abs(perkForCharacter.m_visualAmountPerLevel * (float)num2) : Mathf.Abs(perkForCharacter.m_amountPerLevel * (float)num2));
			m_textField1.text = LocaliseValue(perkForCharacter.m_text1, num3);
			m_textField2.text = LocaliseValue(perkForCharacter.m_text2, num3);
			if (perkForCharacter.m_image == null && !perkForCharacter.m_usePickupForImage)
			{
				m_image.gameObject.SetActive(false);
			}
			else if (perkForCharacter.m_usePickupForImage)
			{
				m_image.gameObject.SetActive(true);
				m_image.sprite = Singleton<Game>.Instance.Player.GetCharacterInfo(text).ReplacementPickup.GetComponent<Pickup>().Icon;
			}
			else
			{
				m_image.gameObject.SetActive(true);
				m_image.sprite = perkForCharacter.m_image;
			}
		}
		else
		{
			m_textField1.text = "NO PERK SET FOR " + Singleton<Game>.Instance.Player.CharacterInfo.Id;
			m_textField2.text = string.Empty;
		}
	}
}
