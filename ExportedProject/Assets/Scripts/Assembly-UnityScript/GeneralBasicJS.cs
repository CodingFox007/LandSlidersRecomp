using System;
using CompilerGenerated;
using UnityEngine;

[Serializable]
public class GeneralBasicJS : MonoBehaviour
{
	public GameObject prefabAvatar;

	public virtual void Start()
	{
		GameObject gameObject = GameObject.Find("AvatarRotate");
		GameObject gameObject2 = GameObject.Find("AvatarScale");
		GameObject gameObject3 = GameObject.Find("AvatarMove");
		LeanTween.rotateAround(gameObject, Vector3.forward, 360f, 5f);
		LeanTween.scale(gameObject2, new Vector3(1.7f, 1.7f, 1.7f), 5f).setEase(LeanTweenType.easeOutBounce);
		LeanTween.moveX(gameObject2, gameObject2.transform.position.x + 5f, 5f).setEase(LeanTweenType.easeOutBounce);
		LeanTween.move(gameObject3, gameObject3.transform.position + new Vector3(-9f, 0f, 1f), 2f).setEase(LeanTweenType.easeInQuad);
		LeanTween.move(gameObject3, gameObject3.transform.position + new Vector3(-6f, 0f, 1f), 2f).setDelay(3f);
		LeanTween.scale(gameObject2, new Vector3(0.2f, 0.2f, 0.2f), 1f).setDelay(7f).setEase(LeanTweenType.easeInOutCirc)
			.setRepeat(5)
			.setLoopPingPong();
		LeanTween.delayedCall(this.gameObject, 0.2f, advancedExamples);
	}

	public virtual void advancedExamples()
	{
		LeanTween.delayedCall(gameObject, 14f, (Action)delegate
		{
			for (int i = 0; i < 10; i++)
			{
				GameObject gameObject = new GameObject("rotator" + i);
				gameObject.transform.position = new Vector3(10.2f, 2.85f, 0f);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(prefabAvatar, Vector3.zero, prefabAvatar.transform.rotation) as GameObject;
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localPosition = new Vector3(0f, 1.5f, 2.5f * (float)i);
				gameObject2.transform.localScale = new Vector3(0f, 0f, 0f);
				LeanTween.scale(gameObject2, new Vector3(0.65f, 0.65f, 0.65f), 1f).setDelay((float)i * 0.2f).setEase(LeanTweenType.easeOutBack);
				float num = LeanTween.tau / 10f * (float)i;
				float r = Mathf.Sin(num + LeanTween.tau * 0f / 3f) * 0.5f + 0.5f;
				float g = Mathf.Sin(num + LeanTween.tau * 1f / 3f) * 0.5f + 0.5f;
				float b = Mathf.Sin(num + LeanTween.tau * 2f / 3f) * 0.5f + 0.5f;
				Color to = new Color(r, g, b);
				LeanTween.color(gameObject2, to, 0.3f).setDelay(1.2f + (float)i * 0.4f);
				LeanTween.moveZ(gameObject2, 0f, 0.3f).setDelay(1.2f + (float)i * 0.4f).setEase(LeanTweenType.easeSpring)
					.setOnComplete(_0024adaptor_0024__GeneralBasicJS_0024callable1_002460_33___0024Action_00240.Adapt((GameObject rot) => LeanTween.rotateAround(rot, Vector3.forward, -1080f, 12f)))
					.setOnCompleteParam(gameObject);
				LeanTween.moveLocalY(gameObject2, 4f, 1.2f).setDelay(5f + (float)i * 0.2f).setLoopPingPong()
					.setRepeat(2)
					.setEase(LeanTweenType.easeInOutQuad);
				LeanTween.alpha(gameObject2, 0f, 0.6f).setDelay(9.2f + (float)i * 0.4f).setDestroyOnComplete(true)
					.setOnComplete(_0024adaptor_0024__GeneralBasicJS_0024callable2_002470_33___0024Action_00241.Adapt(delegate(GameObject rot)
					{
						UnityEngine.Object.Destroy(rot);
					}))
					.setOnCompleteParam(gameObject);
			}
		}).setOnCompleteOnStart(true).setRepeat(-1);
	}

	public virtual void Main()
	{
	}

	internal void _0024advancedExamples_0024closure_00244()
	{
		for (int i = 0; i < 10; i++)
		{
			GameObject gameObject = new GameObject("rotator" + i);
			gameObject.transform.position = new Vector3(10.2f, 2.85f, 0f);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(prefabAvatar, Vector3.zero, prefabAvatar.transform.rotation) as GameObject;
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 1.5f, 2.5f * (float)i);
			gameObject2.transform.localScale = new Vector3(0f, 0f, 0f);
			LeanTween.scale(gameObject2, new Vector3(0.65f, 0.65f, 0.65f), 1f).setDelay((float)i * 0.2f).setEase(LeanTweenType.easeOutBack);
			float num = LeanTween.tau / 10f * (float)i;
			float r = Mathf.Sin(num + LeanTween.tau * 0f / 3f) * 0.5f + 0.5f;
			float g = Mathf.Sin(num + LeanTween.tau * 1f / 3f) * 0.5f + 0.5f;
			float b = Mathf.Sin(num + LeanTween.tau * 2f / 3f) * 0.5f + 0.5f;
			Color to = new Color(r, g, b);
			LeanTween.color(gameObject2, to, 0.3f).setDelay(1.2f + (float)i * 0.4f);
			LeanTween.moveZ(gameObject2, 0f, 0.3f).setDelay(1.2f + (float)i * 0.4f).setEase(LeanTweenType.easeSpring)
				.setOnComplete(_0024adaptor_0024__GeneralBasicJS_0024callable1_002460_33___0024Action_00240.Adapt((GameObject rot) => LeanTween.rotateAround(rot, Vector3.forward, -1080f, 12f)))
				.setOnCompleteParam(gameObject);
			LeanTween.moveLocalY(gameObject2, 4f, 1.2f).setDelay(5f + (float)i * 0.2f).setLoopPingPong()
				.setRepeat(2)
				.setEase(LeanTweenType.easeInOutQuad);
			LeanTween.alpha(gameObject2, 0f, 0.6f).setDelay(9.2f + (float)i * 0.4f).setDestroyOnComplete(true)
				.setOnComplete(_0024adaptor_0024__GeneralBasicJS_0024callable2_002470_33___0024Action_00241.Adapt(delegate(GameObject rot)
				{
					UnityEngine.Object.Destroy(rot);
				}))
				.setOnCompleteParam(gameObject);
		}
	}

	internal LTDescr _0024_0024advancedExamples_0024closure_00244_0024closure_00245(GameObject rot)
	{
		return LeanTween.rotateAround(rot, Vector3.forward, -1080f, 12f);
	}

	internal void _0024_0024advancedExamples_0024closure_00244_0024closure_00246(GameObject rot)
	{
		UnityEngine.Object.Destroy(rot);
	}
}
