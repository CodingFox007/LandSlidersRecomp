using System;
using System.Collections.Generic;
using System.Linq;
using HeavyDutyInspector;
using UnityEngine;

[Serializable]
public abstract class NamedMonoBehaviour : MonoBehaviour
{
	[NMBName]
	public string scriptName;

	[NMBColor]
	public Color scriptNameColor = Color.white;

	protected void InitDictionary<T, U>(List<T> keys, List<U> values, Dictionary<T, U> dictionary)
	{
		dictionary = new Dictionary<T, U>();
		for (int i = 0; i < keys.Count; i++)
		{
			dictionary.Add(keys[i], values[i]);
		}
	}

	protected void PrepareDictionary<T, U>(Dictionary<T, U> dictionary, List<T> keys, List<U> values)
	{
		keys = new List<T>();
		values = new List<U>();
		foreach (KeyValuePair<T, U> item in dictionary.ToList())
		{
			keys.Add(item.Key);
			values.Add(item.Value);
		}
	}
}
