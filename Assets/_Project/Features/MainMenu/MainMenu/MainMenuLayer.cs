using UnityEngine;
using System.Collections;

public class MainMenuLayer : MonoBehaviour
{

	public static MainMenuLayer mInstance = null;
	public static bool isCallFullScreenAds = true;

	public LinkedSpriteManager mSpriteManager = null;
	public SpriteAtlasDataHandler mSpriteAtlasDataHandler = null;
	public TableListHolder mTableListHandler = null;

	public AlphaLayerHandler mAlphaLayer = null;
 
	public GameObject mTittle;
	MenuButton mStatsButton = null;
	MenuButton mScoreButton = null;
	MenuButton mAchievementsButton = null;
//	MenuButton mFaceBookButton = null;
	MenuButton mSettingsButton = null;

	bool isSettingsExpanded = false;
	float settingExpandCollapseDuration = 0.1f;

	public		float TopBarYPositionOffset = 0.1f;
	public		float yPos = 0.1f;
	public		float yPosGap = 0.1f;
	public	float buttonScale = 0.1f;
	public float xPosLeft = -10.0f;
	public float xPosRight = 10.0f;
	public float uiScale = 1.0f;
	public GameObject dailyBonus;

// Use this for initialization
	void Start () { mInstance = this; PlayAudioSounds.sharedHandler().playBgMusic("MenuTune"); StartCoroutine(Instantiate()); }
	
	// Update is called once per frame
	void Update()
	{
		checkAndroidKeys(); 
		positionUI();

	}
	
