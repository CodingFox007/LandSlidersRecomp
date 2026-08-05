using System;
using System.Collections.Generic;
using System.Linq;
using IronSourceJSON;
using UnityEngine;

public class IronSourceEvents : MonoBehaviour
{
	private const string ERROR_CODE = "error_code";

	private const string ERROR_DESCRIPTION = "error_description";

	private static event Action<IronSourceError> _onRewardedVideoAdShowFailedEvent;

	public static event Action<IronSourceError> onRewardedVideoAdShowFailedEvent
	{
		add
		{
			if (IronSourceEvents._onRewardedVideoAdShowFailedEvent == null || !IronSourceEvents._onRewardedVideoAdShowFailedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdShowFailedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onRewardedVideoAdShowFailedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdShowFailedEvent -= value;
			}
		}
	}

	private static event Action _onRewardedVideoAdOpenedEvent;

	public static event Action onRewardedVideoAdOpenedEvent
	{
		add
		{
			if (IronSourceEvents._onRewardedVideoAdOpenedEvent == null || !IronSourceEvents._onRewardedVideoAdOpenedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdOpenedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onRewardedVideoAdOpenedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdOpenedEvent -= value;
			}
		}
	}

	private static event Action _onRewardedVideoAdClosedEvent;

	public static event Action onRewardedVideoAdClosedEvent
	{
		add
		{
			if (IronSourceEvents._onRewardedVideoAdClosedEvent == null || !IronSourceEvents._onRewardedVideoAdClosedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdClosedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onRewardedVideoAdClosedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdClosedEvent -= value;
			}
		}
	}

	private static event Action _onRewardedVideoAdStartedEvent;

	public static event Action onRewardedVideoAdStartedEvent
	{
		add
		{
			if (IronSourceEvents._onRewardedVideoAdStartedEvent == null || !IronSourceEvents._onRewardedVideoAdStartedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdStartedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onRewardedVideoAdStartedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdStartedEvent -= value;
			}
		}
	}

	private static event Action _onRewardedVideoAdEndedEvent;

	public static event Action onRewardedVideoAdEndedEvent
	{
		add
		{
			if (IronSourceEvents._onRewardedVideoAdEndedEvent == null || !IronSourceEvents._onRewardedVideoAdEndedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdEndedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onRewardedVideoAdEndedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdEndedEvent -= value;
			}
		}
	}

	private static event Action<IronSourcePlacement> _onRewardedVideoAdRewardedEvent;

	public static event Action<IronSourcePlacement> onRewardedVideoAdRewardedEvent
	{
		add
		{
			if (IronSourceEvents._onRewardedVideoAdRewardedEvent == null || !IronSourceEvents._onRewardedVideoAdRewardedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdRewardedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onRewardedVideoAdRewardedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAdRewardedEvent -= value;
			}
		}
	}

	private static event Action<bool> _onRewardedVideoAvailabilityChangedEvent;

	public static event Action<bool> onRewardedVideoAvailabilityChangedEvent
	{
		add
		{
			if (IronSourceEvents._onRewardedVideoAvailabilityChangedEvent == null || !IronSourceEvents._onRewardedVideoAvailabilityChangedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAvailabilityChangedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onRewardedVideoAvailabilityChangedEvent.GetInvocationList().Contains(value))
			{
				_onRewardedVideoAvailabilityChangedEvent -= value;
			}
		}
	}

	private static event Action _onInterstitialAdReadyEvent;

	public static event Action onInterstitialAdReadyEvent
	{
		add
		{
			if (IronSourceEvents._onInterstitialAdReadyEvent == null || !IronSourceEvents._onInterstitialAdReadyEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdReadyEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onInterstitialAdReadyEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdReadyEvent -= value;
			}
		}
	}

	private static event Action<IronSourceError> _onInterstitialAdLoadFailedEvent;

	public static event Action<IronSourceError> onInterstitialAdLoadFailedEvent
	{
		add
		{
			if (IronSourceEvents._onInterstitialAdLoadFailedEvent == null || !IronSourceEvents._onInterstitialAdLoadFailedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdLoadFailedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onInterstitialAdLoadFailedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdLoadFailedEvent -= value;
			}
		}
	}

