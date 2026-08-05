using System;
using UnityEngine;

[Serializable]
public class GUISlideImageIn : MonoBehaviour
{
	public Texture2D grumpy;

	private LTRect grumpyRect;

	public virtual void Start()
	{
		grumpyRect = new LTRect(-grumpy.width, 0.5f * (float)Screen.height - (float)grumpy.height / 2f, grumpy.width, grumpy.height);
		LeanTween.move(grumpyRect, new Vector2(0.5f * (float)Screen.width - (float)grumpy.width / 2f, grumpyRect.rect.y), 1f).setEase(LeanTweenType.easeOutQuad);
	}

	public virtual void OnGUI()
	{
		GUI.DrawTexture(grumpyRect.rect, grumpy);
	}

	public virtual void Main()
	{
	}
}
