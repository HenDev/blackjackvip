using UnityEngine;
using System.Collections;

public class BJ_ActivePlayer : BJ_CardsDealSpot {
	
	private BJ_Player mPlayerInstance = null;
	
	public bool isSurrender = false;
	public bool isDoubleSelected = false;
	public bool isSplitPossible = false;
	public bool isPushed = false;
				
	public float totalBet = 0.0f;
	public float totalWin = 0.0f;
	
	// Use this for initialization
	void Start () { }	
	
	public void createTheActivePlayer(Vector3 position, BJ_Player playerInstance, float betAmount)
	{
		mPlayerInstance = playerInstance;
		totalBet = betAmount;
		transform.parent = mPlayerInstance.transform;
		transform.position = position;
		InitPosition = position;		
		name = "ActivePlayer";
		gameObject.layer = mPlayerInstance.gameObject.layer;
		
		createCardCountDisplay();		
		mCardCountLabel.setPosition(new Vector3(0.0f,3.5f,0));
	}
	
	public void destroyActivePlayer()
	{
		deleteAllCards();
		if(mCardCountHolder != null)
		{
			CommonData.destroyMyObject(mCardCountHolder);
			mCardCountHolder = null;
		}
		if(mCardCountLabel != null)
		{
			CommonData.destroyMyLabel(mCardCountLabel);
			mCardCountLabel = null;
		}
		DestroyObject(gameObject);
	}
	
	public void addAlreadyFetchedCardTo(GameObject fetchedCard)
	{
		if(arrFetchedCards == null) arrFetchedCards = new ArrayList();
		arrFetchedCards.Add(fetchedCard);
		cardsCount = getCardsCount();
		isSplitted = true;
	}
	
	public GameObject getTheSplitCard() 
	{
		GameObject splitCard = (GameObject)arrFetchedCards[1];
		arrFetchedCards.Remove(splitCard);
		cardsCount = getCardsCount();
		isSplitted = true;
		return splitCard; 
	}
	
	public void moveThePlayerBy(Vector3 position)
	{
		transform.position += position;
		InitPosition = transform.position;	
		if(arrFetchedCards.Count > 1)
		foreach(GameObject cardObject in arrFetchedCards)
			cardObject.transform.position += position;
		else
		((GameObject)arrFetchedCards[0]).transform.position = InitPosition + new Vector3(-1.0f,0,0);
		
	}
	
	public void makePlayerToDeal()
	{	
		if(isSplitPossible)	
		{ 
			BJ_BottomBar.mInstance.enableSplit();
			isSplitPossible = false;
		}
	}
	
	public void doubleTheBetAndDeal()
	{
		isDoubleSelected = true;		
		dealNewCard();		
	}
	public override void dealNewCard ()
	{		
		base.dealNewCard ();
		PlayAudioSounds.sharedHandler().playSound("CardDraw");
	}
	
	public void surrenderTheHand()
	{
		isSurrender = true;
		if(mDisplayText != null) CommonData.destroyMyObject(mDisplayText);
		mDisplayText = CommonData.createGameObject("DisplayText_Surrender", gameObject, getPositionToDisplayText(), BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteManager, BJ_GamePlayLayer.mInstance.mInGamePopUpSpriteAtlasDataHandler, "Surrender_Text.png", 0);
		mDisplayText.layer = LayerMask.NameToLayer("InGamePopUps");	
		mPlayerInstance.makeNextPlayerToDeal();
	}
	
