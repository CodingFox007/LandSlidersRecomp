using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using I2.Loc.SimpleJSON;
using UnityEngine;

namespace I2.Loc
{
	[AddComponentMenu("I2/Localization/Source")]
	public class LanguageSource : MonoBehaviour
	{
		public enum eGoogleUpdateFrequency
		{
			Always = 0,
			Never = 1,
			Daily = 2,
			Weekly = 3,
			Monthly = 4
		}

		public string Google_WebServiceURL;

		public string Google_SpreadsheetKey;

		public string Google_SpreadsheetName;

		public string Google_LastUpdatedVersion;

		public eGoogleUpdateFrequency GoogleUpdateFrequency = eGoogleUpdateFrequency.Weekly;

		public static string EmptyCategory = "Default";

		public static char[] CategorySeparators = "/\\".ToCharArray();

		public List<TermData> mTerms = new List<TermData>();

		public List<LanguageData> mLanguages = new List<LanguageData>();

		public bool CaseInsensitiveTerms;

		[NonSerialized]
		public Dictionary<string, TermData> mDictionary = new Dictionary<string, TermData>(StringComparer.Ordinal);

		public UnityEngine.Object[] Assets;

		public bool NeverDestroy = true;

		public bool UserAgreesToHaveItOnTheScene;

		public event Action<LanguageSource> Event_OnSourceUpdateFromGoogle;

		public string Export_CSV(string Category, char Separator = ',')
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = mLanguages.Count;
			stringBuilder.AppendFormat("Key{0}Type{0}Desc", Separator);
			foreach (LanguageData mLanguage in mLanguages)
			{
				stringBuilder.Append(Separator);
				AppendString(stringBuilder, GoogleLanguages.GetCodedLanguage(mLanguage.Name, mLanguage.Code), Separator);
			}
			stringBuilder.Append("\n");
			foreach (TermData mTerm in mTerms)
			{
				string term;
				if (string.IsNullOrEmpty(Category) || (Category == EmptyCategory && mTerm.Term.IndexOfAny(CategorySeparators) < 0))
				{
					term = mTerm.Term;
				}
				else
				{
					if (!mTerm.Term.StartsWith(Category) || !(Category != mTerm.Term))
					{
						continue;
					}
					term = mTerm.Term.Substring(Category.Length + 1);
				}
				AppendTerm(stringBuilder, count, term, mTerm, null, mTerm.Languages, mTerm.Languages_Touch, Separator);
				if (mTerm.HasTouchTranslations())
				{
					AppendTerm(stringBuilder, count, term, mTerm, "[touch]", mTerm.Languages_Touch, null, Separator);
				}
			}
			return stringBuilder.ToString();
		}

		private static void AppendTerm(StringBuilder Builder, int nLanguages, string Term, TermData termData, string prefix, string[] aLanguages, string[] aSecLanguages, char Separator)
		{
			AppendString(Builder, Term, Separator);
			if (!string.IsNullOrEmpty(prefix))
			{
				Builder.Append(prefix);
			}
			Builder.Append(Separator);
			Builder.Append(termData.TermType.ToString());
			Builder.Append(Separator);
			AppendString(Builder, termData.Description, Separator);
			for (int i = 0; i < Mathf.Min(nLanguages, aLanguages.Length); i++)
			{
				Builder.Append(Separator);
				string text = aLanguages[i];
				if (string.IsNullOrEmpty(text) && aSecLanguages != null)
				{
					text = aSecLanguages[i];
				}
				AppendString(Builder, text, Separator);
			}
			Builder.Append("\n");
		}

		private static void AppendString(StringBuilder Builder, string Text, char Separator)
		{
			if (!string.IsNullOrEmpty(Text))
			{
				Text = Text.Replace("\\n", "\n");
				if (Text.IndexOfAny((Separator + "\n\"").ToCharArray()) >= 0)
				{
					Text = Text.Replace("\"", "\"\"");
					Builder.AppendFormat("\"{0}\"", Text);
				}
				else
				{
					Builder.Append(Text);
				}
			}
		}

