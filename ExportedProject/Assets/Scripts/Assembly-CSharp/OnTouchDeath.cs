using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class OnTouchDeath : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private UnityEvent m_onTouchDeath = new UnityEvent();

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Death")
		{
			m_onTouchDeath.Invoke();
		}
	}

	public void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Death")
		{
			m_onTouchDeath.Invoke();
		}
	}
}
