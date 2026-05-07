using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
#if MOBILE_INTEGRATIONS_HAS_GOOGLE_MOBILE_ADS
using GoogleMobileAds.Ump.Api;
#endif

public class MobileManagerRoot : MonoBehaviour
{
    private static MobileManagerRoot instance;

    public MobileIntegrationConfig config;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    } 
}

public class AdsManager : MonoBehaviour
{
    public MobileIntegrationConfig config;
    public bool IsInitialized { get; private set; }
    public bool IsRewardedReady { get; private set; }
    public bool IsInterstitialReady { get; private set; }
    public string LastError { get; private set; }
    private bool _initializationRequested;

    private void Awake()
    {
        if (MobilePrivacyConsentState.CanInitializeSdk(config))
        {
            InitializeAds();
        }
    }

    public void InitializeAds()
    {
        if (IsInitialized || _initializationRequested)
        {
            return;
        }

        if (config == null)
        {
            Debug.LogWarning("AdsManager: missing MobileIntegrationConfig.");
            return;
        }

        _initializationRequested = true;
        LevelPlayAdsProvider.Initialize(this);
    }

    public void LoadRewarded()
    {
        LevelPlayAdsProvider.LoadRewarded(this);
    }

    public void LoadInterstitial()
    {
        LevelPlayAdsProvider.LoadInterstitial(this);
    }

    public void ShowInterstitial()
    {
        ShowInterstitial(config != null ? config.interstitialPlacement : string.Empty);
    }

    public void ShowInterstitial(string placement)
    {
        LevelPlayAdsProvider.ShowInterstitial(this, placement);
    }

    public void ShowRewarded()
    {
        ShowRewarded(config != null ? config.rewardedPlacement : string.Empty);
    }

    public void ShowRewarded(string placement)
    {
        LevelPlayAdsProvider.ShowRewarded(this, placement);
    }

    public void LaunchTestSuite()
    {
        LevelPlayAdsProvider.LaunchTestSuite(this);
    }

    internal void SetInitializationState(bool initialized, string errorMessage)
    {
        IsInitialized = initialized;
        LastError = errorMessage;

        if (initialized)
        {
            Debug.Log("AdsManager: LevelPlay initialized successfully.");
        }
        else if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            Debug.LogWarning("AdsManager: initialization failed. " + errorMessage);
        }
    }

    internal void SetRewardedReady(bool isReady, string reason = null)
    {
        IsRewardedReady = isReady;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Debug.Log(isReady
                ? "AdsManager: rewarded ready. " + reason
                : "AdsManager: rewarded unavailable. " + reason);
        }
    }

    internal void SetInterstitialReady(bool isReady, string reason = null)
    {
        IsInterstitialReady = isReady;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            Debug.Log(isReady
                ? "AdsManager: interstitial ready. " + reason
                : "AdsManager: interstitial unavailable. " + reason);
        }
    }

    internal void NotifyReward(string rewardName, int rewardAmount)
    {
        Debug.Log("AdsManager: reward received. Name=" + rewardName + " Amount=" + rewardAmount);
    }

    internal void NotifyAdFailure(string reason)
    {
        LastError = reason;
        Debug.LogWarning("AdsManager: " + reason);
    }
}

public class IAPManager : MonoBehaviour
{
    public MobileIntegrationConfig config;
    public bool IsInitialized { get; private set; }
    public string LastError { get; private set; }
    private bool _initializationRequested;

    [SerializeField] private List<string> registeredProducts = new List<string>();

    public IReadOnlyList<string> RegisteredProducts
    {
        get { return registeredProducts; }
    }

    private void Awake()
    {
        if (MobilePrivacyConsentState.CanInitializeSdk(config))
        {
            InitializePurchases();
        }
    }

    public void InitializePurchases()
    {
        if (IsInitialized || _initializationRequested)
        {
            return;
        }

        if (config == null)
        {
            Debug.LogWarning("IAPManager: missing MobileIntegrationConfig.");
            return;
        }

        _initializationRequested = true;
        UnityIapProvider.Initialize(this);
    }

