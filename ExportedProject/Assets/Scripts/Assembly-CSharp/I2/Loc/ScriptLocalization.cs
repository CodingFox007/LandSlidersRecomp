namespace I2.Loc
{
	public static class ScriptLocalization
	{
		public static string Get(string Term, bool FixForRTL = false)
		{
			return LocalizationManager.GetTermTranslation(Term, FixForRTL);
		}
	}
}
