using System;
using System.Linq;
using HeavyDutyInspector;

[Serializable]
public class LocalizationKey : Keyword
{
	public LocalizationKey()
	{
	}

	private LocalizationKey(string key)
		: base(key)
	{
	}

	public static implicit operator string(LocalizationKey word)
	{
		return word._key.Split('/').Last();
	}

	public static implicit operator LocalizationKey(string key)
	{
		return new LocalizationKey(key);
	}
}
