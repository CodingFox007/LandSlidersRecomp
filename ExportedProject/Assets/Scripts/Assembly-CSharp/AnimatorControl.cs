using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
	[SerializeField]
	private Animator m_animator;

	public void SetBoolTrue(string str)
	{
		m_animator.SetBool(str, true);
	}

	public void SetBoolFalse(string str)
	{
		m_animator.SetBool(str, false);
	}
}
