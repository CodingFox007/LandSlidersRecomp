using System;
using UnityEngine;

public class Snowball : MonoBehaviour
{
	[SerializeField]
	private Renderer m_ballRenderer;

	[SerializeField]
	private Renderer m_shadowRenderer;

	[SerializeField]
	private Collider m_deathCollider;

	[SerializeField]
	private GameObject m_shadow;

	[SerializeField]
	private float m_fallSpeed = 2f;

	[SerializeField]
	private float m_fallAccel = 0.1f;

	private float m_vel;

	private void Start()
	{
		m_vel = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue("Snowball Speed", m_fallSpeed);
		m_ballRenderer.enabled = true;
		m_shadowRenderer.enabled = false;
		UpdateShadow();
	}

	public Vector3 GetVelocity()
	{
		float num = base.transform.rotation.eulerAngles.y - 90f;
		Vector3 vector = new Vector3(Mathf.Cos((float)Math.PI / 180f * num), 0f, 0f - Mathf.Sin((float)Math.PI / 180f * num));
		return vector * (m_vel * Time.deltaTime);
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		m_vel += m_fallAccel;
		position += GetVelocity();
		base.transform.position = position;
		UpdateShadow();
	}

	private void UpdateShadow()
	{
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