    public bool HasProduct(string productId)
    {
        return !string.IsNullOrWhiteSpace(productId) && registeredProducts.Contains(productId);
    }

    public void BuyProduct(string productId)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            Debug.LogWarning("IAPManager: product id is empty.");
            return;
        }

        UnityIapProvider.BuyProduct(this, productId);
    }

    public void RestorePurchases()
    {
        UnityIapProvider.RestorePurchases(this);
    }

    internal IEnumerable<string> GetConfiguredProductIds()
    {
        var added = new HashSet<string>();

        if (config == null)
        {
            yield break;
        }

        for (int i = 0; i < config.consumableIds.Count; i++)
        {
            string productId = config.consumableIds[i];
            if (!string.IsNullOrWhiteSpace(productId) && added.Add(productId))
            {
                yield return productId;
            }
        }

        for (int i = 0; i < config.nonConsumableIds.Count; i++)
        {
            string productId = config.nonConsumableIds[i];
            if (!string.IsNullOrWhiteSpace(productId) && added.Add(productId))
            {
                yield return productId;
            }
        }
    }

    internal bool IsNonConsumable(string productId)
    {
        return config != null && config.nonConsumableIds.Contains(productId);
    }

    internal void SetInitializationState(bool initialized, IEnumerable<string> productIds, string errorMessage)
    {
        IsInitialized = initialized;
        LastError = errorMessage;
        registeredProducts.Clear();

        if (productIds != null)
        {
            foreach (string productId in productIds)
            {
                if (!string.IsNullOrWhiteSpace(productId) && !registeredProducts.Contains(productId))
                {
                    registeredProducts.Add(productId);
                }
            }
        }

        if (initialized)
        {
            Debug.Log("IAPManager: initialized with " + registeredProducts.Count + " products.");
        }
        else if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            Debug.LogWarning("IAPManager: initialization failed. " + errorMessage);
        }
    }

    internal void NotifyPurchaseSucceeded(string productId)
    {
        Debug.Log("IAPManager: purchase succeeded for " + productId);
    }

    internal void NotifyPurchaseFailed(string productId, string reason)
    {
        Debug.LogWarning("IAPManager: purchase failed for " + productId + ". " + reason);
    }
}

public class NotificationManager : MonoBehaviour
{
    public MobileIntegrationConfig config;
    public bool IsInitialized { get; private set; }
    public string LastError { get; private set; }
    private bool _initializationRequested;

    private void Awake()
    {
        if (MobilePrivacyConsentState.CanInitializeSdk(config))
        {
            InitializeNotifications();
        }
    }

    public void InitializeNotifications()
    {
        if (IsInitialized || _initializationRequested)
        {
            return;
        }

        if (config == null)
        {
            Debug.LogWarning("NotificationManager: missing MobileIntegrationConfig.");
            return;
        }

        _initializationRequested = true;
        MobileNotificationsProvider.Initialize(this);
    }

    public void ScheduleDefaultNotification()
    {
        if (config == null)
        {
            Debug.LogWarning("NotificationManager: cannot schedule notification without config.");
            return;
        }

        SendLocalNotification(
            config.notificationTitle,
            config.notificationMessage,
            config.notificationDelaySeconds);
    }

    public void SendLocalNotification(string title, string message, int delaySeconds)
    {
        MobileNotificationsProvider.Schedule(this, title, message, delaySeconds);
    }

    public void CancelAllNotifications()
    {
        MobileNotificationsProvider.CancelAll(this);
    }

