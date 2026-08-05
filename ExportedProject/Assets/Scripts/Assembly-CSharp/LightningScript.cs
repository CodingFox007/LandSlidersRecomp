using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.Utility;

public class LightningScript : MonoBehaviour
{
	[SerializeField]
	private GameObject[] m_bolts;

	[SerializeField]
	private string m_registryID;

	[SerializeField]
	private ChangeLighting m_lightingScript;

	[SerializeField]
	private float m_showLightingIntensityScale = 0.95f;

	[SerializeField]
	private bool m_randomiseBoltsOnRelight = true;

	[SerializeField]
	private float m_factorOnscreenHits = 1f;

	[SerializeField]
	private GameObject m_hitEffect;

	[SerializeField]
	private UnityEvent m_onOnscreenHit = new UnityEvent();

	[SerializeField]
	private UnityEvent m_onOffscreenHit = new UnityEvent();

	private GameObject m_selectedBolt;

	private GameObject m_selectedDestination;

	private bool m_isActive;

	private void Start()
	{
	}

	private void Awake()
	{
		Singleton<Game>.Instance.LevelManager.OnLevelLoadComplete += ClearEffects;
	}

	private void OnDestroy()
	{
		if (Singleton<Game>.Instance != null && Singleton<Game>.Instance.LevelManager != null)
		{
			Singleton<Game>.Instance.LevelManager.OnLevelLoadComplete -= ClearEffects;
		}
	}

	private void ClearEffects()
	{
		foreach (GameObject item in FastPoolManager.GetPool(m_hitEffect).created)
		{
			if (item != null)
			{
				ParticleSystem component = item.GetComponent<ParticleSystem>();
				if (component != null)
				{
					component.Stop();
				}
				item.SetActive(false);
			}
		}
	}

	private void Update()
	{
		if (!m_isActive)
		{
			return;
		}
		bool flag = false;
		if (m_selectedBolt != null)
		{
			flag = m_selectedBolt.activeSelf;
		}
		bool flag2 = false;
		if (m_lightingScript != null)
		{
			flag2 = m_lightingScript.GetSecondaryIntensityScale() >= m_showLightingIntensityScale;
		}
		if (m_randomiseBoltsOnRelight && !flag && flag2 && m_selectedDestination != null)
		{
			m_selectedBolt = m_bolts[Random.Range(0, m_bolts.Length)];
		}
		if (m_selectedDestination == null && m_selectedBolt != null)
		{
			m_selectedBolt.SetActive(false);
		}
		else if (m_selectedBolt != null)
		{
			m_selectedBolt.SetActive(flag2);
			if (flag2)
			{
				m_selectedBolt.transform.localPosition = m_selectedDestination.transform.position - m_selectedBolt.transform.parent.position;
			}
		}
	}

	public void OnTrigger()
	{
		m_isActive = true;
		m_selectedDestination = ObjectRegistry.GetRandom(m_registryID, Random.Range(0f, 1f) <= m_factorOnscreenHits);
		if (m_selectedDestination != null)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Singleton<Game>.Instance.VisualCamera.Camera);
			if (m_selectedDestination.GetComponent<Renderer>() != null && GeometryUtility.TestPlanesAABB(planes, m_selectedDestination.GetComponent<Renderer>().bounds))
			{
				m_onOnscreenHit.Invoke();
			}
			else
			{
				m_onOffscreenHit.Invoke();
			}
			GameObject gameObject = FastPoolManager.GetPool(m_hitEffect).FastInstantiate(m_selectedDestination.transform.position, Quaternion.identity);
			ParticleSystemPoolDestroyer component = gameObject.GetComponent<ParticleSystemPoolDestroyer>();
			if (component != null)
			{
				component.m_prefab = m_hitEffect;
				StartCoroutine(component.Start());
			}
			gameObject.GetComponent<ParticleSystem>().Play();
		}
		if (!m_randomiseBoltsOnRelight && m_bolts.Length != 0 && m_selectedDestination != null)
		{
			m_selectedBolt = m_bolts[Random.Range(0, m_bolts.Length)];
		}
	}

	public void OnUnTrigger()
	{
		if (m_selectedBolt != null)
		{
			m_selectedBolt.SetActive(false);
		}
		m_isActive = false;
	}
}
