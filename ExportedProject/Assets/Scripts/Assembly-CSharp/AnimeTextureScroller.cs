using UnityEngine;

public class AnimeTextureScroller : MonoBehaviour
{
	[SerializeField]
	private Material m_mat;

	[SerializeField]
	private float m_speed;

	private void Start()
	{
		m_mat.mainTextureOffset += new Vector2(0f, -1f);
	}

	private void Update()
	{
		m_mat.mainTextureOffset += new Vector2(0f, m_speed);
	}
}
