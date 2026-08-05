using System;

namespace I2.Loc
{
	[Serializable]
	public class TermData
	{
		public string Term = string.Empty;

		public eTermType TermType;

		public string Description = string.Empty;

		public string[] Languages = new string[0];

		public string[] Languages_Touch = new string[0];

		public string GetTranslation(int idx)
		{
			if (IsTouchType())
			{
				return string.IsNullOrEmpty(Languages_Touch[idx]) ? Languages[idx] : Languages_Touch[idx];
			}
			return string.IsNullOrEmpty(Languages[idx]) ? Languages_Touch[idx] : Languages[idx];
		}

		public bool HasTouchTranslations()
		{
			int i = 0;
			for (int num = Languages_Touch.Length; i < num; i++)
			{
				if (!string.IsNullOrEmpty(Languages_Touch[i]) && !string.IsNullOrEmpty(Languages[i]) && Languages_Touch[i] != Languages[i])
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsTouchType()
		{
			return true;
		}
	}
}
