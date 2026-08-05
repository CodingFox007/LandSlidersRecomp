using UnityEngine;

namespace HeavyDutyInspector
{
	public static class ColorEx
	{
		public static Color GetColorByEnum(ColorEnum color)
		{
			switch (color)
			{
			case ColorEnum.Black:
				return Color.black;
			case ColorEnum.Blue:
				return Color.blue;
			case ColorEnum.Cyan:
				return Color.cyan;
			case ColorEnum.Gray:
				return Color.gray;
			case ColorEnum.Green:
				return Color.green;
			case ColorEnum.Grey:
				return Color.grey;
			case ColorEnum.Magenta:
				return Color.magenta;
			case ColorEnum.Red:
				return Color.red;
			case ColorEnum.White:
				return Color.white;
			case ColorEnum.Yellow:
				return Color.yellow;
			default:
				return Color.clear;
			}
		}
	}
}
