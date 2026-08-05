using System;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using HeavyDutyInspector;
using UnityEngine;

public class ExitTile : MonoBehaviour
{
	[Serializable]
	public class ExitDrawable
	{
		public string m_name;

		[Range(0f, 1f)]
		public float m_chance = 1f;

		[SerializeField]
		[AssetPath(typeof(ExitTileDrawable), PathOptions.RelativeToResources)]
		public string m_path;

		[HideInInspector]
		public ExitTileDrawable m_assetPrefab;

		[HideInInspector]
		public bool m_isLoading;

		[HideInInspector]
		public ExitTileDrawable m_obj;

		public float m_drawHeight;

		public float m_minHeight;

		public float m_maxHeight;

		public Vector3 m_offset;
	}

	private enum State
	{
		Idle = 0,
		Launch = 1,
		BuckleUp = 2,
		BlastOff = 3,
		Climax = 4,
		Falling = 5,
		Landing = 6,
		Abort = 7
	}

	[SerializeField]
	private Rigidbody m_tile;

	[SerializeField]
	private float m_launchDelay = 0.5f;

	private float m_launchTimer;

	[SerializeField]
	private float m_launchSpeed = 10f;

	[SerializeField]
	private GameObject m_underTileDrawable;

	[SerializeField]
	private float m_blastOffHeight = 15f;

	[SerializeField]
	private float m_blastOffSpeed = 20f;

	[SerializeField]
	private float m_blastOffAccel = 1f;

	[SerializeField]
	private float m_startLevelLoadHeight = 300f;

	[SerializeField]
	private float m_changeLevelHeight = 30f;

	[SerializeField]
	private ParticleSystem m_blastOffParticle;

	[SerializeField]
	private ParticleSystem m_blastOffShockwave;

	[SerializeField]
	private ParticleSystem m_fallingParticle;

	[SerializeField]
	private ParticleSystem m_landingParticle;

	[SerializeField]
	private ExitDrawable[] m_exitDrawables;

	private List<ExitDrawable> m_activeDrawables = new List<ExitDrawable>();

	[SerializeField]
	private string m_wipeEffect = "WhiteWipe";

	[SerializeField]
	private float m_wipeStartHeight;

	[SerializeField]
	private float m_wipeEndHeight;

	[SerializeField]
	private string m_wipeDownEffect = "WhiteWipe";

	[SerializeField]
	private float m_wipeDownStartHeight;

	[SerializeField]
	private float m_wipeDownEndHeight;

	private bool m_wipeInStarted;

	private bool m_wipeOutStarted;

	[SerializeField]
	private Level m_levelPrefab;

	[SerializeField]
	private float m_spaceHangTime = 0.5f;

	private bool m_triggered;

	private bool m_loadStarted;

	private float m_startY;

	private float m_vel;

	private RigidbodyConstraints m_oldCharacterConstraints;

	private List<GameObject> m_passengers = new List<GameObject>();

	private StateMachine<State> m_state = new StateMachine<State>();

	private float m_stateTimer;

	private float m_hangTimer;

	private SoundGroupVariation m_loopingSfx;

	private SpawnTile m_spawnTile;

	private void Start()
	{
		m_startY = m_tile.transform.position.y;
		m_state.AddState(State.Idle, IdleEnter, Idle, IdleExit);
		m_state.AddState(State.Launch, LaunchEnter, Launch, LaunchExit);
		m_state.AddState(State.BuckleUp, BuckleUpEnter, BuckleUp, BuckleUpExit);
		m_state.AddState(State.BlastOff, BlastOffEnter, BlastOff, BlastOffExit);
		m_state.AddState(State.Climax, ClimaxEnter, Climax, ClimaxExit);
		m_state.AddState(State.Falling, FallingEnter, Falling, FallingExit);
		m_state.AddState(State.Landing, LandingEnter, Landing, LandingExit);
		m_state.AddState(State.Abort, AbortEnter, Abort, AbortExit);
		m_state.CurrentState = State.Idle;
		ExitDrawable[] exitDrawables = m_exitDrawables;
		foreach (ExitDrawable exitDrawable in exitDrawables)
		{
			if (exitDrawable.m_chance >= UnityEngine.Random.Range(0f, 1f))
			{
				m_activeDrawables.Add(exitDrawable);
			}
		}
	}

	private void OnDestroy()
	{
	}

	private void IdleEnter()
	{
		m_stateTimer = 0f;
	}

