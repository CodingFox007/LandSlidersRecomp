using System;
using UnityEngine;

public class AdManager : MonoBehaviour, Savable
{
	public delegate void AdCallback(bool success);

	private const int GAMES_FOR_INTERSTITIAL = 2;

	[SerializeField]
	private string m_iosAppKey;

	[SerializeField]
	private string m_androidAppKey;

	[SerializeField]
	private RandomRange m_delayInSeconds;

	[SerializeField]
	[TextArea(3, 10)]
	public string m_upsellPriority;

	private string[] m_upsellPriorityList = new string[1];

	private AdCallback m_callback;

	private DateTime m_lastAd;

	private int m_adsIgnored;

	private int m_gamesWithoutAd;

	private int m_gameToShowUpsell;

	private int m_gamesSinceAdWatched;

	private bool m_firstTest = true;

	private bool m_rewardedAdSuccess;

	private bool m_loadedInterstitial;

	public string AppKey
	{
		get
		{
			return m_androidAppKey;
		}
	}

	public void WatchedAd()
	{
		m_adsIgnored = 0;
	}

	public void IgnoredAd()
	{
		m_adsIgnored++;
	}

	public int AdsIgnored()
	{
		return m_adsIgnored;
	}

	private void Start()
	{
		IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
		IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
		IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosededEvent;
		IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
		IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
		IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialFailedEvent;
		IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialFailedEvent;
		IronSource.Agent.init(AppKey, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.REWARDED_VIDEO);
		IronSource.Agent.validateIntegration();
		m_lastAd = DateTime.Now;
		m_delayInSeconds.Randomize();
		m_upsellPriorityList = m_upsellPriority.Split(',');
	}

	private void RewardedVideoAdOpenedEvent()
	{
		m_rewardedAdSuccess = false;
		SetOrientationForAdViewing(false);
	}

	private void RewardedVideoAdRewardedEvent(IronSourcePlacement p)
	{
		WatchedAd();
		m_rewardedAdSuccess = true;
	}

	private void RewardedVideoAdClosededEvent()
	{
		ResumeApp();
		m_callback(m_rewardedAdSuccess);
		m_gamesSinceAdWatched = 0;
		SetOrientationForAdViewing(true);
	}

	private void InterstitialAdOpenedEvent()
	{
		SetOrientationForAdViewing(false);
	}

	private void InterstitialAdClosedEvent()
	{
		m_loadedInterstitial = false;
		m_gamesSinceAdWatched = 0;
		SetOrientationForAdViewing(true);
	}

	private void InterstitialFailedEvent(IronSourceError e)
	{
		m_loadedInterstitial = false;
	}

	private bool IsReady()
	{
		if (ReplayKit.IsRecording || Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}
		return true;
	}

	public bool IsRewardedVideoReady(bool ignoreTime = false)
	{
		return false;
	}

	public bool IsInterstitialReady()
	{
        return false;
	}

	public void EndOfGame(bool revived)
	{
		m_gamesSinceAdWatched++;
		if (revived)
		{
			m_gamesSinceAdWatched = 0;
		}
		if (!m_loadedInterstitial)
		{
			m_loadedInterstitial = true;
			IronSource.Agent.loadInterstitial();
		}
	}

    public void ShowRewardedVideo(AdCallback callback, string placement = null)
    {
        m_callback = callback;
        m_delayInSeconds.Randomize();

        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            PauseApp();
            IronSource.Agent.showRewardedVideo(placement);
        }
        else
        {
            Debug.Log("Rewarded video unavailable");
            callback(false);
        }
    }

    public void ShowInterstitial()
	{
		if (IronSource.Agent.isInterstitialReady())
		{
			IronSource.Agent.showInterstitial();
		}
	}

	public bool TestUnavailableAdUpsell(int min, int max)
	{
		if (m_firstTest)
		{
			m_firstTest = false;
			return false;
		}
		if (m_gamesWithoutAd == 0 || m_gamesWithoutAd > max)
		{
			m_gameToShowUpsell = UnityEngine.Random.Range(min, max + 1);
		}
		if (!IsRewardedVideoReady())
		{
			if (m_gameToShowUpsell == m_gamesWithoutAd)
			{
				m_gamesWithoutAd = 0;
				return true;
			}
			m_gamesWithoutAd++;
		}
		return false;
	}

	public string GetPriorityUpsell()
	{
		if (m_upsellPriorityList.Length == 0)
		{
			return string.Empty;
		}
		return m_upsellPriorityList[UnityEngine.Random.Range(0, m_upsellPriorityList.Length)].Trim();
	}

	private void PauseApp()
	{
		Singleton<Game>.Instance.Player.IsMutedFromAdShowing = true;
		Time.timeScale = 0f;
	}

	private void ResumeApp()
	{
		Singleton<Game>.Instance.Player.IsMutedFromAdShowing = false;
		Time.timeScale = 1f;
	}

	public JSONObject Save()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.AddField("version", Version());
		jSONObject.AddField("lastAd", m_lastAd.Ticks.ToString());
		jSONObject.AddField("adsIgnored", m_adsIgnored);
		jSONObject.AddField("gamesWithoutAd", m_gamesWithoutAd);
		jSONObject.AddField("gameToShowUpsell", m_gameToShowUpsell);
		return jSONObject;
	}

	public bool Verify(JSONObject data)
	{
		return true;
	}

	public void Load(JSONObject data)
	{
		if (data.HasField("lastAd"))
		{
			long ticks = long.Parse(data.GetField("lastAd").AsString);
			m_lastAd = new DateTime(ticks);
		}
		if (data.HasField("adsIgnored"))
		{
			m_adsIgnored = data.GetField("adsIgnored").AsInt;
		}
		if (data.HasField("gamesWithoutAd"))
		{
			m_gamesWithoutAd = data.GetField("gamesWithoutAd").AsInt;
		}
		if (data.HasField("gameToShowUpsell"))
		{
			m_gameToShowUpsell = data.GetField("gameToShowUpsell").AsInt;
		}
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
		m_lastAd = DateTime.Now;
		return Save();
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
		return "Ads";
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

	private void SetOrientationForAdViewing(bool active)
	{
		bool flag = (Screen.autorotateToPortraitUpsideDown = active);
		flag = (Screen.autorotateToPortrait = flag);
		flag = (Screen.autorotateToLandscapeRight = flag);
		Screen.autorotateToLandscapeLeft = flag;
	}
}
