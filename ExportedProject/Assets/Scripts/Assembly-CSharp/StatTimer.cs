using System.Collections.Generic;

public class StatTimer : Stat
{
	public override void Init()
	{
		m_level = new StatTimerValue();
		m_life = new StatTimerValue();
		m_character = new Dictionary<string, StatValue>();
		if (Singleton<Game>.Instance.Player != null && Singleton<Game>.Instance.Player.Character != null)
		{
			SetCurrentCharacter();
		}
		m_total = new StatTimerValue();
	}

	public override void SetCurrentCharacter()
	{
		if (!m_character.ContainsKey(Singleton<Game>.Instance.Player.CharacterId))
		{
			StatTimerValue value = new StatTimerValue();
			m_character.Add(Singleton<Game>.Instance.Player.CharacterId, value);
		}
		m_characterCurrent = m_character[Singleton<Game>.Instance.Player.CharacterId];
	}

	public override StatValue GetCharacter(string name)
	{
		if (!m_character.ContainsKey(name))
		{
			StatTimerValue value = new StatTimerValue();
			m_character.Add(name, value);
		}
		return m_character[name];
	}
}
