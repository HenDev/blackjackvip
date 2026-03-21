using UnityEngine;
using System.Collections;

public enum SpinWheelElementType {Chips=0, FreeSpin};	

[System.Serializable]
public class SpinWheelElements
{
	public SpinWheelElementType mSpinWheelElementType;
	public int SpinWheelElementValue;	
	public TextMesh SpinWheelCountText;
	
	public SpinWheelElements()
	{
		mSpinWheelElementType = SpinWheelElementType.Chips;
		SpinWheelElementValue = 0;
	}
}

public class DailyBonusHandler : MonoBehaviour {
		
	public static DailyBonusHandler mInstance = null;
	
	public LinkedSpriteManager mSpriteManager = null;
	public SpriteAtlasDataHandler mSpriteAtlasDataHandler = null;
	
	private int mMaxNoOfReturnBonusDays = 30;
//	private int mMaxNoOfFriendsBonus = 25;
	
	public int ReturnBonusDays
	{
		get { return PlayerPrefs.GetInt("NoOfContiniousDays"); }
		set { PlayerPrefs.SetInt("NoOfContiniousDays", Mathf.Clamp(value,0,mMaxNoOfReturnBonusDays)); }
	}
	
	public AlphaLayerHandler alphaLayer = null;
	public GameObject SpinWheelPrefab = null;
	public GameObject mDailyBonusBG = null;
	public TextMesh mReturnBonusText = null;
//	public TextMesh mFriendsBonusText = null;
	public TextMesh mTotalBonusText = null;
	
	public SpinWheelElements[] mArrSpinWheelElements = null;
	int[] mArrItemArrangement = null;
	
	public float ShowAndRemoveDuration = 0.5f;
	
	private MenuButton mClaimBonusButton = null;
	private MenuButton mSpinButton = null;
	private GameObject[] mArrSpinWheelItems = null;
	private GameObject mSelectedItem = null;
	private GameObject mSpinWheelObject = null;	
	private bool isDailyBonusDone = false;   // Temp
	private int remainingSpins = 1;
	private int spinRewards = 0;
	
	// Use this for initialization
	void Start () { mInstance = this; alphaLayer.fadeOutAlphaLayer(0.01f); }	
	// Update is called once per frame
	void Update () { }
	
	void Awake()
	{
		 StartCoroutine(CheckForDailyBonus());
	}
	
	IEnumerator CheckForDailyBonus()
	{
		while(true)
		{
			yield return new WaitForSeconds(5.0f);
			if(PopUpManager.mInstance.isPopUpActive) continue;
			
			int currentDay = System.DateTime.Now.Day;
			int currentMonth = System.DateTime.Now.Month;
			int currentYear = System.DateTime.Now.Year;
			
			int storedDay = PlayerPrefs.GetInt("DailyBonusDay");
			int storedMonth = PlayerPrefs.GetInt("DailyBonusMonth");
			int storedYear = PlayerPrefs.GetInt("DailyBonusYear");
						
//			createAndShowDailyBonus();
			if(currentDay != storedDay || currentMonth != storedMonth || currentYear != storedYear)
			{
				Debug.Log("DailyBonus");
				
				int daysPassed = System.DateTime.Now.Subtract(new System.DateTime(storedYear, storedMonth, storedDay)).Days;
				if(daysPassed == 1)
					ReturnBonusDays++;
				else
					ReturnBonusDays = 0;
					
				createAndShowDailyBonus();	
				BlackjackFlurryHandler.sendDailyBonusCreatedFlurryData();
				PlayerPrefs.SetInt("DailyBonusDay", currentDay);
				PlayerPrefs.SetInt("DailyBonusMonth", currentMonth);
				PlayerPrefs.SetInt("DailyBonusYear", currentYear);			
				
				break;
			}		
		}
	}
	
