using UnityEngine;
using UnityEngine.Events;

public class UIBestScoreSelector : MonoBehaviour
{
	public int m_minValue;

	public int m_maxValue;

	[SerializeField]
	public UnityEvent m_onBestScore = new UnityEvent();

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		int num = Mathf.FloorToInt(Singleton<Game>.Instance.StatsManager.GetStat("collectible").Life.Highest);
		if (num >= m_minValue && (m_maxValue == -1 || num < m_maxValue))
		{
			m_onBestScore.Invoke();
		}
	}
}