	private void Idle()
	{
		if (Singleton<Game>.Instance.UIController.CurrentState == UIController.State.InGame && m_triggered && !Singleton<Game>.Instance.Player.IsDead)
		{
			m_state.CurrentState = State.Launch;
		}
	}

	private void IdleExit()
	{
	}

	private void LaunchEnter()
	{
		m_stateTimer = 0f;
		if (m_underTileDrawable != null)
		{
			m_underTileDrawable.SetActive(true);
		}
		MasterAudio.PlaySound("EI-ExitRumble-A");
	}

	private void Launch()
	{
		if (Singleton<Game>.Instance.Player.IsDead || !m_triggered || Singleton<Game>.Instance.Player.IsCinematicMove)
		{
			m_state.CurrentState = State.Abort;
		}
		else if (m_stateTimer >= m_launchDelay)
		{
			Singleton<Game>.Instance.VisualCamera.Shake("Launch");
			Vector3 position = m_tile.transform.position;
			m_vel = m_launchSpeed;
			position.y += m_vel * Time.deltaTime;
			m_tile.MovePosition(position);
			if (position.y >= m_startY + m_blastOffHeight)
			{
				m_state.CurrentState = State.BuckleUp;
			}
		}
	}

	private void LaunchExit()
	{
		Singleton<Game>.Instance.VisualCamera.StopShake("Launch");
	}

	private void BuckleUpEnter()
	{
		m_stateTimer = 0f;
		Singleton<Game>.Instance.Player.BlockInput = true;
		Transform transform = Singleton<Game>.Instance.LevelManager.CurrentLevel.GetTransform("Enemies");
		for (int i = 0; i < transform.childCount; i++)
		{
			Entity component = transform.GetChild(i).gameObject.GetComponent<Entity>();
			if (component != null && !(component.transform.position.y < m_tile.transform.position.y - 0.5f) && !((component.transform.position - m_tile.transform.position).magnitude > 10f))
			{
				m_passengers.Add(component.gameObject);
			}
		}
		foreach (GameObject passenger in m_passengers)
		{
			passenger.transform.parent = base.transform;
		}
	}

	private void BuckleUp()
	{
		if (Singleton<Game>.Instance.Player.IsDead || !m_triggered || Singleton<Game>.Instance.Player.IsCinematicMove)
		{
			Singleton<Game>.Instance.Player.BlockInput = false;
			m_state.CurrentState = State.Abort;
			return;
		}
		Vector3 position = m_tile.transform.position;
		m_vel = m_launchSpeed;
		position.y += m_vel * Time.deltaTime;
		m_tile.MovePosition(position);
		Vector3 position2 = m_tile.transform.position;
		position2.y = Singleton<Game>.Instance.Player.transform.position.y;
		Singleton<Game>.Instance.Player.transform.position = Vector3.MoveTowards(Singleton<Game>.Instance.Player.transform.position, position2, 0.3f);
		if ((position2 - Singleton<Game>.Instance.Player.transform.position).magnitude <= 0.1f)
		{
			m_state.CurrentState = State.BlastOff;
		}
	}

	private void BuckleUpExit()
	{
	}

	private void BlastOffEnter()
	{
		m_stateTimer = 0f;
		Singleton<Game>.Instance.Player.FinishLevel();
		m_tile.GetComponent<FixedJoint>().connectedBody = Singleton<Game>.Instance.Player.Rigidbody;
		Singleton<Game>.Instance.UIController.HUD.SetState(UIHud.State.Results);
		m_tile.transform.parent = Singleton<Game>.Instance.Player.transform;
		if (m_blastOffParticle != null)
		{
			m_blastOffParticle.Play();
		}
		if (m_blastOffShockwave != null)
		{
			m_blastOffShockwave.Play();
		}
		Singleton<Game>.Instance.VisualCamera.Shake("Blast Off");
		MasterAudio.PlaySound("EI-ExitTakeOff-A");
	}

	private void BlastOff()
	{
		Vector3 position = m_tile.transform.position;
		float blastOffAccel = m_blastOffAccel;
		position.y += (m_vel + 0.5f * blastOffAccel * Time.deltaTime) * Time.deltaTime;
		m_vel += blastOffAccel;
		m_vel = Mathf.Min(m_vel, m_blastOffSpeed);
		m_tile.MovePosition(position);
		Vector3 position2 = m_tile.transform.position;
		position2.y = Singleton<Game>.Instance.Player.transform.position.y;
		if (position.y > m_startLevelLoadHeight && !m_loadStarted)
		{
			m_loadStarted = true;
			Singleton<Game>.Instance.LevelManager.LoadNextLevel(m_levelPrefab);
		}
		if (position.y >= m_changeLevelHeight)
		{
			m_state.CurrentState = State.Climax;
		}
	}

