using System;
using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityStandardAssets.Utility;

public class Entity : MonoBehaviour, IEventSystemHandler, ISerializationCallbackReceiver
{
	public enum State
	{
		Idle = 0,
		AlertChase = 1,
		Chase = 2,
		AlertFlee = 3,
		Flee = 4,
		Stunned = 5
	}

	public enum DetectionType
	{
		None = 0,
		Distance = 1,
		FOV = 2,
		Infinite = 3
	}

	[Serializable]
	public class DetectionOptions
	{
		public DetectionType m_detectionType;

		public float m_startRange;

		public float m_endRange;

		public float m_fovAngle;

		public float m_fovRange;

		public string m_distancePerkID;
	}

	public enum IdleType
	{
		Sleep = 0,
		Wander = 1
	}

	[Serializable]
	public class IdleOptions
	{
		public IdleType m_idleType;

		public RandomRange m_turnFequency;

		[NonSerialized]
		public float m_turnTimer;

		public float m_turnSpeed;

		public RandomRange m_stopFrequency;

		public RandomRange m_stopTime;

		[NonSerialized]
		public bool m_isStopped;

		[NonSerialized]
		public float m_stopTimer;

		public float m_accel;

		[NonSerialized]
		public float m_speed;

		public List<string> m_maxSpeedPerkIds;

		public float m_maxSpeed;
	}

	public enum ChaseType
	{
		None = 0,
		Chase = 1,
		Charge = 2,
		Bound = 3
	}

	[Serializable]
	public class ChaseOptions
	{
		public ChaseType m_chaseType;

		public float m_turnSpeed;

		public float m_turnPercentage = 1f;

		public float m_turnStoppedSpeed;

		public float m_accel;

		[NonSerialized]
		public float m_speed;

		public float m_maxSpeed;

		public List<string> m_maxSpeedPerkIds;

		public float m_jumpSpeed;

		public float m_chargeTime;

		public List<string> m_chargePerkIds;

		public DetectionOptions m_detectionOptions;

		[SerializeField]
		public UnityEvent m_onEnter = new UnityEvent();

		[SerializeField]
		public UnityEvent m_onExit = new UnityEvent();
	}

	public enum FleeType
	{
		None = 0,
		Flee = 1,
		Bound = 2
	}

	[Serializable]
	public class FleeOptions
	{
		public FleeType m_fleeType;

		public bool m_fleeFromPlayer;

		public bool m_fleeFromInvinciblePlayer;

		public RandomRange m_turnFequency;

		[NonSerialized]
		public float m_turnTimer;

		public float m_turnSpeed;

		public float m_turnStoppedSpeed;

		public float m_accel;

		[NonSerialized]
		public float m_speed;

		public float m_maxSpeed;

		public List<string> m_maxSpeedPerkIds;

		public float m_jumpSpeed;

		public float m_chargeTime;

		public List<string> m_chargePerkIds;

		public DetectionOptions m_detectionOptions;

		[SerializeField]
		public UnityEvent m_onEnter = new UnityEvent();

		[SerializeField]
		public UnityEvent m_onExit = new UnityEvent();
	}

	[Serializable]
	public class AlertOptions
	{
		public float m_alertTime;
	}

	[Serializable]
	public class EdgeOptions
	{
		public bool m_stopBeforeEdges;

		public float m_stopDistance = 2f;

		public float m_detectWidth = 2f;
	}

	[Serializable]
	public class DestructiveOptions
	{
		public bool m_canDie = true;

		public bool m_canKill = true;

		public bool m_killableByPlayer;

		public float m_strength;

		public float m_fallDeathHeight = -25f;

		public GameObject m_spawnObject;

		public RandomRange m_spawnCount;

		public string m_spawnCountPerkID;

		[TagList(true)]
		public List<string> m_destroyTags;

		[SerializeField]
		public UnityEvent m_onKillEntity = new UnityEvent();

		[SerializeField]
		public UnityEvent m_onDeath = new UnityEvent();

		[SerializeField]
		public UnityEvent m_onWasKilledByEntity = new UnityEvent();
	}

	public enum AvoidType
	{
		None = 0,
		Rotate = 1,
		Teleport = 2,
		MoveVertically = 3,
		Delay = 4
	}

	[Serializable]
	public class AvoidBlockOptions
	{
		public AvoidType m_avoidType;

		public float m_delayTime;

		[NonSerialized]
		public float m_delayTimer;

		public float m_speed;

		[TagList(true)]
		public List<string> m_triggerTags;

		public bool m_triggerOnStay;

		[NonSerialized]
		public bool m_active;

		[NonSerialized]
		public Vector3 m_target;

		[SerializeField]
		public UnityEvent m_onEnter = new UnityEvent();

		[SerializeField]
		public UnityEvent m_onExit = new UnityEvent();
	}

	[Serializable]
	public class KnockbackOptions
	{
		public float m_force = 50000f;

		public State m_stateOnExit;

		public float m_verticalForce = 100000f;
	}

	public class AnimationTriggerState
	{
		public float m_triggeredAtAnimationTime = -100f;

		public bool m_shouldReset;
	}

	[SerializeField]
	private string m_id;

	[SerializeField]
	private Sprite m_icon;

	[SerializeField]
	private bool m_enemyForMissions = true;

	[SerializeField]
	private IdleOptions m_idleOptions;

	[SerializeField]
	private AlertOptions m_alertChaseOptions;

	[SerializeField]
	private ChaseOptions m_chaseOptions;

	[SerializeField]
	private AlertOptions m_alertFleeOptions;

	[SerializeField]
	private FleeOptions m_fleeOptions;

	[SerializeField]
	private EdgeOptions m_edgeOptions;

	[SerializeField]
	private DestructiveOptions m_destructiveOptions;

	[SerializeField]
	private AvoidBlockOptions m_avoidBlockOptions;

	[SerializeField]
	private KnockbackOptions m_knockbackOptions;

	[SerializeField]
	private bool m_scaleOnAlive;

	[SerializeField]
	private AnimationCurve m_scaleOnAliveCurve;

	[SerializeField]
	private float m_scaleOnAliveTime;

	[SerializeField]
	private float m_selfDestructAfterTime;

	private float m_selfDestructTimer;

	private float m_aliveTime;

	private float m_scaleUpTime;

