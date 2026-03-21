using UnityEngine;
using System.Collections;

public class BJ_CardsDealSpot : MonoBehaviour {
	
	public Vector3 InitPosition = Vector3.zero;
	protected ArrayList arrFetchedCards = null;
	
	public string LayerName = "";	
	public int cardsCount = 0;
	public bool isBlackJack = false;
	public bool isBusted = false;
	public bool isSplitted = false;
	
	protected GameObject mBlackJackGlow = null;
	protected GameObject mDisplayText = null;	
	protected GameObject mCardCountHolder = null;
	protected CLabelAtlas mCardCountLabel = null;
	
	protected float dealerYOffset = 0.0f;
		
	// Use this for initialization
	void Start () {	}	
	// Update is called once per frame
	void Update () { 
	
		if(mCardCountLabel != null)
		{
			if(!mCardCountHolder.GetComponent<SpriteData>().isVisible() && cardsCount > 0)
				mCardCountHolder.GetComponent<SpriteData>().setVisible(true);
			mCardCountLabel.setString(cardsCount > 0 ? cardsCount.ToString() : "");
		}
	}
	
	protected void createCardCountDisplay()
	{			
		LinkedSpriteManager spriteManager = BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteManager;
		SpriteAtlasDataHandler spriteAtlasDataHandler = BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteAtlasDataHandler;
		
		mCardCountLabel = CLabelAtlas.create("", spriteManager, spriteAtlasDataHandler, "NumberFont.png", 2);
		mCardCountLabel.addParent(gameObject);
		mCardCountLabel.setPosition(new Vector3(0.0f,0.0f,0));
		mCardCountLabel.gameObject.layer = gameObject.layer;
		mCardCountLabel.setScale(0.7f);
		mCardCountLabel.setSpacing(-5.0f);
		
		mCardCountHolder = CommonData.createGameObject("CardCountHolder", mCardCountLabel.gameObject, Vector3.zero, spriteManager, spriteAtlasDataHandler, "Count_Holder.png", 1);
		mCardCountHolder.layer = gameObject.layer;
		mCardCountHolder.GetComponent<SpriteData>().setVisible(false);
	}
	
	virtual public void dealNewCard()
	{
		if(arrFetchedCards == null) arrFetchedCards = new ArrayList();
		
		GameObject cardObject_ = CasinoCard_Shoe.mIntance.dealACardTo(InitPosition + getNewCardPosition(), onCardDealt);
		arrFetchedCards.Add(cardObject_);
	}
	
	virtual public void moveCardsToStack()
	{
		// Moving Fetched Cards To Stack
		if(arrFetchedCards != null)
		foreach(GameObject cardObject in arrFetchedCards)
			CasinoCard_Shoe.mIntance.moveCardToTheStack(cardObject);
	}
	
	virtual protected void onCardDealt(GameObject sender)
	{
		sender.transform.position += new Vector3(0,0,-arrFetchedCards.Count);
		sender.GetComponent<CasinoCard>().flipCardVertically();
		cardsCount = getCardsCount();
	}		
	
	public void deleteAllCards()
	{
		if(arrFetchedCards != null)
		{
			foreach(GameObject cardObject in arrFetchedCards)
				DestroyObject(cardObject);
			arrFetchedCards.Clear();
		}
		arrFetchedCards = null;
		if(mDisplayText != null)
		{
			CommonData.destroyMyObject(mDisplayText);
			mDisplayText = null;
		}
		if(mBlackJackGlow != null)
		{
			CommonData.destroyMyObject(mBlackJackGlow);
			mBlackJackGlow = null;
		}
		cardsCount = 0;
		isBlackJack = false;
		isBusted = false;
		if(mCardCountHolder != null)
			mCardCountHolder.GetComponent<SpriteData>().setVisible(false);
	}
	
	public int getCardsCount()
	{
		//int cardsCount_ = 0;
		int lengthOfArray = arrFetchedCards.Count;
		
		// Creating new arr
		int[] arrOfCardValues = new int[lengthOfArray];
		for(int i=0; i< lengthOfArray; i++)
		{
			arrOfCardValues[i] = ((GameObject)arrFetchedCards[i]).GetComponent<CasinoCard>().getCardValue();
			arrOfCardValues[i] = Mathf.Clamp(arrOfCardValues[i], 1, 10);
			if(arrOfCardValues[i] == 1) arrOfCardValues[i] = 11; // Assigning 11 for all 1's
		}
		
		// Loop To Count the Card Values
		while(true)
		{
			cardsCount = 0;
			// Counting the card Value
			foreach(int cardValue in arrOfCardValues)
				cardsCount += cardValue;
			
			if(cardsCount <= 21)
				break;
			
			bool isAceDetected = false;
			for(int i=0; i< lengthOfArray; i++)
			{
				if(arrOfCardValues[i] == 11)
				{
					arrOfCardValues[i] = 1;
					isAceDetected = true;
					break;
				}
			}
			
			if(!isAceDetected) break;
		}
		
		checkForGameEndConditions();
		
		return cardsCount;
	}	
	
	virtual protected bool checkForGameEndConditions()
	{
		// Condition Checking
		if(cardsCount == 21 && arrFetchedCards.Count == 2 && !isSplitted) { showBlackJack(); return true; }
		else if(cardsCount > 21) { showBusted(); return true; }
		return false;
	}
	
