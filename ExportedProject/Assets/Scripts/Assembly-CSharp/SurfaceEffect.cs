using System;
using UnityEngine;

[Serializable]
public class SurfaceEffect : Effect
{
	[SerializeField]
	public string m_surfaceTag;

	[SerializeField]
	public float m_minSpeed;

	[SerializeField]
	public float m_maxSpeed;

	[SerializeField]
	public bool m_onlyWhenGrounded = true;

	[SerializeField]
	public bool m_disableYSpeed = true;

	public override void Update()
	{
		if (m_target == null)
		{
			return;
		}
		Vector3 velocity = m_target.velocity;
		if (m_disableYSpeed)
		{
			velocity.y = 0f;
		}
		float magnitude = velocity.magnitude;
		bool flag = (magnitude >= m_minSpeed && magnitude <= m_maxSpeed) || m_minSpeed == m_maxSpeed;
		ParticleSystem[] componentsInChildren = m_particle.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			if (flag && !particleSystem.isPlaying)
			{
				particleSystem.Play();
			}
			particleSystem.enableEmission = flag;
		}
	}
}