	private static event Action _onInterstitialAdOpenedEvent;

	public static event Action onInterstitialAdOpenedEvent
	{
		add
		{
			if (IronSourceEvents._onInterstitialAdOpenedEvent == null || !IronSourceEvents._onInterstitialAdOpenedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdOpenedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onInterstitialAdOpenedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdOpenedEvent -= value;
			}
		}
	}

	private static event Action _onInterstitialAdClosedEvent;

	public static event Action onInterstitialAdClosedEvent
	{
		add
		{
			if (IronSourceEvents._onInterstitialAdClosedEvent == null || !IronSourceEvents._onInterstitialAdClosedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdClosedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onInterstitialAdClosedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdClosedEvent -= value;
			}
		}
	}

	private static event Action _onInterstitialAdShowSucceededEvent;

	public static event Action onInterstitialAdShowSucceededEvent
	{
		add
		{
			if (IronSourceEvents._onInterstitialAdShowSucceededEvent == null || !IronSourceEvents._onInterstitialAdShowSucceededEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdShowSucceededEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onInterstitialAdShowSucceededEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdShowSucceededEvent -= value;
			}
		}
	}

	private static event Action<IronSourceError> _onInterstitialAdShowFailedEvent;

	public static event Action<IronSourceError> onInterstitialAdShowFailedEvent
	{
		add
		{
			if (IronSourceEvents._onInterstitialAdShowFailedEvent == null || !IronSourceEvents._onInterstitialAdShowFailedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdShowFailedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onInterstitialAdShowFailedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdShowFailedEvent -= value;
			}
		}
	}

	private static event Action _onInterstitialAdClickedEvent;

	public static event Action onInterstitialAdClickedEvent
	{
		add
		{
			if (IronSourceEvents._onInterstitialAdClickedEvent == null || !IronSourceEvents._onInterstitialAdClickedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdClickedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onInterstitialAdClickedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdClickedEvent -= value;
			}
		}
	}

	private static event Action _onInterstitialAdRewardedEvent;

	public static event Action onInterstitialAdRewardedEvent
	{
		add
		{
			if (IronSourceEvents._onInterstitialAdRewardedEvent == null || !IronSourceEvents._onInterstitialAdRewardedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdRewardedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onInterstitialAdRewardedEvent.GetInvocationList().Contains(value))
			{
				_onInterstitialAdRewardedEvent -= value;
			}
		}
	}

	private static event Action _onOfferwallOpenedEvent;

	public static event Action onOfferwallOpenedEvent
	{
		add
		{
			if (IronSourceEvents._onOfferwallOpenedEvent == null || !IronSourceEvents._onOfferwallOpenedEvent.GetInvocationList().Contains(value))
			{
				_onOfferwallOpenedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onOfferwallOpenedEvent.GetInvocationList().Contains(value))
			{
				_onOfferwallOpenedEvent -= value;
			}
		}
	}

	private static event Action<IronSourceError> _onOfferwallShowFailedEvent;

	public static event Action<IronSourceError> onOfferwallShowFailedEvent
	{
		add
		{
			if (IronSourceEvents._onOfferwallShowFailedEvent == null || !IronSourceEvents._onOfferwallShowFailedEvent.GetInvocationList().Contains(value))
			{
				_onOfferwallShowFailedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onOfferwallShowFailedEvent.GetInvocationList().Contains(value))
			{
				_onOfferwallShowFailedEvent -= value;
			}
		}
	}

	private static event Action _onOfferwallClosedEvent;

	public static event Action onOfferwallClosedEvent
	{
		add
		{
			if (IronSourceEvents._onOfferwallClosedEvent == null || !IronSourceEvents._onOfferwallClosedEvent.GetInvocationList().Contains(value))
			{
				_onOfferwallClosedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onOfferwallClosedEvent.GetInvocationList().Contains(value))
			{
				_onOfferwallClosedEvent -= value;
			}
		}
	}

	private static event Action<IronSourceError> _onGetOfferwallCreditsFailedEvent;

