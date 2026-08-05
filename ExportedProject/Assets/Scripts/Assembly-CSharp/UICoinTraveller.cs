using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICoinTraveller : MonoBehaviour
{
	[SerializeField]
	private Image m_icon;

	[SerializeField]
	private UnityEvent m_onComplete = new UnityEvent();

	private float m_travelSpeed;

	private Graphic m_target;

	private LeanTweenType m_travelTween;

	private LTDescr m_travelDescr;

	private Vector3 m_vel;

	public bool IsActive
	{
		get
		{
			return m_icon.enabled;
		}
		set
		{
			m_icon.enabled = value;
		}
	}

	public void Spawn(Graphic source, Graphic target, float explodeDistance, float explodeSpeed, float travelSpeed, LeanTweenType explodeTween, LeanTweenType travelTween)
	{
		m_target = target;
		m_travelTween = travelTween;
		base.gameObject.transform.position = source.transform.position;
		m_travelSpeed = travelSpeed;
		float z = Random.Range(0f, 360f);
		Quaternion rotation = base.transform.rotation;
		base.transform.rotation = Quaternion.Euler(0f, 0f, z);
		Vector3 vector = base.gameObject.transform.position + base.transform.up.normalized * explodeDistance;
		base.transform.rotation = rotation;
		float time = (vector - base.transform.position).magnitude / explodeSpeed;
		LTDescr lTDescr = LeanTween.move(base.gameObject, vector, time);
		lTDescr.setIgnoreTimeScale(true);
		lTDescr.setOnComplete(ExplodeComplete);
		lTDescr.setLoopOnce();
		lTDescr.setEase(explodeTween);
	}

	private void ExplodeComplete()
	{
		float time = (m_target.transform.position - base.transform.position).magnitude / m_travelSpeed;
		m_travelDescr = LeanTween.move(base.gameObject, m_target.transform.position, time);
		m_travelDescr.setOnComplete(TravelComplete);
		m_travelDescr.setOnUpdateVector3(TravelUpdate);
		m_travelDescr.setEase(m_travelTween);
		m_travelDescr.setIgnoreTimeScale(true);
	}

	private void TravelUpdate(Vector3 pos)
	{
		m_travelDescr.to = m_target.transform.position;
	}

	private void TravelComplete()
	{
		m_onComplete.Invoke();
		IsActive = false;
	}
}
