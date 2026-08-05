using HeavyDutyInspector;
using UnityEngine;

public class LocalizationKeys
{
	private static KeywordsConfig _config;

	public static KeywordsConfig Config
	{
		get
		{
			if (_config == null)
			{
				_config = Resources.Load("Config/LocalizationKeysConfig") as KeywordsConfig;
			}
			return _config;
		}
	}
}
