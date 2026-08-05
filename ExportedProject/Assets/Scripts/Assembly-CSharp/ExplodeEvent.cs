using UnityEngine;
using UnityEngine.Events;

public class ExplodeEvent : StateMachineBehaviour
{
	[SerializeField]
	private UnityEvent m_explosionEvent = new UnityEvent();

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		m_explosionEvent.Invoke();
	}
}
