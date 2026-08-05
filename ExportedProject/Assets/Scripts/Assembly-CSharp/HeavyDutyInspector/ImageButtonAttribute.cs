using UnityEngine;

namespace HeavyDutyInspector
{
	public class ImageButtonAttribute : PropertyAttribute
	{
		public string imagePath { get; private set; }

		public string buttonFunction { get; private set; }

		public bool hideVariable { get; private set; }

		public ImageButtonAttribute(string imagePath, string buttonFunction, bool hideVariable = false)
		{
			if (imagePath.ToLower().Substring(0, 7).Equals("assets/"))
			{
				imagePath = imagePath.Substring(7);
			}
			this.imagePath = imagePath;
			this.buttonFunction = buttonFunction;
			this.hideVariable = hideVariable;
		}
	}
}
