using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
	[SerializeField]
	protected GameObject m_spawnPrefab;

	[SerializeField]
	protected bool m_spawnInfinite;

	[SerializeField]
	protected bool m_rotateTowardsPlayer;

	[SerializeField]
	protected RandomRange m_spawnCount;

	protected int m_currentCount;

	[SerializeField]
	protected RandomRange m_spawnAtOnce = new RandomRange(1f, 1f);

	[SerializeField]
	protected RandomRange m_spawnDelay;

	protected float m_spawnTimer;

	[SerializeField]
	private RandomRange m_spawnRotation;

	[SerializeField]
	private RandomRange m_xOffset;

	[SerializeField]
	private RandomRange m_yOffset;

	[SerializeField]
	private RandomRange m_zOffset;

	[SerializeField]
	private bool m_spawnOnPlayer;

	[SerializeField]
	private UnityEvent m_onSpawn;

	protected void Start()
	{
		m_spawnCount.Randomize();
		m_spawnDelay.Randomize();
	}

	private void Update()
	{
		if (m_spawnInfinite || !((float)m_currentCount >= m_spawnCount.Value))
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

	public void Spawn()
	{
		GameObject replacement = Singleton<Game>.Instance.Player.Character.GetReplacement(m_spawnPrefab);
		Material materialReplacement = Singleton<Game>.Instance.Player.Character.GetMaterialReplacement(m_spawnPrefab);
		m_spawnAtOnce.Randomize();
		for (int i = 0; (float)i < m_spawnAtOnce.Value; i++)
		{
			GameObject gameObject = Utils.CreateFromPrefab(replacement, replacement.name);
			if (!(gameObject != null))
			{
				continue;
			}
			if (materialReplacement != null)
			{
				SkinnedMeshRenderer[] componentsInChildren = gameObject.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
				{
					skinnedMeshRenderer.sharedMaterial = materialReplacement;
				}
			}
			gameObject.transform.parent = Singleton<Game>.Instance.LevelManager.CurrentLevel.transform;
			if (m_spawnOnPlayer)
			{
				gameObject.transform.position += Singleton<Game>.Instance.Player.transform.position;
			}
			else
			{
				gameObject.transform.position += base.transform.position;
			}
			m_spawnRotation.Randomize();
			gameObject.transform.rotation = Quaternion.Euler(0f, m_spawnRotation.Value, 0f);
			if (m_rotateTowardsPlayer)
			{
				Vector3 position = Singleton<Game>.Instance.Player.transform.position;
				position.y = gameObject.transform.position.y;
				gameObject.transform.LookAt(position);
			}
			m_xOffset.Randomize();
			m_yOffset.Randomize();
			m_zOffset.Randomize();
			gameObject.transform.position += new Vector3(m_xOffset.Value, m_yOffset.Value, m_zOffset.Value);
			GameObject gameObject2 = null;
			if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
			{
				gameObject2 = Singleton<Game>.Instance.Player.Character.GetAdditions(replacement);
			}
			if (gameObject2 != null)
			{
				GameObject gameObject3 = Utils.CreateFromPrefab(gameObject2, gameObject2.name);
				gameObject3.transform.parent = gameObject.transform;
				gameObject3.transform.localPosition = gameObject2.transform.localPosition;
				gameObject3.transform.localRotation = gameObject2.transform.localRotation;
				gameObject3.transform.localScale = gameObject2.transform.localScale;
			}
		}
		m_onSpawn.Invoke();
		m_currentCount++;
	}
}
