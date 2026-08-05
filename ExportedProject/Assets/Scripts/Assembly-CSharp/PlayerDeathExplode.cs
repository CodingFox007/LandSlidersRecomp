using UnityEngine;

public class PlayerDeathExplode : PlayerDeath
{
	[SerializeField]
	private ParticleSystem m_explodeParticle;

	[SerializeField]
	private ParticleSystem m_explodeTrail;

	public override void Die()
	{
		base.Die();
		GameObject gameObject = Singleton<Game>.Instance.Player.Character.gameObject;
		Transform transform = gameObject.transform;
		Vector3 position = transform.position;
		position += new Vector3(Random.Range(-1.5f, 1.5f), 0f, Random.Range(-1.5f, 1.5f));
		ParticleSystem particleSystem = Object.Instantiate(m_explodeParticle, transform.position, m_explodeParticle.transform.rotation);
		particleSystem.transform.parent = null;
		particleSystem.Play();
		Rigidbody[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = componentsInChildren;
		foreach (Rigidbody rigidbody in array)
		{
			rigidbody.mass = 10f;
			rigidbody.drag = 1f;
			rigidbody.useGravity = true;
			rigidbody.gameObject.RemoveComponentsIfExists<Joint>();
			rigidbody.gameObject.RemoveComponentIfExists<ConstantForce>();
			rigidbody.gameObject.RemoveComponentIfExists<CenterOfMass>();
			rigidbody.gameObject.AddComponent<BoxCollider>();
			rigidbody.gameObject.layer = 0;
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.AddExplosionForce(Random.Range(500f, 1000f), position, 10f, Random.Range(1f, 1.25f), ForceMode.Impulse);
			ParticleSystem particleSystem2 = Object.Instantiate(m_explodeTrail, rigidbody.transform.position, m_explodeParticle.transform.rotation);
			particleSystem2.transform.parent = rigidbody.transform;
			particleSystem2.Play();
		}
	}
}
