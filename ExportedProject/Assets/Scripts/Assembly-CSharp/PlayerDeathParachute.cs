using UnityEngine;

public class PlayerDeathParachute : PlayerDeath
{
	[SerializeField]
	private GameObject m_deathObject;

	public override void Die()
	{
		base.Die();
		GameObject gameObject = Singleton<Game>.Instance.Player.Character.gameObject;
		Transform parent = gameObject.transform;
		GameObject gameObject2 = Object.Instantiate(m_deathObject);
		gameObject2.transform.parent = parent;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localRotation = Quaternion.identity;
		Joint[] componentsInChildren = gameObject.GetComponentsInChildren<Joint>();
		Joint[] array = componentsInChildren;
		foreach (Joint joint in array)
		{
			if (joint.connectedBody == Singleton<Game>.Instance.Player.Rigidbody)
			{
				Object.Destroy(joint);
			}
		}
		gameObject.transform.parent = null;
		Rigidbody[] componentsInChildren2 = gameObject.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array2 = componentsInChildren2;
		foreach (Rigidbody rigidbody in array2)
		{
			rigidbody.mass = 1f;
			rigidbody.drag = 20f;
			rigidbody.angularDrag = 100f;
			rigidbody.useGravity = true;
			Vector3 velocity = rigidbody.velocity;
			velocity.y = 0f;
			rigidbody.velocity = velocity;
		}
	}

	private void Update()
	{
	}
}
