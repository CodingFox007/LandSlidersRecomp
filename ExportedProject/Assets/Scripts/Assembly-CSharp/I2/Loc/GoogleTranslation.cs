using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace I2.Loc
{
	public static class GoogleTranslation
	{
		public static void Translate(string text, string LanguageCodeFrom, string LanguageCodeTo, Action<string> OnTranslationReady)
		{
			WWW translationWWW = GetTranslationWWW(text, LanguageCodeFrom, LanguageCodeTo);
			CoroutineManager.pInstance.StartCoroutine(WaitForTranslation(translationWWW, OnTranslationReady, text));
		}

		private static IEnumerator WaitForTranslation(WWW www, Action<string> OnTranslationReady, string OriginalText)
		{
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogError(www.error);
				OnTranslationReady(string.Empty);
			}
			else
			{
				string obj = ParseTranslationResult(www.text, OriginalText);
				OnTranslationReady(obj);
			}
		}

		public static string ForceTranslate(string text, string LanguageCodeFrom, string LanguageCodeTo)
		{
			WWW translationWWW = GetTranslationWWW(text, LanguageCodeFrom, LanguageCodeTo);
			while (!translationWWW.isDone)
			{
			}
			if (!string.IsNullOrEmpty(translationWWW.error))
			{
				Debug.LogError(translationWWW.error);
				return string.Empty;
			}
			return ParseTranslationResult(translationWWW.text, text);
		}

		private static WWW GetTranslationWWW(string text, string LanguageCodeFrom, string LanguageCodeTo)
		{
			LanguageCodeFrom = GoogleLanguages.GetGoogleLanguageCode(LanguageCodeFrom);
			LanguageCodeTo = GoogleLanguages.GetGoogleLanguageCode(LanguageCodeTo);
			text = text.ToLower();
			string url = string.Format("http://www.google.com/translate_t?hl=en&vi=c&ie=UTF8&oe=UTF8&submit=Translate&langpair={0}|{1}&text={2}", LanguageCodeFrom, LanguageCodeTo, Uri.EscapeUriString(text));
			return new WWW(url);
		}

		private static string ParseTranslationResult(string html, string OriginalText)
		{
			try
			{
				int num = html.IndexOf("TRANSLATED_TEXT") + "TRANSLATED_TEXT='".Length;
				int num2 = html.IndexOf("';INPUT_TOOL_PATH", num);
				string input = html.Substring(num, num2 - num);
				input = Regex.Replace(input, "\\\\x([a-fA-F0-9]{2})", (Match match) => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value, NumberStyles.HexNumber)));
				input = Regex.Replace(input, "&#(\\d+);", (Match match) => char.ConvertFromUtf32(int.Parse(match.Groups[1].Value)));
				input = input.Replace("<br>", "\n");
				if (OriginalText.ToUpper() == OriginalText)
				{
					input = input.ToUpper();
				}
				else if (UppercaseFirst(OriginalText) == OriginalText)
				{
					input = UppercaseFirst(input);
				}
				else if (CultureInfo.CurrentCulture.TextInfo.ToTitleCase(OriginalText) == OriginalText)
				{
					input = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
				}
				return input;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				return string.Empty;
			}
		}

		public static string UppercaseFirst(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			char[] array = s.ToLower().ToCharArray();
			array[0] = char.ToUpper(array[0]);
			return new string(array);
		}

		public static string TitleCase(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s);
		}
	}
}
