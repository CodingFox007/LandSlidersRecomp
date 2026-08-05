using UnityEngine;

public class RandomizeScale : MonoBehaviour
{
	[SerializeField]
	private RandomRange m_xScale;

	[SerializeField]
	private RandomRange m_yScale;

	[SerializeField]
	private RandomRange m_zScale;

	private void Start()
	{
		m_xScale.Randomize();
		m_yScale.Randomize();
		m_zScale.Randomize();
		base.transform.localScale = new Vector3(m_xScale.Value, m_yScale.Value, m_zScale.Value);
	}
}
