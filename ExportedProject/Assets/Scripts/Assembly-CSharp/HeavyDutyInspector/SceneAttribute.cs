using UnityEngine;

namespace HeavyDutyInspector
{
	public class SceneAttribute : PropertyAttribute
	{
		public string BasePath { get; private set; }

		public SceneAttribute(string basePath)
		{
			BasePath = basePath;
		}
	}
}
