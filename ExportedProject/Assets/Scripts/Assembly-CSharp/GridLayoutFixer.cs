using UnityEngine;
using UnityEngine.UI;

public class GridLayoutFixer : MonoBehaviour
{
	[SerializeField]
	private GridLayoutGroup m_layoutGroup;

	private void Update()
	{
		if ((Screen.width == 1125 && Screen.height == 2436) || (Screen.width == 2436 && Screen.height == 1125))
		{
			m_layoutGroup.cellSize = new Vector2(270f, 220f);
		}
		else
		{
			m_layoutGroup.cellSize = new Vector2(290f, 220f);
		}
	}
}
