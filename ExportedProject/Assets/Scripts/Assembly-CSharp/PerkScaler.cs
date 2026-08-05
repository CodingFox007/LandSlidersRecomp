using UnityEngine;

public class PerkScaler : MonoBehaviour
{
	[SerializeField]
	private string m_perkID;

	private void Start()
	{
		float currentPerkedValue = Singleton<Game>.Instance.PerkManager.GetCurrentPerkedValue(m_perkID, 1f);
		base.transform.localScale = new Vector3(base.transform.localScale.x * currentPerkedValue, base.transform.localScale.y * currentPerkedValue, base.transform.localScale.z * currentPerkedValue);
	}

	private void Update()
	{
	}
}
