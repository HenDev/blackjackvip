using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SettingsHandler : PopUpBase {
	
	MenuButton mSoundButton = null;
	MenuButton mMusicButton = null;
	MenuButton mPushButton = null;
//	MenuButton mFacebookConnectButton = null;
//	MenuButton mFacebookDisconnectButton = null;
	
	GameObject mSoundButtonSelector = null;
	GameObject mMusicButtonSelector = null;
	GameObject mPushButtonSelector = null;
//	public GameObject mNoInternetPopUp = null;
//	public GameObject mLoadAnimatorBg = null;
//	public GameObject mLoadAnimator = null;
	
	float buttonSelectorGapWidth = 3.6f;
	
//	public static bool FaceBookConnected
//	{
//		get{ return PlayerPrefs.GetInt("IsFaceBookConnected") == 1; }
//		set{ PlayerPrefs.SetInt("IsFaceBookConnected", value?1:0); }
//	}
	
	bool SoundEnabled
	{
		get{ return PlayerPrefs.GetInt("IsSoundEnable") == 1; }
		set{ PlayerPrefs.SetInt("IsSoundEnable", value?1:0); }
	}
	
	bool MusicEnabled
	{
		get{ return PlayerPrefs.GetInt("IsMusicEnable") == 1; }
		set{ PlayerPrefs.SetInt("IsMusicEnable", value?1:0); }
	}
	
	// Use this for initialization
	void Start () { }	
	// Update is called once per frame
	void Update () { checkAndroidKeys(); }
	
	public override void init ()
	{
		IsInitStarted = true;
		base.init ();
		mBackButton.setPosition(new Vector3(0, -9.5f, -1));
		
		createButtons();
		createButtonSelectors();
//		createLoadAnimator();
//		hideLoadAnimator();
//		
//		if(FaceBookConnected && !OnlineSocialHandler.Instance.GetIsLoggedIn())
//			OnlineInterfaceHandler.Instance.SendRequest(eONLINE_REQ_TYPE.LOGIN, "", autoFacebookConnectCallBack);		
		
		IsInitDone = true;
	}
	
	protected override void removePopUp ()
	{
		base.removePopUp ();
		
		removeButtons();
		removeButtonSelectors();
	}
	
	public override void showPopUp ()
	{
//		toggleFacebookButtons();
		base.showPopUp ();
	}
	
	void createButtons()
	{
		float xpos = 4.0f;
		float yStartPos = 2.8f;
		float yGapHeight = 2.5f;
		mMusicButton = MenuButtonManager.sharedManager().createMenuItem("Button_OnOff.png", "Button_OnOff.png", "", "", 3, mSpriteManager, mSpriteAtlasDataHandler, musicSelected);
		mMusicButton.addParent(mPopUpBgHolder);
		mMusicButton.gameObject.layer = gameObject.layer;
		mMusicButton.setPosition(new Vector3(xpos,yStartPos, -3.5f));		
		
		mSoundButton = MenuButtonManager.sharedManager().createMenuItem("Button_OnOff.png", "Button_OnOff.png", "", "", 3, mSpriteManager, mSpriteAtlasDataHandler, soundSelected);
		mSoundButton.addParent(mPopUpBgHolder);
		mSoundButton.gameObject.layer = gameObject.layer;
		mSoundButton.setPosition(new Vector3(xpos,yStartPos - yGapHeight, -3.5f));		
		
		mPushButton = MenuButtonManager.sharedManager().createMenuItem("Button_OnOff.png", "Button_OnOff.png", "", "", 3, mSpriteManager, mSpriteAtlasDataHandler, pushSelected);
		mPushButton.addParent(mPopUpBgHolder);
		mPushButton.gameObject.layer = gameObject.layer;
		mPushButton.setPosition(new Vector3(xpos,yStartPos - yGapHeight*2.0f, -3.5f));
				
//		mFacebookConnectButton = MenuButtonManager.sharedManager().createMenuItem("Button_FBConnect.png", "Button_FBConnect_Pressed.png", "", "", 3, mSpriteManager, mSpriteAtlasDataHandler, connectFacebook);
//		mFacebookConnectButton.addParent(mPopUpBgHolder);
//		mFacebookConnectButton.gameObject.layer = gameObject.layer;
//		mFacebookConnectButton.setPosition(new Vector3(0,-5.5f,-1));
//				
//		mFacebookDisconnectButton = MenuButtonManager.sharedManager().createMenuItem("Button_FBDisConnect.png", "Button_FBDisConnect_Pressed.png", "", "", 3, mSpriteManager, mSpriteAtlasDataHandler, disconnectFacebook);
//		mFacebookDisconnectButton.addParent(mPopUpBgHolder);
//		mFacebookDisconnectButton.gameObject.layer = gameObject.layer;
//		mFacebookDisconnectButton.setPosition(new Vector3(0,-5.5f,-1));
//		
//		toggleFacebookButtons();
	}
	
	void createButtonSelectors()
	{
		mSoundButtonSelector = CommonData.createGameObject("Sound Button Selector", mPopUpBgHolder, mSoundButton.transform.position - transform.position + new Vector3((buttonSelectorGapWidth/2.0f)*(SoundEnabled?-1:1), -0.1f,0), mSpriteManager, mSpriteAtlasDataHandler, "Settings_Button_Selector.png", 5);
		mSoundButtonSelector.layer = gameObject.layer;
		
		mMusicButtonSelector = CommonData.createGameObject("Music Button Selector", mPopUpBgHolder, mMusicButton.transform.position - transform.position + new Vector3((buttonSelectorGapWidth/2.0f)*(MusicEnabled?-1:1), -0.1f,0), mSpriteManager, mSpriteAtlasDataHandler, "Settings_Button_Selector.png", 5);
		mMusicButtonSelector.layer = gameObject.layer;
		
		bool isPushSelected = LocalNotificationHandler.IsLocalNotificationEnabled;
		mPushButtonSelector = CommonData.createGameObject("Push Button Selector", mPopUpBgHolder, mPushButton.transform.position - transform.position + new Vector3((buttonSelectorGapWidth/2.0f)*(isPushSelected?-1:1), -0.1f,0), mSpriteManager, mSpriteAtlasDataHandler, "Settings_Button_Selector.png", 5);
		mPushButtonSelector.layer = gameObject.layer;
	}
	
	void removeButtonSelectors()
	{
		if(mSoundButtonSelector != null)
		{
			CommonData.destroyMyObject(mSoundButtonSelector);
			mSoundButtonSelector = null;
		}
		
		if(mMusicButtonSelector != null)
		{
			CommonData.destroyMyObject(mMusicButtonSelector);
			mMusicButtonSelector = null;
		}
		
		if(mPushButtonSelector != null)
		{
			CommonData.destroyMyObject(mPushButtonSelector);
			mPushButtonSelector = null;
		}
	}
	
	void removeButtons()
	{
		if(mSoundButton != null)
		{
			MenuButtonManager.sharedManager().removeChild(mSoundButton);
			mSoundButton = null;
		}
		
		if(mMusicButton != null)
		{
			MenuButtonManager.sharedManager().removeChild(mMusicButton);
			mMusicButton = null;
		}
		
		if(mPushButton != null)
		{
			MenuButtonManager.sharedManager().removeChild(mPushButton);
			mPushButton = null;
		}
		
//		if(mFacebookConnectButton != null)
//		{
//			MenuButtonManager.sharedManager().removeChild(mFacebookConnectButton);
//			mFacebookConnectButton = null;
//		}
//		
//		if(mFacebookDisconnectButton != null)
//		{
//			MenuButtonManager.sharedManager().removeChild(mFacebookDisconnectButton);
//			mFacebookDisconnectButton = null;
//		}
	}
	
	// Buttons Selected
	void soundSelected()
	{		
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		SoundEnabled = !SoundEnabled;
		toggleButton(mSoundButtonSelector, SoundEnabled);
	}
	
	void musicSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		MusicEnabled = !MusicEnabled;
		toggleButton(mMusicButtonSelector, MusicEnabled);
		PlayAudioSounds.sharedHandler().toggleBgMusic();
	}
	
	void pushSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		LocalNotificationHandler.IsLocalNotificationEnabled = !LocalNotificationHandler.IsLocalNotificationEnabled;
		LocalNotificationHandler.mInstance.scheduleLocalNotificationForBlackjack();
		toggleButton(mPushButtonSelector, LocalNotificationHandler.IsLocalNotificationEnabled);
	}
	
	void toggleButton(GameObject buttonSelector, bool enabled)
	{
		buttonSelector.transform.position += new Vector3(buttonSelectorGapWidth*(enabled?-1:1),0,0);
	}
	
