using UnityEngine;

public class ShareManager : MonoBehaviour
{
	public delegate void ShareCallback(bool result);

	private ShareCallback m_callback;

	private Diffusion m_diffusion;

	private bool m_screenshotAvailable;

	public bool ScreenshotAvailable
	{
		get
		{
			return m_screenshotAvailable;
		}
		set
		{
			m_screenshotAvailable = value;
		}
	}

	private void Start()
	{
		m_diffusion = GetComponent<Diffusion>();
		m_diffusion.eventReceiver = base.gameObject;
	}

	private void ShareAndroid(string text, string imagePath, string url = "")
	{
		Debug.Log("Sharing!");
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.content.Intent");
		androidJavaObject.Call<AndroidJavaObject>("setAction", new object[1] { androidJavaClass.GetStatic<string>("ACTION_SEND") });
		if (url != string.Empty)
		{
			androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
			{
				androidJavaClass.GetStatic<string>("EXTRA_TEXT"),
				text + " " + url
			});
		}
		else
		{
			androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
			{
				androidJavaClass.GetStatic<string>("EXTRA_TEXT"),
				text
			});
		}
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.net.Uri");
		AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.io.File", imagePath);
		AndroidJavaObject androidJavaObject3 = androidJavaClass2.CallStatic<AndroidJavaObject>("fromFile", new object[1] { androidJavaObject2 });
		androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2]
		{
			androidJavaClass.GetStatic<string>("EXTRA_STREAM"),
			androidJavaObject3
		});
		androidJavaObject.Call<AndroidJavaObject>("setType", new object[1] { "image/*" });
		AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject4 = androidJavaClass3.GetStatic<AndroidJavaObject>("currentActivity");
		androidJavaObject4.Call("startActivity", androidJavaObject);
		OnCompleted(DiffusionPlatform.Custom);
	}

	public void Share(ShareCallback callback = null)
	{
		m_callback = callback;
		string text = "file://" + Application.persistentDataPath + "/Screenshot.png";
		Application.CaptureScreenshot(text);
		ShareAndroid("Make Way Share Text", text, string.Empty);
		Singleton<Game>.Instance.StatsManager.GetStat("SharedScore").IncreaseStat(1f);
	}

	public void Share(string text, string filePath, ShareCallback callback = null)
	{
		m_callback = callback;
		ShareAndroid(text, filePath, string.Empty);
		Singleton<Game>.Instance.StatsManager.GetStat("SharedScore").IncreaseStat(1f);
	}

	public void Share(string text, string url, string filePath, ShareCallback callback = null)
	{
		m_callback = callback;
		ShareAndroid(text, filePath, url);
		Singleton<Game>.Instance.StatsManager.GetStat("SharedScore").IncreaseStat(1f);
	}

	public void OnCompleted(DiffusionPlatform platform)
	{
		Debug.Log("Sharing Completed");
		if (m_callback != null)
		{
			m_callback(true);
		}
	}

	public void OnCancelled()
	{
		Debug.Log("Sharing cancelled!");
		if (m_callback != null)
		{
			m_callback(false);
		}
	}

	public object Parse()
	{
		object result = null;
		if (Utils.ParseTokenCount() < 2)
		{
			return result;
		}
		if (Utils.ParseTokenHash(1) == "ScreenshotAvailable".GetHashCode())
		{
			result = ScreenshotAvailable;
		}
		return result;
	}
}
