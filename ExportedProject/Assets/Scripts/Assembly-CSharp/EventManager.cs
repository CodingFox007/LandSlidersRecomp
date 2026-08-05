using System;
using System.Collections.Generic;
using HeavyDutyInspector;
using UnityEngine;

public class EventManager : MonoBehaviour, Savable
{
	[Serializable]
	public class Event
	{
		public string m_id;

		public string m_group;

		public string m_requiredEventId;

		public bool m_repeat;

		public bool m_availableOnLoad;

		[Comment("days.hours:minutes:seconds", CommentType.None, 0)]
		public string m_delayStr;

		[Comment("days.hours:minutes:seconds", CommentType.None, 0)]
		public string m_durationStr;

		[HideInInspector]
		public TimeSpan m_delay = default(TimeSpan);

		[HideInInspector]
		public DateTime m_startDate = default(DateTime);

		[HideInInspector]
		public DateTime m_endDate = default(DateTime);

		[HideInInspector]
		public TimeSpan m_duration = default(TimeSpan);
	}

	public delegate void EventTriggered(string id);

	[SerializeField]
	private Event[] m_events;

	private List<Event> m_availableEvents = new List<Event>();

	private List<Event> m_activeEvents = new List<Event>();

	private List<Event> m_usedEvents = new List<Event>();

	public event EventTriggered OnEventTriggered;

	public event EventTriggered OnEventAvailable;

	private void Awake()
	{
		Event[] events = m_events;
		foreach (Event obj in events)
		{
			if (obj.m_delayStr != null && obj.m_delayStr != string.Empty)
			{
				obj.m_delay = TimeSpan.Parse(obj.m_delayStr);
			}
			else
			{
				obj.m_delay = new TimeSpan(0L);
			}
			if (obj.m_durationStr != null && obj.m_durationStr != string.Empty)
			{
				obj.m_duration = TimeSpan.Parse(obj.m_durationStr);
			}
			else
			{
				obj.m_duration = TimeSpan.FromDays(365000.0);
			}
		}
	}

	private void Update()
	{
		foreach (Event availableEvent in m_availableEvents)
		{
			if (DateTime.Now.Ticks >= availableEvent.m_startDate.Ticks && DateTime.Now.Ticks < availableEvent.m_endDate.Ticks && !m_activeEvents.Contains(availableEvent))
			{
				m_activeEvents.Add(availableEvent);
				if (this.OnEventTriggered != null)
				{
					this.OnEventTriggered(availableEvent.m_id);
				}
			}
		}
	}

	public Event GetEvent(string id)
	{
		Event[] events = m_events;
		foreach (Event obj in events)
		{
			if (obj.m_id == id)
			{
				return obj;
			}
		}
		return null;
	}

