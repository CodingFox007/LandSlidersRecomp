using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;

public class ExampleDictionary : NamedMonoBehaviour, ISerializationCallbackReceiver
{
	[Dictionary("dictionaryExampleValues")]
	public List<string> dictionaryExample;

	[HideInInspector]
	public List<GameObject> dictionaryExampleValues;

	private Dictionary<string, GameObject> actualDictionary = new Dictionary<string, GameObject>();

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		InitDictionary(dictionaryExample, dictionaryExampleValues, actualDictionary);
	}
}
