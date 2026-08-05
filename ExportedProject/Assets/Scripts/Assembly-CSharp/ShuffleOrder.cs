using UnityEngine;

public class ShuffleOrder : MonoBehaviour
{
	[SerializeField]
	private GameObject[] mainCredits;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Shuffle()
	{
		int num = Random.Range(4, 20);
		for (int i = 0; i < num; i++)
		{
			int num2 = Random.Range(0, mainCredits.Length);
			mainCredits[num2].transform.SetAsFirstSibling();
		}
	}
}
