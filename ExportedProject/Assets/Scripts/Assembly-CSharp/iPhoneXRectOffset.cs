using UnityEngine;

public class iPhoneXRectOffset : MonoBehaviour
{
	[SerializeField]
	private Vector2 m_landscapeOffsetMin = Vector2.zero;

	[SerializeField]
	private Vector2 m_landscapeOffsetMax = Vector2.zero;

	[SerializeField]
	private Vector2 m_portraitOffsetMin = Vector2.zero;

	[SerializeField]
	private Vector2 m_portraitOffsetMax = Vector2.zero;

	public void OnPortait()
	{
		if (isiPhoneX())
		{
			GetComponent<RectTransform>().offsetMin = m_portraitOffsetMin;
			GetComponent<RectTransform>().offsetMax = m_portraitOffsetMax;
		}
	}

	public void OnLandscape()
	{
		if (isiPhoneX())
		{
			GetComponent<RectTransform>().offsetMin = m_landscapeOffsetMin;
			GetComponent<RectTransform>().offsetMax = m_landscapeOffsetMax;
		}
	}

	private bool isiPhoneX()
	{
		return false;
	}

	private void Start()
	{
		Singleton<Game>.Instance.AspectRatioChangedEvent += AspectChange;
		AspectChange();
	}

	private void AspectChange()
	{
		float num = (float)Screen.width / (float)Screen.height;
		if (num > 1f)
		{
			OnLandscape();
		}
		else
		{
			OnPortait();
		}
	}
}
