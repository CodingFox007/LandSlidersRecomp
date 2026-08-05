using UnityEngine;
using UnityEngine.UI;

public class AndroidBackButton : MonoBehaviour
{
	public bool m_fireOnUp;

	private void Start()
	{
	}

	private void Update()
	{
		if (((Input.GetKeyUp(KeyCode.Escape) && m_fireOnUp) || (Input.GetKeyDown(KeyCode.Escape) && !m_fireOnUp)) && base.gameObject.GetComponent<Button>() != null)
		{
			base.gameObject.GetComponent<Button>().onClick.Invoke();
		}
	}
}
