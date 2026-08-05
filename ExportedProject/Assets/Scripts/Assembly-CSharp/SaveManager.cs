using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class SaveManager
{
	public delegate void CloudChanged();

	private static Dictionary<Type, SaveType> m_saveTypes = new Dictionary<Type, SaveType>();

	public static List<Savable> m_savables = new List<Savable>();

	private static bool m_icloudChanged = false;

	private static bool m_googleSaveChanged = false;

	private static Dictionary<string, string> m_googleData = new Dictionary<string, string>();

	public static event CloudChanged OnCloudReceived;

	public static event CloudChanged OnCloudChanged;

	[DllImport("__Internal")]
	private static extern void iCloudKV_Synchronize();

	[DllImport("__Internal")]
	private static extern void iCloudKV_SetInt(string key, int value);

	[DllImport("__Internal")]
	private static extern void iCloudKV_SetFloat(string key, float value);

	[DllImport("__Internal")]
	private static extern void iCloudKV_SetString(string key, string value);

	[DllImport("__Internal")]
	private static extern int iCloudKV_GetInt(string key);

	[DllImport("__Internal")]
	private static extern float iCloudKV_GetFloat(string key);

	[DllImport("__Internal")]
	private static extern string iCloudKV_GetString(string key);

	[DllImport("__Internal")]
	private static extern void iCloudKV_Reset();

	public static void InitCloud()
	{
		m_icloudChanged = false;
		JCloudDataDidChangeExternally();
	}

	public static void RegisterSavable(Savable savable)
	{
		m_savables.Add(savable);
	}

	public static void CommitValue(bool test)
	{
		Debug.Log("Storage success: " + test);
	}

	public static void RegisterGoogleData(string ID, string data)
	{
		lock (m_googleData)
		{
			m_googleSaveChanged = true;
			if (m_googleData.ContainsKey(ID))
			{
				m_googleData[ID] = data;
			}
			else
			{
				m_googleData.Add(ID, data);
			}
		}
	}

	public static void CheckGoogleCloud()
	{
		CloudAndroid.LoadFromCloud();
		if (!m_googleSaveChanged)
		{
			return;
		}
		lock (m_googleData)
		{
			foreach (KeyValuePair<string, string> googleDatum in m_googleData)
			{
				Savable savable = null;
				foreach (Savable savable2 in m_savables)
				{
					if (savable2.SaveId() == googleDatum.Key)
					{
						savable = savable2;
						break;
					}
				}
				string value = googleDatum.Value;
				JSONObject dataB = new JSONObject(value);
				JSONObject dataA = savable.Save();
				savable.Merge(dataA, dataB);
				JSONObject json = savable.Save();
				SaveJSON(savable.SaveId(), json);
			}
		}
		m_googleSaveChanged = false;
	}

	public static void Save()
	{
		JSONObject jSONObject = new JSONObject();
		foreach (Savable savable in m_savables)
		{
			JSONObject jSONObject2 = savable.Save();
			SaveJSON(savable.SaveId(), jSONObject2);
			if (savable.UseCloud())
			{
				jSONObject.AddField(savable.SaveId(), jSONObject2);
			}
		}
		CloudAndroid.CommitSaveToCloud("Game", Encoding.UTF8.GetBytes(jSONObject.ToString()), CommitValue);
	}

	public static void Save(Savable savable)
	{
		JSONObject json = savable.Save();
		SaveJSON(savable.SaveId(), json);
	}

	public static void SaveJSON(string id, JSONObject json)
	{
		if (PlayerPrefs.HasKey(id))
		{
			PlayerPrefs.SetString(id + ".previous", PlayerPrefs.GetString(id));
		}
		PlayerPrefs.SetString(id, json.ToString());
	}

	public static IEnumerator Load()
	{
		bool versionChanged = false;
		foreach (Savable savable in m_savables)
		{
			if (!PlayerPrefs.HasKey(savable.SaveId()))
			{
				JSONObject jSONObject = savable.Reset();
				if (jSONObject.ToString() != "null")
				{
					SaveJSON(savable.SaveId(), jSONObject);
				}
			}
			else
			{
				JSONObject data = LoadJSON(savable.SaveId());
				if (savable.GetVersion(data) != savable.Version())
				{
					data = savable.UpdateVersion(data);
					versionChanged = true;
				}
				if (!savable.Verify(data) && PlayerPrefs.HasKey(savable.SaveId() + ".previous"))
				{
					savable.Load(LoadJSON(savable.SaveId() + ".previous"));
				}
				else
				{
					savable.Load(data);
				}
			}
			yield return null;
		}
		if (!versionChanged)
		{
		}
	}

	public static byte[] MergeAndroid(string id, byte[] oldData, byte[] newData)
	{
		foreach (Savable savable in m_savables)
		{
			if (savable.SaveId() == id)
			{
				string text = Encoding.UTF8.GetString(oldData);
				string text2 = Encoding.UTF8.GetString(newData);
				Debug.Log("Merging " + id);
				Debug.Log("Old: " + text);
				Debug.Log("New: " + text2);
				JSONObject dataA = new JSONObject(text);
				Debug.Log("Created 0?");
				JSONObject dataB = new JSONObject(text2);
				Debug.Log("Created?");
				JSONObject jSONObject = savable.MergeToJSON(dataA, dataB);
				string s = jSONObject.ToString();
				Debug.Log("Merged: " + jSONObject);
				return Encoding.UTF8.GetBytes(s);
			}
		}
		return oldData;
	}

	public static JSONObject LoadJSON(string id)
	{
		if (PlayerPrefs.HasKey(id))
		{
			return new JSONObject(PlayerPrefs.GetString(id));
		}
		return null;
	}

	public static void RegisterSaveType<T>(SaveType saveType)
	{
		if (!m_saveTypes.ContainsKey(typeof(T)))
		{
			m_saveTypes.Add(typeof(T), saveType);
		}
	}

	public static Type GetType(string name)
	{
		if (PlayerPrefs.GetInt(name, 0) != 0 || PlayerPrefs.GetInt(name, 1) != 1)
		{
			return typeof(int);
		}
		if (PlayerPrefs.GetFloat(name, 0.5f) != 0.5f || PlayerPrefs.GetFloat(name, 1.5f) != 1.5f)
		{
			return typeof(float);
		}
		string valueStr = PlayerPrefs.GetString(name);
		Type typeByValue = GetTypeByValue(valueStr);
		if (typeByValue != null)
		{
			return typeByValue;
		}
		return typeof(string);
	}

	public static Type GetTypeByValue(string valueStr)
	{
		foreach (SaveType value in m_saveTypes.Values)
		{
			if (value.IsType(valueStr))
			{
				return value.GetValueType();
			}
		}
		return typeof(string);
	}

	public static bool HasField(string name)
	{
		return PlayerPrefs.HasKey(name);
	}

	public static void AddField<T>(string name, T value)
	{
		if (m_saveTypes.ContainsKey(typeof(T)))
		{
			string value2 = m_saveTypes[typeof(T)].ToString(value);
			PlayerPrefs.SetString(name, value2);
		}
	}

	public static string ToString<T>(T value)
	{
		if (m_saveTypes.ContainsKey(typeof(T)))
		{
			return m_saveTypes[typeof(T)].ToString(value);
		}
		return null;
	}

	public static T GetField<T>(string name)
	{
		if (m_saveTypes.ContainsKey(typeof(T)))
		{
			string valueStr = PlayerPrefs.GetString(name);
			return (T)m_saveTypes[typeof(T)].FromString(valueStr);
		}
		return default(T);
	}

	public static T FromString<T>(string value)
	{
		if (m_saveTypes.ContainsKey(typeof(T)))
		{
			return (T)m_saveTypes[typeof(T)].FromString(value);
		}
		return default(T);
	}

	public static SaveType GetSaveType(string value)
	{
		Type typeByValue = GetTypeByValue(value);
		if (typeByValue != null && m_saveTypes.ContainsKey(typeByValue))
		{
			return m_saveTypes[typeByValue];
		}
		return null;
	}

	public static bool CheckICloud()
	{
		return false;
	}

	public static void JCloudDataDidChangeExternally()
	{
		m_icloudChanged = true;
	}
}
