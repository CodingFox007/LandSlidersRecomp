using System;
using UnityEngine;

[Serializable]
public class SpawnRulesMissionTile : SpawnRules
{
	[SerializeField]
	private bool m_spawnIfActiveMission;

	public override bool CheckTile(HexTile tile)
	{
		if (!m_spawnIfActiveMission && Singleton<Game>.Instance.MissionManager.ActiveMission)
		{
			return false;
		}
		if (!base.CheckTile(tile))
		{
			return false;
		}
		return true;
	}
}
