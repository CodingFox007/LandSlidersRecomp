using UnityEngine;

public class PowerupInvincible : Powerup
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
	private float m_invincibleGracePeriod;

	[SerializeField]
	private float m_zoomOutTime = 0.3f;

	private float m_zoomOutTimer;

	[SerializeField]
	private float m_timeScaleLerpOut = 0.3f;

	private float m_timeScaleLerpOutTimer;

	private bool m_activationSequenceComplete;

	private float m_fullTime;

	public Material PlayerMaterial
	{
		get
		{
			return m_playerMaterial;
		}
	}

	public void AddTime(float time)
	{
		m_timer += time;
	}

	public override void Activate()
	{
		base.Activate();
		m_fullTime = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Hot Sauce Time", m_time);
		Singleton<Game>.Instance.Player.Invincible = true;
		Singleton<Game>.Instance.Player.Character.SetMaterials(m_playerMaterial);
		m_timer = 0f;
		if (!m_isSilent)
		{
			Singleton<Game>.Instance.VisualCamera.Camera.fieldOfView = m_fovZoom;
			Singleton<Game>.Instance.VisualCamera.CanZoom = false;
		}
		Time.timeScale = m_timeScale;
		m_activateTimer = 0f;
		m_zoomOutTimer = 0f;
		m_timeScaleLerpOutTimer = 0f;
		m_activationSequenceComplete = false;
		if (!m_isSilent)
		{
			Singleton<Game>.Instance.UIController.HUD.ShowPowerup();
			Singleton<Game>.Instance.UIController.HUD.SetPowerupProgress(1f);
			Singleton<Game>.Instance.VisualCamera.Shake("Invincible");
		}
	}

	public override void Deactivate()
	{
		base.Deactivate();
		Singleton<Game>.Instance.Player.InvincibleGracePeriod = m_invincibleGracePeriod;
		Singleton<Game>.Instance.Player.Invincible = false;
		Singleton<Game>.Instance.Player.Character.UnsetMaterials();
		Singleton<Game>.Instance.UIController.HUD.HidePowerup();
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
			if (!m_isSilent)
			{
				Singleton<Game>.Instance.VisualCamera.Camera.fieldOfView = Mathf.Lerp(m_fovZoom, Singleton<Game>.Instance.VisualCamera.NormalZoomFOV, m_zoomOutTimer / m_zoomOutTime);
			}
			if (m_zoomOutTimer < m_zoomOutTime || m_timeScaleLerpOutTimer < m_timeScaleLerpOut)
			{
				return;
			}
			Singleton<Game>.Instance.VisualCamera.CanZoom = true;
			m_activationSequenceComplete = true;
		}
		m_timer += Time.deltaTime;
		Singleton<Game>.Instance.UIController.HUD.SetPowerupProgress(1f - m_timer / m_fullTime);
		if (m_timer >= m_fullTime)
		{
			Deactivate();
		}
		if (base.IsActive)
		{
			Singleton<Game>.Instance.Player.Invincible = true;
		}
	}
}
