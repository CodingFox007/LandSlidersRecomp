using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.UI;

public class RetryTile : MonoBehaviour
{
	private enum State
	{
		Idle = 0,
		BuckleUp = 1,
		ChangeCamera = 2,
		Transition = 3,
		LoadLevel = 4,
		Raise = 5,
		Landing = 6
	}

	[SerializeField]
	private Rigidbody m_tile;

	[SerializeField]
	private GameObject m_transitionCameraPrefab;

	private GameObject m_transitionCamera;

	[SerializeField]
	private float m_fovTarget = 10f;

	[SerializeField]
	private LeanTweenType m_fovTweenIn = LeanTweenType.easeOutBack;

	[SerializeField]
	private LeanTweenType m_fovTweenOut = LeanTweenType.easeOutBack;

	[SerializeField]
	private float m_fovTweenTimeIn = 0.5f;

	[SerializeField]
	private float m_fovTweenTimeOut = 0.5f;

	[SerializeField]
	private LeanTweenType m_offsetTweenIn = LeanTweenType.linear;

	[SerializeField]
	private LeanTweenType m_offsetTweenOut = LeanTweenType.linear;

	[SerializeField]
	private float m_offsetTweenTimeIn = 0.1f;

	[SerializeField]
	private float m_offsetTweenTimeOut = 0.1f;

	private bool m_triggered;

	private StateMachine<State> m_state = new StateMachine<State>();

	private float m_stateTimer;

	private void Start()
	{
		m_state.AddState(State.Idle, IdleEnter, Idle, IdleExit);
		m_state.AddState(State.BuckleUp, BuckleUpEnter, BuckleUp, BuckleUpExit);
		m_state.AddState(State.ChangeCamera, ChangeCameraEnter, ChangeCamera, ChangeCameraExit);
		m_state.AddState(State.Transition, TransitionEnter, Transition, TransitionExit);
		m_state.AddState(State.LoadLevel, LoadLevelEnter, LoadLevel, LoadLevelExit);
		m_state.AddState(State.Raise, RaiseEnter, Raise, RaiseExit);
		m_state.AddState(State.Landing, LandingEnter, Landing, LandingExit);
		m_state.CurrentState = State.Idle;
	}

	private void IdleEnter()
	{
		m_stateTimer = 0f;
	}

	private void Idle()
	{
		if ((Singleton<Game>.Instance.UIController.CurrentState == UIController.State.Retry || Singleton<Game>.Instance.UIController.CurrentState == UIController.State.StartLevel || Singleton<Game>.Instance.UIController.CurrentState == UIController.State.TutorialLevel || Singleton<Game>.Instance.UIController.CurrentState == UIController.State.InGame) && Singleton<Game>.Instance.UIController.CanChangeMenu() && m_triggered && !Singleton<Game>.Instance.Player.IsDead)
		{
			m_state.CurrentState = State.BuckleUp;
		}
	}

	private void IdleExit()
	{
	}

	private void BuckleUpEnter()
	{
		m_stateTimer = 0f;
	}

	private void BuckleUp()
	{
		if (m_stateTimer < 0.25f)
		{
			return;
		}
		if (!m_triggered || Singleton<Game>.Instance.Player.IsDead)
		{
			m_state.CurrentState = State.Idle;
			return;
		}
		Vector3 position = m_tile.transform.position;
		position.y = Singleton<Game>.Instance.Player.transform.position.y;
		Singleton<Game>.Instance.Player.transform.position = Vector3.MoveTowards(Singleton<Game>.Instance.Player.transform.position, position, 1f);
		if ((position - Singleton<Game>.Instance.Player.transform.position).magnitude <= 0.01f)
		{
			m_state.CurrentState = State.ChangeCamera;
			if (Singleton<Game>.Instance.Player.IsRandomCharacter)
			{
				Singleton<Game>.Instance.Player.PickRandomCharacter();
			}
		}
	}

	private void BuckleUpExit()
	{
	}

	private void ChangeCameraEnter()
	{
		m_stateTimer = 0f;
		Singleton<Game>.Instance.Player.FinishLevel();
		Singleton<Game>.Instance.Player.Rigidbody.velocity = Vector3.zero;
		m_tile.GetComponent<FixedJoint>().connectedBody = Singleton<Game>.Instance.Player.Rigidbody;
	}

	private void ChangeCamera()
	{
		m_transitionCamera = Object.Instantiate(m_transitionCameraPrefab);
		m_transitionCamera.name = m_transitionCameraPrefab.name;
		m_transitionCamera.GetComponent<Camera>().fieldOfView = Singleton<Game>.Instance.VisualCamera.Camera.fieldOfView;
		m_transitionCamera.transform.SetParent(Singleton<Game>.Instance.VisualCamera.transform, false);
		m_transitionCamera.transform.localPosition = Vector3.zero;
		m_transitionCamera.transform.localRotation = Quaternion.identity;
		m_transitionCamera.transform.localScale = Vector3.one;
		Singleton<Game>.Instance.VisualCamera.shadowsOnlyOnPlayer = true;
		m_state.CurrentState = State.Transition;
	}