	public static event Action<IronSourceError> onGetOfferwallCreditsFailedEvent
	{
		add
		{
			if (IronSourceEvents._onGetOfferwallCreditsFailedEvent == null || !IronSourceEvents._onGetOfferwallCreditsFailedEvent.GetInvocationList().Contains(value))
			{
				_onGetOfferwallCreditsFailedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onGetOfferwallCreditsFailedEvent.GetInvocationList().Contains(value))
			{
				_onGetOfferwallCreditsFailedEvent -= value;
			}
		}
	}

	private static event Action<Dictionary<string, object>> _onOfferwallAdCreditedEvent;

	public static event Action<Dictionary<string, object>> onOfferwallAdCreditedEvent
	{
		add
		{
			if (IronSourceEvents._onOfferwallAdCreditedEvent == null || !IronSourceEvents._onOfferwallAdCreditedEvent.GetInvocationList().Contains(value))
			{
				_onOfferwallAdCreditedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onOfferwallAdCreditedEvent.GetInvocationList().Contains(value))
			{
				_onOfferwallAdCreditedEvent -= value;
			}
		}
	}

	private static event Action<bool> _onOfferwallAvailableEvent;

	public static event Action<bool> onOfferwallAvailableEvent
	{
		add
		{
			if (IronSourceEvents._onOfferwallAvailableEvent == null || !IronSourceEvents._onOfferwallAvailableEvent.GetInvocationList().Contains(value))
			{
				_onOfferwallAvailableEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onOfferwallAvailableEvent.GetInvocationList().Contains(value))
			{
				_onOfferwallAvailableEvent -= value;
			}
		}
	}

	private static event Action _onBannerAdLoadedEvent;

	public static event Action onBannerAdLoadedEvent
	{
		add
		{
			if (IronSourceEvents._onBannerAdLoadedEvent == null || !IronSourceEvents._onBannerAdLoadedEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdLoadedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onBannerAdLoadedEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdLoadedEvent -= value;
			}
		}
	}

	private static event Action<IronSourceError> _onBannerAdLoadFailedEvent;

	public static event Action<IronSourceError> onBannerAdLoadFailedEvent
	{
		add
		{
			if (IronSourceEvents._onBannerAdLoadFailedEvent == null || !IronSourceEvents._onBannerAdLoadFailedEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdLoadFailedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onBannerAdLoadFailedEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdLoadFailedEvent -= value;
			}
		}
	}

	private static event Action _onBannerAdClickedEvent;

	public static event Action onBannerAdClickedEvent
	{
		add
		{
			if (IronSourceEvents._onBannerAdClickedEvent == null || !IronSourceEvents._onBannerAdClickedEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdClickedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onBannerAdClickedEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdClickedEvent -= value;
			}
		}
	}

	private static event Action _onBannerAdScreenPresentedEvent;

	public static event Action onBannerAdScreenPresentedEvent
	{
		add
		{
			if (IronSourceEvents._onBannerAdScreenPresentedEvent == null || !IronSourceEvents._onBannerAdScreenPresentedEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdScreenPresentedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onBannerAdScreenPresentedEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdScreenPresentedEvent -= value;
			}
		}
	}

	private static event Action _onBannerAdScreenDismissedEvent;

	public static event Action onBannerAdScreenDismissedEvent
	{
		add
		{
			if (IronSourceEvents._onBannerAdScreenDismissedEvent == null || !IronSourceEvents._onBannerAdScreenDismissedEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdScreenDismissedEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onBannerAdScreenDismissedEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdScreenDismissedEvent -= value;
			}
		}
	}

	private static event Action _onBannerAdLeftApplicationEvent;

	public static event Action onBannerAdLeftApplicationEvent
	{
		add
		{
			if (IronSourceEvents._onBannerAdLeftApplicationEvent == null || !IronSourceEvents._onBannerAdLeftApplicationEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdLeftApplicationEvent += value;
			}
		}
		remove
		{
			if (IronSourceEvents._onBannerAdLeftApplicationEvent.GetInvocationList().Contains(value))
			{
				_onBannerAdLeftApplicationEvent -= value;
			}
		}
	}

