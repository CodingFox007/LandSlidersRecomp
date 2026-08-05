using System;
using System.Collections.Generic;
using UnityEngine;

namespace I2.Loc
{
	public class ResourceManager : MonoBehaviour
	{
		private static ResourceManager mInstance;

		public UnityEngine.Object[] Assets;

		private Dictionary<string, UnityEngine.Object> mResourcesCache = new Dictionary<string, UnityEngine.Object>();

		private bool mCleaningScheduled;

		public static ResourceManager pInstance
		{
			get
			{
				if (mInstance == null)
				{
					mInstance = (ResourceManager)UnityEngine.Object.FindObjectOfType(typeof(ResourceManager));
				}
				if (mInstance == null)
				{
					GameObject gameObject = new GameObject("I2ResourceManager", typeof(ResourceManager));
					gameObject.hideFlags |= HideFlags.HideAndDontSave;
					mInstance = gameObject.GetComponent<ResourceManager>();
				}
				if (Application.isPlaying)
				{
					UnityEngine.Object.DontDestroyOnLoad(mInstance.gameObject);
				}
				return mInstance;
			}
		}

		public void OnLevelWasLoaded()
		{
			LocalizationManager.UpdateSources();
		}

		public T GetAsset<T>(string Name) where T : UnityEngine.Object
		{
			T val = FindAsset(Name) as T;
			if (val != null)
			{
				return val;
			}
			return LoadFromResources<T>(Name);
		}

		private UnityEngine.Object FindAsset(string Name)
		{
			if (Assets != null)
			{
				int i = 0;
				for (int num = Assets.Length; i < num; i++)
				{
					if (Assets[i] != null && Assets[i].name == Name)
					{
						return Assets[i];
					}
				}
			}
			return null;
		}

		public bool HasAsset(UnityEngine.Object Obj)
		{
			if (Assets == null)
			{
				return false;
			}
			return Array.IndexOf(Assets, Obj) >= 0;
		}

		public T LoadFromResources<T>(string Path) where T : UnityEngine.Object
		{
			UnityEngine.Object value;
			if (mResourcesCache.TryGetValue(Path, out value) && value != null)
			{
				return value as T;
			}
			T val = (T)null;
			if (Path.EndsWith("]"))
			{
				int num = Path.LastIndexOf("[");
				int length = Path.Length - num - 2;
				string value2 = Path.Substring(num + 1, length);
				Path = Path.Substring(0, num);
				T[] array = Resources.LoadAll<T>(Path);
				int i = 0;
				for (int num2 = array.Length; i < num2; i++)
				{
					if (array[i].name.Equals(value2))
					{
						val = array[i];
						break;
					}
				}
			}
			else
			{
				val = Resources.Load<T>(Path);
			}
			mResourcesCache[Path] = val;
			if (!mCleaningScheduled)
			{
				Invoke("CleanResourceCache", 0.1f);
				mCleaningScheduled = true;
			}
			return val;
		}

		public void CleanResourceCache()
		{
			mResourcesCache.Clear();
			Resources.UnloadUnusedAssets();
			CancelInvoke();
			mCleaningScheduled = false;
		}
	}
}
