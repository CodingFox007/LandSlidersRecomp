using UnityEngine;

public class iPhoneXScaleOffset : MonoBehaviour
{
	[SerializeField]
	private float m_scale = 1f;

	[SerializeField]
	private float m_yOffset = 165f;

	[SerializeField]
	private RectTransform m_rectTransform;

	private bool isiPhoneX()
	{
		return false;
	}

	private void OnEnable()
	{
		if (isiPhoneX() && m_rectTransform != null)
		{
			m_rectTransform.transform.localScale = Vector3.one * m_scale;
			Vector2 offsetMax = m_rectTransform.offsetMax;
			offsetMax.y = 0f - m_yOffset;
			m_rectTransform.offsetMax = offsetMax;
		}
	}
}
