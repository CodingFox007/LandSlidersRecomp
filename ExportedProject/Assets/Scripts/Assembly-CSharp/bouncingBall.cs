using UnityEngine;

public class bouncingBall : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve m_bounceCurve;

	[SerializeField]
	private float m_yHeight;

	[SerializeField]
	private float m_bounceTime;

	private float bounceTimer;

	[SerializeField]
	private float m_rotationFactor;

	private float rotSpeed;

	private Vector3 ogPos;

	private Vector3 vel = Vector3.zero;

	private void Start()
	{
		ogPos = base.transform.localPosition;
	}

	private void Update()
	{
		if (bounceTimer > m_bounceTime)
		{
			bounceTimer = 0f;
			vel = Singleton<Game>.Instance.Player.Rigidbody.velocity;
			rotSpeed = m_rotationFactor * vel.magnitude;
		}
		base.transform.localRotation = Quaternion.AngleAxis(rotSpeed, Vector3.Cross(Vector3.up, vel.normalized)) * base.transform.localRotation;
		base.transform.localPosition = new Vector3(ogPos.x, ogPos.y + m_yHeight * m_bounceCurve.Evaluate(bounceTimer / m_bounceTime), ogPos.z);
		if (!Singleton<Game>.Instance.VisualCamera.Target.IsAirWalking() && !Singleton<Game>.Instance.Player.IsFrozen)
		{
			bounceTimer += Time.deltaTime;
		}
	}
}
