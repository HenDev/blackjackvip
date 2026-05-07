using System;
using System.Collections;
using UnityEngine;
#if MOBILE_INTEGRATIONS_HAS_GOOGLE_PLAY_REVIEW
using Google.Play.Review;
#endif
#if MOBILE_INTEGRATIONS_HAS_GOOGLE_PLAY_APPUPDATE
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif

public class InAppReviewManager : MonoBehaviour
{
    public MobileIntegrationConfig config;
    public bool IsBusy { get; private set; }
    public string LastError { get; private set; }

    public void RequestReviewFlow()
    {
        if (IsBusy)
        {
            Debug.LogWarning("InAppReviewManager: review flow is already running.");
            return;
        }

        StartCoroutine(InAppReviewProvider.RequestReviewFlow(this));
    }

    internal void SetBusy(bool isBusy)
    {
        IsBusy = isBusy;
    }

    internal void SetError(string error)
    {
        LastError = error;
        if (!string.IsNullOrWhiteSpace(error))
        {
            Debug.LogWarning("InAppReviewManager: " + error);
        }
    }
}

public class InAppUpdateManager : MonoBehaviour
{
    public MobileIntegrationConfig config;
    public bool IsBusy { get; private set; }
    public bool IsUpdateAvailable { get; private set; }
    public bool IsFlexibleUpdateDownloaded { get; private set; }
    public string LastError { get; private set; }
    public string LastStatus { get; private set; }

    public void CheckForUpdate()
    {
        if (IsBusy)
        {
            Debug.LogWarning("InAppUpdateManager: update flow is already running.");
            return;
        }

        StartCoroutine(InAppUpdateProvider.CheckForUpdate(this));
    }

    public void StartConfiguredUpdate()
    {
        if (config != null && config.preferImmediateAppUpdate)
        {
            StartImmediateUpdate();
            return;
        }

        StartFlexibleUpdate();
    }

    public void StartFlexibleUpdate()
    {
        if (IsBusy)
        {
            Debug.LogWarning("InAppUpdateManager: update flow is already running.");
            return;
        }

        StartCoroutine(InAppUpdateProvider.StartUpdate(this, false));
    }

    public void StartImmediateUpdate()
    {
        if (IsBusy)
        {
            Debug.LogWarning("InAppUpdateManager: update flow is already running.");
            return;
        }

        StartCoroutine(InAppUpdateProvider.StartUpdate(this, true));
    }

    public void CompleteFlexibleUpdate()
    {
        if (IsBusy)
        {
            Debug.LogWarning("InAppUpdateManager: update flow is already running.");
            return;
        }

        StartCoroutine(InAppUpdateProvider.CompleteFlexibleUpdate(this));
    }

    internal void SetBusy(bool isBusy)
    {
        IsBusy = isBusy;
    }

    internal void SetError(string error)
    {
        LastError = error;
        if (!string.IsNullOrWhiteSpace(error))
        {
            Debug.LogWarning("InAppUpdateManager: " + error);
        }
    }

    internal void SetStatus(string status)
    {
        LastStatus = status;
        if (!string.IsNullOrWhiteSpace(status))
        {
            Debug.Log("InAppUpdateManager: " + status);
        }
    }

    internal void SetUpdateAvailability(bool isAvailable)
    {
        IsUpdateAvailable = isAvailable;
    }

    internal void SetFlexibleUpdateDownloaded(bool isDownloaded)
    {
        IsFlexibleUpdateDownloaded = isDownloaded;
    }
}

internal static class InAppReviewProvider
{
    public static IEnumerator RequestReviewFlow(InAppReviewManager manager)
    {
        manager.SetBusy(true);
        manager.SetError(null);

#if MOBILE_INTEGRATIONS_HAS_GOOGLE_PLAY_REVIEW && UNITY_ANDROID && !UNITY_EDITOR
        ReviewManager reviewManager = new ReviewManager();
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;

        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            manager.SetError("RequestReviewFlow failed: " + requestFlowOperation.Error);
            manager.SetBusy(false);
            yield break;
        }

        var playReviewInfo = requestFlowOperation.GetResult();
        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;

        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            manager.SetError("LaunchReviewFlow failed: " + launchFlowOperation.Error);
        }
        else
        {
            manager.SetStatus("In-app review flow completed.");
        }
#else
        manager.SetError("Google Play In-App Review is only available on Android with com.google.play.review installed.");
        yield return null;
#endif

        manager.SetBusy(false);
    }
}

internal static class InAppUpdateProvider
{
#if MOBILE_INTEGRATIONS_HAS_GOOGLE_PLAY_APPUPDATE && UNITY_ANDROID && !UNITY_EDITOR
    private static AppUpdateManager _appUpdateManager;
    private static AppUpdateInfo _cachedUpdateInfo;

    private static AppUpdateManager GetManager()
    {
        if (_appUpdateManager == null)
        {
            _appUpdateManager = new AppUpdateManager();
        }

        return _appUpdateManager;
    }
#endif