//	IEnumerator AutoConnectFacebook()
//	{
//		while(true)
//		{
//			yield return new WaitForSeconds(5*60);
//			if(OnlineSocialHandler.Instance.GetIsLoggedIn()) break;
//			Debug.Log("AutoConnect Facebook");
//			OnlineInterfaceHandler.Instance.SendRequest(eONLINE_REQ_TYPE.LOGIN,"",autoFacebookConnectCallBack);			
//		}		
//	}
//	
//	void autoFacebookConnectCallBack(string strResult ,eONLINE_REQ_TYPE eRequestType,IList<UserData> data = null)
//	{
//		Debug.Log("Test CallBack : "+strResult);
//		if(eRequestType == eONLINE_REQ_TYPE.LOGIN)
//		{			
//			if(strResult != "Logged In") return;
//			Debug.Log("Facebook Connected");
//			FaceBookConnected = true;
//			toggleFacebookButtons();
//		}
//		else// if(eRequestType == eONLINE_REQ_TYPE.NO_INTERNET_CONNECTION)
//		{
//			Debug.Log("Facebook Connection Failed");
//			StartCoroutine(AutoConnectFacebook());
//		}
//	}
//	
//	void facebookConnectCallBack(string strResult ,eONLINE_REQ_TYPE eRequestType,IList<UserData> data = null)
//	{
//		Debug.Log("Test CallBack : "+strResult);
//		if(eRequestType == eONLINE_REQ_TYPE.LOGIN || eRequestType == eONLINE_REQ_TYPE.LOGOUT)
//		{			
//			FaceBookConnected = eRequestType == eONLINE_REQ_TYPE.LOGIN;
//			toggleFacebookButtons();
//			Debug.Log("Facebook "+(FaceBookConnected?"Connected":"Disconnected"));
//		}
//		else
//		{
//			Debug.Log("Facebook Connection Failed");	
//			showNoInternetConnection();
//		}
//		hideLoadAnimator();
//	}
//	
//	void connectFacebook()
//	{
//		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
//		OnlineInterfaceHandler.Instance.SendRequest(eONLINE_REQ_TYPE.LOGIN,"",facebookConnectCallBack);
//		showLoadAnimator();
//		
//		mFacebookConnectButton.setTouchEnable(false);
//	}
//	
//	void disconnectFacebook()
//	{
//		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
//		OnlineInterfaceHandler.Instance.SendRequest(eONLINE_REQ_TYPE.LOGOUT,"",facebookConnectCallBack);
//		showLoadAnimator();
//		
//		mFacebookDisconnectButton.setTouchEnable(false);
//	}
//	
//	void toggleFacebookButtons()
//	{
//		mFacebookConnectButton.setTouchEnable(!FaceBookConnected);
//		mFacebookConnectButton.setVisible(!FaceBookConnected);
//		mFacebookDisconnectButton.setTouchEnable(FaceBookConnected);
//		mFacebookDisconnectButton.setVisible(FaceBookConnected);		
//	}
//	
//	// Loading Animator
//	void createLoadAnimator()
//	{		
//		CRotateBy rotate = mLoadAnimator.AddComponent<CRotateBy>();
//		CRepeat repeat = mLoadAnimator.AddComponent<CRepeat>();
//		
//		rotate.actionWith(mLoadAnimator, -Vector3.forward*70, 0.1f);
//		repeat.actionWithAction(rotate, -1);
//		repeat.runAction();
//	}
//	
//	void showLoadAnimator()
//	{
//		mLoadAnimator.SetActive(true);
//		mLoadAnimatorBg.SetActive(true);
//	}
//	
//	void hideLoadAnimator()
//	{
//		mLoadAnimator.SetActive(false);
//		mLoadAnimatorBg.SetActive(false);
//	}
//	
//	// No Internet Connecction	
//	void showNoInternetConnection()
//	{
//		Debug.Log("NoInternetMessage Show");
//		if(mNoInternetPopUp.activeSelf) return;
//		mNoInternetPopUp.SetActive(true);
//		mBackButton.setTouchEnable(false);
//		
//		CommonData.removeAllActionsFrom(mNoInternetPopUp);
//		
//		CMoveBy move = mNoInternetPopUp.AddComponent<CMoveBy>();
//		CEaseExponential ease = mNoInternetPopUp.AddComponent<CEaseExponential>();
//		CDelayTime delay = mNoInternetPopUp.AddComponent<CDelayTime>();
//		CCallFunc call = mNoInternetPopUp.AddComponent<CCallFunc>();
//		CSequence seq = mNoInternetPopUp.AddComponent<CSequence>();
//		
//		move.actionWith(mNoInternetPopUp, new Vector2(0, -10.0f), 0.5f);
//		ease.actionWithAction(move, EaseType.EaseOut);
//		delay.actionWithDuration(2.0f);
//		call.actionWithCallBack(hideNoInternetConnection);
//		seq.actionWithActions(ease, delay, call);
//		
//		seq.runAction();
//	}
//	
//	void hideNoInternetConnection()
//	{		
//		CommonData.removeAllActionsFrom(mNoInternetPopUp);
//		
//		CMoveBy move = mNoInternetPopUp.AddComponent<CMoveBy>();
//		CEaseExponential ease = mNoInternetPopUp.AddComponent<CEaseExponential>();
//		CCallFunc call = mNoInternetPopUp.AddComponent<CCallFunc>();
//		CSequence seq = mNoInternetPopUp.AddComponent<CSequence>();
//		
//		move.actionWith(mNoInternetPopUp, new Vector2(0, 10.0f), 0.5f);
//		ease.actionWithAction(move, EaseType.EaseIn);
//		call.actionWithCallBack(hideNoInternetConnectionCompleted);
//		seq.actionWithActions(ease, call);
//		
//		seq.runAction();
//	}
//	
//	void hideNoInternetConnectionCompleted()
//	{
//		Debug.Log("NoInternetMessage Hide");
//		mNoInternetPopUp.SetActive(false);
//		mBackButton.setTouchEnable(true);	
////		if(FaceBookConnected) mFacebookDisconnectButton.setTouchEnable(true);
////		else mFacebookConnectButton.setTouchEnable(true);
//	}
}
