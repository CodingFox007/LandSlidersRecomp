using UnityEngine;

public class GameObjectController : MonoBehaviour
{
	[SerializeField]
	private GameObject m_obj;

	public void SetObj(GameObject obj)
	{
		m_obj = obj;
	}

	public void Destory(float delay = 0f)
	{
		Object.Destroy(m_obj, delay);
	}

	public void Destory(GameObject obj)
	{
		Object.Destroy(obj);
	}

	public void SetParentNull(GameObject obj)
	{
		obj.transform.SetParent(null);
	}
}
