using DarkTonic.MasterAudio;
using UnityEngine;

public class BombEvent : StateMachineBehaviour
{
	[SoundGroup]
	[SerializeField]
	private string m_soundGroup;

	[SerializeField]
	private Renderer m_visualCheck;

	[SerializeField]
	private bool m_stopSound = true;

	private SoundGroupVariation m_variation;

	public void PlaySFX()
	{
		if (!(m_visualCheck != null) || m_visualCheck.isVisible)
		{
			PlaySoundResult playSoundResult = MasterAudio.PlaySound(m_soundGroup);
			if (playSoundResult != null)
			{
				m_variation = playSoundResult.ActingVariation;
			}
		}
	}

	public void Stop()
	{
		if (m_variation != null && m_variation.IsPlaying)
		{
			m_variation.Stop();
		}
	}

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		PlaySFX();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (m_stopSound)
		{
			Stop();
		}
	}
}
