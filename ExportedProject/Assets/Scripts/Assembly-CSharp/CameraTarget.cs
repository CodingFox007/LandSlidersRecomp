using System;
using HeavyDutyInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraTarget : MonoBehaviour
{
	[SerializeField]
	private bool m_isPeekTarget;

	[HideConditional("m_isPeekTarget", false)]
	[SerializeField]
	private AnimationCurve m_dragCurve;

	[HideConditional("m_isPeekTarget", false)]
	[SerializeField]
	private float m_dragTime = 1f;

	private float m_dragTimer;

	[HideConditional("m_isPeekTarget", false)]
	[SerializeField]
	private float m_maxSpeed = 10f;

	[HideConditional("m_isPeekTarget", true)]
	[SerializeField]
	private float m_maxPeekDist = 10f;

	[HideConditional("m_isPeekTarget", true)]
	[SerializeField]
	private float m_peekWallScale = 0.15f;

	[HideConditional("m_isPeekTarget", false)]
	[SerializeField]
	private float m_touchScale = 0.1f;

	[SerializeField]
	private float m_wallTouchScale = 0.7f;

	[SerializeField]
	public float m_touchScaleSpeed = 0.005f;

	private float m_currentTouchScale = 1f;

	[HideConditional("m_isPeekTarget", false)]
	[SerializeField]
	private float m_airWalkTime = 0.5f;

	private float m_airWalkTimer = 0.5f;

	private bool m_airWalking;

	[HideConditional("m_isPeekTarget", false)]
	[SerializeField]
	private float m_minFallHeight = 3f;

	[HideConditional("m_isPeekTarget", false)]
	[SerializeField]
	private float m_velocityResScale = 1f;

	private CameraTarget m_otherTarget;

	private Plane m_groundPlane = new Plane(new Vector3(0f, -1f, 0f), 0f);

	private int m_onGroundCounter;

	private float m_onGroundTimer;

	private float m_groundHeight;

	private string m_groundSurfaceType = string.Empty;

	private float m_actualGroundHeight = float.MaxValue;

	private bool m_touchDown;

	private Vector3 m_touchDownPos = Vector3.zero;

	private Vector3 m_touchDownCurrentPos = Vector3.zero;

	private int m_touchDownFrameCount;

	private Rigidbody m_rigidBody;

	private Vector3 m_invertedCameraTouchDownPos;

	private Collider m_collider;

	private int m_layerMask;

	private Vector3 m_previousMousePosition = Vector3.zero;

	private float m_totalMouseMovement;

	[HideInInspector]
	public bool m_hasBeenDragged;

	public bool IsTouchDown
	{
		get
		{
			return m_touchDown;
		}
	}

	public Vector3 TouchDownPos
	{
		get
		{
			return m_touchDownPos;
		}
	}

	public Vector3 TouchDownCurrentPos
	{
		get
		{
			return m_touchDownCurrentPos;
		}
	}

	public bool IsOnGround
	{
		get
		{
			if (m_isPeekTarget)
			{
				return true;
			}
			return m_onGroundCounter > 0;
		}
	}

	public float DistanceOffGroundActual
	{
		get
		{
			return base.transform.position.y - m_actualGroundHeight;
		}
	}

	public float DistanceOffGround
	{
		get
		{
			return base.transform.position.y - m_groundHeight;
		}
	}

	public float GroundHeight
	{
		get
		{
			return m_groundHeight;
		}
	}

	public string GroundSurfaceType
	{
		get
		{
			return m_groundSurfaceType;
		}
	}

	public bool IsAirWalking()
	{
		return m_airWalking;
	}

	private void Start()
	{
		m_layerMask = 1 << LayerMask.NameToLayer("Ground");
		m_layerMask |= 1 << LayerMask.NameToLayer("Water");
		m_hasBeenDragged = false;
		m_currentTouchScale = m_touchScale;
		m_rigidBody = base.gameObject.GetComponent<Rigidbody>();
		m_collider = base.gameObject.GetComponent<Collider>();
	}

	public void Init(CameraTarget otherTarget)
	{
		m_hasBeenDragged = false;
		m_otherTarget = otherTarget;
		FixedJoint component = base.gameObject.GetComponent<FixedJoint>();
		if (component != null)
		{
			component.connectedBody = Singleton<Game>.Instance.Player.GetComponent<Rigidbody>();
		}
	}

	public void Disconnect()
	{
		FixedJoint component = base.gameObject.GetComponent<FixedJoint>();
		if (component != null)
		{
			component.connectedBody = null;
		}
	}

	private void Update()
	{
		m_airWalkTimer -= Time.deltaTime;
		m_airWalkTimer = Mathf.Max(m_airWalkTimer, 0f);
		m_dragTimer += Time.deltaTime;
		m_dragTimer = Mathf.Min(m_dragTimer, m_dragTime);
		if (m_currentTouchScale < m_touchScale && !Singleton<Game>.Instance.Player.IsCollidingWall)
		{
			m_currentTouchScale += m_touchScaleSpeed;
			m_currentTouchScale = Mathf.Min(m_currentTouchScale, m_touchScale);
		}
	}

	private void UpdateInput()
	{
		if (!Singleton<Game>.Instance.IsLoaded)
		{
			m_touchDownPos = Vector3.zero;
			m_touchDown = false;
		}
		else if (Singleton<Game>.Instance.Player.BlockInput || Singleton<Game>.Instance.Player.IsBeingKnocked || Singleton<Game>.Instance.Player.IsFrozen || Singleton<Game>.Instance.UIController.CurrentState == UIController.State.NewBest || Singleton<Game>.Instance.UIController.CurrentState == UIController.State.LevelChange || UIHud.m_waitAFrameBestScore || Singleton<Game>.Instance.UIController.HUD.m_newBestCheck.activeInHierarchy)
		{
			m_touchDownPos = Vector3.zero;
			m_touchDown = false;
		}
		else if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject())
		{
			m_touchDownPos = Vector3.zero;
			m_touchDown = false;
		}
		else if (Input.GetMouseButtonUp(0) || !Input.GetMouseButton(0))
		{
			m_touchDownPos = Vector3.zero;
			m_touchDown = false;
			if (m_totalMouseMovement < 2f)
			{
				m_dragTimer = m_dragTime;
			}
		}
		else
		{
			if (!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
			{
				return;
			}
			Ray ray = Singleton<Game>.Instance.RayCastCamera.ScreenPointToRay(Input.mousePosition);
			m_groundPlane.distance = Singleton<Game>.Instance.Player.transform.position.y;
			float enter = 0f;
			if (!m_groundPlane.Raycast(ray, out enter))
			{
				return;
			}
			if (!m_touchDown)
			{
				m_totalMouseMovement = 0f;
				m_previousMousePosition = Input.mousePosition;
				m_touchDown = true;
				m_touchDownFrameCount = 0;
				m_touchDownPos = ray.GetPoint(enter);
				if (Singleton<Game>.Instance.Player.IsInverted)
				{
					m_invertedCameraTouchDownPos = Singleton<Game>.Instance.VisualCamera.Camera.transform.position;
				}
			}
			else
			{
				m_touchDownFrameCount++;
			}
			m_touchDownCurrentPos = ray.GetPoint(enter);
			if (Singleton<Game>.Instance.Player.IsInverted)
			{
				Vector3 vector = m_invertedCameraTouchDownPos - Singleton<Game>.Instance.VisualCamera.Camera.transform.position;
				m_touchDownCurrentPos = m_touchDownCurrentPos + vector + vector;
				Vector3 vector2 = m_touchDownPos - m_touchDownCurrentPos;
				m_touchDownCurrentPos = m_touchDownCurrentPos + vector2 + vector2;
			}
			m_totalMouseMovement += (m_previousMousePosition - Input.mousePosition).magnitude;
			m_previousMousePosition = Input.mousePosition;
			if (m_totalMouseMovement > 1f)
			{
				m_hasBeenDragged = true;
			}
		}
	}

	private void FixedUpdate()
	{
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.LevelChange)
		{
			m_hasBeenDragged = false;
		}
		if (m_onGroundTimer > 0f)
		{
			m_onGroundTimer -= Time.fixedDeltaTime;
		}
		else
		{
			m_onGroundCounter = 0;
		}
		UpdateInput();
		m_groundHeight = CalculateGroundHeight(out m_groundSurfaceType);
		if (m_isPeekTarget)
		{
			m_groundHeight = m_otherTarget.m_groundHeight;
		}
		Vector3 velocity = m_rigidBody.velocity;
		float drag = m_rigidBody.drag;
		if (m_touchDown)
		{
			if (Singleton<Game>.Instance.Player.IsCollidingWall)
			{
				m_currentTouchScale = m_touchScale * m_wallTouchScale;
			}
			if (m_isPeekTarget && Singleton<Game>.Instance.Player.IsCollidingWall && m_touchDown)
			{
				Vector3 vector = m_touchDownCurrentPos - m_otherTarget.m_touchDownPos;
				vector = vector.normalized * (vector.magnitude * m_peekWallScale);
				m_touchDownPos = m_otherTarget.m_touchDownPos + vector;
			}
			Vector3 vector2 = m_touchDownCurrentPos - m_touchDownPos;
			vector2 *= m_currentTouchScale;
			vector2.y = 0f;
			float maxSpeedBasedOnResolution = GetMaxSpeedBasedOnResolution(vector2);
			if (vector2.magnitude > maxSpeedBasedOnResolution)
			{
				vector2 = vector2.normalized * maxSpeedBasedOnResolution;
			}
			if (m_touchDownFrameCount > 0)
			{
				float y = velocity.y;
				velocity = -vector2 / Time.deltaTime;
				velocity.y = y;
				drag = 0f;
				m_dragTimer = 0f;
			}
		}
		else
		{
			drag = m_dragCurve.Evaluate(m_dragTimer / m_dragTime);
		}
		if (m_rigidBody.useGravity || Singleton<Game>.Instance.Player.IsFrozen)
		{
			drag = 0f;
		}
		UpdatePeeking();
		velocity = UpdateFallingAndAirWalk(velocity);
		if (Singleton<Game>.Instance.Player.IsDead)
		{
			velocity.x = 0f;
			velocity.z = 0f;
		}
		if (m_collider != null)
		{
		}
		m_rigidBody.velocity = velocity;
		m_rigidBody.drag = drag;
	}

	private float CalculateGroundHeight(out string surfaceType)
	{
		surfaceType = "Air";
		float result = float.MaxValue;
		m_actualGroundHeight = float.MaxValue;
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(base.transform.position + new Vector3(0f, 10f, 0f), -base.transform.up.normalized), out hitInfo, 50f, m_layerMask))
		{
			surfaceType = hitInfo.collider.tag;
			bool flag = hitInfo.collider.tag.StartsWith("Ground", StringComparison.Ordinal);
			if (flag)
			{
				result = hitInfo.point.y;
			}
			if (flag || hitInfo.collider.tag.StartsWith("Water", StringComparison.Ordinal))
			{
				m_actualGroundHeight = hitInfo.point.y;
			}
		}
		return result;
	}

	private float GetMaxSpeedBasedOnResolution(Vector3 velocity)
	{
		if (velocity.magnitude <= 0.1f)
		{
			return 0f;
		}
		float maxSpeed = m_maxSpeed;
		Vector3 right = Singleton<Game>.Instance.VisualCamera.transform.right;
		Vector3 forward = Singleton<Game>.Instance.VisualCamera.transform.forward;
		forward.y = 0f;
		right = Vector3.Project(velocity, right);
		forward = Vector3.Project(velocity, forward);
		if (Screen.width < Screen.height)
		{
			return Mathf.Lerp(m_maxSpeed, m_maxSpeed * m_velocityResScale, right.magnitude / forward.magnitude);
		}
		return Mathf.Lerp(m_maxSpeed, m_maxSpeed * m_velocityResScale, forward.magnitude / right.magnitude);
	}

	private Vector3 UpdateFallingAndAirWalk(Vector3 velocity)
	{
		if (Singleton<Game>.Instance.Player.IsCinematicMove)
		{
			m_rigidBody.useGravity = false;
		}
		if (Singleton<Game>.Instance.Player.IsBetweenLevels || Singleton<Game>.Instance.Player.IsCinematicMove)
		{
			return velocity;
		}
		bool flag = m_rigidBody.useGravity || Singleton<Game>.Instance.Player.IsBeingKnocked;
		float num = base.transform.position.y - m_groundHeight;
		if ((m_groundHeight == float.MaxValue && !IsOnGround) || (!IsOnGround && num > m_minFallHeight))
		{
			if (m_airWalkTimer > 0f)
			{
				m_airWalking = true;
				flag = false;
			}
			else if (!Singleton<Game>.Instance.Player.GodMode || m_groundHeight != float.MaxValue)
			{
				m_airWalking = false;
				velocity.x = 0f;
				velocity.z = 0f;
				if (m_groundHeight == float.MaxValue)
				{
					Singleton<Game>.Instance.Player.Kill(null);
				}
			}
		}
		else
		{
			m_airWalking = false;
			m_airWalkTimer = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Hover Time", m_airWalkTime);
		}
		if (!m_isPeekTarget)
		{
			if (IsOnGround || m_airWalking || !Singleton<Game>.Instance.LevelManager.IsLoaded)
			{
				velocity.y = 0f;
				flag = false;
			}
			else
			{
				flag = true;
			}
		}
		else
		{
			flag = false;
		}
		m_rigidBody.useGravity = flag;
		if (!m_isPeekTarget && !Singleton<Game>.Instance.Player.IsDead)
		{
			Singleton<Game>.Instance.Player.UseGravity = flag;
		}
		return velocity;
	}

	public void ResetAirWalkingToAir()
	{
		m_airWalkTimer = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Hover Time", m_airWalkTime);
		m_airWalking = true;
	}

	private void UpdatePeeking()
	{
		if (!m_isPeekTarget)
		{
			return;
		}
		if (Singleton<Game>.Instance.Player.IsCollidingWall && m_touchDown)
		{
			Vector3 vector = new Vector3(base.transform.position.x, m_otherTarget.transform.position.y, base.transform.position.z);
			Vector3 vector2 = vector - m_otherTarget.transform.position;
			Vector3 vector3 = m_touchDownPos - m_touchDownCurrentPos;
			float num = Mathf.Lerp(0f, m_maxPeekDist, vector3.magnitude / m_maxPeekDist);
			if (vector2.magnitude > num)
			{
				vector2 = vector2.normalized * num;
			}
			GetComponent<Rigidbody>().MovePosition(m_otherTarget.transform.position + vector2);
		}
		else
		{
			base.transform.localPosition = Vector3.zero;
		}
	}

	private void OnDrawGizmos()
	{
		if (m_touchDown)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(m_touchDownPos, 1f);
			Gizmos.DrawSphere(m_touchDownCurrentPos, 0.25f);
		}
	}

	private void OnCollisionEnter(Collision collisionInfo)
	{
		if (collisionInfo.collider.tag.StartsWith("Ground", StringComparison.Ordinal))
		{
			m_onGroundTimer = 0.1f;
			m_onGroundCounter++;
		}
	}

	private void OnCollisionStay(Collision collisionInfo)
	{
		if (collisionInfo.collider.tag.StartsWith("Ground", StringComparison.Ordinal))
		{
			m_onGroundTimer = 0.1f;
		}
	}

	private void OnCollisionExit(Collision collisionInfo)
	{
		if (collisionInfo.collider.tag.StartsWith("Ground", StringComparison.Ordinal))
		{
			m_onGroundCounter--;
		}
	}
}