	private void ChangeCameraExit()
	{
	}

	private void TransitionEnter()
	{
		m_stateTimer = 0f;
		Singleton<Game>.Instance.UIController.HUD.SetState(UIHud.State.Results);
		m_tile.transform.parent = Singleton<Game>.Instance.Player.transform;
		Singleton<Game>.Instance.UIController.EffectsTop.StartEffect("SpotlightWipe");
		LTDescr lTDescr = LeanTween.value(m_transitionCamera, m_transitionCamera.GetComponent<Camera>().fieldOfView, m_fovTarget, m_fovTweenTimeIn);
		lTDescr.setEase(m_fovTweenIn);
		lTDescr.setOnUpdate(TweenFovUpdate);
		Vector3 to = m_transitionCamera.transform.InverseTransformVector(Singleton<Game>.Instance.Player.TransitionOffset);
		LTDescr lTDescr2 = LeanTween.moveLocal(m_transitionCamera, to, m_offsetTweenTimeIn);
		lTDescr2.setEase(m_offsetTweenIn);
	}

	private void Transition()
	{
		if (Singleton<Game>.Instance.UIController.EffectsTop.IsComplete && !LeanTween.isTweening(m_transitionCamera))
		{
			m_state.CurrentState = State.LoadLevel;
		}
	}

	private void TransitionExit()
	{
	}

	private void LoadLevelEnter()
	{
		m_stateTimer = 0f;
		Singleton<Game>.Instance.LevelManager.LoadNextLevel();
	}

	private void LoadLevel()
	{
		if (Singleton<Game>.Instance.LevelManager.CurrentLevel.IsComplete)
		{
			MasterAudio.TriggerPlaylistClip("Main");
			m_state.CurrentState = State.Raise;
		}
	}

	private void LoadLevelExit()
	{
	}

	private void RaiseEnter()
	{
		m_stateTimer = 0f;
		SpawnTile spawnTile = Object.FindObjectOfType<SpawnTile>();
		if (spawnTile != null)
		{
			spawnTile.SetVisible(true);
			Singleton<Game>.Instance.Player.RespawnPlayer(spawnTile.transform.position);
		}
		Renderer[] componentsInChildren = m_tile.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = false;
		}
		Graphic[] componentsInChildren2 = m_tile.GetComponentsInChildren<Graphic>();
		Graphic[] array2 = componentsInChildren2;
		foreach (Graphic graphic in array2)
		{
			graphic.enabled = false;
		}
		Singleton<Game>.Instance.UIController.EffectsTop.EndEffect();
		LTDescr lTDescr = LeanTween.value(m_transitionCamera, m_transitionCamera.GetComponent<Camera>().fieldOfView, Singleton<Game>.Instance.VisualCamera.Camera.fieldOfView, m_fovTweenTimeOut);
		lTDescr.setEase(m_fovTweenOut);
		lTDescr.setOnUpdate(TweenFovUpdate);
		LTDescr lTDescr2 = LeanTween.moveLocal(m_transitionCamera, Vector3.zero, m_offsetTweenTimeOut);
		lTDescr2.setEase(m_offsetTweenOut);
	}

	private void Raise()
	{
		if (Singleton<Game>.Instance.UIController.EffectsTop.IsComplete && !LeanTween.isTweening(m_transitionCamera))
		{
			Singleton<Game>.Instance.VisualCamera.shadowsOnlyOnPlayer = false;
			Object.Destroy(m_transitionCamera);
			m_state.CurrentState = State.Landing;
		}
	}

	private void RaiseExit()
	{
	}

	private void LandingEnter()
	{
		m_stateTimer = 0f;
		SpawnTile spawnTile = Object.FindObjectOfType<SpawnTile>();
		Vector3 position = m_tile.transform.position;
		m_tile.GetComponent<FixedJoint>().connectedBody = null;
		Singleton<Game>.Instance.Player.StartLevel();
		Singleton<Game>.Instance.UIController.HUD.SetState(UIHud.State.Normal);
		m_tile.transform.parent = null;
		position.y = spawnTile.transform.position.y;
		m_tile.MovePosition(position);
		Object.Destroy(m_tile.gameObject);
	}

	private void Landing()
	{
	}

	private void LandingExit()
	{
	}

	private void FixedUpdate()
	{
		m_stateTimer += Time.deltaTime;
		m_state.Update();
	}

	private void TweenFovUpdate(float value)
	{
		m_transitionCamera.GetComponent<Camera>().fieldOfView = value;
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
