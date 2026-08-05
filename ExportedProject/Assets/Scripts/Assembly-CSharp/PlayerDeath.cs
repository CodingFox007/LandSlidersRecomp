using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[Serializable]
public class PlayerDeath : MonoBehaviour, IEventSystemHandler
{
	public enum Location
	{
		Ground = 0,
		Water = 1,
		Air = 2
	}

	public string m_id;

	public float m_wipeDelay;

	public float m_timeScale = 0.5f;

	public float m_timeScaleLerp = 0.1f;

	public Location m_location;

	public List<string> m_disallowObjectNames;

	public List<string> m_disallowCharacterIDs;

	public List<string> m_restrictToCharacterIDs;

	public bool m_allowIfKilledByEntityEvenOffGround;

	public bool m_turnOffExclamation;

	public bool m_allowIfFrozen = true;

	[SerializeField]
	public UnityEvent m_onDie = new UnityEvent();

	public virtual void Die()
	{
		m_onDie.Invoke();
	}
}
