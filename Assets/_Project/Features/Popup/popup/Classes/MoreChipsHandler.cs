using UnityEngine;
using System.Collections;

public class MoreChipsHandler : PopUpBase
{
    public float[] mChipsCount = new float[0];
    public float[] mChipsCost = new float[0];
    float rateReviewChips = 500;
    public CLabelAtlas mChipsBalanceLabel = null;
    CLabelAtlas timerLabel = null;
    MenuButton claimBonus = null;
    MenuButton rateReviewButton = null;
    GameObject claimBonusText = null;

    public TextMesh[] mArrChipsCostLabels = null;
//	bool mIsShopLoaded = false;
//	bool mIsShopLoadingFailed = false;

    GameObject chipsBalanceHolder = null;
    ArrayList mButtonsArray = null;

    // For Mac In App Chips Cost
    private string[] ids;

    public static bool RateReviewDone
    {
        get { return PlayerPrefs.GetInt("RateReviewDone") == 1; }
        set { PlayerPrefs.SetInt("RateReviewDone", value ? 1 : 0); }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        checkAndroidKeys();
    }

//	void Awake()
//	{
//		ids = new string[] {"1", "2","3","4","5","6","7"};
//		
//		//ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StartLoadingScreen,"",null);
//		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.GetInAppData,"",GetInAppData);
//	}


    public override void init()
    {
        IsInitStarted = true;
        base.init();

        createBonus();
        createButtons();
        createChipsBalance();
        createRateReview();
        IsInitDone = true;
    }

    protected override void removePopUp()
    {
        base.removePopUp();

        removeBonus();
        removeButtons();
        removeChipsBalance();
        removeRateReviewToReload();
    }

    void createChipsBalance()
    {
        chipsBalanceHolder = CommonData.createGameObject("Chips Balance Holder", mPopUpBgHolder,
            new Vector3(0, -10.0f, 0), mSpriteManager, mSpriteAtlasDataHandler, "ChipBalanceHolder.png", 2);
        mPopUpBgHolder.layer = gameObject.layer;

        mChipsBalanceLabel = CLabelAtlas.create(BJ_BottomBar.mInstance.PlayerChipsBalance.ToString(), mSpriteManager,
            mSpriteAtlasDataHandler, "NumberFont.png", 3);
        mChipsBalanceLabel.addParent(chipsBalanceHolder);
        mChipsBalanceLabel.gameObject.layer = gameObject.layer;
        mChipsBalanceLabel.setSpacing(-5.0f);
    }

    void createBonus()
    {
        claimBonus = MenuButtonManager.sharedManager().createMenuItem("ShopStrip_Button.png",
            "ShopStrip_Button_Pressed.png", "", "", 2, mSpriteManager, mSpriteAtlasDataHandler, claimBonusSelected);
        claimBonus.addParent(mPopUpBgHolder);
        claimBonus.gameObject.layer = gameObject.layer;
        claimBonus.setPosition(new Vector3(0, 8.6f, -1));

        timerLabel = CLabelAtlas.create("00:00:00", mSpriteManager, mSpriteAtlasDataHandler, "NumberFont1.png", 4);
        timerLabel.addParent(claimBonus.gameObject);
        timerLabel.gameObject.layer = gameObject.layer;
        timerLabel.setScale(1.0f);
        timerLabel.setVisible(false);
        setTimer(BJ_BottomBar.mInstance.mBonusHandler.timerHours, BJ_BottomBar.mInstance.mBonusHandler.timerMinutes,
            Mathf.FloorToInt(BJ_BottomBar.mInstance.mBonusHandler.timerSeconds));

        claimBonusText = CommonData.createGameObject("ClaimBonusText", claimBonus.gameObject, Vector3.zero,
            mSpriteManager, mSpriteAtlasDataHandler, "ClaimBonusText.png", 3);
        claimBonusText.layer = gameObject.layer;

        CScaleTo scale1 = claimBonusText.AddComponent<CScaleTo>();
        CScaleTo scale2 = claimBonusText.AddComponent<CScaleTo>();
        CSequence seq = claimBonusText.AddComponent<CSequence>();
        CRepeat repeat = claimBonusText.AddComponent<CRepeat>();

        scale1.actionWith(claimBonusText, new Vector2(1.1f, 1.1f), 0.2f);
        scale2.actionWith(claimBonusText, new Vector2(0.9f, 0.9f), 0.2f);
        seq.actionWithActions(scale1, scale2);
        repeat.actionWithAction(seq, -1);
        repeat.runAction();

        toggleBonusDisplay();
    }

