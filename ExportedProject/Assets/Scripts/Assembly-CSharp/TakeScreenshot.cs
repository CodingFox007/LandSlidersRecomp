using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			Application.CaptureScreenshot("/Users/michaelszewczyk/Desktop/itc" + Time.time + ".png", 1);
			MonoBehaviour.print("Screenshotted!");
		}
	}
}
