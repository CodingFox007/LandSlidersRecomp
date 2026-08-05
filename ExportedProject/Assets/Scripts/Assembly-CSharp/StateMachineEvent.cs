using System;
using UnityEngine;

public class StateMachineEvent : StateMachineBehaviour
{
	[Serializable]
	public class AnimMessage
	{
		public float m_timeNormalized;

		public string m_message;

		[HideInInspector]
		public bool m_triggered;
	}

	[SerializeField]
	private string m_enterMessage;

	[SerializeField]
	private string m_exitMessage;

	[SerializeField]
	private AnimMessage[] m_timedMessages;

	[SerializeField]
	private bool m_disableAnimatorOnExit;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		if (m_enterMessage != null && m_enterMessage != string.Empty)
		{
			animator.gameObject.BroadcastMessage(m_enterMessage, SendMessageOptions.DontRequireReceiver);
		}
		AnimMessage[] timedMessages = m_timedMessages;
		foreach (AnimMessage animMessage in timedMessages)
		{
			animMessage.m_triggered = false;
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		if (m_exitMessage != null && m_exitMessage != string.Empty)
		{
			animator.gameObject.BroadcastMessage(m_exitMessage, SendMessageOptions.DontRequireReceiver);
		}
		if (m_disableAnimatorOnExit)
		{
			animator.enabled = false;
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		AnimMessage[] timedMessages = m_timedMessages;
		foreach (AnimMessage animMessage in timedMessages)
		{
			if (!animMessage.m_triggered && stateInfo.normalizedTime > animMessage.m_timeNormalized)
			{
				if (animMessage.m_message != null && animMessage.m_message != string.Empty)
				{
					animator.gameObject.BroadcastMessage(animMessage.m_message, SendMessageOptions.DontRequireReceiver);
				}
				animMessage.m_triggered = true;
			}
		}
	}
}
