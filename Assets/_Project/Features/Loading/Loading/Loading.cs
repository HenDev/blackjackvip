using UnityEngine;
using System.Collections;
using System;

public class Loading : MonoBehaviour {
	private MobileManagerRoot mMobileManagerRoot = null;
	
	public static bool IsAdsRemoved 
	{
		get { return PlayerPrefs.GetInt("IsAdsRemoved")==1;}
		set { PlayerPrefs.SetInt("IsAdsRemoved", value?1:0); }
	}
	
	// Use this for initialization
	void Start () { firstTimeInitialize(); StartCoroutine(Initialize()); }
	
	IEnumerator Initialize()
	{
		yield return new WaitForFixedUpdate();

		mMobileManagerRoot = FindObjectOfType<MobileManagerRoot>();

		if(ShouldWaitForPrivacyConsent())
		{
			yield return StartCoroutine(RequestNativePrivacyConsent());
		}
		else
		{
			InitializeMobileLibraries();
		}
		
		Application.LoadLevel(1);
	}
	
	bool ShouldWaitForPrivacyConsent()
	{
		if(mMobileManagerRoot == null || mMobileManagerRoot.config == null)
			return false;

		return !MobilePrivacyConsentState.CanInitializeSdk(mMobileManagerRoot.config);
	}

	IEnumerator RequestNativePrivacyConsent()
	{
		PrivacyManager privacyManager = FindObjectOfType<PrivacyManager>();
		if(privacyManager == null)
		{
			MobilePrivacyConsentState.ClearConsent();
			Debug.LogWarning("Loading: PrivacyManager was not found in the startup scene. Consent will be requested again on the next launch.");
			yield break;
		}

		if(privacyManager.config == null && mMobileManagerRoot != null)
			privacyManager.config = mMobileManagerRoot.config;

		bool completed = false;
		privacyManager.RequestStartupPrivacyConsent(() => completed = true);
		while(!completed)
			yield return null;

		InitializeMobileLibraries();
	}

	void InitializeMobileLibraries()
	{
		if(mMobileManagerRoot == null || mMobileManagerRoot.config == null)
			return;

		PrivacyManager privacyManager = FindObjectOfType<PrivacyManager>();
		if(privacyManager != null)
		{
			if(privacyManager.config == null)
				privacyManager.config = mMobileManagerRoot.config;

			privacyManager.InitializePrivacy();
		}

		AdsManager adsManager = FindObjectOfType<AdsManager>();
		if(adsManager != null)
		{
			if(adsManager.config == null)
				adsManager.config = mMobileManagerRoot.config;

			adsManager.InitializeAds();
		}

		IAPManager iapManager = FindObjectOfType<IAPManager>();
		if(iapManager != null)
		{
			if(iapManager.config == null)
				iapManager.config = mMobileManagerRoot.config;

			iapManager.InitializePurchases();
		}

		NotificationManager notificationManager = FindObjectOfType<NotificationManager>();
		if(notificationManager != null)
		{
			if(notificationManager.config == null)
				notificationManager.config = mMobileManagerRoot.config;

			notificationManager.InitializeNotifications();
		}
	}
	
	void firstTimeInitialize()
	{
		// First Time Initialization
		if(PlayerPrefs.GetInt("FirstTimeLoad") == 0)
		{
			PlayerPrefs.SetInt("FirstTimeLoad",1);
			PlayerPrefs.SetInt("IsSoundEnable", 1);
			PlayerPrefs.SetInt("IsMusicEnable", 1);
			
			PlayerPrefs.SetInt("PlayerChipBalance", 2000);			
			ApplicationTimer.resetSavedTimerBefore(0, 0, 5);
			
			MoreChipsHandler.RateReviewDone = false;
			IsAdsRemoved = false;					
		}
		
		// Update
		if(PlayerPrefs.GetInt("FirstTimeLoadUpdate") == 0)
		{
			PlayerPrefs.SetInt("FirstTimeLoadUpdate",1);
			PlayerPrefs.SetInt("LocalNotificationEnabled", 1);
			
			// Daily Bonus
			int currentDay = System.DateTime.Now.Day;
			int currentMonth = System.DateTime.Now.Month;
			int currentYear = System.DateTime.Now.Year;			
			PlayerPrefs.SetInt("DailyBonusDay", currentDay);
			PlayerPrefs.SetInt("DailyBonusMonth", currentMonth);
			PlayerPrefs.SetInt("DailyBonusYear", currentYear);
			PlayerPrefs.SetInt("NoOfContiniousDays", 0);
		}
	}
}
