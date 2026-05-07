using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MobileIntegrationConfig",
    menuName = "Mobile Integrations/Config")]
public class MobileIntegrationConfig : ScriptableObject
{
    [Header("Project")]
    public string companyName = "DefaultCompany";
    public string productName = "NewMobileGame";
    public string bundleVersion = "1.0.0";
    public string androidBundleId = "com.company.newmobilegame";
    public string iosBundleId = "com.company.newmobilegame";
    public int androidVersionCode = 1;
    public string iosBuildNumber = "1";
    public bool forcePortrait = true;

    [Header("LevelPlay Ads")]
    public string androidAppKey;
    public string iosAppKey;
    public string levelPlayUserId;
    public bool enableLevelPlayTestSuite;
    public string androidRewardedAdUnitId;
    public string iosRewardedAdUnitId;
    public string androidInterstitialAdUnitId;
    public string iosInterstitialAdUnitId;
    public string rewardedPlacement = "rewarded_default";
    public string interstitialPlacement = "interstitial_default";
    public string customAdsPackage;

    [Header("Google Mobile Ads")]
    public string googleMobileAdsAndroidAppId;
    public string googleMobileAdsIosAppId;

    [Header("Google Play")]
    public bool preferImmediateAppUpdate;
    public bool allowAssetPackDeletionForAppUpdate;

    [Header("IAP")]
    public List<string> consumableIds = new List<string>();
    public List<string> nonConsumableIds = new List<string>();

    [Header("Privacy")]
    public bool gdprRequired = true;
    public bool requestAttOnStartup = true;
    public bool allowAdsWhenUmpUpdateFails = false;
    public bool forceGdprDebugEeaInTestMode = false;
    public List<string> umpTestDeviceHashedIds = new List<string>();
    [TextArea(2, 4)]
    public string iosTrackingUsageDescription = "This identifier will be used to deliver more relevant ads and improve monetization.";

    [Header("Notifications")]
    public string notificationTitle = "We miss you";
    public string notificationMessage = "Come back and play again.";
    public int notificationDelaySeconds = 14400;
    public string androidNotificationChannelId = "mobile_integrations_default";
    public string androidNotificationChannelName = "General Notifications";

    public string GetLevelPlayAppKey()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return androidAppKey;
#elif UNITY_IOS && !UNITY_EDITOR
        return iosAppKey;
#else
        if (!string.IsNullOrWhiteSpace(androidAppKey))
        {
            return androidAppKey;
        }

        return iosAppKey;
#endif
    }

    public string GetLevelPlayRewardedAdUnitId()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return androidRewardedAdUnitId;
#elif UNITY_IOS && !UNITY_EDITOR
        return iosRewardedAdUnitId;
#else
        if (!string.IsNullOrWhiteSpace(androidRewardedAdUnitId))
        {
            return androidRewardedAdUnitId;
        }

        return iosRewardedAdUnitId;
#endif
    }

    public string GetLevelPlayInterstitialAdUnitId()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return androidInterstitialAdUnitId;
#elif UNITY_IOS && !UNITY_EDITOR
        return iosInterstitialAdUnitId;
#else
        if (!string.IsNullOrWhiteSpace(androidInterstitialAdUnitId))
        {
            return androidInterstitialAdUnitId;
        }

        return iosInterstitialAdUnitId;
#endif
    }
}
