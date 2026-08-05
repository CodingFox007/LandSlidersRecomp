using UnityEngine;

public class UIPrizeSelectControl : MonoBehaviour
{
	public void SpawnCoins()
	{
		Singleton<Game>.Instance.UIController.PrizeSelect.SpawnCoins();
	}
}