    public static IEnumerator CheckForUpdate(InAppUpdateManager manager)
    {
        manager.SetBusy(true);
        manager.SetError(null);
        manager.SetFlexibleUpdateDownloaded(false);

#if MOBILE_INTEGRATIONS_HAS_GOOGLE_PLAY_APPUPDATE && UNITY_ANDROID && !UNITY_EDITOR
        var updateInfoOperation = GetManager().GetAppUpdateInfo();
        yield return updateInfoOperation;

        if (updateInfoOperation.Error != AppUpdateErrorCode.NoError)
        {
            manager.SetError("GetAppUpdateInfo failed: " + updateInfoOperation.Error);
            manager.SetUpdateAvailability(false);
            manager.SetBusy(false);
            yield break;
        }

        _cachedUpdateInfo = updateInfoOperation.GetResult();
        bool isAvailable = _cachedUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable ||
                           _cachedUpdateInfo.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress;
        manager.SetUpdateAvailability(isAvailable);
        manager.SetFlexibleUpdateDownloaded(_cachedUpdateInfo.InstallStatus == InstallStatus.Downloaded);
        manager.SetStatus("Update availability: " + _cachedUpdateInfo.UpdateAvailability + ", install status: " + _cachedUpdateInfo.InstallStatus);
#else
        manager.SetError("Google Play In-App Update is only available on Android with com.google.play.appupdate installed.");
        manager.SetUpdateAvailability(false);
        yield return null;
#endif

        manager.SetBusy(false);
    }

    public static IEnumerator StartUpdate(InAppUpdateManager manager, bool immediate)
    {
        manager.SetBusy(true);
        manager.SetError(null);

#if MOBILE_INTEGRATIONS_HAS_GOOGLE_PLAY_APPUPDATE && UNITY_ANDROID && !UNITY_EDITOR
        if (_cachedUpdateInfo == null)
        {
            var infoOperation = GetManager().GetAppUpdateInfo();
            yield return infoOperation;
            if (infoOperation.Error != AppUpdateErrorCode.NoError)
            {
                manager.SetError("GetAppUpdateInfo failed: " + infoOperation.Error);
                manager.SetBusy(false);
                yield break;
            }

            _cachedUpdateInfo = infoOperation.GetResult();
        }

        AppUpdateOptions options = immediate
            ? AppUpdateOptions.ImmediateAppUpdateOptions(manager.config != null && manager.config.allowAssetPackDeletionForAppUpdate)
            : AppUpdateOptions.FlexibleAppUpdateOptions(manager.config != null && manager.config.allowAssetPackDeletionForAppUpdate);

        if (!_cachedUpdateInfo.IsUpdateTypeAllowed(options))
        {
            manager.SetError((immediate ? "Immediate" : "Flexible") + " app update is not allowed for the current update state.");
            manager.SetBusy(false);
            yield break;
        }

        var startUpdateOperation = GetManager().StartUpdate(_cachedUpdateInfo, options);
        yield return startUpdateOperation;

        if (startUpdateOperation.Error != AppUpdateErrorCode.NoError)
        {
            manager.SetError("StartUpdate failed: " + startUpdateOperation.Error);
            manager.SetBusy(false);
            yield break;
        }

        manager.SetStatus((immediate ? "Immediate" : "Flexible") + " app update flow started.");

        if (!immediate)
        {
            var refreshOperation = GetManager().GetAppUpdateInfo();
            yield return refreshOperation;
            if (refreshOperation.Error == AppUpdateErrorCode.NoError)
            {
                _cachedUpdateInfo = refreshOperation.GetResult();
                manager.SetFlexibleUpdateDownloaded(_cachedUpdateInfo.InstallStatus == InstallStatus.Downloaded);
                manager.SetUpdateAvailability(_cachedUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable ||
                                              _cachedUpdateInfo.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress);
                manager.SetStatus("Flexible update install status: " + _cachedUpdateInfo.InstallStatus);
            }
        }
#else
        manager.SetError("Google Play In-App Update is only available on Android with com.google.play.appupdate installed.");
        yield return null;
#endif

        manager.SetBusy(false);
    }

    public static IEnumerator CompleteFlexibleUpdate(InAppUpdateManager manager)
    {
        manager.SetBusy(true);
        manager.SetError(null);

#if MOBILE_INTEGRATIONS_HAS_GOOGLE_PLAY_APPUPDATE && UNITY_ANDROID && !UNITY_EDITOR
        var completeUpdateOperation = GetManager().CompleteUpdate();
        yield return completeUpdateOperation;

        if (completeUpdateOperation.Error != AppUpdateErrorCode.NoError)
        {
            manager.SetError("CompleteUpdate failed: " + completeUpdateOperation.Error);
            manager.SetBusy(false);
            yield break;
        }

        manager.SetFlexibleUpdateDownloaded(false);
        manager.SetStatus("Flexible update completed.");
#else
        manager.SetError("Google Play In-App Update is only available on Android with com.google.play.appupdate installed.");
        yield return null;
#endif

        manager.SetBusy(false);
    }
}

