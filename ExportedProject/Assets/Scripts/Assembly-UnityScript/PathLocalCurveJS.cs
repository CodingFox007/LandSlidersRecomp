using System;
using Boo.Lang.Runtime;
using UnityEngine;

[Serializable]
public class PathLocalCurveJS : MonoBehaviour
{
	public AnimationCurve customAnimationCurve;

	public Transform pt1;

	public Transform pt2;

	public Transform pt3;

	public Transform pt4;

	private Transform containingSphere;

	private LTSpline spline;

	private GameObject ltLogo;

	public virtual void Start()
	{
		ltLogo = GameObject.Find("LeanTweenLogo");
		containingSphere = GameObject.Find("ContaingCube").transform;
		Vector3[] array = new Vector3[6] { pt1.position, pt1.position, pt2.position, pt3.position, pt4.position, pt4.position };
		spline = new LTSpline(array);
		LeanTween.moveSplineLocal(ltLogo, array, 3f).setEase(LeanTweenType.easeInQuad).setOrientToPath(true)
			.setRepeat(-1);
	}

	public virtual void Update()
	{
		float y = containingSphere.transform.eulerAngles.y + Time.deltaTime * 3f;
		Vector3 eulerAngles = containingSphere.transform.eulerAngles;
		float num = (eulerAngles.y = y);
		Vector3 vector = (containingSphere.transform.eulerAngles = eulerAngles);
	}

	public virtual void OnDrawGizmos()
	{
		if (!RuntimeServices.EqualityOperator(spline, null))
		{
			spline.gizmoDraw(1f);
		}
	}

	public virtual void Main()
	{
	}
}
