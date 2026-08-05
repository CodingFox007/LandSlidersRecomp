using UnityEngine;

public class UISpecialEventEnabler : MonoBehaviour
{
	[SerializeField]
	private GameObject m_object;

	private void OnEnable()
	{
		if (m_object != null)
		{
			m_object.SetActive(Singleton<Game>.Instance.SpecialEventManager.AnyLocked());
		}
	}
}
