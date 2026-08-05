using UnityEngine;

public class SpawnTile : MonoBehaviour
{
	private void Start()
	{
		if (!Singleton<Game>.Instance.LevelManager.IsStartLevel && !Singleton<Game>.Instance.LevelManager.IsRetryLevel)
		{
			SetVisible(false);
		}
	}

	public void SetVisible(bool visible)
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.enabled = visible;
		}
	}

	public void PlayerLeftTile()
	{
		Singleton<Game>.Instance.UIController.HUD.HideSpawnHUD();
	}
}
