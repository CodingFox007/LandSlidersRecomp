using System;
using System.Collections;
using UnityEngine;

public class TestingEverything : MonoBehaviour
{
	public GameObject cube1;

	public GameObject cube2;

	public GameObject cube3;

	public GameObject cube4;

	private bool eventGameObjectWasCalled;

	private bool eventGeneralWasCalled;

	private LTDescr lt1;

	private LTDescr lt2;

	private LTDescr lt3;

	private LTDescr lt4;

	private LTDescr[] groupTweens;

	private GameObject[] groupGOs;

	private int groupTweensCnt;

	private int rotateRepeat;

	private int rotateRepeatAngle;

	private void Start()
	{
		LeanTest.timeout = 7f;
		LeanTest.expected = 24;
		LeanTween.init(1203);
		LeanTween.addListener(cube1, 0, eventGameObjectCalled);
		LeanTest.expect(!LeanTween.isTweening(), "NOTHING TWEEENING AT BEGINNING");
		LeanTest.expect(!LeanTween.isTweening(cube1), "OBJECT NOT TWEEENING AT BEGINNING");
		LeanTween.dispatchEvent(0);
		LeanTest.expect(eventGameObjectWasCalled, "EVENT GAMEOBJECT RECEIVED");
		LeanTest.expect(!LeanTween.removeListener(cube2, 0, eventGameObjectCalled), "EVENT GAMEOBJECT NOT REMOVED");
		LeanTest.expect(LeanTween.removeListener(cube1, 0, eventGameObjectCalled), "EVENT GAMEOBJECT REMOVED");
		LeanTween.addListener(1, eventGeneralCalled);
		LeanTween.dispatchEvent(1);
		LeanTest.expect(eventGeneralWasCalled, "EVENT ALL RECEIVED");
		LeanTest.expect(LeanTween.removeListener(1, eventGeneralCalled), "EVENT ALL REMOVED");
		lt1 = LeanTween.move(cube1, new Vector3(3f, 2f, 0.5f), 1.1f);
		LeanTween.move(cube2, new Vector3(-3f, -2f, -0.5f), 1.1f);
		LeanTween.reset();
		rotateRepeat = (rotateRepeatAngle = 0);
		LeanTween.rotateAround(cube3, Vector3.forward, 360f, 0.1f).setRepeat(3).setOnComplete(rotateRepeatFinished)
			.setOnCompleteOnRepeat(true)
			.setDestroyOnComplete(true);
		LeanTween.delayedCall(0.8f, rotateRepeatAllFinished);
		StartCoroutine(timeBasedTesting());
	}

