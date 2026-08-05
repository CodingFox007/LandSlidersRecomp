using UnityEngine;
using UnityEngine.Events;

public class OnApplicationPaused : MonoBehaviour
{
	[SerializeField]
	private UnityEvent m_onPause = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onResume = new UnityEvent();

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			m_onPause.Invoke();
		}
		else
		{
			m_onResume.Invoke();
		}
	}
}
