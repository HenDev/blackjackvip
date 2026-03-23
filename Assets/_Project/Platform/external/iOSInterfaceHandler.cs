using UnityEngine;
using System;
using System.Collections;

using System.Runtime.InteropServices;

public class iOSInterfaceHandler : ExternalInterfaceHandler 
{
#if UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void _SendInAppRequest(bool isConsumable , int requestedID);
	
	[DllImport ("__Internal")]
	private static extern void _FullScreenAds();
	
	[DllImport ("__Internal")]
	private static extern void _BannerAds(bool isVisible,bool isOnTop);

	[DllImport ("__Internal")]
	private static extern void _SendScore(int id , float score);
	
	[DllImport ("__Internal")]
	private static extern void _SendAchievement(int id,float percentage);
	
	[DllImport ("__Internal")]
	private static extern void _SendFlurry(string key);
	
	[DllImport ("__Internal")]
	private static extern void _ShowScore();
	
	[DllImport ("__Internal")]
	private static extern void _ShowAchievement();
	
	[DllImport ("__Internal")]
	private static extern void _RateApplication();
	
	[DllImport ("__Internal")]
	private static extern void _ShowPopUp(string Heading, string msg);
	
	[DllImport ("__Internal")]
	private static extern void _FaceBook();
	
	[DllImport ("__Internal")]
	private static extern void _Twitter();
	
	[DllImport ("__Internal")]
	private static extern void _MoreGames();
	
	
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
		
#if UNITY_IPHONE
		switch(eRequestType)
		{
			case eEXTERNAL_REQ_TYPE.InAppConsumable://1
			{
				_SendInAppRequest(true,int.Parse(strData));
			}
			break;
			case eEXTERNAL_REQ_TYPE.InAppNonConsumable://2
			{
				_SendInAppRequest(false,int.Parse(strData));
			}
			break;
			case eEXTERNAL_REQ_TYPE.Show_Banner_Top_Ads://3
			{
				if(mAdsHander.IsAdsDisplayAllowed(eRequestType))
					_BannerAds(true,true);
			}
			break;
			case eEXTERNAL_REQ_TYPE.Show_Banner_Bottom_Ads://4
			{
				if(mAdsHander.IsAdsDisplayAllowed(eRequestType))
					_BannerAds(true,false);
			}
			break;
			case eEXTERNAL_REQ_TYPE.Show_FullScreen_Ads://5
			{
				if(mAdsHander.IsAdsDisplayAllowed(eRequestType))
					_FullScreenAds();	
			}
			break;
			case eEXTERNAL_REQ_TYPE.Hide_Banner_Ads://6
			{
				if(mAdsHander.IsAdsDisplayAllowed(eRequestType))
					_BannerAds(false,true);
			}
			break;
			case eEXTERNAL_REQ_TYPE.Hide_FullScreen_Ads://7
			{
				if(mAdsHander.IsAdsDisplayAllowed(eRequestType))
					_FullScreenAds();	
			}
			break;
			case eEXTERNAL_REQ_TYPE.Send_Score://8
			{
				string[] array = strData.Split('_');
				
				if(array.Length == 2)
				{
					_SendScore(int.Parse(array[0]),float.Parse(array[1]));
				}
				else
				{
					_SendScore(1,float.Parse(strData));
				}
			}
			break;
			case eEXTERNAL_REQ_TYPE.Send_Achievement://9
			{
				string[] array = strData.Split('_');
				
				if(array.Length == 2)
					_SendAchievement(int.Parse(array[0]),int.Parse(array[1]));
				else
					_SendAchievement(int.Parse(strData),100);
			}
			break;
			case eEXTERNAL_REQ_TYPE.Send_Flurry://10
			{
				_SendFlurry(strData);
			}
			break;
			case eEXTERNAL_REQ_TYPE.Show_Score://11
			{
				_ShowScore();
			}
			break;
			case eEXTERNAL_REQ_TYPE.Show_Achievement://12
			{
				_ShowAchievement();
			}
			break;
			case eEXTERNAL_REQ_TYPE.RateApplication://13
			{
				_RateApplication();
			}	
			break;
			case eEXTERNAL_REQ_TYPE.ShowPopup://14
			{
				string[] array = strData.Split('_');
				
				if(array.Length == 2)
					_ShowPopUp(array[0],array[1]);
				else
					_ShowPopUp(array[0],"Content is missing");
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
						
			default:
			{
			}
			break;
		}
#endif
		return false;
	}
	

}
