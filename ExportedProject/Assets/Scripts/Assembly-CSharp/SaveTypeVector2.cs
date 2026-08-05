using System;
using UnityEngine;

public class SaveTypeVector2 : SaveType
{
	public bool IsType(string valueStr)
	{
		return valueStr.StartsWith("(v2)");
	}

	public Type GetValueType()
	{
		return typeof(Vector2);
	}

	public object FromString(string valueStr)
	{
		valueStr = valueStr.Replace("(v2)", string.Empty);
		char[] separator = new char[1] { ',' };
		string[] array = valueStr.Split(separator);
		return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
	}

	public string ToString(object obj)
	{
		Vector2 vector = (Vector2)obj;
		return "(v2)" + vector.x + "," + vector.y;
	}

	public string ShowEditorField(string valueStr)
	{
		Vector2 vector = (Vector2)FromString(valueStr);
		Vector2 vector2 = vector;
		return ToString(vector2);
	}
}
