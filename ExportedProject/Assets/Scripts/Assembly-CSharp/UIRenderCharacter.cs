using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIRenderCharacter : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private string m_characterId;

	private Character m_character;

	private Character m_characterPrefab;

	[SerializeField]
	private Camera m_cameraPrefab;

	private Camera m_camera;

	[SerializeField]
	private RawImage m_image;

	[SerializeField]
	private Vector3 m_characterPos;

	[SerializeField]
	private Vector3 m_cameraOffset;

	private Quaternion m_cameraRot;

	private Vector3 m_cameraPos;

	[SerializeField]
	private bool m_ignoreLocked;

	[SerializeField]
	private Rigidbody m_characterAnchorPrefab;

	private Rigidbody m_characterAnchor;

	[SerializeField]
	private RandomRange m_randomForce = new RandomRange(500f, 5000f);

	[SerializeField]
	private Material m_lockedMaterial;

	[SerializeField]
	private Material m_goldMaterial;

	[SerializeField]
	private bool m_useCurrentCharacter;

	private bool m_golden;

	private RenderTexture m_texture;

	private IEnumerator m_loader;

	public Character Prefab
	{
		get
		{
			return m_characterPrefab;
		}
	}

	public string CharacterId
	{
		get
		{
			return m_characterId;
		}
		set
		{
			m_characterId = value;
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

	private void Start()
	{
	}

	public void Load()
	{
		m_texture = new RenderTexture(512, 512, 24);
		m_texture.Create();
		m_image.texture = m_texture;
		m_characterAnchor = Object.Instantiate(m_characterAnchorPrefab);
		m_characterAnchor.name = "CharacterPreviewAnchor";
		m_characterAnchor.transform.position = m_characterPos;
		m_camera = Object.Instantiate(m_cameraPrefab);
		m_camera.name = "CharacterPreviewCamera";
		m_camera.targetTexture = m_texture;
		m_cameraRot = m_camera.transform.rotation;
		m_cameraPos = m_camera.transform.position;
		if (m_useCurrentCharacter)
		{
			SetCharacter(Singleton<Game>.Instance.Player.CharacterId);
		}
		else
		{
			SetCharacter(m_characterId);
		}
	}

	public void Unload()
	{
		if (m_characterAnchor != null)
		{
			Object.Destroy(m_characterAnchor.gameObject);
		}
		if (m_camera != null)
		{
			Object.Destroy(m_camera.gameObject);
		}
		if (m_character != null)
		{
			Object.Destroy(m_character.gameObject);
		}
		if (m_texture != null)
		{
			m_texture.Release();
			Object.Destroy(m_texture);
			m_texture = null;
		}
		m_characterId = string.Empty;
		m_character = null;
	}

	public void SetCharacter(string id, bool force = false)
	{
		if ((!force && m_character != null && m_characterId == id) || m_camera == null || m_characterAnchor == null)
		{
			return;
		}
		CharacterInfo characterInfo = Singleton<Game>.Instance.Player.GetCharacterInfo(id);
		if (characterInfo != null)
		{
			m_characterId = characterInfo.Id;
			m_camera.transform.parent = null;
			if (m_character != null)
			{
				Object.Destroy(m_character.gameObject);
			}
			if (m_loader != null)
			{
				StopCoroutine(m_loader);
			}
			m_characterPrefab = null;
			m_loader = LoadCharacter(characterInfo);
			StartCoroutine(m_loader);
		}
	}

	private IEnumerator LoadCharacter(CharacterInfo info)
	{
		ResourceRequest request = Resources.LoadAsync<Character>(info.ResourcePath);
		while (!request.isDone)
		{
			yield return null;
		}
		m_characterPrefab = request.asset as Character;
		if (m_character != null)
		{
			Object.Destroy(m_character.gameObject);
		}
		if (m_characterPrefab == null)
		{
			yield break;
		}
		m_character = Utils.CreateFromPrefab(m_characterPrefab, "CharacterPreview");
		m_character.transform.position = m_characterAnchor.transform.position;
		m_character.SetupPhysics(m_characterAnchor);
		m_camera.transform.parent = m_character.transform;
		m_camera.transform.localPosition = m_cameraOffset + m_cameraPos;
		m_camera.transform.rotation = m_cameraRot;
		if (info.Locked && !m_ignoreLocked)
		{
			Renderer[] componentsInChildren = m_character.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.shadowCastingMode = ShadowCastingMode.Off;
				renderer.receiveShadows = false;
				Material[] array2 = new Material[renderer.sharedMaterials.Length];
				for (int j = 0; j < renderer.sharedMaterials.Length; j++)
				{
					if (renderer.gameObject.GetComponent<MaterialReplacer>() != null && renderer.sharedMaterials[j].name == renderer.gameObject.GetComponent<MaterialReplacer>().m_sourceMaterial.name)
					{
						array2[j] = renderer.gameObject.GetComponent<MaterialReplacer>().m_destMaterial;
					}
					else if (m_lockedMaterial != null)
					{
						array2[j] = m_lockedMaterial;
					}
				}
				renderer.sharedMaterials = array2;
			}
		}
		else if (info.IsGolden)
		{
			Renderer[] componentsInChildren2 = m_character.GetComponentsInChildren<Renderer>();
			Renderer[] array3 = componentsInChildren2;
			foreach (Renderer renderer2 in array3)
			{
				renderer2.shadowCastingMode = ShadowCastingMode.Off;
				renderer2.receiveShadows = false;
				if (m_goldMaterial != null)
				{
					Material[] array4 = new Material[renderer2.sharedMaterials.Length];
					for (int l = 0; l < renderer2.sharedMaterials.Length; l++)
					{
						array4[l] = m_goldMaterial;
					}
					renderer2.sharedMaterials = array4;
				}
			}
			AddRandomForce();
		}
		else
		{
			Renderer[] componentsInChildren3 = m_character.GetComponentsInChildren<Renderer>();
			Renderer[] array5 = componentsInChildren3;
			foreach (Renderer renderer3 in array5)
			{
				renderer3.shadowCastingMode = ShadowCastingMode.Off;
				renderer3.receiveShadows = false;
			}
			AddRandomForce();
		}
	}

	public void AddRandomForce()
	{
		Vector3 force = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
		m_randomForce.Randomize();
		force *= m_randomForce.Value;
		m_characterAnchor.AddForce(force, ForceMode.Impulse);
	}
}