    void createButtons()
    {
        for (int i = 0; i < 6; i++)
        {
            MenuButton holderFrame = MenuButtonManager.sharedManager().createMenuItem("ShopStrip_Button.png",
                "ShopStrip_Button_Pressed.png", "", "", 2, mSpriteManager, mSpriteAtlasDataHandler, buttonSelected);
            holderFrame.addParent(mPopUpBgHolder);
            holderFrame.gameObject.layer = gameObject.layer;
            holderFrame.setTag(i);
            holderFrame.setPosition(new Vector3(0, (RateReviewDone ? 6.0f : 6.3f) - i * (RateReviewDone ? 2.7f : 2.3f),
                0));

            GameObject ChipImage = CommonData.createGameObject("Chip Image", holderFrame.gameObject,
                new Vector3(-5.0f, 0.0f, 0), mSpriteManager, mSpriteAtlasDataHandler, "ChipImage" + (i + 1) + ".png",
                3);
            ChipImage.layer = gameObject.layer;

            if (i == 5)
            {
                GameObject BestOfferText = CommonData.createGameObject("BestOffer Image", holderFrame.gameObject,
                    new Vector3(-5.0f, 0.0f, 0), mSpriteManager, mSpriteAtlasDataHandler, "BestOfferText.png", 4);
                BestOfferText.layer = gameObject.layer;
            }

            CLabelAtlas chipLabel = CLabelAtlas.create(mChipsCount[i].ToString(), mSpriteManager,
                mSpriteAtlasDataHandler, "NumberFont1.png", 3, CLabelAtlas.LabelTextAlignment.Left);
            chipLabel.addParent(holderFrame.gameObject);
            chipLabel.gameObject.layer = gameObject.layer;
            chipLabel.setSpacing(-5.0f);
            chipLabel.setScale(0.8f);
            chipLabel.setPosition(new Vector3(-4.0f, 0, 0));

            mArrChipsCostLabels[i].text = "$" + mChipsCost[i].ToString();
            mArrChipsCostLabels[i].transform.parent = holderFrame.transform;
            mArrChipsCostLabels[i].transform.position = holderFrame.transform.position + new Vector3(5.5f, 0, -3);

            if (mButtonsArray == null) mButtonsArray = new ArrayList();
            mButtonsArray.Add(holderFrame);
        }
    }

//	public void GetInAppData(eEXTERNAL_REQ_TYPE eType,string str , string status)
//	{
//		switch(eType)
//		{
//		case eEXTERNAL_REQ_TYPE.GetInAppData:
//			Debug.Log("....received status.."+status);
//			if(status == "true")
//			{
//     			Debug.Log("Data Received");
//				for(int i=0; i< mArrChipsCostLabels.Length; i++)
//				{
//					mArrChipsCostLabels[i].text = ids[i];// "10$";
//				}
////				mIsShopLoaded = true;
//				
//				// enable current screen button
//				//CheckButton(true);
//			}
//			else
//			{
//     			Debug.Log("Data not Received");
////				GUIItem item = null;
////				OnSelectedEvent(item);
//			}
//			
////			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StopLoadingScreen,"",null);
//			Debug.Log("Stop Loading Screen.....");
//			break;
//		}
//	}

