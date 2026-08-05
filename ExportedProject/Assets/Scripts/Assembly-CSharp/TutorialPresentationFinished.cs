using UnityEngine;

public class TutorialPresentationFinished : MonoBehaviour
{
	public GameObject[] avatars;

	public AnimationCurve gameOverCurve;

	private LTRect gameOverButtonRect;

	private void Start()
	{
		gameOverButtonRect = new LTRect((float)(-Screen.width) * 0.5f, (float)Screen.height * 0.4f, (float)Screen.width * 0.2f, (float)Screen.height * 0.2f);
		for (int i = 0; i < avatars.Length; i++)
		{
			LeanTween.moveY(avatars[i], avatars[i].transform.position.y + 4f, 1f).setDelay((float)i * 0.25f).setLoopPingPong()
				.setRepeat(2)
				.setEase(LeanTweenType.easeOutSine);
			LeanTween.rotate(avatars[i], avatars[i].transform.eulerAngles + new Vector3(45f, 0f, 0f), 1f).setLoopPingPong().setRepeat(2)
				.setEase(LeanTweenType.easeInQuad);
		}
		LeanTween.delayedCall(base.gameObject, 5f, presentationOver);
	}

	private void presentationOver()
	{
		LeanTween.move(gameOverButtonRect, new Vector2((float)Screen.width * 0.4f, gameOverButtonRect.y), 0.5f).setEase(gameOverCurve);
	}

	private void OnGUI()
	{
		if (GUI.Button(gameOverButtonRect.rect, "Play again?"))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
