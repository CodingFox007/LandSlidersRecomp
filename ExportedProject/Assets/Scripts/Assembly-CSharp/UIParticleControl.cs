using HeavyDutyInspector;
using UnityEngine;

public class UIParticleControl : MonoBehaviour
{
	[Comment("UI Particle Sizes != Game Particle Sizes", CommentType.None, 0)]
	[SerializeField]
	private bool m_spawnFromPrefab;

	[HideConditional("m_spawnFromPrefab", true)]
	[SerializeField]
	private ParticleSystem m_particlePrefab;

	[HideConditional("m_spawnFromPrefab", false)]
	[SerializeField]
	private ParticleSystem m_particle;

	[HideConditional("m_spawnFromPrefab", true)]
	[SerializeField]
	private Transform m_parent;

	public void PlayParticle()
	{
		if (m_spawnFromPrefab)
		{
			m_particle = Object.Instantiate(m_particlePrefab, m_parent.position, Quaternion.identity);
			m_particle.transform.SetParent(m_parent, true);
			m_particle.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		if (m_particle != null)
		{
			m_particle.Play();
		}
	}

	public void PlayParticleSpecific(ParticleSystem go2)
	{
		m_particle = Object.Instantiate(go2, m_parent.position, go2.transform.localRotation);
		m_particle.transform.SetParent(m_parent, true);
		m_particle.transform.localScale = new Vector3(1f, 1f, 1f);
		if (m_particle != null)
		{
			m_particle.Play();
		}
	}

	public void StopParticle()
	{
		if (m_particle != null)
		{
			m_particle.Stop();
		}
	}
}
