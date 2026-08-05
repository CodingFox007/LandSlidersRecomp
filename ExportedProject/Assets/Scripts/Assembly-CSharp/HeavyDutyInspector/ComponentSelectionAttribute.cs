using System;
using UnityEngine;

namespace HeavyDutyInspector
{
	public class ComponentSelectionAttribute : PropertyAttribute
	{
		public Type componentType { get; private set; }

		public string fieldName { get; private set; }

		public string[] requiredValues { get; private set; }

		public string defaultObject { get; private set; }

		public bool isPrefab { get; private set; }

		public ComponentSelectionAttribute()
		{
			fieldName = string.Empty;
		}

		public ComponentSelectionAttribute(string fieldName)
		{
			this.fieldName = fieldName;
		}

		public ComponentSelectionAttribute(string fieldName, params string[] requiredValues)
		{
			this.fieldName = fieldName;
			this.requiredValues = requiredValues;
		}

		public ComponentSelectionAttribute(string defaultObject, bool isPrefab)
		{
			this.defaultObject = defaultObject;
			this.isPrefab = isPrefab;
		}

		public ComponentSelectionAttribute(string defaultObject, bool isPrefab, string fieldName)
		{
			this.fieldName = fieldName;
			this.defaultObject = defaultObject;
			this.isPrefab = isPrefab;
		}

		public ComponentSelectionAttribute(string defaultObject, bool isPrefab, string fieldName, params string[] requiredValues)
		{
			this.fieldName = fieldName;
			this.requiredValues = requiredValues;
			this.defaultObject = defaultObject;
			this.isPrefab = isPrefab;
		}
	}
}
