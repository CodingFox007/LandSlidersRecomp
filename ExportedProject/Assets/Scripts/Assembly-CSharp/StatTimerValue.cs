using UnityEngine;

public class StatTimerValue : StatValue
{
	public StatTimerValue()
	{
		m_active = false;
	}

	public override void Update()
	{
		if (base.IsActive)
		{
			base.Current += Time.deltaTime;
		}
	}

	public override void Reset()
	{
		m_current = 0f;
		m_active = false;
	}
}
