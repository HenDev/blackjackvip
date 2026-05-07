using UnityEngine;
using System.Collections;

public class BJ_BottomBar : MonoBehaviour {
	
	public static BJ_BottomBar mInstance = null;
	public LinkedSpriteManager mSpriteManager = null;
	public SpriteAtlasDataHandler mSpriteAtlasDataHandler = null;
	public LinkedSpriteManager mChipsSpriteManager = null;
	public SpriteAtlasDataHandler mChipsSpriteAtlasDataHandler = null;
	public LinkedSpriteManager mButtonsSpriteManager = null;
	public SpriteAtlasDataHandler mButtonsSpriteAtlasDataHandler = null;
	
	public BonusHandler mBonusHandler = null;
	
	public GameObject mAdsBar = null;
	public GameObject mTopBarHolder_min = null;
	public GameObject mTopBarHolder_max = null;
	public GameObject betInsuranceBg = null;
	public GameObject mChipsBarHolder = null;
	GameObject bonusTimerHolder = null;
	
	private bool mIsTopBarActive = false;
	private float topBarYPosHidden = 0.0f;
	private float topBarYposShown = 0.0f;
		
	public float[] mArrChipValues = new float[0];
//	ArrayList mArrHistoryOfDealer = new ArrayList();
	
	public float holderMoveSpeed = 0.2f;
	public float chipsMoveSpeed = 0.2f;
		
	MenuButton mDealButton = null;
	MenuButton mClearButton = null;
	MenuButton mRebetButton = null;
	MenuButton mBackButton = null;
	MenuButton mStandButton = null;
	MenuButton mDoubleButton = null;
	MenuButton mHitButton = null;
	MenuButton mSplitButton = null;
	MenuButton mSurrenderButton = null;	
	MenuButton mYesButton = null;
	MenuButton mNoButton = null;
	MenuButton mAddChipsButton = null;
	MenuButton[] mArrChipsButtons = null;
	MenuButton mAdsRemoveButton = null;
//	MenuButton mRestorePrchaseButton = null;
//	MenuToggleButton mSoundsButton = null;
//	MenuToggleButton mMusicsButton = null;
	
	MenuButton bonusButton = null;
	
	CLabelAtlas mChipsBalanceLabel = null;
	public CLabelAtlas timerLabel = null;
	CLabelAtlas mBetPlacedLabel = null;
	
	GameObject mBetButtonsHolder = null;
	GameObject mDealButtonsHolder = null;
	GameObject mBetInsuranceHolder = null;
	GameObject mBetsPlacedDisplayHolder = null;
	
	GameObject mDGChip = null;
	
	private int balanceUpdateRate = 0;
	private int currentChipsBalance = 0;
	public int PlayerChipsBalance
	{
		get { return PlayerPrefs.GetInt("PlayerChipBalance"); }
		set { PlayerPrefs.SetInt("PlayerChipBalance", (value > 0 ? value : 0)); }
	}
	
//	CLabelAtlas mTotalBetPlacedLabel = null;
//	CLabelAtlas mTotalWinPlacedLabel = null;
//	CLabelAtlas[] mDealerHistoryLabel = null;
	
	bool mIsChipsVisible = false;
	int mCurrentSelectedChipIndex = 1;
	