	public bool IsEventActive(string id)
	{
		foreach (Event activeEvent in m_activeEvents)
		{
			if (activeEvent.m_id == id)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsEventGroupActive(string groupId)
	{
		foreach (Event activeEvent in m_activeEvents)
		{
			if (activeEvent.m_group == groupId)
			{
				return true;
			}
		}
		return false;
	}

	public TimeSpan EventAvailableIn(string id)
	{
		foreach (Event availableEvent in m_availableEvents)
		{
			if (availableEvent.m_id == id)
			{
				return availableEvent.m_startDate.Subtract(DateTime.Now);
			}
		}
		return new TimeSpan(0L);
	}

	public TimeSpan EventGroupAvailableIn(string groupId)
	{
		foreach (Event availableEvent in m_availableEvents)
		{
			if (availableEvent.m_group == groupId)
			{
				return availableEvent.m_startDate.Subtract(DateTime.Now);
			}
		}
		return new TimeSpan(0L);
	}

	public string EventAvailableInStr(string id)
	{
		TimeSpan timeSpan = Singleton<Game>.Instance.EventManager.EventAvailableIn(id);
		return string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public string EventGroupAvailableInStr(string id)
	{
		TimeSpan timeSpan = Singleton<Game>.Instance.EventManager.EventGroupAvailableIn(id);
		return string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}

	public Event GetActiveEventInGroup(string groupId)
	{
		foreach (Event activeEvent in m_activeEvents)
		{
			if (activeEvent.m_group == groupId)
			{
				return activeEvent;
			}
		}
		return null;
	}

	public void UseEvent(string id)
	{
		Event evt = GetEvent(id);
		UseEvent(evt);
	}

	public void UseEvent(Event evt)
	{
		if (evt == null)
		{
			return;
		}
		m_activeEvents.Remove(evt);
		m_availableEvents.Remove(evt);
		m_usedEvents.Add(evt);
		Event[] events = m_events;
		foreach (Event obj in events)
		{
			if (obj.m_requiredEventId == evt.m_id && !m_usedEvents.Contains(obj) && !m_activeEvents.Contains(obj) && !m_availableEvents.Contains(obj))
			{
				obj.m_startDate = DateTime.Now + obj.m_delay;
				obj.m_endDate = obj.m_startDate + obj.m_duration;
				m_availableEvents.Add(obj);
				if (this.OnEventAvailable != null)
				{
					this.OnEventAvailable(obj.m_id);
				}
			}
		}
		if (evt.m_repeat)
		{
			evt.m_startDate = DateTime.Now + evt.m_delay;
			evt.m_endDate = evt.m_startDate + evt.m_duration;
			m_availableEvents.Add(evt);
			if (this.OnEventAvailable != null)
			{
				this.OnEventAvailable(evt.m_id);
			}
		}
	}

	public bool Verify(JSONObject data)
	{
		return true;
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		JSONObject jSONObject2 = new JSONObject();
		Event[] events = m_events;
		foreach (Event obj in events)
		{
			JSONObject jSONObject3 = new JSONObject();
			jSONObject3.AddField("startDate", obj.m_startDate.Ticks.ToString());
			jSONObject3.AddField("endDate", obj.m_endDate.Ticks.ToString());
			jSONObject2.AddField(obj.m_id, jSONObject3);
		}
		jSONObject.AddField("events", jSONObject2);
		JSONObject jSONObject4 = new JSONObject();
		foreach (Event availableEvent in m_availableEvents)
		{
			jSONObject4.AddField(availableEvent.m_id, true);
		}
		jSONObject.AddField("available", jSONObject4);
		JSONObject jSONObject5 = new JSONObject();
		foreach (Event activeEvent in m_activeEvents)
		{
			jSONObject5.AddField(activeEvent.m_id, true);
		}
		jSONObject.AddField("active", jSONObject5);
		JSONObject jSONObject6 = new JSONObject();
		foreach (Event usedEvent in m_usedEvents)
		{
			jSONObject6.AddField(usedEvent.m_id, true);
		}
		jSONObject.AddField("used", jSONObject6);
		return jSONObject;
	}

	public void Load(JSONObject data)
	{
		m_availableEvents.Clear();
		m_activeEvents.Clear();
		m_usedEvents.Clear();
		if (data.HasField("events"))
		{
			JSONObject field = data.GetField("events");
			foreach (string key in field.keys)
			{
				Event obj = GetEvent(key);
				if (obj != null)
				{
					JSONObject field2 = field.GetField(key);
					if (field2.HasField("startDate"))
					{
						long ticks = long.Parse(field2.GetField("startDate").AsString);
						obj.m_startDate = new DateTime(ticks);
					}
					if (field2.HasField("endDate"))
					{
						long ticks2 = long.Parse(field2.GetField("endDate").AsString);
						obj.m_endDate = new DateTime(ticks2);
					}
				}
			}
		}
		if (data.HasField("available"))
		{
			JSONObject field3 = data.GetField("available");
			if (!field3.IsNull)
			{
				foreach (string key2 in field3.keys)
				{
					Event obj2 = GetEvent(key2);
					if (obj2 != null)
					{
						m_availableEvents.Add(obj2);
					}
				}
			}
		}
		if (data.HasField("active"))
		{
			JSONObject field4 = data.GetField("active");
			if (!field4.IsNull)
			{
				foreach (string key3 in field4.keys)
				{
					Event obj3 = GetEvent(key3);
					if (obj3 != null)
					{
						m_activeEvents.Add(obj3);
					}
				}
			}
		}
		if (data.HasField("used"))
		{
			JSONObject field5 = data.GetField("used");
			if (!field5.IsNull)
			{
				foreach (string key4 in field5.keys)
				{
					Event obj4 = GetEvent(key4);
					if (obj4 != null)
					{
						m_usedEvents.Add(obj4);
					}
				}
			}
		}
		CheckStartEvents();
	}

	public void Merge(JSONObject dataA, JSONObject dataB)
	{
	}

	public JSONObject MergeToJSON(JSONObject dataA, JSONObject dataB)
	{
		return dataA;
	}

	public JSONObject Reset()
	{
		m_availableEvents.Clear();
		m_activeEvents.Clear();
		m_usedEvents.Clear();
		CheckStartEvents();
		return Save();
	}

	private void CheckStartEvents()
	{
		Event[] events = m_events;
		foreach (Event obj in events)
		{
			if (obj.m_availableOnLoad && !m_availableEvents.Contains(obj) && !m_activeEvents.Contains(obj) && !m_usedEvents.Contains(obj))
			{
				m_availableEvents.Add(obj);
				obj.m_startDate = DateTime.Now + obj.m_delay;
				obj.m_endDate = obj.m_startDate + obj.m_duration;
			}
		}
	}

	public JSONObject UpdateVersion(JSONObject data)
	{
		JSONObject result = data;
		string version = GetVersion(data);
		if (!(version == string.Empty))
		{
			switch (version)
			{
			case "0.0.1":
			case "0.0.2":
			case "0.0.3":
				break;
			default:
				goto IL_0051;
			}
		}
		result = Reset();
		goto IL_0051;
		IL_0051:
		return result;
	}

	public string SaveId()
	{
		return "Events";
	}

	public string Version()
	{
		return "0.0.4";
	}

	public string GetVersion(JSONObject data)
	{
		if (!data.HasField("version"))
		{
			return string.Empty;
		}
		return data.GetField("version").AsString;
	}

	public bool UseCloud()
	{
		return false;
	}

	public object Parse()
	{
		object result = null;
		if (Utils.ParseTokenCount() < 3)
		{
			return result;
		}
		if (Utils.ParseTokenHash(1) == "Group".GetHashCode())
		{
			string text = Utils.ParseToken(2);
			if (Utils.ParseTokenCount() >= 4)
			{
				if (Utils.ParseTokenHash(3) == "Active".GetHashCode())
				{
					result = IsEventGroupActive(text);
				}
				else if (Utils.ParseTokenHash(3) == "AvailableIn".GetHashCode())
				{
					result = EventGroupAvailableIn(text);
				}
				else if (Utils.ParseTokenHash(3) == "AvailableInStr".GetHashCode())
				{
					result = EventGroupAvailableInStr(text);
				}
			}
		}
		else
		{
			Event obj = GetEvent(Utils.ParseToken(1));
			if (obj == null)
			{
				return result;
			}
			if (Utils.ParseTokenHash(2) == "Available".GetHashCode())
			{
				result = m_availableEvents.Contains(obj);
			}
			else if (Utils.ParseTokenHash(2) == "Active".GetHashCode())
			{
				result = m_activeEvents.Contains(obj);
			}
			else if (Utils.ParseTokenHash(2) == "Used".GetHashCode())
			{
				result = m_usedEvents.Contains(obj);
			}
			else if (Utils.ParseTokenHash(2) == "AvailableIn".GetHashCode())
			{
				result = EventAvailableIn(obj.m_id);
			}
			else if (Utils.ParseTokenHash(2) == "AvailableInStr".GetHashCode())
			{
				result = EventAvailableInStr(obj.m_id);
			}
		}
		return result;
	}
}
