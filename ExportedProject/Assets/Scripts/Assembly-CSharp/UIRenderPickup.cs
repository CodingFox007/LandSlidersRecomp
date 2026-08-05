using UnityEngine;
using UnityEngine.Rendering;

public class UIRenderPickup : MonoBehaviour
{
	[SerializeField]
	private Pickup m_pickupPrefab;

	private Pickup m_pickup;

	[SerializeField]
	private float m_scale = 25f;

	[SerializeField]
	private Vector3 m_rotation;

	[SerializeField]
	private Material m_lockedMaterial;

	[SerializeField]
	private Material m_goldMaterial;

	[SerializeField]
	private Material m_replaceSourceMaterialPickup;

	[SerializeField]
	private Material m_replaceDestMaterialPickup;

	private bool m_golden;

	public Pickup Prefab
	{
		get
		{
			return m_pickupPrefab;
		}
	}

	public bool IsGolden
	{
		get
		{
			return m_golden;
		}
		set
		{
			m_golden = value;
		}
	}

	public bool IsVisible
	{
		set
		{
			if (m_pickup == null)
			{
				return;
			}
			Renderer[] componentsInChildren = m_pickup.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if ((bool)renderer)
				{
					renderer.enabled = value;
				}
			}
		}
	}

	private void Start()
	{
	}

	public void SetPickup(Pickup prefab, bool animate = false, bool force = false)
	{
		if (!force && m_pickupPrefab == prefab && m_pickup != null)
		{
			return;
		}
		if (m_pickup != null)
		{
			Object.Destroy(m_pickup.gameObject);
		}
		m_pickupPrefab = prefab;
		if (!(m_pickupPrefab == null))
		{
			m_pickup = Utils.CreateFromPrefab(m_pickupPrefab, m_pickupPrefab.name);
			m_pickup.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));
			m_pickup.transform.parent = base.transform;
			m_pickup.transform.localPosition = Vector3.zero;
			m_pickup.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
			m_pickup.transform.rotation = Quaternion.Euler(m_rotation);
			m_pickup.ShadowEnabled = false;
			ParticleSystem[] componentsInChildren = m_pickup.GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.gameObject.SetActive(false);
			}
			Collider[] componentsInChildren2 = m_pickup.GetComponentsInChildren<Collider>();
			Collider[] array2 = componentsInChildren2;
			foreach (Collider collider in array2)
			{
				collider.enabled = false;
			}
			Animator component = m_pickup.GetComponent<Animator>();
			component.enabled = false;
			m_pickup.m_onLoad.AddListener(OnPickupLoad);
			m_pickup.m_onLoad.Invoke();
		}
	}

	private void SetupPickup(CharacterInfo character)
	{
		GameObject replacementPickup = character.ReplacementPickup;
		if (!(m_pickupPrefab == replacementPickup.GetComponent<Pickup>()) || IsGolden != character.IsGolden)
		{
			return;
		}
		if (character.Locked)
		{
			Renderer[] componentsInChildren = m_pickup.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (renderer.enabled)
				{
					Material[] array2 = new Material[renderer.sharedMaterials.Length];
					for (int j = 0; j < renderer.sharedMaterials.Length; j++)
					{
						array2[j] = m_lockedMaterial;
					}
					renderer.sharedMaterials = array2;
				}
			}
		}
		else
		{
			if (!character.IsGolden)
			{
				return;
			}
			Renderer[] componentsInChildren2 = m_pickup.GetComponentsInChildren<Renderer>();
			Renderer[] array3 = componentsInChildren2;
			foreach (Renderer renderer2 in array3)
			{
				if (renderer2.enabled)
				{
					Material[] array4 = new Material[renderer2.sharedMaterials.Length];
					for (int l = 0; l < renderer2.sharedMaterials.Length; l++)
					{
						array4[l] = m_goldMaterial;
					}
					renderer2.sharedMaterials = array4;
				}
			}
		}
	}

	private void OnPickupLoad()
	{
		SetupPickup(Singleton<Game>.Instance.Player.GetCharacterInfoForRandomCharacter());
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			SetupPickup(availableCharacter);
		}
		Renderer[] componentsInChildren = m_pickup.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if ((bool)renderer)
			{
				renderer.shadowCastingMode = ShadowCastingMode.Off;
				renderer.receiveShadows = false;
				if (renderer.sharedMaterial == m_replaceSourceMaterialPickup)
				{
					renderer.sharedMaterial = m_replaceDestMaterialPickup;
				}
			}
		}
	}

	private void Update()
	{
	}
}