	[Dictionary("m_deathParticlesValues")]
	public List<string> m_deathParticles;

	[HideInInspector]
	public List<ParticleSystem> m_deathParticlesValues;

	private Dictionary<string, ParticleSystem> m_deathParticle;

	private StateMachine<State> m_state = new StateMachine<State>();

	private float m_stateTimer;

	[SerializeField]
	private ParticleSystem m_selfDestroyParticles;

	private bool m_selfDestroy;

	private GameObject m_target;

	private GameObject m_preferredTarget;

	private Quaternion m_turnTarget;

	private Rigidbody m_rigidBody;

	private bool m_dead;

	private Animator m_animator;

	private int m_onGroundCounter;

	private float m_onGroundTimer;

	private float m_animationTimer;

	private bool m_avoidBlockDisabled;

	private Dictionary<string, AnimationTriggerState> m_animationTriggerStates = new Dictionary<string, AnimationTriggerState>();

	private const float minTimeBeforeTriggerReset = 0.02f;

	public bool IsOnGround
	{
		get
		{
			return m_onGroundCounter > 0;
		}
	}

	public bool IsDead
	{
		get
		{
			return m_dead;
		}
	}

	public string Id
	{
		get
		{
			return m_id;
		}
	}

	public Sprite Icon
	{
		get
		{
			return m_icon;
		}
	}

	public bool KillableByPlayer
	{
		get
		{
			return m_destructiveOptions.m_killableByPlayer;
		}
	}

	public bool AvoidBlockDisabled
	{
		get
		{
			return m_avoidBlockDisabled;
		}
		set
		{
			m_avoidBlockDisabled = value;
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_deathParticles, m_deathParticlesValues, out m_deathParticle, "deathParticles");
	}

	public void LaunchIntoSpace()
	{
		Debug.Log("LaunchIntoSpace: " + base.gameObject.name);
		m_rigidBody.useGravity = false;
		m_rigidBody.mass = 0.1f;
		m_rigidBody.velocity = new Vector3(0f, 0f, 0f);
		m_rigidBody.AddTorque(new Vector3(0f, 0f, UnityEngine.Random.Range(-1, 1)), ForceMode.Impulse);
	}

	private void Start()
	{
		m_rigidBody = GetComponent<Rigidbody>();
		m_animator = GetComponent<Animator>();
		m_state.AddState(State.Idle, IdleEnter, Idle, IdleExit);
		m_state.AddState(State.AlertChase, AlertChaseEnter, AlertChase, AlertChaseExit);
		m_state.AddState(State.Chase, ChaseEnter, Chase, ChaseExit);
		m_state.AddState(State.AlertFlee, AlertFleeEnter, AlertFlee, AlertFleeExit);
		m_state.AddState(State.Flee, FleeEnter, Flee, FleeExit);
		m_state.AddState(State.Stunned, StunnedEnter, Stunned, StunnedExit);
		m_dead = false;
		m_avoidBlockDisabled = false;
		m_state.CurrentState = State.Idle;
		m_scaleUpTime = 0f;
		if (m_scaleOnAlive)
		{
			float num = m_scaleOnAliveCurve.Evaluate(0f);
			base.transform.localScale = new Vector3(num, num, num);
		}
	}

	private AnimationTriggerState GetTriggerState(string name)
	{
		AnimationTriggerState value = null;
		if (!m_animationTriggerStates.TryGetValue(name, out value))
		{
			value = new AnimationTriggerState();
			m_animationTriggerStates[name] = value;
		}
		return value;
	}

	private void SetAnimatorTrigger(string name)
	{
		if (m_animator != null)
		{
			AnimationTriggerState triggerState = GetTriggerState(name);
			if (triggerState.m_shouldReset)
			{
				Debug.LogWarning("Pretty sure this trigger should have been reset (" + name + ")");
				m_animator.ResetTrigger(name);
				triggerState.m_shouldReset = false;
			}
			triggerState.m_triggeredAtAnimationTime = m_animationTimer;
			m_animator.SetTrigger(name);
		}
	}

	private void ResetAnimatorTrigger(string name)
	{
		if (m_animator != null)
		{
			AnimationTriggerState triggerState = GetTriggerState(name);
			float num = m_animationTimer - triggerState.m_triggeredAtAnimationTime;
			if (num > 0.02f)
			{
				m_animator.ResetTrigger(name);
				triggerState.m_triggeredAtAnimationTime = -100f;
				triggerState.m_shouldReset = false;
			}
			else
			{
				triggerState.m_shouldReset = true;
			}
		}
	}

	private void Update()
	{
		foreach (KeyValuePair<string, AnimationTriggerState> animationTriggerState in m_animationTriggerStates)
		{
			if (animationTriggerState.Value.m_shouldReset)
			{
				ResetAnimatorTrigger(animationTriggerState.Key);
			}
		}
		if (m_scaleOnAlive && m_scaleOnAliveTime > 0f && m_scaleUpTime <= m_scaleOnAliveTime)
		{
			float num = m_scaleOnAliveCurve.Evaluate(m_scaleUpTime / m_scaleOnAliveTime);
			base.transform.localScale = new Vector3(num, num, num);
			m_scaleUpTime += Time.deltaTime;
		}
		m_animationTimer += Time.deltaTime;
		if (m_selfDestructAfterTime > 0f && m_selfDestructTimer < m_selfDestructAfterTime && !m_dead)
		{
			m_selfDestructTimer += Time.deltaTime;
			if (m_selfDestructTimer >= m_selfDestructAfterTime)
			{
				m_selfDestroy = true;
				Kill(null);
			}
		}
		m_aliveTime += Time.deltaTime;
	}

