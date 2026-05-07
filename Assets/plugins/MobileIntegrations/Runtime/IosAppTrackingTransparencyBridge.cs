using System.Runtime.InteropServices;
using UnityEngine;

internal static class IosAppTrackingTransparencyBridge
{
    public const int NotDeterminedStatus = 0;
    public const int RestrictedStatus = 1;
    public const int DeniedStatus = 2;
    public const int AuthorizedStatus = 3;

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern int MIGetTrackingAuthorizationStatus();

    [DllImport("__Internal")]
    private static extern void MIRequestTrackingAuthorization(string gameObjectName, string callbackMethod);
#endif

    public static int GetTrackingAuthorizationStatus()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return MIGetTrackingAuthorizationStatus();
#else
        return AuthorizedStatus;
#endif
    }

    public static void RequestTrackingAuthorization(string gameObjectName, string callbackMethod)
    {
#if UNITY_IOS && !UNITY_EDITOR
        MIRequestTrackingAuthorization(gameObjectName, callbackMethod);
#else
        Debug.Log("IosAppTrackingTransparencyBridge: ATT request skipped outside iOS runtime.");
#endif
    }
}
