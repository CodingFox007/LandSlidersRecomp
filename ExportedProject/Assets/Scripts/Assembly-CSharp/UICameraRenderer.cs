using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICameraRenderer : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private RawImage m_image;

	[SerializeField]
	public UnityEvent m_onCameraEnabled = new UnityEvent();

	[SerializeField]
	public UnityEvent m_onCameraDisabled = new UnityEvent();

	private bool m_cameraEnabled;

	private void OnEnable()
	{
		m_cameraEnabled = false;
		if (m_cameraEnabled)
		{
			m_onCameraEnabled.Invoke();
		}
		else
		{
			m_onCameraDisabled.Invoke();
		}
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
	}

	public void Play()
	{
	}

	public void Stop()
	{
	}
}
