using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class ParticleSystemPoolDestroyer : MonoBehaviour
	{
		public GameObject m_prefab;

		public float minDuration = 8f;

		public float maxDuration = 10f;

		private float m_MaxLifetime;

		private bool m_EarlyStop;

		public IEnumerator Start()
		{
			ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = systems;
			foreach (ParticleSystem particleSystem in array)
			{
				m_MaxLifetime = Mathf.Max(particleSystem.startLifetime, m_MaxLifetime);
			}
			float stopTime = Time.time + Random.Range(minDuration, maxDuration);
			while (Time.time < stopTime || m_EarlyStop)
			{
				yield return null;
			}
			yield return new WaitForSeconds(m_MaxLifetime);
			if (m_prefab != null)
			{
				FastPool pool = FastPoolManager.GetPool(m_prefab, false);
				if (pool != null && this != null)
				{
					pool.FastDestroy(base.gameObject);
				}
			}
			else if (this != null)
			{
				Object.Destroy(base.gameObject);
			}
		}

		public void Stop()
		{
			m_EarlyStop = true;
		}
	}
}
