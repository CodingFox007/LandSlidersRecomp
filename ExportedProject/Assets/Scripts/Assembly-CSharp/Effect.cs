using System;
using UnityEngine;

[Serializable]
public class Effect
{
	[SerializeField]
	public ParticleSystem m_particlePrefab;

	[NonSerialized]
	public ParticleSystem m_particle;

	[SerializeField]
	public bool m_attachToPlayer;

	[NonSerialized]
	public Rigidbody m_target;

	public virtual void Update()
	{
	}
}
