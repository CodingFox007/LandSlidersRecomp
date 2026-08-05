using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class GameCamera : MonoBehaviour
{
	[Serializable]
	public class ZoomOptions
	{
		[SerializeField]
		public bool m_canZoom;

		[SerializeField]
		public float m_smoothTime = 0.3f;

		[NonSerialized]
		public float m_smoothTimeCurrent;

		[NonSerialized]
		public float m_smoothVelocity;

		[SerializeField]
		public float m_minSpeed = 0.1f;

		[SerializeField]
		public float m_maxSpeed = 0.5f;

		[SerializeField]
		public float m_normalFOV = 30f;

		[SerializeField]
		public float m_zoomFOV = 60f;

		[SerializeField]
		public float m_zoomOutScale = 0.5f;
	}

	[Serializable]
	public class TiltOptions
	{
		[SerializeField]
		public float m_tilt;

		[SerializeField]
		public float m_tiltCurrent;

		[SerializeField]
		public float m_smoothVelocity;

		[SerializeField]
		public float m_smoothTime = 0.3f;

		public float m_min = -1f;

		public float m_max = 1f;

		public float m_scale = 1f;
	}

	[Serializable]
	public class AspectZoomValue
	{
		public string m_name = "16/9";

		public float m_aspectRatio = 1.7777f;

		public float m_zoom = 45f;
	}

	[Serializable]
	public class SkyboxOptions
	{
		public Material m_skybox;

		[Range(-90f, 90f)]
		public float m_minRot;

		[Range(-90f, 90f)]
		public float m_maxRot;

		public float m_minHeight;

		public float m_maxHeight;

		public float m_currentRotation;

		public Color m_fogColor = new Color(0.7058824f, 47f / 51f, 0.972549f, 1f);

		public Texture2D m_fogTexture;

		public float m_fogMinHeight = -80f;

		public float m_fogMaxHeight = -5f;

		public Color m_colorize = Color.white;

		public Texture2D m_currentTexture;

		[HideInInspector]
		public float m_originalMinRot;

		[HideInInspector]
		public float m_originalMaxRot;

		[HideInInspector]
		public float m_originalMinHeight;

		[HideInInspector]
		public float m_originalMaxHeight;

		[HideInInspector]
		public Color m_originalFogColor;

		[HideInInspector]
		public float m_originalFogMinHeight;

		[HideInInspector]
		public float m_originalFogMaxHeight;
	}

	[Serializable]
	public class CameraShake
	{
		[SerializeField]
		public string m_name;

		[SerializeField]
		public float m_warmUpTime = 1f;

		[SerializeField]
		public float m_shakeAmount = 1f;

		[SerializeField]
		public float m_shakePeriod = 1f;

		[SerializeField]
		public AnimationCurve m_amountCurve;

		[SerializeField]
		public AnimationCurve m_peroidCurve;

		[SerializeField]
		public float m_disableTime;

		[HideInInspector]
		public float m_timer;

		[HideInInspector]
		public float m_timeActive;

		[HideInInspector]
		public bool m_active;

		[HideInInspector]
		public Vector3 m_target;

		[SerializeField]
		public UnityEvent m_onStart = new UnityEvent();

		[SerializeField]
		public UnityEvent m_onStop = new UnityEvent();
	}

	[SerializeField]
	private CameraTarget m_targetPrefab;

	private CameraTarget m_target;

	[SerializeField]
	private CameraTarget m_peekTargetPrefab;

	private CameraTarget m_peekTarget;

	[SerializeField]
	private float m_targetDiffMax = 0.1f;

	public Vector3 m_offsetDirection;

	public float m_baseOffsetLength;

	private float m_currentOffsetLength;

	[SerializeField]
	private float m_smoothTime = 0.3f;

	[SerializeField]
	private float m_smoothDeceleration = 0.0001f;

	private float m_smoothTimeCurrent;

	private Vector3 m_smoothVelocity = Vector3.zero;

	private Vector3 m_currentTargetPos = Vector3.zero;

	private Vector3 m_currentTargetOffsetPos = Vector3.zero;

	private Vector3 m_desiredTargetOffsetPos = Vector3.zero;

	[SerializeField]
	private ZoomOptions m_zoomOptions;

	private Camera m_camera;

	[SerializeField]
	public TiltOptions m_xTilt;

	[SerializeField]
	public TiltOptions m_yTilt;

	[SerializeField]
	private AspectZoomValue[] m_aspectZoomValues;

	[SerializeField]
	private SkyboxOptions m_skyboxOptions;

	[SerializeField]
	private CameraShake[] m_cameraShakes;

	private CameraShake m_cameraShake;

	public Texture2D DefaultSkyboxTexture;

	private Quaternion defaultLocalRotation;

	public Transform m_testParalaxTransform;

	private Quaternion m_gyroBasis = Quaternion.identity;

	private Quaternion m_rotationalOffset = Quaternion.identity;

	private bool m_hasSetBasis;

	private static bool __changeEverythingFirstTime = true;

	[HideInInspector]
	public bool shadowsOnlyOnPlayer;

	public CameraTarget Target
	{
		get
		{
			return m_target;
		}
	}

	public CameraTarget PeekTarget
	{
		get
		{
			return m_peekTarget;
		}
	}

	public SkyboxOptions SkyBoxOptions
	{
		get
		{
			return m_skyboxOptions;
		}
	}

	public Camera Camera
	{
		get
		{
			return m_camera;
		}
	}

	public bool CanZoom
	{
		get
		{
			return m_zoomOptions.m_canZoom;
		}
		set
		{
			m_zoomOptions.m_canZoom = value;
		}
	}

	public float NormalZoomFOV
	{
		get
		{
			return m_zoomOptions.m_normalFOV;
		}
	}

	public float DefaultMaxHeight
	{
		get
		{
			return m_skyboxOptions.m_originalMaxHeight;
		}
	}

	public float DefaultMinHeight
	{
		get
		{
			return m_skyboxOptions.m_originalMinHeight;
		}
	}

	public float DefaultMaxRot
	{
		get
		{
			return m_skyboxOptions.m_originalMaxRot;
		}
	}

	public float DefaultMinRot
	{
		get
		{
			return m_skyboxOptions.m_originalMinRot;
		}
	}

	public Color DefaultFogColor
	{
		get
		{
			return m_skyboxOptions.m_originalFogColor;
		}
	}

	public float DefaultFogMinHeight
	{
		get
		{
			return m_skyboxOptions.m_originalFogMinHeight;
		}
	}

	public float DefaultFogMaxHeight
	{
		get
		{
			return m_skyboxOptions.m_originalFogMaxHeight;
		}
	}

	private Quaternion DeviceRotation
	{
		get
		{
			if (m_testParalaxTransform != null)
			{
				return m_testParalaxTransform.transform.localRotation;
			}
			return Quaternion.identity;
		}
	}

	private void Start()
	{
		m_camera = base.gameObject.GetComponent<Camera>();
		m_target = Utils.CreateFromPrefab(m_targetPrefab, "CameraTargetPlayer");
		m_peekTarget = Utils.CreateFromPrefab(m_peekTargetPrefab, "CameraTargetPeek");
		m_skyboxOptions.m_originalMaxHeight = m_skyboxOptions.m_maxHeight;
		m_skyboxOptions.m_originalMaxRot = m_skyboxOptions.m_maxRot;
		m_skyboxOptions.m_originalMinHeight = m_skyboxOptions.m_minHeight;
		m_skyboxOptions.m_originalMinRot = m_skyboxOptions.m_minRot;
		m_skyboxOptions.m_originalFogColor = m_skyboxOptions.m_fogColor;
		m_skyboxOptions.m_originalFogMinHeight = m_skyboxOptions.m_fogMinHeight;
		m_skyboxOptions.m_originalFogMaxHeight = m_skyboxOptions.m_fogMaxHeight;
		m_skyboxOptions.m_currentRotation = float.MaxValue;
		m_skyboxOptions.m_currentTexture = null;
		RenderSettings.skybox.mainTexture = DefaultSkyboxTexture;
		ConnectToPlayer();
		defaultLocalRotation = base.transform.localRotation;
		m_offsetDirection = base.transform.position - m_target.transform.position;
		m_currentOffsetLength = (m_baseOffsetLength = m_offsetDirection.magnitude);
		m_offsetDirection /= m_baseOffsetLength;
		Singleton<Game>.Instance.AspectRatioChangedEvent += AspectRatioChanged;
		AspectRatioChanged();
	}

	private float GetFrictionValue(float frictionAmount)
	{
		float p = Time.unscaledDeltaTime * 60f;
		return 1f - Mathf.Pow(1f - frictionAmount, p);
	}

	private void Update()
	{
		UpdateSkybox();
		if (Singleton<Game>.Instance.Player.IsReduceMotionEnabled || (Singleton<Game>.Instance != null && Singleton<Game>.Instance.LevelManager != null && Singleton<Game>.Instance.LevelManager.IsRetryLevel))
		{
			m_rotationalOffset = Quaternion.identity;
			m_hasSetBasis = false;
		}
		else
		{
			Quaternion deviceRotation = DeviceRotation;
			if (m_testParalaxTransform != null)
			{
				m_testParalaxTransform.transform.localRotation = deviceRotation;
			}
			if (!m_hasSetBasis || Time.time < 1f)
			{
				m_hasSetBasis = true;
				m_gyroBasis = deviceRotation;
			}
			else
			{
				m_gyroBasis = Quaternion.Slerp(m_gyroBasis, deviceRotation, GetFrictionValue(0.003f));
				m_gyroBasis = Quaternion.RotateTowards(deviceRotation, m_gyroBasis, 25f);
			}
			Vector3 lhs = Quaternion.Inverse(m_gyroBasis) * Vector3.up;
			Vector3 rhs = Quaternion.Inverse(m_gyroBasis) * Vector3.right;
			Vector3 vector = Vector3.Cross(lhs, rhs);
			Quaternion b = Quaternion.FromToRotation(deviceRotation * vector, m_gyroBasis * vector);
			b = Quaternion.Slerp(Quaternion.identity, b, 0.1f);
			m_rotationalOffset = Quaternion.Slerp(m_rotationalOffset, b, GetFrictionValue(0.05f));
		}
		if (base.transform.parent != null)
		{
			m_currentTargetPos = base.transform.parent.position;
		}
		Vector3 vector2 = m_target.transform.position - m_currentTargetPos;
		Vector3 vector3 = m_peekTarget.transform.position - m_currentTargetPos;
		vector3.y = vector2.y;
		Vector3 vector4 = vector3 - vector2;
		if (Singleton<Game>.Instance.Player.IsCollidingWall && vector4.magnitude > m_targetDiffMax)
		{
			m_desiredTargetOffsetPos = vector3;
			m_smoothTimeCurrent = m_smoothTime;
		}
		else
		{
			m_desiredTargetOffsetPos = vector2;
		}
		m_currentTargetOffsetPos = Vector3.SmoothDamp(m_currentTargetOffsetPos, m_desiredTargetOffsetPos, ref m_smoothVelocity, m_smoothTimeCurrent);
		m_smoothTimeCurrent -= m_smoothDeceleration;
		UpdateZoom();
		UpdateShake();
		Vector3 vector5 = Vector3.zero;
		if (m_cameraShake != null)
		{
			vector5 = m_cameraShake.m_target;
		}
		Quaternion quaternion = defaultLocalRotation;
		base.transform.rotation = m_rotationalOffset * quaternion;
		base.transform.position = m_currentTargetPos + m_currentTargetOffsetPos - base.transform.rotation * Vector3.forward * m_currentOffsetLength + vector5;
		Singleton<Game>.Instance.RayCastCamera.transform.rotation = quaternion;
		Singleton<Game>.Instance.RayCastCamera.transform.position = m_currentTargetPos + m_currentTargetOffsetPos - quaternion * Vector3.forward * m_currentOffsetLength;
	}

	private static bool SetStruct<T>(ref T currentValue, T newValue)
	{
		if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
		{
			return __changeEverythingFirstTime;
		}
		currentValue = newValue;
		return true;
	}

	private static bool SetShaderFloat(string shaderValueName, ref float currentValue, float newValue)
	{
		if (SetStruct(ref currentValue, newValue))
		{
			Shader.SetGlobalFloat(shaderValueName, newValue);
			return true;
		}
		return false;
	}

	private static bool SetShaderColor(string shaderValueName, ref Color currentValue, Color newValue)
	{
		if (SetStruct(ref currentValue, newValue))
		{
			Shader.SetGlobalColor(shaderValueName, newValue);
			return true;
		}
		return false;
	}

	private static bool SetShaderTexture(string shaderValueName, ref Texture2D currentValue, Texture2D newValue)
	{
		if (newValue == null)
		{
			newValue = Texture2D.whiteTexture;
		}
		if (SetStruct(ref currentValue, newValue))
		{
			Shader.SetGlobalTexture(shaderValueName, newValue);
			return true;
		}
		return false;
	}

	private void UpdateSkybox()
	{
		float minRot = DefaultMinRot;
		float maxRot = DefaultMaxRot;
		float minHeight = DefaultMinHeight;
		float maxHeight = DefaultMaxHeight;
		Color fogColor = DefaultFogColor;
		float fogMinHeight = DefaultFogMinHeight;
		float fogMaxHeight = DefaultFogMaxHeight;
		Texture2D fogTexture = DefaultSkyboxTexture;
		Color colorize = Color.white;
		ChangeSkybox.GetHighestPriority(out minRot, out maxRot, out minHeight, out maxHeight, out fogColor, out fogMinHeight, out fogMaxHeight, out colorize, out fogTexture);
        if (Singleton<Game>.Instance.LevelManager.IsRetryLevel)
        {
            minRot += 180f;
            maxRot += 180f;
        }
        Debug.Log("Skybox Colorize: " + colorize);
        SetShaderColor("_FogColor", ref m_skyboxOptions.m_fogColor, fogColor);
		SetShaderFloat("_FogMinHeight", ref m_skyboxOptions.m_fogMinHeight, fogMinHeight);
		SetShaderFloat("_FogMaxHeight", ref m_skyboxOptions.m_fogMaxHeight, fogMaxHeight);
		if (fogTexture == null || fogTexture == Texture2D.whiteTexture)
		{
			fogTexture = Singleton<Game>.Instance.VisualCamera.DefaultSkyboxTexture;
		}
		SetShaderTexture("_FogTexture", ref m_skyboxOptions.m_fogTexture, fogTexture);
		m_skyboxOptions.m_minRot = minRot;
		m_skyboxOptions.m_maxRot = maxRot;
		m_skyboxOptions.m_minHeight = minHeight;
		m_skyboxOptions.m_maxHeight = maxHeight;
		float y = base.transform.position.y;
		y = Mathf.InverseLerp(m_skyboxOptions.m_minHeight, m_skyboxOptions.m_maxHeight, y);
		float num = Mathf.Lerp(m_skyboxOptions.m_minRot, m_skyboxOptions.m_maxRot, y);
		if (SetStruct(ref m_skyboxOptions.m_currentRotation, num))
		{
			Quaternion q = Quaternion.AngleAxis(num, base.transform.right);
			Matrix4x4 value = Matrix4x4.TRS(Vector3.zero, q, new Vector3(1f, 1f, 1f));
			m_skyboxOptions.m_skybox.SetMatrix("_Rotation", value);
			Shader.SetGlobalFloat("_FogRotation", num * 0.004f + 0.2f);
		}


		if (SetStruct(ref m_skyboxOptions.m_colorize, colorize))
		{

            m_skyboxOptions.m_skybox.SetColor(
                "_Tint", colorize
            );
        }
		Texture2D texture2D = DefaultSkyboxTexture;
		Player player = Singleton<Game>.Instance.Player;
		if (player != null && player.Character != null && player.Character.m_skyboxTexture != null)
		{
			texture2D = player.Character.m_skyboxTexture;
		}
		if (texture2D != m_skyboxOptions.m_currentTexture)
		{
			m_skyboxOptions.m_currentTexture = texture2D;
			m_skyboxOptions.m_skybox.SetTexture("_Tex", texture2D);
		}
		__changeEverythingFirstTime = false;
	}

	private void UpdateZoom()
	{
		if (m_zoomOptions.m_canZoom)
		{
			float magnitude = m_target.GetComponent<Rigidbody>().velocity.magnitude;
			magnitude -= m_zoomOptions.m_minSpeed;
			magnitude = Mathf.Max(magnitude, 0f);
			float fieldOfView = m_camera.fieldOfView;
			float normalFOV = m_zoomOptions.m_normalFOV;
			normalFOV = Mathf.Lerp(m_zoomOptions.m_normalFOV, m_zoomOptions.m_zoomFOV, magnitude / m_zoomOptions.m_maxSpeed);
			if (normalFOV > fieldOfView)
			{
				m_zoomOptions.m_smoothTimeCurrent = m_zoomOptions.m_smoothTime * m_zoomOptions.m_zoomOutScale;
			}
			else
			{
				m_zoomOptions.m_smoothTimeCurrent = m_zoomOptions.m_smoothTime;
			}
			m_camera.fieldOfView = Mathf.SmoothDamp(m_camera.fieldOfView, normalFOV, ref m_zoomOptions.m_smoothVelocity, m_zoomOptions.m_smoothTimeCurrent);
		}
	}

	private void AspectRatioChanged()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float num2 = 100000f;
		float a = 0f;
		float num3 = 0f;
		float b = 0f;
		AspectZoomValue[] aspectZoomValues = m_aspectZoomValues;
		foreach (AspectZoomValue aspectZoomValue in aspectZoomValues)
		{
			if (aspectZoomValue.m_aspectRatio < num2)
			{
				num2 = aspectZoomValue.m_aspectRatio;
				a = aspectZoomValue.m_zoom;
			}
			if (aspectZoomValue.m_aspectRatio > num3)
			{
				num3 = aspectZoomValue.m_aspectRatio;
				b = aspectZoomValue.m_zoom;
			}
		}
		float t = (num - num2) / (num3 - num2);
		float num4 = (m_currentOffsetLength = Mathf.Lerp(a, b, t));
		Singleton<Game>.Instance.RayCastCamera.ResetAspect();
		Singleton<Game>.Instance.RayCastCamera.transform.localPosition = m_offsetDirection * num4;
		Singleton<Game>.Instance.RayCastCamera.enabled = false;
	}

	public void ResetTargets(Vector3 pos)
	{
		m_target.transform.position = pos;
		m_target.GetComponent<Rigidbody>().velocity = Vector3.zero;
		m_peekTarget.transform.position = pos;
		m_peekTarget.GetComponent<Rigidbody>().velocity = Vector3.zero;
	}

	public void Reset()
	{
		m_camera.fieldOfView = m_zoomOptions.m_normalFOV;
	}

	public void Shake(string shakeId)
	{
		CameraShake cameraShake = null;
		CameraShake[] cameraShakes = m_cameraShakes;
		foreach (CameraShake cameraShake2 in cameraShakes)
		{
			if (cameraShake2.m_name == shakeId)
			{
				cameraShake = cameraShake2;
				break;
			}
		}
		if (cameraShake != null && m_cameraShake != cameraShake)
		{
			m_cameraShake = cameraShake;
			m_cameraShake.m_active = true;
			m_cameraShake.m_timeActive = 0f;
			m_cameraShake.m_timer = 0f;
			m_cameraShake.m_target = Vector3.zero;
			m_cameraShake.m_onStart.Invoke();
		}
	}

	private void UpdateShake()
	{
		if (m_cameraShake == null || !m_cameraShake.m_active)
		{
			return;
		}
		m_cameraShake.m_timeActive += Time.deltaTime;
		m_cameraShake.m_timer += Time.deltaTime;
		if (m_cameraShake.m_disableTime != 0f && m_cameraShake.m_timeActive >= m_cameraShake.m_disableTime)
		{
			StopShake(m_cameraShake.m_name);
			return;
		}
		float time = Mathf.Clamp(m_cameraShake.m_timeActive / m_cameraShake.m_warmUpTime, 0f, 1f);
		float num = m_cameraShake.m_peroidCurve.Evaluate(time) * m_cameraShake.m_shakePeriod;
		float num2 = m_cameraShake.m_amountCurve.Evaluate(time) * m_cameraShake.m_shakeAmount;
		if (!(m_cameraShake.m_timer < num))
		{
			if (Time.timeScale != 0f)
			{
				Vector3 target = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
				target *= num2;
				m_cameraShake.m_target = target;
				m_cameraShake.m_timer = 0f;
			}
			else
			{
				m_cameraShake.m_target = Vector3.zero;
				m_cameraShake.m_timer = 0f;
			}
		}
	}

	public void StopShake(string shakeId)
	{
		if (m_cameraShake != null && !(m_cameraShake.m_name != shakeId))
		{
			m_cameraShake.m_onStop.Invoke();
			m_cameraShake.m_active = false;
			m_cameraShake = null;
		}
	}

	public void DisconnectFromPlayer()
	{
		base.transform.parent = null;
		Target.Disconnect();
	}

	public void ConnectToPlayer()
	{
		m_target.transform.position = Singleton<Game>.Instance.Player.transform.position;
		m_peekTarget.transform.parent = m_target.transform;
		m_peekTarget.transform.localPosition = Vector3.zero;
		m_target.Init(m_peekTarget);
		m_peekTarget.Init(m_target);
	}

	public void OnPreRender()
	{
		if (!(Camera.current == Camera) || !shadowsOnlyOnPlayer)
		{
			return;
		}
		int num = LayerMask.NameToLayer("Player");
		MeshRenderer[] componentsInChildren = Singleton<Game>.Instance.Player.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			if (meshRenderer.gameObject.layer == num && meshRenderer.shadowCastingMode != ShadowCastingMode.Off)
			{
				meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
			}
		}
	}

	public void OnPostRender()
	{
		if (!(Camera.current == Camera) || !shadowsOnlyOnPlayer)
		{
			return;
		}
		int num = LayerMask.NameToLayer("Player");
		MeshRenderer[] componentsInChildren = Singleton<Game>.Instance.Player.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			if (meshRenderer.gameObject.layer == num && meshRenderer.shadowCastingMode != ShadowCastingMode.Off)
			{
				meshRenderer.shadowCastingMode = ShadowCastingMode.On;
			}
		}
	}
}
