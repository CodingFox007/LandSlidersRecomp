using UnityEngine;

public class PositionChange : MonoBehaviour
{
	public void SetPositionY(float pos)
	{
		Vector3 localPosition = base.transform.localPosition;
		base.transform.localPosition = new Vector3(localPosition.x, pos, localPosition.z);
	}

	public void SetPositionX(float pos)
	{
		Vector3 localPosition = base.transform.localPosition;
		base.transform.localPosition = new Vector3(pos, localPosition.y, localPosition.z);
	}
}
