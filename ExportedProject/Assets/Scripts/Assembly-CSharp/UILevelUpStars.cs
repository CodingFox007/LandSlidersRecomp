using UnityEngine;
using UnityEngine.UI;

public class UILevelUpStars : MonoBehaviour
{
	[SerializeField]
	private Image[] m_stars;

	private void OnEnable()
	{
		object obj = Utils.Parse("Stats.collectible.Character" + Singleton<Game>.Instance.Player.CharacterId + ".Current");
		if (obj == null || obj.GetType() != typeof(float))
		{
			return;
		}
		float f = (float)obj;
		int num = 0;
		for (int i = 0; i < m_stars.Length; i++)
		{
			num += Singleton<Game>.Instance.Player.XPTiers[i].m_target;
			if (Mathf.FloorToInt(f) >= num)
			{
				m_stars[i].enabled = true;
			}
			else
			{
				m_stars[i].enabled = false;
			}
		}
	}
}