	// Notifications
	protected void showBlackJack()
	{
		PlayAudioSounds.sharedHandler().playSound("BlackJack");
		Debug.Log("Show Blackjack");
		if(mDisplayText != null) return;
		mDisplayText = CommonData.createGameObject("DisplayText_BlackJack", gameObject, getPositionToDisplayText(), BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteManager, BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteAtlasDataHandler, "BlackJack_Text.png", 1);
		mDisplayText.layer = LayerMask.NameToLayer("InGamePopUps");
		isBlackJack = true;
		
		CScaleTo scale1 = mDisplayText.AddComponent<CScaleTo>();
		CScaleTo scale2 = mDisplayText.AddComponent<CScaleTo>();
		CSequence seq = mDisplayText.AddComponent<CSequence>();
		CRepeat repeat1 = mDisplayText.AddComponent<CRepeat>();		
		
		scale1.actionWith(mDisplayText, new Vector2(1.1f,1.1f), 0.2f);
		scale2.actionWith(mDisplayText, new Vector2(0.9f,0.9f), 0.2f);
		seq.actionWithActions(scale1, scale2);
		repeat1.actionWithAction(seq, -1);
		repeat1.runAction();
				
		// Glow
		mBlackJackGlow = CommonData.createGameObject("Glow_BlackJack", gameObject, getPositionToDisplayText(), BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteManager, BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteAtlasDataHandler, "Sunrays.png", 0);
		mBlackJackGlow.layer = LayerMask.NameToLayer("InGamePopUps");
		
		CRotateBy rotate = mBlackJackGlow.AddComponent<CRotateBy>();
		CRepeat repeat = mBlackJackGlow.AddComponent<CRepeat>();
		
		rotate.actionWith(mBlackJackGlow, new Vector3(0,0,100), 0.1f);
		repeat.actionWithAction(rotate, -1);
		
		repeat.runAction();
	}
	
	protected void showPush()
	{
		PlayAudioSounds.sharedHandler().playSound("GamePush");
		Debug.Log("Show Push");
		if(mDisplayText != null || this == null) return;
		mDisplayText = CommonData.createGameObject("DisplayText_Push", gameObject, getPositionToDisplayText(), BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteManager, BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteAtlasDataHandler, "Push_Text.png", 0);
		mDisplayText.layer = LayerMask.NameToLayer("InGamePopUps");
		
		CScaleTo scale1 = mDisplayText.AddComponent<CScaleTo>();
		CScaleTo scale2 = mDisplayText.AddComponent<CScaleTo>();
		CSequence seq = mDisplayText.AddComponent<CSequence>();
		CRepeat repeat1 = mDisplayText.AddComponent<CRepeat>();		
		
		scale1.actionWith(mDisplayText, new Vector2(1.1f,1.1f), 0.2f);
		scale2.actionWith(mDisplayText, new Vector2(0.9f,0.9f), 0.2f);
		seq.actionWithActions(scale1, scale2);
		repeat1.actionWithAction(seq, 1);
		repeat1.runAction();
	}
	
	protected void showWin()
	{
		PlayAudioSounds.sharedHandler().playSound("GameWon");
		Debug.Log("Show Win");
		if(mDisplayText != null || this == null) return;
		mDisplayText = CommonData.createGameObject("DisplayText_Win", gameObject, getPositionToDisplayText(), BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteManager, BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteAtlasDataHandler, "Win_Text.png", 0);
		mDisplayText.layer = LayerMask.NameToLayer("InGamePopUps");
		
		CScaleTo scale1 = mDisplayText.AddComponent<CScaleTo>();
		CScaleTo scale2 = mDisplayText.AddComponent<CScaleTo>();
		CSequence seq = mDisplayText.AddComponent<CSequence>();
		CRepeat repeat1 = mDisplayText.AddComponent<CRepeat>();		
		
		scale1.actionWith(mDisplayText, new Vector2(1.1f,1.1f), 0.2f);
		scale2.actionWith(mDisplayText, new Vector2(0.9f,0.9f), 0.2f);
		seq.actionWithActions(scale1, scale2);
		repeat1.actionWithAction(seq, 1);
		repeat1.runAction();
	}
	
	protected void showBusted()
	{
		PlayAudioSounds.sharedHandler().playSound("Busted");
		Debug.Log("Show Busted");
		if(mDisplayText != null) return;
		mDisplayText = CommonData.createGameObject("DisplayText_Bust", gameObject, getPositionToDisplayText(), BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteManager, BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteAtlasDataHandler, "Bust_Text.png", 0);
		mDisplayText.layer = LayerMask.NameToLayer("InGamePopUps");
		isBusted = true;	
		
		CScaleTo scale1 = mDisplayText.AddComponent<CScaleTo>();
		CScaleTo scale2 = mDisplayText.AddComponent<CScaleTo>();
		CSequence seq = mDisplayText.AddComponent<CSequence>();
		CRepeat repeat1 = mDisplayText.AddComponent<CRepeat>();		
		
		scale1.actionWith(mDisplayText, new Vector2(1.1f,1.1f), 0.2f);
		scale2.actionWith(mDisplayText, new Vector2(0.9f,0.9f), 0.2f);
		seq.actionWithActions(scale1, scale2);
		repeat1.actionWithAction(seq, 1);
		repeat1.runAction();	
	}
	
	virtual protected Vector3 getPositionToDisplayText()
	{
		int NoOfCards = arrFetchedCards.Count;
		Vector3 initPositionOfCards = ((GameObject)arrFetchedCards[0]).transform.position - transform.position;
		return initPositionOfCards + new Vector3(NoOfCards * 0.25f, dealerYOffset,0);
	}
	
	virtual protected Vector3 getNewCardPosition()
	{
		return new Vector3(-1.0f + 1.5f * arrFetchedCards.Count,0.0f * arrFetchedCards.Count,0);
	}
}
