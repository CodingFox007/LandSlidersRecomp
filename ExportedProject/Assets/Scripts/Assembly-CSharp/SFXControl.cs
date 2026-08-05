using DarkTonic.MasterAudio;
using UnityEngine;

public class SFXControl : MonoBehaviour
{
	[SoundGroup]
	[SerializeField]
	private string m_soundGroup;

	[SerializeField]
	private bool m_is3dSound;

	[SerializeField]
	private bool m_playOnAwake;

	[SerializeField]
	private Renderer m_visualCheck;

	private SoundGroupVariation m_variation;

	private void Awake()
	{
		if (m_playOnAwake)
		{
			if (m_is3dSound)
			{
				PlaySFX3D();
			}
			else
			{
				PlaySFX();
			}
		}
	}

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

	public void PlaySFX3D()
	{
		if (!(m_visualCheck != null) || m_visualCheck.isVisible)
		{
			PlaySoundResult playSoundResult = MasterAudio.PlaySound3DAtTransform(m_soundGroup, base.gameObject.transform);
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

	public void PlaySFXByName(string sfx)
	{
		if (!(m_visualCheck != null) || m_visualCheck.isVisible)
		{
			PlaySoundResult playSoundResult = MasterAudio.PlaySound(sfx);
			if (playSoundResult != null)
			{
				m_variation = playSoundResult.ActingVariation;
			}
		}
	}

	public void PlaySFX3DByName(string sfx)
	{
		if (!(m_visualCheck != null) || m_visualCheck.isVisible)
		{
			PlaySoundResult playSoundResult = MasterAudio.PlaySound3DAtTransform(sfx, base.gameObject.transform);
			if (playSoundResult != null)
			{
				m_variation = playSoundResult.ActingVariation;
			}
		}
	}

	public void TriggerPlaylist(string name)
	{
		MasterAudio.TriggerPlaylistClip(name);
	}

	public void ChangePlaylist(string name)
	{
		MasterAudio.ChangePlaylistByName(name, false);
	}
}
