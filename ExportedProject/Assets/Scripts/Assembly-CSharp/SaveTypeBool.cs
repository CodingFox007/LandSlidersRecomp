using System;

public class SaveTypeBool : SaveType
{
	public bool IsType(string valueStr)
	{
		return valueStr.StartsWith("(b)");
	}

	public Type GetValueType()
	{
		return typeof(bool);
	}

	public object FromString(string valueStr)
	{
		valueStr = valueStr.Replace("(b)", string.Empty);
		return bool.Parse(valueStr);
	}

	public string ToString(object obj)
	{
		return "(b)" + (bool)obj/*cast due to .constrained prefix*/;
	}

	public string ShowEditorField(string valueStr)
	{
		bool flag = (bool)FromString(valueStr);
		bool flag2 = flag;
		return ToString(flag2);
	}
}
