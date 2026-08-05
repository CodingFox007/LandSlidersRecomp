using System;
using UnityEngine;

public class SaveTypeVector3 : SaveType
{
	public bool IsType(string valueStr)
	{
		return valueStr.StartsWith("(v3)");
	}

	public Type GetValueType()
	{
		return typeof(Vector3);
	}

	public object FromString(string valueStr)
	{
		valueStr = valueStr.Replace("(v3)", string.Empty);
		char[] separator = new char[1] { ',' };
		string[] array = valueStr.Split(separator);
		return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
	}

	public string ToString(object obj)
	{
		Vector3 vector = (Vector3)obj;
		return "(v3)" + vector.x + "," + vector.y + "," + vector.z;
	}

	public string ShowEditorField(string valueStr)
	{
		Vector3 vector = (Vector3)FromString(valueStr);
		Vector3 vector2 = vector;
		return ToString(vector2);
	}
}
