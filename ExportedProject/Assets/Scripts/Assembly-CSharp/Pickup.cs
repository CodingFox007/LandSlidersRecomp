using System;
using HeavyDutyInspector;
using I2.Loc;
using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{
	[SerializeField]
	private string m_name;

	[SerializeField]
	private string m_namePlural;

	[SerializeField]
	private Sprite m_icon;

	[SerializeField]
	private string m_currencyId;

	[SerializeField]
	private int m_currencyValue;

	[SerializeField]
	private bool m_destoryOnPickup = true;

	[SerializeField]
	private bool m_canMagnetize = true;

	[SerializeField]
	private float m_magnetSpeed = 0.5f;

	private float m_magnetCurrentSpeed;

	[SerializeField]
	private float m_magnetAccel = 0.01f;

	[SerializeField]
	private Effect m_pickupEffect;

	[SerializeField]
	private bool m_randomiseStart = true;

	[Comment("Optinal Pickup Animation", CommentType.None, 0)]
	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private string m_animationTrigger = string.Empty;

	[SerializeField]
	private string m_animationMagnetBool = string.Empty;

	[SerializeField]
	private Renderer m_shadow;

	[SerializeField]
	private float m_heightOffGround = 2f;

	[SerializeField]
	public UnityEvent m_onLoad = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onPickup = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onDestory = new UnityEvent();

	[SerializeField]
	private bool m_neverCollect;

	private bool m_collectible = true;

	private bool m_collected;

	private bool m_magnetEnabled;

	public ParticleSystem m_detachableParticles;

	private Vector3 m_velocity;

	private float m_spawnTimer = 1f;

	private float m_spawnSpeed = 1f;

	private float m_spawnJumpHeight;

	private Vector3 m_spawnedFromPosition = Vector3.zero;

	private Vector3 m_spawnToPosition = Vector3.zero;

	private bool m_spawning;

	private static float s_spawnAngle;

	public Sprite Icon
	{
		get
		{
			return m_icon;
		}
	}

	public string Name
	{
		get
		{
			return ScriptLocalization.Get(m_name, true);
		}
	}

	public string NamePlural
	{
		get
		{
			return ScriptLocalization.Get(m_namePlural, true);
		}
	}

	public string CurrencyId
	{
		get
		{
			return m_currencyId;
		}
	}

	public int CurrentValue
	{
		get
		{
			return m_currencyValue;
		}
	}

	public bool ShadowEnabled
	{
		get
		{
			if (m_shadow == null)
			{
				return false;
			}
			return m_shadow.enabled;
		}
		set
		{
			if (!(m_shadow == null))
			{
				m_shadow.enabled = value;
			}
		}
	}

	public Effect PickupEffect
	{
		get
		{
			return m_pickupEffect;
		}
	}

	public bool IsCollected
	{
		get
		{
			return m_collected;
		}
	}

	private bool IsWatingForPlayerToMove
	{
		get
		{
			return base.transform.position.y < 100f && !Singleton<Game>.Instance.VisualCamera.Target.m_hasBeenDragged;
		}
	}

	private void Awake()
	{
		m_collectible = true;
		m_collected = false;
		m_magnetEnabled = false;
		m_spawning = false;
		m_animator = base.gameObject.GetComponent<Animator>();
		if (m_animator != null && m_randomiseStart)
		{
			m_animator.Play(m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, UnityEngine.Random.Range(0f, 1f));
		}
		m_spawnToPosition = (m_spawnedFromPosition = base.transform.position);
	}

	public void RandomizeAnimator()
	{
		m_animator = base.gameObject.GetComponent<Animator>();
		if (m_animator != null && m_randomiseStart)
		{
			m_animator.Play(m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, UnityEngine.Random.Range(0f, 1f));
		}
	}

	private void Start()
	{
		if (base.transform.position.y > 30f)
		{
			SphereCollider component = base.gameObject.GetComponent<SphereCollider>();
			if (component != null)
			{
				component.radius = 5f;
			}
			Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
			if (rigidbody != null)
			{
				rigidbody.useGravity = false;
				rigidbody.isKinematic = true;
				rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
				rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			}
			ShadowEnabled = false;
		}
		if (!Singleton<Game>.Instance.LevelManager.IsNormalLevel)
		{
			return;
		}
		m_onLoad.Invoke();
		if (Singleton<Game>.Instance.Player.CharacterInfo == null || !(m_currencyId == "collectible") || !Singleton<Game>.Instance.Player.CharacterInfo.IsGolden)
		{
			return;
		}
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (!(renderer.GetComponent<ParticleSystem>() != null) && !(renderer == m_shadow) && renderer.enabled)
			{
				renderer.sharedMaterial = Singleton<Game>.Instance.Player.Character.GoldMaterial;
			}
		}
	}

	private void OnDestroy()
	{
		m_onDestory.Invoke();
	}

	public void OnSpawned(SpawnPickups.SpawnOptions spawnOptions, int numSpawnedWith)
	{
		if (spawnOptions == null || spawnOptions.m_useDefault)
		{
			spawnOptions = Singleton<Game>.Instance.DefaultSpawnPickupsOptions;
		}
		m_collectible = false;
		m_spawning = true;
		m_spawnedFromPosition = base.transform.position;
		m_spawnTimer = 0f;
		m_spawnSpeed = spawnOptions.m_speed.RandomValue;
		bool flag = false;
		m_spawnJumpHeight = spawnOptions.m_height.RandomValue;
		int num = 0;
		float num2 = 360f;
		if (numSpawnedWith > 1)
		{
			num2 = 360f / (float)numSpawnedWith;
		}
		while (!flag)
		{
			num++;
			s_spawnAngle += num2 * UnityEngine.Random.Range(0.3f, 1.7f);
			float num3 = s_spawnAngle;
			float num4 = spawnOptions.m_distance.RandomValue;
			if (num > 10)
			{
				num4 *= Mathf.Pow(0.9f, num - 8);
			}
			Vector3 vector = new Vector3(Mathf.Sin((float)Math.PI / 180f * num3), 0f, Mathf.Cos((float)Math.PI / 180f * num3));
			m_spawnToPosition = m_spawnedFromPosition + vector * num4;
			string surfaceType = string.Empty;
			float num5 = CalculateGroundHeight(m_spawnToPosition, out surfaceType);
			if (num5 == float.MaxValue && num > 20)
			{
				num5 = m_spawnedFromPosition.y - m_heightOffGround;
			}
			m_spawnToPosition.y = num5 + m_heightOffGround;
			if (Mathf.Abs(m_spawnToPosition.y - m_spawnedFromPosition.y) < 1f || num > 20)
			{
				flag = true;
			}
		}
		if (base.gameObject.GetComponent<Collider>() != null)
		{
			base.gameObject.GetComponent<Collider>().enabled = false;
		}
	}

	public void EnableMagnet(bool value)
	{
		if (value && m_canMagnetize && !Singleton<Game>.Instance.Player.IsDead && !IsWatingForPlayerToMove)
		{
			if (!m_magnetEnabled)
			{
				Vector3 position = Singleton<Game>.Instance.Player.transform.position;
				if (Mathf.Abs(base.transform.position.y - position.y) < 5f)
				{
					m_magnetCurrentSpeed = 0f;
					m_magnetEnabled = true;
				}
			}
		}
		else
		{
			m_magnetEnabled = false;
		}
		if (m_animator != null && m_animationMagnetBool != string.Empty)
		{
			m_animator.SetBool(m_animationMagnetBool, m_magnetEnabled);
		}
	}

	public void DestoryPickup()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (m_collected)
		{
			return;
		}
		if (m_spawning)
		{
			m_spawnTimer = Mathf.Clamp01(m_spawnTimer + m_spawnSpeed * Time.deltaTime);
			Vector3 vector = Vector3.Lerp(m_spawnedFromPosition, m_spawnToPosition, m_spawnTimer);
			float num = (m_spawnTimer - 0.5f) * 2f;
			num *= num;
			vector.y += (1f - num) * m_spawnJumpHeight;
			base.transform.position = vector;
			if (base.gameObject.GetComponent<Collider>() != null)
			{
				base.gameObject.GetComponent<Collider>().enabled = true;
			}
			string surfaceType = string.Empty;
			float num2 = CalculateGroundHeight(base.transform.position, out surfaceType);
			if (m_spawnTimer == 1f)
			{
				m_spawning = false;
				m_collectible = true;
			}
			if (m_shadow != null)
			{
				if (num2 == float.MaxValue)
				{
					m_shadow.enabled = false;
				}
				else
				{
					m_shadow.enabled = true;
				}
				Vector3 position = vector;
				position.y = num2 + 0.1f;
				m_shadow.transform.position = position;
			}
		}
		else if (m_magnetEnabled)
		{
			m_magnetCurrentSpeed += m_magnetAccel * Time.deltaTime * 60f;
			m_magnetCurrentSpeed = Mathf.Min(m_magnetCurrentSpeed, m_magnetSpeed);
			if (Singleton<Game>.Instance.Player.IsDead)
			{
				EnableMagnet(false);
				return;
			}
			Vector3 target = Singleton<Game>.Instance.Player.transform.position + Vector3.up * m_heightOffGround;
			Vector3 position2 = base.transform.position;
			Vector3 position3 = Vector3.MoveTowards(position2, target, m_magnetCurrentSpeed * Time.deltaTime * 60f);
			base.transform.position = position3;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		CollisionCheck(other);
	}

	private void OnTriggerStay(Collider other)
	{
		CollisionCheck(other);
	}

	private void CollisionCheck(Collider other)
	{
		if (!m_collectible || m_collected || IsWatingForPlayerToMove || m_neverCollect)
		{
			return;
		}
		if (other.tag != "Player")
		{
			EnableMagnet(true);
		}
		else if (Singleton<Game>.Instance.LevelManager.IsLoaded || !(base.transform.position.y < 30f))
		{
			m_collected = true;
			if (m_pickupEffect.m_particlePrefab != null)
			{
				Singleton<Game>.Instance.EffectManager.StartEffect(m_pickupEffect, base.gameObject.transform.position, Color.white);
			}
			if (m_animator != null && m_animationTrigger != string.Empty)
			{
				m_animator.SetTrigger(m_animationTrigger);
			}
			else if (m_destoryOnPickup)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			CollectReward();
			if (m_detachableParticles != null)
			{
				m_detachableParticles.transform.SetParent(base.transform.parent, true);
				m_detachableParticles.Stop();
				GameObjectController gameObjectController = m_detachableParticles.gameObject.AddComponent<GameObjectController>();
				gameObjectController.SetObj(m_detachableParticles.gameObject);
				gameObjectController.Destory(m_detachableParticles.startLifetime);
			}
			m_onPickup.Invoke();
		}
	}

	protected virtual void CollectReward()
	{
		if (m_currencyId != string.Empty)
		{
			Singleton<Game>.Instance.InventoryManager.AddCurrency(m_currencyId, m_currencyValue);
			if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.InGame)
			{
				Singleton<Game>.Instance.StatsManager.GetStat(m_currencyId + "EarnedInGame").IncreaseStat(m_currencyValue);
			}
		}
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
}
