using UnityEngine;

public class PlayerDeathFall : PlayerDeath
{
	[SerializeField]
	private GameObject m_fallParticle;

	public override void Die()
	{
		base.Die();
		Singleton<Game>.Instance.Player.UseGravity = true;
		GameObject gameObject = Singleton<Game>.Instance.Player.Character.gameObject;
		Transform transform = gameObject.transform;
		GameObject gameObject2 = Object.Instantiate(m_fallParticle, transform.position, m_fallParticle.transform.rotation);
		ParticleSystem[] componentsInChildren = gameObject2.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.startColor = Singleton<Game>.Instance.Player.CharacterInfo.ParticleColor;
		}
	}
}
