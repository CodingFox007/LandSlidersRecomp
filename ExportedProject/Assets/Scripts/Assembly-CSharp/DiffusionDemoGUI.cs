using UnityEngine;

public class DiffusionDemoGUI : MonoBehaviour
{
	public Diffusion diffusion;

	private void Start()
	{
		diffusion.eventReceiver = base.gameObject;
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(20f, 20f, 100f, 50f), "Share"))
		{
			diffusion.Share();
		}
		if (GUI.Button(new Rect(20f, 100f, 100f, 50f), "Share pic"))
		{
			diffusion.Share("Check out my photo!", null, "file://" + Application.streamingAssetsPath + "/diffusionDemo.png");
		}
		if (GUI.Button(new Rect(20f, 200f, 100f, 50f), "Post Twitter"))
		{
			diffusion.PostToTwitter("Check out my photo!", null, "file://" + Application.streamingAssetsPath + "/diffusionDemo.png");
		}
		if (GUI.Button(new Rect(20f, 300f, 100f, 50f), "Post Facebook"))
		{
			diffusion.PostToFacebook("Check out my photo!", null, "file://" + Application.streamingAssetsPath + "/diffusionDemo.png");
		}
		if (GUI.Button(new Rect(20f, 400f, 100f, 50f), "Check Logins"))
		{
			Debug.Log("Facebook: " + Diffusion.isFacebookConnected());
			Debug.Log("Twitter: " + Diffusion.isTwitterConnected());
		}
	}

	private void OnCompleted(DiffusionPlatform platform)
	{
		Debug.Log("Thanks for sharing on " + platform);
		switch (platform)
		{
		case DiffusionPlatform.SaveToCameraRoll:
			Camera.main.backgroundColor = Color.magenta;
			break;
		case DiffusionPlatform.Twitter:
			Camera.main.backgroundColor = Color.cyan;
			break;
		default:
			Camera.main.backgroundColor = Color.green;
			break;
		}
		Invoke("ResetColor", 2f);
	}

	private void OnCancelled()
	{
		Debug.Log("User cancelled!");
		Camera.main.backgroundColor = Color.red;
		Invoke("ResetColor", 2f);
	}

	private void ResetColor()
	{
		Camera.main.backgroundColor = Color.blue;
	}
}
