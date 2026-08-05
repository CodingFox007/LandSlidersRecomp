using UnityEngine;
using UnityEngine.UI;

public class UICallToAction : MonoBehaviour
{
	[SerializeField]
	private Image m_avoidIcon;

	[SerializeField]
	private Sprite[] m_enemyIcons;

	private int m_enemyIdx;

	public void GenerateEnemyIcons()
	{
		m_enemyIdx = 0;
		Utils.ShuffleArray(m_enemyIcons);
	}

	public void SetRandomEnemySprite()
	{
		Sprite sprite = null;
		if (m_enemyIcons.Length > 0)
		{
			sprite = m_enemyIcons[m_enemyIdx];
		}
		m_avoidIcon.sprite = sprite;
		m_enemyIdx++;
		if (m_enemyIdx >= m_enemyIcons.Length)
		{
			m_enemyIdx = 0;
		}
	}
}
