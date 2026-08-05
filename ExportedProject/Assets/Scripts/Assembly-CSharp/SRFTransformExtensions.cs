using System.Collections.Generic;
using UnityEngine;

public static class SRFTransformExtensions
{
	public static IEnumerable<Transform> GetChildren(this Transform t)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			yield return t.GetChild(i);
		}
	}

	public static void ResetLocal(this Transform t)
	{
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
	}

	public static GameObject CreateChild(this Transform t, string name)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.parent = t;
		gameObject.transform.ResetLocal();
		gameObject.gameObject.layer = t.gameObject.layer;
		return gameObject;
	}

	public static void SetParentMaintainLocals(this Transform t, Transform parent)
	{
		Vector3 localPosition = t.localPosition;
		Quaternion localRotation = t.localRotation;
		Vector3 localScale = t.localScale;
		t.parent = parent;
		t.localPosition = localPosition;
		t.localRotation = localRotation;
		t.localScale = localScale;
	}

	public static void SetLocals(this Transform t, Transform from)
	{
		t.localPosition = from.localPosition;
		t.localRotation = from.localRotation;
		t.localScale = from.localScale;
	}

	public static void Match(this Transform t, Transform from)
	{
		t.position = from.position;
		t.rotation = from.rotation;
	}

	public static void DestroyChildren(this Transform t)
	{
		foreach (object item in t)
		{
			Object.Destroy(((Transform)item).gameObject);
		}
	}
}
