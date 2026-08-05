using UnityEngine;

public static class VectorExtensions
{
	public static void FromVector3(this Vector2 vector2, Vector3 vector3)
	{
		vector2.x = vector3.x;
		vector2.y = vector3.z;
	}

	public static void FromVector4(this Vector2 vector2, Vector4 vector4)
	{
		vector2.x = vector4.x;
		vector2.y = vector4.z;
	}

	public static void FromVector4(this Vector3 vector3, Vector4 vector4)
	{
		vector3.x = vector4.x;
		vector3.y = vector4.z;
		vector3.y = vector4.z;
	}

	public static Vector2 ToVector2(this Vector3 vector3)
	{
		return new Vector2(vector3.x, vector3.y);
	}

	public static Vector2 ToVector2(this Vector4 vector4)
	{
		return new Vector2(vector4.x, vector4.y);
	}

	public static Vector3 ToVector3(this Vector4 vector4)
	{
		return new Vector3(vector4.x, vector4.y, vector4.z);
	}

	public static void Parse(this Vector2 vector2, string str)
	{
		char[] separator = new char[1] { ',' };
		string[] array = str.Split(separator);
		vector2 = new Vector2(float.Parse(array[0]), float.Parse(array[1]));
	}
}
