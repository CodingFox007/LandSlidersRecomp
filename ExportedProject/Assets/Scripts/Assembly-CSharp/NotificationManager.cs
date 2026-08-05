using System;
using UnityEngine;

public class NotificationManager : Singleton<NotificationManager>, Savable
{
	private bool m_enabled;

	public void EnableNotifications()
	{
		m_enabled = true;
	}

	private static void _ClearApplicationBadgeNumber()
	{
	}

	public void SetNotification(string body, string action, DateTime time, bool forDebug = false)
	{
		if (m_enabled || forDebug)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("com.prettygreat.notifications.Notifier");
			androidJavaObject2.CallStatic("sendNotification", androidJavaObject, "Land Sliders", body, body, (int)(time - DateTime.Now).TotalSeconds);
		}
	}

	private void OnApplicationPause()
	{
		if (Singleton<Game>.Instance.IsLoaded)
		{
			TimeSpan timeSpan = Singleton<Game>.Instance.EventManager.EventGroupAvailableIn("Prizes");
			if (!m_enabled || timeSpan > TimeSpan.Zero)
			{
				_ClearApplicationBadgeNumber();
			}
		}
	}

	public void ClearNotifications()
	{
	}

	private void ClearNotificationsIOS()
	{
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		jSONObject.AddField("enabled", m_enabled);
		return jSONObject;
	}

	public void Load(JSONObject data)
	{
		m_enabled = data.GetField("enabled").AsBool;
	}

	public void Merge(JSONObject dataA, JSONObject dataB)
	{
	}

	public JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB)
	{
		return dataA;
	}

	public bool Verify(JSONObject data)
	{
		return true;
	}

	public JSONObject Reset()
	{
		m_enabled = false;
		return Save();
	}

	public JSONObject UpdateVersion(JSONObject data)
	{
		JSONObject result = data;
		string version = GetVersion(data);
		if (!(version == string.Empty))
		{
			switch (version)
			{
			case "0.0.1":
			case "0.0.2":
			case "0.0.3":
				break;
			default:
				goto IL_0051;
			}
		}
		result = Reset();
		goto IL_0051;
		IL_0051:
		return result;
	}

	public string SaveId()
	{
		return "Notifications";
	}

	public string Version()
	{
		return "0.0.4";
	}

	public string GetVersion(JSONObject data)
	{
		if (!data.HasField("version"))
		{
			return string.Empty;
		}
		return data.GetField("version").AsString;
	}

	public bool UseCloud()
	{
		return false;
	}
}