	private void BlastOffExit()
	{
	}

	private void ClimaxEnter()
	{
		m_stateTimer = 0f;
		m_hangTimer = 0f;
		Singleton<Game>.Instance.UIController.HUD.ShowResults();
	}

	private void Climax()
	{
		float vel = m_vel;
		Vector3 position = m_tile.transform.position;
		float num = Physics.gravity.y * 0.04f;
		position.y += (m_vel + 0.5f * num * Time.deltaTime) * Time.deltaTime;
		m_vel += num;
		m_vel = Mathf.Max(m_vel, 0f);
		m_tile.MovePosition(position);
		if (vel > 0f && m_vel <= 0f)
		{
			PlaySoundResult playSoundResult = MasterAudio.PlaySound("EI-ExitSpaceIdle-A");
			m_loopingSfx = playSoundResult.ActingVariation;
			foreach (GameObject passenger in m_passengers)
			{
				if (passenger != null)
				{
					passenger.GetComponent<Entity>().LaunchIntoSpace();
					passenger.transform.parent = null;
				}
			}
		}
		if (m_vel <= 0f)
		{
			m_hangTimer += Time.deltaTime;
		}
		if (Singleton<Game>.Instance.LevelManager.CurrentLevel.IsComplete && m_vel <= 0f)
		{
			SpawnTile spawnTile = UnityEngine.Object.FindObjectOfType<SpawnTile>();
			if (spawnTile == null)
			{
				Debug.LogError("Level Missing SpawnTile");
				Singleton<Game>.Instance.LevelManager.LoadNextLevel(m_levelPrefab);
			}
			else if (m_hangTimer >= m_spaceHangTime)
			{
				m_state.CurrentState = State.Falling;
			}
		}
	}

	private void ClimaxExit()
	{
		Singleton<Game>.Instance.UIController.HUD.HideResults();
		m_loopingSfx.Stop();
	}

	private void FallingEnter()
	{
		m_stateTimer = 0f;
		Singleton<Game>.Instance.VisualCamera.Shake("Reentery");
		m_wipeInStarted = false;
		m_wipeOutStarted = false;
		if (m_fallingParticle != null)
		{
			m_fallingParticle.Play();
		}
		MasterAudio.PlaySound("EI-ExitFalling-A");
		m_spawnTile = UnityEngine.Object.FindObjectOfType<SpawnTile>();
	}

	private void Falling()
	{
		if (Singleton<Game>.Instance.Player.Character.GetComponent<Rigidbody>() != null)
		{
			Singleton<Game>.Instance.Player.Character.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			Singleton<Game>.Instance.Player.Character.transform.localRotation = Quaternion.identity;
		}
		Vector3 position = m_tile.transform.position;
		float num = Physics.gravity.y * 0.04f;
		position.y += (m_vel + 0.5f * num * Time.deltaTime) * Time.deltaTime;
		position.y = Mathf.Max(position.y, m_spawnTile.transform.position.y);
		m_vel += num;
		m_vel = Mathf.Max(m_vel, -500f);
		m_tile.MovePosition(position);
		if (position.y <= m_spawnTile.transform.position.y)
		{
			m_state.CurrentState = State.Landing;
		}
	}

	private void FallingExit()
	{
		foreach (GameObject passenger in m_passengers)
		{
			if (passenger != null)
			{
				UnityEngine.Object.Destroy(passenger);
			}
		}
	}

	private void LandingEnter()
	{
		m_stateTimer = 0f;
		SpawnTile spawnTile = m_spawnTile;
		Vector3 position = m_tile.transform.position;
		m_tile.GetComponent<FixedJoint>().connectedBody = null;
		Singleton<Game>.Instance.Player.StartLevel();
		Singleton<Game>.Instance.UIController.HUD.SetState(UIHud.State.Normal);
		m_tile.transform.parent = null;
		position.y = spawnTile.transform.position.y;
		m_tile.MovePosition(position);
		m_landingParticle.transform.parent = spawnTile.transform;
		UnityEngine.Object.Destroy(m_tile.gameObject);
		Singleton<Game>.Instance.VisualCamera.Shake("Landing Impact");
		m_blastOffParticle.Stop();
		if (spawnTile != null)
		{
			if (m_landingParticle != null)
			{
				m_landingParticle.Play();
			}
			spawnTile.SetVisible(true);
		}
		MasterAudio.PlaySound("EI-ExitImpact-A");
	}

