using UnityEngine;
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
using Unity.Services.LevelPlay;
#endif

internal static class LevelPlayAdsProvider
{
    public static void Initialize(AdsManager manager)
    {
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
        if (manager.config == null)
        {
            manager.NotifyAdFailure("Missing MobileIntegrationConfig.");
            return;
        }

        if (string.IsNullOrWhiteSpace(manager.config.GetLevelPlayAppKey()))
        {
            manager.SetInitializationState(false, "LevelPlay app key is empty.");
            return;
        }

        LevelPlayAdsBridge bridge = manager.GetComponent<LevelPlayAdsBridge>();
        if (bridge == null)
        {
            bridge = manager.gameObject.AddComponent<LevelPlayAdsBridge>();
        }

        bridge.Initialize(manager);
#else
        manager.SetInitializationState(false, "LevelPlay package is not installed.");
#endif
    }

    public static void LoadRewarded(AdsManager manager)
    {
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
        LevelPlayAdsBridge bridge = manager.GetComponent<LevelPlayAdsBridge>();
        if (bridge == null)
        {
            manager.NotifyAdFailure("LevelPlay bridge is missing.");
            return;
        }

        bridge.LoadRewarded();
#else
        manager.NotifyAdFailure("LevelPlay package is not installed.");
#endif
    }

    public static void LoadInterstitial(AdsManager manager)
    {
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
        LevelPlayAdsBridge bridge = manager.GetComponent<LevelPlayAdsBridge>();
        if (bridge == null)
        {
            manager.NotifyAdFailure("LevelPlay bridge is missing.");
            return;
        }

        bridge.LoadInterstitial();
#else
        manager.NotifyAdFailure("LevelPlay package is not installed.");
#endif
    }

    public static void LoadBanner(AdsManager manager, bool isOnTop)
    {
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
        LevelPlayAdsBridge bridge = manager.GetComponent<LevelPlayAdsBridge>();
        if (bridge == null)
        {
            manager.NotifyAdFailure("LevelPlay bridge is missing.");
            return;
        }

        bridge.LoadBanner(isOnTop);
#else
        manager.NotifyAdFailure("LevelPlay package is not installed.");
#endif
    }

    public static void ShowRewarded(AdsManager manager, string placement)
    {
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
        LevelPlayAdsBridge bridge = manager.GetComponent<LevelPlayAdsBridge>();
        if (bridge == null)
        {
            manager.NotifyAdFailure("LevelPlay bridge is missing.");
            return;
        }

        bridge.ShowRewarded(placement);
#else
        manager.NotifyAdFailure("LevelPlay package is not installed.");
#endif
    }

    public static void ShowInterstitial(AdsManager manager, string placement)
    {
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
        LevelPlayAdsBridge bridge = manager.GetComponent<LevelPlayAdsBridge>();
        if (bridge == null)
        {
            manager.NotifyAdFailure("LevelPlay bridge is missing.");
            return;
        }

        bridge.ShowInterstitial(placement);
#else
        manager.NotifyAdFailure("LevelPlay package is not installed.");
#endif
    }

    public static void ShowBanner(AdsManager manager, bool isOnTop)
    {
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
        LevelPlayAdsBridge bridge = manager.GetComponent<LevelPlayAdsBridge>();
        if (bridge == null)
        {
            manager.NotifyAdFailure("LevelPlay bridge is missing.");
            return;
        }

        bridge.ShowBanner(isOnTop);
#else
        manager.NotifyAdFailure("LevelPlay package is not installed.");
#endif
    }

    public static void HideBanner(AdsManager manager)
    {
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
        LevelPlayAdsBridge bridge = manager.GetComponent<LevelPlayAdsBridge>();
        if (bridge == null)
        {
            manager.NotifyAdFailure("LevelPlay bridge is missing.");
            return;
        }

        bridge.HideBanner();
#else
        manager.NotifyAdFailure("LevelPlay package is not installed.");
#endif
    }

    public static void DestroyBanner(AdsManager manager)
    {
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
        LevelPlayAdsBridge bridge = manager.GetComponent<LevelPlayAdsBridge>();
        if (bridge == null)
        {
            manager.NotifyAdFailure("LevelPlay bridge is missing.");
            return;
        }

        bridge.DestroyBanner();
#else
        manager.NotifyAdFailure("LevelPlay package is not installed.");
#endif
    }

    public static void LaunchTestSuite(AdsManager manager)
    {
#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
        LevelPlayAdsBridge bridge = manager.GetComponent<LevelPlayAdsBridge>();
        if (bridge == null)
        {
            manager.NotifyAdFailure("LevelPlay bridge is missing.");
            return;
        }

        bridge.LaunchTestSuite();
#else
        manager.NotifyAdFailure("LevelPlay package is not installed.");
#endif
    }
}

