using System;
using UnityEngine;

public class ControlGroundHeight : MonoBehaviour
{
	[SerializeField]
	private float m_heightAboveGround;

	[SerializeField]
	private float m_smoothTime = 1f;

	private float m_velocity;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		string surfaceType = string.Empty;
		float num = CalculateGroundHeight(base.transform.position, out surfaceType);
		if (num != float.MaxValue)
		{
			float target = num + m_heightAboveGround;
			Vector3 position = base.transform.position;
			position.y = Mathf.SmoothDamp(position.y, target, ref m_velocity, m_smoothTime);
			base.transform.position = position;
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
