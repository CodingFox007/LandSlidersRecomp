using UnityEngine;

public class SpawnPlaceholder : MonoBehaviour
{
	[SerializeField]
	private GameObject m_spawnPrefab;

	[SerializeField]
	private bool m_randomRotated = true;

	[SerializeField]
	private bool m_lateSpawn;

	[SerializeField]
	private string m_specialEventSpawnIfDay;

	[SerializeField]
	private string m_specialEventSpawnIfNotDay;

	[SerializeField]
	private string m_specialEventOnlyOnEvent;

	private void Start()
	{
		if (!m_lateSpawn || Singleton<Game>.Instance.LevelManager.CurrentLevel == null)
		{
			Spawn();
		}
		else
		{
			Singleton<Game>.Instance.LevelManager.CurrentLevel.OnLoadPreComplete += Spawn;
		}
	}

	private void OnDestroy()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.LevelManager != null && Singleton<Game>.Instance.LevelManager.CurrentLevel != null)
		{
			Singleton<Game>.Instance.LevelManager.CurrentLevel.OnLoadPreComplete -= Spawn;
		}
	}

	private void Spawn()
	{
		if ((m_specialEventOnlyOnEvent != string.Empty && Singleton<Game>.Instance.SpecialEventManager.GetID() != m_specialEventOnlyOnEvent) || (m_specialEventSpawnIfDay != string.Empty && Singleton<Game>.Instance.SpecialEventManager.CurrentDay() != m_specialEventSpawnIfDay) || (m_specialEventSpawnIfNotDay != string.Empty && Singleton<Game>.Instance.SpecialEventManager.CurrentDay() == m_specialEventSpawnIfNotDay))
		{
			return;
		}
		GameObject gameObject = m_spawnPrefab;
		if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
		{
			gameObject = Singleton<Game>.Instance.Player.Character.GetReplacement(gameObject);
		}
		if (gameObject != null)
		{
			Vector3 position = base.transform.position + gameObject.transform.position;
			GameObject gameObject2 = Object.Instantiate(gameObject, position, gameObject.transform.localRotation);
			gameObject2.transform.parent = base.transform.parent;
			if (m_randomRotated)
			{
				gameObject2.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			}
			else
			{
				gameObject2.transform.rotation = base.transform.rotation;
			}
			GameObject gameObject3 = null;
			if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
			{
				gameObject3 = Singleton<Game>.Instance.Player.Character.GetAdditions(gameObject);
			}
			if (gameObject3 != null)
			{
				GameObject gameObject4 = Utils.CreateFromPrefab(gameObject3, gameObject3.name);
				gameObject4.transform.parent = gameObject2.transform;
				gameObject4.transform.localPosition = gameObject3.transform.localPosition;
				gameObject4.transform.localRotation = gameObject3.transform.localRotation;
				gameObject4.transform.localScale = gameObject3.transform.localScale;
			}
		}
		Object.Destroy(base.gameObject);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(base.transform.position, new Vector3(1f, 1f, 1f));
	}
}