	IEnumerator Instantiate()
	{
		while(true)
		{
			yield return new WaitForEndOfFrame();
			if(isCallFullScreenAds)
			{
				Debug.Log("Full Screen Ads");
				ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Show_FullScreen_Ads, "", null);
				isCallFullScreenAds = false;
 			}
			init();
			break;
		}
	}

	void init()
	{
		TopBarYPositionOffset =MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
			6.0f,  
			5.7f,  
			5.35f,  
			4.00f,  
			3.8f,   
			3.5f,  
			3.2f,   
			2.0f,   
			0.0f,    
			-1.0f,  
			2.0f,   
			1.0f,   
			0.0f   
		);
		yPos = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
			22.0f, 
			21.0f,  
			21.2f, 
			19.4f,  
			19.0f, 
			18.5f,  
			18.0f,  
			16.0f, 
			14.0f, 
			13.3f, 
			16.0f, 
			15.0f,  
			14.0f   
		);
		yPosGap = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
			3.2f,   
			3.1f,  
			3.0f,  
			3.0f,   
			3.0f,   
			3.0f,  
			2.75f,  
			3.0f,  
			2.5f,  
			2.5f,   
			3.0f,  
			2.75f,  
			2.5f    
		);
		buttonScale = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
			1.35f,  
			1.33f,  
			1.3f,   
			1.3f,   
			1.3f,   
			1.3f,   
			1.15f, 
			1.3f,  
			1.0f,   
			1.0f,  
			1.3f,   
			1.15f,  
			1.0f    
		);
 
		Debug.Log("CurrentLoadedDeviceResolution: " + MultiplePlatformPortingHandler.Instance.CurrentLoadedDeviceResolution);

		switch (MultiplePlatformPortingHandler.Instance.CurrentLoadedDeviceResolution)
		{
			case MyDeviceResolutions.Res_3_2:
				xPosRight = 10.5f;
				xPosLeft = -10.5f;
				yPos = 10.0f;
				break;
			case MyDeviceResolutions.Res_19_5_9:
				xPosRight = 10.0f;
				xPosLeft = -10.0f;
				yPos = 17.4f;
				mTittle.transform.position = new Vector3(0, MultiplePlatformPortingHandler.Instance.ScreenTop - 7.0f, 0);
				break;
			case MyDeviceResolutions.Res_21_9:
				yPos = 19.0f;
				mTittle.transform.position = new Vector3(0, MultiplePlatformPortingHandler.Instance.ScreenTop - 8.7f, 0);
				break;
			case MyDeviceResolutions.Res_20_9:
				yPos = 19.0f;
				mTittle.transform.position = new Vector3(0, MultiplePlatformPortingHandler.Instance.ScreenTop - 8.3f, 0);
				break;
			case MyDeviceResolutions.Res_22_9:
				yPos = 19.0f;
				mTittle.transform.position = new Vector3(0, MultiplePlatformPortingHandler.Instance.ScreenTop - 9.2f, 0);
				break;
			case MyDeviceResolutions.Res_16_10:
				yPos = 8.0f;
				this.gameObject.transform.position = new Vector3(0,4.0f,0);
				break;
			case MyDeviceResolutions.Res_5_3:
				yPos = 12.8f;
				mTittle.transform.position = new Vector3(0, MultiplePlatformPortingHandler.Instance.ScreenTop - 3.2f, 0);
				break;
			case MyDeviceResolutions.Res_16_9:
				yPos = 13.0f;
				mTittle.transform.position = new Vector3(0, MultiplePlatformPortingHandler.Instance.ScreenTop - 5.2f, 0);
				break;
			case MyDeviceResolutions.Res_18_9:
				yPos = 16.5f;
				mTittle.transform.position = new Vector3(0, MultiplePlatformPortingHandler.Instance.ScreenTop - 6.2f, 0);
				break;
			case MyDeviceResolutions.Res_18_5_9:
				yPos = 17.0f;
				mTittle.transform.position = new Vector3(0, MultiplePlatformPortingHandler.Instance.ScreenTop - 6.6f, 0);
				break;
			case MyDeviceResolutions.Res_19_9:
				yPos = 17.0f;
				mTittle.transform.position = new Vector3(0, MultiplePlatformPortingHandler.Instance.ScreenTop - 7.0f, 0);
				break;
			case MyDeviceResolutions.Res_10_7:
				yPos = 9.5f;
				break;
			default:
				break;
		}
		 
		GetComponentInChildren<SpriteData>().gameObject.transform.position += new Vector3(0,TopBarYPositionOffset,0);

		mStatsButton = MenuButtonManager.sharedManager().createMenuItem("Button_Stats.png", "Button_Stats_Pressed.png", "", "", 1, mSpriteManager, mSpriteAtlasDataHandler, statsSelected);
		mStatsButton.addParent(gameObject);
		mStatsButton.gameObject.layer = gameObject.layer;	
		mStatsButton.transform.localScale = Vector3.one*buttonScale;
		
		mScoreButton = MenuButtonManager.sharedManager().createMenuItem("Button_Score.png", "Button_Score_Pressed.png", "", "", 1, mSpriteManager, mSpriteAtlasDataHandler, scoreSelected);
		mScoreButton.addParent(gameObject);
		mScoreButton.gameObject.layer = gameObject.layer;
		mScoreButton.transform.localScale = Vector3.one*buttonScale;
		
		mAchievementsButton = MenuButtonManager.sharedManager().createMenuItem("Button_Achievement.png", "Button_Achievement_Pressed.png", "", "", 1, mSpriteManager, mSpriteAtlasDataHandler, achievementSelected);
		mAchievementsButton.addParent(gameObject);
		mAchievementsButton.gameObject.layer = gameObject.layer;
		mAchievementsButton.transform.localScale = Vector3.one*buttonScale;
		
		mSettingsButton = MenuButtonManager.sharedManager().createMenuItem("Button_Setting.png", "Button_Setting_Pressed.png", "", "", 2, mSpriteManager, mSpriteAtlasDataHandler, settingsSelected);
		mSettingsButton.addParent(gameObject);
		mSettingsButton.gameObject.layer = gameObject.layer;
		mSettingsButton.transform.localScale = Vector3.one*buttonScale;
		
