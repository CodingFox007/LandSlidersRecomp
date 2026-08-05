using System;
using UnityEngine;

public class LevelClouds : MonoBehaviour
{
	[Serializable]
	public class CloudRing
	{
		public Cloud[] m_clouds;

		public RandomRange m_angleSpacing;

		public RandomRange m_distance;

		public RandomRange m_height;

		public bool m_removeFront;
	}

	[SerializeField]
	private CloudRing[] m_cloudRings;

	public CloudRing[] CloudRings
	{
		get
		{
			return m_cloudRings;
		}
	}
}
