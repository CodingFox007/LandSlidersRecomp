using System;
using System.Collections.Generic;

namespace HeavyDutyInspector
{
	[Serializable]
	public class KeywordCategory
	{
		public string name;

		[NonSerialized]
		public bool expanded;

		public List<string> keywords = new List<string>();

		public KeywordCategory()
		{
			name = string.Empty;
		}

		public KeywordCategory(string name)
		{
			this.name = name;
		}
	}
}
