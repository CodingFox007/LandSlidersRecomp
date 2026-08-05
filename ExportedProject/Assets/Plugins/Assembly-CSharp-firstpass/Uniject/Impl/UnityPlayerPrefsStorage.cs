using UnityEngine;

namespace Uniject.Impl
{
	public class UnityPlayerPrefsStorage : IStorage
	{
		public int GetInt(string key, int defaultValue)
		{
			return PlayerPrefs.GetInt(key, defaultValue);
		}

		public void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
		}

		public string GetString(string key, string defaultValue)
		{
			return PlayerPrefs.GetString(key, defaultValue);
		}

		public void SetString(string key, string val)
		{
			PlayerPrefs.SetString(key, val);
		}
	}
}
