using System.Collections.Generic;
using UnityEngine;

public class FastPoolManager : MonoBehaviour
{
	[SerializeField]
	private List<FastPool> predefinedPools;

	private Dictionary<int, FastPool> pools;

	private Transform curTransform;

	public static FastPoolManager Instance { get; private set; }

	public Dictionary<int, FastPool> Pools
	{
		get
		{
			return pools;
		}
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			curTransform = GetComponent<Transform>();
			pools = new Dictionary<int, FastPool>();
		}
		else
		{
			Debug.LogError("You cannot have more than one instance of FastPoolManager in the scene!");
		}
	}

	private void Start()
	{
		for (int i = 0; i < predefinedPools.Count; i++)
		{
			if (predefinedPools[i].Init(curTransform))
			{
				pools.Add(predefinedPools[i].ID, predefinedPools[i]);
			}
		}
		predefinedPools.Clear();
	}

	public static FastPool CreatePoolC<T>(T component, bool warmOnLoad = true, int preloadCount = 0, int capacity = 0) where T : Component
	{
		if (component != null)
		{
			return CreatePool(component.gameObject, warmOnLoad, preloadCount, capacity);
		}
		return null;
	}

	public static FastPool CreatePool(GameObject prefab, bool warmOnLoad = true, int preloadCount = 0, int capacity = 0)
	{
		if (prefab != null)
		{
			if (!Instance.pools.ContainsKey(prefab.GetInstanceID()))
			{
				FastPool fastPool = new FastPool(prefab, Instance.curTransform, warmOnLoad, preloadCount, capacity);
				Instance.pools.Add(prefab.GetInstanceID(), fastPool);
				return fastPool;
			}
			return Instance.pools[prefab.GetInstanceID()];
		}
		Debug.LogError("Creating pool with null object");
		return null;
	}

	public static FastPool GetPool(GameObject prefab, bool createIfNotExists = true)
	{
		if (prefab != null)
		{
			if (Instance.pools.ContainsKey(prefab.GetInstanceID()))
			{
				return Instance.pools[prefab.GetInstanceID()];
			}
			return CreatePool(prefab);
		}
		Debug.LogError("Trying to get pool for null object");
		return null;
	}

	public static bool HasPool(GameObject prefab)
	{
		return Instance.pools.ContainsKey(prefab.GetInstanceID());
	}

	public static FastPool GetPool(Component component, bool createIfNotExists = true)
	{
		if (component != null)
		{
			GameObject gameObject = component.gameObject;
			if (Instance.pools.ContainsKey(gameObject.GetInstanceID()))
			{
				return Instance.pools[gameObject.GetInstanceID()];
			}
			return CreatePool(gameObject);
		}
		Debug.LogError("Trying to get pool for null object");
		return null;
	}

	public static void DestroyPool(FastPool pool)
	{
		pool.ClearCache();
		Instance.pools.Remove(pool.ID);
	}
}
