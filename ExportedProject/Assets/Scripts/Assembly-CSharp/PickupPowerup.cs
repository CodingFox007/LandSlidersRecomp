using UnityEngine;

public class PickupPowerup : Pickup
{
	[SerializeField]
	private string m_powerupId;

	protected override void CollectReward()
	{
		if (m_powerupId != string.Empty)
		{
			Singleton<Game>.Instance.Player.ActivatePowerup(m_powerupId);
			Singleton<Game>.Instance.StatsManager.GetStat("PowerupUsed").IncreaseStat(1f);
		}
	}
}
