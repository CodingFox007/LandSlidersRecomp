using System.Collections.Generic;
using System.IO;
using ArabicSupport;
using HeavyDutyInspector;
using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class UISharingControl : MonoBehaviour, IEventSystemHandler
{
	[Multiline]
	[SerializeField]
	private string m_text;

	[SerializeField]
	private string[] m_textInputs;

	[Comment("Will fill the last {n} in m_text", CommentType.None, 0)]
	[SerializeField]
	private string[] m_randomTextInput;

	[SerializeField]
	private string m_url;

	[SerializeField]
	private RectTransform m_captureArea;

	[SerializeField]
	private UICameraRenderer m_cameraRenderer;

	[SerializeField]
	public UnityEvent m_onShareComplete = new UnityEvent();

	public void SetScreenshotAvailable()
	{
		Singleton<Game>.Instance.ShareManager.ScreenshotAvailable = true;
	}

	public void ShowShareOptions()
	{
		List<object> list = new List<object>();
		list.AddRange(Utils.Parse(m_textInputs));
		if (m_randomTextInput.Length > 0)
		{
			string term = m_randomTextInput[Random.Range(0, m_randomTextInput.Length)];
			term = ScriptLocalization.Get(term, true);
			list.Add(term);
		}
		TakePhoto();
		if (m_cameraRenderer != null)
		{
			m_cameraRenderer.Stop();
		}
		string filePath = Application.persistentDataPath + "/image.png";
		string empty = string.Empty;
		if (LocalizationManager.IsRight2Left)
		{
			LocalizationManager.IsRight2Left = false;
			empty = ArabicFixer.Fix(string.Format(ScriptLocalization.Get(m_text), list.ToArray()));
			LocalizationManager.IsRight2Left = true;
		}
		else
		{
			empty = string.Format(ScriptLocalization.Get(m_text, true), list.ToArray());
		}
		Singleton<Game>.Instance.ShareManager.Share(empty, Singleton<Game>.Instance.StoreURL, filePath, ShareComplete);
	}

	public void TakePhoto()
	{
		int pixelWidth = Singleton<Game>.Instance.UIController.UICamera.pixelWidth;
		int pixelHeight = Singleton<Game>.Instance.UIController.UICamera.pixelHeight;
		RenderTexture renderTexture = new RenderTexture(pixelWidth, pixelHeight, 24);
		Singleton<Game>.Instance.UIController.UICamera.targetTexture = renderTexture;
		Singleton<Game>.Instance.UIController.UICamera.Render();
		Singleton<Game>.Instance.UIController.UICamera.targetTexture = null;
		Vector3[] array = new Vector3[4];
		m_captureArea.GetWorldCorners(array);
		Vector2 lhs = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 lhs2 = new Vector2(float.MinValue, float.MinValue);
		for (int i = 0; i < 4; i++)
		{
			Vector3 position = array[i];
			Vector2 rhs = Singleton<Game>.Instance.UIController.UICamera.WorldToScreenPoint(position);
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
			{
				rhs.y = (float)pixelHeight - rhs.y;
			}
			lhs = Vector2.Min(lhs, rhs);
			lhs2 = Vector2.Max(lhs2, rhs);
		}
		lhs = Vector2.Max(lhs, Vector2.zero);
		lhs2 = Vector2.Min(lhs2, new Vector2(pixelWidth, pixelHeight));
		lhs += Vector2.one;
		lhs2 -= Vector2.one;
		int num = Mathf.Max(1, Mathf.FloorToInt(lhs2.x - lhs.x));
		int num2 = Mathf.Max(1, Mathf.FloorToInt(lhs2.y - lhs.y));
		string path = Application.persistentDataPath + "/image.png";
		RenderTexture.active = renderTexture;
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGB24, false);
		texture2D.ReadPixels(new Rect(lhs.x, lhs.y, num, num2), 0, 0, false);
		RenderTexture.active = null;
		byte[] bytes = texture2D.EncodeToPNG();
		File.WriteAllBytes(path, bytes);
		bytes = null;
		Object.Destroy(texture2D);
		texture2D = null;
		renderTexture.Release();
		Object.Destroy(renderTexture);
		renderTexture = null;
	}

	private void Update()
	{
	}

	private void ShareComplete(bool result)
	{
		m_onShareComplete.Invoke();
	}

	public void OnDisableScreen()
	{
		Singleton<Game>.Instance.UIController.HideSharing();
	}
}