    internal void SetInitializationState(bool initialized, string errorMessage)
    {
        IsInitialized = initialized;
        LastError = errorMessage;

        if (initialized)
        {
            Debug.Log("NotificationManager: initialized successfully.");
        }
        else if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            Debug.LogWarning("NotificationManager: initialization failed. " + errorMessage);
        }
    }

    internal string GetAndroidChannelId()
    {
        return config != null && !string.IsNullOrWhiteSpace(config.androidNotificationChannelId)
            ? config.androidNotificationChannelId
            : "mobile_integrations_default";
    }

    internal string GetAndroidChannelName()
    {
        return config != null && !string.IsNullOrWhiteSpace(config.androidNotificationChannelName)
            ? config.androidNotificationChannelName
            : "General Notifications";
    }
}

public class PrivacyManager : MonoBehaviour
{
    public MobileIntegrationConfig config;
    public bool ConsentGranted { get; private set; }
    public bool IsPrivacyFlowInProgress { get; private set; }
    public string LastPrivacyError { get; private set; }

    private bool _privacyPrepared;
    private Action<int> _onTrackingAuthorizationCompleted;

    private void Awake()
    {
        InitializePrivacy();
    }

    public void InitializePrivacy()
    {
        if (_privacyPrepared)
        {
            return;
        }

        if (config == null)
        {
            Debug.LogWarning("PrivacyManager: missing MobileIntegrationConfig.");
            return;
        }

        _privacyPrepared = true;
        Debug.Log("PrivacyManager: privacy flow prepared. GDPR=" + config.gdprRequired + ", ATT=" + config.requestAttOnStartup);
    }

    public void RequestStartupPrivacyConsent(Action onCompleted)
    {
        if (IsPrivacyFlowInProgress)
        {
            return;
        }

        StartCoroutine(RequestStartupPrivacyConsentRoutine(onCompleted));
    }

    private IEnumerator RequestStartupPrivacyConsentRoutine(Action onCompleted)
    {
        IsPrivacyFlowInProgress = true;
        InitializePrivacy();

        bool requiresGdprConsent = RequiresGdprConsent();
        bool requiresAttAuthorization = RequiresAttAuthorization();
        bool gdprGranted = !requiresGdprConsent;
        bool attGranted = !requiresAttAuthorization;
        bool gdprResolved = !requiresGdprConsent;
        bool attResolved = !requiresAttAuthorization;
        bool gdprRequestCompleted = !requiresGdprConsent;
        bool attRequestCompleted = !requiresAttAuthorization;

        if (requiresGdprConsent)
        {
            RequestGdprConsent((granted, resolved) =>
            {
                gdprGranted = granted;
                gdprResolved = resolved;
                gdprRequestCompleted = true;
            });

            while (!gdprRequestCompleted)
            {
                yield return null;
            }
        }

        if (requiresAttAuthorization)
        {
            RequestTrackingAuthorization((granted, resolved) =>
            {
                attGranted = granted;
                attResolved = resolved;
                attRequestCompleted = true;
            });

            while (!attRequestCompleted)
            {
                yield return null;
            }
        }

        if (gdprResolved && attResolved)
        {
            SetConsent(gdprGranted, attGranted);
        }
        else
        {
            MobilePrivacyConsentState.ClearConsent();
            Debug.LogWarning("PrivacyManager: privacy flow did not resolve completely. Consent state was not persisted.");
        }

        IsPrivacyFlowInProgress = false;

        if (onCompleted != null)
        {
            onCompleted();
        }
    }

    public void RequestGdprConsent(Action<bool, bool> onCompleted)
    {
        if (config == null || !config.gdprRequired)
        {
            if (onCompleted != null)
            {
                onCompleted(true, true);
            }

            return;
        }

#if MOBILE_INTEGRATIONS_HAS_GOOGLE_MOBILE_ADS
        var requestParameters = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false
        };

        if (config.forceGdprDebugEeaInTestMode)
        {
            var debugSettings = new ConsentDebugSettings
            {
                DebugGeography = DebugGeography.EEA
            };

            if (config.umpTestDeviceHashedIds != null)
            {
                for (int i = 0; i < config.umpTestDeviceHashedIds.Count; i++)
                {
                    string hashedId = config.umpTestDeviceHashedIds[i];
                    if (!string.IsNullOrWhiteSpace(hashedId))
                    {
                        debugSettings.TestDeviceHashedIds.Add(hashedId.Trim());
                    }
                }
            }

            requestParameters.ConsentDebugSettings = debugSettings;
            Debug.Log("PrivacyManager: UMP debug geography forced to EEA for test mode.");
        }

