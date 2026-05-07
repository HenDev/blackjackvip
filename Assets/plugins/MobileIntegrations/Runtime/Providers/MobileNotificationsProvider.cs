using System;
using UnityEngine;
#if MOBILE_INTEGRATIONS_HAS_MOBILE_NOTIFICATIONS && UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if MOBILE_INTEGRATIONS_HAS_MOBILE_NOTIFICATIONS && UNITY_IOS
using Unity.Notifications.iOS;
#endif

internal static class MobileNotificationsProvider
{
    public static void Initialize(NotificationManager manager)
    {
#if MOBILE_INTEGRATIONS_HAS_MOBILE_NOTIFICATIONS
#if UNITY_ANDROID
        RegisterAndroidChannel(manager);
#endif
        manager.SetInitializationState(true, null);
#else
        manager.SetInitializationState(false, "Unity Mobile Notifications package is not installed.");
#endif
    }

    public static void Schedule(NotificationManager manager, string title, string message, int delaySeconds)
    {
#if MOBILE_INTEGRATIONS_HAS_MOBILE_NOTIFICATIONS
        if (!manager.IsInitialized)
        {
            Initialize(manager);
        }

        int safeDelaySeconds = Mathf.Max(1, delaySeconds);

#if UNITY_ANDROID
        RegisterAndroidChannel(manager);
        var notification = new AndroidNotification
        {
            Title = title,
            Text = message,
            FireTime = DateTime.Now.AddSeconds(safeDelaySeconds)
        };
        AndroidNotificationCenter.SendNotification(notification, manager.GetAndroidChannelId());
#elif UNITY_IOS
        var notification = new iOSNotification
        {
            Identifier = Guid.NewGuid().ToString(),
            Title = title,
            Body = message,
            ShowInForeground = true,
            ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
            Trigger = new iOSNotificationTimeIntervalTrigger
            {
                TimeInterval = TimeSpan.FromSeconds(safeDelaySeconds),
                Repeats = false
            }
        };
        iOSNotificationCenter.ScheduleNotification(notification);
#else
        Debug.LogWarning("NotificationManager: local notifications are not supported on this platform.");
#endif
#else
        manager.SetInitializationState(false, "Unity Mobile Notifications package is not installed.");
#endif
    }

    public static void CancelAll(NotificationManager manager)
    {
#if MOBILE_INTEGRATIONS_HAS_MOBILE_NOTIFICATIONS
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#else
        Debug.LogWarning("NotificationManager: cancel notifications is not supported on this platform.");
#endif
#else
        manager.SetInitializationState(false, "Unity Mobile Notifications package is not installed.");
#endif
    }

#if MOBILE_INTEGRATIONS_HAS_MOBILE_NOTIFICATIONS && UNITY_ANDROID
    private static void RegisterAndroidChannel(NotificationManager manager)
    {
        var channel = new AndroidNotificationChannel
        {
            Id = manager.GetAndroidChannelId(),
            Name = manager.GetAndroidChannelName(),
            Importance = Importance.Default,
            Description = "Mobile Integrations notifications"
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
#endif
}