#if MOBILE_INTEGRATIONS_HAS_LEVELPLAY
internal sealed class LevelPlayAdsBridge : MonoBehaviour
{
    private AdsManager _manager;
    private LevelPlayRewardedAd _rewardedAd;
    private LevelPlayInterstitialAd _interstitialAd;
    private LevelPlayBannerAd _bannerAd;
    private bool _isBannerLoaded;
    private bool _isBannerOnTop;
    private bool _bannerDisplayOnLoad;
    private bool _subscribedToSdkEvents;

    public void Initialize(AdsManager manager)
    {
        _manager = manager;

        LevelPlay.SetConsent(MobilePrivacyConsentState.HasGdprConsent);

        if (_manager.config.enableLevelPlayTestSuite)
        {
            LevelPlay.SetMetaData("is_test_suite", "enable");
        }

        SubscribeSdkEvents();

        if (string.IsNullOrWhiteSpace(_manager.config.levelPlayUserId))
        {
            LevelPlay.Init(_manager.config.GetLevelPlayAppKey());
        }
        else
        {
            LevelPlay.Init(_manager.config.GetLevelPlayAppKey(), _manager.config.levelPlayUserId);
        }
    }

    public void LoadRewarded()
    {
        if (_rewardedAd == null)
        {
            _manager.NotifyAdFailure("Rewarded ad unit id is missing or SDK is not initialized.");
            return;
        }

        _rewardedAd.LoadAd();
    }

    public void LoadInterstitial()
    {
        if (_interstitialAd == null)
        {
            _manager.NotifyAdFailure("Interstitial ad unit id is missing or SDK is not initialized.");
            return;
        }

        _interstitialAd.LoadAd();
    }

    public void LoadBanner(bool isOnTop)
    {
        LevelPlayBannerAd bannerAd = CreateBannerAd(isOnTop, false);
        if (bannerAd == null)
        {
            return;
        }

        _isBannerLoaded = false;
        _manager.SetBannerReady(false, "Banner ad loading.");
        bannerAd.LoadAd();
    }

