using System;
using System.Collections.Generic;
using IronSourceJSON;
using UnityEngine;

public class AndroidAgent : IronSourceIAgent
{
	private static AndroidJavaObject _androidBridge;

	private static readonly string AndroidBridge = "com.ironsource.unity.androidbridge.AndroidBridge";

	private const string REWARD_AMOUNT = "reward_amount";

	private const string REWARD_NAME = "reward_name";

	private const string PLACEMENT_NAME = "placement_name";

	public AndroidAgent()
	{
		Debug.Log("AndroidAgent ctr");
	}

    private AndroidJavaObject getBridge()
    {
    #if !UNITY_ANDROID
            return null;
    #else
        if (_androidBridge == null)
        {
            try
            {
                using (AndroidJavaClass androidJavaClass =
                       new AndroidJavaClass(AndroidBridge))
                {
                    _androidBridge =
                        androidJavaClass.CallStatic<AndroidJavaObject>("getInstance");
                }
            }
            catch (Exception e)
            {
                Debug.Log("IronSource disabled: " + e.Message);
                return null;
            }
        }

        return _androidBridge;
    #endif
        }

    public void reportAppStarted()
    {
        var bridge = getBridge();
        if (bridge != null)
            bridge.Call("reportAppStarted");
    }

    public void onApplicationPause(bool pause)
	{
		if (pause)
		{
			getBridge().Call("onPause");
		}
		else
		{
			getBridge().Call("onResume");
		}
	}

	public void setAge(int age)
	{
		getBridge().Call("setAge", age);
	}

	public void setGender(string gender)
	{
		getBridge().Call("setGender", gender);
	}

	public void setMediationSegment(string segment)
	{
		getBridge().Call("setMediationSegment", segment);
	}

	public string getAdvertiserId()
	{
		return getBridge().Call<string>("getAdvertiserId", new object[0]);
	}

	public void validateIntegration()
	{
		getBridge().Call("validateIntegration");
	}

	public void shouldTrackNetworkState(bool track)
	{
		getBridge().Call("shouldTrackNetworkState", track);
	}

	public bool setDynamicUserId(string dynamicUserId)
	{
		return getBridge().Call<bool>("setDynamicUserId", new object[1] { dynamicUserId });
	}

	public void setAdaptersDebug(bool enabled)
	{
		getBridge().Call("setAdaptersDebug", enabled);
	}

	public void setUserId(string userId)
	{
		getBridge().Call("setUserId", userId);
	}

	public void init(string appKey)
	{
		getBridge().Call("setPluginData", "Unity", IronSource.pluginVersion(), IronSource.unityVersion());
		getBridge().Call("init", appKey);
	}

	public void init(string appKey, params string[] adUnits)
	{
		getBridge().Call("setPluginData", "Unity", IronSource.pluginVersion(), IronSource.unityVersion());
		getBridge().Call("init", appKey, adUnits);
	}

	public void showRewardedVideo()
	{
		getBridge().Call("showRewardedVideo");
	}

	public void showRewardedVideo(string placementName)
	{
		getBridge().Call("showRewardedVideo", placementName);
	}

    public bool isRewardedVideoAvailable()
    {
        #if UNITY_EDITOR || UNITY_IOS
                return false;
        #else
            return getBridge().Call<bool>("isRewardedVideoAvailable");
        #endif
            }

    public bool isRewardedVideoPlacementCapped(string placementName)
	{
		return getBridge().Call<bool>("isRewardedVideoPlacementCapped", new object[1] { placementName });
	}

	public IronSourcePlacement getPlacementInfo(string placementName)
	{
		string text = getBridge().Call<string>("getPlacementInfo", new object[1] { placementName });
		IronSourcePlacement result = null;
		if (text != null)
		{
			Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
			string placementName2 = dictionary["placement_name"].ToString();
			string rewardName = dictionary["reward_name"].ToString();
			int rewardAmount = Convert.ToInt32(dictionary["reward_amount"].ToString());
			result = new IronSourcePlacement(placementName2, rewardName, rewardAmount);
		}
		return result;
	}

	public void loadInterstitial()
	{
		getBridge().Call("loadInterstitial");
	}

	public void showInterstitial()
	{
		getBridge().Call("showInterstitial");
	}

	public void showInterstitial(string placementName)
	{
		getBridge().Call("showInterstitial", placementName);
	}

	public bool isInterstitialReady()
	{
		return getBridge().Call<bool>("isInterstitialReady", new object[0]);
	}

	public bool isInterstitialPlacementCapped(string placementName)
	{
		return getBridge().Call<bool>("isInterstitialPlacementCapped", new object[1] { placementName });
	}

	public void showOfferwall()
	{
		getBridge().Call("showOfferwall");
	}

	public void showOfferwall(string placementName)
	{
		getBridge().Call("showOfferwall", placementName);
	}

	public void getOfferwallCredits()
	{
		getBridge().Call("getOfferwallCredits");
	}

	public bool isOfferwallAvailable()
	{
		return getBridge().Call<bool>("isOfferwallAvailable", new object[0]);
	}

	public void loadBanner(IronSourceBannerSize size, IronSourceBannerPosition position)
	{
		getBridge().Call("loadBanner", (int)size, (int)position);
	}

	public void loadBanner(IronSourceBannerSize size, IronSourceBannerPosition position, string placementName)
	{
		getBridge().Call("loadBanner", (int)size, (int)position, placementName);
	}

	public void destroyBanner()
	{
		getBridge().Call("destroyBanner");
	}

	public bool isBannerPlacementCapped(string placementName)
	{
		return getBridge().Call<bool>("isBannerPlacementCapped", new object[1] { placementName });
	}
}