		public WWW Export_Google_CreateWWWcall(eSpreadsheetUpdateMode UpdateMode = eSpreadsheetUpdateMode.Replace)
		{
			string value = Export_Google_CreateData();
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("key", Google_SpreadsheetKey);
			wWWForm.AddField("action", "SetLanguageSource");
			wWWForm.AddField("data", value);
			wWWForm.AddField("updateMode", UpdateMode.ToString());
			return new WWW(Google_WebServiceURL, wWWForm);
		}

		private string Export_Google_CreateData()
		{
			List<string> categories = GetCategories(true);
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (string item in categories)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append("<I2Loc>");
				}
				string value = Export_CSV(item);
				stringBuilder.Append(item);
				stringBuilder.Append("<I2Loc>");
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public string Import_CSV(string Category, string CSVstring, eSpreadsheetUpdateMode UpdateMode = eSpreadsheetUpdateMode.Replace, char Separator = ',')
		{
			List<string[]> list = LocalizationReader.ReadCSV(CSVstring, Separator);
			string[] array = list[0];
			int num = 1;
			int num2 = -1;
			int num3 = -1;
			string[] texts = new string[1] { "Key" };
			string[] texts2 = new string[1] { "Type" };
			string[] texts3 = new string[2] { "Desc", "Description" };
			if (array.Length > 1 && ArrayContains(array[0], texts))
			{
				if (UpdateMode == eSpreadsheetUpdateMode.Replace)
				{
					ClearAllData();
				}
				if (array.Length > 2)
				{
					if (ArrayContains(array[1], texts2))
					{
						num2 = 1;
						num = 2;
					}
					if (ArrayContains(array[1], texts3))
					{
						num3 = 1;
						num = 2;
					}
				}
				if (array.Length > 3)
				{
					if (ArrayContains(array[2], texts2))
					{
						num2 = 2;
						num = 3;
					}
					if (ArrayContains(array[2], texts3))
					{
						num3 = 2;
						num = 3;
					}
				}
				int num4 = Mathf.Max(array.Length - num, 0);
				int[] array2 = new int[num4];
				for (int i = 0; i < num4; i++)
				{
					string Language;
					string code;
					GoogleLanguages.UnPackCodeFromLanguageName(array[i + num], out Language, out code);
					int num5 = -1;
					num5 = (string.IsNullOrEmpty(code) ? GetLanguageIndex(Language) : GetLanguageIndexFromCode(code));
					if (num5 < 0)
					{
						LanguageData languageData = new LanguageData();
						languageData.Name = Language;
						languageData.Code = code;
						mLanguages.Add(languageData);
						num5 = mLanguages.Count - 1;
					}
					array2[i] = num5;
				}
				num4 = mLanguages.Count;
				int j = 0;
				for (int count = mTerms.Count; j < count; j++)
				{
					TermData termData = mTerms[j];
					if (termData.Languages.Length < num4)
					{
						Array.Resize(ref termData.Languages, num4);
						Array.Resize(ref termData.Languages_Touch, num4);
					}
				}
				int k = 1;
				for (int count2 = list.Count; k < count2; k++)
				{
					array = list[k];
					string Term = ((!string.IsNullOrEmpty(Category)) ? (Category + "/" + array[0]) : array[0]);
					bool flag = false;
					if (Term.EndsWith("[touch]"))
					{
						Term = Term.Remove(Term.Length - "[touch]".Length);
						flag = true;
					}
					ValidateFullTerm(ref Term);
					TermData termData2 = GetTermData(Term);
					if (termData2 == null)
					{
						termData2 = new TermData();
						termData2.Term = Term;
						termData2.Languages = new string[mLanguages.Count];
						termData2.Languages_Touch = new string[mLanguages.Count];
						for (int l = 0; l < mLanguages.Count; l++)
						{
							termData2.Languages[l] = (termData2.Languages_Touch[l] = string.Empty);
						}
						mTerms.Add(termData2);
						mDictionary.Add(Term, termData2);
					}
					else if (UpdateMode == eSpreadsheetUpdateMode.AddNewTerms)
					{
						continue;
					}
					if (num2 > 0)
					{
						termData2.TermType = GetTermType(array[num2]);
					}
					if (num3 > 0)
					{
						termData2.Description = array[num3];
					}
					for (int m = 0; m < array2.Length && m < array.Length - num; m++)
					{
						if (!string.IsNullOrEmpty(array[m + num]))
						{
							if (flag)
							{
								termData2.Languages_Touch[array2[m]] = array[m + num];
							}
							else
							{
								termData2.Languages[array2[m]] = array[m + num];
							}
						}
					}
				}
				return string.Empty;
			}
			return "Bad Spreadsheet Format.\nFirst columns should be 'Key', 'Type' and 'Desc'";
		}

