using System;
using UnityEngine;

[Serializable]
public class GeneralEventsListenersJS : MonoBehaviour
{
	private Vector3 towardsRotation;

	private float turnForLength;

	private float turnForIter;

	private Vector3 fromColor;

	public GeneralEventsListenersJS()
	{
		turnForLength = 0.5f;
	}

	public virtual void Awake()
	{
		LeanTween.LISTENERS_MAX = 100;
		LeanTween.EVENTS_MAX = 2;
		fromColor = new Vector3(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b);
	}

	public virtual void Start()
	{
		LeanTween.addListener(gameObject, 0, changeColor);
		LeanTween.addListener(gameObject, 1, jumpUp);
	}

	public virtual void jumpUp(LTEvent e)
	{
		GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 300f);
	}

	public virtual void changeColor(LTEvent e)
	{
		Transform transform = e.data as Transform;
		float num = Vector3.Distance(transform.position, this.transform.position);
		Vector3 to = new Vector3(UnityEngine.Random.Range(0f, 1f), 0f, UnityEngine.Random.Range(0f, 1f));
		LeanTween.value(gameObject, updateColor, fromColor, to, 0.8f).setLoopPingPong(1).setDelay(num * 0.05f);
	}

	public virtual void updateColor(Vector3 v)
	{
		GetComponent<Renderer>().material.color = new Color(v.x, v.y, v.z);
	}

	public virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer != 2)
		{
			towardsRotation = new Vector3(0f, UnityEngine.Random.Range(-180, 180), 0f);
		}
	}

	public virtual void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.layer != 2)
		{
			turnForIter = 0f;
			turnForLength = UnityEngine.Random.Range(0.5f, 1.5f);
		}
	}

	public virtual void FixedUpdate()
	{
		if (!(turnForIter >= turnForLength))
		{
			GetComponent<Rigidbody>().MoveRotation(GetComponent<Rigidbody>().rotation * Quaternion.Euler(towardsRotation * Time.deltaTime));
			turnForIter += Time.deltaTime;
		}
		GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 4.5f);
	}

	public virtual void OnMouseDown()
	{
		if (Input.GetKey(KeyCode.J))
		{
			LeanTween.dispatchEvent(1);
		}
		else
		{
			LeanTween.dispatchEvent(0, transform);
		}
	}

	public virtual void Main()
	{
	}
}