	// Use this for initialization
	void Start () {	
		
		currentChipsBalance = PlayerChipsBalance;
		topBarYPosHidden = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
			55.0f,   
			52.0f,   
			48.0f,  
			47.0f,  
			46.0f,  
			45.0f,  
			45.0f,  
			37.5f,   
			34.5f,  
			32.7f,  
			35.0f, 
			33.0f,   
			31.0f   
		);
		topBarYposShown = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
			50.4f,  
			47.8f,   
			45.36F,   
			44.5f,   
			43.0f,  
			42.5f,  
			40.5f,   
			35.0f,  
			32.3f,   
			30.7f,  
			33.0f, 
			31.0f,  
			29.0f    
		);
		float yPosOffset = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
			2.1f,  
			2.1f,  
			2.1f, 
			2.1f, 
			2.1f, 
			2.1f,  
			2.1f,   
			2.1f,  
			2.1f,  
			2.1f,   
			2.1f, 
			2.1f,   
			2.1f  
		);
		float top = MultiplePlatformPortingHandler.Instance.ScreenTop;
		float posBottom =MultiplePlatformPortingHandler.Instance.ScreenBottom + yPosOffset;
		transform.position = new Vector3(0,posBottom,0);
		Debug.Log("Top: " + top);
		if(mAdsBar != null)
		{
			if(Loading.IsAdsRemoved || Application.loadedLevelName == "2-MainMenuScene"){
				mTopBarHolder_max.SetActive(false);
				mTopBarHolder_min.SetActive(false);
				mAdsBar.transform.position = new Vector3(0, top+yPosOffset, mAdsBar.transform.position.z);
			}
			else
			{

				topBarYPosHidden -= MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
					0.0f,   
					0.0f,  
					0.0f,  
					0.0f,  
					0.0f,   
					0.0f,   
					0.2f,  
					0.0f,   
					0.0f,   
					0.0f,  
					4.25f,   
					4.4f,   
					4.1f    
				);
				topBarYposShown -= MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
					0.0f,   
					0.0f,   
					0.0f,   
					0.0f,  
					0.0f,  
					0.0f,  
					0.0f, 
					0.0f,  
					0.0f,  
					0.0f,  
					4.25f,   
					4.4f,   
					4.1f    
				); 
				mAdsBar.transform.position = new Vector3(0, top - yPosOffset, mAdsBar.transform.position.z);	
			}
		}
		
		removeTopBarMax();
		removeTopBarMin();
		
		StartCoroutine(Initialize()); 
	}	
	// Update is called once per frame
	void Update () { updateChipsDisplay(); checkAndroidKeys(); }
	
	void init()
	{
		if(Application.loadedLevelName == "2-MainMenuScene")
		{
			// Main Menu
			initMenuBar();
		}
		else
		{
			// Game Play
			initGameBar();
		}
	}
	
	protected void checkAndroidKeys()
	{
		if(mBackButton != null && mBackButton.isTouchEnabled() && BJ_GamePlayLayer.mInstance != null && Input.GetKeyDown(KeyCode.Escape)) // Back Button
		{
			backSelected();
		}
	}
	
	void initMenuBar()
	{
		mInstance = this;
		createChipsBalance();
		createBonusDisplay();
		createAdsButtons();
		createSoundButtons();
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Hide_Banner_Ads, "", null);
	}
	
	void initGameBar()
	{
		mInstance = this;
		createChipsBalance();
		createBonusDisplay();
		createChipsBar();
		createButtons();
		createBetsPlaced();
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Show_Banner_Top_Ads, "", null);
	}
	
	IEnumerator Initialize()
	{
		while(true)
		{
			yield return new WaitForFixedUpdate();
			init();
			break;
		}
	}
	
	void createChipsBar()
	{	
		mChipsBarHolder.SetActive(true);
		
		float yAxis = -0.15f;
		float xGap = 4.0f;
		mArrChipsButtons = new MenuButton[5];
		for(int i=0; i<5; i++)
		{					
			string buttonName = "Chip_"+(i+TableHandler.startChipIndex)+".png";
			MenuButton markerButton = MenuButtonManager.sharedManager().createMenuItem(buttonName, buttonName, "", "", 0, mChipsSpriteManager, mChipsSpriteAtlasDataHandler, chipsSelected);
			markerButton.addParent(mChipsBarHolder);
			Debug.Log("mChipsBarHolder Position : "+mChipsBarHolder.transform.position);
			markerButton.setPosition(new Vector3((i-2)*xGap,yAxis,-1.0f));
			markerButton.setTag(i+TableHandler.startChipIndex);
			markerButton.setTouchEnable(false);
			markerButton.gameObject.layer = gameObject.layer;
			mArrChipsButtons[i] = markerButton;			
		}			
		showChips();
	}
	
	void createBonusDisplay()
	{
		bonusTimerHolder = CommonData.createGameObject("Bonus Timer Holder", gameObject, new Vector3(0,-1.1f,-3), mSpriteManager, mSpriteAtlasDataHandler, "TimerHolder.png", 3);
		bonusTimerHolder.layer = gameObject.layer;
		
		timerLabel = CLabelAtlas.create("00:00:00", mSpriteManager, mSpriteAtlasDataHandler, "NumberFont1.png", 4);
		timerLabel.addParent(bonusTimerHolder);
		timerLabel.gameObject.layer = gameObject.layer;
		timerLabel.setScale(0.8f);
		
		bonusButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/ClaimBonus_Button.png", "Buttons/ClaimBonus_Button_Pressed.png", "", "", 0, mSpriteManager, mSpriteAtlasDataHandler, addChipsSelected);
		bonusButton.addParent(gameObject);
		bonusButton.gameObject.layer = gameObject.layer;
		bonusButton.setPosition(new Vector3(0,-1.2f,-3));
		
		setTimer(mBonusHandler.timerHours, mBonusHandler.timerMinutes, Mathf.FloorToInt(mBonusHandler.timerSeconds));
		
		if(mBonusHandler.isBonusActive) bonusTimerHolder.transform.position += new Vector3(0,1.7f,0);
		else 
		{
			bonusButton.transform.position += new Vector3(0,1.7f,0);
			bonusButton.setTouchEnable(false);
		}
	}
	
	void createAdsButtons()
	{
		if(!Loading.IsAdsRemoved)
		{
			mAdsRemoveButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/RemoveAds_Button.png", "Buttons/RemoveAds_Button_Pressed.png", "", "", 5, mSpriteManager, mSpriteAtlasDataHandler, removeAdsSelected);
			mAdsRemoveButton.addParent(gameObject);
			mAdsRemoveButton.setPosition(new Vector3(-9.5f,-0.2f,-3));
		}
				
//		mRestorePurchaseButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/RestorePurchase_Button.png", "Buttons/RestorePurchase_Button_Pressed.png", "", "", 5, mSpriteManager, mSpriteAtlasDataHandler, restorePurchaseSelected);
//		mRestorePurchaseButton.addParent(gameObject);
//		mRestorePurchaseButton.setPosition(new Vector3(9.5f,-0.2f,-3));
//		mRestorePurchaseButton.setEnable(false);
	}
	
	void createChipsBalance()
	{
		GameObject chipsBalanceHolder = CommonData.createGameObject("Chip Balance Holder", gameObject, new Vector3(0,0.6f,-3), mSpriteManager, mSpriteAtlasDataHandler, "ChipBalanceHolder.png", 5);
		chipsBalanceHolder.layer = gameObject.layer;		
		
		mChipsBalanceLabel = CLabelAtlas.create(PlayerChipsBalance.ToString(), mSpriteManager, mSpriteAtlasDataHandler, "NumberFont.png", 6, CLabelAtlas.LabelTextAlignment.Right);
		mChipsBalanceLabel.addParent(chipsBalanceHolder);
		mChipsBalanceLabel.setPosition(new Vector3(2.5f,0,0));
		mChipsBalanceLabel.setSpacing(-5.0f);
		mChipsBalanceLabel.setScale(1.0f);
		mChipsBalanceLabel.gameObject.layer = gameObject.layer;
		
		mAddChipsButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/AddChips_Button.png", "Buttons/AddChips_Button_Pressed.png", "", "", 7, mSpriteManager, mSpriteAtlasDataHandler, addChipsSelected);
		mAddChipsButton.addParent(chipsBalanceHolder);
		mAddChipsButton.gameObject.layer = gameObject.layer;
		mAddChipsButton.setPosition(new Vector3(4.0f,0,0));
		
		mChipsBalanceLabel.setPosition(new Vector3(4.5f,0,0));
		mAddChipsButton.setPosition(new Vector3(6.0f,0,0));
		
		mDGChip = CommonData.createGameObject("DG_Chip", chipsBalanceHolder , new Vector3(-5.5f,0,0), mSpriteManager, mSpriteAtlasDataHandler, "DG_Chip/DG_Chip_1.png", 10);
		mDGChip.layer = gameObject.layer;		
		//CommonData.addIdealAnimationTo(mDGChip, "DG_Chip/DG_Chip_", 6, 10);
	}
	
	void createBetsPlaced()
	{
		mBetsPlacedDisplayHolder = CommonData.createGameObject("Bets Placed Display", gameObject, new Vector3(0,6.6f,0), mSpriteManager, mSpriteAtlasDataHandler, "BetHolder.png", 0);
		mBetsPlacedDisplayHolder.layer = gameObject.layer;
		
		mBetPlacedLabel = CLabelAtlas.create("0", mSpriteManager, mSpriteAtlasDataHandler, "NumberFont.png", 1);
		mBetPlacedLabel.addParent(mBetsPlacedDisplayHolder);
		mBetPlacedLabel.setPosition(new Vector3(0.9f,0,0));
		mBetPlacedLabel.setSpacing(-5.0f);
		mBetPlacedLabel.setScale(0.7f);
		mBetPlacedLabel.gameObject.layer = gameObject.layer;
	}
	
	void createSoundButtons()
	{		
//		mSoundsButton = MenuToggleButton.createToggleButton("Buttons/Button_SoundOn.png", "Buttons/Button_SoundOff.png", "", "", 1, mSpriteManager, mSpriteAtlasDataHandler, soundsSelected, PlayerPrefs.GetInt("IsSoundEnable"));
//		mSoundsButton.addParent(gameObject);
//		mSoundsButton.setPosition(new Vector3(8.3f,0,-3));
//		mSoundsButton.gameObject.layer = gameObject.layer;
//		
//		mMusicsButton = MenuToggleButton.createToggleButton("Buttons/Button_MusicOn.png", "Buttons/Button_MusicOff.png", "", "", 1, mSpriteManager, mSpriteAtlasDataHandler, musicSelected, PlayerPrefs.GetInt("IsMusicEnable"));
//		mMusicsButton.addParent(gameObject);
//		mMusicsButton.setPosition(new Vector3(10.7f,0,-3));
//		mMusicsButton.gameObject.layer = gameObject.layer;	
	}
	
	void createButtons()
	{						
		mBackButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/Back_Button.png", "Buttons/Back_Button_Pressed.png", "Buttons/Back_Button_Disabled.png", "", 5, mSpriteManager, mSpriteAtlasDataHandler, backSelected);
		mBackButton.addParent(gameObject);
		mBackButton.setPosition(new Vector3(-9.5f,-0.2f,-3));
				
		mClearButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/Clear_Button.png", "Buttons/Clear_Button_Pressed.png", "Buttons/Clear_Button_Disabled.png", "", 5, mSpriteManager, mSpriteAtlasDataHandler, clearSelected);
		mClearButton.addParent(gameObject);
		mClearButton.setPosition(new Vector3(9.5f,-0.2f,-3));
		mClearButton.setEnable(false);
				
		mBetButtonsHolder = CommonData.createEmptyGameObject("BJ_BetButtonsHolder", gameObject, new Vector3(0,0,5));
		mBetButtonsHolder.layer = gameObject.layer;
		
		mRebetButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/Rebet_Button.png", "Buttons/Rebet_Button_Pressed.png", "Buttons/Rebet_Button_Disabled.png", "", 0, mButtonsSpriteManager, mButtonsSpriteAtlasDataHandler, rebetSelected);
		mRebetButton.addParent(mBetButtonsHolder);
		mRebetButton.setPosition(new Vector3(0.0f,9.0f,5));
		mRebetButton.setEnable(false);
		mRebetButton.setVisible(false);
		
		mDealButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/Deal_Button.png", "Buttons/Deal_Button_Pressed.png", "Buttons/Deal_Button_Disabled.png", "", 0, mButtonsSpriteManager, mButtonsSpriteAtlasDataHandler, dealSelected);
		mDealButton.addParent(mBetButtonsHolder);
		mDealButton.setPosition( new Vector3(0.0f,9.0f,5));
		mDealButton.setEnable(false);
				
		float buttonsYPosition = 3.5f;
		mDealButtonsHolder = CommonData.createEmptyGameObject("BJ_DealButtonsHolder", gameObject, new Vector3(0,-32,5));
		mDealButtonsHolder.layer = gameObject.layer;
		
		mHitButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/Hit_Button.png", "Buttons/Hit_Button_Pressed.png", "Buttons/Hit_Button_Disabled.png", "", 0, mButtonsSpriteManager, mButtonsSpriteAtlasDataHandler, hitSelected);
		mHitButton.addParent(mDealButtonsHolder);
		mHitButton.setPosition(new Vector3(7.5f,6.5f,5));	
		mHitButton.setEnable(false);	
		
		mStandButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/Stand_Button.png", "Buttons/Stand_Button_Pressed.png", "Buttons/Stand_Button_Disabled.png", "", 0, mButtonsSpriteManager, mButtonsSpriteAtlasDataHandler, standSelected);
		mStandButton.addParent(mDealButtonsHolder);
		mStandButton.setPosition(new Vector3(-7.5f,6.5f,5));
		mStandButton.setEnable(false);
		
		mSurrenderButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/Surrender_Button.png", "Buttons/Surrender_Button_Pressed.png", "Buttons/Surrender_Button_Disabled.png", "", 0, mButtonsSpriteManager, mButtonsSpriteAtlasDataHandler, surrenderSelected);
		mSurrenderButton.addParent(mDealButtonsHolder);
		mSurrenderButton.setPosition(new Vector3(8.0f,buttonsYPosition,5));
		mSurrenderButton.setEnable(false);
		
		mDoubleButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/Double_Button.png", "Buttons/Double_Button_Pressed.png", "Buttons/Double_Button_Disabled.png", "", 0, mButtonsSpriteManager, mButtonsSpriteAtlasDataHandler, doubleSelected);
		mDoubleButton.addParent(mDealButtonsHolder);
		mDoubleButton.setPosition(new Vector3(0.0f,buttonsYPosition,5));
		mDoubleButton.setEnable(false);
		
		mSplitButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/Split_Button.png", "Buttons/Split_Button_Pressed.png", "Buttons/Split_Button_Disabled.png", "", 0, mButtonsSpriteManager, mButtonsSpriteAtlasDataHandler, splitSelected);
		mSplitButton.addParent(mDealButtonsHolder);
		mSplitButton.setPosition(new Vector3(-8.0f,buttonsYPosition,5));	
		mSplitButton.setEnable(false);	
		
		mBetInsuranceHolder = CommonData.createEmptyGameObject("BJ_BetInsuranceHolder", gameObject, new Vector3(0,-32,5));
		mBetInsuranceHolder.layer = gameObject.layer;
		
		betInsuranceBg.transform.parent = mBetInsuranceHolder.transform;
		betInsuranceBg.transform.position = mBetInsuranceHolder.transform.position + new Vector3(0,16.5f,5);
		betInsuranceBg.SetActive(true);
		
		mYesButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/Yes_Button.png", "Buttons/Yes_Button_Pressed.png", "Buttons/Yes_Button_Disabled.png", "", 0, mButtonsSpriteManager, mButtonsSpriteAtlasDataHandler, yesSelected);
		mYesButton.addParent(mBetInsuranceHolder);
		mYesButton.setPosition(new Vector3(-3.5f,buttonsYPosition,5));	
		mYesButton.setEnable(false);	
		
		mNoButton = MenuButtonManager.sharedManager().createMenuItem("Buttons/No_Button.png", "Buttons/No_Button_Pressed.png", "Buttons/No_Button_Disabled.png", "", 0, mButtonsSpriteManager, mButtonsSpriteAtlasDataHandler, noSelected);
		mNoButton.addParent(mBetInsuranceHolder);
		mNoButton.setPosition(new Vector3(3.5f,buttonsYPosition,5));	
		mNoButton.setEnable(false);	
		
		changeToBetting();
	}
	
	// Top Bar For Max Limit
	public void showTopBarMax()
	{
		if(mIsTopBarActive) return;		
		// Top Bar Movement		
		moveTopBarTo(new Vector2(0, transform.position.y +   topBarYposShown), 0.5f, 3.0f, EaseType.EaseOut, null, mTopBarHolder_max);
	}
	
	// Top Bar For Min Limit
	public void showTopBarMin()
	{
		if(mIsTopBarActive) return;		
		// Top Bar Movement		
		moveTopBarTo(new Vector2(0, transform.position.y +  topBarYposShown), 0.5f, 3.0f, EaseType.EaseOut, null, mTopBarHolder_min);
	}
	
	void moveTopBarTo(Vector2 position, float duration, float delayDuration, EaseType easeActionType, CCallFunc.CallBack callBack, GameObject topBar)
	{
		CMoveTo move = topBar.GetComponent<CMoveTo>();
		CEaseExponential ease = topBar.GetComponent<CEaseExponential>();
		CCallFunc call = topBar.GetComponent<CCallFunc>();
		CSequence seq = topBar.GetComponent<CSequence>();
		CDelayTime delay = topBar.GetComponent<CDelayTime>();
		
		move.actionWith(topBar, position, duration);
		ease.actionWithAction(move, easeActionType);
		delay.actionWithDuration(delayDuration);
		call.actionWithCallBack(callBack);
		seq.actionWithActions(ease, delay, call);
		
		if(callBack != null)
		seq.runAction();
		else
		ease.runAction();
	}
	
	void removeTopBarMax()
	{
		moveTopBarTo(new Vector2(0, transform.position.y + topBarYPosHidden), 0.5f, 0.0f, EaseType.EaseIn, topBarRemoved, mTopBarHolder_max);
	}
	void removeTopBarMin()
	{
		moveTopBarTo(new Vector2(0,transform.position.y +  topBarYPosHidden), 0.5f, 0.0f, EaseType.EaseIn, topBarRemoved, mTopBarHolder_min);
	}
	
	void topBarRemoved()
	{
		mIsTopBarActive = false;
	}
	
	public void toggleClaimBonusDisplay()
	{
		if(mBonusHandler.isBonusActive)
		{
			bonusTimerHolder.transform.position += new Vector3(0,1.7f,0);
			bonusButton.transform.position -= new Vector3(0,1.7f,0);
			bonusButton.setTouchEnable(true);
		}
		else
		{			
			bonusTimerHolder.transform.position -= new Vector3(0,1.7f,0);
			bonusButton.transform.position += new Vector3(0,1.7f,0);
			bonusButton.setTouchEnable(false);
		}
	}
	
	// Disabling And Enabling Buttons for various states		
	public void disableAllButtonsTouch()
	{
		Debug.Log("ButtonDisableTouch");
		mDealButton.setTouchEnable(false);
		mClearButton.setTouchEnable(false);
		mRebetButton.setTouchEnable(false);
		mStandButton.setTouchEnable(false);
		mDoubleButton.setTouchEnable(false);
		mHitButton.setTouchEnable(false);
		mSplitButton.setTouchEnable(false);
		mSurrenderButton.setTouchEnable(false);
		mNoButton.setTouchEnable(false);
		mYesButton.setTouchEnable(false);
		
	}		
	public void enableAllButtonsTouch()
	{
		Debug.Log("ButtonEnableTouch");
		Debug.Log(BJ_GamePlayLayer.mInstance.mCurrentGameState.ToString());
		if(BJ_GamePlayLayer.mInstance.mCurrentGameState == BJ_GamePlayStates.BJ_WaitForInsurance)
		{
			mNoButton.setEnable(true);
			mYesButton.setEnable(true);
			mNoButton.setTouchEnable(true);
			mYesButton.setTouchEnable(true);
			return;
		}
		mDealButton.setTouchEnable(true);
		mClearButton.setTouchEnable(true);
		mRebetButton.setTouchEnable(true);
		mStandButton.setTouchEnable(true);
		mDoubleButton.setTouchEnable(true);
		mHitButton.setTouchEnable(true);
		mSplitButton.setTouchEnable(true);
		mSurrenderButton.setTouchEnable(true);
	}
	public void disableAllButtons()
	{
		Debug.Log("ButtonDisable");
		mDealButton.setEnable(false);
		mClearButton.setEnable(false);
		mRebetButton.setEnable(false);
		mStandButton.setEnable(false);
		mDoubleButton.setEnable(false);
		mHitButton.setEnable(false);
		mSplitButton.setTouchEnable(false);
		mSurrenderButton.setEnable(false);
		mNoButton.setEnable(false);
		mYesButton.setEnable(false);
		mRebetButton.setVisible(false);
	}
	
	public void enableAllButtons()
	{		
		switch(BJ_GamePlayLayer.mInstance.mCurrentGameState)
		{
		case BJ_GamePlayStates.BJ_WaitForInsurance:
		Debug.Log("ButtonEnable : "+BJ_GamePlayLayer.mInstance.mCurrentGameState);
			mYesButton.setEnable(true);
			mNoButton.setEnable(true);
			break;
		case BJ_GamePlayStates.BJ_PlayersTurn:
		Debug.Log("ButtonEnable : "+BJ_GamePlayLayer.mInstance.mCurrentGameState);
			mHitButton.setEnable(true);
			mStandButton.setEnable(true);
			mDoubleButton.setEnable(true);
			mSurrenderButton.setEnable(true);
			mSplitButton.setTouchEnable(true);
			break;
		case BJ_GamePlayStates.BJ_Betting:
		Debug.Log("ButtonEnable : "+BJ_GamePlayLayer.mInstance.mCurrentGameState);
			if(BJ_GamePlayLayer.mInstance.isPrevBetAvailable)
			{
				mDealButton.setVisible(false);
				mRebetButton.setEnable(true);
				mRebetButton.setVisible(true);
			}
			break;
		case BJ_GamePlayStates.BJ_StackingCards:
		case BJ_GamePlayStates.BJ_Shuffling:
		case BJ_GamePlayStates.BJ_PayOut:
		case BJ_GamePlayStates.BJ_InsurancePayout:
		case BJ_GamePlayStates.BJ_DealersTurn:
		case BJ_GamePlayStates.BJ_Dealing:
		default:
			break;
		}	
//			mDealButton.setEnable(true);
//			mClearButton.setEnable(true);
//			mRebetButton.setEnable(true);
//			mChipButton.setEnable(true);
//			mStandButton.setEnable(true);
//			mDoubleButton.setEnable(true);
//			mHitButton.setEnable(true);
//			mSurrenderButton.setEnable(true);
	}	
	
	public void enableButtonsToDeal()
	{
		mRebetButton.setEnable(false);
		//mDealButton.setEnable(true);
		mClearButton.setEnable(true);
		mRebetButton.setVisible(false);
		mDealButton.setVisible(true);
	}
	
	public void enableButtonsToPlayerHit()
	{
		Debug.Log("ButtonEnableHit : "+BJ_GamePlayLayer.mInstance.mCurrentGameState);
		mStandButton.setEnable(true);
		mHitButton.setEnable(true);
	}	
	
	public void enableSplit()
	{
		mSplitButton.setEnable(true);
	}
	
	public void disableSplit()
	{
		mSplitButton.setEnable(false);
	}
	
	public void disableButtonsForPopUp()
	{
		bonusButton.setTouchEnable(false);
		mAddChipsButton.setTouchEnable(false);
	 	if(mAdsRemoveButton != null)
			mAdsRemoveButton.setTouchEnable(false);
	}
	
	public void enableButtonsAfterPopUp()
	{
		bonusButton.setTouchEnable(true);
		mAddChipsButton.setTouchEnable(true);
		if(mAdsRemoveButton != null) 
			mAdsRemoveButton.setTouchEnable(true);
	}
	
	// Changing Buttons for the States
	void changeToDealing()
	{
		hideChips();
		disableAllButtonsTouch();
		mClearButton.setEnable(false);
		moveHolderDown(mBetButtonsHolder);
		moveHolderUp(mDealButtonsHolder);
	}
	
	public void changeToBetting()
	{
		showChips();
		enableAllButtons();
		disableAllButtonsTouch();		
		moveHolderDown(mDealButtonsHolder);
		moveHolderUp(mBetButtonsHolder);		
	}	
	
	public void showBetInsurance()
	{
		disableAllButtonsTouch();
		moveHolderDown(mDealButtonsHolder);
		moveHolderUp(mBetInsuranceHolder);	
	}
	
	public void removeBetInsurance()
	{
		disableAllButtonsTouch();
		moveHolderDown(mBetInsuranceHolder);		
		moveHolderUp(mDealButtonsHolder);
	}
	
	void addMoveActionTo(GameObject holderObject, Vector2 moveLocation, float duration, bool isEnableButtons)
	{
		if(holderObject.GetComponent<CMoveTo>() != null)
			DestroyImmediate(holderObject.GetComponent<CMoveTo>());
		if(holderObject.GetComponent<CCallFunc>() != null)
			DestroyImmediate(holderObject.GetComponent<CCallFunc>());
		if(holderObject.GetComponent<CSequence>() != null)
			DestroyImmediate(holderObject.GetComponent<CSequence>());		
		
		CMoveTo move = holderObject.AddComponent<CMoveTo>();
		CCallFunc call = holderObject.AddComponent<CCallFunc>();
		CSequence seq = holderObject.AddComponent<CSequence>();
		
		move.actionWith(holderObject, moveLocation, duration);
		call.actionWithCallBack(enableAllButtonsTouch);
		if(isEnableButtons)
		seq.actionWithActions(move, call);
		else
		seq.actionWithActions(move);
		seq.runAction();
	}
	
	void moveHolderUp(GameObject holderObject)
	{
		addMoveActionTo(holderObject, transform.position + new Vector3(0,0,holderObject.transform.position.z), holderMoveSpeed, true);		
	}
	
	void moveHolderDown(GameObject holderObject)
	{
		addMoveActionTo(holderObject, transform.position + new Vector3(0,-32.0f,holderObject.transform.position.z), holderMoveSpeed, false);
	}
	
		
	// Button Selected	
	void dealSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("DealPressed");
		if(BJ_GamePlayLayer.mInstance.dealCards())
		{	
			changeToDealing();
			updateChipsBalance(-Mathf.CeilToInt(BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().totalBet));
			StatsHandler.HandsPlayed++;
			StatsHandler.TotalBets += Mathf.CeilToInt(BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().totalBet);
			StatsHandler.MaxBet = Mathf.CeilToInt(BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().totalBet);
			setTotalWin(0.0f);
			BlackjackFlurryHandler.sendHandPlayedFlurryData();
		}
	}
	
	void hitSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		disableSplit();
		if(BJ_GamePlayLayer.mInstance.mCurrentGameState != BJ_GamePlayStates.BJ_PlayersTurn) return;
		BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().dealNewCard();
		disableAllButtons();
	}
	
	void standSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		disableSplit();
		BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().makeNextPlayerToDeal();
	}
	
	void clearSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ClearSelected");
		disableAllButtons();
		removeTopBarMax();
		BJ_GamePlayLayer.mInstance.clearChipsFromTable();
		showChips();
		enableAllButtons();
	}
	
	void claimBonusSelected()
	{		
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		mBonusHandler.claimBonus();
	}
	
	void addChipsSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		PopUpManager.mInstance.showPopUp(PopUpManager.PopUpType.MoreChips, popUpRemovedCallBack);
		if(MainMenuLayer.mInstance != null)
		{
//			mSoundsButton.setTouchEnable(false);
//			mMusicsButton.setTouchEnable(false);
			MainMenuLayer.mInstance.disableMainMenu();
		}
		else
		{
			mDealButton.setTouchEnable(false);
			mClearButton.setTouchEnable(false);
			mRebetButton.setTouchEnable(false);
			mStandButton.setTouchEnable(false);
			mDoubleButton.setTouchEnable(false);
			mHitButton.setTouchEnable(false);
			mSplitButton.setTouchEnable(false);
			mSurrenderButton.setTouchEnable(false);
			mNoButton.setTouchEnable(false);
			mYesButton.setTouchEnable(false);
			mBackButton.setTouchEnable(false);
		}		
		bonusButton.setTouchEnable(false);
		mAddChipsButton.setTouchEnable(false);		
		disableAllChipsButtons();
	}
	
	void popUpRemovedCallBack()
	{
		if(MainMenuLayer.mInstance != null)
		{
//			mSoundsButton.setTouchEnable(true);
//			mMusicsButton.setTouchEnable(true);
			MainMenuLayer.mInstance.enableMainMenu();		
		}
		else
		{
			mDealButton.setTouchEnable(true);
			mClearButton.setTouchEnable(true);
			mRebetButton.setTouchEnable(true);
			mStandButton.setTouchEnable(true);
			mDoubleButton.setTouchEnable(true);
			mHitButton.setTouchEnable(true);
			mSplitButton.setTouchEnable(true);
			mSurrenderButton.setTouchEnable(true);
			mNoButton.setTouchEnable(true);
			mYesButton.setTouchEnable(true);
			mBackButton.setTouchEnable(true);
		}
		if(mBonusHandler.isBonusActive) bonusButton.setTouchEnable(true);
		mAddChipsButton.setTouchEnable(true);
		if(mIsChipsVisible) enableAllChipsButtons();
	}
	
	void backSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		BJ_GamePlayLayer.mInstance.mAlphaLayer.fadeInAlphaLayer(0.2f, 1.0f);		
		CCallFunc.createCallAfterDelay(loadMainMenu, 0.3f).runAction();
	}
	
	void restorePurchaseSelected()
	{
	}
	
	void removeAdsSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.InAppNonConsumable,"1",InAppPurchase);	
	}
	
	void loadMainMenu()
	{
		Application.LoadLevel(1);
	}
	
	void rebetSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");		
		if(checkIsBalanceAvailable(Mathf.CeilToInt(BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().getPrevBetAmount())))
		{		
			mDealButton.setEnable(true);
			enableButtonsToDeal();
			BJ_GamePlayLayer.mInstance.placePreviousBet();
		}
	}
	
	void doubleSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");		
		if(checkIsBalanceAvailable(Mathf.CeilToInt(BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().totalBet)))
		{		
			disableAllButtons();
			disableSplit();
			BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().doubleTheBetAndDeal();
		}
	}
	
	void splitSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		if(checkIsBalanceAvailable(Mathf.CeilToInt(BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().totalBet)))
		{
			disableAllButtons();
			disableSplit();
			BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().splitTheCards();
		}
	}
		
	bool checkIsBalanceAvailable(int chipValue)
	{
		if(chipValue <= PlayerChipsBalance) return true;
		
		PopUpManager.mInstance.showPopUp(PopUpManager.PopUpType.InsufficientFunds, popUpRemovedCallBack);
		
		mDealButton.setTouchEnable(false);
		mClearButton.setTouchEnable(false);
		mRebetButton.setTouchEnable(false);
		mStandButton.setTouchEnable(false);
		mDoubleButton.setTouchEnable(false);
		mHitButton.setTouchEnable(false);
		mSplitButton.setTouchEnable(false);
		mSurrenderButton.setTouchEnable(false);
		mNoButton.setTouchEnable(false);
		mYesButton.setTouchEnable(false);
		mBackButton.setTouchEnable(false);
		
		bonusButton.setTouchEnable(false);
		mAddChipsButton.setTouchEnable(false);		
		disableAllChipsButtons();
		
		return false;
	}
	
	void surrenderSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		disableAllButtons();
		disableSplit();
		BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().surrenderTheHand();		
	}
	
	void chipsSelected(MenuButton sender)
	{
		PlayAudioSounds.sharedHandler().playSound("ChipPlaced");
		int index = sender.getTag();
		setChipsMarkerTo(index);
		
		if(checkIsBalanceAvailable(Mathf.CeilToInt(BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().totalBet + mArrChipValues[index])))
		{				
			BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().placeBetsSelected();
			float totalBet = BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().totalBet;
			if(totalBet >= TableHandler.minBet && (totalBet <= TableHandler.maxBet || TableHandler.maxBet == -1))
			{
				removeTopBarMin();
				mDealButton.setEnable(true);
			}
			else
			{
				if(totalBet > TableHandler.maxBet && TableHandler.maxBet != -1) showTopBarMax();
				else if(totalBet < TableHandler.minBet) showTopBarMin();
				mDealButton.setEnable(false);
			}
		}
	}
	
	void yesSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		
		if(checkIsBalanceAvailable(Mathf.CeilToInt(BJ_GamePlayLayer.mInstance.player.totalBet/2.0f)))
		{
			disableAllButtons();
			BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().insureThePlayer();
			CCallFunc.createCallAfterDelay(BJ_GamePlayLayer.mInstance.askForInsuranceDone, 1.0f).runAction();
		}
	}
	
	void noSelected()
	{
		disableAllButtons();
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		CCallFunc.createCallAfterDelay(BJ_GamePlayLayer.mInstance.askForInsuranceDone, 1.0f).runAction();
	}
	
	void soundsSelected()
	{		
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		PlayerPrefs.SetInt("IsSoundEnable", PlayerPrefs.GetInt("IsSoundEnable")==0?1:0);
	}
	
	void musicSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		PlayerPrefs.SetInt("IsMusicEnable", PlayerPrefs.GetInt("IsMusicEnable")==0?1:0);
		PlayAudioSounds.sharedHandler().toggleBgMusic();
	}
	
	// Chips Marker
	void setChipsMarkerTo(int index)
	{
//		mSelectedChipMarker.transform.position = mArrChipsButtons[index-1].transform.position;
		mCurrentSelectedChipIndex = index;
	}
	
	void disableAllChipsButtons()
	{
		if(mArrChipsButtons == null) return;
		foreach(MenuButton chipButton in mArrChipsButtons)
			chipButton.setTouchEnable(false);
	}
	
	void enableAllChipsButtons()
	{
		if(mArrChipsButtons == null) return;
		foreach(MenuButton chipButton in mArrChipsButtons)
			chipButton.setTouchEnable(true);
		mIsChipsVisible = true;
	}
	
	public void showChips()
	{
		if(mChipsBarHolder == null || mIsChipsVisible) return;
		setChipsMarkerTo(mCurrentSelectedChipIndex);
		if(mChipsBarHolder.GetComponent<CMoveTo>() != null)
			DestroyImmediate(mChipsBarHolder.GetComponent<CMoveTo>());
		if(mChipsBarHolder.GetComponent<CCallFunc>() != null)
			DestroyImmediate(mChipsBarHolder.GetComponent<CCallFunc>());
		if(mChipsBarHolder.GetComponent<CSequence>() != null)
			DestroyImmediate(mChipsBarHolder.GetComponent<CSequence>());		
		
		CMoveTo move = mChipsBarHolder.AddComponent<CMoveTo>();
		CCallFunc call = mChipsBarHolder.AddComponent<CCallFunc>();
		CSequence seq = mChipsBarHolder.AddComponent<CSequence>();
		
		move.actionWith(mChipsBarHolder, transform.position + new Vector3(0,3.5f,mChipsBarHolder.transform.position.z), chipsMoveSpeed);
		call.actionWithCallBack(enableAllChipsButtons);
		seq.actionWithActions(move, call);
		seq.runAction();
	}
	
	public void hideChips()
	{
		if(mChipsBarHolder == null || !mIsChipsVisible) return;
		mIsChipsVisible = false;
		if(mChipsBarHolder.GetComponent<CMoveTo>() != null)
			DestroyImmediate(mChipsBarHolder.GetComponent<CMoveTo>());
		if(mChipsBarHolder.GetComponent<CCallFunc>() != null)
			DestroyImmediate(mChipsBarHolder.GetComponent<CCallFunc>());
		if(mChipsBarHolder.GetComponent<CSequence>() != null)
			DestroyImmediate(mChipsBarHolder.GetComponent<CSequence>());
		
		CMoveTo move = mChipsBarHolder.AddComponent<CMoveTo>();
		CCallFunc call = mChipsBarHolder.AddComponent<CCallFunc>();
		CSequence seq = mChipsBarHolder.AddComponent<CSequence>();
		
		move.actionWith(mChipsBarHolder, transform.position + new Vector3(0,-0.5f,mChipsBarHolder.transform.position.z), chipsMoveSpeed);  // 0.05f
		call.actionWithCallBack(disableAllChipsButtons);
		seq.actionWithActions(move, call);
		seq.runAction();
	}
	
	// Getters
	public float getCurrentSelectedChipValue()
	{
		return mArrChipValues[mCurrentSelectedChipIndex];
	}
	public int getCurrentSelectedChipIndex()
	{
		return mCurrentSelectedChipIndex;
	}

