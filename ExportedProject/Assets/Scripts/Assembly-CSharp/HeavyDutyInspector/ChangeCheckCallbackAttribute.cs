using UnityEngine;

namespace HeavyDutyInspector
{
	public class ChangeCheckCallbackAttribute : PropertyAttribute
	{
		public string callback { get; private set; }

		public ChangeCheckCallbackAttribute(string callbackName)
		{
			callback = callbackName;
		}
	}
}
