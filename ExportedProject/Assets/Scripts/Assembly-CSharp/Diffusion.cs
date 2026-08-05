using UnityEngine;

public class Diffusion : MonoBehaviour
{
	public delegate void OnCompleted(DiffusionPlatform platform);

	public delegate void OnCancelled();

	public string message;

	public string url;

	public int platformsToHide;

	public string[] customPlatforms;

	public GameObject eventReceiver;

	public OnCompleted onCompleted;

	public OnCancelled onCancelled;

	private void Start()
	{
		Prewarm();
		string[] array = customPlatforms;
		foreach (string platformClass in array)
		{
			AddCustomPlatform(platformClass);
		}
	}

	public static bool isFacebookConnected()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
		}
		return false;
	}

	public static bool isTwitterConnected()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
		}
		return false;
	}

	public void PostToTwitter()
	{
		PostToTwitter(message, url, null);
	}

	public void PostToTwitter(string message)
	{
		PostToTwitter(message, null, null);
	}

	public void PostToTwitter(string message, string imagePath)
	{
		PostToTwitter(message, null, imagePath);
	}

	public void PostToTwitter(string message, string url, string imagePath)
	{
		if (string.IsNullOrEmpty(message))
		{
			message = string.Empty;
		}
		if (string.IsNullOrEmpty(url))
		{
			url = string.Empty;
		}
		if (string.IsNullOrEmpty(imagePath))
		{
			imagePath = string.Empty;
		}
		Debug.Log("[Diffusion] Sharing message: " + message + " | " + url + " | " + imagePath);
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
		}
	}

	public void PostToFacebook()
	{
		PostToFacebook(message, url, null);
	}

	public void PostToFacebook(string message)
	{
		PostToFacebook(message, null, null);
	}

	public void PostToFacebook(string message, string imagePath)
	{
		PostToFacebook(message, null, imagePath);
	}

	public void PostToFacebook(string message, string url, string imagePath)
	{
		if (string.IsNullOrEmpty(message))
		{
			message = string.Empty;
		}
		if (string.IsNullOrEmpty(url))
		{
			url = string.Empty;
		}
		if (string.IsNullOrEmpty(imagePath))
		{
			imagePath = string.Empty;
		}
		Debug.Log("[Diffusion] Sharing message: " + message + " | " + url + " | " + imagePath);
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
		}
	}

	public void Share()
	{
		Share(message, url, null, platformsToHide);
	}

	public void Share(string message, string filePath)
	{
		Share(message, null, filePath, platformsToHide);
	}

	public void Share(string message, string url, string filePath)
	{
		Share(message, url, filePath, platformsToHide);
	}

	public void Share(string message, string url, string filePath, int platforms)
	{
		if (message == null)
		{
			message = string.Empty;
		}
		url = ((!string.IsNullOrEmpty(url)) ? AddPrefix("http://", url) : string.Empty);
		filePath = ((!string.IsNullOrEmpty(filePath)) ? AddPrefix("file://", filePath) : string.Empty);
		Debug.Log("[Diffusion] Sharing message: " + message + " | " + url + " | " + filePath);
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
		}
	}

	private void Completed(string platformAsInt)
	{
		DiffusionPlatform diffusionPlatform = (DiffusionPlatform)int.Parse(platformAsInt);
		Debug.Log("Using correct Completed code!");
		if (eventReceiver != null)
		{
			eventReceiver.SendMessage("OnCompleted", diffusionPlatform, SendMessageOptions.DontRequireReceiver);
		}
		if (onCompleted != null)
		{
			onCompleted(diffusionPlatform);
		}
	}

	private void Cancelled()
	{
		Debug.Log("Using correct Cancelled code!");
		if (eventReceiver != null)
		{
			eventReceiver.SendMessage("OnCancelled", SendMessageOptions.DontRequireReceiver);
		}
		if (onCancelled != null)
		{
			onCancelled();
		}
	}

	private static void Prewarm()
	{
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
		}
	}

	private static void AddCustomPlatform(string platformClass)
	{
		if (!string.IsNullOrEmpty(platformClass) && Application.platform != RuntimePlatform.IPhonePlayer)
		{
		}
	}

	private string AddPrefix(string prefix, string url)
	{
		if (url.Substring(0, prefix.Length) != prefix)
		{
			url = prefix + url;
		}
		return url;
	}
}