	void createItemsForTheSections()
	{
		if(mArrSpinWheelElements == null) return;
		if(mArrSpinWheelItems != null) return;
		
		mArrItemArrangement = CommonData.generateRandomNumbers(0, mArrSpinWheelElements.Length-1);
		
		float AngleGap = 360.0f/mArrSpinWheelElements.Length;
		float AngleOffset = mSpinWheelObject.GetComponent<SpinWheelHandler>().AngleOffsetForFirstSector;
		
		mArrSpinWheelItems = new GameObject[mArrSpinWheelElements.Length];		
		for(int i=0; i<mArrSpinWheelElements.Length; i++)
		{
			float currentAngle = (AngleGap*(i+0.5f) + AngleOffset) * Mathf.Deg2Rad;
			float distance = 5.3f;
			float xPos = distance * Mathf.Sin(currentAngle);
			float yPos = distance * Mathf.Cos(currentAngle);			
			string itemName = mArrSpinWheelElements[mArrItemArrangement[i]].mSpinWheelElementType == SpinWheelElementType.Chips ? "Chip" : "FreeSpin";
			GameObject spinWheelItem = CommonData.createGameObject("SpinWheelItem_"+(i+1), mSpinWheelObject.GetComponent<SpinWheelHandler>().mSpinWheelGameObject, new Vector3(xPos,yPos,0), mSpriteManager, mSpriteAtlasDataHandler, itemName+".png", 0);
			spinWheelItem.layer = gameObject.layer;
			spinWheelItem.transform.Rotate(Vector3.forward*-currentAngle*Mathf.Rad2Deg);						
			mArrSpinWheelItems[i] = spinWheelItem;			
							
			if(mArrSpinWheelElements[mArrItemArrangement[i]].mSpinWheelElementType == SpinWheelElementType.Chips)
			{
				TextMesh sectorText = mArrSpinWheelElements[mArrItemArrangement[i]].SpinWheelCountText;
				sectorText.transform.rotation = Quaternion.identity;
				sectorText.transform.parent = mSpinWheelObject.GetComponent<SpinWheelHandler>().mSpinWheelGameObject.transform;
				sectorText.transform.Rotate(Vector3.forward*-currentAngle*Mathf.Rad2Deg);
				sectorText.transform.position = sectorText.transform.parent.position + new Vector3(xPos, yPos, -5);
				sectorText.text = mArrSpinWheelElements[mArrItemArrangement[i]].SpinWheelElementValue.ToString();
			}
		}
	}
		
	void createAndShowDailyBonus()
	{		
		if(mClaimBonusButton != null || isDailyBonusDone) return;
		Debug.Log("CreateDailyBonus");
		isDailyBonusDone = true;
		remainingSpins = 1;
		spinRewards = 0;
		
		mReturnBonusText.text = "0";
//		mFriendsBonusText.text = "0";
		mTotalBonusText.text = "0";
		
		mClaimBonusButton = MenuButtonManager.sharedManager().createMenuItem("Button_ClaimBonus.png", "Button_ClaimBonus_Pressed.png", "", "", 0, mSpriteManager, mSpriteAtlasDataHandler, claimBonusSelected);
		mClaimBonusButton.addParent(gameObject);
		mClaimBonusButton.setPosition(new Vector3(-20.0f,-15.0f,0));
		mClaimBonusButton.gameObject.layer = gameObject.layer;
		mClaimBonusButton.setTouchEnable(false);
				
		createSpinWheel();
		showDailyBonus();
		Debug.Log("DailyBonusCreated");
	}
	
	void createSpinWheel()
	{
		mSpinWheelObject = (GameObject)Instantiate(SpinWheelPrefab);//, new Vector3(0,1.0f,5), Quaternion.identity);
		mSpinWheelObject.layer = gameObject.layer;
		mSpinWheelObject.transform.parent = mDailyBonusBG.transform;
		mSpinWheelObject.transform.position = mDailyBonusBG.transform.position + new Vector3(0, 3.3f, -1);
		
		mSpinButton = MenuButtonManager.sharedManager().createMenuItem("Button_Spin.png", "Button_Spin_Pressed.png", "Button_Spin_Disabled.png", "", 0, mSpriteManager, mSpriteAtlasDataHandler, spinSelected);
		mSpinButton.addParent(mSpinWheelObject);
		mSpinButton.setPosition(new Vector3(0.0f,0.0f,0));
		mSpinButton.gameObject.layer = gameObject.layer;
		mSpinButton.setTouchEnable(false);
		
		animateSpinButton();		
		createItemsForTheSections();
	}
	
