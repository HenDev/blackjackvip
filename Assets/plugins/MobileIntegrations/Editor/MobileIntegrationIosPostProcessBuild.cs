using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class MobileIntegrationIosPostProcessBuild
{
    [PostProcessBuild(900)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }

        Type plistDocumentType = Type.GetType("UnityEditor.iOS.Xcode.PlistDocument, UnityEditor.iOS.Extensions.Xcode");
        if (plistDocumentType == null)
        {
            Debug.LogWarning("MobileIntegrationIosPostProcessBuild: UnityEditor.iOS.Extensions.Xcode is not available. NSUserTrackingUsageDescription was not injected.");
            return;
        }

        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
        object plist = Activator.CreateInstance(plistDocumentType);
        plistDocumentType.GetMethod("ReadFromFile")?.Invoke(plist, new object[] { plistPath });

        object root = plistDocumentType.GetProperty("root")?.GetValue(plist, null);
        if (root == null)
        {
            Debug.LogWarning("MobileIntegrationIosPostProcessBuild: Failed to access Info.plist root document.");
            return;
        }

        MobileIntegrationConfig config = FindConfig();
        string usageDescription = config != null && !string.IsNullOrWhiteSpace(config.iosTrackingUsageDescription)
            ? config.iosTrackingUsageDescription
            : "This identifier will be used to deliver more relevant ads and improve monetization.";

        root.GetType().GetMethod("SetString")?.Invoke(root, new object[]
        {
            "NSUserTrackingUsageDescription",
            usageDescription
        });

        plistDocumentType.GetMethod("WriteToFile")?.Invoke(plist, new object[] { plistPath });
    }

    private static MobileIntegrationConfig FindConfig()
    {
        string[] guids = AssetDatabase.FindAssets("t:MobileIntegrationConfig");
        string assetPath = guids.Select(AssetDatabase.GUIDToAssetPath).FirstOrDefault();
        if (string.IsNullOrWhiteSpace(assetPath))
        {
            return null;
        }

        return AssetDatabase.LoadAssetAtPath<MobileIntegrationConfig>(assetPath);
    }
}
