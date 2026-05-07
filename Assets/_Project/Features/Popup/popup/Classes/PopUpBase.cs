using UnityEngine;
using System.Collections;

public class PopUpBase : MonoBehaviour {
		
	public LinkedSpriteManager mSpriteManager = null;
	public SpriteAtlasDataHandler mSpriteAtlasDataHandler = null;
	public GameObject mPopUpBgHolder = null;
	
	protected MenuButton mBackButton = null;
	protected float popUpYpos = 0.0f;
	
	protected bool IsInitStarted = false;
	protected bool IsInitDone = false;
	protected bool IsHideStarted = false;
	
 
	
	public virtual void init()
	{	
		popUpYpos = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
			60.0f,  
			60.0f,  
			48.0f, 
			48.0f,  
			48.0f,  
			46.0f, 
			44.0f, 
			40.0f,  
			35.0f,  
			33.0f,  
			33.0f,  
			32f,  
			30.0f   
		);

		transform.position = new Vector3(0,popUpYpos, transform.position.z);
		
		mBackButton = MenuButtonManager.sharedManager().createMenuItem("Back_Button.png", "Back_Button_Pressed.png", "", "", 2, mSpriteManager, mSpriteAtlasDataHandler, backSelected);
		mBackButton.addParent(mPopUpBgHolder);
		mBackButton.gameObject.layer = gameObject.layer;
		mBackButton.setPosition(new Vector3(0,-12.6f,0));
	}
	
	protected virtual void removePopUp()
	{
		if(mBackButton != null)
		{
			MenuButtonManager.sharedManager().removeChild(mBackButton);
			mBackButton = null;
		}
	}	
	
	protected void movePopUpTo(float yPos, EaseType easeActionType, CCallFunc.CallBack callBackFunction)
	{
		if(GetComponent<CMoveTo>() != null)
			Destroy(GetComponent<CMoveTo>());
		if(GetComponent<CEaseExponential>() != null)
			Destroy(GetComponent<CEaseExponential>());
		if(GetComponent<CCallFunc>() != null)
			Destroy(GetComponent<CCallFunc>());
		if(GetComponent<CSequence>() != null)
			Destroy(GetComponent<CSequence>());
		
		CMoveTo move = gameObject.AddComponent<CMoveTo>();
		CEaseExponential ease = gameObject.AddComponent<CEaseExponential>();
		CCallFunc call = gameObject.AddComponent<CCallFunc>();
		CSequence seq = gameObject.AddComponent<CSequence>();
		
		move.actionWith(gameObject, new Vector2(0,yPos), 0.5f);
		ease.actionWithAction(move, easeActionType);
		call.actionWithCallBack(callBackFunction);
		seq.actionWithActions(ease, call);
		
		seq.runAction();
	}
	
	public virtual void showPopUp()
	{
		if(!IsInitDone) 
		{
			if(!IsInitStarted) removePopUp();
			init();
		}
		mBackButton.setTouchEnable(false);
		movePopUpTo(0.0f, EaseType.EaseOut, showCompleted);
	}
	
	public void hidePopUp()
	{
		IsHideStarted = true;
		removePopUpSelected();
		movePopUpTo(popUpYpos, EaseType.EaseIn, hideCompleted);
	}
	
	void backSelected()
	{
		if(IsHideStarted) return;
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		hidePopUp();		
		PopUpManager.mInstance.alphalayer.GetComponent<AlphaLayerHandler>().fadeOutAlphaLayer(0.5f);
	}
	
	protected virtual void removePopUpSelected()
	{		
	}
		
	protected virtual void showCompleted()
	{
		mBackButton.setTouchEnable(true);
	}
		
	void hideCompleted()
	{
		if(PopUpManager.mInstance.mPopUpRemoveCallBack != null)
			PopUpManager.mInstance.mPopUpRemoveCallBack();
		PopUpManager.mInstance.isPopUpActive = false;
		IsHideStarted = false;
	}
	
	// Android Buttons
	protected void checkAndroidKeys()
	{
		if(Input.GetKeyDown(KeyCode.Escape)) // Back Button
		{
			backSelected();
		}
	}
}
