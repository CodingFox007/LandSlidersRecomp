using UnityEngine;

public class TestWiggle : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, -0.1f + Random.Range(-0.01f, 0.01f));
	}
}
