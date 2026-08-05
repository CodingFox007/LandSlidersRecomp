using UnityEngine;

public class TestingUniqueId : MonoBehaviour
{
	private GameObject ltLogo;

	private int cube1ScaleZId;

	private int cube2ScaleYId;

	private int slowShakeId;

	private GameObject cube1;

	private GameObject cube2;

	private void Awake()
	{
	}

	private void Start()
	{
		ltLogo = GameObject.Find("LeanTweenLogo");
		loopTestClamp();
		loopTestPingPong();
		LeanTween.delayedCall(1f, loopCancel);
		LeanTween.delayedCall(10f, loopCancel2);
		LeanTween.delayedCall(5f, loopPause);
		LeanTween.delayedCall(8f, loopResume);
	}

	private void pauseNow()
	{
		Time.timeScale = 0f;
		Debug.Log("pausing");
	}

	private void endlessCallback()
	{
		Debug.Log("endless");
	}

	public void loopTestClamp()
	{
		cube1 = GameObject.Find("Cube1");
		cube1.transform.localScale = new Vector3(1f, 1f, 1f);
		cube1ScaleZId = LeanTween.scaleZ(cube1, 4f, 1f).setEase(LeanTweenType.easeOutElastic).setRepeat(7)
			.setLoopClamp()
			.id;
		slowShakeId = LeanTween.moveY(cube1, cube1.transform.position.y + 0.3f, 1f).setEase(LeanTweenType.easeInOutCirc).setRepeat(-1)
			.setLoopPingPong()
			.id;
	}

	public void loopTestPingPong()
	{
		cube2 = GameObject.Find("Cube2");
		cube2.transform.localScale = new Vector3(1f, 1f, 1f);
		cube2ScaleYId = LeanTween.scaleY(cube2, 4f, 1f).setEase(LeanTweenType.easeOutQuad).setLoopPingPong()
			.setRepeat(8)
			.id;
	}

	public void loopCancel()
	{
		LeanTween.cancel(cube1, cube1ScaleZId);
	}

	public void loopCancel2()
	{
		LeanTween.cancel(cube1, slowShakeId);
	}

	public void loopPause()
	{
		LeanTween.pause(cube2ScaleYId);
	}

	public void loopResume()
	{
		LeanTween.resume(cube2ScaleYId);
	}

	public void punchTest()
	{
		LeanTween.moveX(ltLogo, 7f, 1f).setEase(LeanTweenType.punch);
	}
}