    public void ShowRewarded(string placement)
    {
        if (_rewardedAd == null)
        {
            _manager.NotifyAdFailure("Rewarded ad is not initialized.");
            return;
        }

        if (!_rewardedAd.IsAdReady())
        {
            _manager.NotifyAdFailure("Rewarded ad is not ready.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(placement) && LevelPlayRewardedAd.IsPlacementCapped(placement))
        {
            _manager.NotifyAdFailure("Rewarded placement is capped: " + placement);
            return;
        }

        if (string.IsNullOrWhiteSpace(placement))
        {
            _rewardedAd.ShowAd();
        }
        else
        {
            _rewardedAd.ShowAd(placement);
        }
    }

    public void ShowInterstitial(string placement)
    {
        if (_interstitialAd == null)
        {
            _manager.NotifyAdFailure("Interstitial ad is not initialized.");
            return;
        }

        if (!_interstitialAd.IsAdReady())
        {
            _manager.NotifyAdFailure("Interstitial ad is not ready.");
            return;
        }

        if (string.IsNullOrWhiteSpace(placement))
        {
            _interstitialAd.ShowAd();
        }
        else
        {
            _interstitialAd.ShowAd(placement);
        }
    }

    public void ShowBanner(bool isOnTop)
    {
        LevelPlayBannerAd bannerAd = CreateBannerAd(isOnTop, true);
        if (bannerAd == null)
        {
            return;
        }

        if (_isBannerLoaded)
        {
            bannerAd.ShowAd();
            return;
        }

        if (!_bannerDisplayOnLoad)
        {
            DestroyBanner();
            bannerAd = CreateBannerAd(isOnTop, true);
            if (bannerAd == null)
            {
                return;
            }
        }

        _manager.SetBannerReady(false, "Banner ad loading.");
        bannerAd.LoadAd();
    }

    public void HideBanner()
    {
        if (_bannerAd == null)
        {
            _manager.NotifyAdFailure("Banner ad is not initialized.");
            return;
        }

        _bannerAd.HideAd();
    }

    public void DestroyBanner()
    {
        if (_bannerAd == null)
        {
            return;
        }

        UnsubscribeBannerEvents(_bannerAd);
        _bannerAd.DestroyAd();
        _bannerAd = null;
        _isBannerLoaded = false;
        _bannerDisplayOnLoad = false;
        _manager.SetBannerReady(false, "Banner ad destroyed.");
    }

    public void LaunchTestSuite()
    {
        LevelPlay.LaunchTestSuite();
    }

    private void SubscribeSdkEvents()
    {
        if (_subscribedToSdkEvents)
        {
            return;
        }

        LevelPlay.OnInitSuccess += OnInitSuccess;
        LevelPlay.OnInitFailed += OnInitFailed;
        _subscribedToSdkEvents = true;
    }

    private void OnInitSuccess(LevelPlayConfiguration configuration)
    {
        _manager.SetInitializationState(true, null);
        LevelPlay.ValidateIntegration();
        CreateRewardedAd();
        CreateInterstitialAd();
        LoadRewarded();
        LoadInterstitial();

        if (_manager.config.enableLevelPlayTestSuite)
        {
            LevelPlay.LaunchTestSuite();
        }
    }

    private void OnInitFailed(LevelPlayInitError error)
    {
        _manager.SetInitializationState(false, error != null ? error.ToString() : "LevelPlay init failed.");
    }

    private void CreateRewardedAd()
    {
        if (string.IsNullOrWhiteSpace(_manager.config.GetLevelPlayRewardedAdUnitId()) || _rewardedAd != null)
        {
            return;
        }

        _rewardedAd = new LevelPlayRewardedAd(_manager.config.GetLevelPlayRewardedAdUnitId());
        _rewardedAd.OnAdLoaded += OnRewardedLoaded;
        _rewardedAd.OnAdLoadFailed += OnRewardedLoadFailed;
        _rewardedAd.OnAdDisplayed += OnRewardedDisplayed;
        _rewardedAd.OnAdDisplayFailed += OnRewardedDisplayFailed;
        _rewardedAd.OnAdRewarded += OnRewardedRewarded;
        _rewardedAd.OnAdClosed += OnRewardedClosed;
    }

    private void CreateInterstitialAd()
    {
        if (string.IsNullOrWhiteSpace(_manager.config.GetLevelPlayInterstitialAdUnitId()) || _interstitialAd != null)
        {
            return;
        }

        _interstitialAd = new LevelPlayInterstitialAd(_manager.config.GetLevelPlayInterstitialAdUnitId());
        _interstitialAd.OnAdLoaded += OnInterstitialLoaded;
        _interstitialAd.OnAdLoadFailed += OnInterstitialLoadFailed;
        _interstitialAd.OnAdDisplayed += OnInterstitialDisplayed;
        _interstitialAd.OnAdDisplayFailed += OnInterstitialDisplayFailed;
        _interstitialAd.OnAdClosed += OnInterstitialClosed;
    }

    private LevelPlayBannerAd CreateBannerAd(bool isOnTop, bool displayOnLoad)
    {
        if (string.IsNullOrWhiteSpace(_manager.config.GetLevelPlayBannerAdUnitId()))
        {
            _manager.NotifyAdFailure("Banner ad unit id is missing or SDK is not initialized.");
            return null;
        }

        if (_bannerAd != null && _isBannerOnTop == isOnTop)
        {
            return _bannerAd;
        }

        DestroyBanner();

        LevelPlayBannerPosition position = isOnTop
            ? LevelPlayBannerPosition.TopCenter
            : LevelPlayBannerPosition.BottomCenter;

        LevelPlayBannerAd.Config config = new LevelPlayBannerAd.Config.Builder()
            .SetSize(LevelPlayAdSize.BANNER)
            .SetPosition(position)
            .SetPlacementName(_manager.config.bannerPlacement)
            .SetDisplayOnLoad(displayOnLoad)
            .SetRespectSafeArea(true)
            .Build();

        _bannerAd = new LevelPlayBannerAd(_manager.config.GetLevelPlayBannerAdUnitId(), config);
        _isBannerOnTop = isOnTop;
        _bannerDisplayOnLoad = displayOnLoad;
        _isBannerLoaded = false;
        SubscribeBannerEvents(_bannerAd);
        return _bannerAd;
    }

    private void SubscribeBannerEvents(LevelPlayBannerAd bannerAd)
    {
        bannerAd.OnAdLoaded += OnBannerLoaded;
        bannerAd.OnAdLoadFailed += OnBannerLoadFailed;
        bannerAd.OnAdDisplayed += OnBannerDisplayed;
        bannerAd.OnAdDisplayFailed += OnBannerDisplayFailed;
        bannerAd.OnAdClicked += OnBannerClicked;
        bannerAd.OnAdCollapsed += OnBannerCollapsed;
        bannerAd.OnAdExpanded += OnBannerExpanded;
        bannerAd.OnAdLeftApplication += OnBannerLeftApplication;
    }

    private void UnsubscribeBannerEvents(LevelPlayBannerAd bannerAd)
    {
        bannerAd.OnAdLoaded -= OnBannerLoaded;
        bannerAd.OnAdLoadFailed -= OnBannerLoadFailed;
        bannerAd.OnAdDisplayed -= OnBannerDisplayed;
        bannerAd.OnAdDisplayFailed -= OnBannerDisplayFailed;
        bannerAd.OnAdClicked -= OnBannerClicked;
        bannerAd.OnAdCollapsed -= OnBannerCollapsed;
        bannerAd.OnAdExpanded -= OnBannerExpanded;
        bannerAd.OnAdLeftApplication -= OnBannerLeftApplication;
    }

    private void OnRewardedLoaded(LevelPlayAdInfo adInfo)
    {
        _manager.SetRewardedReady(true, "Rewarded ad loaded.");
    }

    private void OnRewardedLoadFailed(LevelPlayAdError error)
    {
        _manager.SetRewardedReady(false, error != null ? error.ToString() : "Rewarded ad load failed.");
    }

    private void OnRewardedDisplayed(LevelPlayAdInfo adInfo)
    {
        _manager.SetRewardedReady(false, "Rewarded ad displayed.");
    }

    private void OnRewardedDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        _manager.SetRewardedReady(false, error != null ? error.ToString() : "Rewarded ad display failed.");
    }

    private void OnRewardedRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        string rewardName = reward != null ? reward.Name : "reward";
        int rewardAmount = reward != null ? reward.Amount : 0;
        _manager.NotifyReward(rewardName, rewardAmount);
    }

