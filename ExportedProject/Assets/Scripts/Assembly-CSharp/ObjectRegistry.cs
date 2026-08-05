using System.Collections.Generic;
using UnityEngine;

public class ObjectRegistry : MonoBehaviour
{
	private static Dictionary<string, List<GameObject>> m_registry = new Dictionary<string, List<GameObject>>();

	[SerializeField]
	private string m_id;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		AddObject(m_id, base.gameObject);
	}

	private void OnDisable()
	{
		RemoveObject(m_id, base.gameObject);
	}

	public static GameObject GetRandom(string id, bool preferVisible)
	{
		if (!m_registry.ContainsKey(id))
		{
			return null;
		}
		if (m_registry[id].Count == 0)
		{
			return null;
		}
		if (preferVisible)
		{
			List<GameObject> list = new List<GameObject>();
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Singleton<Game>.Instance.VisualCamera.Camera);
			foreach (GameObject item in m_registry[id])
			{
				if (item.GetComponent<Renderer>() != null && GeometryUtility.TestPlanesAABB(planes, item.GetComponent<Renderer>().bounds))
				{
					list.Add(item);
				}
			}
			if (list.Count != 0)
			{
				return list[Random.Range(0, list.Count)];
			}
		}
		return m_registry[id][Random.Range(0, m_registry[id].Count)];
	}

	public static void AddObject(string id, GameObject obj)
	{
		if (!m_registry.ContainsKey(id))
		{
			m_registry.Add(id, new List<GameObject>());
		}
		m_registry[id].Add(obj);
	}

	public static void RemoveObject(string id, GameObject obj)
	{
		if (m_registry.ContainsKey(id))
		{
			m_registry[id].Remove(obj);
		}
	}
}
