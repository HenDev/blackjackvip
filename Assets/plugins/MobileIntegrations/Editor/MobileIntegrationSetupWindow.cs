using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class MobileIntegrationSetupWindow : EditorWindow
{
    private const string PluginRootPath = "Assets/plugins/MobileIntegrations";
    private const string RuntimeFolderPath = PluginRootPath + "/Runtime";
    private const string ConfigFolderPath = "Assets/MobileIntegrations/Resources";
    private const string ConfigAssetPath = ConfigFolderPath + "/MobileIntegrationConfig.asset";
    private const string LevelPlayPackage = "com.unity.services.levelplay";
    private const string UnityIapPackage = "com.unity.purchasing";
    private const string MobileNotificationsPackage = "com.unity.mobile.notifications";
    private const string GoogleMobileAdsPackage = "com.google.ads.mobile";
    private const string GooglePlayReviewPackage = "com.google.play.review";
    private const string GooglePlayAppUpdatePackage = "com.google.play.appupdate";
    private const string OpenUpmRegistryName = "OpenUPM";
    private const string OpenUpmRegistryUrl = "https://package.openupm.com";
    private const string OpenUpmGoogleScope = "com.google";

    private static readonly ProviderPackageInfo[] ProviderPackages =
    {
        new ProviderPackageInfo("Unity LevelPlay", LevelPlayPackage),
        new ProviderPackageInfo("Unity IAP", UnityIapPackage),
        new ProviderPackageInfo("Mobile Notifications", MobileNotificationsPackage),
        new ProviderPackageInfo("Google Mobile Ads / UMP", GoogleMobileAdsPackage),
        new ProviderPackageInfo("Google Play In-App Review", GooglePlayReviewPackage),
        new ProviderPackageInfo("Google Play In-App Update", GooglePlayAppUpdatePackage)
    };

    private Vector2 _scroll;
    private MobileIntegrationConfig _config;
    private SerializedObject _configObject;
    private ListRequest _listRequest;
    private AddRequest _addRequest;
    private readonly Dictionary<string, bool> _installedPackages = new Dictionary<string, bool>();
    private readonly Queue<string> _packageQueue = new Queue<string>();
    private string _pendingPackageName;
    private string _statusMessage = "Press Refresh to validate packages and project setup.";
    private MessageType _statusType = MessageType.Info;

    [MenuItem("Tools/Mobile Integrations/Setup Window")]
    public static void ShowWindow()
    {
        GetWindow<MobileIntegrationSetupWindow>("Mobile Integrations");
    }

    private void OnEnable()
    {
        LoadOrCreateConfig();
        RefreshValidation();
        EditorApplication.update += UpdateRequests;
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateRequests;
    }

    private void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        EditorGUILayout.LabelField("Mobile Integrations Setup", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(_statusMessage, _statusType);

        DrawValidationSection();
        EditorGUILayout.Space(8f);
        DrawProviderSection();
        EditorGUILayout.Space(8f);
        DrawPackageSection();
        EditorGUILayout.Space(8f);
        DrawProjectConfigurationSection();
        EditorGUILayout.Space(8f);
        DrawSceneSection();
        EditorGUILayout.Space(8f);
        DrawConfigSection();

        EditorGUILayout.EndScrollView();
    }

    private void DrawValidationSection()
    {
        EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope("box"))
        {
            DrawStatusRow("Plugin root", AssetDatabase.IsValidFolder(PluginRootPath));
            DrawStatusRow("Runtime folder", AssetDatabase.IsValidFolder(RuntimeFolderPath));
            DrawStatusRow("Plugin runtime type", TypeExists("MobileManagerRoot"));
            DrawStatusRow("LevelPlay package", IsPackageInstalled(LevelPlayPackage), true);
            DrawStatusRow("Google Mobile Ads / UMP", IsPackageInstalled(GoogleMobileAdsPackage), true);
            DrawStatusRow("Google Play In-App Review", IsPackageInstalled(GooglePlayReviewPackage), true);
            DrawStatusRow("Google Play In-App Update", IsPackageInstalled(GooglePlayAppUpdatePackage), true);
            DrawStatusRow("Config asset", _config != null);
            DrawStatusRow("Android LevelPlay app key", _config != null && HasAndroidLevelPlayAppKey());
            DrawStatusRow("iOS LevelPlay app key", _config != null && HasIosLevelPlayAppKey());
            DrawStatusRow("Android rewarded ad unit", _config != null && HasAndroidRewardedAdUnitId());
            DrawStatusRow("iOS rewarded ad unit", _config != null && HasIosRewardedAdUnitId());
            DrawStatusRow("Android interstitial ad unit", _config != null && HasAndroidInterstitialAdUnitId());
            DrawStatusRow("iOS interstitial ad unit", _config != null && HasIosInterstitialAdUnitId());
            DrawStatusRow("Android Google Mobile Ads App ID", _config != null && HasAndroidGoogleMobileAdsAppId());
            DrawStatusRow("iOS Google Mobile Ads App ID", _config != null && HasIosGoogleMobileAdsAppId());
            DrawStatusRow("MobileManager in scene", GameObject.Find("MobileManager") != null);
            DrawStatusRow("Android bundle id", !string.IsNullOrWhiteSpace(GetBundleId(NamedBuildTarget.Android)));
            DrawStatusRow("iOS bundle id", !string.IsNullOrWhiteSpace(GetBundleId(NamedBuildTarget.iOS)));
            DrawStatusRow("Android IL2CPP + ARM64", IsAndroidConfigured());
            DrawStatusRow("Portrait orientation", IsPortraitConfigured());
        }

        if (GUILayout.Button("Refresh Validation"))
        {
            RefreshValidation();
        }
    }

    private void DrawProviderSection()
    {
        EditorGUILayout.LabelField("Provider Validation", EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope("box"))
        {
            foreach (ProviderPackageInfo providerPackage in ProviderPackages)
            {
                DrawStatusRow(providerPackage.displayName, IsPackageInstalled(providerPackage.packageName), true);
            }

            if (_config != null)
            {
                EditorGUILayout.Space(4f);
                EditorGUI.BeginChangeCheck();
                _config.customAdsPackage = EditorGUILayout.TextField("Custom Ads Package", _config.customAdsPackage);
                SaveConfigIfChanged(EditorGUI.EndChangeCheck());

                using (new EditorGUI.DisabledScope(IsBusy() || string.IsNullOrWhiteSpace(_config.customAdsPackage)))
                {
                    if (GUILayout.Button("Install Custom Ads Package"))
                    {
                        InstallPackage(_config.customAdsPackage.Trim());
                    }
                }
            }
        }
    }

    private void DrawPackageSection()
    {
        EditorGUILayout.LabelField("Package Installation", EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope("box"))
        {
            using (new EditorGUI.DisabledScope(IsBusy()))
            {
                if (GUILayout.Button("Install Required Unity Packages"))
                {
                    InstallRequiredPackages();
                }

                if (GUILayout.Button("Install Unity LevelPlay"))
                {
                    InstallPackage(LevelPlayPackage);
                }

                if (GUILayout.Button("Install Unity IAP"))
                {
                    InstallPackage(UnityIapPackage);
                }

                if (GUILayout.Button("Install Mobile Notifications"))
                {
                    InstallPackage(MobileNotificationsPackage);
                }

                if (GUILayout.Button("Install Google UMP"))
                {
                    InstallGoogleUmpPackage();
                }
            }
        }
    }

    private void DrawProjectConfigurationSection()
    {
        EditorGUILayout.LabelField("Project Configuration", EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.HelpBox(
                "These values are applied through PlayerSettings so the project is ready for Android/iOS integrations without manual setup in multiple menus.",
                MessageType.None);

            if (_config != null)
            {
                EditorGUI.BeginChangeCheck();
                _config.companyName = EditorGUILayout.TextField("Company Name", _config.companyName);
                _config.productName = EditorGUILayout.TextField("Product Name", _config.productName);
                _config.bundleVersion = EditorGUILayout.TextField("Bundle Version", _config.bundleVersion);
                _config.androidBundleId = EditorGUILayout.TextField("Android Bundle Id", _config.androidBundleId);
                _config.iosBundleId = EditorGUILayout.TextField("iOS Bundle Id", _config.iosBundleId);
                _config.androidVersionCode = Mathf.Max(1, EditorGUILayout.IntField("Android Version Code", _config.androidVersionCode));
                _config.iosBuildNumber = EditorGUILayout.TextField("iOS Build Number", _config.iosBuildNumber);
                _config.forcePortrait = EditorGUILayout.Toggle("Force Portrait", _config.forcePortrait);
                SaveConfigIfChanged(EditorGUI.EndChangeCheck());
            }

            using (new EditorGUI.DisabledScope(_config == null))
            {
                if (GUILayout.Button("Apply Recommended Player Settings"))
                {
                    ApplyPlayerSettings();
                }
            }
        }
    }

    private void DrawSceneSection()
    {
        EditorGUILayout.LabelField("Scene Setup", EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.HelpBox(
                "Creates the MobileManager object in the active scene and assigns the shared config asset to the plugin runtime components.",
                MessageType.None);

            if (GUILayout.Button("Create or Update MobileManager"))
            {
                CreateOrUpdateMobileManager();
            }
        }
    }

    private void DrawConfigSection()
    {
        EditorGUILayout.LabelField("Runtime Config", EditorStyles.boldLabel);

        if (_config == null)
        {
            if (GUILayout.Button("Create Config Asset"))
            {
                LoadOrCreateConfig(true);
            }

            return;
        }

        if (_configObject == null)
        {
            _configObject = new SerializedObject(_config);
        }

        _configObject.Update();

        using (new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("LevelPlay", EditorStyles.boldLabel);
            DrawProperty("androidAppKey");
            DrawProperty("iosAppKey");
            DrawProperty("levelPlayUserId");
            DrawProperty("enableLevelPlayTestSuite");
            DrawProperty("androidRewardedAdUnitId");
            DrawProperty("iosRewardedAdUnitId");
            DrawProperty("androidInterstitialAdUnitId");
            DrawProperty("iosInterstitialAdUnitId");
            DrawProperty("rewardedPlacement");
            DrawProperty("interstitialPlacement");

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Google Mobile Ads", EditorStyles.boldLabel);
            DrawProperty("googleMobileAdsAndroidAppId");
            DrawProperty("googleMobileAdsIosAppId");

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Google Play", EditorStyles.boldLabel);
            DrawProperty("preferImmediateAppUpdate");
            DrawProperty("allowAssetPackDeletionForAppUpdate");

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("IAP", EditorStyles.boldLabel);
            DrawProperty("consumableIds", true);
            DrawProperty("nonConsumableIds", true);

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Privacy", EditorStyles.boldLabel);
            DrawProperty("gdprRequired");
            DrawProperty("requestAttOnStartup");
            DrawProperty("allowAdsWhenUmpUpdateFails");
            DrawProperty("forceGdprDebugEeaInTestMode");
            DrawProperty("umpTestDeviceHashedIds", true);
            DrawProperty("iosTrackingUsageDescription");

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField("Notifications", EditorStyles.boldLabel);
            DrawProperty("notificationTitle");
            DrawProperty("notificationMessage");
            DrawProperty("notificationDelaySeconds");
            DrawProperty("androidNotificationChannelId");
            DrawProperty("androidNotificationChannelName");
        }

        if (_configObject.ApplyModifiedProperties())
        {
            EditorUtility.SetDirty(_config);
            SyncGoogleMobileAdsSettings();
            AssetDatabase.SaveAssets();
        }
    }

    private void DrawProperty(string propertyName, bool includeChildren = false)
    {
        SerializedProperty property = _configObject.FindProperty(propertyName);
        if (property != null)
        {
            EditorGUILayout.PropertyField(property, includeChildren);
        }
    }

    private void DrawStatusRow(string label, bool value, bool dependsOnRefresh = false)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(190f));
        string statusText = dependsOnRefresh && _installedPackages.Count == 0
            ? "Unknown"
            : value ? "Installed" : "Missing";
        EditorGUILayout.LabelField(statusText, GUILayout.Width(70f));
        EditorGUILayout.EndHorizontal();
    }

    private void RefreshValidation()
    {
        LoadOrCreateConfig();
        _installedPackages.Clear();
        _listRequest = Client.List(true, false);
        SetStatus("Refreshing package list and project validation...", MessageType.Info);
    }

    private void InstallGoogleUmpPackage()
    {
        if (EnsureOpenUpmScopedRegistry())
        {
            AssetDatabase.Refresh();
            InstallPackage(GoogleMobileAdsPackage);
        }
    }

    private bool EnsureOpenUpmScopedRegistry()
    {
        string manifestPath = Path.Combine(Directory.GetCurrentDirectory(), "Packages", "manifest.json");
        if (!File.Exists(manifestPath))
        {
            SetStatus("Packages/manifest.json was not found.", MessageType.Error);
            return false;
        }

        string manifestContent = File.ReadAllText(manifestPath);
        bool hasRegistryUrl = manifestContent.IndexOf(OpenUpmRegistryUrl, StringComparison.OrdinalIgnoreCase) >= 0;
        bool hasGoogleScope = manifestContent.IndexOf('"' + OpenUpmGoogleScope + '"', StringComparison.OrdinalIgnoreCase) >= 0;
        if (hasRegistryUrl && hasGoogleScope)
        {
            return true;
        }

        string registryBlock =
            "  \"scopedRegistries\": [\n" +
            "    {\n" +
            "      \"name\": \"" + OpenUpmRegistryName + "\",\n" +
            "      \"url\": \"" + OpenUpmRegistryUrl + "\",\n" +
            "      \"scopes\": [\n" +
            "        \"" + OpenUpmGoogleScope + "\"\n" +
            "      ]\n" +
            "    }\n" +
            "  ],\n";

        if (manifestContent.IndexOf("\"scopedRegistries\"", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            SetStatus("Add OpenUPM scoped registry manually in Packages/manifest.json, then press Install Google UMP again.", MessageType.Warning);
            return false;
        }

        int dependenciesIndex = manifestContent.IndexOf("\"dependencies\"", StringComparison.OrdinalIgnoreCase);
        if (dependenciesIndex < 0)
        {
            SetStatus("The manifest does not contain a dependencies section.", MessageType.Error);
            return false;
        }

        manifestContent = manifestContent.Insert(dependenciesIndex, registryBlock);
        File.WriteAllText(manifestPath, manifestContent);
        SetStatus("OpenUPM scoped registry added. Installing Google Mobile Ads / UMP...", MessageType.Info);
        return true;
    }

    private void InstallRequiredPackages()
    {
        foreach (ProviderPackageInfo providerPackage in ProviderPackages)
        {
            if (providerPackage.packageName == GoogleMobileAdsPackage)
            {
                continue;
            }

            if (!IsPackageInstalled(providerPackage.packageName) && !_packageQueue.Contains(providerPackage.packageName))
            {
                _packageQueue.Enqueue(providerPackage.packageName);
            }
        }

        if (_packageQueue.Count == 0)
        {
            SetStatus("Required Unity packages are already installed.", MessageType.Info);
            return;
        }

        if (!IsBusy())
        {
            InstallPackage(_packageQueue.Dequeue());
        }
    }

    private void InstallPackage(string packageName)
    {
        if (IsBusy())
        {
            SetStatus("Another package operation is in progress. Wait for it to finish first.", MessageType.Warning);
            return;
        }

        _pendingPackageName = packageName;
        _addRequest = Client.Add(packageName);
        SetStatus("Installing package: " + packageName, MessageType.Info);
    }

    private void UpdateRequests()
    {
        if (_listRequest != null && _listRequest.IsCompleted)
        {
            if (_listRequest.Status == StatusCode.Success)
            {
                _installedPackages.Clear();
                foreach (var package in _listRequest.Result)
                {
                    _installedPackages[package.name] = true;
                }

                SyncGoogleMobileAdsSettings();
                SetStatus("Validation refreshed.", MessageType.Info);
            }
            else
            {
                string errorMessage = _listRequest.Error != null
                    ? _listRequest.Error.message
                    : "Unknown package list error.";
                SetStatus("Package list refresh failed: " + errorMessage, MessageType.Error);
            }

            _listRequest = null;
            Repaint();
        }

        if (_addRequest != null && _addRequest.IsCompleted)
        {
            if (_addRequest.Status == StatusCode.Success)
            {
                SetStatus("Package installed: " + _pendingPackageName, MessageType.Info);
                _addRequest = null;
                _pendingPackageName = null;

                if (_packageQueue.Count > 0)
                {
                    InstallPackage(_packageQueue.Dequeue());
                }
                else
                {
                    RefreshValidation();
                }
            }
            else
            {
                string errorMessage = _addRequest.Error != null
                    ? _addRequest.Error.message
                    : "Unknown package installation error.";
                SetStatus("Package installation failed: " + errorMessage, MessageType.Error);
                _addRequest = null;
                _pendingPackageName = null;
                _packageQueue.Clear();
            }

            Repaint();
        }
    }

    private void ApplyPlayerSettings()
    {
        if (_config == null)
        {
            SetStatus("Config asset is missing.", MessageType.Error);
            return;
        }

        if (string.IsNullOrWhiteSpace(_config.companyName) ||
            string.IsNullOrWhiteSpace(_config.productName) ||
            string.IsNullOrWhiteSpace(_config.androidBundleId) ||
            string.IsNullOrWhiteSpace(_config.iosBundleId))
        {
            SetStatus("Company, product name and bundle identifiers are required before applying settings.", MessageType.Warning);
            return;
        }

        PlayerSettings.companyName = _config.companyName.Trim();
        PlayerSettings.productName = _config.productName.Trim();
        PlayerSettings.bundleVersion = string.IsNullOrWhiteSpace(_config.bundleVersion) ? "1.0.0" : _config.bundleVersion.Trim();
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, _config.androidBundleId.Trim());
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, _config.iosBundleId.Trim());
        PlayerSettings.Android.bundleVersionCode = Mathf.Max(1, _config.androidVersionCode);
        PlayerSettings.iOS.buildNumber = string.IsNullOrWhiteSpace(_config.iosBuildNumber) ? "1" : _config.iosBuildNumber.Trim();
        PlayerSettings.defaultInterfaceOrientation = _config.forcePortrait ? UIOrientation.Portrait : UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = !_config.forcePortrait;
        PlayerSettings.allowedAutorotateToLandscapeRight = !_config.forcePortrait;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);

        EditorUtility.SetDirty(_config);
        SyncGoogleMobileAdsSettings();
        AssetDatabase.SaveAssets();

        SetStatus("Recommended PlayerSettings applied successfully.", MessageType.Info);
    }

    private void CreateOrUpdateMobileManager()
    {
        LoadOrCreateConfig(true);

        GameObject manager = GameObject.Find("MobileManager");
        if (manager == null)
        {
            manager = new GameObject("MobileManager");
            Undo.RegisterCreatedObjectUndo(manager, "Create MobileManager");
        }

        MobileManagerRoot root = AddComponentIfMissing<MobileManagerRoot>(manager);
        AdsManager ads = AddComponentIfMissing<AdsManager>(manager);
        IAPManager iap = AddComponentIfMissing<IAPManager>(manager);
        NotificationManager notifications = AddComponentIfMissing<NotificationManager>(manager);
        PrivacyManager privacy = AddComponentIfMissing<PrivacyManager>(manager);
        InAppReviewManager inAppReview = AddComponentIfMissing<InAppReviewManager>(manager);
        InAppUpdateManager inAppUpdate = AddComponentIfMissing<InAppUpdateManager>(manager);
        InternetChecker checker = AddComponentIfMissing<InternetChecker>(manager);

        root.config = _config;
        ads.config = _config;
        iap.config = _config;
        notifications.config = _config;
        privacy.config = _config;
        inAppReview.config = _config;
        inAppUpdate.config = _config;
        checker.logWhenOffline = true;

        EditorUtility.SetDirty(manager);
        if (manager.scene.IsValid())
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(manager.scene);
        }

        SetStatus("MobileManager is ready in the current scene.", MessageType.Info);
    }

    private void LoadOrCreateConfig(bool createIfMissing = false)
    {
        _config = AssetDatabase.LoadAssetAtPath<MobileIntegrationConfig>(ConfigAssetPath);

        if (_config == null && createIfMissing)
        {
            EnsureFolder("Assets/MobileIntegrations");
            EnsureFolder(ConfigFolderPath);

            _config = CreateInstance<MobileIntegrationConfig>();
            AssetDatabase.CreateAsset(_config, ConfigAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        _configObject = _config != null ? new SerializedObject(_config) : null;
    }

    private bool IsPackageInstalled(string packageName)
    {
        if (_installedPackages.TryGetValue(packageName, out bool installed))
        {
            return installed;
        }

        string manifestPath = Path.Combine(Directory.GetCurrentDirectory(), "Packages", "manifest.json");
        if (!File.Exists(manifestPath))
        {
            return false;
        }

        string manifestContent = File.ReadAllText(manifestPath);
        return manifestContent.IndexOf(packageName, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool HasAndroidLevelPlayAppKey()
    {
        return !string.IsNullOrWhiteSpace(_config.androidAppKey);
    }

    private bool HasIosLevelPlayAppKey()
    {
        return !string.IsNullOrWhiteSpace(_config.iosAppKey);
    }

    private bool HasAndroidRewardedAdUnitId()
    {
        return !string.IsNullOrWhiteSpace(_config.androidRewardedAdUnitId);
    }

    private bool HasIosRewardedAdUnitId()
    {
        return !string.IsNullOrWhiteSpace(_config.iosRewardedAdUnitId);
    }

    private bool HasAndroidInterstitialAdUnitId()
    {
        return !string.IsNullOrWhiteSpace(_config.androidInterstitialAdUnitId);
    }

    private bool HasIosInterstitialAdUnitId()
    {
        return !string.IsNullOrWhiteSpace(_config.iosInterstitialAdUnitId);
    }

    private bool HasAndroidGoogleMobileAdsAppId()
    {
        return !string.IsNullOrWhiteSpace(_config.googleMobileAdsAndroidAppId);
    }

    private bool HasIosGoogleMobileAdsAppId()
    {
        return !string.IsNullOrWhiteSpace(_config.googleMobileAdsIosAppId);
    }

    private void SyncGoogleMobileAdsSettings()
    {
        if (_config == null || !IsPackageInstalled(GoogleMobileAdsPackage))
        {
            return;
        }

        Type settingsType = FindType("GoogleMobileAds.Editor.GoogleMobileAdsSettings");
        if (settingsType == null)
        {
            return;
        }

        MethodInfo loadInstanceMethod = settingsType.GetMethod("LoadInstance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (loadInstanceMethod == null)
        {
            return;
        }

        UnityEngine.Object settingsInstance = loadInstanceMethod.Invoke(null, null) as UnityEngine.Object;
        if (settingsInstance == null)
        {
            return;
        }

        SerializedObject serializedSettings = new SerializedObject(settingsInstance);
        bool changed = false;
        changed |= SetStringIfDifferent(serializedSettings.FindProperty("adMobAndroidAppId"), _config.googleMobileAdsAndroidAppId);
        changed |= SetStringIfDifferent(serializedSettings.FindProperty("adMobIOSAppId"), _config.googleMobileAdsIosAppId);
        changed |= SetStringIfDifferent(serializedSettings.FindProperty("userTrackingUsageDescription"), _config.iosTrackingUsageDescription);

        if (!changed)
        {
            return;
        }

        serializedSettings.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(settingsInstance);
        AssetDatabase.SaveAssets();
    }

    private static bool SetStringIfDifferent(SerializedProperty property, string value)
    {
        if (property == null)
        {
            return false;
        }

        string safeValue = value ?? string.Empty;
        if (property.stringValue == safeValue)
        {
            return false;
        }

        property.stringValue = safeValue;
        return true;
    }

    private static string GetBundleId(NamedBuildTarget targetGroup)
    {
        return PlayerSettings.GetApplicationIdentifier(targetGroup);
    }

    private static bool IsAndroidConfigured()
    {
        bool il2cpp = PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) == ScriptingImplementation.IL2CPP;
        bool arm64 = PlayerSettings.Android.targetArchitectures == AndroidArchitecture.ARM64;
        return il2cpp && arm64;
    }

    private static bool IsPortraitConfigured()
    {
        return PlayerSettings.defaultInterfaceOrientation == UIOrientation.Portrait ||
               (PlayerSettings.allowedAutorotateToPortrait &&
                !PlayerSettings.allowedAutorotateToLandscapeLeft &&
                !PlayerSettings.allowedAutorotateToLandscapeRight);
    }

    private static bool TypeExists(string typeName)
    {
        return FindType(typeName) != null;
    }

    private static Type FindType(string typeName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        return null;
    }

    private bool IsBusy()
    {
        return _listRequest != null || _addRequest != null;
    }

    private void SetStatus(string message, MessageType type)
    {
        _statusMessage = message;
        _statusType = type;
    }

    private void SaveConfigIfChanged(bool changed)
    {
        if (!changed || _config == null)
        {
            return;
        }

        EditorUtility.SetDirty(_config);
        SyncGoogleMobileAdsSettings();
        AssetDatabase.SaveAssets();
    }

    private static T AddComponentIfMissing<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = Undo.AddComponent<T>(go);
        }

        return component;
    }

    private static void EnsureFolder(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string normalizedPath = folderPath.Replace("\\", "/");
        string parentFolder = Path.GetDirectoryName(normalizedPath).Replace("\\", "/");
        string folderName = Path.GetFileName(normalizedPath);

        if (!string.IsNullOrEmpty(parentFolder) && !AssetDatabase.IsValidFolder(parentFolder))
        {
            EnsureFolder(parentFolder);
        }

        AssetDatabase.CreateFolder(parentFolder, folderName);
    }

    private readonly struct ProviderPackageInfo
    {
        public readonly string displayName;
        public readonly string packageName;

        public ProviderPackageInfo(string displayName, string packageName)
        {
            this.displayName = displayName;
            this.packageName = packageName;
        }
    }
}





