using UnityEngine;
using System.Collections;

public class InsufficientChipsHandler : PopUpBase {
	
	public CLabelAtlas mChipsBalanceLabel = null;
	MenuButton mBuyNowButton = null;
	
	// Use this for initialization
	void Start () { }	
	// Update is called once per frame
	void Update () { checkAndroidKeys(); }
	
	public override void init ()
	{
		IsInitStarted = true;
		base.init ();
		
		mBackButton.setPosition(new Vector3(0, -7.0f, 0));
		
		mChipsBalanceLabel = CLabelAtlas.create(BJ_BottomBar.mInstance.PlayerChipsBalance.ToString(), mSpriteManager, mSpriteAtlasDataHandler, "NumberFont.png", 3);
		mChipsBalanceLabel.gameObject.layer = gameObject.layer;
		mChipsBalanceLabel.setScale(0.8f);
		mChipsBalanceLabel.addParent(mPopUpBgHolder);
		mChipsBalanceLabel.setPosition(new Vector3(0,2.5f,0));
		
		mBuyNowButton = MenuButtonManager.sharedManager().createMenuItem("BuyNow_Button.png", "BuyNow_Button_Pressed.png", "", "", 3, mSpriteManager, mSpriteAtlasDataHandler, buyNowSelected);
		mBuyNowButton.addParent(mPopUpBgHolder);
		mBuyNowButton.gameObject.layer = gameObject.layer;
		mBuyNowButton.setPosition(new Vector3(0,-3.5f,0));
		IsInitDone = true;
	}
	
	protected override void removePopUp ()
	{
		base.removePopUp ();
		
		if(mChipsBalanceLabel != null)
		{
			CommonData.destroyMyLabel(mChipsBalanceLabel);
			mChipsBalanceLabel = null;
		}
		
		if(mBuyNowButton != null)
		{
			MenuButtonManager.sharedManager().removeChild(mBuyNowButton);
			mBuyNowButton = null;
		}
	}
	
	void buyNowSelected()
	{
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		movePopUpTo(30.0f, EaseType.EaseOut, popUpRemoved);
		PopUpManager.mInstance.showPopUp(PopUpManager.PopUpType.MoreChips, PopUpManager.mInstance.mPopUpRemoveCallBack);
	}
	
	void popUpRemoved()
	{
		
	}
}
