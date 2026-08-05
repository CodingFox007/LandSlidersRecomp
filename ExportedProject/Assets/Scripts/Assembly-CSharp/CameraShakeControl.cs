using UnityEngine;

public class CameraShakeControl : MonoBehaviour
{
	[SerializeField]
	private string m_shakeId;

	public void Trigger()
	{
		Singleton<Game>.Instance.VisualCamera.Shake(m_shakeId);
	}

	public void UnTrigger()
	{
		Singleton<Game>.Instance.VisualCamera.StopShake(m_shakeId);
	}
}
