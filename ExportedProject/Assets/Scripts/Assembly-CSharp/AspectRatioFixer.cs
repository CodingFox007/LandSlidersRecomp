using UnityEngine;
using UnityEngine.UI;

public class AspectRatioFixer : MonoBehaviour
{
	[SerializeField]
	private AspectRatioFitter m_aspectFitter;

	private void Update()
	{
		if (Screen.width == 2436 && Screen.height == 1125)
		{
			m_aspectFitter.aspectRatio = 2.17f;
		}
		else if (Screen.width == 1125 && Screen.height == 2436)
		{
			m_aspectFitter.aspectRatio = 0.87f;
		}
	}
}