//		mFaceBookButton = MenuButtonManager.sharedManager().createMenuItem("Button_Facebook.png", "Button_Facebook_Pressed.png", "", "", 1, mSpriteManager, mSpriteAtlasDataHandler, facebookSelected);
//		mFaceBookButton.addParent(gameObject);
//		mFaceBookButton.setPosition(new Vector3(xPosRight,yPos-yPosGap,0));
//		mFaceBookButton.gameObject.layer = gameObject.layer;
//		mFaceBookButton.transform.localScale = Vector3.one*buttonScale;
		
		mAlphaLayer.fadeOutAlphaLayer(0.2f);
		
		disableMainMenu();
		CCallFunc.createCallAfterDelay(enableMainMenu, 0.5f).runAction();
	}

	void positionUI()
	{
		if (mStatsButton)
		{
			mStatsButton.setPosition(new Vector3(xPosLeft,yPos,0));
			mScoreButton.setPosition(new Vector3(xPosLeft,yPos-yPosGap,0));
			mAchievementsButton.setPosition(new Vector3(xPosLeft,yPos-yPosGap*2,0));
			mSettingsButton.setPosition(new Vector3(xPosRight,yPos,0));	
			
 			dailyBonus.transform.localScale = new Vector3(uiScale,uiScale,1);

		}

	}
	
	// Android Buttons
	void checkAndroidKeys()
	{
		if(Input.GetKeyDown(KeyCode.Escape) && mStatsButton.isTouchEnabled()) // Back Button
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.ShowPopup, "", null);
		}
	}
	
	// Disable / Enable
	public void disableMainMenu()
	{
		mStatsButton.setTouchEnable(false);
		mScoreButton.setTouchEnable(false);
		mSettingsButton.setTouchEnable(false);
		mAchievementsButton.setTouchEnable(false);
//		mFaceBookButton.setTouchEnable(false);
		mTableListHandler.disableTableListHandler();
		BJ_BottomBar.mInstance.disableButtonsForPopUp();
	}
	
	public void enableMainMenu()
	{
		mStatsButton.setTouchEnable(true);
		mScoreButton.setTouchEnable(true);
		mSettingsButton.setTouchEnable(true);
		mAchievementsButton.setTouchEnable(true);
//		mFaceBookButton.setTouchEnable(true);
		mTableListHandler.enableTableListHandler();
		BJ_BottomBar.mInstance.enableButtonsAfterPopUp();
	}
	
	// Buttons Selected
	void statsSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		PopUpManager.mInstance.showPopUp(PopUpManager.PopUpType.Stats, enableMainMenu);
		disableMainMenu();
	}
	
//	void facebookSelected()
//	{
//		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
//		PopUpManager.mInstance.showPopUp(PopUpManager.PopUpType.FriendsLeaderboard, enableMainMenu);
//		disableMainMenu();
//	}
	
	void settingsSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		PopUpManager.mInstance.showPopUp(PopUpManager.PopUpType.Settings, enableMainMenu);
		disableMainMenu();
	}
	
	void scoreSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Show_Score, "", null);
	}
	
	void achievementSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Show_Achievement, "", null);
	}
	
//	void disableSettingsButtons()
//	{
//		mAchievementsButton.setEnable(false);
//		mFaceBookButton.setEnable(false);
//		mStatsButton.setEnable(false);
//		mScoreButton.setEnable(false);
//	}
//	
//	void moveTheButtonTo(MenuButton button, float yOffset)
//	{
//		CMoveTo move = button.GetComponent<CMoveTo>();
//		move.actionWith(button.gameObject, button.transform.position+new Vector3(0, yOffset, 0), settingExpandCollapseDuration);
//		move.runAction();
//	}
//	
//	void moveTheToggleButtonTo(MenuToggleButton button, float yOffset)
//	{
//		CMoveTo move = button.GetComponent<CMoveTo>();
//		move.actionWith(button.gameObject, button.transform.position+new Vector3(0, yOffset, 0), settingExpandCollapseDuration);
//		move.runAction();
//	}
}
