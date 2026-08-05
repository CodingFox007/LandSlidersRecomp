using I2.Loc;
using UnityEngine;

public class ScaleLanguage : MonoBehaviour
{
	[SerializeField]
	private string m_language;

	[SerializeField]
	private float m_scale;

	[SerializeField]
	private float m_elseScale;

	private void OnEnable()
	{
		if (m_language == LocalizationManager.CurrentLanguage)
		{
			base.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
		}
		else
		{
			base.transform.localScale = new Vector3(m_elseScale, m_elseScale, m_elseScale);
		}
	}
}
