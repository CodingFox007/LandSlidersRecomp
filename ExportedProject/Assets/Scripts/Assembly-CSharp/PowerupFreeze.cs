using UnityEngine;

public class PowerupFreeze : Powerup
{
	[SerializeField]
	private float m_time;

	private float m_timer;

	[SerializeField]
	private Material m_playerMaterial;

	[SerializeField]
	private float m_fovZoom = 20f;

	[SerializeField]
	private float m_timeScale = 0.1f;

	[SerializeField]
	private float m_activateWaitTime = 0.5f;

	private float m_activateTimer;

	[SerializeField]
	private float m_hasFreezePowerGracePeriod = 0.7f;

	[SerializeField]
	private float m_zoomOutTime = 0.3f;

	private float m_zoomOutTimer;

	[SerializeField]
	private float m_enemyDrag = 2f;

	[SerializeField]
	private float m_timeScaleLerpOut = 0.3f;

	private float m_timeScaleLerpOutTimer;

	private bool m_activationSequenceComplete;

	public override void Activate()
	{
		base.Activate();
		Singleton<Game>.Instance.Player.HasFeezePower = true;
		Singleton<Game>.Instance.Player.Character.SetMaterials(m_playerMaterial);
		m_timer = 0f;
		Singleton<Game>.Instance.VisualCamera.Camera.fieldOfView = m_fovZoom;
		Singleton<Game>.Instance.VisualCamera.CanZoom = false;
		Time.timeScale = m_timeScale;
		m_activateTimer = 0f;
		m_zoomOutTimer = 0f;
		m_timeScaleLerpOutTimer = 0f;
		m_activationSequenceComplete = false;
		Singleton<Game>.Instance.UIController.HUD.ShowPowerup();
		Singleton<Game>.Instance.UIController.HUD.SetPowerupProgress(1f);
		Singleton<Game>.Instance.VisualCamera.Shake("Invincible");
		Transform transform = Singleton<Game>.Instance.LevelManager.CurrentLevel.GetTransform("Enemies");
		for (int i = 0; i < transform.childCount; i++)
		{
			Animator component = transform.GetChild(i).GetComponent<Animator>();
			if (component != null)
			{
				component.speed = 0f;
			}
			Rigidbody component2 = transform.GetChild(i).GetComponent<Rigidbody>();
			if ((bool)component2)
			{
				component2.useGravity = false;
				component2.drag = m_enemyDrag;
			}
		}
		Transform transform2 = Singleton<Game>.Instance.LevelManager.CurrentLevel.GetTransform("Rabbits");
		for (int j = 0; j < transform2.childCount; j++)
		{
			Animator component3 = transform2.GetChild(j).GetComponent<Animator>();
			if (component3 != null)
			{
				component3.speed = 0f;
			}
			Rigidbody component4 = transform2.GetChild(j).GetComponent<Rigidbody>();
			if ((bool)component4)
			{
				component4.useGravity = false;
				component4.drag = m_enemyDrag;
			}
		}
	}

	public override void Deactivate()
	{
		base.Deactivate();
		Singleton<Game>.Instance.Player.HasFreezePowerGracePeriod = m_hasFreezePowerGracePeriod;
		Singleton<Game>.Instance.Player.HasFeezePower = false;
		Singleton<Game>.Instance.Player.Character.UnsetMaterials();
		Singleton<Game>.Instance.UIController.HUD.HidePowerup();
		Transform transform = Singleton<Game>.Instance.LevelManager.CurrentLevel.GetTransform("Enemies");
		for (int i = 0; i < transform.childCount; i++)
		{
			Animator component = transform.GetChild(i).GetComponent<Animator>();
			if (component != null)
			{
				component.speed = 1f;
			}
			Rigidbody component2 = transform.GetChild(i).GetComponent<Rigidbody>();
			if ((bool)component2)
			{
				component2.useGravity = true;
				component2.drag = 0f;
			}
		}
		Transform transform2 = Singleton<Game>.Instance.LevelManager.CurrentLevel.GetTransform("Rabbits");
		for (int j = 0; j < transform2.childCount; j++)
		{
			Animator component3 = transform2.GetChild(j).GetComponent<Animator>();
			if (component3 != null)
			{
				component3.speed = 1f;
			}
			Rigidbody component4 = transform2.GetChild(j).GetComponent<Rigidbody>();
			if ((bool)component4)
			{
				component4.useGravity = true;
				component4.drag = 0f;
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (!m_activationSequenceComplete)
		{
			m_activateTimer += Time.unscaledDeltaTime;
			if (m_activateTimer < m_activateWaitTime)
			{
				return;
			}
			m_zoomOutTimer += Time.unscaledDeltaTime;
			m_timeScaleLerpOutTimer += Time.unscaledDeltaTime;
			Time.timeScale = Mathf.Lerp(m_timeScale, 1f, m_timeScaleLerpOutTimer / m_timeScaleLerpOut);
			Singleton<Game>.Instance.VisualCamera.Camera.fieldOfView = Mathf.Lerp(m_fovZoom, Singleton<Game>.Instance.VisualCamera.NormalZoomFOV, m_zoomOutTimer / m_zoomOutTime);
			if (m_zoomOutTimer < m_zoomOutTime || m_timeScaleLerpOutTimer < m_timeScaleLerpOut)
			{
				return;
			}
			Singleton<Game>.Instance.VisualCamera.CanZoom = true;
			m_activationSequenceComplete = true;
		}
		m_timer += Time.deltaTime;
		Singleton<Game>.Instance.UIController.HUD.SetPowerupProgress(1f - m_timer / m_time);
		if (m_timer >= m_time)
		{
			Deactivate();
		}
		if (base.IsActive)
		{
			Singleton<Game>.Instance.Player.HasFeezePower = true;
		}
	}
}
