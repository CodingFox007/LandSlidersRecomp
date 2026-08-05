using UnityEngine;
using UnityEngine.UI;

public class UIReviveButton : MonoBehaviour
{
	[SerializeField]
	private Image m_fillImage;

	[SerializeField]
	private float m_timeOnScreen = 3f;

	private float m_timerSpeed = 1f;

	private float m_currentTime;

	public bool m_running = true;

	private void Start()
	{
	}

	private void Update()
	{
		if (m_running)
		{
			AddTime(Time.fixedDeltaTime);
		}
	}

	private void DisableButton()
	{
		base.gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		m_running = false;
		m_fillImage.fillAmount = 1f;
		m_currentTime = 0f;
		m_timerSpeed = 1f;
	}

	public void AddTime(float amount)
	{
		bool flag = m_timeOnScreen == m_currentTime;
		m_currentTime = Mathf.Min(m_timeOnScreen, m_currentTime + amount * m_timerSpeed);
		m_fillImage.fillAmount = 1f - m_currentTime / m_timeOnScreen;
		if (!flag && m_timeOnScreen == m_currentTime)
		{
			Time.timeScale = 1f;
			Singleton<Game>.Instance.Player.GoToWipe();
			GetComponent<Animator>().SetTrigger("buttonOut");
			Singleton<Game>.Instance.AdManager.EndOfGame(false);
		}
	}

	public void SetTimerSpeed(float speed)
	{
		m_timerSpeed = speed;
	}
}
