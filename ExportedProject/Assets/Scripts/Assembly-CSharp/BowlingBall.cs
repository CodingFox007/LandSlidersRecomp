using UnityEngine;

public class BowlingBall : MonoBehaviour
{
	[SerializeField]
	private Rigidbody m_rigidBody;

	[SerializeField]
	private float m_velScale = 1f;

	private void FixedUpdate()
	{
		Vector3 velocity = Singleton<Game>.Instance.Player.Rigidbody.velocity;
		m_rigidBody.AddTorque(Vector3.Cross(Vector3.up, velocity * m_velScale), ForceMode.Force);
	}
}
