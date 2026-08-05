using System;
using UnityEngine;

public class SaveTypeColor : SaveType
{
	public bool IsType(string valueStr)
	{
		return valueStr.StartsWith("(col)");
	}

	public Type GetValueType()
	{
		return typeof(Color);
	}

	public object FromString(string valueStr)
	{
		valueStr = valueStr.Replace("(col)", string.Empty);
		char[] separator = new char[1] { ',' };
		string[] array = valueStr.Split(separator);
		return new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
	}

	public string ToString(object obj)
	{
		Color color = (Color)obj;
		return "(col)" + color.r + "," + color.g + "," + color.b + "," + color.a;
	}

	public string ShowEditorField(string valueStr)
	{
		Color color = (Color)FromString(valueStr);
		Color color2 = color;
		return ToString(color2);
	}
}
