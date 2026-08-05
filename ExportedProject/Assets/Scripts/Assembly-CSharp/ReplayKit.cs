using UnityEngine;
using UnityEngine.Events;

public class ReplayKit : MonoBehaviour
{
	private delegate void InvokedFunction(string message);

	public bool debugForceAvailable;

	public UnityEvent onRecordingStarted;

	public UnityEvent onRecordingStopped;

	public UnityEvent onRecordingReadyForShare;

	public UnityEvent onSharingStarted;

	public UnityEvent onSharingEnded;

	public UnityEvent onRecordingAvailable;

	public UnityEvent onRecordingUnavailable;

	private static ReplayKit _instance;

	private InvokedFunction m_invokeFunction;

	private float m_invokeFunctionInTime;

	private bool m_isWaitingForRecordingToStart;

	private bool m_hasRecordingToShare;

	private bool m_isAvailable;

	private bool m_wasAvailable;

	public static bool IsRecording;

	public static ReplayKit Instance
	{
		get
		{
			return _instance;
		}
	}

	public bool isRecording { get; private set; }

	public bool isSharing { get; private set; }

	public bool wasMicrophoneOnWhenRecordingStarted { get; private set; }

	public bool isAvailable
	{
		get
		{
			return m_isAvailable || debugForceAvailable;
		}
	}

	private void WaitAndInvoke(float secondsToWait, InvokedFunction func)
	{
		if (m_invokeFunction == null)
		{
			m_invokeFunction = func;
			m_invokeFunctionInTime = secondsToWait;
		}
	}

	private static string GetGameObjectPath(Transform transform)
	{
		string text = transform.name;
		while (transform.parent != null)
		{
			transform = transform.parent;
			text = transform.name + "/" + text;
		}
		return text;
	}

	private void Start()
	{
		_instance = this;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
		}
		m_wasAvailable = !isAvailable;
		SendAvailabilityChangedEvent();
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	private void Update()
	{
		SendAvailabilityChangedEvent();
		if (m_invokeFunction != null)
		{
			m_invokeFunctionInTime -= Time.unscaledDeltaTime;
			if (m_invokeFunctionInTime <= 0f)
			{
				m_invokeFunction("true");
				m_invokeFunction = null;
			}
		}
	}

	public void StartRecording()
	{
		m_hasRecordingToShare = false;
		if (isAvailable && !isRecording && !m_isWaitingForRecordingToStart)
		{
			if (debugForceAvailable)
			{
				m_isWaitingForRecordingToStart = true;
				WaitAndInvoke(2f, OnRecordingStarted);
			}
			else if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
			}
		}
	}

	public void StopRecording()
	{
		if (isRecording)
		{
			bool flag = true;
			if (debugForceAvailable)
			{
				flag = true;
				WaitAndInvoke(2f, OnRecordingStopped);
			}
			else if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
			}
			if (flag)
			{
				isRecording = false;
				onRecordingStopped.Invoke();
			}
		}
	}

	public void ShareRecording()
	{
		if (!isSharing && m_hasRecordingToShare)
		{
			if (debugForceAvailable)
			{
				isSharing = true;
				WaitAndInvoke(2f, OnShareDismissed);
			}
			else if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
			}
		}
	}

	private void OnRecordingStarted(string message)
	{
		m_isWaitingForRecordingToStart = false;
		isRecording = message == "true";
		IsRecording = isRecording;
		if (isRecording)
		{
			onRecordingStarted.Invoke();
		}
	}

	private void OnRecordingStopped(string message)
	{
		IsRecording = false;
		m_hasRecordingToShare = message == "true";
		if (isRecording)
		{
			isRecording = false;
			onRecordingStopped.Invoke();
		}
		if (m_hasRecordingToShare)
		{
			onRecordingReadyForShare.Invoke();
		}
	}

	private void OnRecordingAvailabilityChanged(string message)
	{
		m_isAvailable = message == "true";
		SendAvailabilityChangedEvent();
	}

	private void OnShareDismissed(string message)
	{
		if (isSharing)
		{
			isSharing = false;
			onSharingEnded.Invoke();
		}
	}

	private void OnShareActivitiesCompleted(string message)
	{
	}

	private void SendAvailabilityChangedEvent()
	{
		if (isAvailable != m_wasAvailable)
		{
			m_wasAvailable = isAvailable;
			if (m_wasAvailable)
			{
				onRecordingAvailable.Invoke();
			}
			else
			{
				onRecordingUnavailable.Invoke();
			}
		}
	}
}