    void createRateReview()
    {
        if (RateReviewDone) return;
        rateReviewButton = MenuButtonManager.sharedManager().createMenuItem("RateReview_Button.png",
            "RateReview_Button_Pressed.png", "", "", 2, mSpriteManager, mSpriteAtlasDataHandler, rateReviewSelected);
        rateReviewButton.addParent(mPopUpBgHolder);
        rateReviewButton.gameObject.layer = gameObject.layer;
        rateReviewButton.setPosition(new Vector3(0, 6.3f - 6 * 2.3f, 0));

//		GameObject rateReviewHolder = CommonData.createGameObject("Chip Image", rateReviewButton.gameObject, new Vector3(-5.0f, 0.0f, 0), mSpriteManager, mSpriteAtlasDataHandler, "ChipImage.png", 3);
//		rateReviewHolder.layer = gameObject.layer;

        GameObject rateReviewText = CommonData.createGameObject("Rate Review Image", rateReviewButton.gameObject,
            new Vector3(0.0f, 0.0f, 0), mSpriteManager, mSpriteAtlasDataHandler, "RateReivewText.png", 3);
        rateReviewText.layer = gameObject.layer;

//		GameObject FreeText = CommonData.createGameObject("Free Image", rateReviewButton.gameObject, new Vector3(-5.0f, 0.0f, 0), mSpriteManager, mSpriteAtlasDataHandler, "FreeText.png", 4);
//		FreeText.layer = gameObject.layer;

//		CLabelAtlas chipLabel = CLabelAtlas.create(rateReviewChips.ToString(), mSpriteManager, mSpriteAtlasDataHandler, "NumberFont1.png", 3, CLabelAtlas.LabelTextAlignment.Left);
//		chipLabel.addParent(rateReviewButton.gameObject);
//		chipLabel.gameObject.layer = gameObject.layer;
//		chipLabel.setSpacing(-5.0f);
//		chipLabel.setPosition(new Vector3(-3.5f,0,0));
    }

    void removeRateReviewToReload()
    {
        if (rateReviewButton != null)
        {
            if (rateReviewButton.GetComponentInChildren<CLabelAtlas>() != null)
                CommonData.destroyMyLabel(rateReviewButton.GetComponentInChildren<CLabelAtlas>());

            SpriteData[] arrSprites = rateReviewButton.GetComponentsInChildren<SpriteData>();
            foreach (SpriteData sprData in arrSprites)
                CommonData.destroyMyObject(sprData.gameObject);

            MenuButtonManager.sharedManager().removeChild(rateReviewButton);
            rateReviewButton = null;
        }
    }

    void removeBonus()
    {
        if (claimBonus != null)
        {
            MenuButtonManager.sharedManager().removeChild(claimBonus);
            claimBonus = null;
        }

        if (timerLabel != null)
        {
            CommonData.destroyMyLabel(timerLabel);
            timerLabel = null;
        }

        if (claimBonusText != null)
        {
            CommonData.destroyMyObject(claimBonusText);
            claimBonusText = null;
        }
    }

    void removeButtons()
    {
        if (mButtonsArray != null)
            foreach (MenuButton button in mButtonsArray)
            {
                CLabelAtlas labelAtlas = button.GetComponentInChildren<CLabelAtlas>();
                if (labelAtlas != null) CommonData.destroyMyLabel(labelAtlas);

                SpriteData[] arrSprites = button.GetComponentsInChildren<SpriteData>();
                if (arrSprites != null)
                    foreach (SpriteData sprData in arrSprites)
                        CommonData.destroyMyObject(sprData.gameObject);

                MenuButtonManager.sharedManager().removeChild(button);
            }

        if (mArrChipsCostLabels != null)
            foreach (TextMesh textLabel in mArrChipsCostLabels)
                textLabel.text = "";
    }

    void removeChipsBalance()
    {
        if (chipsBalanceHolder != null)
        {
            if (mChipsBalanceLabel != null)
            {
                CommonData.destroyMyLabel(mChipsBalanceLabel);
                mChipsBalanceLabel = null;
            }

            CommonData.destroyMyObject(chipsBalanceHolder);
            mChipsBalanceLabel = null;
        }
    }

    public void toggleBonusDisplay()
    {
        if (BJ_BottomBar.mInstance.mBonusHandler.isBonusActive)
        {
            claimBonus.setTouchEnable(true);
            claimBonusText.GetComponent<SpriteData>().setVisible(true);
            timerLabel.setVisible(false);
        }
        else
        {
            claimBonus.setTouchEnable(false);
            claimBonusText.GetComponent<SpriteData>().setVisible(false);
            timerLabel.setVisible(true);
        }
    }

    void buttonSelected(MenuButton sender)
    {
        PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
        int buttonIndex = sender.getTag() + 1;

        //payUserWith(buttonIndex.ToString());		
        ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.InAppConsumable, buttonIndex.ToString(),
            InAppPurchase);
    }

    void removeRateReview()
    {
        if (rateReviewButton == null) return;
//		CommonData.destroyMyLabel(rateReviewButton.GetComponentInChildren<CLabelAtlas>());
        SpriteData[] arrSprites = rateReviewButton.GetComponentsInChildren<SpriteData>();
        foreach (SpriteData sprData in arrSprites)
            CommonData.destroyMyObject(sprData.gameObject);
        MenuButtonManager.sharedManager().removeChild(rateReviewButton);
        rateReviewButton = null;
    }

