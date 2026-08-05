using UnityEngine;

public class PathSpline2d : MonoBehaviour
{
	public Transform[] trans;

	public Texture2D spriteTexture;

	private LTSpline cr;

	private GameObject sprite1;

	private GameObject sprite2;

	private float iter;

	private void Start()
	{
		cr = new LTSpline(trans[0].position, trans[1].position, trans[2].position, trans[3].position, trans[4].position);
		sprite1 = GameObject.Find("sprite1");
		sprite2 = GameObject.Find("sprite2");
		sprite1.AddComponent<SpriteRenderer>();
		sprite1.GetComponent<SpriteRenderer>().sprite = Sprite.Create(spriteTexture, new Rect(0f, 0f, 100f, 100f), new Vector2(50f, 50f), 10f);
		sprite2.AddComponent<SpriteRenderer>();
		sprite2.GetComponent<SpriteRenderer>().sprite = Sprite.Create(spriteTexture, new Rect(0f, 0f, 100f, 100f), new Vector2(0f, 0f), 10f);
		LTDescr lTDescr = LeanTween.moveSpline(sprite2, new Vector3[5]
		{
			Vector3.zero,
			Vector3.zero,
			new Vector3(1f, 1f, 1f),
			new Vector3(2f, 1f, 1f),
			new Vector3(2f, 1f, 1f)
		}, 1.5f).setOrientToPath2d(true);
		lTDescr.setUseEstimatedTime(true);
	}

	private void Update()
	{
		cr.place2d(sprite1.transform, iter);
		iter += Time.deltaTime * 0.1f;
		if (iter > 1f)
		{
			iter = 0f;
		}
	}

	private void OnDrawGizmos()
	{
		if (cr != null)
		{
			cr.gizmoDraw();
		}
	}
}
