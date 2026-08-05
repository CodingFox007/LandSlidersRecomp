using UnityEngine;
using UnityEngine.UI;

public class UITextPriceSpecial : MonoBehaviour
{
	private void OnEnable()
	{
		base.gameObject.GetComponent<Text>().text = Unibiller.GetPurchasableItemById(Singleton<Game>.Instance.SpecialEventManager.GetBuyIAP()).localizedPriceString;
	}
}
