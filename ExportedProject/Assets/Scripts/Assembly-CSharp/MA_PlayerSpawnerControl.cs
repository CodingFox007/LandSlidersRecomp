using UnityEngine;

public class MA_PlayerSpawnerControl : MonoBehaviour
{
	public GameObject Player;

	private float nextSpawnTime;

	private bool PlayerActive
	{
		get
		{
			return Player.activeInHierarchy;
		}
	}

	private void Awake()
	{
		base.useGUILayout = false;
		nextSpawnTime = -1f;
	}

	private void Update()
	{
		if (!PlayerActive)
		{
			if (nextSpawnTime < 0f)
			{
				nextSpawnTime = Time.time + 1f;
			}
			if (Time.time >= nextSpawnTime)
			{
				Player.SetActive(true);
				nextSpawnTime = -1f;
			}
		}
	}
}
