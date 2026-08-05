using UnityEngine;
using UnityEngine.Events;

public class UICheckPurchased : MonoBehaviour
{
	[SerializeField]
	private string m_purchaseID;

	[SerializeField]
	private UnityEvent m_onEnablePurchased = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onEnableUnpurchased = new UnityEvent();

	public void OnEnable()
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
			if (flag)
			{
				m_onEnableUnpurchased.Invoke();
			}
			else
			{
				m_onEnablePurchased.Invoke();
			}
		}
		else if (m_purchaseID != string.Empty && m_purchaseID.Contains("."))
		{
			if (Unibiller.GetPurchaseCount(Unibiller.GetPurchasableItemById(m_purchaseID)) > 0)
			{
				m_onEnablePurchased.Invoke();
			}
			else
			{
				m_onEnableUnpurchased.Invoke();
			}
		}
	}
}
