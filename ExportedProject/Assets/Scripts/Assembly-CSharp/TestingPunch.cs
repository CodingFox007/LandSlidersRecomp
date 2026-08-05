using System;
using UnityEngine;

public class TestingPunch : MonoBehaviour
{
	public AnimationCurve exportCurve;

	private void Start()
	{
		Debug.Log("exported curve:" + curveToString(exportCurve));
	}

	private void Update()
	{
		LeanTween.dtManual = Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.Q))
		{
			LeanTween.moveLocalX(base.gameObject, 5f, 1f).setOnComplete((Action)delegate
			{
				Debug.Log("on complete move local X");
			}).setOnCompleteOnStart(true);
			GameObject gameObject = GameObject.Find("DirectionalLight");
			Light lt = gameObject.GetComponent<Light>();
			LeanTween.value(lt.gameObject, lt.intensity, 0f, 1.5f).setEase(LeanTweenType.linear).setLoopPingPong()
				.setRepeat(-1)
				.setOnUpdate(delegate(float val)
				{
					lt.intensity = val;
				});
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			LeanTween.scale(base.gameObject, Vector3.one * 3f, 1f).setEase(LeanTweenType.easeSpring);
			MonoBehaviour.print("scale punch!");
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			LeanTween.rotateAroundLocal(base.gameObject, base.transform.forward, -80f, 5f).setPoint(new Vector3(1.25f, 0f, 0f));
			MonoBehaviour.print("rotate punch!");
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			MonoBehaviour.print("move punch!");
			Time.timeScale = 0.25f;
			float start = Time.realtimeSinceStartup;
			LeanTween.moveX(base.gameObject, 1f, 1f).setOnComplete(destroyOnComp).setOnCompleteParam(base.gameObject)
				.setOnComplete((Action)delegate
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					float num = realtimeSinceStartup - start;
					Debug.Log("start:" + start + " end:" + realtimeSinceStartup + " diff:" + num);
				});
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			LeanTween.color(base.gameObject, new Color(1f, 0f, 0f, 0.5f), 1f);
			Color to = new Color(UnityEngine.Random.Range(0f, 1f), 0f, UnityEngine.Random.Range(0f, 1f), 0f);
			GameObject gameObject2 = GameObject.Find("LCharacter");
			LeanTween.color(gameObject2, to, 4f).setLoopPingPong(1).setEase(LeanTweenType.easeOutBounce);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			LeanTween.delayedCall(base.gameObject, 0.3f, delayedMethod).setRepeat(4).setOnCompleteOnRepeat(true)
				.setOnCompleteParam("hi");
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			LeanTween.value(base.gameObject, updateColor, new Color(1f, 0f, 0f, 1f), Color.blue, 4f);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			LeanTween.delayedCall(0.05f, enterMiniGameStart).setOnCompleteParam(new object[1] { string.Empty + 5 });
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			LeanTween.value(base.gameObject, delegate(Vector2 val)
			{
				base.transform.position = new Vector3(val.x, base.transform.position.y, base.transform.position.z);
			}, new Vector2(0f, 0f), new Vector2(5f, 100f), 1f).setEase(LeanTweenType.easeOutBounce);
			GameObject l = GameObject.Find("LCharacter");
			Debug.Log("x:" + l.transform.position.x + " y:" + l.transform.position.y);
			LeanTween.value(l, new Vector2(l.transform.position.x, l.transform.position.y), new Vector2(l.transform.position.x, l.transform.position.y + 5f), 1f).setOnUpdate(delegate(Vector2 val)
			{
				Debug.Log("tweening vec2 val:" + val);
				l.transform.position = new Vector3(val.x, val.y, base.transform.position.z);
			});
		}
	}

	private void enterMiniGameStart(object val)
	{
		object[] array = (object[])val;
		int num = int.Parse((string)array[0]);
		Debug.Log("level:" + num);
	}

	private void updateColor(Color c)
	{
		GameObject gameObject = GameObject.Find("LCharacter");
		gameObject.GetComponent<Renderer>().material.color = c;
	}

	private void delayedMethod(object myVal)
	{
		string text = myVal as string;
		Debug.Log("delayed call:" + Time.time + " myVal:" + text);
	}

	private void destroyOnComp(object p)
	{
		GameObject obj = (GameObject)p;
		UnityEngine.Object.Destroy(obj);
	}

	private string curveToString(AnimationCurve curve)
	{
		string text = string.Empty;
		for (int i = 0; i < curve.length; i++)
		{
			string text2 = text;
			text = text2 + "new Keyframe(" + curve[i].time + "f, " + curve[i].value + "f)";
			if (i < curve.length - 1)
			{
				text += ", ";
			}
		}
		return "new AnimationCurve( " + text + " )";
	}
}
