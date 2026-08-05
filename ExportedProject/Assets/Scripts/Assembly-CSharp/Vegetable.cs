using System;
using HeavyDutyInspector;
using UnityEngine;

[Serializable]
public class Vegetable
{
	public string name;

	public float plantHeight;

	public VegetableType vegetableType;

	public Texture2D image;

	[ComponentSelection]
	public Component component;
}
