using UnityEngine;

public class lassoo : MonoBehaviour
{
	[SerializeField]
	private float m_rotationSpeed;

	private void Update()
	{
		Vector3 eulerAngles = base.transform.localRotation.eulerAngles;
		eulerAngles.y += m_rotationSpeed * Time.deltaTime * 60f;
		base.transform.localRotation = Quaternion.Euler(eulerAngles);
	}
}
