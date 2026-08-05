using System;
using HeavyDutyInspector;
using UnityEngine;

[Serializable]
public class ExitDrawable
{
	public string m_name;

	[Range(0f, 1f)]
	public float m_chance = 1f;

	[SerializeField]
	[AssetPath(typeof(GameObject), PathOptions.RelativeToResources)]
	public string m_path;

	[HideInInspector]
	public UnityEngine.Object m_assetPrefab;

	[HideInInspector]
	public bool m_isLoading;

	[HideInInspector]
	public GameObject m_obj;

	public float m_drawHeight;

	public float m_minHeight;

	public float m_maxHeight;

	public Vector3 m_offset;
}
