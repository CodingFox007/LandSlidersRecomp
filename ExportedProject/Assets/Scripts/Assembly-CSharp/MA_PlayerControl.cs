using DarkTonic.MasterAudio;
using UnityEngine;

public class MA_PlayerControl : MonoBehaviour
{
	public GameObject ProjectilePrefab;

	public bool canShoot = true;

	private const float MOVE_SPEED = 10f;

	private Transform trans;

	private float lastMoveAmt;

	private void Awake()
	{
		base.useGUILayout = false;
		trans = base.transform;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name.StartsWith("Enemy("))
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnDisable()
	{
	}

	private void OnBecameInvisible()
	{
	}

	private void OnBecameVisible()
	{
	}

	private void Update()
	{
		float num = Input.GetAxis("Horizontal") * 10f * Time.deltaTime;
		if (num != 0f)
		{
			if (lastMoveAmt == 0f)
			{
				MasterAudio.FireCustomEvent("PlayerMoved", trans.position);
			}
		}
		else if (lastMoveAmt != 0f)
		{
			MasterAudio.FireCustomEvent("PlayerStoppedMoving", trans.position);
		}
		lastMoveAmt = num;
		Vector3 position = trans.position;
		position.x += num;
		trans.position = position;
		if (canShoot && Input.GetMouseButtonDown(0))
		{
			Vector3 position2 = trans.position;
			position2.y += 1f;
			Object.Instantiate(ProjectilePrefab, position2, ProjectilePrefab.transform.rotation);
		}
	}
}
