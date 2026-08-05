using UnityEngine;

public class TestingAlphaFade : MonoBehaviour
{
	private GameObject sphere1;

	private void Start()
	{
		LeanTween.alpha(base.gameObject, 0f, 1f).setLoopPingPong();
		sphere1 = GameObject.Find("Sphere1");
		LeanTween.alpha(sphere1, 0f, 1f).setLoopPingPong();
	}
}