        ConsentInformation.Update(requestParameters, updateError =>
        {
            if (updateError != null)
            {
                LastPrivacyError = updateError.Message;
                Debug.LogWarning("PrivacyManager: UMP consent update failed. " + LastPrivacyError);

                if (onCompleted != null)
                {
                    bool canRequestAds = ConsentInformation.CanRequestAds();
                    if (!canRequestAds && config.allowAdsWhenUmpUpdateFails)
                    {
                        Debug.LogWarning("PrivacyManager: continuing with ads because allowAdsWhenUmpUpdateFails is enabled.");
                        onCompleted(true, true);
                    }
                    else
                    {
                        onCompleted(canRequestAds, canRequestAds);
                    }
                }

                return;
            }

            if (ConsentInformation.CanRequestAds())
            {
                LastPrivacyError = null;
                Debug.Log("PrivacyManager: GDPR consent already available or not required.");

                if (onCompleted != null)
                {
                    onCompleted(true, true);
                }

                return;
            }

            ConsentForm.LoadAndShowConsentFormIfRequired(showError =>
            {
                if (showError != null)
                {
                    LastPrivacyError = showError.Message;
                    Debug.LogWarning("PrivacyManager: UMP consent form failed. " + LastPrivacyError);
                }
                else
                {
                    LastPrivacyError = null;
                    Debug.Log("PrivacyManager: GDPR consent form completed.");
                }

                bool canRequestAds = ConsentInformation.CanRequestAds();
                if (onCompleted != null)
                {
                    onCompleted(canRequestAds, true);
                }
            });
        });
#else
        LastPrivacyError = "Google UMP package is not installed. GDPR native consent is unavailable.";
        Debug.LogWarning("PrivacyManager: " + LastPrivacyError);
        if (onCompleted != null)
        {
            onCompleted(false, false);
        }
#endif
    }

    public void RequestTrackingAuthorization(Action<bool, bool> onCompleted)
    {
        StartCoroutine(RequestTrackingAuthorizationRoutine(onCompleted));
    }

    private IEnumerator RequestTrackingAuthorizationRoutine(Action<bool, bool> onCompleted)
    {
        if (config == null || !config.requestAttOnStartup)
        {
            if (onCompleted != null)
            {
                onCompleted(true, true);
            }

            yield break;
        }

        bool requestCompleted = false;
        bool granted = false;

#if UNITY_IOS && !UNITY_EDITOR
        int currentStatus = IosAppTrackingTransparencyBridge.GetTrackingAuthorizationStatus();
        if (currentStatus == IosAppTrackingTransparencyBridge.AuthorizedStatus)
        {
            granted = true;
            requestCompleted = true;
        }
        else if (currentStatus == IosAppTrackingTransparencyBridge.DeniedStatus || currentStatus == IosAppTrackingTransparencyBridge.RestrictedStatus)
        {
            granted = false;
            requestCompleted = true;
        }
        else
        {
            string callbackTarget = gameObject.name;
            _onTrackingAuthorizationCompleted = status =>
            {
                granted = status == IosAppTrackingTransparencyBridge.AuthorizedStatus;
                requestCompleted = true;
            };
            IosAppTrackingTransparencyBridge.RequestTrackingAuthorization(callbackTarget, nameof(OnIosTrackingAuthorizationCompleted));
        }
#else
        granted = true;
        requestCompleted = true;
#endif

        while (!requestCompleted)
        {
            yield return null;
        }

        Debug.Log("PrivacyManager: ATT authorization resolved to " + granted);

        if (onCompleted != null)
        {
            onCompleted(granted, true);
        }
    }

    public void OnIosTrackingAuthorizationCompleted(string statusValue)
    {
        int status;
        if (!int.TryParse(statusValue, out status))
        {
            status = IosAppTrackingTransparencyBridge.DeniedStatus;
        }

        if (_onTrackingAuthorizationCompleted != null)
        {
            _onTrackingAuthorizationCompleted(status);
            _onTrackingAuthorizationCompleted = null;
        }
    }

    public bool ShowPrivacyOptionsForm()
    {
#if MOBILE_INTEGRATIONS_HAS_GOOGLE_MOBILE_ADS
        if (ConsentInformation.PrivacyOptionsRequirementStatus != PrivacyOptionsRequirementStatus.Required)
        {
            Debug.Log("PrivacyManager: privacy options form is not currently required.");
            return false;
        }

        ConsentForm.ShowPrivacyOptionsForm(showError =>
        {
            if (showError != null)
            {
                LastPrivacyError = showError.Message;
                Debug.LogWarning("PrivacyManager: failed to show privacy options form. " + LastPrivacyError);
                return;
            }

            LastPrivacyError = null;
            Debug.Log("PrivacyManager: privacy options form closed successfully.");
        });

        return true;
#else
        LastPrivacyError = "Google UMP package is not installed. ShowPrivacyOptionsForm is unavailable.";
        Debug.LogWarning("PrivacyManager: " + LastPrivacyError);
        return false;
#endif
    }

    private bool RequiresGdprConsent()
    {
        return config != null && config.gdprRequired && !MobilePrivacyConsentState.HasResolvedConsent;
    }

    private bool RequiresAttAuthorization()
    {
        return config != null && config.requestAttOnStartup && !MobilePrivacyConsentState.HasResolvedConsent;
    }

    public void SetConsent(bool granted)
    {
        SetConsent(granted, granted);
    }

    public void SetConsent(bool gdprGranted, bool attGranted)
    {
        ConsentGranted = gdprGranted;
        MobilePrivacyConsentState.SaveConsent(gdprGranted, attGranted);
        Debug.Log("PrivacyManager: consent state changed. GDPR=" + gdprGranted + ", ATT=" + attGranted);
    }
}

