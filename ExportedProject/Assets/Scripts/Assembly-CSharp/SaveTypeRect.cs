using System;
using UnityEngine;

public class SaveTypeRect : SaveType
{
	public bool IsType(string valueStr)
	{
		return valueStr.StartsWith("(rect)");
	}

	public Type GetValueType()
	{
		return typeof(Rect);
	}

	public object FromString(string valueStr)
	{
		valueStr = valueStr.Replace("(rect)", string.Empty);
		char[] separator = new char[1] { ',' };
		string[] array = valueStr.Split(separator);
		return new Rect(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
	}

	public string ToString(object obj)
	{
		Rect rect = (Rect)obj;
		return "(rect)" + rect.x + "," + rect.y + "," + rect.width + "," + rect.height;
	}

	public string ShowEditorField(string valueStr)
	{
		Rect rect = (Rect)FromString(valueStr);
		Rect rect2 = rect;
		return ToString(rect2);
	}
}
