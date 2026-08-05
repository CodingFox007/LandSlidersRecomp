using UnityEngine;

namespace HeavyDutyInspector
{
	public class DictionaryAttribute : PropertyAttribute
	{
		public string valuesListName { get; private set; }

		public KeywordsConfig keywordConfig { get; private set; }

		public DictionaryAttribute(string valuesListName)
		{
			keywordConfig = null;
			this.valuesListName = valuesListName;
		}

		public DictionaryAttribute(string valuesListName, string keywordsConfigFile)
		{
			keywordConfig = Resources.Load(keywordsConfigFile) as KeywordsConfig;
			this.valuesListName = valuesListName;
		}
	}
}
