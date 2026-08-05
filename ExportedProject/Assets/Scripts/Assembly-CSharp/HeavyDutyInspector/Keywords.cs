using UnityEngine;

namespace HeavyDutyInspector
{
	public class Keywords
	{
		private static KeywordsConfig _config;

		public static KeywordsConfig Config
		{
			get
			{
				if (_config == null)
				{
					_config = Resources.Load("Config/KeywordsConfig") as KeywordsConfig;
				}
				return _config;
			}
		}
	}
}