	private void FixedUpdate()
	{
		if (m_onGroundTimer > 0f)
		{
			m_onGroundTimer -= Time.fixedDeltaTime;
		}
		else
		{
			m_onGroundCounter = 0;
		}
		Vector3 velocity = m_rigidBody.velocity;
		bool flag = velocity.magnitude > 0.01f;
		bool flag2 = Quaternion.Angle(base.transform.rotation, m_turnTarget) > 0.1f;
		if (!Singleton<Game>.Instance.Player.HasFeezePower)
		{
			m_state.Update();
		}
		else
		{
			Rigidbody component = GetComponent<Rigidbody>();
			if (component != null)
			{
				component.useGravity = m_onGroundTimer <= 0f;
			}
		}
		if (!IsOnGround)
		{
			if (!m_edgeOptions.m_stopBeforeEdges && flag && !m_avoidBlockDisabled)
			{
				string surfaceType = string.Empty;
				float num = CalculateGroundHeight(base.transform.position, out surfaceType);
				if (num == float.MaxValue)
				{
					velocity.y = m_rigidBody.velocity.y;
					m_rigidBody.velocity = velocity;
				}
			}
			DeathCheck();
		}
		if (!(m_animator != null))
		{
			return;
		}
		AnimatorControllerParameter[] parameters = m_animator.parameters;
		foreach (AnimatorControllerParameter animatorControllerParameter in parameters)
		{
			if (animatorControllerParameter.name == "IsMoving")
			{
				m_animator.SetBool("IsMoving", flag || flag2);
				break;
			}
		}
	}

	private void IdleEnter()
	{
		m_stateTimer = 0f;
		m_idleOptions.m_stopFrequency.Randomize();
		m_idleOptions.m_stopTime.Randomize();
		m_idleOptions.m_turnFequency.Randomize();
		m_idleOptions.m_speed = 0f;
		m_idleOptions.m_isStopped = false;
		m_idleOptions.m_stopTimer = 0f;
		m_idleOptions.m_turnTimer = 0f;
		m_turnTarget = base.transform.rotation;
		SetAnimatorTrigger("Idle");
	}

	private void Idle()
	{
		m_stateTimer += Time.fixedDeltaTime;
		if (m_chaseOptions.m_chaseType != ChaseType.None && DistanceCheck(m_chaseOptions.m_detectionOptions, true))
		{
			m_target = GetTarget();
			m_state.CurrentState = State.AlertChase;
			return;
		}
		if ((m_fleeOptions.m_fleeFromPlayer || (m_fleeOptions.m_fleeFromInvinciblePlayer && Singleton<Game>.Instance.Player.Invincible)) && DistanceCheck(m_fleeOptions.m_detectionOptions, true))
		{
			m_target = GetTarget();
			m_state.CurrentState = State.AlertFlee;
			return;
		}
		switch (m_idleOptions.m_idleType)
		{
		case IdleType.Sleep:
			IdleSleep();
			break;
		case IdleType.Wander:
			IdleWander();
			break;
		}
	}

	private void IdleSleep()
	{
	}

