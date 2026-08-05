using UnityEngine;

public class ExampleSpline : MonoBehaviour
{
	public Transform[] trans;

	private LTSpline spline;

	private GameObject ltLogo;

	private GameObject ltLogo2;

	private float iter;

	private void Start()
	{
		spline = new LTSpline(trans[0].position, trans[1].position, trans[2].position, trans[3].position, trans[4].position);
		ltLogo = GameObject.Find("LeanTweenLogo1");
		ltLogo2 = GameObject.Find("LeanTweenLogo2");
		LeanTween.moveSpline(ltLogo2, spline.pts, 1f).setEase(LeanTweenType.easeInOutQuad).setLoopPingPong()
			.setOrientToPath(true);
		LTDescr lTDescr = LeanTween.moveSpline(ltLogo2, new Vector3[5]
		{
			Vector3.zero,
			Vector3.zero,
			new Vector3(1f, 1f, 1f),
			new Vector3(2f, 1f, 1f),
			new Vector3(2f, 1f, 1f)
		}, 1.5f);
		lTDescr.setUseEstimatedTime(true);
	}

	private void Update()
	{
		ltLogo.transform.position = spline.point(iter);
		iter += Time.deltaTime * 0.1f;
		if (iter > 1f)
		{
			iter = 0f;
		}
	}

	private void OnDrawGizmos()
	{
		if (spline != null)
		{
			spline.gizmoDraw();
		}
	}
}
