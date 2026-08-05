using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class DevGiftManager : MonoBehaviour, Savable
{
	private string m_userID;

	public string ID
	{
		get
		{
			if (m_userID == string.Empty)
			{
				m_userID += GenerateLetter();
				m_userID += GenerateNumber();
				m_userID += GenerateLetter();
				m_userID += GenerateNumber();
				m_userID += GenerateLetter();
				m_userID += GenerateNumber();
				m_userID += GenerateLetter();
				m_userID += GenerateNumber();
				SaveManager.Save();
			}
			return m_userID;
		}
	}

	public string GenerateLetter()
	{
		return Convert.ToChar(65 + UnityEngine.Random.Range(0, 26)).ToString();
	}

	public string GenerateNumber()
	{
		return UnityEngine.Random.Range(0, 10).ToString();
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		jSONObject.AddField("id", m_userID);
		return jSONObject;
	}

	public bool Verify(JSONObject data)
	{
		return true;
	}

	public void Load(JSONObject data)
	{
		if (data != null && (bool)data.GetField("id"))
		{
			m_userID = data.GetField("id").AsString;
		}
	}

	public void Merge(JSONObject dataA, JSONObject dataB)
	{
	}

	public JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB)
	{
		return dataA;
	}

	public JSONObject Reset()
	{
		JSONObject result = new JSONObject();
		m_userID = string.Empty;
		return result;
	}

	public JSONObject UpdateVersion(JSONObject data)
	{
		JSONObject result = data;
		string version = GetVersion(data);
		if (!(version == string.Empty))
		{
			switch (version)
			{
			case "0.0.1":
			case "0.0.2":
			case "0.0.3":
				break;
			default:
				goto IL_0051;
			}
		}
		result = Reset();
		goto IL_0051;
		IL_0051:
		return result;
	}

	public string SaveId()
	{
		return "UserID";
	}

	public string Version()
	{
		return "0.0.4";
	}

	public string GetVersion(JSONObject data)
	{
		if (!data.HasField("version"))
		{
			return string.Empty;
		}
		return data.GetField("version").AsString;
	}

	public bool UseCloud()
	{
		return false;
	}

	public static byte[] StringToByteArray(string hex)
	{
		int length = hex.Length;
		byte[] array = new byte[length / 2];
		for (int i = 0; i < length; i += 2)
		{
			array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
		}
		return array;
	}

	public void Update()
	{
		string text = string.Empty;
		if (!(text != string.Empty))
		{
			return;
		}
		if (text.StartsWith("landsliders://"))
		{
			text = text.Substring("landsliders://".Length);
		}
		PlayerPrefs.DeleteKey("url");
		byte[] array = Convert.FromBase64String(text);
		byte[] array2 = new byte[array.Length];
		string str = string.Empty;
		byte[] rgbKey = StringToByteArray("0f0201030504060709080a0b0c0d0e00");
		byte[] rgbIV = StringToByteArray("2031425364758697a8b9cadbecfd1e1f");
		using (RijndaelManaged rijndaelManaged = new RijndaelManaged())
		{
			rijndaelManaged.Mode = CipherMode.CBC;
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(rgbKey, rgbIV))
			{
				using (MemoryStream stream = new MemoryStream(array))
				{
					using (CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Read))
					{
						int count = cryptoStream.Read(array2, 0, array2.Length);
						str = Encoding.UTF8.GetString(array2, 0, count);
					}
				}
			}
		}
		JSONObject jSONObject = new JSONObject(str);
		if (jSONObject == null || jSONObject.keys == null)
		{
			PlayerPrefs.DeleteKey("url");
			return;
		}
		string text2 = string.Empty;
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		bool flag = false;
		bool flag2 = true;
		foreach (string key in jSONObject.keys)
		{
			if (key == "userid")
			{
				if (jSONObject[key].AsString != ID)
				{
					text2 += "ID/No match ";
					continue;
				}
				text2 += "ID/Match ";
				flag = true;
			}
			else if (key == "uniqueGift")
			{
				if (PlayerPrefs.HasKey(jSONObject[key].AsInt.ToString()))
				{
					text2 += "Claimed/Yes ";
					continue;
				}
				text2 += "Claimed/No ";
				PlayerPrefs.SetInt(jSONObject[key].AsInt.ToString(), 1);
				flag2 = false;
			}
			else
			{
				string text3 = text2;
				text2 = text3 + "Giving " + jSONObject[key].AsInt + "x " + key + ";";
				dictionary.Add(key, jSONObject[key].AsInt);
			}
		}
		if (!flag || flag2)
		{
			return;
		}
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			Singleton<Game>.Instance.InventoryManager.AddCurrency(item.Key, item.Value);
		}
	}
}
