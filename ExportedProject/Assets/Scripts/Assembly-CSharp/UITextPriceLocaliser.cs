using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class UITextPriceLocaliser : MonoBehaviour
{
	[SerializeField]
	private string m_purchaseID;

	[SerializeField]
	private string m_purchasedString;

	[SerializeField]
	private string m_unlockCheck;

	private void OnEnable()
	{
		RefreshText();
	}

	public void RefreshText()
	{
		if (m_purchaseID != string.Empty && !m_purchaseID.Contains("."))
		{
			bool flag = false;
			string[] array = m_purchaseID.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				CharacterInfo characterInfo = Singleton<Game>.Instance.Player.GetCharacterInfo(text.Trim());
				if (characterInfo != null && characterInfo.Locked)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				base.gameObject.GetComponent<Text>().text = ScriptLocalization.Get(m_purchasedString, true);
			}
			return;
		}
		if (m_purchaseID != string.Empty && m_purchaseID.Contains(".") && Unibiller.GetPurchaseCount(Unibiller.GetPurchasableItemById(m_purchaseID)) > 0 && m_purchasedString != string.Empty)
		{
			base.gameObject.GetComponent<Text>().text = ScriptLocalization.Get(m_purchasedString, true);
			return;
		}
		bool flag2 = true;
		if (m_unlockCheck != string.Empty)
		{
			string[] array3 = m_unlockCheck.Split(',');
			string[] array4 = array3;
			foreach (string text2 in array4)
			{
				CharacterInfo characterInfo2 = Singleton<Game>.Instance.Player.GetCharacterInfo(text2.Trim());
				if (characterInfo2 != null && characterInfo2.Locked)
				{
					flag2 = false;
					break;
				}
			}
		}
		else
		{
			flag2 = false;
		}
		if (flag2)
		{
			base.gameObject.GetComponent<Text>().text = ScriptLocalization.Get(m_purchasedString, true);
		}
		else
		{
			base.gameObject.GetComponent<Text>().text = Unibiller.GetPurchasableItemById(m_purchaseID).localizedPriceString;
		}
	}
}
