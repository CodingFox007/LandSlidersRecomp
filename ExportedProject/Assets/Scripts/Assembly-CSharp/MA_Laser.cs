using UnityEngine;

public class MA_Laser : MonoBehaviour
{
	private Transform trans;

	private void Awake()
	{
		base.useGUILayout = false;
		trans = base.transform;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.StartsWith("Enemy("))
		{
			Object.Destroy(collision.gameObject);
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		float num = 10f * Time.deltaTime;
		Vector3 position = trans.position;
		position.y += num;
		trans.position = position;
		if (trans.position.y > 7f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
