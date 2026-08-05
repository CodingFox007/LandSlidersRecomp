using UnityEngine;

public class UIHackOffsetRectTransform : MonoBehaviour
{
	public void HackSetYOffset(float position)
	{
		Vector2 anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
		anchoredPosition.y = position;
		GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
	}
}
