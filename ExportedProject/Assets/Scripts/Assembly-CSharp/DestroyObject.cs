using UnityEngine;

public class DestroyObject : MonoBehaviour
{
	[SerializeField]
	private float delay;

	public void Destroy(GameObject obj)
	{
		Object.Destroy(obj, delay);
	}
}
