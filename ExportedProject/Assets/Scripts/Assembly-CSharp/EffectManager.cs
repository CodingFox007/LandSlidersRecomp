using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class EffectManager : MonoBehaviour
{
	private List<Effect> m_effects = new List<Effect>();

	public void StartEffect(Effect effect, Rigidbody target, Color color)
	{
		Vector3 zero = Vector3.zero;
		Quaternion rotation = effect.m_particlePrefab.transform.rotation;
		Transform parent = null;
		if (effect.m_attachToPlayer)
		{
			effect.m_target = Singleton<Game>.Instance.Player.GetComponent<Rigidbody>();
			parent = effect.m_target.gameObject.transform;
			zero = Vector3.zero;
		}
		else if (target != null)
		{
			effect.m_target = target;
			parent = effect.m_target.gameObject.transform;
			zero = Vector3.zero;
		}
		if (effect.m_particle == null)
		{
			effect.m_particle = FastPoolManager.GetPool(effect.m_particlePrefab.gameObject).FastInstantiate(zero, rotation, parent).GetComponent<ParticleSystem>();
			ParticleSystemPoolDestroyer component = effect.m_particle.GetComponent<ParticleSystemPoolDestroyer>();
			if (component != null)
			{
				component.m_prefab = effect.m_particlePrefab.gameObject;
				StartCoroutine(component.Start());
			}
			effect.m_particle.Play();
		}
		ParticleSystem[] componentsInChildren = effect.m_particle.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.startColor = color;
		}
		m_effects.Add(effect);
	}

	public void StartEffect(Effect effect, Vector3 position, Color color)
	{
		Vector3 zero = Vector3.zero;
		if (effect.m_particlePrefab == null)
		{
			return;
		}
		Quaternion rotation = effect.m_particlePrefab.transform.rotation;
		if (effect.m_attachToPlayer)
		{
			effect.m_target = Singleton<Game>.Instance.Player.GetComponent<Rigidbody>();
			zero = Vector3.zero;
		}
		else
		{
			zero = position;
		}
		Transform parent = null;
		if (effect.m_attachToPlayer)
		{
			parent = effect.m_target.gameObject.transform;
		}
		if (effect.m_particle == null)
		{
			effect.m_particle = FastPoolManager.GetPool(effect.m_particlePrefab.gameObject).FastInstantiate(zero, rotation, parent).GetComponent<ParticleSystem>();
			ParticleSystemPoolDestroyer component = effect.m_particle.GetComponent<ParticleSystemPoolDestroyer>();
			if (component != null)
			{
				component.m_prefab = effect.m_particlePrefab.gameObject;
				StartCoroutine(component.Start());
			}
			effect.m_particle.Play();
		}
		ParticleSystem[] componentsInChildren = effect.m_particle.GetComponentsInChildren<ParticleSystem>();
		ParticleSystem[] array = componentsInChildren;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.startColor = color;
		}
		m_effects.Add(effect);
	}

	public void StopEffect(Effect effect, bool deleteEffect = false)
	{
		if (effect.m_particlePrefab == null)
		{
			return;
		}
		if (effect.m_particle != null)
		{
			effect.m_particle.Stop();
		}
		if (deleteEffect)
		{
			FastPool pool = FastPoolManager.GetPool(effect.m_particlePrefab.gameObject, false);
			if (pool != null)
			{
				pool.FastDestroy(effect.m_particle.gameObject);
			}
		}
		m_effects.Remove(effect);
	}

	private void Update()
	{
		List<Effect> list = new List<Effect>();
		foreach (Effect effect in m_effects)
		{
			if (effect.m_particle == null)
			{
				list.Add(effect);
			}
			else
			{
				effect.Update();
			}
		}
		foreach (Effect item in list)
		{
			StopEffect(item);
		}
	}
}
