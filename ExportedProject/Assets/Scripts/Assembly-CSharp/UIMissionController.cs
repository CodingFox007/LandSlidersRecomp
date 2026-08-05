using UnityEngine;

public class UIMissionController : MonoBehaviour
{
	public void MissionCollectReward()
	{
		Singleton<Game>.Instance.MissionManager.CollectReward(Singleton<Game>.Instance.UIController.HUD.MissionPassedIdx);
	}

	public void UnPauseGame()
	{
		Time.timeScale = 1f;
	}
}