//	// History Of Dealer
//	public void setDealerCardValueToHistory(int cardCount)
//	{
//		Debug.Log("DealerHistoryUpdated");
//		mArrHistoryOfDealer.Insert(0, cardCount);
//		if(mArrHistoryOfDealer.Count > 9)
//			mArrHistoryOfDealer.RemoveAt(9);
//		adjustHistoryDisplay();
//	}
//	
//	void adjustHistoryDisplay()
//	{
//		for(int i=0; i< mArrHistoryOfDealer.Count; i++)
//			mDealerHistoryLabel[i].setString(((int)mArrHistoryOfDealer[i]).ToString());
//	}
	
	public void setTotalBet(float totalBet)
	{		
		mBetPlacedLabel.setString((Mathf.FloorToInt(totalBet)).ToString());
	}
	
	public void setTotalWin(float totalWin)
	{
//		mTotalWinPlacedLabel.setString(totalWin.ToString());
	}
	
	public void setTimer(int hours, int minutes, int seconds)
	{
		string hours_ = (hours < 10 ? "0" : "")+hours;
		string minutes_ = (minutes < 10 ? "0" : "")+minutes;
		string seconds_ = (seconds < 10 ? "0" : "")+seconds;
		timerLabel.setString(hours_+"/"+minutes_+"/"+seconds_);
	}
	
	public void updateChipsBalance(int chips)
	{
		PlayerChipsBalance += chips;		
		PopUpManager.mInstance.updateChipsBalance(PlayerChipsBalance);
		
		balanceUpdateRate = Mathf.FloorToInt((PlayerChipsBalance - currentChipsBalance) * 0.05f);
		if(balanceUpdateRate == 0) balanceUpdateRate = PlayerChipsBalance > currentChipsBalance ? 1 : -1;
		
		if(PlayerChipsBalance > currentChipsBalance)
			changeChipsBalanceColorTo(Color.green);
		else
			changeChipsBalanceColorTo(Color.red);
	}
	
	private void updateChipsDisplay()
	{
		if(currentChipsBalance == PlayerChipsBalance) return;
		currentChipsBalance += balanceUpdateRate;// PlayerChipsBalance > currentChipsBalance ? 1 : -1;
		if(Mathf.Abs(currentChipsBalance - PlayerChipsBalance) < Mathf.Abs(balanceUpdateRate)) 
		{
			currentChipsBalance = PlayerChipsBalance;
			changeChipsBalanceColorTo(Color.white);
//			OnlineInterfaceHandler.Instance.SendRequest(eONLINE_REQ_TYPE.UPDATE_VALUE_TO_SERVER,currentChipsBalance.ToString(),null,"PlayerChipBalance");
		}
		mChipsBalanceLabel.setString(currentChipsBalance.ToString());		
	}				
	
	private void changeChipsBalanceColorTo(Color chipsColor)
	{
		if(mChipsBalanceLabel.GetComponent<CChangeColorLabelAtlas>() != null)
			DestroyImmediate(mChipsBalanceLabel.GetComponent<CChangeColorLabelAtlas>());
		
		CChangeColorLabelAtlas colorChange = mChipsBalanceLabel.gameObject.AddComponent<CChangeColorLabelAtlas>();
		colorChange.actionWith(mChipsBalanceLabel, chipsColor, 0.02f);
		colorChange.runAction();
	}
	
	
	// DG Chips Animation
	public void startDgChipsAnimation()
	{
		int chipCount = Random.Range(5,10);
		for(int i=0; i< chipCount; i++)
			createNewDgChipAt(new Vector3(0, -transform.position.y ,0), i*0.1f);
	}	
	
	void createNewDgChipAt(Vector3 position, float delayDuration)
	{
		GameObject dgChip = CommonData.createGameObject("DG_Chip", gameObject , position, mSpriteManager, mSpriteAtlasDataHandler, "DG_Chip/DG_Chip_1.png", 10);
		dgChip.layer = gameObject.layer;
		
		CommonData.addIdealAnimationTo(dgChip, "DG_Chip/DG_Chip_", 6, Random.Range(10,21));
		
		CDelayTime delay = dgChip.AddComponent<CDelayTime>();
		//CMoveTo move = dgChip.AddComponent<CMoveTo>();
		CBezierTo move = dgChip.AddComponent<CBezierTo>();
		CEaseExponential ease = dgChip.AddComponent<CEaseExponential>();
		CCallFunc call = dgChip.AddComponent<CCallFunc>();
		CSequence seq = dgChip.AddComponent<CSequence>();
		
		delay.actionWithDuration(Mathf.Max(0.01f, delayDuration));
		//move.actionWith(dgChip, mDGChip.transform.position, 0.5f);
		move.actionWith(dgChip, mDGChip.transform.position, position+new Vector3(-4.0f,2.0f,0), mDGChip.transform.position+new Vector3(0,3.0f,0), 0.5f);
		ease.actionWithAction(move, EaseType.EaseOut);
		call.actionWithCallBack(destroyDgChip);
		seq.actionWithActions(delay, ease, call);
		
		seq.runAction();
	}
	
	void destroyDgChip(GameObject sender)
	{
		CommonData.removeAllActionsFrom(mDGChip);
		
		CScaleTo scaleUp = mDGChip.AddComponent<CScaleTo>();
		CScaleTo scaleDown = mDGChip.AddComponent<CScaleTo>();
		CSequence seq = mDGChip.AddComponent<CSequence>();
		
		scaleUp.actionWith(mDGChip, new Vector3(1.5f,1.5f,1.5f), 0.05f);
		scaleDown.actionWith(mDGChip, new Vector3(1.0f,1.0f,1.0f), 0.05f);
		seq.actionWithActions(scaleUp, scaleDown);
		
		PlayAudioSounds.sharedHandler().playSound("ChipCollected");
		
		seq.runAction();
		
		CommonData.destroyMyObject(sender);
	}
	
	void removeAds()
	{
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Hide_Banner_Ads, "", null);
		Loading.IsAdsRemoved = true;
		MenuButtonManager.sharedManager().removeChild(mAdsRemoveButton);
	}
	
	void InAppPurchase(eEXTERNAL_REQ_TYPE reqType , string requestedId , string receivedStatus)
	{
#if UNITY_STANDALONE_OSX
			// enable current Screen button
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StopLoadingScreen,"",null);
#endif
		Debug.Log("InAppPurchase().....Data Received..."+receivedStatus);
		
		switch(reqType)
		{
		case eEXTERNAL_REQ_TYPE.InAppNonConsumable:
			if(receivedStatus == "true")
			{
				switch(requestedId)
				{
					case "1":
					Debug.Log("InAppNonConsumable: Request 1 completed");
					removeAds();
					break;
					case "2":
					Debug.Log("InAppNonConsumable: Request 1 completed");
					break;
					case "3":
					break;
				}
			}
			else
			{
				// purchase failed.
			}
			break;
		default:
			Debug.Log("InAppPurchase()....Invalid received Data");
			break;
			
		}
		
	}
}
