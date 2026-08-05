using System;

public class SaveTypeJSON : SaveType
{
	public bool IsType(string valueStr)
	{
		JSONObject jSONObject = new JSONObject(valueStr);
		if (jSONObject.type != JSONObject.Type.NULL)
		{
			return true;
		}
		return false;
	}

	public Type GetValueType()
	{
		return typeof(JSONObject);
	}

	public object FromString(string valueStr)
	{
		return new JSONObject(valueStr);
	}

	public string ToString(object obj)
	{
		JSONObject jSONObject = (JSONObject)obj;
		return jSONObject.ToString();
	}

	public string ShowEditorField(string valueStr)
	{
		if (valueStr != valueStr)
		{
		}
		return valueStr;
	}
}
