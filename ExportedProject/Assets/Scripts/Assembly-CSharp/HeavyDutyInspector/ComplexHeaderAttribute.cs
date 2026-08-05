using UnityEngine;

namespace HeavyDutyInspector
{
	public class ComplexHeaderAttribute : PropertyAttribute
	{
		public string text { get; private set; }

		public Style style { get; private set; }

		public Alignment textAlignment { get; private set; }

		public Color textColor { get; private set; }

		public Color backgroundColor { get; private set; }

		public ComplexHeaderAttribute(string text, Style style, Alignment textAlignment, ColorEnum textColor, ColorEnum backgroundColor)
		{
			this.text = text;
			this.style = style;
			this.textAlignment = textAlignment;
			this.textColor = ColorEx.GetColorByEnum(textColor);
			this.backgroundColor = ColorEx.GetColorByEnum(backgroundColor);
			base.order = -2147483647;
		}

		public ComplexHeaderAttribute(string text, Style style, Alignment textAlignment, float textColorR, float textColorG, float textColorB, float backgroundColorR, float backgroundColorG, float backgroundColorB)
		{
			this.text = text;
			this.style = style;
			this.textAlignment = textAlignment;
			textColor = new Color(textColorR, textColorG, textColorB);
			backgroundColor = new Color(backgroundColorR, backgroundColorG, backgroundColorB);
			base.order = -2147483647;
		}
	}
}
