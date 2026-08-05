using UnityEngine;

public class TempTestingCancel : MonoBehaviour
{
	public bool isTweening;

	public bool tweenOverride;

	private LTDescr tween;

	private void Start()
	{
		tween = LeanTween.move(base.gameObject, base.transform.position + Vector3.one * 3f, Random.Range(2, 2)).setRepeat(-1).setLoopClamp();
	}

	public void Update()
	{
		if (tween != null)
		{
			isTweening = LeanTween.isTweening(base.gameObject);
			if (tweenOverride)
			{
				LeanTween.cancel(base.gameObject);
			}
		}
	}
}
