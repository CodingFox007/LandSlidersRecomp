using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Elevator : MonoBehaviour, IEventSystemHandler
{
	private enum State
	{
		WaitingDown = 0,
		MovingUp = 1,
		WaitingUp = 2,
		MovingDown = 3
	}

	[SerializeField]
	private GameObject m_movingObject;

	[SerializeField]
	private AnimationCurve m_curveUp;

	[SerializeField]
	private float m_upTime = 1f;

	[SerializeField]
	private AnimationCurve m_curveDown;

	[SerializeField]
	private float m_downTime = 1f;

	[SerializeField]
	private UnityEvent m_onUpTriggered = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onDownTriggered = new UnityEvent();

	private float m_minHeight;

	private float m_maxHeight = 7.5f;

	private float m_timer;

	private State m_state;

	private bool m_playerColliding;

	private GameObject m_rider;

	private void Start()
	{
		m_state = State.WaitingDown;
		m_minHeight = base.transform.position.y;
		m_maxHeight = m_minHeight + 7.5f;
	}

	private void FixedUpdate()
	{
		Vector3 position = m_movingObject.GetComponent<Rigidbody>().position;
		if (m_rider == null)
		{
			m_playerColliding = false;
		}
		switch (m_state)
		{
		case State.WaitingDown:
			position.y = m_minHeight;
			if (m_playerColliding)
			{
				m_timer = 0f;
				m_state = State.MovingUp;
				m_onUpTriggered.Invoke();
			}
			break;
		case State.MovingUp:
		{
			m_timer += Time.deltaTime;
			float num2 = m_maxHeight - m_minHeight;
			position.y = m_minHeight + m_curveUp.Evaluate(m_timer / m_upTime) * num2;
			if (m_timer >= m_upTime)
			{
				m_state = State.WaitingUp;
				if (m_rider == Singleton<Game>.Instance.Player.gameObject)
				{
					Singleton<Game>.Instance.StatsManager.GetStat("RideElevator").IncreaseStat(1f);
				}
			}
			break;
		}
		case State.WaitingUp:
			position.y = m_maxHeight;
			if (!m_playerColliding)
			{
				m_timer = 0f;
				m_state = State.MovingDown;
				m_onDownTriggered.Invoke();
			}
			break;
		case State.MovingDown:
		{
			m_timer += Time.deltaTime;
			float num = m_maxHeight - m_minHeight;
			position.y = m_minHeight + m_curveDown.Evaluate(m_timer / m_downTime) * num;
			if (m_timer >= m_downTime)
			{
				m_state = State.WaitingDown;
			}
			break;
		}
		}
		m_movingObject.GetComponent<Rigidbody>().MovePosition(position);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.transform.parent != null) || !(other.transform.parent.GetComponent<AcidBall>() != null))
		{
			m_playerColliding = true;
			m_rider = other.gameObject;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!(other.transform.parent != null) || !(other.transform.parent.GetComponent<AcidBall>() != null))
		{
			m_playerColliding = true;
			m_rider = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!(other.transform.parent != null) || !(other.transform.parent.GetComponent<AcidBall>() != null))
		{
			m_playerColliding = false;
			m_rider = null;
		}
	}
}