#if UNITY_IPHONE
	void rateReviewSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		//PlayerPrefs.SetInt("IsRateAppSelected",1);
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.RateApplication,"",RateTheAppCallBack);
	}
#elif UNITY_ANDROID
	void rateReviewSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		
		RateReviewDone = true;		
		removeRateReview();
		
		foreach(MenuButton button in mButtonsArray)
			button.transform.position += new Vector3(0, -0.3f -  button.getTag() * (0.4f),0);
		
		BJ_BottomBar.mInstance.updateChipsBalance((int)rateReviewChips);
		
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.RateApplication,"",null);
	}
#endif 

    void claimBonusSelected()
    {
        PlayAudioSounds.sharedHandler().playSound("Bonus");
        BJ_BottomBar.mInstance.mBonusHandler.claimBonus();
        BlackjackFlurryHandler.sendClaimBonusFlurryData();
    }

    public void setTimer(int hours, int minutes, int seconds)
    {
        if (timerLabel == null) return;
        string hours_ = (hours < 10 ? "0" : "") + hours;
        string minutes_ = (minutes < 10 ? "0" : "") + minutes;
        string seconds_ = (seconds < 10 ? "0" : "") + seconds;
        timerLabel.setString(hours_ + "/" + minutes_ + "/" + seconds_);
    }

    // After Succesfull InApp Purchace Adding Chips
    void payUserWith(string requestedId)
    {
        int buttonIndex = 0;
        int.TryParse(requestedId, out buttonIndex);
        buttonIndex--;
        BJ_BottomBar.mInstance.updateChipsBalance((int)mChipsCount[buttonIndex]);
        PlayAudioSounds.sharedHandler().playSound("Bonus");
        BlackjackFlurryHandler.sendChipPurchaseFlurryData(buttonIndex + 1, (int)mChipsCount[buttonIndex],
            (int)mChipsCost[buttonIndex]);
    }

    void RateTheAppCallBack(eEXTERNAL_REQ_TYPE reqType, string requestedId, string receivedStatus)
    {
        if (reqType == eEXTERNAL_REQ_TYPE.RateApplication && receivedStatus == "True")
        {
            RateReviewDone = true;
            removeRateReview();

            foreach (MenuButton button in mButtonsArray)
                button.transform.position += new Vector3(0, -0.3f - button.getTag() * (0.4f), 0);

//			BJ_BottomBar.mInstance.updateChipsBalance((int)rateReviewChips);
        }
    }

//	void RateTheAppCallBack(eEXTERNAL_REQ_TYPE reqType , string requestedId , string receivedStatus)
//	{
//		if(reqType == eEXTERNAL_REQ_TYPE.RateApplication && receivedStatus == "True")
//		{			
//			RateReviewDone = true;		
//			removeRateReview();
//			
//			foreach(MenuButton button in mButtonsArray)
//				button.transform.position += new Vector3(0, -0.3f -  button.getTag() * (0.4f),0);
//			
//			//BJ_BottomBar.mInstance.updateChipsBalance((int)rateReviewChips);
//		}		
//	}

    void InAppPurchase(eEXTERNAL_REQ_TYPE reqType, string requestedId, string receivedStatus)
    {
        Debug.Log("InAppPurchase().....Data Received..." + receivedStatus);

        switch (reqType)
        {
            case eEXTERNAL_REQ_TYPE.InAppConsumable:
                if (receivedStatus == "true")
                {
                    payUserWith(requestedId);

                    switch (requestedId)
                    {
                        case "1":
                            Debug.Log("InAppConsumable: Request 1 completed");
                            break;
                        case "2":
                            Debug.Log("InAppConsumable: Request 2 completed");
                            break;
                        case "3":
                            break;
                    }
                }
                else
                {
                    // purchase failed..
                }

                break;
            case eEXTERNAL_REQ_TYPE.InAppNonConsumable:
                if (receivedStatus == "true")
                {
                    switch (requestedId)
                    {
                        case "1":
                            Debug.Log("InAppNonConsumable: Request 1 completed");
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
//		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StopLoadingScreen,"",null);
//		enableAllButtons();		
    }
}