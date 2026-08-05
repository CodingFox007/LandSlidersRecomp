using UnityEngine;

public class MA_EnemyOne : MonoBehaviour
{
	public GameObject ExplosionParticlePrefab;

	private Transform trans;

	private float speed;

	private float horizSpeed;

	private void Awake()
	{
		base.useGUILayout = false;
		trans = base.transform;
		speed = (float)Random.Range(-3, -8) * Time.deltaTime;
		horizSpeed = (float)Random.Range(-3, 3) * Time.deltaTime;
	}

	private void OnCollisionEnter(Collision collision)
	{
		Object.Instantiate(ExplosionParticlePrefab, trans.position, Quaternion.identity);
	}

	private void Update()
	{
		Vector3 position = trans.position;
		position.x += horizSpeed;
		position.y += speed;
		trans.position = position;
		trans.Rotate(Vector3.down * 300f * Time.deltaTime);
		if (trans.position.y < -5f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
