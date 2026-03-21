using UnityEngine;
using System.Collections;

public class TopBarInterfaceHandler : MonoBehaviour {
	
	public static TopBarInterfaceHandler mInstance = null;
	public DC_ChipsBalanceHandler mChipsBalanceHandler = null;
	public DC_ExperienceHandler mExperienceHandler = null;
	public DC_FriendsHandler mFriendsHandler = null;
	public DC_PlayerProfileHandler mPlayerProfileHandler = null;
	public DC_SettingsHandler mSettingsHandler = null;
	
	public LinkedSpriteManager mSpriteManager = null;
	public SpriteAtlasDataHandler mSpriteAtlasDataHandler = null;
	
	GameObject mTopBarImage = null;
	GameObject mXpProgressHolder = null;
	
	MenuButton mBackButton = null;
	MenuButton mPlayerButton = null;
	MenuButton mLevelsButton = null;
	MenuButton mSettingsButton = null;
	MenuButton mFriendsButton = null;
	
	CProgressBar mLevelProgress = null;
	
	CLabelAtlas mXpLevelNoLabel = null;
	
	// Use this for initialization
	void Start () {	StartCoroutine(Initialize()); }	
	// Update is called once per frame
	void Update () { }
	
	
	void init()
	{
		mTopBarImage = CommonData.createGameObject("TopBar Holder", gameObject, Vector3.zero, mSpriteManager, mSpriteAtlasDataHandler, "TopBar.png", 0);
		mTopBarImage.layer = gameObject.layer;
		
		mBackButton = MenuButtonManager.sharedManager().createMenuItem("Button_Back.png", "Button_Back_Selected.png", "", "", 1, mSpriteManager, mSpriteAtlasDataHandler, backSelected);
		mBackButton.gameObject.layer = gameObject.layer;
		mBackButton.addParent(gameObject);
		mBackButton.setPosition(new Vector3(-14.7f,0,0));
		
		mSettingsButton = MenuButtonManager.sharedManager().createMenuItem("Button_Settings.png", "Button_Settings_Selected.png", "", "", 1, mSpriteManager, mSpriteAtlasDataHandler, settingsSelected);
		mSettingsButton.gameObject.layer = gameObject.layer;
		mSettingsButton.addParent(gameObject);
		mSettingsButton.setPosition(new Vector3(14.3f,0,0));
		
		
		mLevelsButton = MenuButtonManager.sharedManager().createMenuItem("Button_Level.png", "Button_Level_Selected.png", "", "", 4, mSpriteManager, mSpriteAtlasDataHandler, levelSelected);
		mLevelsButton.gameObject.layer = gameObject.layer;
		mLevelsButton.addParent(gameObject);
		mLevelsButton.setPosition(new Vector3(-9.6f,0,0));
		
		mXpProgressHolder = CommonData.createGameObject("Progress Holder", gameObject, new Vector3(-5.5f,0,0), mSpriteManager, mSpriteAtlasDataHandler, "XP_ProgressBar.png", 1);
		mXpProgressHolder.layer = gameObject.layer;
		
		mLevelProgress = CProgressBar.create(mSpriteManager, mSpriteAtlasDataHandler, "XP_Progress.png", 2);
		mLevelProgress.addParent(mXpProgressHolder);
		mLevelProgress.gameObject.layer = gameObject.layer;
		mLevelProgress.setPercentage(50);	
		
		mXpLevelNoLabel = CLabelAtlas.create("9", mSpriteManager, mSpriteAtlasDataHandler, "Item_Numbers.png", 5);
		mXpLevelNoLabel.addParent(mLevelsButton.gameObject);
		mXpLevelNoLabel.gameObject.layer = gameObject.layer;
		mXpLevelNoLabel.setSpacing(-5.0f);
		mXpLevelNoLabel.setScale(1.0f);
		
		
		mPlayerButton = MenuButtonManager.sharedManager().createMenuItem("Button_Player.png", "Button_Player_Selected.png", "", "", 1, mSpriteManager, mSpriteAtlasDataHandler, playerSelected);
		mPlayerButton.gameObject.layer = gameObject.layer;
		mPlayerButton.addParent(gameObject);
		mPlayerButton.setPosition(new Vector3(-12.3f,0,0));
		
		
		mFriendsButton = MenuButtonManager.sharedManager().createMenuItem("Button_Friends.png", "Button_Friends_Selected.png", "", "", 1, mSpriteManager, mSpriteAtlasDataHandler, friendsSelected);
		mFriendsButton.gameObject.layer = gameObject.layer;
		mFriendsButton.addParent(gameObject);
		mFriendsButton.setPosition(new Vector3(12,0,0));
		
		mChipsBalanceHandler.init();
		
	}
	
	// Buttons Selected
	void backSelected()
	{
		Application.LoadLevel(1);
	}
	
	void settingsSelected()
	{
		Application.LoadLevel(Application.loadedLevel);
	}
	
	void playerSelected()
	{
	}
	
	void levelSelected()
	{
	}
	
	void friendsSelected()
	{
	}
	
	
	IEnumerator Initialize()
	{
		while(true)
		{
			yield return new WaitForEndOfFrame();			
			init();					
			break;
		}
	}
	
}
