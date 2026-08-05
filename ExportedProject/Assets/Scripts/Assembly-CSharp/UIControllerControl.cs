using UnityEngine;

public class UIControllerControl : MonoBehaviour
{
	public void ShowChooseCharacter()
	{
		Singleton<Game>.Instance.UIController.ShowChooseCharacter();
	}

	public void ShowChooseControls()
	{
		Singleton<Game>.Instance.UIController.ShowChooseControls();
	}
}
