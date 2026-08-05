using UnityEngine;
using UnityEngine.EventSystems;

public class PinPrompt : MonoBehaviour, IEventSystemHandler
{
	public void Trigger()
	{
		Singleton<Game>.Instance.UIController.ShowPinPrompt();
	}
}
