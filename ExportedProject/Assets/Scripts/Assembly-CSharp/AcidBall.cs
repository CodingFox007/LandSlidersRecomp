using System;
using DarkTonic.MasterAudio;
using UnityEngine;

public class AcidBall : MonoBehaviour
{
	[SerializeField]
	private Renderer m_ballRenderer;

	[SerializeField]
	private Renderer m_shadowRenderer;

	[SerializeField]
	private Renderer m_ringRenderer;

	[SerializeField]
	private Collider m_deathCollider;

	[SerializeField]
	private GameObject m_shadow;

	[SerializeField]
	private float m_shadowMinScale = 0.1f;

	[SerializeField]
	private float m_shadowMaxScale = 2f;

	[SerializeField]
	private float m_shadowMinHeight = 7.5f;

	[SerializeField]
	private float m_shadowMaxHeight = 75f;

	[SerializeField]
	private ParticleSystem m_particle;

	[SerializeField]
	private float m_fallSpeed = 2f;

	[SerializeField]
	private float m_fallAccel = 0.1f;

	private float m_vel;

	[SerializeField]
	private float m_explosionSize = 2f;

	private float m_groundHeight;

	private bool m_exploded;

	private bool m_heightCalculated;

	private void Start()
	{
		m_vel = m_fallSpeed;
		m_ringRenderer.enabled = true;
		m_particle.startSize = m_explosionSize;
		m_ringRenderer.transform.localScale = new Vector3(m_explosionSize, m_explosionSize, m_explosionSize);
		m_deathCollider.transform.localScale = new Vector3(m_explosionSize, m_explosionSize, m_explosionSize);
		m_ballRenderer.enabled = false;
		m_shadowRenderer.enabled = false;
		m_ringRenderer.enabled = false;
		UpdateShadow();
	}

	private void Update()
	{
		if (!m_heightCalculated)
		{
			string surfaceType = string.Empty;
			m_groundHeight = CalculateGroundHeight(base.transform.position, out surfaceType);
			if (m_groundHeight == float.MaxValue || surfaceType == "Air" || surfaceType == "Water")
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			m_heightCalculated = true;
			m_ballRenderer.enabled = true;
			m_shadowRenderer.enabled = true;
			m_ringRenderer.enabled = true;
		}
		Vector3 position = base.transform.position;
		if (position.y > m_groundHeight)
		{
			m_vel += m_fallAccel;
			position.y -= m_vel * Time.deltaTime;
			base.transform.position = position;
		}
		if (position.y <= m_groundHeight)
		{
			position.y = m_groundHeight;
			base.transform.position = position;
			if (!m_exploded)
			{
				MasterAudio.PlaySound("NP-SpiderAcidImpact");
				m_particle.Play();
				m_ballRenderer.enabled = false;
				m_shadowRenderer.enabled = false;
				m_ringRenderer.enabled = false;
				m_exploded = true;
				m_deathCollider.enabled = true;
			}
			else
			{
				m_deathCollider.enabled = false;
				if (!m_particle.isPlaying)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
		UpdateShadow();
	}

	private void UpdateShadow()
	{
		Vector3 position = m_shadow.transform.position;
		position.y = m_groundHeight + 0.01f;
		m_shadow.transform.position = position;
		m_ringRenderer.transform.position = position;
		float t = Mathf.InverseLerp(m_shadowMaxHeight, m_shadowMinHeight, base.transform.position.y);
		float num = Mathf.Lerp(m_shadowMinScale, m_shadowMaxScale, t) * m_explosionSize;
		m_shadow.transform.localScale = new Vector3(num, num, num);
	}

	private float CalculateGroundHeight(Vector3 pos, out string surfaceType)
	{
		surfaceType = "Air";
		float result = float.MaxValue;
		int num = 1 << LayerMask.NameToLayer("Ground");
		num |= 1 << LayerMask.NameToLayer("Water");
		RaycastHit hitInfo;
		if (Physics.Raycast(new Ray(pos + new Vector3(0f, 10f, 0f), -base.transform.up.normalized), out hitInfo, 100f, num))
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
