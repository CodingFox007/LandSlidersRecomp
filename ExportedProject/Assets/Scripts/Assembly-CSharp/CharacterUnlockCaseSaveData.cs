public class CharacterUnlockCaseSaveData : Savable
{
	private static CharacterUnlockCaseSaveData _instance;

	public int m_openedBoxes;

	public int m_goldUnlock;

	public static CharacterUnlockCaseSaveData Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new CharacterUnlockCaseSaveData();
			}
			return _instance;
		}
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		jSONObject.AddField("openedBoxes", m_openedBoxes);
		jSONObject.AddField("goldUnlock", m_goldUnlock);
		return jSONObject;
	}

	public bool Verify(JSONObject data)
	{
		return true;
	}

	public void Load(JSONObject data)
	{
		if (data != null && (bool)data.GetField("openedBoxes"))
		{
			m_openedBoxes = data.GetField("openedBoxes").AsInt;
		}
		if (data != null && (bool)data.GetField("goldUnlock"))
		{
			m_goldUnlock = data.GetField("goldUnlock").AsInt;
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
		m_openedBoxes = 0;
		m_goldUnlock = 0;
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
		return "CharacterUnlockCase";
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
}
