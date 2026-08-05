public interface IronSourceIAgent
{
	void reportAppStarted();

	void onApplicationPause(bool pause);

	void setAge(int age);

	void setGender(string gender);

	void setMediationSegment(string segment);

	string getAdvertiserId();

	void validateIntegration();

	void shouldTrackNetworkState(bool track);

	bool setDynamicUserId(string dynamicUserId);

	void setAdaptersDebug(bool enabled);

	void setUserId(string userId);

	void init(string appKey);

	void init(string appKey, params string[] adUnits);

	void showRewardedVideo();

	void showRewardedVideo(string placementName);

	bool isRewardedVideoAvailable();

	bool isRewardedVideoPlacementCapped(string placementName);

	IronSourcePlacement getPlacementInfo(string name);

	void loadInterstitial();

	void showInterstitial();

	void showInterstitial(string placementName);

	bool isInterstitialReady();

	bool isInterstitialPlacementCapped(string placementName);

	void showOfferwall();

	void showOfferwall(string placementName);

	bool isOfferwallAvailable();

	void getOfferwallCredits();

	void loadBanner(IronSourceBannerSize size, IronSourceBannerPosition position);

	void loadBanner(IronSourceBannerSize size, IronSourceBannerPosition position, string placementName);

	void destroyBanner();

	bool isBannerPlacementCapped(string placementName);
}