	private void Landing()
	{
	}

	private void LandingExit()
	{
	}

	private void AbortEnter()
	{
		m_stateTimer = 0f;
	}

	private void Abort()
	{
		Vector3 position = m_tile.transform.position;
		float num = Physics.gravity.y * 0.01f;
		position.y += (m_vel + 0.5f * num * Time.deltaTime) * Time.deltaTime;
		position.y = Mathf.Max(position.y, m_startY);
		m_vel += num;
		m_tile.MovePosition(position);
		if (position.y == m_startY)
		{
			m_state.CurrentState = State.Idle;
		}
	}

	private void AbortExit()
	{
	}

	private void FixedUpdate()
	{
		m_stateTimer += Time.deltaTime;
		m_state.Update();
		if (m_state.CurrentState != State.Idle)
		{
			foreach (ExitDrawable activeDrawable in m_activeDrawables)
			{
				if (m_tile.transform.position.y > activeDrawable.m_drawHeight)
				{
					if (activeDrawable.m_assetPrefab == null && !activeDrawable.m_isLoading)
					{
						StartCoroutine(LoadDrawableResources(activeDrawable));
						activeDrawable.m_isLoading = true;
					}
					Vector3 position = m_tile.transform.position;
					if (activeDrawable.m_obj == null && activeDrawable.m_assetPrefab != null)
					{
						Quaternion rotation = activeDrawable.m_assetPrefab.transform.rotation;
						activeDrawable.m_obj = UnityEngine.Object.Instantiate(activeDrawable.m_assetPrefab, position, rotation);
						activeDrawable.m_obj.transform.parent = base.transform;
					}
					if (activeDrawable.m_obj != null)
					{
						float num = activeDrawable.m_maxHeight - activeDrawable.m_minHeight;
						float num2 = (position.y - activeDrawable.m_drawHeight) / (m_changeLevelHeight - activeDrawable.m_drawHeight);
						position.y -= num * num2;
						position += activeDrawable.m_offset;
						activeDrawable.m_obj.transform.position = position;
					}
				}
				else
				{
					if (activeDrawable.m_obj != null)
					{
						UnityEngine.Object.Destroy(activeDrawable.m_obj.gameObject);
					}
					if (activeDrawable.m_assetPrefab != null)
					{
						UnloadDrawableResources(activeDrawable);
					}
				}
			}
		}
		if (m_state.CurrentState == State.Falling)
		{
			if (m_tile.transform.position.y < m_wipeDownStartHeight && !m_wipeInStarted)
			{
				m_wipeInStarted = true;
				Singleton<Game>.Instance.UIController.EffectsBack.StartEffect(m_wipeDownEffect);
				MasterAudio.FireCustomEvent("ExitSpace", base.transform.position);
			}
			if (m_wipeInStarted && !m_wipeOutStarted && m_tile.transform.position.y < m_wipeDownEndHeight && Singleton<Game>.Instance.UIController.EffectsBack.IsComplete)
			{
				m_wipeOutStarted = true;
				Singleton<Game>.Instance.UIController.EffectsBack.EndEffect();
			}
		}
		else
		{
			if (m_tile.transform.position.y > m_wipeStartHeight && !m_wipeInStarted)
			{
				m_wipeInStarted = true;
				Singleton<Game>.Instance.UIController.EffectsBack.StartEffect(m_wipeEffect);
				MasterAudio.FireCustomEvent("StartSpace", base.transform.position);
			}
			if (m_wipeInStarted && !m_wipeOutStarted && m_tile.transform.position.y > m_wipeEndHeight && Singleton<Game>.Instance.UIController.EffectsBack.IsComplete)
			{
				m_wipeOutStarted = true;
				Singleton<Game>.Instance.UIController.EffectsBack.EndEffect();
			}
		}
	}

	private IEnumerator LoadDrawableResources(ExitDrawable drawable)
	{
		ResourceRequest request = Resources.LoadAsync<ExitTileDrawable>(drawable.m_path);
		while (!request.isDone)
		{
			yield return null;
		}
		drawable.m_assetPrefab = request.asset as ExitTileDrawable;
		drawable.m_isLoading = false;
	}

	private void UnloadDrawableResources(ExitDrawable drawable)
	{
		if (drawable.m_assetPrefab != null)
		{
			drawable.m_assetPrefab = null;
			drawable.m_isLoading = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			m_triggered = true;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player")
		{
			m_triggered = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			m_triggered = false;
		}
	}
}
