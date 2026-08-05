using UnityEngine;

public class randomizeAnimationSpeed : MonoBehaviour
{
	[SerializeField]
	private Animator m_animator;

	[SerializeField]
	private float m_minSpeed;

	[SerializeField]
	private float m_maxSpeed;

	private void Start()
	{
		m_animator.speed = Random.Range(m_minSpeed, m_maxSpeed);
	}

	private void Update()
	{
	}
}
