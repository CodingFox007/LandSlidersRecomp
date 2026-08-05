using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FastPool
{
	[SerializeField]
	[Tooltip("Prefab that would be used as source")]
	private GameObject sourcePrefab;

	[Tooltip("Cache size (maximum amount of the cached items).\r\n[0 - unlimited]")]
	public int Capacity;

	[Tooltip("How much items must be cached at the start")]
	public int PreloadCount;

	[Tooltip("How to call events OnFastInstantiate and OnFastDestroy. Note, that if use choose notification via Interface, you must implement IFastPoolObject in any script on your sourcePrefab")]
	public PoolItemNotificationType NotificationType;

	[Tooltip("Load source prefab in the memory while scene is loading. A bit slower scene loading, but faster instantiating of new objects in pool")]
	public bool WarmOnLoad = true;

	[Tooltip("Parent cached objects to FastPoolManager game object.\r\n[WARNING] Turning this option on will make objects cached a bit slower.")]
	public bool ParentOnCache;

	[SerializeField]
	[HideInInspector]
	private int cached_internal;

	public List<GameObject> cache;

	[NonSerialized]
	[HideInInspector]
	public List<GameObject> created;

	private Transform parentTransform;

	public int ID { get; private set; }

	public string Name
	{
		get
		{
			return sourcePrefab.name;
		}
	}

	public int Cached
	{
		get
		{
			return cache.Count;
		}
	}

	public bool IsValid { get; private set; }

	public FastPool(GameObject prefab, Transform rootTransform = null, bool warmOnLoad = true, int preloadCount = 0, int capacity = 0)
	{
		sourcePrefab = prefab;
		PreloadCount = preloadCount;
		Capacity = capacity;
		WarmOnLoad = warmOnLoad;
		Init(rootTransform);
	}

	public bool Init(Transform rootTransform)
	{
		if (sourcePrefab != null)
		{
			cached_internal = 0;
			cache = new List<GameObject>();
			created = new List<GameObject>();
			parentTransform = rootTransform;
			ID = sourcePrefab.GetInstanceID();
			if (WarmOnLoad)
			{
				string name = sourcePrefab.name + "_SceneSource";
				sourcePrefab = MakeClone(Vector3.zero, Quaternion.identity, parentTransform);
				sourcePrefab.name = name;
				sourcePrefab.SetActive(false);
			}
			for (int i = cache.Count; i < PreloadCount; i++)
			{
				FastDestroy(MakeClone(Vector3.zero, Quaternion.identity, null));
			}
			IsValid = true;
		}
		else
		{
			Debug.LogError("Creating pool with null prefab!");
			IsValid = false;
		}
		return IsValid;
	}

	public void ClearCache()
	{
		foreach (GameObject item in cache)
		{
			UnityEngine.Object.Destroy(item);
		}
		cache.Clear();
		cached_internal = 0;
	}

	public T FastInstantiate<T>(Transform parent = null) where T : Component
	{
		return FastInstantiate<T>(Vector3.zero, Quaternion.identity, parent);
	}

	public T FastInstantiate<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
	{
		GameObject gameObject = FastInstantiate(position, rotation, parent);
		return (!(gameObject != null)) ? ((T)null) : gameObject.GetComponent<T>();
	}

	public GameObject FastInstantiate(Transform parent = null)
	{
		return FastInstantiate(Vector3.zero, Quaternion.identity, parent);
	}

	public GameObject FastInstantiate(Vector3 position, Quaternion rotation, Transform parent = null)
	{
		GameObject gameObject;
		while (cache.Count > 0)
		{
			gameObject = cache[cache.Count - 1];
			cache.RemoveAt(cache.Count - 1);
			cached_internal--;
			if (gameObject != null)
			{
				Transform transform = gameObject.transform;
				transform.localPosition = position;
				transform.localRotation = rotation;
				if (parent != null)
				{
					transform.SetParent(parent, false);
				}
				gameObject.SetActive(true);
				switch (NotificationType)
				{
				case PoolItemNotificationType.Interface:
					gameObject.GetComponent<IFastPoolItem>().OnFastInstantiate();
					break;
				case PoolItemNotificationType.SendMessage:
					gameObject.SendMessage("OnFastInstantiate");
					break;
				case PoolItemNotificationType.BroadcastMessage:
					gameObject.BroadcastMessage("OnFastInstantiate");
					break;
				}
				if (!created.Contains(gameObject))
				{
					created.Add(gameObject);
				}
				return gameObject;
			}
			Debug.LogWarning("The pool with the " + sourcePrefab.name + " prefab contains null entry. Don't destroy cached items manually!");
		}
		gameObject = MakeClone(position, rotation, parent);
		created.Add(gameObject);
		if (WarmOnLoad)
		{
			gameObject.SetActive(true);
		}
		return gameObject;
	}

	public void FastDestroy<T>(T sceneObject) where T : Component
	{
		if (sceneObject != null)
		{
			FastDestroy(sceneObject.gameObject);
		}
		else
		{
			Debug.LogWarning("Attempt to destroy a null object");
		}
	}

	public void FastDestroy(GameObject sceneObject)
	{
		if (sceneObject != null)
		{
			if (cache.Count < Capacity || Capacity <= 0)
			{
				cache.Insert(cache.Count, sceneObject);
				cached_internal++;
				if (ParentOnCache)
				{
					sceneObject.transform.SetParent(parentTransform, false);
				}
				switch (NotificationType)
				{
				case PoolItemNotificationType.Interface:
					sceneObject.GetComponent<IFastPoolItem>().OnFastDestroy();
					break;
				case PoolItemNotificationType.SendMessage:
					sceneObject.SendMessage("OnFastDestroy");
					break;
				}
				sceneObject.SetActive(false);
			}
			else
			{
				UnityEngine.Object.Destroy(sceneObject);
			}
		}
		else
		{
			Debug.LogWarning("Attempt to destroy a null object");
		}
	}

	private GameObject MakeClone(Vector3 position, Quaternion rotation, Transform parent)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(sourcePrefab, position, rotation);
		gameObject.transform.SetParent(parent, false);
		return gameObject;
	}
}
