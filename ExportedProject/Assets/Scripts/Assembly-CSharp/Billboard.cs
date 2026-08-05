using UnityEngine;

public class Billboard : MonoBehaviour
{
	private void Update()
	{
		base.transform.LookAt(Singleton<Game>.Instance.VisualCamera.Camera.transform.position, -Singleton<Game>.Instance.VisualCamera.Camera.transform.up);
	}
}