	private void IdleWander()
	{
		Vector3 velocity = m_rigidBody.velocity;
		Vector3 vel = velocity;
		if (m_stateTimer > 1f && !m_idleOptions.m_isStopped)
		{
			bool avoidComplete = false;
			vel = AvoidBlockUpdate(vel, out avoidComplete);
		}
		if (!m_avoidBlockOptions.m_active)
		{
			m_idleOptions.m_turnTimer += Time.fixedDeltaTime;
			if (m_idleOptions.m_turnTimer > m_idleOptions.m_turnFequency.Value)
			{
				m_turnTarget = Quaternion.Lerp(Quaternion.LookRotation(-base.transform.right, base.transform.up), Quaternion.LookRotation(base.transform.right, base.transform.up), UnityEngine.Random.Range(0f, 1f));
				m_idleOptions.m_turnTimer = 0f;
				m_idleOptions.m_turnFequency.Randomize();
			}
			if (m_idleOptions.m_stopFrequency.Value != 0f && m_idleOptions.m_stopTime.Value != 0f)
			{
				m_idleOptions.m_stopTimer += Time.fixedDeltaTime;
				if (!m_idleOptions.m_isStopped)
				{
					if (m_idleOptions.m_stopTimer > m_idleOptions.m_stopFrequency.Value)
					{
						m_idleOptions.m_isStopped = true;
						m_idleOptions.m_stopTimer = 0f;
						m_idleOptions.m_stopFrequency.Randomize();
					}
				}
				else if (m_idleOptions.m_stopTimer > m_idleOptions.m_stopTime.Value)
				{
					m_idleOptions.m_isStopped = false;
					m_idleOptions.m_stopTimer = 0f;
					m_idleOptions.m_stopTime.Randomize();
				}
			}
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, m_turnTarget, m_idleOptions.m_turnSpeed * Time.fixedDeltaTime * 60f);
		}
		Vector3 forward = base.transform.forward;
		forward.y = 0f;
		if (!m_avoidBlockOptions.m_active)
		{
			m_idleOptions.m_speed += m_idleOptions.m_accel;
		}
		vel = forward.normalized * m_idleOptions.m_speed;
		float num = m_idleOptions.m_maxSpeed;
		foreach (string maxSpeedPerkId in m_idleOptions.m_maxSpeedPerkIds)
		{
			num = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue(maxSpeedPerkId, num);
		}
		if (vel.magnitude > num)
		{
			vel = vel.normalized * num;
		}
		if (EdgeCheck(m_edgeOptions.m_stopDistance) || m_idleOptions.m_isStopped)
		{
			vel = Vector3.zero;
		}
		vel.y = velocity.y;
		m_rigidBody.velocity = vel;
	}

	private void IdleExit()
	{
		ResetAnimatorTrigger("Idle");
	}

	private void AlertChaseEnter()
	{
		m_stateTimer = 0f;
		SetAnimatorTrigger("AlertChase");
		Singleton<Game>.Instance.StatsManager.GetStat("EnemyAlerted").IncreaseStat(1f);
	}

	private void AlertChase()
	{
		m_stateTimer += Time.fixedDeltaTime;
		if (m_stateTimer >= m_alertChaseOptions.m_alertTime)
		{
			m_state.CurrentState = State.Chase;
		}
	}

	private void AlertChaseExit()
	{
		ResetAnimatorTrigger("AlertChase");
	}

	private void ChaseEnter()
	{
		m_stateTimer = 0f;
		m_chaseOptions.m_speed = 0f;
		m_turnTarget = base.transform.rotation;
		SetAnimatorTrigger("Chase");
		m_chaseOptions.m_onEnter.Invoke();
	}

	private void Chase()
	{
		m_stateTimer += Time.fixedDeltaTime;
		if ((m_fleeOptions.m_fleeFromPlayer || (m_fleeOptions.m_fleeFromInvinciblePlayer && Singleton<Game>.Instance.Player.Invincible)) && DistanceCheck(m_fleeOptions.m_detectionOptions, true))
		{
			m_target = GetTarget();
			m_state.CurrentState = State.Flee;
			return;
		}
		switch (m_chaseOptions.m_chaseType)
		{
		case ChaseType.Chase:
			ChaseChase();
			break;
		case ChaseType.Charge:
			ChaseCharge();
			break;
		case ChaseType.Bound:
			ChaseBound();
			break;
		}
	}

	private void ChaseChase()
	{
		Vector3 velocity = m_rigidBody.velocity;
		Vector3 vector = velocity;
		m_target = GetTarget();
		if (m_target == null || !DistanceCheck(m_chaseOptions.m_detectionOptions, false))
		{
			m_target = null;
		}
		if (m_target != null)
		{
			if (m_stateTimer > 1f)
			{
				bool avoidComplete = false;
				vector = AvoidBlockUpdate(vector, out avoidComplete);
				if (avoidComplete && !m_avoidBlockDisabled)
				{
					m_chaseOptions.m_speed = 0f;
				}
			}
			if (!m_avoidBlockOptions.m_active)
			{
				Vector3 position = m_target.transform.position;
				Vector3 vector2 = position - base.transform.position;
				vector2.y = 0f;
				m_turnTarget = Quaternion.LookRotation(vector2.normalized, base.transform.up);
				float num = Quaternion.Angle(base.transform.rotation, m_turnTarget);
				float a = num * (1f - Mathf.Pow(1f - Mathf.Clamp01(m_chaseOptions.m_turnPercentage), 60f * Time.fixedDeltaTime));
				a = Mathf.Min(a, m_chaseOptions.m_turnSpeed * 60f * Time.fixedDeltaTime);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, m_turnTarget, a);
				Vector3 forward = base.transform.forward;
				forward.y = 0f;
				m_chaseOptions.m_speed += m_chaseOptions.m_accel;
				vector = forward.normalized * m_chaseOptions.m_speed;
				float num2 = m_chaseOptions.m_maxSpeed;
				foreach (string maxSpeedPerkId in m_chaseOptions.m_maxSpeedPerkIds)
				{
					num2 = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue(maxSpeedPerkId, num2);
				}
				if (vector.magnitude > num2)
				{
					vector = vector.normalized * num2;
				}
			}
		}
		else
		{
			if (velocity.magnitude < 0.1f)
			{
				m_state.CurrentState = State.Idle;
				return;
			}
			vector *= 0.95f;
		}
		if (EdgeCheck(m_edgeOptions.m_stopDistance))
		{
			m_chaseOptions.m_speed = 0f;
			vector = Vector3.zero;
		}
		vector.y = velocity.y;
		m_rigidBody.velocity = vector;
	}

	private void ChaseCharge()
	{
		Vector3 velocity = m_rigidBody.velocity;
		Vector3 vector = velocity;
		m_target = GetTarget();
		if (m_target != null)
		{
			Vector3 position = m_target.transform.position;
			Vector3 vector2 = position - base.transform.position;
			vector2.y = 0f;
			if (m_stateTimer < m_chaseOptions.m_chargeTime)
			{
				vector = Vector3.zero;
				m_turnTarget = Quaternion.LookRotation(vector2.normalized, base.transform.up);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, m_turnTarget, m_chaseOptions.m_turnStoppedSpeed * Time.fixedDeltaTime * 60f);
				m_animator.SetBool("ChargingUp", true);
			}
			else
			{
				if (m_animator.GetBool("ChargingUp"))
				{
					Singleton<Game>.Instance.StatsManager.GetStat(m_id + "Charged").IncreaseStat(1f);
				}
				m_animator.SetBool("ChargingUp", false);
				if (m_stateTimer > m_chaseOptions.m_chargeTime + 0.1f)
				{
					bool avoidComplete = false;
					vector = AvoidBlockUpdate(vector, out avoidComplete);
					if (m_avoidBlockOptions.m_active)
					{
						m_state.CurrentState = State.Stunned;
						return;
					}
				}
				if (!m_avoidBlockOptions.m_active)
				{
					m_turnTarget = Quaternion.LookRotation(vector2.normalized, base.transform.up);
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, m_turnTarget, m_chaseOptions.m_turnSpeed * Time.fixedDeltaTime * 60f);
					Vector3 forward = base.transform.forward;
					forward.y = 0f;
					m_chaseOptions.m_speed += m_chaseOptions.m_accel;
					vector = forward.normalized * m_chaseOptions.m_speed;
					if (vector.magnitude > m_chaseOptions.m_maxSpeed)
					{
						vector = vector.normalized * m_chaseOptions.m_maxSpeed;
					}
				}
			}
		}
		else
		{
			if (velocity.magnitude < 0.1f)
			{
				m_state.CurrentState = State.Idle;
				return;
			}
			vector *= 0.95f;
		}
		if (EdgeCheck(m_edgeOptions.m_stopDistance))
		{
			m_chaseOptions.m_speed = 0f;
			vector = Vector3.zero;
		}
		vector.y = velocity.y;
		m_rigidBody.velocity = vector;
	}

	private void ChaseBound()
	{
		Vector3 velocity = m_rigidBody.velocity;
		Vector3 velocity2 = velocity;
		string surfaceType = string.Empty;
		float num = CalculateGroundHeight(base.transform.position, out surfaceType);
		float num2 = base.transform.position.y - num;
		if (num == float.MaxValue)
		{
			num2 = float.MaxValue;
		}
		m_target = GetTarget();
		if (m_target == null || (num2 <= 0.1f && !DistanceCheck(m_chaseOptions.m_detectionOptions, false)))
		{
			m_target = null;
		}
		if (m_target != null)
		{
			Vector3 position = m_target.transform.position;
			Vector3 vector = position - base.transform.position;
			vector.y = 0f;
			float num3 = m_chaseOptions.m_chargeTime;
			foreach (string chargePerkId in m_chaseOptions.m_chargePerkIds)
			{
				num3 = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue(chargePerkId, num3, true);
			}
			if (num2 > 0.1f)
			{
				m_stateTimer = 0f;
			}
			else if (m_stateTimer < num3)
			{
				velocity2 = Vector3.zero;
				m_turnTarget = Quaternion.LookRotation(vector.normalized, base.transform.up);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, m_turnTarget, m_chaseOptions.m_turnStoppedSpeed * Time.fixedDeltaTime * 60f);
				m_animator.SetBool("ChargingUp", true);
			}
			else
			{
				m_animator.SetBool("ChargingUp", false);
				if (velocity2.magnitude <= 0.01f)
				{
					Vector3 forward = base.transform.forward;
					forward.y = 0f;
					velocity2 = forward.normalized * m_chaseOptions.m_maxSpeed;
					velocity.y = m_chaseOptions.m_jumpSpeed;
				}
			}
		}
		else
		{
			if (velocity.magnitude < 0.1f)
			{
				m_state.CurrentState = State.Idle;
				return;
			}
			velocity2 *= 0.95f;
		}
		velocity2.y = velocity.y;
		m_rigidBody.velocity = velocity2;
	}

	private void ChaseExit()
	{
		if (m_animator != null)
		{
			ResetAnimatorTrigger("Chase");
			m_animator.SetBool("ChargingUp", false);
		}
		m_chaseOptions.m_onExit.Invoke();
	}

	private void AlertFleeEnter()
	{
		m_stateTimer = 0f;
		SetAnimatorTrigger("AlertFlee");
	}

	private void AlertFlee()
	{
		m_stateTimer += Time.fixedDeltaTime;
		if (m_stateTimer >= m_alertFleeOptions.m_alertTime)
		{
			m_state.CurrentState = State.Flee;
		}
	}

	private void AlertFleeExit()
	{
		ResetAnimatorTrigger("AlertFlee");
	}

	private void FleeEnter()
	{
		m_stateTimer = 0f;
		m_fleeOptions.m_speed = 0f;
		m_fleeOptions.m_turnFequency.Randomize();
		m_fleeOptions.m_turnTimer = 0f;
		m_turnTarget = Quaternion.Lerp(Quaternion.LookRotation(-base.transform.right, base.transform.up), Quaternion.LookRotation(base.transform.right, base.transform.up), UnityEngine.Random.Range(0f, 1f));
		SetAnimatorTrigger("Flee");
		m_fleeOptions.m_onEnter.Invoke();
	}

	private void Flee()
	{
		switch (m_fleeOptions.m_fleeType)
		{
		case FleeType.Flee:
			FleeFlee();
			break;
		case FleeType.Bound:
			FleeBound();
			break;
		}
	}

	private void FleeFlee()
	{
		m_stateTimer += Time.fixedDeltaTime;
		Vector3 velocity = m_rigidBody.velocity;
		Vector3 vector = velocity;
		m_target = GetTarget();
		if (m_target == null || !DistanceCheck(m_fleeOptions.m_detectionOptions, false))
		{
			m_target = null;
		}
		if (m_fleeOptions.m_fleeFromInvinciblePlayer && !Singleton<Game>.Instance.Player.Invincible)
		{
			m_target = null;
		}
		if (m_target != null)
		{
			Vector3 position = m_target.transform.position;
			Vector3 vector2 = base.transform.position - position;
			float magnitude = vector2.magnitude;
			vector2.y = 0f;
			if (m_stateTimer > 1f)
			{
				bool avoidComplete = false;
				vector = AvoidBlockUpdate(vector, out avoidComplete);
			}
			if (!m_avoidBlockOptions.m_active)
			{
				m_fleeOptions.m_turnTimer += Time.fixedDeltaTime;
				if (m_fleeOptions.m_turnTimer > m_fleeOptions.m_turnFequency.Value)
				{
					Vector3 normalized = vector2.normalized;
					Vector3 forward = Vector3.Lerp(normalized, -base.transform.right, 0.75f);
					Vector3 forward2 = Vector3.Lerp(normalized, base.transform.right, 0.75f);
					m_turnTarget = Quaternion.Lerp(Quaternion.LookRotation(forward, base.transform.up), Quaternion.LookRotation(forward2, base.transform.up), UnityEngine.Random.Range(0f, 1f));
					m_fleeOptions.m_turnTimer = 0f;
					m_fleeOptions.m_turnFequency.Randomize();
				}
			}
			if (m_stateTimer < 1f)
			{
				m_turnTarget = Quaternion.LookRotation(vector2.normalized, base.transform.up);
			}
			if (magnitude <= m_fleeOptions.m_detectionOptions.m_startRange && Quaternion.Angle(m_turnTarget, Quaternion.LookRotation(-vector2.normalized, base.transform.up)) < 30f)
			{
				m_turnTarget = Quaternion.LookRotation(vector2.normalized, base.transform.up);
			}
			Vector3 forward3 = base.transform.forward;
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, m_turnTarget, m_fleeOptions.m_turnSpeed * Time.fixedDeltaTime * 60f);
			forward3.y = 0f;
			m_fleeOptions.m_speed += m_fleeOptions.m_accel;
			vector = forward3.normalized * m_fleeOptions.m_speed;
			float num = m_fleeOptions.m_maxSpeed;
			foreach (string maxSpeedPerkId in m_fleeOptions.m_maxSpeedPerkIds)
			{
				num = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue(maxSpeedPerkId, num);
			}
			if (vector.magnitude > num)
			{
				vector = vector.normalized * num;
			}
		}
		else
		{
			if (velocity.magnitude < 0.1f)
			{
				if (m_chaseOptions.m_chaseType != ChaseType.None && DistanceCheck(m_chaseOptions.m_detectionOptions, true))
				{
					m_target = GetTarget();
					m_state.CurrentState = State.Chase;
				}
				else
				{
					m_state.CurrentState = State.Idle;
				}
				return;
			}
			vector *= 0.95f;
		}
		if (EdgeCheck(m_edgeOptions.m_stopDistance))
		{
			m_fleeOptions.m_speed = 0f;
			vector = Vector3.zero;
		}
		vector.y = velocity.y;
		m_rigidBody.velocity = vector;
	}

	private void FleeBound()
	{
		m_stateTimer += Time.fixedDeltaTime;
		Vector3 velocity = m_rigidBody.velocity;
		Vector3 velocity2 = velocity;
		string surfaceType = string.Empty;
		float num = CalculateGroundHeight(base.transform.position, out surfaceType);
		float num2 = base.transform.position.y - num;
		if (num == float.MaxValue)
		{
			num2 = float.MaxValue;
		}
		m_target = GetTarget();
		if (m_target == null || (num2 <= 0.1f && !DistanceCheck(m_fleeOptions.m_detectionOptions, false)))
		{
			m_target = null;
		}
		if (m_fleeOptions.m_fleeFromInvinciblePlayer && !Singleton<Game>.Instance.Player.Invincible)
		{
			m_target = null;
		}
		if (m_target != null)
		{
			Vector3 position = m_target.transform.position;
			Vector3 vector = base.transform.position - position;
			float magnitude = vector.magnitude;
			vector.y = 0f;
			vector.y = 0f;
			if (!m_avoidBlockOptions.m_active)
			{
				m_fleeOptions.m_turnTimer += Time.fixedDeltaTime;
				if (m_fleeOptions.m_turnTimer > m_fleeOptions.m_turnFequency.Value)
				{
					Vector3 normalized = vector.normalized;
					Vector3 forward = Vector3.Lerp(normalized, -base.transform.right, 0.75f);
					Vector3 forward2 = Vector3.Lerp(normalized, base.transform.right, 0.75f);
					m_turnTarget = Quaternion.Lerp(Quaternion.LookRotation(forward, base.transform.up), Quaternion.LookRotation(forward2, base.transform.up), UnityEngine.Random.Range(0f, 1f));
					m_fleeOptions.m_turnTimer = 0f;
					m_fleeOptions.m_turnFequency.Randomize();
				}
			}
			if (m_stateTimer < 1f)
			{
				m_turnTarget = Quaternion.LookRotation(vector.normalized, base.transform.up);
			}
			if (magnitude <= m_fleeOptions.m_detectionOptions.m_startRange && Quaternion.Angle(m_turnTarget, Quaternion.LookRotation(-vector.normalized, base.transform.up)) < 30f)
			{
				m_turnTarget = Quaternion.LookRotation(vector.normalized, base.transform.up);
			}
			float num3 = m_fleeOptions.m_chargeTime;
			foreach (string chargePerkId in m_fleeOptions.m_chargePerkIds)
			{
				num3 = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue(chargePerkId, num3, true);
			}
			if (num2 > 0.1f)
			{
				m_stateTimer = 0f;
			}
			else if (m_stateTimer < num3)
			{
				velocity2 = Vector3.zero;
				m_turnTarget = Quaternion.LookRotation(vector.normalized, base.transform.up);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, m_turnTarget, m_fleeOptions.m_turnStoppedSpeed * Time.fixedDeltaTime * 60f);
				m_animator.SetBool("ChargingUp", true);
			}
			else
			{
				m_animator.SetBool("ChargingUp", false);
				if (velocity2.magnitude <= 0.01f)
				{
					Vector3 forward3 = base.transform.forward;
					forward3.y = 0f;
					velocity2 = forward3.normalized * m_fleeOptions.m_maxSpeed;
					velocity.y = m_fleeOptions.m_jumpSpeed;
				}
			}
		}
		else
		{
			if (velocity.magnitude < 0.1f)
			{
				if (m_chaseOptions.m_chaseType != ChaseType.None && DistanceCheck(m_chaseOptions.m_detectionOptions, true))
				{
					m_target = GetTarget();
					m_state.CurrentState = State.Chase;
				}
				else
				{
					m_state.CurrentState = State.Idle;
				}
				return;
			}
			velocity2 *= 0.95f;
		}
		velocity2.y = velocity.y;
		m_rigidBody.velocity = velocity2;
	}

	private void FleeExit()
	{
		if (m_animator != null)
		{
			ResetAnimatorTrigger("Flee");
			m_animator.SetBool("ChargingUp", false);
		}
		m_fleeOptions.m_onExit.Invoke();
	}

	private void StunnedEnter()
	{
		m_stateTimer = 0f;
		SetAnimatorTrigger("Stunned");
		m_avoidBlockOptions.m_onEnter.Invoke();
	}

	private void Stunned()
	{
		Vector3 velocity = m_rigidBody.velocity;
		Vector3 vel = velocity;
		bool avoidComplete = false;
		vel = AvoidBlockUpdate(vel, out avoidComplete);
		if (avoidComplete)
		{
			m_state.CurrentState = State.Idle;
		}
		m_rigidBody.velocity = vel;
	}

	private void StunnedExit()
	{
		if (m_animator != null)
		{
			ResetAnimatorTrigger("Stunned");
		}
		m_avoidBlockOptions.m_onExit.Invoke();
	}

	private float GetSurfaceTypeAbove(out string surfaceType)
	{
		float result = float.MaxValue;
		surfaceType = string.Empty;
		surfaceType = "Air";
		int num = 1 << LayerMask.NameToLayer("Ground");
		num |= 1 << LayerMask.NameToLayer("Water");
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(base.transform.position + new Vector3(0f, 50f, 0f), -base.transform.up.normalized), out hitInfo, 100f, num))
		{
			surfaceType = hitInfo.collider.tag;
			result = hitInfo.point.y;
		}
		return result;
	}

	private void DeathCheck()
	{
		if (!(base.transform.position.y <= 0f))
		{
			return;
		}
		string surfaceType = string.Empty;
		GetSurfaceTypeAbove(out surfaceType);
		if (base.transform.position.y < m_destructiveOptions.m_fallDeathHeight || surfaceType.StartsWith("Water", StringComparison.Ordinal))
		{
			if (surfaceType.StartsWith("Water", StringComparison.Ordinal))
			{
				Kill(null, 1f);
			}
			else
			{
				Kill(null);
			}
			Singleton<Game>.Instance.StatsManager.GetStat(m_id + "KilledBy" + surfaceType).IncreaseStat(1f);
		}
	}

	public void Kill(GameObject killer, float delay = 0f, bool countsAsStatistic = true)
	{
		if (m_dead || !m_destructiveOptions.m_canDie)
		{
			return;
		}
		string surfaceType = string.Empty;
		float surfaceTypeAbove = GetSurfaceTypeAbove(out surfaceType);
		ParticleSystem particleSystem = null;
		if (m_deathParticle.ContainsKey(surfaceType))
		{
			particleSystem = m_deathParticle[surfaceType];
		}
		if (m_selfDestroy && m_selfDestroyParticles != null)
		{
			particleSystem = m_selfDestroyParticles;
		}
		if (particleSystem != null)
		{
			Vector3 position = base.transform.position;
			if (surfaceTypeAbove != float.MaxValue)
			{
				position.y = surfaceTypeAbove;
			}
			if (FastPoolManager.HasPool(particleSystem.gameObject))
			{
				GameObject gameObject = FastPoolManager.GetPool(particleSystem.gameObject).FastInstantiate(position + particleSystem.gameObject.transform.localPosition, particleSystem.transform.rotation);
				ParticleSystemPoolDestroyer component = gameObject.GetComponent<ParticleSystemPoolDestroyer>();
				if (component != null)
				{
					component.m_prefab = particleSystem.gameObject;
				}
			}
			else
			{
				UnityEngine.Object.Instantiate(particleSystem.gameObject, position + particleSystem.gameObject.transform.localPosition, particleSystem.transform.rotation);
			}
		}
		UnityEngine.Object.Destroy(base.gameObject, delay);
		m_dead = true;
		m_destructiveOptions.m_onDeath.Invoke();
		if (countsAsStatistic)
		{
			if (m_enemyForMissions)
			{
				Singleton<Game>.Instance.StatsManager.GetStat("EnemyKilled").IncreaseStat(1f);
			}
			Singleton<Game>.Instance.StatsManager.GetStat(m_id + "Killed").IncreaseStat(1f);
		}
		if (!(killer != null))
		{
			return;
		}
		if (countsAsStatistic)
		{
			Singleton<Game>.Instance.StatsManager.GetStat(m_id + "KilledBy" + killer.name).IncreaseStat(1f);
		}
		int num = m_destructiveOptions.m_spawnCount.RandomInt;
		if (m_destructiveOptions.m_spawnObject == null && killer == Singleton<Game>.Instance.Player.gameObject)
		{
			num = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Collect Kill", num);
		}
		GameObject gameObject2 = null;
		gameObject2 = ((!(m_destructiveOptions.m_spawnObject == null)) ? Singleton<Game>.Instance.Player.Character.GetReplacement(m_destructiveOptions.m_spawnObject) : Singleton<Game>.Instance.Player.CharacterInfo.ReplacementPickup);
		m_destructiveOptions.m_onWasKilledByEntity.Invoke();
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject3 = Utils.CreateFromPrefab(gameObject2, gameObject2.name);
			if (gameObject3 != null)
			{
				gameObject3.transform.parent = Singleton<Game>.Instance.LevelManager.CurrentLevel.transform;
				gameObject3.transform.position += base.transform.position;
				Pickup component2 = gameObject3.GetComponent<Pickup>();
				if (component2 != null)
				{
					component2.OnSpawned(null, num);
				}
			}
		}
	}

	private GameObject GetTarget()
	{
		if (m_preferredTarget != null)
		{
			return m_preferredTarget;
		}
		return Singleton<Game>.Instance.Player.gameObject;
	}

	private bool DistanceCheck(DetectionOptions options, bool start)
	{
		if (!Singleton<Game>.Instance.LevelManager.CurrentLevel.IsComplete)
		{
			return false;
		}
		if (Singleton<Game>.Instance.Player.IsCinematicMove)
		{
			return false;
		}
		float currentValue = options.m_startRange;
		if (!start)
		{
			currentValue = options.m_endRange;
		}
		currentValue = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue(options.m_distancePerkID, currentValue);
		GameObject target = GetTarget();
		if (target == null)
		{
			return false;
		}
		if (options.m_detectionType == DetectionType.None)
		{
			return false;
		}
		if (options.m_detectionType == DetectionType.Infinite)
		{
			return true;
		}
		if (options.m_detectionType == DetectionType.Distance)
		{
			Vector3 position = target.transform.position;
			if ((position - base.transform.position).magnitude <= currentValue)
			{
				return true;
			}
		}
		else if (options.m_detectionType == DetectionType.FOV)
		{
			Vector3 vector = target.transform.position - base.transform.position;
			Vector3 forward = base.transform.forward;
			float num = Vector3.Angle(vector, forward);
			if (num < options.m_fovAngle && vector.magnitude < options.m_fovRange)
			{
				return true;
			}
			if (vector.magnitude < currentValue)
			{
				return true;
			}
		}
		return false;
	}

	private bool EdgeCheck(float distance)
	{
		if (!m_edgeOptions.m_stopBeforeEdges || m_avoidBlockDisabled)
		{
			return false;
		}
		Vector3 position = base.transform.position;
		position += base.transform.forward * distance;
		Vector3 pos = position - base.transform.right * m_edgeOptions.m_detectWidth * 0.5f;
		Vector3 pos2 = position + base.transform.right * m_edgeOptions.m_detectWidth * 0.5f;
		string surfaceType = string.Empty;
		float a = CalculateGroundHeight(pos, out surfaceType);
		string surfaceType2 = string.Empty;
		float b = CalculateGroundHeight(pos2, out surfaceType2);
		float num = 0f;
		num = Mathf.Min(a, b);
		float num2 = base.transform.position.y - num;
		if (num == float.MaxValue || num2 > 3f)
		{
			return true;
		}
		return false;
	}

	private Vector3 AvoidBlockActivate(Vector3 vel)
	{
		if (m_avoidBlockOptions.m_active)
		{
			return vel;
		}
		m_avoidBlockOptions.m_active = true;
		m_avoidBlockOptions.m_delayTimer = 0f;
		if (m_avoidBlockOptions.m_avoidType == AvoidType.Rotate)
		{
			Vector3 vector = base.transform.right;
			if (UnityEngine.Random.Range(0, 10) > 5)
			{
				vector = -vector;
			}
			m_turnTarget = Quaternion.Lerp(Quaternion.LookRotation(-base.transform.forward, base.transform.up), Quaternion.LookRotation(vector, base.transform.up), UnityEngine.Random.Range(0f, 1f));
		}
		else if (m_avoidBlockOptions.m_avoidType == AvoidType.Teleport)
		{
			vel = Vector3.zero;
		}
		else if (m_avoidBlockOptions.m_avoidType == AvoidType.MoveVertically)
		{
			vel = Vector3.zero;
			m_avoidBlockOptions.m_target = base.transform.position;
			m_avoidBlockOptions.m_target.y = 7.5f;
		}
		return vel;
	}

	private Vector3 AvoidBlockUpdate(Vector3 vel, out bool avoidComplete)
	{
		avoidComplete = false;
		if (m_avoidBlockOptions.m_avoidType == AvoidType.None || m_avoidBlockDisabled)
		{
			if (m_avoidBlockDisabled)
			{
				avoidComplete = true;
			}
			return vel;
		}
		if (!m_avoidBlockOptions.m_active)
		{
			if (!(m_rigidBody.velocity.magnitude <= 0.005f))
			{
				return vel;
			}
			vel = AvoidBlockActivate(vel);
		}
		m_avoidBlockOptions.m_delayTimer += Time.fixedDeltaTime;
		if (m_avoidBlockOptions.m_delayTimer < m_avoidBlockOptions.m_delayTime)
		{
			return Vector3.zero;
		}
		if (m_avoidBlockOptions.m_avoidType == AvoidType.Delay)
		{
			m_avoidBlockOptions.m_active = false;
			avoidComplete = true;
			return vel;
		}
		if (m_avoidBlockOptions.m_avoidType == AvoidType.Rotate)
		{
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, m_turnTarget, m_avoidBlockOptions.m_speed * Time.fixedDeltaTime * 60f);
			if (Quaternion.Angle(base.transform.rotation, m_turnTarget) <= 0.1f)
			{
				avoidComplete = true;
				m_avoidBlockOptions.m_active = false;
			}
		}
		else if (m_avoidBlockOptions.m_avoidType != AvoidType.Teleport && m_avoidBlockOptions.m_avoidType == AvoidType.MoveVertically)
		{
			m_avoidBlockOptions.m_target.x = base.transform.position.x;
			m_avoidBlockOptions.m_target.z = base.transform.position.z;
			m_rigidBody.MovePosition(m_rigidBody.transform.position + new Vector3(0f, m_avoidBlockOptions.m_speed, 0f));
			if (base.transform.position.y >= m_avoidBlockOptions.m_target.y)
			{
				avoidComplete = true;
				m_avoidBlockOptions.m_active = false;
			}
		}
		return vel;
	}

	private float CalculateGroundHeight(Vector3 pos, out string surfaceType)
	{
		surfaceType = "Air";
		float result = float.MaxValue;
		int num = 1 << LayerMask.NameToLayer("Ground");
		num |= 1 << LayerMask.NameToLayer("Water");
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(pos + new Vector3(0f, 10f, 0f), -base.transform.up.normalized), out hitInfo, 50f, num))
		{
			surfaceType = hitInfo.collider.tag;
			if (hitInfo.collider.tag.StartsWith("Ground", StringComparison.Ordinal))
			{
				result = hitInfo.point.y;
			}
		}
		return result;
	}

	private void FightOtherEntity(Entity other)
	{
		if (!other.IsDead && other.m_destructiveOptions.m_canDie && other.m_destructiveOptions.m_strength != m_destructiveOptions.m_strength)
		{
			if (m_destructiveOptions.m_strength > other.m_destructiveOptions.m_strength)
			{
				m_destructiveOptions.m_onKillEntity.Invoke();
				other.Kill(base.gameObject);
			}
			else
			{
				Kill(other.gameObject);
			}
		}
	}

	public void SetPreferredTarget(GameObject target)
	{
		m_preferredTarget = target;
		if (m_target != null)
		{
			m_target = m_preferredTarget;
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

	private void OnTriggerEnter(Collider other)
	{
		if (!m_avoidBlockOptions.m_active && m_avoidBlockOptions.m_avoidType != AvoidType.None && m_avoidBlockOptions.m_triggerTags.Contains(other.tag))
		{
			Vector3 velocity = m_rigidBody.velocity;
			AvoidBlockActivate(velocity);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!m_avoidBlockOptions.m_active && m_avoidBlockOptions.m_avoidType != AvoidType.None && m_avoidBlockOptions.m_triggerOnStay && !(m_rigidBody.velocity.magnitude <= 0.005f) && m_avoidBlockOptions.m_triggerTags.Contains(other.tag))
		{
			Vector3 velocity = m_rigidBody.velocity;
			AvoidBlockActivate(velocity);
		}
	}

	public void OnTriggerChildEnter(GameObject child, Collider other)
	{
		if (child.tag == "Death" && other.tag == "Entity")
		{
			Entity entity = other.GetComponent<Entity>();
			if (entity == null)
			{
				entity = other.GetComponentInParent<Entity>();
			}
			if (entity != null && m_destructiveOptions.m_canKill)
			{
				FightOtherEntity(entity);
			}
			else
			{
				Debug.Log("Entity Component Not Found");
			}
		}
		else if (child.tag == "Death" && m_destructiveOptions.m_destroyTags.Contains(other.tag))
		{
			UnityEngine.Object.Destroy(other.gameObject);
		}
		else if (child.tag == "Freeze")
		{
			Player component = other.GetComponent<Player>();
			if (component != null)
			{
				component.Freeze(GetComponent<Snowball>().GetVelocity() * 30f);
				Kill(null);
			}
		}
		else if (child.tag == "DestroySelf" && other.gameObject.layer == LayerMask.NameToLayer("Walls") && m_aliveTime > 0.3f)
		{
			m_selfDestroy = true;
			Kill(null);
		}
	}

	public State GetState()
	{
		return m_state.CurrentState;
	}

	public void OnCollisionChildEnter(GameObject child, Collision collision)
	{
	}

	public void Knockback(Player player)
	{
		if (m_state.CurrentState == State.Chase)
		{
			m_state.CurrentState = m_knockbackOptions.m_stateOnExit;
			Vector3 force = player.transform.position - base.gameObject.transform.position;
			force.y = 0f;
			force.Normalize();
			force *= m_knockbackOptions.m_force;
			force.y = m_knockbackOptions.m_verticalForce;
			player.IsBeingKnocked = true;
			player.AddKnockbackForce(force);
			base.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Vector3 position = base.transform.position;
		position += base.transform.forward * m_edgeOptions.m_stopDistance;
		Vector3 vector = position - base.transform.right * m_edgeOptions.m_detectWidth * 0.5f;
		Vector3 vector2 = position + base.transform.right * m_edgeOptions.m_detectWidth * 0.5f;
		Gizmos.DrawRay(vector + new Vector3(0f, 1f, 0f), -base.transform.up.normalized);
		Gizmos.DrawRay(vector2 + new Vector3(0f, 1f, 0f), -base.transform.up.normalized);
	}
}
