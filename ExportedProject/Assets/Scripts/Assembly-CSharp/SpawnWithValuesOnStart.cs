using UnityEngine;

public class SpawnWithValuesOnStart : MonoBehaviour
{
	[SerializeField]
	private bool m_destroyAfterwards;

	private int m_destroyTimer = 10;

	private void Start()
	{
		SpawnValues component = base.gameObject.GetComponent<SpawnValues>();
		if (component != null)
		{
			GameObject randomPrefab = component.GetRandomPrefab(Singleton<Game>.Instance.LevelManager.CurrentLevel);
			GameObject gameObject = Utils.CreateFromPrefab(randomPrefab, randomPrefab.name);
			if (gameObject != null)
			{
				gameObject.transform.parent = Singleton<Game>.Instance.LevelManager.CurrentLevel.transform;
				gameObject.transform.position += base.transform.position;
			}
		}
	}

	private void Update()
	{
		if (m_destroyAfterwards && m_destroyTimer > 0)
		{
			m_destroyTimer--;
			if (m_destroyTimer == 0)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
