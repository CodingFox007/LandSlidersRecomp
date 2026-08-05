using System.Collections.Generic;
using UnityEngine;

public class TurnWorldParticle : MonoBehaviour
{
	[SerializeField]
	private bool m_reversed;

	[SerializeField]
	private bool m_zChangesY = true;

	private List<ParticleSystem> m_emitters = new List<ParticleSystem>();

	private void Start()
	{
		m_emitters.AddRange(GetComponentsInChildren<ParticleSystem>());
	}

	private void Update()
	{
	}
}
