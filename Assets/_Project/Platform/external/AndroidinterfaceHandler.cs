using UnityEngine;
using System;
using System.Collections;

public class AndroidinterfaceHandler : ExternalInterfaceHandler 
{
	#if UNITY_ANDROID
	//public static AndroidJavaClass javaClass = new AndroidJavaClass("com.dumadugames.unityandroid.JarClass");
	
	/*public static AndroidJavaObject getCurrentActivity () {
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	}*/
	
	
	public static void _SendInAppRequest(bool isConsumable , int itemId) {
		/*
		javaClass.CallStatic("purchaseItem", isConsumable, itemId, getCurrentActivity());
	*/
	}
	
	public static void _FullScreenAds() {
		AdsManager adsManager = UnityEngine.Object.FindFirstObjectByType<AdsManager>();
		if (adsManager == null)
		{
			GameObject mobileManager = GameObject.Find("MobileManager");
			if (mobileManager != null)
			{
				adsManager = mobileManager.GetComponent<AdsManager>();
			}
		}

		if (adsManager == null)
		{
			Debug.LogWarning("AndroidinterfaceHandler: AdsManager not found. Interstitial request ignored.");
			return;
		}

		adsManager.ShowInterstitial();
	}
	
	public static void _BannerAds(bool isVisible,bool isOnTop) {
		/*if(isVisible) {
			javaClass.CallStatic("showBanner", isOnTop, getCurrentActivity());
		}
		else {
			javaClass.CallStatic("hideBanner", isOnTop, getCurrentActivity());
		}*/
	}
	
	public static void _SendScore(int id, int score) {
		/*
		javaClass.CallStatic("submitScore", id, score, getCurrentActivity());
	*/
	}
	
	public static void _SendScore(int id, float score) {
		/*
		javaClass.CallStatic("submitScore", id, score, getCurrentActivity());
	*/
	}
	
	public static void _SendAchievement(string id) {
		/*
		javaClass.CallStatic("unlockAchievement", id , getCurrentActivity());
	*/
	}
	
	
	
	public static void _SendAchievement(string id, int status) {
		/*
		javaClass.CallStatic("incrementAchievement", id, status, getCurrentActivity());
	*/
	}
	
	public static void _ShowScores() {
		/*
		javaClass.CallStatic("showLeaderBoards", getCurrentActivity());
	*/
	}
	
	public static void _ShowAchievements() {
		/*
		javaClass.CallStatic("showAchievements", getCurrentActivity());
	*/
	}
	
	public static void _ShowPopUp(string title, string msg , bool isQuitRequested) {/*
		if(isQuitRequested)
			javaClass.CallStatic("showExitPopUp", getCurrentActivity());
		else
			javaClass.CallStatic("showPopUp", title, msg, getCurrentActivity());*/
	}
	
	public static void _RateApp() {
		/*
		javaClass.CallStatic("rateApp", getCurrentActivity());
	*/
	}
	
	public static void _SubmitFlurryEvent(string key) {
		/*
		javaClass.CallStatic("submitFlurryEvent", key, getCurrentActivity());
	*/
	}	
	
	public static void _FaceBook() {
		/*
		javaClass.CallStatic("like", getCurrentActivity());
	*/
	}	
	
	public static void _Twitter() {
		/*
		javaClass.CallStatic("follow", getCurrentActivity());
	*/
	}	
	
	public static void _MoreGames() {
		/*
		javaClass.CallStatic("moreGames", getCurrentActivity());
	*/
	}
	
	public static void _LocalNotification(string date, string msg, int classId) {
		/*
		javaClass.CallStatic("localNotification", date, msg, classId, getCurrentActivity());
	*/
	}
	
	public static void _CancelAllLocalNotifications()
	{
		/*
		javaClass.CallStatic("cancellocalNotification", true, getCurrentActivity());
	*/
	}
	
	#endif
	void Awake()
	{
		Initialize();
	}