	private IEnumerator timeBasedTesting()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Time.timeScale = 0.25f;
		float tweenTime = 0.2f;
		float start = Time.realtimeSinceStartup;
		bool onUpdateWasCalled = false;
		LeanTween.moveX(cube1, -5f, tweenTime).setOnUpdate((Action<float>)delegate
		{
			onUpdateWasCalled = true;
		}).setOnComplete((Action)delegate
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num5 = realtimeSinceStartup - start;
			LeanTest.expect(Mathf.Abs(tweenTime * (1f / Time.timeScale) - num5) < 0.05f, "SCALED TIMING DIFFERENCE", "expected to complete in roughly 0.8f but completed in " + num5);
			LeanTest.expect(Mathf.Approximately(cube1.transform.position.x, -5f), "SCALED ENDING POSITION", "expected to end at -5f, but it ended at " + cube1.transform.position.x);
			LeanTest.expect(onUpdateWasCalled, "ON UPDATE FIRED");
		});
		yield return new WaitForSeconds(1f);
		Time.timeScale = 1f;
		groupTweens = new LTDescr[1200];
		groupGOs = new GameObject[groupTweens.Length];
		groupTweensCnt = 0;
		int descriptionMatchCount = 0;
		for (int num = 0; num < groupTweens.Length; num++)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			UnityEngine.Object.Destroy(gameObject.GetComponent(typeof(BoxCollider)));
			gameObject.transform.position = new Vector3(0f, 0f, num * 3);
			gameObject.name = "c" + num;
			groupGOs[num] = gameObject;
			groupTweens[num] = LeanTween.move(gameObject, base.transform.position + Vector3.one * 3f, 0.6f).setOnComplete(groupTweenFinished);
			if (LeanTween.description(groupTweens[num].id).trans == groupTweens[num].trans)
			{
				descriptionMatchCount++;
			}
		}
		LeanTween.delayedCall(base.gameObject, 0.82f, groupTweensFinished);
		LeanTest.expect(descriptionMatchCount == groupTweens.Length, "GROUP IDS MATCH");
		LeanTest.expect(LeanTween.maxSearch <= groupTweens.Length + 5, "MAX SEARCH OPTIMIZED", "maxSearch:" + LeanTween.maxSearch);
		LeanTest.expect(LeanTween.isTweening(), "SOMETHING IS TWEENING");
		float previousXlt4 = cube4.transform.position.x;
		lt4 = LeanTween.moveX(cube4, 5f, 1.1f).setOnComplete((Action)delegate
		{
			LeanTest.expect(cube4 != null && previousXlt4 != cube4.transform.position.x, "RESUME OUT OF ORDER", string.Concat("cube4:", cube4, " previousXlt4:", previousXlt4, " cube4.transform.position.x:", (!(cube4 != null)) ? 0f : cube4.transform.position.x));
		});
		lt4.resume();
		yield return new WaitForSeconds(0.1f);
		int countBeforeCancel = LeanTween.tweensRunning;
		lt1.cancel();
		LeanTest.expect(countBeforeCancel == LeanTween.tweensRunning, "CANCEL AFTER RESET SHOULD FAIL", "expected " + countBeforeCancel + " but got " + LeanTween.tweensRunning);
		LeanTween.cancel(cube2);
		int tweenCount = 0;
		for (int num2 = 0; num2 < groupTweens.Length; num2++)
		{
			if (LeanTween.isTweening(groupGOs[num2]))
			{
				tweenCount++;
			}
			if (num2 % 3 == 0)
			{
				LeanTween.pause(groupGOs[num2]);
			}
			else if (num2 % 3 == 1)
			{
				groupTweens[num2].pause();
			}
			else
			{
				LeanTween.pause(groupTweens[num2].id);
			}
		}
		LeanTest.expect(tweenCount == groupTweens.Length, "GROUP ISTWEENING", "expected " + groupTweens.Length + " tweens but got " + tweenCount);
		yield return new WaitForEndOfFrame();
		tweenCount = 0;
		for (int num3 = 0; num3 < groupTweens.Length; num3++)
		{
			if (num3 % 3 == 0)
			{
				LeanTween.resume(groupGOs[num3]);
			}
			else if (num3 % 3 == 1)
			{
				groupTweens[num3].resume();
			}
			else
			{
				LeanTween.resume(groupTweens[num3].id);
			}
			if ((num3 % 2 != 0) ? LeanTween.isTweening(groupGOs[num3]) : LeanTween.isTweening(groupTweens[num3].id))
			{
				tweenCount++;
			}
		}
		LeanTest.expect(tweenCount == groupTweens.Length, "GROUP RESUME");
		LeanTest.expect(!LeanTween.isTweening(cube1), "CANCEL TWEEN LTDESCR");
		LeanTest.expect(!LeanTween.isTweening(cube2), "CANCEL TWEEN LEANTWEEN");
		int ltCount = 0;
		GameObject[] allGos = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		GameObject[] array = allGos;
		foreach (GameObject gameObject2 in array)
		{
			if (gameObject2.name == "~LeanTween")
			{
				ltCount++;
			}
		}
		LeanTest.expect(ltCount == 1, "RESET CORRECTLY CLEANS UP");
	}

	private void rotateRepeatFinished()
	{
		if (Mathf.Abs(cube3.transform.eulerAngles.z) < 0.0001f)
		{
			rotateRepeatAngle++;
		}
		rotateRepeat++;
	}

	private void rotateRepeatAllFinished()
	{
		LeanTest.expect(rotateRepeatAngle == 3, "ROTATE AROUND MULTIPLE", "expected 3 times received " + rotateRepeatAngle + " times");
		LeanTest.expect(rotateRepeat == 3, "ROTATE REPEAT");
		LeanTest.expect(cube3 == null, "DESTROY ON COMPLETE", "cube3:" + cube3);
	}

	private void groupTweenFinished()
	{
		groupTweensCnt++;
	}

	private void groupTweensFinished()
	{
		LeanTest.expect(groupTweensCnt == groupTweens.Length, "GROUP FINISH", "expected " + groupTweens.Length + " tweens but got " + groupTweensCnt);
	}

	private void eventGameObjectCalled(LTEvent e)
	{
		eventGameObjectWasCalled = true;
	}

	private void eventGeneralCalled(LTEvent e)
	{
		eventGeneralWasCalled = true;
	}
}
