using System;
using System.Collections.Generic;
using UnityEngine;

public class LeanAudio : MonoBehaviour
{
	public static float MIN_FREQEUNCY_PERIOD = 1E-05f;

	public static int PROCESSING_ITERATIONS_MAX = 50000;

	public static List<float> generatedWaveDistances;

	public static LeanAudioOptions options()
	{
		return new LeanAudioOptions();
	}

	public static AudioClip createAudio(AnimationCurve volume, AnimationCurve frequency, LeanAudioOptions options = null)
	{
		if (options == null)
		{
			options = new LeanAudioOptions();
		}
		float[] wave = createAudioWave(volume, frequency, options);
		return createAudioFromWave(wave, options);
	}

	private static float[] createAudioWave(AnimationCurve volume, AnimationCurve frequency, LeanAudioOptions options)
	{
		float time = volume[volume.length - 1].time;
		List<float> list = new List<float>();
		generatedWaveDistances = new List<float>();
		float num = 0f;
		for (int i = 0; i < PROCESSING_ITERATIONS_MAX; i++)
		{
			float num2 = frequency.Evaluate(num);
			if (num2 < MIN_FREQEUNCY_PERIOD)
			{
				num2 = MIN_FREQEUNCY_PERIOD;
			}
			float num3 = volume.Evaluate(num + 0.5f * num2);
			if (options.vibrato != null)
			{
				for (int j = 0; j < options.vibrato.Length; j++)
				{
					float num4 = Mathf.Abs(Mathf.Sin(1.5708f + num * (1f / options.vibrato[j][0]) * (float)Math.PI));
					float num5 = 1f - options.vibrato[j][1];
					num4 = options.vibrato[j][1] + num5 * num4;
					num3 *= num4;
				}
			}
			if (num + 0.5f * num2 >= time)
			{
				break;
			}
			generatedWaveDistances.Add(num2);
			num += num2;
			list.Add(num);
			list.Add((i % 2 != 0) ? num3 : (0f - num3));
			if (i >= PROCESSING_ITERATIONS_MAX - 1)
			{
				Debug.LogError("LeanAudio has reached it's processing cap. To avoid this error increase the number of iterations ex: LeanAudio.PROCESSING_ITERATIONS_MAX = " + PROCESSING_ITERATIONS_MAX * 2);
			}
		}
		float[] array = new float[list.Count];
		for (int k = 0; k < array.Length; k++)
		{
			array[k] = list[k];
		}
		return array;
	}

	private static AudioClip createAudioFromWave(float[] wave, LeanAudioOptions options)
	{
		float num = wave[wave.Length - 2];
		float[] array = new float[(int)((float)options.frequencyRate * num)];
		int num2 = 0;
		float num3 = wave[num2];
		float num4 = 0f;
		float num5 = wave[num2];
		float num6 = wave[num2 + 1];
		for (int i = 0; i < array.Length; i++)
		{
			float num7 = (float)i / (float)options.frequencyRate;
			if (num7 > wave[num2])
			{
				num4 = wave[num2];
				num2 += 2;
				num3 = wave[num2] - wave[num2 - 2];
				num6 = wave[num2 + 1];
			}
			num5 = num7 - num4;
			float num8 = num5 / num3;
			float num9 = Mathf.Sin(num8 * (float)Math.PI);
			num9 *= num6;
			array[i] = num9;
		}
		int lengthSamples = array.Length;
		AudioClip audioClip = AudioClip.Create("Generated Audio", lengthSamples, 1, options.frequencyRate, false);
		audioClip.SetData(array, 0);
		return audioClip;
	}

	public static AudioClip generateAudioFromCurve(AnimationCurve curve, int frequencyRate = 44100)
	{
		float time = curve[curve.length - 1].time;
		float num = time;
		float[] array = new float[(int)((float)frequencyRate * num)];
		for (int i = 0; i < array.Length; i++)
		{
			float time2 = (float)i / (float)frequencyRate;
			array[i] = curve.Evaluate(time2);
		}
		int lengthSamples = array.Length;
		AudioClip audioClip = AudioClip.Create("Generated Audio", lengthSamples, 1, frequencyRate, false);
		audioClip.SetData(array, 0);
		return audioClip;
	}

	public static void playAudio(AudioClip audio, Vector3 pos, float volume, float pitch)
	{
		AudioSource audioSource = playClipAt(audio, pos);
		audioSource.minDistance = 1f;
		audioSource.pitch = pitch;
		audioSource.volume = volume;
	}

	public static AudioSource playClipAt(AudioClip clip, Vector3 pos)
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.position = pos;
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.Play();
		UnityEngine.Object.Destroy(gameObject, clip.length);
		return audioSource;
	}

	public static void printOutAudioClip(AudioClip audioClip, ref AnimationCurve curve, float scaleX = 1f)
	{
		float[] array = new float[audioClip.samples * audioClip.channels];
		audioClip.GetData(array, 0);
		int i = 0;
		Keyframe[] array2 = new Keyframe[array.Length];
		for (; i < array.Length; i++)
		{
			array2[i] = new Keyframe((float)i * scaleX, array[i]);
		}
		curve = new AnimationCurve(array2);
	}
}
