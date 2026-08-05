using System;
using System.Collections.Generic;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
	[Serializable]
	public class Perk
	{
		public string m_ID;

		public string m_text1;

		public Sprite m_image;

		public string m_text2;

		public float m_amountPerLevel;

		public float m_visualAmountPerLevel;

		public bool m_isPercentage;

		public bool m_usePickupForImage;

		public List<string> m_characters;
	}

	[SerializeField]
	private List<Perk> m_perks;

	private Dictionary<string, Perk> m_perkDictionary = new Dictionary<string, Perk>();

	private Dictionary<string, string> m_perkLookup = new Dictionary<string, string>();

	private void Start()
	{
		foreach (Perk perk in m_perks)
		{
			foreach (string character in perk.m_characters)
			{
				m_perkDictionary.Add(character, perk);
				m_perkLookup.Add(character, perk.m_ID);
				m_perkDictionary.Add(character + "Gold", perk);
				m_perkLookup.Add(character + "Gold", perk.m_ID);
			}
		}
	}

	public float GetCurrentPerkedValue(string perkID, float currentValue)
	{
		return GetCurrentPerkedValue(perkID, currentValue, false);
	}

	public int GetCurrentPerkedValue(string perkID, int currentValue)
	{
		return GetCurrentPerkedValue(perkID, currentValue, false);
	}

	public float GetCurrentPerkedValue(string perkID, float currentValue, bool reverse)
	{
		string id = Singleton<Game>.Instance.Player.CharacterInfo.Id;
		int num = Singleton<Game>.Instance.Player.CurrentXPTierIdx;
		if (perkID == string.Empty || m_perkLookup[id] != perkID || num == 0)
		{
			return currentValue;
		}
		Perk perk = m_perkDictionary[id];
		if (num > 5)
		{
			num = 5;
		}
		float num2 = perk.m_amountPerLevel * (float)num;
		if (reverse)
		{
			num2 = 0f - num2;
		}
		if (perk.m_isPercentage)
		{
			return currentValue * (1f + num2 / 100f);
		}
		return currentValue + num2;
	}

	public void Validate()
	{
		foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
		{
			if (availableCharacter.Id.EndsWith("Gold"))
			{
				if (Singleton<Game>.Instance.Player.GetCharacterInfo(availableCharacter.Id.Substring(0, availableCharacter.Id.Length - 4)) != null)
				{
					continue;
				}
				Debug.LogError("Gold character " + availableCharacter.Id + " has no non-gold variant!!!");
			}
			int num = 0;
			foreach (Perk perk in m_perks)
			{
				if (perk.m_characters.Contains(availableCharacter.Id))
				{
					num++;
				}
			}
			if (num == 0)
			{
				Debug.LogError("No perk for character " + availableCharacter.Id);
			}
			else if (num > 1)
			{
				Debug.LogError("Character " + availableCharacter.Id + " in is more than one perk!");
			}
		}
	}

	public Perk GetPerkForCharacter(string characterID)
	{
		if (characterID.EndsWith("Gold"))
		{
			characterID = characterID.Substring(0, characterID.Length - 4);
		}
		foreach (Perk perk in m_perks)
		{
			if (perk.m_characters.Contains(characterID))
			{
				return perk;
			}
		}
		return null;
	}

	public int GetCurrentPerkedValue(string perkID, int currentValue, bool reverse)
	{
		string id = Singleton<Game>.Instance.Player.CharacterInfo.Id;
		int num = Singleton<Game>.Instance.Player.CurrentXPTierIdx;
		if (perkID == string.Empty || m_perkLookup[id] != perkID || num == 0)
		{
			return currentValue;
		}
		Perk perk = m_perkDictionary[id];
		if (num > 5)
		{
			num = 5;
		}
		float num2 = perk.m_amountPerLevel * (float)num;
		if (reverse)
		{
			num2 = 0f - num2;
		}
		if (perk.m_isPercentage)
		{
			return (int)((float)currentValue * (1f + num2 / 100f));
		}
		return currentValue + (int)num2;
	}
}
