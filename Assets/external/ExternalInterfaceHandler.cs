using UnityEngine;
using System;
using System.Collections;

public enum eEXTERNAL_REQ_TYPE
{
	Initialize, // initializing internet check and other things
	GetInAppData,
	InAppConsumable,	//1
	InAppNonConsumable,		//2
	Show_Banner_Top_Ads,	//3
	Show_Banner_Bottom_Ads,	//4
	Show_FullScreen_Ads,	//5
	Hide_Banner_Ads,		//6
	Hide_FullScreen_Ads,	//7
	Send_Score,				//8
	Send_Achievement,		//9
	Send_Flurry,			//10
	Show_Score,				//11
	Show_Achievement,		//12
	RateApplication,		//13
	ShowPopup,				//14
	Facebook,				//15
	Twitter,				//16
	MoreGames,				//17
	ApplicationQuit,
	StartLoadingScreen,
	StopLoadingScreen,
	SendAndroidLocalNotification,
	CancelAndroidLocalNotification
}

public class ExternalInterfaceHandler : MonoBehaviour 
{
	static ExternalInterfaceHandler mInstance = null;
	
	public delegate void ReceivedCallBack(eEXTERNAL_REQ_TYPE eRequestType, string strRequestedData, string result);
	public ReceivedCallBack OnCallBack;
	
	protected string mPrevRequestedData;
	
	protected eEXTERNAL_REQ_TYPE mePrevRequestedType;
	
	
	protected AdsHandler mAdsHander = new AdsHandler();
	
	
	public Texture2D bg;
	public Texture2D _tRotateTexture;
	protected float rotAngle = 0;
    protected Vector2 pivotPoint;
	protected Rect rect1;
	protected Rect rect2;
	protected int speed;
	
	
	void Awake()
	{
		Initialize();
	}
	// Use this for initialization
	public virtual void Initialize()
	{
		bg = Resources.Load("bg", typeof(Texture2D)) as Texture2D;
		_tRotateTexture = Resources.Load("loading_indicator", typeof(Texture2D)) as Texture2D;
		pivotPoint = new Vector2(Screen.width / 2, Screen.height / 2);
		rect1 = new Rect(pivotPoint.x - bg.width/2,pivotPoint.y - bg.height/2,bg.width,bg.height);
		pivotPoint = new Vector2(Screen.width / 2, Screen.height / 2-20);
		rect2 = new Rect(pivotPoint.x - _tRotateTexture.width/2,pivotPoint.y - _tRotateTexture.height/2,_tRotateTexture.width,_tRotateTexture.height);
		speed = 200;
		enabled = false;
	}
	
	public static ExternalInterfaceHandler Instance // Makes sure object is there available always in the scene
	{
		get
		{
			if(mInstance == null)
			{
				GameObject obj = new GameObject();
				obj.name = "ExternalInterfaceHandler";
				DontDestroyOnLoad(obj);
#if UNITY_EDITOR || UNITY_WEBPLAYER
				mInstance = obj.AddComponent<ExternalInterfaceHandler>();
#elif UNITY_IPHONE
				mInstance = obj.AddComponent<iOSInterfaceHandler>();
#elif UNITY_ANDROID
				mInstance = obj.AddComponent<AndroidinterfaceHandler>();
#elif UNITY_STANDALONE_OSX
				mInstance = obj.AddComponent<MacOSInterfaceHandler>();
#endif
			}
			return mInstance;
		}
	}
	void OnGUI() {
	 
        GUI.DrawTexture(rect1,bg);
		GUIUtility.RotateAroundPivot(rotAngle, pivotPoint);
        rotAngle += speed * Time.deltaTime;
		GUI.DrawTexture(rect2,_tRotateTexture);
	}

	public virtual bool SendRequest(eEXTERNAL_REQ_TYPE eRequestType, string strData,ReceivedCallBack callback)
	{
		Debug.Log(".....Editor Mode...." + strData );
		if(callback != null)
		{
			mePrevRequestedType = eRequestType;
			mPrevRequestedData = strData;
			OnCallBack = callback;
			
		}
		Receiver("true");
		switch(eRequestType)
		{
			case eEXTERNAL_REQ_TYPE.StartLoadingScreen:
			{
				enabled = true;
			}
				break;
			case eEXTERNAL_REQ_TYPE.StopLoadingScreen:
			{
				enabled = false;
			}
				break;
		}
		
		return true;
	}
	
	public virtual void Receiver(string status)
	{
		if(OnCallBack == null)
			return;
		string[] array = status.Split('_'); 
		
		if(array[0] == "Restore")
		{
			
			if(OnCallBack != null)
			{
				ReceivedCallBack temp = OnCallBack;
				OnCallBack = null;
				for(int i = 1;i<array.Length;i++)
				{
					temp(eEXTERNAL_REQ_TYPE.InAppNonConsumable,array[i],"true");
				}
			}
		}
		else if(array[0] == "RateApp")
		{
			// Rate App Callback
			ReceivedCallBack temp = OnCallBack;
			OnCallBack = null;
			temp(eEXTERNAL_REQ_TYPE.RateApplication,"",(array[1]=="0").ToString());
		}
		else
		{
			if(OnCallBack != null)
			{
				ReceivedCallBack temp = OnCallBack;
				OnCallBack = null;
				temp(mePrevRequestedType,mPrevRequestedData,status);
			}
		}
		
	}
}
