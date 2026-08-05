using System;
using CompilerGenerated;
using UnityEngine;

[Serializable]
public class GeneralBasics2dJS : MonoBehaviour
{
	public Texture2D dudeTexture;

	public GameObject prefabParticles;

	public virtual void Start()
	{
		GameObject gameObject = createSpriteDude("avatarRotate", new Vector3(-2.51208f, 10.7119f, -14.37754f), true);
		GameObject gameObject2 = createSpriteDude("avatarScale", new Vector3(2.51208f, 10.2119f, -14.37754f), true);
		GameObject gameObject3 = createSpriteDude("avatarMove", new Vector3(-3.1208f, 7.100643f, -14.37754f), true);
		LeanTween.rotateAround(gameObject, Vector3.forward, -360f, 5f);
		LeanTween.scale(gameObject2, new Vector3(1.7f, 1.7f, 1.7f), 5f).setEase(LeanTweenType.easeOutBounce);
		LeanTween.moveX(gameObject2, gameObject2.transform.position.x + 1f, 5f).setEase(LeanTweenType.easeOutBounce);
		LeanTween.move(gameObject3, gameObject3.transform.position + new Vector3(1.7f, 0f, 0f), 2f).setEase(LeanTweenType.easeInQuad);
		LeanTween.move(gameObject3, gameObject3.transform.position + new Vector3(2f, -1f, 0f), 2f).setDelay(3f);
		LeanTween.scale(gameObject2, new Vector3(0.2f, 0.2f, 0.2f), 1f).setDelay(7f).setEase(LeanTweenType.easeInOutCirc)
			.setRepeat(5)
			.setLoopPingPong();
		LeanTween.delayedCall(this.gameObject, 0.2f, advancedExamples);
	}

	public virtual GameObject createSpriteDude(string name, Vector3 pos, bool hasParticles)
	{
		GameObject gameObject = new GameObject(name);
		SpriteRenderer spriteRenderer = (SpriteRenderer)gameObject.AddComponent(typeof(SpriteRenderer));
		gameObject.GetComponent<Renderer>().material.color = new Color(0f, 0.70980394f, 1f);
		spriteRenderer.sprite = Sprite.Create(dudeTexture, new Rect(0f, 0f, 256f, 256f), new Vector2(0.5f, 0f), 256f);
		gameObject.transform.position = pos;
		if (hasParticles)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(prefabParticles, Vector3.zero, prefabParticles.transform.rotation) as GameObject;
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = prefabParticles.transform.position;
		}
		return gameObject;
	}

	public virtual void advancedExamples()
	{
		LeanTween.delayedCall(gameObject, 14f, (Action)delegate
		{
			for (int i = 0; i < 10; i++)
			{
				GameObject gameObject = new GameObject("rotator" + i);
				gameObject.transform.position = new Vector3(2.71208f, 7.100643f, -12.37754f);
				GameObject gameObject2 = createSpriteDude("dude" + i, new Vector3(-2.51208f, 7.100643f, -14.37754f), false);
				gameObject2.transform.parent = gameObject.transform;
				gameObject2.transform.localPosition = new Vector3(0f, 0.5f, 0.5f * (float)i);
				gameObject2.transform.localScale = new Vector3(0f, 0f, 0f);
				LeanTween.scale(gameObject2, new Vector3(0.65f, 0.65f, 0.65f), 1f).setDelay((float)i * 0.2f).setEase(LeanTweenType.easeOutBack);
				float num = LeanTween.tau / 10f * (float)i;
				float r = Mathf.Sin(num + LeanTween.tau * 0f / 3f) * 0.5f + 0.5f;
				float g = Mathf.Sin(num + LeanTween.tau * 1f / 3f) * 0.5f + 0.5f;
				float b = Mathf.Sin(num + LeanTween.tau * 2f / 3f) * 0.5f + 0.5f;
				Color to = new Color(r, g, b);
				LeanTween.color(gameObject2, to, 0.3f).setDelay(1.2f + (float)i * 0.4f);
				LeanTween.moveLocalZ(gameObject2, -2f, 0.3f).setDelay(1.2f + (float)i * 0.4f).setEase(LeanTweenType.easeSpring)
					.setOnComplete(_0024adaptor_0024__GeneralBasicJS_0024callable1_002460_33___0024Action_00240.Adapt((GameObject rot) => LeanTween.rotateAround(rot, Vector3.forward, -1080f, 12f)))
					.setOnCompleteParam(gameObject);
				LeanTween.moveLocalY(gameObject2, 1.17f, 1.2f).setDelay(5f + (float)i * 0.2f).setLoopPingPong(1)
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

	internal void _0024advancedExamples_0024closure_00247()
	{
		for (int i = 0; i < 10; i++)
		{
			GameObject gameObject = new GameObject("rotator" + i);
			gameObject.transform.position = new Vector3(2.71208f, 7.100643f, -12.37754f);
			GameObject gameObject2 = createSpriteDude("dude" + i, new Vector3(-2.51208f, 7.100643f, -14.37754f), false);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = new Vector3(0f, 0.5f, 0.5f * (float)i);
			gameObject2.transform.localScale = new Vector3(0f, 0f, 0f);
			LeanTween.scale(gameObject2, new Vector3(0.65f, 0.65f, 0.65f), 1f).setDelay((float)i * 0.2f).setEase(LeanTweenType.easeOutBack);
			float num = LeanTween.tau / 10f * (float)i;
			float r = Mathf.Sin(num + LeanTween.tau * 0f / 3f) * 0.5f + 0.5f;
			float g = Mathf.Sin(num + LeanTween.tau * 1f / 3f) * 0.5f + 0.5f;
			float b = Mathf.Sin(num + LeanTween.tau * 2f / 3f) * 0.5f + 0.5f;
			Color to = new Color(r, g, b);
			LeanTween.color(gameObject2, to, 0.3f).setDelay(1.2f + (float)i * 0.4f);
			LeanTween.moveLocalZ(gameObject2, -2f, 0.3f).setDelay(1.2f + (float)i * 0.4f).setEase(LeanTweenType.easeSpring)
				.setOnComplete(_0024adaptor_0024__GeneralBasicJS_0024callable1_002460_33___0024Action_00240.Adapt((GameObject rot) => LeanTween.rotateAround(rot, Vector3.forward, -1080f, 12f)))
				.setOnCompleteParam(gameObject);
			LeanTween.moveLocalY(gameObject2, 1.17f, 1.2f).setDelay(5f + (float)i * 0.2f).setLoopPingPong(1)
				.setEase(LeanTweenType.easeInOutQuad);
			LeanTween.alpha(gameObject2, 0f, 0.6f).setDelay(9.2f + (float)i * 0.4f).setDestroyOnComplete(true)
				.setOnComplete(_0024adaptor_0024__GeneralBasicJS_0024callable2_002470_33___0024Action_00241.Adapt(delegate(GameObject rot)
				{
					UnityEngine.Object.Destroy(rot);
				}))
				.setOnCompleteParam(gameObject);
		}
	}

	internal LTDescr _0024_0024advancedExamples_0024closure_00247_0024closure_00248(GameObject rot)
	{
		return LeanTween.rotateAround(rot, Vector3.forward, -1080f, 12f);
	}

	internal void _0024_0024advancedExamples_0024closure_00247_0024closure_00249(GameObject rot)
	{
		UnityEngine.Object.Destroy(rot);
	}
}