	void animateSpinButton()
	{
		CommonData.removeAllActionsFrom(mSpinButton.gameObject);
		
		CScaleTo scaleUp = mSpinButton.gameObject.AddComponent<CScaleTo>();
		CScaleTo scaleDown = mSpinButton.gameObject.AddComponent<CScaleTo>();
		CEaseElastic easeDown = mSpinButton.gameObject.AddComponent<CEaseElastic>();
		CDelayTime delay = mSpinButton.gameObject.AddComponent<CDelayTime>();
		CCallFunc call = mSpinButton.gameObject.AddComponent<CCallFunc>();
		CSequence seq = mSpinButton.gameObject.AddComponent<CSequence>();
		CRepeat repeat = mSpinButton.gameObject.AddComponent<CRepeat>();
		
		scaleUp.actionWith(mSpinButton.gameObject, Vector2.one*1.5f, 0.1f);
		scaleDown.actionWith(mSpinButton.gameObject, Vector2.one, 0.5f);
		easeDown.actionWithAction(scaleDown, EaseType.EaseOut);
		delay.actionWithDuration(0.5f);
		call.actionWithCallBack(checkStopSpinButtonAnimation);
		seq.actionWithActions(scaleUp, easeDown, delay, call);
		repeat.actionWithAction(seq, -1);
		
		repeat.runAction();
	}	
			
	void checkStopSpinButtonAnimation()
	{
		if(!mSpinButton._isEnabled)
			CommonData.removeAllActionsFrom(mSpinButton.gameObject);
	}
	
	void showClaimBonus()
	{
		// ClaimBonus Button Animation
		CMoveTo move = mClaimBonusButton.gameObject.AddComponent<CMoveTo>();
		CEaseExponential ease = mClaimBonusButton.gameObject.AddComponent<CEaseExponential>();
		
		move.actionWith(mClaimBonusButton.gameObject, new Vector2(0.0f, mClaimBonusButton.transform.position.y), ShowAndRemoveDuration);
		ease.actionWithAction(move, EaseType.EaseOut);
		ease.runAction();
	}
	
	void showDailyBonus()
	{
		MainMenuLayer.mInstance.disableMainMenu();
		BJ_BottomBar.mInstance.disableButtonsForPopUp();
		
		alphaLayer.fadeInAlphaLayer(ShowAndRemoveDuration, 0.7f);		
		moveSpinWheelTo(0.0f, EaseType.EaseOut);
		
		CDelayTime delay = GetComponent<CDelayTime>();
		CCallFunc call = GetComponent<CCallFunc>();		
		CSequence seq = GetComponent<CSequence>();
		
		delay.actionWithDuration(ShowAndRemoveDuration);
		call.actionWithCallBack(enableCloseButton);
		seq.actionWithActions(delay, call);
		seq.runAction();		
	}
	
	void removeDailyBonus()
	{
		alphaLayer.fadeOutAlphaLayer(ShowAndRemoveDuration*1.5f);	
		moveSpinWheelTo(15.0f, EaseType.EaseIn);	
		
		CDelayTime delay = GetComponent<CDelayTime>();
		CCallFunc call = GetComponent<CCallFunc>();		
		CSequence seq = GetComponent<CSequence>();
		
		delay.actionWithDuration(ShowAndRemoveDuration);
		call.actionWithCallBack(destroyDailyBonusAndEnableGamePlay);
		seq.actionWithActions(delay, call);
		seq.runAction();
	}
	
	void moveSpinWheelTo(float yposition, EaseType easeType)
	{		
		CMoveTo move = mDailyBonusBG.GetComponent<CMoveTo>();
		CEaseExponential ease = mDailyBonusBG.GetComponent<CEaseExponential>();
		
		move.actionWith(mDailyBonusBG, new Vector2(0,yposition), ShowAndRemoveDuration);
		ease.actionWithAction(move, easeType);
		
		ease.runAction();
	}
	
	void enableCloseButton()
	{
		mClaimBonusButton.setTouchEnable(true);
		mSpinButton.setTouchEnable(true);
	}
	
	void destroyDailyBonusAndEnableGamePlay()
	{		
		destroyDailyBonus();
		
		MainMenuLayer.mInstance.enableMainMenu();
		BJ_BottomBar.mInstance.enableButtonsAfterPopUp();
	}
	
	void claimBonusSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ClaimBonus");
		BlackjackFlurryHandler.sendDailyBonusClaimedFlurryData();
		removeDailyBonus();
		mClaimBonusButton.setTouchEnable(false);
		mSpinButton.setTouchEnable(false);
		
		// Close Button Animation
		CMoveTo move = mClaimBonusButton.gameObject.GetComponent<CMoveTo>();
		CEaseExponential ease = mClaimBonusButton.gameObject.GetComponent<CEaseExponential>();
		
