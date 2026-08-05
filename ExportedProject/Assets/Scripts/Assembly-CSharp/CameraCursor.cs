using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;
using UnityStandardAssets.Utility;

public class CameraCursor : MonoBehaviour, ISerializationCallbackReceiver
{
	[Dictionary("m_effectsValues")]
	public List<string> m_effectPrefabs;

	[HideInInspector]
	public List<GameObject> m_effectsValues;

	private Dictionary<string, GameObject> m_effects;

	private bool m_oldTouchDown;

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		Utils.InitDictionary(m_effectPrefabs, m_effectsValues, out m_effects, "effects");
	}

	private void Start()
	{
		m_oldTouchDown = false;
		foreach (KeyValuePair<string, GameObject> effect in m_effects)
		{
			if (effect.Value == null)
			{
				continue;
			}
			FastPool pool = FastPoolManager.GetPool(effect.Value, false);
			if (pool != null)
			{
				foreach (GameObject item in pool.cache)
				{
					ParticleSystemPoolDestroyer component = item.GetComponent<ParticleSystemPoolDestroyer>();
					if (component != null)
					{
						component.m_prefab = effect.Value;
					}
				}
			}
			else
			{
				Debug.LogError("TouchDown Effect: " + effect.Value.name + "  missing pool");
			}
		}
	}

	private void Update()
	{
		if (Singleton<Game>.Instance.IsTouchDown && !m_oldTouchDown)
		{
			string surfaceType = string.Empty;
			float height = float.MaxValue;
			Vector3 vector = CalculateTouchPos(out surfaceType, out height);
			if (height == float.MaxValue)
			{
				surfaceType = "Air";
				vector = Singleton<Game>.Instance.TouchDownPos;
			}
			if (m_effects.ContainsKey(surfaceType))
			{
				GameObject gameObject = m_effects[surfaceType];
				if (gameObject != null)
				{
					Quaternion rotation = gameObject.transform.rotation;
					Vector3 position = vector + gameObject.transform.position;
					GameObject gameObject2 = FastPoolManager.GetPool(gameObject).FastInstantiate(position, rotation);
					ParticleSystemPoolDestroyer component = gameObject2.GetComponent<ParticleSystemPoolDestroyer>();
					if (component != null)
					{
						component.m_prefab = gameObject;
						StartCoroutine(component.Start());
					}
					gameObject2.GetComponent<ParticleSystem>().Play();
					ParticleSystem[] componentsInChildren = gameObject2.GetComponentsInChildren<ParticleSystem>();
					ParticleSystem[] array = componentsInChildren;
					foreach (ParticleSystem particleSystem in array)
					{
						particleSystem.startColor = Singleton<Game>.Instance.Player.CharacterInfo.ParticleColor;
					}
				}
			}
		}
		m_oldTouchDown = Singleton<Game>.Instance.IsTouchDown;
	}

	private Vector3 CalculateTouchPos(out string surfaceType, out float height)
	{
		Vector3 result = Vector3.zero;
		surfaceType = "Air";
		height = float.MaxValue;
		int num = 1 << LayerMask.NameToLayer("Ground");
		num |= 1 << LayerMask.NameToLayer("Water");
		Ray ray = Singleton<Game>.Instance.RayCastCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 100f, num))
		{
			result = hitInfo.point;
			surfaceType = hitInfo.collider.tag;
			height = hitInfo.point.y;
		}
		return result;
	}
}
