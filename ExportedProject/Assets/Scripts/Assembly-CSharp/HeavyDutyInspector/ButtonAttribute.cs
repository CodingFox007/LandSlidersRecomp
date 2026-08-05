using UnityEngine;

namespace HeavyDutyInspector
{
	public class ButtonAttribute : PropertyAttribute
	{
		public string buttonText { get; private set; }

		public string buttonFunction { get; private set; }

		public bool hideVariable { get; private set; }

		public ButtonAttribute(string buttonText, string buttonFunction, bool hideVariable = false)
		{
			this.buttonText = buttonText;
			this.buttonFunction = buttonFunction;
			this.hideVariable = hideVariable;
		}
	}
}