		move.actionWith(mClaimBonusButton.gameObject, new Vector2(20.0f, mClaimBonusButton.transform.position.y), ShowAndRemoveDuration);
		ease.actionWithAction(move, EaseType.EaseIn);
		ease.runAction();
	}
	
	void resetSpinWheel()
	{
		destroySpinWheel();
		createSpinWheel();
		mSpinButton.setEnable(true);	
		mSpinButton.setTouchEnable(true);
		animateSpinButton();
		PlayAudioSounds.sharedHandler().playSound("SpinWheelReset");
	}
	
	void selectedItemAnimationCompleted()
	{
		remainingSpins--;
		if(remainingSpins > 0) { resetSpinWheel(); return; }
		
		destroySelectedItem();
		updateReturnBonus();
	}
	
	void updateReturnBonus()
	{
		PlayAudioSounds.sharedHandler().playSound("BonusTextPopUp");
		addJumpAnimationTo(mReturnBonusText.gameObject, updateTotalBonus);
		int returnBonus = ReturnBonusDays * 100;
		spinRewards += returnBonus;
		mReturnBonusText.text = returnBonus.ToString();
	}
	
//	void updateFriendsBonus()
//	{
//		PlayAudioSounds.sharedHandler().playSound("BonusTextPopUp");
//		addJumpAnimationTo(mFriendsBonusText.gameObject, updateTotalBonus);
////		int friendsBonus = Mathf.Clamp(OnlineSocialHandler.Instance.GetNoofFriendsPlaying(), 0, mMaxNoOfFriendsBonus) * 100;
////		spinRewards += friendsBonus;
////		mFriendsBonusText.text = friendsBonus.ToString();
//	}
	
	void updateTotalBonus()
	{
		PlayAudioSounds.sharedHandler().playSound("BonusTextPopUp");
		addJumpAnimationTo(mTotalBonusText.gameObject, showClaimBonus);
		mTotalBonusText.text = spinRewards.ToString();
	}
	
	void addJumpAnimationTo(GameObject _object, CCallFunc.CallBack _completedCallBack)
	{
		CMoveBy moveUp = _object.AddComponent<CMoveBy>();
		CMoveBy moveDown = _object.AddComponent<CMoveBy>();
		CSequence seq = _object.AddComponent<CSequence>();
		CRepeat repeat = _object.AddComponent<CRepeat>();
		CCallFunc call = _object.AddComponent<CCallFunc>();
		CSequence final = _object.AddComponent<CSequence>();
		
		moveUp.actionWith(_object, new Vector2(0, 1.0f), 0.2f);
		moveDown.actionWith(_object, new Vector2(0, -1.0f), 0.2f);
		seq.actionWithActions(moveUp, moveDown);
		repeat.actionWithAction(seq, 1);
		call.actionWithCallBack(_completedCallBack);
		final.actionWithActions(repeat, call);
		
		final.runAction();
	}
	
	void spinSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		mSpinButton.setEnable(false);
		mSpinWheelObject.GetComponent<SpinWheelHandler>().makeWheelToSpin(spinWheelCallBack);
		PlayAudioSounds.sharedHandler().playSound("WheelSpin");
	}
	
	void spinWheelCallBack(int selected)
	{
		Debug.Log("Selected Item No : "+selected);
		PlayAudioSounds.sharedHandler().stopSound("WheelSpin");	
		if(mArrSpinWheelElements[mArrItemArrangement[selected-1]].mSpinWheelElementType == SpinWheelElementType.FreeSpin)
			remainingSpins += Random.Range(1,3);
		else
			spinRewards += mArrSpinWheelElements[mArrItemArrangement[selected-1]].SpinWheelElementValue;
		
		addAnimationForTheItemInTheWheel(selected);
		createAndAddAnimationForSelectedItem(selected);		
	}
	
	void addAnimationForTheItemInTheWheel(int selected)
	{
		PlayAudioSounds.sharedHandler().playSound("GiftPopUp");
		// Animation for the Item in the wheel
		GameObject ItemInTheWheel = mArrSpinWheelItems[selected-1];
		
		CScaleTo ScaleTo1 = ItemInTheWheel.AddComponent<CScaleTo>();
		CScaleTo ScaleTo2 = ItemInTheWheel.AddComponent<CScaleTo>();
		CScaleTo ScaleTo3 = ItemInTheWheel.AddComponent<CScaleTo>();
		CScaleTo ScaleTo4 = ItemInTheWheel.AddComponent<CScaleTo>();
		CDelayTime delay = ItemInTheWheel.AddComponent<CDelayTime>();
		CSequence seq = ItemInTheWheel.AddComponent<CSequence>();
		CRepeat repeat = ItemInTheWheel.AddComponent<CRepeat>();
		
		Vector2 initialScale = ItemInTheWheel.transform.localScale;
		float actionSpeed = 0.15f;
		ScaleTo1.actionWith(ItemInTheWheel, new Vector2(2.0f*initialScale.x, 1.5f*initialScale.y), actionSpeed); 
		ScaleTo2.actionWith(ItemInTheWheel, new Vector2(1.5f*initialScale.x, 2.0f*initialScale.y), actionSpeed/2); 
		ScaleTo3.actionWith(ItemInTheWheel, new Vector2(2.0f*initialScale.x, 2.0f*initialScale.y), actionSpeed/2); 
		ScaleTo4.actionWith(ItemInTheWheel, new Vector2(1.0f*initialScale.x, 1.0f*initialScale.y), actionSpeed); 
		delay.actionWithDuration(0.5f);
		seq.actionWithActions(ScaleTo1, ScaleTo2, ScaleTo3, ScaleTo4, delay);
		repeat.actionWithAction(seq, 1);
		
		repeat.runAction();
	}
	
	void createAndAddAnimationForSelectedItem(int selected)
	{		
		// Create and animate SelectedItem
		string itemName = mArrSpinWheelElements[mArrItemArrangement[selected-1]].mSpinWheelElementType == SpinWheelElementType.Chips ? "Chip" : "FreeSpin";
		mSelectedItem = CommonData.createGameObject("Selected Item", mSpinWheelObject, Vector3.up*7.0f, mSpriteManager, mSpriteAtlasDataHandler, itemName+".png", 1);
		mSelectedItem.layer = gameObject.layer;
		
		CBezierTo beizer = mSelectedItem.AddComponent<CBezierTo>();
		CEaseExponential ease = mSelectedItem.AddComponent<CEaseExponential>();
		CCallFunc call = mSelectedItem.AddComponent<CCallFunc>();
		CDelayTime delay = mSelectedItem.AddComponent<CDelayTime>();
		CSequence seq = mSelectedItem.AddComponent<CSequence>();
		
		float xPos = Random.Range(3.0f, 5.0f) * ((Random.Range(0,1.0f) > 0.5f ? 1 : -1));
		Vector2 spinWheelPosition = mSpinWheelObject.transform.position;
		beizer.actionWith(mSelectedItem, spinWheelPosition, spinWheelPosition+new Vector2(xPos,11.0f), spinWheelPosition +new Vector2(0, 1.0f), 0.5f);
		ease.actionWithAction(beizer, EaseType.EaseOut);
		delay.actionWithDuration(1.5f);
		call.actionWithCallBack(selectedItemAnimationCompleted);
		seq.actionWithActions(ease, delay, call);
		seq.runAction();
	}
	
	void destroySelectedItem()
	{	
		if(mSelectedItem != null)
		{
			CommonData.destroyMyObject(mSelectedItem);
			mSelectedItem = null;
		}
	}
	
	void destroySpinWheel()
	{
		if(mArrSpinWheelItems != null)
		{
			foreach(GameObject spinWheelItem in mArrSpinWheelItems)
				CommonData.destroyMyObject(spinWheelItem);
			mArrSpinWheelItems = null;
		}
		
		destroySelectedItem();
		
		if(mSpinButton != null)
		{
			MenuButtonManager.sharedManager().removeChild(mSpinButton);
			mSpinButton = null;
		}
		
		if(mSpinWheelObject != null)
		{
			DestroyObject(mSpinWheelObject);
			mSpinWheelObject = null;
		}
	}	
	
	void destroyDailyBonus()
	{		
		BJ_BottomBar.mInstance.updateChipsBalance(spinRewards);
		if(mClaimBonusButton != null)
		{
			MenuButtonManager.sharedManager().removeChild(mClaimBonusButton);
			mClaimBonusButton = null;
		}
		
		destroySpinWheel();
		
		if(mDailyBonusBG != null)
		{
			DestroyObject(mDailyBonusBG);
			mDailyBonusBG = null;
		}
	}
	
}
