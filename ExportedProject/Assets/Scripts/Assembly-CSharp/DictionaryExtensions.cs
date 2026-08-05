using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DictionaryExtensions
{
	public static Dictionary<TKey, TValue> Shuffle<TKey, TValue>(this Dictionary<TKey, TValue> source)
	{
		return source.OrderBy((KeyValuePair<TKey, TValue> x) => Random.Range(0, source.Count)).ToDictionary((KeyValuePair<TKey, TValue> item) => item.Key, (KeyValuePair<TKey, TValue> item) => item.Value);
	}
}
