using UnityEngine;

namespace HeavyDutyInspector
{
	public class BackgroundAttribute : PropertyAttribute
	{
		public Color color { get; private set; }

		public BackgroundAttribute(ColorEnum color)
		{
			this.color = ColorEx.GetColorByEnum(color);
			base.order = int.MaxValue;
		}

		public BackgroundAttribute(float r, float g, float b)
		{
			color = new Color(r, g, b);
		}
	}
}