	protected override bool checkForGameEndConditions ()
	{
		if(!base.checkForGameEndConditions())
		{
			if(isDoubleSelected || cardsCount == 21)
				CCallFunc.createCallAfterDelay(mPlayerInstance.makeNextPlayerToDeal, CasinoCard_Shoe.mIntance.CasinoCardDealSpeed_Min).runAction();
			else if(arrFetchedCards.Count > 2) BJ_BottomBar.mInstance.enableButtonsToPlayerHit();			
			else if(arrFetchedCards.Count == 2)
			{
				if(isSplitted)
					BJ_BottomBar.mInstance.enableButtonsToPlayerHit();
				else if(getCasinoCardValueOf((GameObject)arrFetchedCards[0]) == getCasinoCardValueOf((GameObject)arrFetchedCards[1]))
				{
					if(isSplitted) BJ_BottomBar.mInstance.enableSplit();
					else
					isSplitPossible = true;
				}
				else if(isBlackJack)
					CCallFunc.createCallAfterDelay(mPlayerInstance.makeNextPlayerToDeal, CasinoCard_Shoe.mIntance.CasinoCardDealSpeed_Min).runAction();					
//				else if(isSplitted)
//					BJ_BottomBar.mInstance.enableButtonsToPlayerHit();
			}
		}
		else if(isBusted)
			CCallFunc.createCallAfterDelay(mPlayerInstance.makeNextPlayerToDeal, CasinoCard_Shoe.mIntance.CasinoCardDealSpeed_Min).runAction();
			//mPlayerInstance.makeNextPlayerToDeal();
	
		return true;
	}
	
	int getCasinoCardValueOf(GameObject cardObject) { return cardObject.GetComponent<CasinoCard>().getCardValue(); }
	
	public void payoutPlayer(int dealerCardCount, bool isDealerBlackjack, bool isDealerBusted)
	{
		Debug.Log("Payout");
		if(isSurrender)
		{
			Debug.Log("Surrender 1:2 payout");
			totalWin = totalBet /2.0f;
		}
		else if(isBlackJack)
		{
			Debug.Log("Blackjack 3:2 payout");	
			totalWin = totalBet * 1.5f;
			StatsHandler.BlackJacks++;
		}
		else if(isDealerBusted && !isBusted)
		{
			Debug.Log("Dealer Busted 1:1 payout");
			totalWin = totalBet;
			if(isDoubleSelected)
				totalWin += totalBet;
			showWin();
			StatsHandler.HandsWon++;
		}
		else if(!isBusted)
		{
			if(cardsCount > dealerCardCount)
			{
				Debug.Log("Normal Win 1:1 payout");
				totalWin = totalBet;
				showWin();
				StatsHandler.HandsWon++;
			}
			else if(cardsCount == dealerCardCount)
			{
				Debug.Log("Push Money Returns");
				showPush();
				isPushed = true;
				StatsHandler.HandsPushed++;
			}
			else if(cardsCount < dealerCardCount) // No Win Conditions
			{
				Debug.Log("Dealer Blackjack No payout");
				StatsHandler.HandsLost++;
				PlayAudioSounds.sharedHandler().playSound("GameLost");
			}
		}
		else if(isDealerBlackjack)// No Win Conditions
		{
			Debug.Log("Dealer Blackjack No payout");
			StatsHandler.HandsLost++;
		}
		else// No Win Conditions
		{
			Debug.Log("No Payout");
			StatsHandler.HandsLost++;
			PlayAudioSounds.sharedHandler().playSound("GameLost");
		}
		
		StatsHandler.TotalWin += Mathf.CeilToInt(totalWin);
		StatsHandler.MaxWin = Mathf.CeilToInt(totalWin);
//		OnlineInterfaceHandler.Instance.SendRequest(eONLINE_REQ_TYPE.UPDATE_VALUE_TO_SERVER,StatsHandler.MaxWin.ToString(),null,"PlayerMaxWin");
		int winToUpdate = StatsHandler.MaxWin;
#if UNITY_ANDROID
		winToUpdate = Mathf.CeilToInt(totalWin);
#endif
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Score, winToUpdate.ToString(), null);
		int lastCardValue = getCasinoCardValueOf((GameObject)arrFetchedCards[arrFetchedCards.Count-1]);
		BJ_AchievementHandler.mInstance.checkForLuckyHitAchievement(cardsCount, lastCardValue);
		BJ_AchievementHandler.mInstance.checkForPlayerWinBasedAchievements(totalWin > 0, cardsCount, Mathf.CeilToInt(totalWin));
		BJ_AchievementHandler.mInstance.checkForWittySurrender(isSurrender, isDealerBlackjack);
		BJ_AchievementHandler.mInstance.checkForDoubleDownAchievement(isDoubleSelected, cardsCount);
		//BJ_GamePlayLayer.mInstance.setTotalWin();
	}
}
