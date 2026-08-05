using UnityEngine;

public class SnowballSpawner : Spawner
{
	private Entity m_entity;

	[SerializeField]
	private Entity.State m_spawnState = Entity.State.Chase;

	[SerializeField]
	private float m_postFreezeDontThrow = 1f;

	private new void Start()
	{
		m_entity = GetComponent<Entity>();
		base.Start();
	}

	private void Update()
	{
		if ((m_spawnInfinite || !((float)m_currentCount >= m_spawnCount.Value)) && (!(m_entity != null) || m_entity.GetState() == m_spawnState) && !Singleton<Game>.Instance.Player.IsFrozen && !(Singleton<Game>.Instance.Player.GetPostFreezeTimer < m_postFreezeDontThrow))
		{
			m_spawnTimer += Time.deltaTime;
			if (m_spawnTimer >= m_spawnDelay.Value)
			{
				m_spawnDelay.Randomize();
				m_spawnTimer = 0f;
				Spawn();
			}
		}
	}
}
