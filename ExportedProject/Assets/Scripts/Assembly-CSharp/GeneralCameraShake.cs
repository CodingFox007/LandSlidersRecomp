using System;
using UnityEngine;

public class GeneralCameraShake : MonoBehaviour
{
	private GameObject avatarBig;

	private float jumpIter = 9.5f;

	private AudioClip boomAudioClip;

	public AnimationCurve boomAudioCurve;

	private void Start()
	{
		avatarBig = GameObject.Find("AvatarBig");
		boomAudioClip = createAudio(boomAudioCurve, 50);
		bigGuyJump();
	}

	private void bigGuyJump()
	{
		float height = Mathf.PerlinNoise(jumpIter, 0f) * 10f;
		height = height * height * 0.3f;
		LeanTween.moveY(avatarBig, height, 1f).setEase(LeanTweenType.easeInOutQuad).setOnComplete((Action)delegate
		{
			LeanTween.moveY(avatarBig, 0f, 0.27f).setEase(LeanTweenType.easeInQuad).setOnComplete((Action)delegate
			{
				LeanTween.cancel(base.gameObject);
				float num = height * 0.2f;
				float time = 0.42f;
				float time2 = 1.6f;
				LTDescr shakeTween = LeanTween.rotateAroundLocal(base.gameObject, Vector3.right, num, time).setEase(LeanTweenType.easeShake).setLoopClamp()
					.setRepeat(-1);
				LeanTween.value(base.gameObject, num, 0f, time2).setOnUpdate(delegate(float val)
				{
					shakeTween.setTo(Vector3.right * val);
				}).setEase(LeanTweenType.easeOutQuad);
				GameObject[] array = GameObject.FindGameObjectsWithTag("Respawn");
				GameObject[] array2 = array;
				foreach (GameObject gameObject in array2)
				{
					gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 100f * height);
				}
				GameObject[] array3 = GameObject.FindGameObjectsWithTag("GameController");
				GameObject[] array4 = array3;
				foreach (GameObject gameObject2 in array4)
				{
					float z = gameObject2.transform.eulerAngles.z;
					z = ((z > 0f && z < 180f) ? 1 : (-1));
					gameObject2.GetComponent<Rigidbody>().AddForce(new Vector3(z, 0f, 0f) * 15f * height);
				}
				playAudio(boomAudioClip, base.transform.position, height * 0.2f, 0.34f);
				LeanTween.delayedCall(2f, bigGuyJump);
			});
		});
		jumpIter += 5.2f;
	}

	private AudioClip createAudio(AnimationCurve curve, int granularity)
	{
		int num = 440 * granularity;
		float[] array = new float[granularity];
		float time = curve[curve.length - 1].time;
		for (int i = 0; i < array.Length; i++)
		{
			float time2 = (float)i / (float)array.Length * time;
			array[i] = curve.Evaluate(time2);
		}
		int lengthSamples = (int)((float)num * time);
		AudioClip audioClip = AudioClip.Create(string.Empty, lengthSamples, 1, num, false);
		audioClip.SetData(array, 0);
		return audioClip;
	}

	private void playAudio(AudioClip audioClip, Vector3 pos, float volume, float pitch)
	{
		AudioSource audioSource = PlayClipAt(audioClip, pos);
		audioSource.minDistance = 1f;
		audioSource.pitch = pitch;
		audioSource.volume = volume;
	}

	private AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.position = pos;
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.Play();
		UnityEngine.Object.Destroy(gameObject, clip.length);
		return audioSource;
	}
}
