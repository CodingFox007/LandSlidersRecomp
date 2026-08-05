using UnityEngine;

public class PlayerDeathAngel : PlayerDeath
{
	[SerializeField]
	private Material m_deathMaterial;

	[SerializeField]
	private GameObject m_deathObject;

	[SerializeField]
	private float m_flyDelay = 3f;

	private float m_flyDelayTimer;

	[SerializeField]
	private float m_flySpeed = 16f;

	public override void Die()
	{
		base.Die();
		GameObject gameObject = Singleton<Game>.Instance.Player.Character.gameObject;
		Transform parent = gameObject.transform;
		Singleton<Game>.Instance.Player.Character.SetMaterials(m_deathMaterial);
		GameObject gameObject2 = Object.Instantiate(m_deathObject);
		gameObject2.transform.parent = parent;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localRotation = Quaternion.identity;
	}

	private void Update()
	{
		m_flyDelayTimer += Time.unscaledDeltaTime;
		if (!(m_flyDelayTimer < m_flyDelay))
		{
			Singleton<Game>.Instance.Player.Rigidbody.velocity = new Vector3(0f, m_flySpeed, 0f);
		}
	}
}
