using UnityEngine;

public class SelfDestructor : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Object.Destroy(animator.gameObject);
	}
}