	private void Awake()
	{
		base.gameObject.name = "IronSourceEvents";
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void onRewardedVideoAdShowFailed(string description)
	{
		if (IronSourceEvents._onRewardedVideoAdShowFailedEvent != null)
		{
			IronSourceError errorFromErrorString = getErrorFromErrorString(description);
			IronSourceEvents._onRewardedVideoAdShowFailedEvent(errorFromErrorString);
		}
	}

	public void onRewardedVideoAdOpened(string empty)
	{
		if (IronSourceEvents._onRewardedVideoAdOpenedEvent != null)
		{
			IronSourceEvents._onRewardedVideoAdOpenedEvent();
		}
	}

	public void onRewardedVideoAdClosed(string empty)
	{
		if (IronSourceEvents._onRewardedVideoAdClosedEvent != null)
		{
			IronSourceEvents._onRewardedVideoAdClosedEvent();
		}
	}

	public void onRewardedVideoAdStarted(string empty)
	{
		if (IronSourceEvents._onRewardedVideoAdStartedEvent != null)
		{
			IronSourceEvents._onRewardedVideoAdStartedEvent();
		}
	}

	public void onRewardedVideoAdEnded(string empty)
	{
		if (IronSourceEvents._onRewardedVideoAdEndedEvent != null)
		{
			IronSourceEvents._onRewardedVideoAdEndedEvent();
		}
	}

	public void onRewardedVideoAdRewarded(string description)
	{
		if (IronSourceEvents._onRewardedVideoAdRewardedEvent != null)
		{
			IronSourcePlacement placementFromString = getPlacementFromString(description);
			IronSourceEvents._onRewardedVideoAdRewardedEvent(placementFromString);
		}
	}

	public void onRewardedVideoAvailabilityChanged(string stringAvailable)
	{
		bool obj = stringAvailable == "true";
		if (IronSourceEvents._onRewardedVideoAvailabilityChangedEvent != null)
		{
			IronSourceEvents._onRewardedVideoAvailabilityChangedEvent(obj);
		}
	}

	public void onInterstitialAdReady()
	{
		if (IronSourceEvents._onInterstitialAdReadyEvent != null)
		{
			IronSourceEvents._onInterstitialAdReadyEvent();
		}
	}

	public void onInterstitialAdLoadFailed(string description)
	{
		if (IronSourceEvents._onInterstitialAdLoadFailedEvent != null)
		{
			IronSourceError errorFromErrorString = getErrorFromErrorString(description);
			IronSourceEvents._onInterstitialAdLoadFailedEvent(errorFromErrorString);
		}
	}

	public void onInterstitialAdOpened(string empty)
	{
		if (IronSourceEvents._onInterstitialAdOpenedEvent != null)
		{
			IronSourceEvents._onInterstitialAdOpenedEvent();
		}
	}

	public void onInterstitialAdClosed(string empty)
	{
		if (IronSourceEvents._onInterstitialAdClosedEvent != null)
		{
			IronSourceEvents._onInterstitialAdClosedEvent();
		}
	}

	public void onInterstitialAdShowSucceeded(string empty)
	{
		if (IronSourceEvents._onInterstitialAdShowSucceededEvent != null)
		{
			IronSourceEvents._onInterstitialAdShowSucceededEvent();
		}
	}

	public void onInterstitialAdShowFailed(string description)
	{
		if (IronSourceEvents._onInterstitialAdShowFailedEvent != null)
		{
			IronSourceError errorFromErrorString = getErrorFromErrorString(description);
			IronSourceEvents._onInterstitialAdShowFailedEvent(errorFromErrorString);
		}
	}

	public void onInterstitialAdClicked(string empty)
	{
		if (IronSourceEvents._onInterstitialAdClickedEvent != null)
		{
			IronSourceEvents._onInterstitialAdClickedEvent();
		}
	}

	public void onInterstitialAdRewarded(string empty)
	{
		if (IronSourceEvents._onInterstitialAdRewardedEvent != null)
		{
			IronSourceEvents._onInterstitialAdRewardedEvent();
		}
	}

	public void onOfferwallOpened(string empty)
	{
		if (IronSourceEvents._onOfferwallOpenedEvent != null)
		{
			IronSourceEvents._onOfferwallOpenedEvent();
		}
	}

	public void onOfferwallShowFailed(string description)
	{
		if (IronSourceEvents._onOfferwallShowFailedEvent != null)
		{
			IronSourceError errorFromErrorString = getErrorFromErrorString(description);
			IronSourceEvents._onOfferwallShowFailedEvent(errorFromErrorString);
		}
	}

	public void onOfferwallClosed(string empty)
	{
		if (IronSourceEvents._onOfferwallClosedEvent != null)
		{
			IronSourceEvents._onOfferwallClosedEvent();
		}
	}

	public void onGetOfferwallCreditsFailed(string description)
	{
		if (IronSourceEvents._onGetOfferwallCreditsFailedEvent != null)
		{
			IronSourceError errorFromErrorString = getErrorFromErrorString(description);
			IronSourceEvents._onGetOfferwallCreditsFailedEvent(errorFromErrorString);
		}
	}

	public void onOfferwallAdCredited(string json)
	{
		if (IronSourceEvents._onOfferwallAdCreditedEvent != null)
		{
			IronSourceEvents._onOfferwallAdCreditedEvent(Json.Deserialize(json) as Dictionary<string, object>);
		}
	}

	public void onOfferwallAvailable(string stringAvailable)
	{
		bool obj = stringAvailable == "true";
		if (IronSourceEvents._onOfferwallAvailableEvent != null)
		{
			IronSourceEvents._onOfferwallAvailableEvent(obj);
		}
	}

	public void onBannerAdLoaded()
	{
		if (IronSourceEvents._onBannerAdLoadedEvent != null)
		{
			IronSourceEvents._onBannerAdLoadedEvent();
		}
	}

	public void onBannerAdLoadFailed(string description)
	{
		if (IronSourceEvents._onBannerAdLoadFailedEvent != null)
		{
			IronSourceError errorFromErrorString = getErrorFromErrorString(description);
			IronSourceEvents._onBannerAdLoadFailedEvent(errorFromErrorString);
		}
	}

	public void onBannerAdClicked()
	{
		if (IronSourceEvents._onBannerAdClickedEvent != null)
		{
			IronSourceEvents._onBannerAdClickedEvent();
		}
	}

	public void onBannerAdScreenPresented()
	{
		if (IronSourceEvents._onBannerAdScreenPresentedEvent != null)
		{
			IronSourceEvents._onBannerAdScreenPresentedEvent();
		}
	}

	public void onBannerAdScreenDismissed()
	{
		if (IronSourceEvents._onBannerAdScreenDismissedEvent != null)
		{
			IronSourceEvents._onBannerAdScreenDismissedEvent();
		}
	}

	public void onBannerAdLeftApplication()
	{
		if (IronSourceEvents._onBannerAdLeftApplicationEvent != null)
		{
			IronSourceEvents._onBannerAdLeftApplicationEvent();
		}
	}

	public IronSourceError getErrorFromErrorString(string description)
	{
		if (!string.IsNullOrEmpty(description))
		{
			Dictionary<string, object> dictionary = Json.Deserialize(description) as Dictionary<string, object>;
			if (dictionary != null)
			{
				int errorCode = Convert.ToInt32(dictionary["error_code"].ToString());
				string errorDescription = dictionary["error_description"].ToString();
				return new IronSourceError(errorCode, errorDescription);
			}
			return new IronSourceError(-1, string.Empty);
		}
		return new IronSourceError(-1, string.Empty);
	}

	public IronSourcePlacement getPlacementFromString(string jsonPlacement)
	{
		Dictionary<string, object> dictionary = Json.Deserialize(jsonPlacement) as Dictionary<string, object>;
		int rewardAmount = Convert.ToInt32(dictionary["placement_reward_amount"].ToString());
		string rewardName = dictionary["placement_reward_name"].ToString();
		string placementName = dictionary["placement_name"].ToString();
		return new IronSourcePlacement(placementName, rewardName, rewardAmount);
	}
}
