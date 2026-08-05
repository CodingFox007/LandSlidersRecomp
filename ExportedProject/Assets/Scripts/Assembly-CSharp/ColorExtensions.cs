using UnityEngine;

public static class ColorExtensions
{
	public static void FromVector4(this Color color, Vector4 vector4)
	{
		color.r = vector4.x;
		color.g = vector4.y;
		color.b = vector4.z;
		color.a = vector4.w;
	}
}
