using System;
using CompilerGenerated;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class TestingAllJS : MonoBehaviour
{
	public AnimationCurve customAnimationCurve;

	public AnimationCurve shakeCurve;

	public Transform pt1;

	public Transform pt2;

	public Transform pt3;

	public Transform pt4;

	public Transform pt5;

	private int exampleIter;

	private __TestingAllJS_0024callable0_002432_33__[] exampleFunctions;

	private bool useEstimatedTime;

	private GameObject ltLogo;

	private LTDescr moveId;

	private LTDescr pingPongDescr;

	public TestingAllJS()
	{
		exampleFunctions = new __TestingAllJS_0024callable0_002432_33__[14]
		{
			updateValue3Example, loopTestPingPong, loopTestClamp, moveOnACurveExample, punchTest, customTweenExample, moveExample, rotateExample, scaleExample, updateValueExample,
			alphaExample, moveLocalExample, delayedCallExample, rotateAroundExample
		};
		useEstimatedTime = true;
	}

	public virtual void Awake()
	{
		LeanTween.init(400);
	}

	public virtual void Start()
	{
		ltLogo = GameObject.Find("LeanTweenLogo");
		cycleThroughExamples();
	}

	public virtual void OnGUI()
	{
		GUI.Label(new Rect(0.03f * (float)Screen.width, 0.03f * (float)Screen.height, 0.5f * (float)Screen.width, 0.3f * (float)Screen.height), "useEstimatedTime:" + useEstimatedTime);
	}

	public virtual void cycleThroughExamples()
	{
		if (exampleIter == 0)
		{
			useEstimatedTime = !useEstimatedTime;
			Time.timeScale = ((!useEstimatedTime) ? 1 : 0);
		}
		exampleFunctions[exampleIter]();
		exampleIter = ((exampleIter + 1 < Extensions.get_length((System.Array)exampleFunctions)) ? (exampleIter + 1) : 0);
		LeanTween.delayedCall(1.05f, cycleThroughExamples).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void updateValue3Example()
	{
		Debug.Log("updateValue3Example");
		LeanTween.value(ltLogo, updateValue3ExampleCallback, new Vector3(0f, 270f, 0f), new Vector3(30f, 270f, 180f), 0.5f).setEase(LeanTweenType.easeInBounce).setLoopPingPong()
			.setRepeat(2)
			.setOnUpdateVector3(updateValue3ExampleUpdate)
			.setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void updateValue3ExampleUpdate(Vector3 val)
	{
		Debug.Log("val:" + val);
	}

	public virtual void updateValue3ExampleCallback(Vector3 val)
	{
		ltLogo.transform.eulerAngles = val;
	}

	public virtual void loopTestClamp()
	{
		Debug.Log("loopTestClamp");
		GameObject gameObject = GameObject.Find("Cube1");
		float z = 1f;
		Vector3 localScale = gameObject.transform.localScale;
		float num = (localScale.z = z);
		Vector3 vector = (gameObject.transform.localScale = localScale);
		moveId = LeanTween.scaleZ(gameObject, 4f, 1f).setEase(LeanTweenType.easeOutElastic).setLoopClamp()
			.setRepeat(7)
			.setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void loopTestPingPong()
	{
		Debug.Log("loopTestPingPong");
		GameObject gameObject = GameObject.Find("Cube2");
		float y = 1f;
		Vector3 localScale = gameObject.transform.localScale;
		float num = (localScale.y = y);
		Vector3 vector = (gameObject.transform.localScale = localScale);
		pingPongDescr = LeanTween.scaleY(gameObject, 4f, 1f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(4)
			.setUseEstimatedTime(useEstimatedTime);
		Debug.Log("id:" + pingPongDescr.id);
	}

	public virtual void moveOnACurveExample()
	{
		Debug.Log("moveOnACurveExample");
		Vector3[] to = new Vector3[8]
		{
			ltLogo.transform.position,
			pt1.position,
			pt2.position,
			pt3.position,
			pt3.position,
			pt4.position,
			pt5.position,
			ltLogo.transform.position
		};
		LeanTween.move(ltLogo, to, 1f).setEase(LeanTweenType.easeInQuad).setOrientToPath(true)
			.setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void punchTest()
	{
		LeanTween.moveX(ltLogo, 7f, 1f).setEase(LeanTweenType.punch).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void customTweenExample()
	{
		Debug.Log("customTweenExample");
		LeanTween.moveX(ltLogo, -10f, 0.5f).setEase(customAnimationCurve).setUseEstimatedTime(useEstimatedTime);
		LeanTween.moveX(ltLogo, 0f, 0.5f).setDelay(0.5f).setEase(customAnimationCurve)
			.setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void moveExample()
	{
		Debug.Log("moveExample");
		LeanTween.move(ltLogo, new Vector3(-2f, -1f, 0f), 0.5f).setUseEstimatedTime(useEstimatedTime);
		LeanTween.move(ltLogo, ltLogo.transform.position, 0.5f).setDelay(0.5f).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void rotateExample()
	{
		Debug.Log("rotateExample");
		LeanTween.rotate(ltLogo, new Vector3(0f, 360f, 0f), 1f).setEase(LeanTweenType.easeOutQuad).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void scaleExample()
	{
		Debug.Log("scaleExample");
		Vector3 localScale = ltLogo.transform.localScale;
		LeanTween.scale(ltLogo, new Vector3(localScale.x + 0.2f, localScale.y + 0.2f, localScale.z + 0.2f), 1f).setEase(LeanTweenType.easeOutBounce).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void updateValueExample()
	{
		Debug.Log("updateValueExample");
		LeanTween.value(ltLogo, updateValueExampleCallback, ltLogo.transform.eulerAngles.y, 270f, 1f).setEase(LeanTweenType.easeOutElastic).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void updateValueExampleCallback(float val)
	{
		Vector3 eulerAngles = ltLogo.transform.eulerAngles;
		float num = (eulerAngles.y = val);
		Vector3 vector = (ltLogo.transform.eulerAngles = eulerAngles);
	}

	public virtual void delayedCallExample()
	{
		Debug.Log("delayedCallExample");
		LeanTween.delayedCall(0.5f, delayedCallExampleCallback).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void delayedCallExampleCallback()
	{
		Debug.Log("Delayed function was called");
		Vector3 localScale = gameObject.transform.localScale;
		LeanTween.scale(ltLogo, new Vector3(localScale.x - 0.2f, localScale.y - 0.2f, localScale.z - 0.2f), 0.5f).setEase(LeanTweenType.easeInOutCirc).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void alphaExample()
	{
		Debug.Log("alphaExample");
		GameObject gameObject = GameObject.Find("LCharacter");
		LeanTween.alpha(gameObject, 0f, 0.5f).setUseEstimatedTime(useEstimatedTime);
		LeanTween.alpha(gameObject, 1f, 0.5f).setDelay(0.5f).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void moveLocalExample()
	{
		Debug.Log("moveLocalExample");
		GameObject gameObject = GameObject.Find("LCharacter");
		Vector3 localPosition = gameObject.transform.localPosition;
		LeanTween.moveLocal(gameObject, new Vector3(0f, 2f, 0f), 0.5f).setUseEstimatedTime(useEstimatedTime);
		LeanTween.moveLocal(gameObject, localPosition, 0.5f).setDelay(0.5f).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void rotateAroundExample()
	{
		Debug.Log("rotateAroundExample");
		GameObject gameObject = GameObject.Find("LCharacter");
		LeanTween.rotateAround(gameObject, Vector3.up, 360f, 1f).setUseEstimatedTime(useEstimatedTime);
	}

	public virtual void moveXExample()
	{
		LeanTween.moveX(ltLogo, 5f, 0.5f);
	}

	public virtual void rotateXExample()
	{
	}

	public virtual void scaleXExample()
	{
	}

	public virtual void loopPause()
	{
		moveId.pause();
	}

	public virtual void loopResume()
	{
		moveId.resume();
	}

	public virtual void loopCancel()
	{
		pingPongDescr.cancel();
	}

	public virtual void Main()
	{
	}
}
