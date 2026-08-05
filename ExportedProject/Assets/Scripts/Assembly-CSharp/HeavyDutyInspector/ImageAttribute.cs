using System;
using UnityEngine;

namespace HeavyDutyInspector
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public class ImageAttribute : PropertyAttribute
	{
		public string imagePath { get; private set; }

		public Alignment alignment { get; private set; }

		public ImageAttribute(string imagePath, Alignment alignment = Alignment.Center, int order = 0)
		{
			if (imagePath.ToLower().Substring(0, 7).Equals("assets/"))
			{
				imagePath = imagePath.Substring(7);
			}
			this.imagePath = imagePath;
			this.alignment = alignment;
			base.order = order;
		}
	}
}
