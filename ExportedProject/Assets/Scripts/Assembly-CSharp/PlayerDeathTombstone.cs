using UnityEngine;

public class PlayerDeathTombstone : PlayerDeath
{
	[SerializeField]
	private GameObject m_tombstone;

	public override void Die()
	{
		base.Die();
		GameObject gameObject = Singleton<Game>.Instance.Player.Character.gameObject;
		Transform transform = gameObject.transform;
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = false;
		}
		Rigidbody[] componentsInChildren2 = gameObject.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array2 = componentsInChildren2;
		foreach (Rigidbody rigidbody in array2)
		{
			rigidbody.gameObject.RemoveComponentsIfExists<Joint>();
			rigidbody.gameObject.RemoveComponentIfExists<ConstantForce>();
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
		transform.rotation = Quaternion.identity;
		GameObject gameObject2 = Object.Instantiate(m_tombstone);
		gameObject2.transform.parent = transform;
		gameObject2.transform.localPosition = Vector3.zero;
		if (Singleton<Game>.Instance.Player.KilledBy != null)
		{
			Entity entity = Singleton<Game>.Instance.Player.KilledBy.GetComponent<Entity>();
			if (entity == null)
			{
				entity = Singleton<Game>.Instance.Player.KilledBy.GetComponentInParent<Entity>();
			}
			if (entity != null)
			{
				entity.Kill(null, 0f, false);
			}
		}
	}
}
