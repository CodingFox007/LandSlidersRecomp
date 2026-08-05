using System;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;

namespace I2.Loc
{
	public static class LocalizationManager
	{
		public delegate void OnLocalizeCallback();

		private static string mCurrentLanguage;

		private static string mLanguageCode;

		public static bool IsRight2Left = false;

		public static List<LanguageSource> Sources = new List<LanguageSource>();

		public static string[] GlobalSources = new string[1] { "I2Languages" };

		private static string[] LanguagesRTL = new string[20]
		{
			"ar-DZ", "ar", "ar-BH", "ar-EG", "ar-IQ", "ar-JO", "ar-KW", "ar-LB", "ar-LY", "ar-MA",
			"ar-OM", "ar-QA", "ar-SA", "ar-SY", "ar-TN", "ar-AE", "ar-YE", "he", "ur", "ji"
		};

		public static string CurrentLanguage
		{
			get
			{
				InitializeIfNeeded();
				return mCurrentLanguage;
			}
			set
			{
				string supportedLanguage = GetSupportedLanguage(value);
				if (!string.IsNullOrEmpty(supportedLanguage) && mCurrentLanguage != supportedLanguage)
				{
					SetLanguageAndCode(supportedLanguage, GetLanguageCode(supportedLanguage));
				}
			}
		}

		public static string CurrentLanguageCode
		{
			get
			{
				InitializeIfNeeded();
				return mLanguageCode;
			}
			set
			{
				if (mLanguageCode != value)
				{
					string languageFromCode = GetLanguageFromCode(value);
					if (!string.IsNullOrEmpty(languageFromCode))
					{
						SetLanguageAndCode(languageFromCode, value);
					}
				}
			}
		}

		public static event OnLocalizeCallback OnLocalizeEvent;

		private static void InitializeIfNeeded()
		{
			if (string.IsNullOrEmpty(mCurrentLanguage))
			{
				UpdateSources();
				SelectStartupLanguage();
			}
		}

		private static void SetLanguageAndCode(string LanguageName, string LanguageCode, bool RememberLanguage = true)
		{
			if (mCurrentLanguage != LanguageName || mLanguageCode != LanguageCode)
			{
				if (RememberLanguage)
				{
					PlayerPrefs.SetString("I2 Language", LanguageName);
				}
				mCurrentLanguage = LanguageName;
				mLanguageCode = LanguageCode;
				IsRight2Left = IsRTL(mLanguageCode);
				LocalizeAll();
			}
		}

		private static void SelectStartupLanguage()
		{
			string text = PlayerPrefs.GetString("I2 Language", string.Empty);
			string text2 = Application.systemLanguage.ToString();
			if (HasLanguage(text, true, false))
			{
				CurrentLanguage = text;
				return;
			}
			if (text2 == "ChineseSimplified")
			{
				text2 = "Chinese (Simplified)";
			}
			if (text2 == "ChineseTraditional")
			{
				text2 = "Chinese (Traditional)";
			}
			string supportedLanguage = GetSupportedLanguage(text2);
			if (!string.IsNullOrEmpty(supportedLanguage))
			{
				SetLanguageAndCode(supportedLanguage, GetLanguageCode(supportedLanguage), false);
				return;
			}
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				if (Sources[i].mLanguages.Count > 0)
				{
					SetLanguageAndCode(Sources[i].mLanguages[0].Name, Sources[i].mLanguages[0].Code, false);
					break;
				}
			}
		}

		public static string GetTermTranslation(string Term, bool FixForRTL = false)
		{
			string Translation;
			if (TryGetTermTranslation(Term, out Translation, FixForRTL))
			{
				return Translation;
			}
			return string.Empty;
		}

