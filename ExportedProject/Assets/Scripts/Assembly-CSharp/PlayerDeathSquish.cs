using UnityEngine;

public class PlayerDeathSquish : PlayerDeath
{
	[SerializeField]
	private GameObject m_squishParticle;

	public override void Die()
	{
		base.Die();
		GameObject gameObject = Singleton<Game>.Instance.Player.Character.gameObject;
		Transform transform = gameObject.transform;
		Rigidbody[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = componentsInChildren;
		foreach (Rigidbody rigidbody in array)
		{
			rigidbody.gameObject.RemoveComponentsIfExists<Joint>();
			rigidbody.gameObject.RemoveComponentIfExists<ConstantForce>();
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
		Vector3 localScale = transform.localScale;
		localScale.x = 1.5f;
		localScale.z = 1.5f;
		localScale.y = 0.1f;
		transform.localScale = localScale;
		transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
		Object.Instantiate(m_squishParticle, transform.position, m_squishParticle.transform.rotation);
	}
}