    private void OnRewardedClosed(LevelPlayAdInfo adInfo)
    {
        LoadRewarded();
    }

    private void OnInterstitialLoaded(LevelPlayAdInfo adInfo)
    {
        _manager.SetInterstitialReady(true, "Interstitial ad loaded.");
    }

    private void OnInterstitialLoadFailed(LevelPlayAdError error)
    {
        _manager.SetInterstitialReady(false, error != null ? error.ToString() : "Interstitial ad load failed.");
    }

    private void OnInterstitialDisplayed(LevelPlayAdInfo adInfo)
    {
        _manager.SetInterstitialReady(false, "Interstitial ad displayed.");
    }

    private void OnInterstitialDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        _manager.SetInterstitialReady(false, error != null ? error.ToString() : "Interstitial ad display failed.");
    }

    private void OnInterstitialClosed(LevelPlayAdInfo adInfo)
    {
        LoadInterstitial();
    }

    private void OnBannerLoaded(LevelPlayAdInfo adInfo)
    {
        _isBannerLoaded = true;
        _manager.SetBannerReady(true, "Banner ad loaded.");
    }

    private void OnBannerLoadFailed(LevelPlayAdError error)
    {
        _isBannerLoaded = false;
        _manager.SetBannerReady(false, error != null ? error.ToString() : "Banner ad load failed.");
    }

    private void OnBannerDisplayed(LevelPlayAdInfo adInfo)
    {
        _manager.SetBannerReady(true, "Banner ad displayed.");
    }

    private void OnBannerDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
    {
        _manager.SetBannerReady(false, error != null ? error.ToString() : "Banner ad display failed.");
    }

    private void OnBannerClicked(LevelPlayAdInfo adInfo)
    {
        Debug.Log("AdsManager: banner clicked.");
    }

    private void OnBannerCollapsed(LevelPlayAdInfo adInfo)
    {
        Debug.Log("AdsManager: banner collapsed.");
    }

    private void OnBannerExpanded(LevelPlayAdInfo adInfo)
    {
        Debug.Log("AdsManager: banner expanded.");
    }

    private void OnBannerLeftApplication(LevelPlayAdInfo adInfo)
    {
        Debug.Log("AdsManager: banner left application.");
    }

    private void OnDestroy()
    {
        if (_subscribedToSdkEvents)
        {
            LevelPlay.OnInitSuccess -= OnInitSuccess;
            LevelPlay.OnInitFailed -= OnInitFailed;
        }

        if (_rewardedAd != null)
        {
            _rewardedAd.OnAdLoaded -= OnRewardedLoaded;
            _rewardedAd.OnAdLoadFailed -= OnRewardedLoadFailed;
            _rewardedAd.OnAdDisplayed -= OnRewardedDisplayed;
            _rewardedAd.OnAdDisplayFailed -= OnRewardedDisplayFailed;
            _rewardedAd.OnAdRewarded -= OnRewardedRewarded;
            _rewardedAd.OnAdClosed -= OnRewardedClosed;
            _rewardedAd.DestroyAd();
        }

        if (_interstitialAd != null)
        {
            _interstitialAd.OnAdLoaded -= OnInterstitialLoaded;
            _interstitialAd.OnAdLoadFailed -= OnInterstitialLoadFailed;
            _interstitialAd.OnAdDisplayed -= OnInterstitialDisplayed;
            _interstitialAd.OnAdDisplayFailed -= OnInterstitialDisplayFailed;
            _interstitialAd.OnAdClosed -= OnInterstitialClosed;
            _interstitialAd.DestroyAd();
        }

        if (_bannerAd != null)
        {
            UnsubscribeBannerEvents(_bannerAd);
            _bannerAd.DestroyAd();
        }
    }
}
#endif