public static class MobilePrivacyConsentState
{
    private const string ConsentResolvedKey = "MobilePrivacyConsentResolved";
    private const string GdprConsentKey = "MobilePrivacyGdprConsent";
    private const string AttConsentKey = "MobilePrivacyAttConsent";

    public static bool HasResolvedConsent
    {
        get { return PlayerPrefs.GetInt(ConsentResolvedKey, 0) == 1; }
    }

    public static bool HasGdprConsent
    {
        get { return PlayerPrefs.GetInt(GdprConsentKey, 0) == 1; }
    }

    public static bool HasAttConsent
    {
        get { return PlayerPrefs.GetInt(AttConsentKey, 0) == 1; }
    }

    public static bool CanInitializeSdk(MobileIntegrationConfig config)
    {
        if (config == null)
        {
            return true;
        }

        bool requiresConsent = config.gdprRequired || config.requestAttOnStartup;
        return !requiresConsent || HasResolvedConsent;
    }

    public static void SaveConsent(bool gdprGranted, bool attGranted)
    {
        PlayerPrefs.SetInt(ConsentResolvedKey, 1);
        PlayerPrefs.SetInt(GdprConsentKey, gdprGranted ? 1 : 0);
        PlayerPrefs.SetInt(AttConsentKey, attGranted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ClearConsent()
    {
        PlayerPrefs.DeleteKey(ConsentResolvedKey);
        PlayerPrefs.DeleteKey(GdprConsentKey);
        PlayerPrefs.DeleteKey(AttConsentKey);
        PlayerPrefs.Save();
    }
}

public class InternetChecker : MonoBehaviour
{
    public bool logWhenOffline = true;

    public bool HasInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    private void Update()
    {
        if (logWhenOffline && !HasInternet())
        {
            Debug.Log("InternetChecker: no internet connection available.");
        }
    }
}



