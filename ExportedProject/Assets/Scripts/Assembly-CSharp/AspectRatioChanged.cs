using UnityEngine;
using UnityEngine.Events;

public class AspectRatioChanged : MonoBehaviour
{
	[SerializeField]
	private UnityEvent m_onPortrait = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onLandscape = new UnityEvent();

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
			m_onLandscape.Invoke();
		}
		else
		{
			m_onPortrait.Invoke();
		}
	}
}
