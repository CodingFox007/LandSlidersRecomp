using System;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
	public enum Priority
	{
		Default = 0,
		Level = 1,
		Character = 2
	}

	public class SkyBoxOptions
	{
		public float m_minRot;

		public float m_maxRot;

		public float m_minHeight;

		public float m_maxHeight;

		public Color m_colorize = Color.white;

		public Texture2D m_fogTexture;

		public Color m_fogColor;

		public float m_fogMinHeight;

		public float m_fogMaxHeight;
	}

	[SerializeField]
	private Priority m_priority;

	[SerializeField]
	[Range(-90f, 90f)]
	private float m_minRot;

	[SerializeField]
	[Range(-90f, 90f)]
	private float m_maxRot;

	[SerializeField]
	private float m_minHeight;

	[SerializeField]
	private float m_maxHeight;

	[SerializeField]
	private bool m_useDefaultFogSettings = true;

	[SerializeField]
	private Color m_fogColor = new Color(0.7058824f, 47f / 51f, 0.972549f, 1f);

	[SerializeField]
	private Color m_colorize = Color.white;

	[SerializeField]
	private float m_fogMinHeight = -80f;

	[SerializeField]
	private float m_fogMaxHeight = -5f;

	[SerializeField]
	private Texture2D m_fogTexture;

	[SerializeField]
	private float m_tweenTime;

	private float m_timer;

	private bool m_active;

	private static Dictionary<Priority, SkyBoxOptions> m_skyboxOptions = new Dictionary<Priority, SkyBoxOptions>();

	public void Awake()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.VisualCamera != null)
		{

            SetSkyBoxOptions(Priority.Default, Singleton<Game>.Instance.VisualCamera.DefaultMinRot, Singleton<Game>.Instance.VisualCamera.DefaultMaxRot, Singleton<Game>.Instance.VisualCamera.DefaultMinHeight, Singleton<Game>.Instance.VisualCamera.DefaultMaxHeight, Singleton<Game>.Instance.VisualCamera.DefaultFogColor, Singleton<Game>.Instance.VisualCamera.DefaultFogMinHeight, Singleton<Game>.Instance.VisualCamera.DefaultFogMaxHeight, Singleton<Game>.Instance.VisualCamera.DefaultSkyboxTexture, Color.white);
		}
	}

	public void Trigger()
	{
		m_active = true;
		m_timer = 0f;
	}

	public void Untrigger()
	{
		m_active = false;
		m_timer = 0f;
		RemoveSkyBoxOptions(m_priority);
	}

	private void Update()
	{
		if (m_active)
		{
			m_timer += Time.deltaTime;
			float num = 1f;
			if (m_tweenTime != 0f)
			{
				num = m_timer / m_tweenTime;
			}
			float minRot = Mathf.Lerp(Singleton<Game>.Instance.VisualCamera.DefaultMinRot, m_minRot, num);
			float maxRot = Mathf.Lerp(Singleton<Game>.Instance.VisualCamera.DefaultMaxRot, m_maxRot, num);
			float minHeight = Mathf.Lerp(Singleton<Game>.Instance.VisualCamera.DefaultMinHeight, m_minHeight, num);
			float maxHeight = Mathf.Lerp(Singleton<Game>.Instance.VisualCamera.DefaultMaxHeight, m_maxHeight, num);
			Color colorize = Color.Lerp(Color.white, m_colorize, num);
			num = ((!m_useDefaultFogSettings) ? num : 0f);
			Color fogColor = Color.Lerp(Singleton<Game>.Instance.VisualCamera.DefaultFogColor, m_fogColor, num);
			float fogMinHeight = Mathf.Lerp(Singleton<Game>.Instance.VisualCamera.DefaultFogMinHeight, m_fogMinHeight, num);
			float fogMaxHeight = Mathf.Lerp(Singleton<Game>.Instance.VisualCamera.DefaultFogMaxHeight, m_fogMaxHeight, num);
            
            SetSkyBoxOptions(m_priority, minRot, maxRot, minHeight, maxHeight, fogColor, fogMinHeight, fogMaxHeight, m_fogTexture, colorize);
			if (m_timer >= m_tweenTime)
			{
				m_timer = 0f;
				m_active = false;
			}
		}
	}

	private static void SetSkyBoxOptions(Priority priority, float minRot, float maxRot, float minHeight, float maxHeight, Color fogColor, float fogMinHeight, float fogMaxHeight, Texture2D fogTexture, Color colorize)
	{
       
        SkyBoxOptions skyBoxOptions = new SkyBoxOptions();
		skyBoxOptions.m_minRot = minRot;
		skyBoxOptions.m_maxRot = maxRot;
		skyBoxOptions.m_minHeight = minHeight;
		skyBoxOptions.m_maxHeight = maxHeight;
		skyBoxOptions.m_fogColor = fogColor;
		skyBoxOptions.m_fogMinHeight = fogMinHeight;
		skyBoxOptions.m_fogMaxHeight = fogMaxHeight;
		skyBoxOptions.m_fogTexture = fogTexture;
		skyBoxOptions.m_colorize = colorize;
		if (m_skyboxOptions.ContainsKey(priority))
		{
			m_skyboxOptions[priority] = skyBoxOptions;
		}
		else
		{
			m_skyboxOptions.Add(priority, skyBoxOptions);
		}
        Debug.Log(
               "SetSkyBoxOptions: " +
               priority +
               " Colorize=" +
               colorize
           );
    }

	private static void RemoveSkyBoxOptions(Priority priority)
	{
		m_skyboxOptions.Remove(priority);
	}

	private static bool IsHighestPriority(Priority priority)
	{
		Array values = Enum.GetValues(typeof(Priority));
		foreach (Priority item in values)
		{
			int num = (int)item;
			if (num > (int)priority && m_skyboxOptions.ContainsKey(item))
			{
				return false;
			}
		}
		return true;
	}

	public static void GetHighestPriority(out float minRot, out float maxRot, out float minHeight, out float maxHeight, out Color fogColor, out float fogMinHeight, out float fogMaxHeight, out Color colorize, out Texture2D fogTexture)
	{
		if (m_skyboxOptions == null || m_skyboxOptions.Count <= 0)
		{
			minRot = Singleton<Game>.Instance.VisualCamera.DefaultMinRot;
			maxRot = Singleton<Game>.Instance.VisualCamera.DefaultMaxRot;
			minHeight = Singleton<Game>.Instance.VisualCamera.DefaultMinHeight;
			maxHeight = Singleton<Game>.Instance.VisualCamera.DefaultMaxHeight;
			fogColor = Singleton<Game>.Instance.VisualCamera.DefaultFogColor;
			fogMinHeight = Singleton<Game>.Instance.VisualCamera.DefaultFogMinHeight;
			fogMaxHeight = Singleton<Game>.Instance.VisualCamera.DefaultFogMaxHeight;
			fogTexture = Singleton<Game>.Instance.VisualCamera.DefaultSkyboxTexture;
			colorize = Color.white;
			return;
		}
		Priority key = Priority.Default;
        if (m_skyboxOptions.ContainsKey(Priority.Character))
        {
            Debug.Log("Using CHARACTER skybox");
            key = Priority.Character;
        }
        else if (m_skyboxOptions.ContainsKey(Priority.Level))
        {
            Debug.Log("Using LEVEL skybox");
            key = Priority.Level;
        }
        SkyBoxOptions skyBoxOptions = m_skyboxOptions[key];
		minRot = skyBoxOptions.m_minRot;
		maxRot = skyBoxOptions.m_maxRot;
		minHeight = skyBoxOptions.m_minHeight;
		maxHeight = skyBoxOptions.m_maxHeight;
		fogColor = skyBoxOptions.m_fogColor;
		fogMinHeight = skyBoxOptions.m_fogMinHeight;
		fogMaxHeight = skyBoxOptions.m_fogMaxHeight;
        Debug.Log(
            "GetHighestPriority: key=" + key +
            " color=" + skyBoxOptions.m_colorize
        );
        colorize = skyBoxOptions.m_colorize;
		fogTexture = skyBoxOptions.m_fogTexture;
	}
}