		public static bool TryGetTermTranslation(string Term, out string Translation, bool FixForRTL = false)
		{
			Translation = string.Empty;
			if (string.IsNullOrEmpty(Term))
			{
				return false;
			}
			InitializeIfNeeded();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				if (Sources[i].TryGetTermTranslation(Term, out Translation))
				{
					if (IsRight2Left && FixForRTL)
					{
						Translation = ArabicFixer.Fix(Translation);
					}
					return true;
				}
			}
			return false;
		}

		internal static void LocalizeAll()
		{
			Localize[] array = (Localize[])Resources.FindObjectsOfTypeAll(typeof(Localize));
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				Localize localize = array[i];
				localize.OnLocalize();
			}
			if (LocalizationManager.OnLocalizeEvent != null)
			{
				LocalizationManager.OnLocalizeEvent();
			}
			ResourceManager.pInstance.CleanResourceCache();
		}

		public static bool UpdateSources()
		{
			UnregisterDeletededSources();
			RegisterSourceInResources();
			RegisterSceneSources();
			return Sources.Count > 0;
		}

		private static void UnregisterDeletededSources()
		{
			for (int num = Sources.Count - 1; num >= 0; num--)
			{
				if (Sources[num] == null)
				{
					RemoveSource(Sources[num]);
				}
			}
		}

		private static void RegisterSceneSources()
		{
			LanguageSource[] array = (LanguageSource[])Resources.FindObjectsOfTypeAll(typeof(LanguageSource));
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				if (!Sources.Contains(array[i]))
				{
					AddSource(array[i]);
				}
			}
		}

		private static void RegisterSourceInResources()
		{
			string[] globalSources = GlobalSources;
			foreach (string name in globalSources)
			{
				GameObject asset = ResourceManager.pInstance.GetAsset<GameObject>(name);
				LanguageSource languageSource = ((!asset) ? null : asset.GetComponent<LanguageSource>());
				if ((bool)languageSource && !Sources.Contains(languageSource))
				{
					AddSource(languageSource);
				}
			}
		}

		internal static void AddSource(LanguageSource Source)
		{
			if (!Sources.Contains(Source))
			{
				Sources.Add(Source);
				Source.UpdateDictionary();
				Source.Import_Google();
			}
		}

		internal static void RemoveSource(LanguageSource Source)
		{
			Sources.Remove(Source);
		}

		public static bool IsGlobalSource(string SourceName)
		{
			return Array.IndexOf(GlobalSources, SourceName) >= 0;
		}

		public static bool HasLanguage(string Language, bool AllowDiscartingRegion = true, bool Initialize = true)
		{
			if (Initialize)
			{
				InitializeIfNeeded();
			}
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				if (Sources[i].GetLanguageIndex(Language, false) >= 0)
				{
					return true;
				}
			}
			if (AllowDiscartingRegion)
			{
				int j = 0;
				for (int count2 = Sources.Count; j < count2; j++)
				{
					if (Sources[j].GetLanguageIndex(Language) >= 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static string GetSupportedLanguage(string Language)
		{
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				int languageIndex = Sources[i].GetLanguageIndex(Language, false);
				if (languageIndex >= 0)
				{
					return Sources[i].mLanguages[languageIndex].Name;
				}
			}
			int j = 0;
			for (int count2 = Sources.Count; j < count2; j++)
			{
				int languageIndex2 = Sources[j].GetLanguageIndex(Language);
				if (languageIndex2 >= 0)
				{
					return Sources[j].mLanguages[languageIndex2].Name;
				}
			}
			return string.Empty;
		}

		public static string GetLanguageCode(string Language)
		{
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				int languageIndex = Sources[i].GetLanguageIndex(Language);
				if (languageIndex >= 0)
				{
					return Sources[i].mLanguages[languageIndex].Code;
				}
			}
			return string.Empty;
		}

		public static string GetLanguageFromCode(string Code)
		{
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				int languageIndexFromCode = Sources[i].GetLanguageIndexFromCode(Code);
				if (languageIndexFromCode >= 0)
				{
					return Sources[i].mLanguages[languageIndexFromCode].Name;
				}
			}
			return string.Empty;
		}

		public static List<string> GetAllLanguages()
		{
			List<string> list = new List<string>();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				int j = 0;
				for (int count2 = Sources[i].mLanguages.Count; j < count2; j++)
				{
					if (!list.Contains(Sources[i].mLanguages[j].Name))
					{
						list.Add(Sources[i].mLanguages[j].Name);
					}
				}
			}
			return list;
		}

		public static List<string> GetCategories()
		{
			List<string> list = new List<string>();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				Sources[i].GetCategories(false, list);
			}
			return list;
		}

		public static List<string> GetTermsList()
		{
			if (Sources.Count == 0)
			{
				UpdateSources();
			}
			if (Sources.Count == 1)
			{
				return Sources[0].GetTermsList();
			}
			HashSet<string> hashSet = new HashSet<string>();
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				hashSet.UnionWith(Sources[i].GetTermsList());
			}
			return new List<string>(hashSet);
		}

		public static TermData GetTermData(string term)
		{
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				TermData termData = Sources[i].GetTermData(term);
				if (termData != null)
				{
					return termData;
				}
			}
			return null;
		}

		public static LanguageSource GetSourceContaining(string term, bool fallbackToFirst = true)
		{
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				if (Sources[i].GetTermData(term) != null)
				{
					return Sources[i];
				}
			}
			return (!fallbackToFirst || Sources.Count <= 0) ? null : Sources[0];
		}

		public static UnityEngine.Object FindAsset(string value)
		{
			int i = 0;
			for (int count = Sources.Count; i < count; i++)
			{
				UnityEngine.Object obj = Sources[i].FindAsset(value);
				if ((bool)obj)
				{
					return obj;
				}
			}
			return null;
		}

		public static string GetVersion()
		{
			return "2.5.0 f1";
		}

		public static int GetRequiredWebServiceVersion()
		{
			return 2;
		}

		private static bool IsRTL(string Code)
		{
			return Array.IndexOf(LanguagesRTL, Code) >= 0;
		}
	}
}
