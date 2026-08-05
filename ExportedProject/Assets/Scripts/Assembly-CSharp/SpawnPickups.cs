using System;
using UnityEngine;

public class SpawnPickups : MonoBehaviour
{
	[Serializable]
	public class SpawnOptions
	{
		public bool m_useDefault = true;

		public RandomRange m_distance;

		public RandomRange m_height;

		public RandomRange m_speed;
	}

	[SerializeField]
	private Pickup m_pickup;

	[SerializeField]
	private RandomRange m_count;

	[SerializeField]
	private SpawnOptions m_spawnOptions = new SpawnOptions();

	[SerializeField]
	public bool triggerNextUpdate;

	[SerializeField]
	public bool triggerOnSpawn;

	public void Trigger()
	{
		m_count.Randomize();
		int count = Mathf.FloorToInt(m_count.Value);
		Spawn(count);
	}

	public void Start()
	{
		triggerNextUpdate = false;
		if (triggerOnSpawn)
		{
			Trigger();
		}
	}

	public void Update()
	{
		if (triggerNextUpdate)
		{
			triggerNextUpdate = false;
			Trigger();
		}
	}

	public void Spawn(int count)
	{
		GameObject gameObject = null;
		gameObject = ((!(m_pickup == null)) ? Singleton<Game>.Instance.Player.Character.GetReplacement(m_pickup.gameObject) : Singleton<Game>.Instance.Player.CharacterInfo.ReplacementPickup);
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject2 = Utils.CreateFromPrefab(gameObject, gameObject.name);
			if (gameObject2 != null)
			{
				gameObject2.transform.parent = Singleton<Game>.Instance.LevelManager.CurrentLevel.transform;
				gameObject2.transform.position += base.transform.position;
				Pickup component = gameObject2.GetComponent<Pickup>();
				if (component != null)
				{
					component.OnSpawned(m_spawnOptions, count);
				}
			}
		}
	}
}
