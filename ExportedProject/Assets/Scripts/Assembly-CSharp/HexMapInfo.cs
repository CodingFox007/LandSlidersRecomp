using System;
using HeavyDutyInspector;
using UnityEngine;

[Serializable]
public class HexMapInfo
{
	[SerializeField]
	[AssetPath(typeof(HexMap), PathOptions.RelativeToResources)]
	public string m_path;

	[SerializeField]
	public float m_chance;
}
