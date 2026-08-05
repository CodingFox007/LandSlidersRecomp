using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Utils
{
	private static StringBuilder[] m_parsedTokens;

	private static int[] m_parsedTokensHashed;

	private static int m_parsedTokenCount;

	public static T CreateFromPrefab<T>(T prefab, string name = "") where T : MonoBehaviour
	{
		if (prefab == null)
		{
			Debug.LogError(typeof(T).ToString() + " Prefab Missing");
			return (T)null;
		}
		T result = UnityEngine.Object.Instantiate(prefab);
		if (name != string.Empty)
		{
			result.name = name;
		}
		else
		{
			result.name = typeof(T).ToString();
		}
		return result;
	}

	public static GameObject CreateFromPrefab(GameObject prefab, string name = "")
	{
		if (prefab == null)
		{
			Debug.LogError(name + " Prefab Missing");
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab);
		if (name != string.Empty)
		{
			gameObject.name = name;
		}
		return gameObject;
	}

	public static void InitDictionary<T, U>(List<T> keys, List<U> values, out Dictionary<T, U> dictionary, string dictionaryName)
	{
		dictionary = null;
		try
		{
			InitDictionary(keys, values, out dictionary);
		}
		catch (Exception ex)
		{
			Debug.LogError("Invalid " + dictionaryName + " (" + ex.Message + ")");
		}
	}

	public static void InitDictionary<T, U>(List<T> keys, List<U> values, out Dictionary<T, U> dictionary)
	{
		dictionary = new Dictionary<T, U>();
		int num = Mathf.Min(keys.Count, values.Count);
		for (int i = 0; i < num; i++)
		{
			dictionary.Add(keys[i], values[i]);
		}
	}

	public static void ShuffleArray<T>(T[] array)
	{
		int num = array.Length;
		while (num > 1)
		{
			int num2 = UnityEngine.Random.Range(0, num--);
			T val = array[num];
			array[num] = array[num2];
			array[num2] = val;
		}
	}

	public static void ShuffleList<T>(List<T> list, int seed)
	{
		System.Random random = new System.Random(seed);
		int count = list.Count;
		while (count > 1)
		{
			int index = random.Next(0, count--);
			T value = list[count];
			list[count] = list[index];
			list[index] = value;
		}
	}

	public static string ParseToken(int idx)
	{
		return m_parsedTokens[idx].ToString();
	}

	public static int ParseTokenHash(int idx)
	{
		return m_parsedTokensHashed[idx];
	}

	public static int ParseTokenCount()
	{
		return m_parsedTokenCount;
	}

	public static object Parse(string str)
	{
		object result = str;
		if (m_parsedTokens == null)
		{
			m_parsedTokens = new StringBuilder[8];
			m_parsedTokensHashed = new int[8];
			for (int i = 0; i < 8; i++)
			{
				m_parsedTokens[i] = new StringBuilder(64);
				m_parsedTokensHashed[i] = 0;
			}
		}
		m_parsedTokenCount = 0;
		int num = 0;
		int num2 = 0;
		char[] array = str.ToCharArray();
		for (int j = 0; j <= array.Length; j++)
		{
			if (j == array.Length || array[j] == '.')
			{
				m_parsedTokens[num].Remove(0, m_parsedTokens[num].Length);
				for (int k = num2; k < j; k++)
				{
					m_parsedTokens[num].Append(array[k]);
				}
				m_parsedTokensHashed[num] = m_parsedTokens[num].ToString().GetHashCode();
				num++;
				m_parsedTokenCount++;
				num2 = j + 1;
			}
		}
		if (ParseTokenCount() < 2)
		{
			return result;
		}
		switch (ParseToken(0))
		{
		case "Stats":
			result = ((!(Singleton<Game>.Instance.StatsManager == null)) ? Singleton<Game>.Instance.StatsManager.Parse() : null);
			break;
		case "StatsTimer":
			result = ((!(Singleton<Game>.Instance.StatsManager == null)) ? Singleton<Game>.Instance.StatsManager.Parse() : null);
			break;
		case "Mission":
			result = ((!(Singleton<Game>.Instance.MissionManager == null)) ? Singleton<Game>.Instance.MissionManager.Parse() : null);
			break;
		case "Character":
			result = ((Singleton<Game>.Instance.Player.CharacterInfo != null) ? Singleton<Game>.Instance.Player.CharacterInfo.Parse() : null);
			break;
		case "LastPlayedCharacter":
			result = ((Singleton<Game>.Instance.Player.LastPlayedCharacter != null) ? Singleton<Game>.Instance.Player.LastPlayedCharacter.Parse() : null);
			break;
		case "Player":
			result = ((!(Singleton<Game>.Instance.Player == null)) ? Singleton<Game>.Instance.Player.Parse() : null);
			break;
		case "Events":
			result = ((!(Singleton<Game>.Instance.EventManager == null)) ? Singleton<Game>.Instance.EventManager.Parse() : null);
			break;
		case "Levels":
			result = ((!(Singleton<Game>.Instance.LevelManager == null)) ? Singleton<Game>.Instance.LevelManager.Parse() : null);
			break;
		case "Sharing":
			result = ((!(Singleton<Game>.Instance.ShareManager == null)) ? Singleton<Game>.Instance.ShareManager.Parse() : null);
			break;
		case "Inventory":
			result = ((!(Singleton<Game>.Instance.InventoryManager == null)) ? Singleton<Game>.Instance.InventoryManager.Parse() : null);
			break;
		}
		return result;
	}

	public static object[] Parse(string[] strs)
	{
		object[] array = new object[strs.Length];
		for (int i = 0; i < strs.Length; i++)
		{
			array[i] = Parse(strs[i]);
		}
		return array;
	}

	public static IEnumerator ClearUnusedAssets()
	{
		AsyncOperation unload = Resources.UnloadUnusedAssets();
		while (!unload.isDone)
		{
			yield return null;
		}
	}
}