	public override bool SendRequest(eEXTERNAL_REQ_TYPE eRequestType, string strData, ReceivedCallBack callback)
	{
		
	
		if(callback != null)
		{
			mePrevRequestedType = eRequestType;
			mPrevRequestedData = strData;
			OnCallBack = callback;
		}
#if UNITY_ANDROID	
		int result = 10;
		switch(eRequestType)
		{
			case eEXTERNAL_REQ_TYPE.InAppConsumable:
			{
				_SendInAppRequest(true,int.Parse(strData));
			}
			break;
			case eEXTERNAL_REQ_TYPE.InAppNonConsumable:
			{
				_SendInAppRequest(false,int.Parse(strData));
			}
			break;
			case eEXTERNAL_REQ_TYPE.Show_Banner_Top_Ads:
			{
				if(mAdsHander.IsAdsDisplayAllowed(eRequestType))
					_BannerAds(true,true);
			}
			break;
			case eEXTERNAL_REQ_TYPE.Show_Banner_Bottom_Ads:
			{
				if(mAdsHander.IsAdsDisplayAllowed(eRequestType))
					_BannerAds(true,false);
			}
			break;
			case eEXTERNAL_REQ_TYPE.Show_FullScreen_Ads:
			{
//			Debug.Log("FullScreen Ads Check");
				if(mAdsHander.IsAdsDisplayAllowed(eRequestType))
//			{
//			Debug.Log("FullScreen Ads Check True");
					_FullScreenAds();	
//			}
//			Debug.Log("FullScreen Ads Check Done");
			}
			break;
			case eEXTERNAL_REQ_TYPE.Hide_Banner_Ads:
			{
				if(mAdsHander.IsAdsDisplayAllowed(eRequestType))
					_BannerAds(false,true);
			}
			break;
			case eEXTERNAL_REQ_TYPE.Hide_FullScreen_Ads:
			{
				if(mAdsHander.IsAdsDisplayAllowed(eRequestType))
					_FullScreenAds();	
			}
			break;
			case eEXTERNAL_REQ_TYPE.Send_Score:
			{
				string[] array = strData.Split('_');
				if(array.Length == 2)
				{
					if(int.TryParse(array[1],out result))
						_SendScore(int.Parse(array[0]),int.Parse(array[1]));
					else
						_SendScore(int.Parse(array[0]),float.Parse(array[1]));
				}
				else
				{
				if(int.TryParse(strData,out result))	
					_SendScore(1,int.Parse(strData));
				else
					_SendScore(1,float.Parse(strData));
				}
			}
			break;
			case eEXTERNAL_REQ_TYPE.Send_Achievement:
			{
//				string[] array = strData.Split('_');
				
			_SendAchievement(strData);
			
//				if(array.Length == 2)
//					_SendAchievement(int.Parse(strData).ToString(),int.Parse(array[1])); // Dhivakar
//				else
//					_SendAchievement(int.Parse(array[0]));
			}
			break;
			
			case eEXTERNAL_REQ_TYPE.Send_Flurry:
			{
				_SubmitFlurryEvent(strData);
			}
			break;
			
			case eEXTERNAL_REQ_TYPE.Show_Score:
			{
				_ShowScores();
			}
			break;
			case eEXTERNAL_REQ_TYPE.Show_Achievement:
			{
				_ShowAchievements();
			}
			break;
			case eEXTERNAL_REQ_TYPE.ShowPopup: 
			{
				string[] array = strData.Split('_');
				
				if(array.Length == 2)
					_ShowPopUp(array[0],array[1],true);
				else
					_ShowPopUp(array[0],"Content is missing",true);
			}
			break;
			case eEXTERNAL_REQ_TYPE.Facebook://15
			{
				_FaceBook();
			}
			break;
			
			case eEXTERNAL_REQ_TYPE.Twitter://16
			{
				_Twitter();
			}
			break;
			
			case eEXTERNAL_REQ_TYPE.MoreGames://17
			{
				_MoreGames();
			}
			break;
			case eEXTERNAL_REQ_TYPE.ApplicationQuit://18
			{
				string[] array = strData.Split('_');
				
				if(array.Length == 2)
					_ShowPopUp(array[0],array[1],true);
				else
					_ShowPopUp(array[0],"Content is missing",true);
			}
			break;
			case eEXTERNAL_REQ_TYPE.RateApplication://18
			{
				_RateApp();
			}
			break;
			case eEXTERNAL_REQ_TYPE.SendAndroidLocalNotification:
				{
				LocalNotificationHandler.debugLog += "\nSending Local Notification: "+strData;
				Debug.Log("Sending Local Notification: "+strData);
			
				int indexToSeperate = strData.LastIndexOf('_');
				string classId_ = strData.Substring(indexToSeperate+1);
				strData = strData.Substring(0, indexToSeperate);
				indexToSeperate = strData.LastIndexOf('_');
				string date = strData.Substring(0, indexToSeperate);
				string msg = strData.Substring(indexToSeperate+1);
				int classId = 0;
				int.TryParse(classId_, out classId);
			
				Debug.Log("Local Notification : "+date+" "+msg+" "+classId);
				LocalNotificationHandler.debugLog += "\nLocal Notification : "+date+" "+msg+" "+classId;
				_LocalNotification(date, msg, classId);
			}
			break;
			case eEXTERNAL_REQ_TYPE.CancelAndroidLocalNotification:
			_CancelAllLocalNotifications();
			break;
			default:
			{
			}
			break;
		}
#endif
		
		return false;
	}
}
