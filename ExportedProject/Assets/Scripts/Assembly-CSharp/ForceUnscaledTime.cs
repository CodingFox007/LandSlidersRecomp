using UnityEngine;

public class ForceUnscaledTime : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particle;

	private void Start()
	{
	}

	private void Update()
	{
		particle.Simulate(Time.unscaledDeltaTime, true, false);
	}
}
