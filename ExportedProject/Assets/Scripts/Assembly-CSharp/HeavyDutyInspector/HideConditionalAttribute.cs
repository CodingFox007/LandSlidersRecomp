using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HeavyDutyInspector
{
	public class HideConditionalAttribute : PropertyAttribute
	{
		public enum ConditionType
		{
			IsNotNull = 0,
			Bool = 1,
			IntOrEnum = 2,
			FloatRange = 3
		}

		public ConditionType conditionType { get; private set; }

		public string variableName { get; private set; }

		public bool boolValue { get; private set; }

		public List<int> enumValues { get; private set; }

		public float minValue { get; private set; }

		public float maxValue { get; private set; }

		public bool isNotNull { get; private set; }

		public HideConditionalAttribute(string conditionVariableName)
		{
			conditionType = ConditionType.IsNotNull;
			variableName = conditionVariableName;
		}

		public HideConditionalAttribute(string conditionVariableName, bool visibleState)
		{
			conditionType = ConditionType.Bool;
			variableName = conditionVariableName;
			boolValue = visibleState;
		}

		public HideConditionalAttribute(string conditionVariableName, params int[] visibleState)
		{
			conditionType = ConditionType.IntOrEnum;
			variableName = conditionVariableName;
			enumValues = new List<int>();
			enumValues = visibleState.ToList();
		}

		public HideConditionalAttribute(string conditionVariableName, float minValue, float maxValue)
		{
			conditionType = ConditionType.FloatRange;
			variableName = conditionVariableName;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
}
