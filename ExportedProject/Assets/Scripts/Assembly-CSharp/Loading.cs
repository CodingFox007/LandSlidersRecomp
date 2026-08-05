using System.Collections;
using UnityEngine;

public class Loading : MonoBehaviour
{
	[SerializeField]
	private Camera m_camera;

	[SerializeField]
	private Renderer m_ring;

	[SerializeField]
	private Renderer m_background;

	[SerializeField]
	private Renderer m_createdBy;

	[SerializeField]
	private Renderer m_logo;

	private float m_rot;

	private int m_rotFrame;

	[SerializeField]
	private float m_fadeTime = 1f;

	private float m_fadeTimer;

	private static bool m_loadStarted;

	private static bool m_loadComplete;

	private Vector3 m_originalLogoScale;

	private void Start()
	{
		bool flag;
		do
		{
			flag = true;
			Object[] array = Object.FindObjectsOfType(typeof(GameObject));
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = (GameObject)array[i];
				if (gameObject.transform.root == gameObject.transform && gameObject.name != "LoadingCamera")
				{
					Object.DestroyImmediate(gameObject);
					flag = false;
					break;
				}
			}
		}
		while (!flag);
		m_originalLogoScale = m_logo.transform.localScale;
		SetImages();
		StartCoroutine(Load());
	}

	private void SetImages()
	{
		Vector3 vector = m_camera.ScreenToWorldPoint(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		Vector3 vector2 = m_camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
		Vector3 localPosition = vector;
		localPosition.z = 0f;
		m_background.transform.localPosition = localPosition;
		Vector3 localScale = (vector2 - vector) * 2f;
		localScale.x = Mathf.Max(localScale.x, localScale.y);
		localScale.y = Mathf.Max(localScale.x, localScale.y);
		m_background.transform.localScale = localScale;
		localPosition = m_ring.transform.localPosition;
		localPosition.y = vector.y - vector2.y * 0.75f;
		m_ring.transform.localPosition = localPosition;
		localPosition = m_createdBy.transform.localPosition;
		localPosition.x = vector2.x;
		localPosition.y = 0f - vector2.y;
		m_createdBy.transform.localPosition = localPosition;
		if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.width > Screen.height)
		{
			m_logo.transform.localScale = m_originalLogoScale * 1.25f;
		}
	}

	private void Update()
	{
		SetImages();
		m_rotFrame++;
		if (m_rotFrame > 2)
		{
			m_rotFrame = 0;
			m_rot -= 30f;
		}
		m_ring.transform.localRotation = Quaternion.Euler(0f, 0f, m_rot);
		if (m_loadComplete)
		{
			m_fadeTimer += Time.unscaledDeltaTime;
			float a = Mathf.Lerp(1f, 0f, m_fadeTimer / m_fadeTime);
			Color color = m_ring.material.color;
			color.a = a;
			m_ring.material.color = color;
			color = m_background.material.color;
			color.a = a;
			m_background.material.color = color;
			if (m_fadeTimer >= m_fadeTime)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private IEnumerator Load()
	{
		if (!m_loadStarted)
		{
			m_loadStarted = true;
			yield return null;
			while (!Application.CanStreamedLevelBeLoaded("Main"))
			{
				yield return null;
			}
			AsyncOperation loading = Application.LoadLevelAdditiveAsync("Main");
			while (!loading.isDone)
			{
				yield return null;
			}
			Time.timeScale = 0f;
			yield return StartCoroutine(Singleton<Game>.Instance.LoadScene());
			Time.timeScale = 1f;
			m_loadComplete = true;
		}
	}
}
