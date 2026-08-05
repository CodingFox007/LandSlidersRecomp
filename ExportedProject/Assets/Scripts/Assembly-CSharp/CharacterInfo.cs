using System;
using ArabicSupport;
using HeavyDutyInspector;
using I2.Loc;
using UnityEngine;

[Serializable]
public class CharacterInfo : ICloneable
{
	public enum Group
	{
		People = 0,
		Animals = 1,
		Things = 2,
		Seasonal = 3
	}

	[SerializeField]
	private string m_uniqueId;

	[SerializeField]
	private string m_name;

	public Group m_group;

	[SerializeField]
	private bool m_unlockable = true;

	[SerializeField]
	private bool m_purchaseableOnly;

	[SerializeField]
	private bool m_alwaysUnlocked;

	[SerializeField]
	public bool m_countsForTotal = true;

	[SerializeField]
	private string m_specialEventWinnableDay = string.Empty;

	[Multiline]
	[SerializeField]
	private string m_unlockText;

	[SerializeField]
	private GameObject m_replacementPickup;

	[SerializeField]
	private string m_requiredCurrency;

	public bool m_useCustomLockedSelectionPerkText;

	public string m_lockedSelectionPerkText = string.Empty;

	[SerializeField]
	private int m_requiredCurrencyAmount;

	[SerializeField]
	public SurfaceEffect[] m_surfaceEffects;

	[SerializeField]
	public SurfaceEffect[] m_surfaceDeathEffects;

	[SerializeField]
	private Color m_particleColor = Color.white;

	public bool m_faceTargetDirection;

	[SerializeField]
	[AssetPath(typeof(Character), PathOptions.RelativeToResources)]
	private string m_path;

	private bool m_golden;

	[SerializeField]
	public bool m_useCustomTransitionOffset;

	[SerializeField]
	public Vector3 m_customOffsetTransitionOffset = Vector3.zero;

	public string Name
	{
		get
		{
			if (m_golden)
			{
				return ScriptLocalization.Get(m_name, true) + ScriptLocalization.Get("Characters/Gold Suffix", true);
			}
			return ScriptLocalization.Get(m_name, true);
		}
	}

	public string UnlockText
	{
		get
		{
			if (m_requiredCurrency == string.Empty)
			{
				return ScriptLocalization.Get(m_unlockText, true);
			}
			string empty = string.Empty;
			if (LocalizationManager.IsRight2Left)
			{
				LocalizationManager.IsRight2Left = false;
				empty = ArabicFixer.Fix(string.Format(ScriptLocalization.Get(m_unlockText), Singleton<Game>.Instance.InventoryManager.GetCurrency(m_requiredCurrency)));
				LocalizationManager.IsRight2Left = true;
			}
			else
			{
				empty = string.Format(ScriptLocalization.Get(m_unlockText, true), Singleton<Game>.Instance.InventoryManager.GetCurrency(m_requiredCurrency));
			}
			return empty;
		}
	}

	public string Id
	{
		get
		{
			if (m_golden)
			{
				return m_uniqueId + "Gold";
			}
			return m_uniqueId;
		}
	}

	public Color ParticleColor
	{
		get
		{
			return m_particleColor;
		}
	}

	public bool Locked
	{
		get
		{
			if (m_requiredCurrency != string.Empty)
			{
				return Singleton<Game>.Instance.InventoryManager.GetCurrency(m_requiredCurrency) < m_requiredCurrencyAmount;
			}
			return !m_alwaysUnlocked && Singleton<Game>.Instance.InventoryManager.GetCurrency(Id) <= 0;
		}
	}

	public bool Unlockable
	{
		get
		{
			if (m_unlockable)
			{
				return true;
			}
			if (m_specialEventWinnableDay != string.Empty && Singleton<Game>.Instance != null && Singleton<Game>.Instance.SpecialEventManager != null && Singleton<Game>.Instance.SpecialEventManager.CurrentDay() == m_specialEventWinnableDay)
			{
				return true;
			}
			return false;
		}
	}

	public bool PurchaseableOnly
	{
		get
		{
			return m_purchaseableOnly;
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

	public GameObject ReplacementPickup
	{
		get
		{
			return m_replacementPickup;
		}
	}

	public string ResourcePath
	{
		get
		{
			return m_path;
		}
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public object Parse()
	{
		object result = null;
		if (Utils.ParseTokenCount() < 2)
		{
			return result;
		}
		if (Utils.ParseTokenHash(1) == "Name".GetHashCode())
		{
			result = Name;
		}
		else if (Utils.ParseTokenHash(1) == "PickupName".GetHashCode())
		{
			GameObject replacementPickup = ReplacementPickup;
			if (replacementPickup != null)
			{
				result = ((Utils.ParseTokenCount() < 3 || Utils.ParseTokenHash(2) != "Plural".GetHashCode()) ? replacementPickup.GetComponent<Pickup>().Name : replacementPickup.GetComponent<Pickup>().NamePlural);
			}
		}
		else if (Utils.ParseTokenHash(1) == "UnlockedCount".GetHashCode())
		{
			float num = 0f;
			foreach (CharacterInfo availableCharacter in Singleton<Game>.Instance.Player.AvailableCharacters)
			{
				if (!availableCharacter.Locked)
				{
					num += 1f;
				}
			}
			result = num;
		}
		return result;
	}
}