		private bool ArrayContains(string MainText, params string[] texts)
		{
			int i = 0;
			for (int num = texts.Length; i < num; i++)
			{
				if (MainText.IndexOf(texts[i], StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		public static eTermType GetTermType(string type)
		{
			int i = 0;
			for (int num = 6; i <= num; i++)
			{
				eTermType eTermType2 = (eTermType)i;
				if (string.Equals(eTermType2.ToString(), type, StringComparison.OrdinalIgnoreCase))
				{
					return (eTermType)i;
				}
			}
			return eTermType.Text;
		}

		public void Import_Google(bool ForceUpdate = false)
		{
			if (GoogleUpdateFrequency == eGoogleUpdateFrequency.Never || !Application.isPlaying)
			{
				return;
			}
			string sourcePlayerPrefName = GetSourcePlayerPrefName();
			if (!ForceUpdate && GoogleUpdateFrequency != eGoogleUpdateFrequency.Always)
			{
				string s = PlayerPrefs.GetString("LastGoogleUpdate_" + sourcePlayerPrefName, string.Empty);
				DateTime result;
				if (DateTime.TryParse(s, out result))
				{
					double totalDays = (DateTime.Now - result).TotalDays;
					switch (GoogleUpdateFrequency)
					{
					case eGoogleUpdateFrequency.Daily:
						if (totalDays < 1.0)
						{
							return;
						}
						break;
					case eGoogleUpdateFrequency.Weekly:
						if (totalDays < 8.0)
						{
							return;
						}
						break;
					case eGoogleUpdateFrequency.Monthly:
						if (totalDays < 31.0)
						{
							return;
						}
						break;
					}
				}
			}
			PlayerPrefs.SetString("LastGoogleUpdate_" + sourcePlayerPrefName, DateTime.Now.ToString());
			string text = PlayerPrefs.GetString("I2Source_" + Google_SpreadsheetKey, string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				Import_Google_Result(text, eSpreadsheetUpdateMode.Replace);
			}
			CoroutineManager.pInstance.StartCoroutine(Import_Google_Coroutine());
		}

		private string GetSourcePlayerPrefName()
		{
			if (Array.IndexOf(LocalizationManager.GlobalSources, base.name) >= 0)
			{
				return base.name;
			}
			return Application.loadedLevelName + "_" + base.name;
		}

		private IEnumerator Import_Google_Coroutine()
		{
			WWW www = Import_Google_CreateWWWcall();
			if (www == null)
			{
				yield break;
			}
			while (!www.isDone)
			{
				yield return null;
			}
			if (string.IsNullOrEmpty(www.error) && www.text != "\"\"")
			{
				PlayerPrefs.SetString("I2Source_" + Google_SpreadsheetKey, www.text);
				PlayerPrefs.Save();
				Import_Google_Result(www.text, eSpreadsheetUpdateMode.Replace);
				if (this.Event_OnSourceUpdateFromGoogle != null)
				{
					this.Event_OnSourceUpdateFromGoogle(this);
				}
				LocalizationManager.LocalizeAll();
				Debug.Log("Done Google Sync '" + www.text + "'");
			}
			else
			{
				Debug.Log("Language Source was up-to-date with Google Spreadsheet");
			}
		}

		public WWW Import_Google_CreateWWWcall(bool ForceUpdate = false)
		{
			if (!HasGoogleSpreadsheet())
			{
				return null;
			}
			string url = string.Format("{0}?key={1}&action=GetLanguageSource&version={2}", Google_WebServiceURL, Google_SpreadsheetKey, (!ForceUpdate) ? Google_LastUpdatedVersion : "0");
			return new WWW(url);
		}

		public bool HasGoogleSpreadsheet()
		{
			return !string.IsNullOrEmpty(Google_WebServiceURL) && !string.IsNullOrEmpty(Google_SpreadsheetKey);
		}

		public string Import_Google_Result(string JsonString, eSpreadsheetUpdateMode UpdateMode)
		{
			string result = string.Empty;
			if (JsonString == "\"\"")
			{
				Debug.Log("Language Source was up to date");
				return result;
			}
			JSONClass asObject = JSON.Parse(JsonString).AsObject;
			if (UpdateMode == eSpreadsheetUpdateMode.Replace)
			{
				ClearAllData();
			}
			Google_LastUpdatedVersion = asObject["version"];
			int num = int.Parse(asObject["script_version"]);
			if (num < LocalizationManager.GetRequiredWebServiceVersion())
			{
				result = "The current Google WebService is not supported.\nPlease, delete the WebService from the Google Drive and Install the latest version.";
			}
			JSONClass asObject2 = asObject["spreadsheet"].AsObject;
			foreach (KeyValuePair<string, JSONNode> item in asObject2)
			{
				Import_CSV(item.Key, item.Value, UpdateMode);
				if (UpdateMode == eSpreadsheetUpdateMode.Replace)
				{
					UpdateMode = eSpreadsheetUpdateMode.Merge;
				}
			}
			return result;
		}

		public List<string> GetCategories(bool OnlyMainCategory = false, List<string> Categories = null)
		{
			if (Categories == null)
			{
				Categories = new List<string>();
			}
			foreach (TermData mTerm in mTerms)
			{
				string categoryFromFullTerm = GetCategoryFromFullTerm(mTerm.Term, OnlyMainCategory);
				if (!Categories.Contains(categoryFromFullTerm))
				{
					Categories.Add(categoryFromFullTerm);
				}
			}
			Categories.Sort();
			return Categories;
		}

		public static string GetKeyFromFullTerm(string FullTerm, bool OnlyMainCategory = false)
		{
			int num = ((!OnlyMainCategory) ? FullTerm.LastIndexOfAny(CategorySeparators) : FullTerm.IndexOfAny(CategorySeparators));
			return (num >= 0) ? FullTerm.Substring(num + 1) : FullTerm;
		}

		public static string GetCategoryFromFullTerm(string FullTerm, bool OnlyMainCategory = false)
		{
			int num = ((!OnlyMainCategory) ? FullTerm.LastIndexOfAny(CategorySeparators) : FullTerm.IndexOfAny(CategorySeparators));
			return (num >= 0) ? FullTerm.Substring(0, num) : EmptyCategory;
		}

		public static void DeserializeFullTerm(string FullTerm, out string Key, out string Category, bool OnlyMainCategory = false)
		{
			int num = ((!OnlyMainCategory) ? FullTerm.LastIndexOfAny(CategorySeparators) : FullTerm.IndexOfAny(CategorySeparators));
			if (num < 0)
			{
				Category = EmptyCategory;
				Key = FullTerm;
			}
			else
			{
				Category = FullTerm.Substring(0, num);
				Key = FullTerm.Substring(num + 1);
			}
		}

		internal static string GetKeyFromFullTerm(string FullTerm)
		{
			int num = FullTerm.LastIndexOfAny(CategorySeparators);
			return FullTerm.Substring(num + 1);
		}

		internal static string GetCategoryFromFullTerm(string FullTerm)
		{
			int num = FullTerm.LastIndexOfAny(CategorySeparators);
			if (num < 0)
			{
				return EmptyCategory;
			}
			return FullTerm.Substring(0, num);
		}

		internal static void DeserializeFullTerm(string FullTerm, out string Key, out string Category)
		{
			int num = FullTerm.LastIndexOfAny(CategorySeparators);
			if (num < 0)
			{
				Category = EmptyCategory;
				Key = FullTerm;
			}
			else
			{
				Category = FullTerm.Substring(0, num);
				Key = FullTerm.Substring(num + 1);
			}
		}

		private void Awake()
		{
			if (NeverDestroy)
			{
				if (ManagerHasASimilarSource())
				{
					UnityEngine.Object.Destroy(this);
					return;
				}
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			LocalizationManager.AddSource(this);
			UpdateDictionary();
		}

		public void UpdateDictionary()
		{
			StringComparer stringComparer = ((!CaseInsensitiveTerms) ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
			if (mDictionary.Comparer != stringComparer)
			{
				mDictionary = new Dictionary<string, TermData>(stringComparer);
			}
			else
			{
				mDictionary.Clear();
			}
			int i = 0;
			for (int count = mTerms.Count; i < count; i++)
			{
				ValidateFullTerm(ref mTerms[i].Term);
				if (mTerms[i].Languages_Touch == null || mTerms[i].Languages_Touch.Length != mTerms[i].Languages.Length)
				{
					mTerms[i].Languages_Touch = new string[mTerms[i].Languages.Length];
				}
				mDictionary[mTerms[i].Term] = mTerms[i];
			}
		}

		public string GetSourceName()
		{
			string text = base.gameObject.name;
			Transform parent = base.transform.parent;
			while ((bool)parent)
			{
				text = parent.name + "_" + text;
				parent = parent.parent;
			}
			return text;
		}

		public int GetLanguageIndex(string language, bool AllowDiscartingRegion = true)
		{
			int i = 0;
			for (int count = mLanguages.Count; i < count; i++)
			{
				if (string.Compare(mLanguages[i].Name, language, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			if (AllowDiscartingRegion)
			{
				int j = 0;
				for (int count2 = mLanguages.Count; j < count2; j++)
				{
					if (AreTheSameLanguage(mLanguages[j].Name, language))
					{
						return j;
					}
				}
			}
			return -1;
		}

		public int GetLanguageIndexFromCode(string Code)
		{
			int i = 0;
			for (int count = mLanguages.Count; i < count; i++)
			{
				if (string.Compare(mLanguages[i].Code, Code, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		public static bool AreTheSameLanguage(string Language1, string Language2)
		{
			Language1 = GetLanguageWithoutRegion(Language1);
			Language2 = GetLanguageWithoutRegion(Language2);
			return string.Compare(Language1, Language2, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static string GetLanguageWithoutRegion(string Language)
		{
			int num = Language.IndexOfAny("(/\\[,{".ToCharArray());
			if (num < 0)
			{
				return Language;
			}
			return Language.Substring(0, num).Trim();
		}

		public void AddLanguage(string LanguageName, string LanguageCode)
		{
			if (GetLanguageIndex(LanguageName, false) < 0)
			{
				LanguageData languageData = new LanguageData();
				languageData.Name = LanguageName;
				languageData.Code = LanguageCode;
				mLanguages.Add(languageData);
				int count = mLanguages.Count;
				int i = 0;
				for (int count2 = mTerms.Count; i < count2; i++)
				{
					Array.Resize(ref mTerms[i].Languages, count);
					Array.Resize(ref mTerms[i].Languages_Touch, count);
				}
			}
		}

		public void RemoveLanguage(string LanguageName)
		{
			int languageIndex = GetLanguageIndex(LanguageName);
			if (languageIndex < 0)
			{
				return;
			}
			int count = mLanguages.Count;
			int i = 0;
			for (int count2 = mTerms.Count; i < count2; i++)
			{
				for (int j = languageIndex + 1; j < count; j++)
				{
					mTerms[i].Languages[j - 1] = mTerms[i].Languages[j];
				}
				Array.Resize(ref mTerms[i].Languages, count - 1);
				for (int k = languageIndex + 1; k < count; k++)
				{
					mTerms[i].Languages_Touch[k - 1] = mTerms[i].Languages_Touch[k];
				}
				Array.Resize(ref mTerms[i].Languages_Touch, count - 1);
			}
			mLanguages.RemoveAt(languageIndex);
		}

		public List<string> GetLanguages()
		{
			List<string> list = new List<string>();
			int i = 0;
			for (int count = mLanguages.Count; i < count; i++)
			{
				list.Add(mLanguages[i].Name);
			}
			return list;
		}

		public string GetTermTranslation(string term)
		{
			int languageIndex = GetLanguageIndex(LocalizationManager.CurrentLanguage);
			if (languageIndex < 0)
			{
				return string.Empty;
			}
			TermData termData = GetTermData(term);
			if (termData != null)
			{
				return termData.GetTranslation(languageIndex);
			}
			return string.Empty;
		}

		public bool TryGetTermTranslation(string term, out string Translation)
		{
			int languageIndex = GetLanguageIndex(LocalizationManager.CurrentLanguage);
			if (languageIndex >= 0)
			{
				TermData termData = GetTermData(term);
				if (termData != null)
				{
					Translation = termData.GetTranslation(languageIndex);
					return true;
				}
			}
			Translation = string.Empty;
			return false;
		}

		public TermData AddTerm(string term)
		{
			return AddTerm(term, eTermType.Text);
		}

		public TermData GetTermData(string term)
		{
			if (mDictionary.Count == 0)
			{
				UpdateDictionary();
			}
			TermData value;
			if (!mDictionary.TryGetValue(term, out value))
			{
				return null;
			}
			return value;
		}

		public bool ContainsTerm(string term)
		{
			return GetTermData(term) != null;
		}

		public List<string> GetTermsList()
		{
			if (mDictionary.Count != mTerms.Count)
			{
				UpdateDictionary();
			}
			return new List<string>(mDictionary.Keys);
		}

		public TermData AddTerm(string NewTerm, eTermType termType)
		{
			ValidateFullTerm(ref NewTerm);
			NewTerm = NewTerm.Trim();
			TermData termData = GetTermData(NewTerm);
			if (termData == null)
			{
				termData = new TermData();
				termData.Term = NewTerm;
				termData.TermType = termType;
				termData.Languages = new string[mLanguages.Count];
				termData.Languages_Touch = new string[mLanguages.Count];
				mTerms.Add(termData);
				mDictionary.Add(NewTerm, termData);
			}
			return termData;
		}

		public void RemoveTerm(string term)
		{
			int i = 0;
			for (int count = mTerms.Count; i < count; i++)
			{
				if (mTerms[i].Term == term)
				{
					mTerms.RemoveAt(i);
					mDictionary.Remove(term);
					break;
				}
			}
		}

		public static void ValidateFullTerm(ref string Term)
		{
			Term = Term.Replace('\\', '/');
			Term = Term.Trim();
			if (Term.StartsWith(EmptyCategory) && Term.Length > EmptyCategory.Length && Term[EmptyCategory.Length] == '/')
			{
				Term = Term.Substring(EmptyCategory.Length + 1);
			}
		}

		public bool IsEqualTo(LanguageSource Source)
		{
			if (Source.mLanguages.Count != mLanguages.Count)
			{
				return false;
			}
			int i = 0;
			for (int count = mLanguages.Count; i < count; i++)
			{
				if (Source.GetLanguageIndex(mLanguages[i].Name) < 0)
				{
					return false;
				}
			}
			if (Source.mTerms.Count != mTerms.Count)
			{
				return false;
			}
			for (int j = 0; j < mTerms.Count; j++)
			{
				if (Source.GetTermData(mTerms[j].Term) == null)
				{
					return false;
				}
			}
			return true;
		}

		internal bool ManagerHasASimilarSource()
		{
			int i = 0;
			for (int count = LocalizationManager.Sources.Count; i < count; i++)
			{
				LanguageSource languageSource = LocalizationManager.Sources[i];
				if (languageSource != null && languageSource.IsEqualTo(this) && languageSource != this)
				{
					return true;
				}
			}
			return false;
		}

		public void ClearAllData()
		{
			mTerms.Clear();
			mLanguages.Clear();
			mDictionary.Clear();
		}

		public UnityEngine.Object FindAsset(string Name)
		{
			if (Assets != null)
			{
				int i = 0;
				for (int num = Assets.Length; i < num; i++)
				{
					if (Assets[i] != null && Assets[i].name == Name)
					{
						return Assets[i];
					}
				}
			}
			return null;
		}

		public bool HasAsset(UnityEngine.Object Obj)
		{
			return Array.IndexOf(Assets, Obj) >= 0;
		}
	}
}
