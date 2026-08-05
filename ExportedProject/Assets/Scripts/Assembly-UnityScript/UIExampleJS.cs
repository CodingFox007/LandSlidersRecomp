using System;
using CompilerGenerated;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIExampleJS : MonoBehaviour
{
	public RectTransform button;

	public virtual void Start()
	{
		Debug.Log("For better examples unzip the 4.6_Examples folder!");
		if (button == null)
		{
			Debug.LogError("Button not assigned! Create a new button via Hierarchy->Create->UI->Button. Then assign it to the button variable");
			return;
		}
		LeanTween.value(button.gameObject, button.anchoredPosition, new Vector2(200f, 100f), 1f).setOnUpdateVector3(_0024adaptor_0024__UIExampleJS_0024callable3_002416_25___0024Action_00242.Adapt((Vector3 val) => button.anchoredPosition = new Vector2(val.x, val.y)));
		LeanTween.value(gameObject, 1f, 0.5f, 1f).setOnUpdate(delegate(float volume)
		{
			Debug.Log("volume:" + volume);
		});
		LeanTween.value(gameObject, gameObject.transform.position, gameObject.transform.position + new Vector3(0f, 1f, 0f), 1f).setOnUpdateVector3(_0024adaptor_0024__UIExampleJS_0024callable5_002428_25___0024Action_00243.Adapt((Vector3 val) => gameObject.transform.position = val));
		LeanTween.value(gameObject, Color.red, Color.green, 1f).setOnUpdateColor(delegate(Color val)
		{
			Image image = (Image)button.gameObject.GetComponent(typeof(Image));
			image.color = val;
		});
		LeanTween.move(button, new Vector3(200f, -100f, 0f), 1f).setDelay(1f);
		LeanTween.rotateAround(button, Vector3.forward, 90f, 1f).setDelay(2f);
		LeanTween.scale(button, button.localScale * 2f, 1f).setDelay(3f);
		LeanTween.rotateAround(button, Vector3.forward, -90f, 1f).setDelay(4f).setEase(LeanTweenType.easeInOutElastic);
	}

	public virtual void Main()
	{
	}

	internal Vector2 _0024Start_0024closure_002413(Vector3 val)
	{
		return button.anchoredPosition = new Vector2(val.x, val.y);
	}

	internal void _0024Start_0024closure_002414(float volume)
	{
		Debug.Log("volume:" + volume);
	}

	internal Vector3 _0024Start_0024closure_002415(Vector3 val)
	{
		return gameObject.transform.position = val;
	}

	internal void _0024Start_0024closure_002416(Color val)
	{
		Image image = (Image)button.gameObject.GetComponent(typeof(Image));
		image.color = val;
	}
}
