using UnityEngine;
using System.Collections;

// States of Possible BlackJack Gameplay
public enum BJ_GamePlayStates {BJ_Betting=0, BJ_Dealing, BJ_PlayersTurn, BJ_DealersTurn, BJ_WaitForInsurance, BJ_InsurancePayout, BJ_PayOut, BJ_StackingCards, BJ_Shuffling};

public class BJ_GamePlayLayer : MonoBehaviour {
	
	public static BJ_GamePlayLayer mInstance = null;
	
	public BJ_GamePlayStates mCurrentGameState = BJ_GamePlayStates.BJ_Betting;
	public BJ_Dealer BJ_DealerInstance = null;
	
	public AlphaLayerHandler mAlphaLayer = null;
	
	// SpriteManagers
//	public LinkedSpriteManager mGamePlaySpriteManager = null;
//	public SpriteAtlasDataHandler mGamePlaySpriteAtlasDataHandler = null;
	public LinkedSpriteManager mInGamePopUpSpriteManager = null;
	public SpriteAtlasDataHandler mInGamePopUpSpriteAtlasDataHandler = null;
	public LinkedSpriteManager mChipsSpriteManager = null;
	public SpriteAtlasDataHandler mIChipsSpriteAtlasDataHandler = null;
	
	public BJ_Player player = null;
	
	private bool isPlayerToDeal = true;
	public bool isPrevBetAvailable = false;
	// Use this for initialization
	void Start () {
		PlayAudioSounds.sharedHandler().playBgMusic("GameTune");
		mInstance = this;
		StartCoroutine(Instantiate());		
	}
	
	IEnumerator Instantiate()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.1f);
			
			float yPosOffset = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(2.0f, 0.0f, -1.0f, 4.0f, 5.0f, 3.0f, 1.0f, 5.0f, 6.0f);
			CasinoCard_Shoe.mIntance.gameObject.transform.position += new Vector3(0, yPosOffset, 0);			
			yPosOffset = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(4.0f, 3.0f, 4.0f, 4.0f, 4.0f, 4.0f, 3.5f, 4.0f, 4.0f);
			if(!Loading.IsAdsRemoved) CasinoCard_Shoe.mIntance.gameObject.transform.position += new Vector3(0, -yPosOffset, 0);				
			
			// Creating Player
			getCurrentPlayerToDeal().createPlayer();			
			
			// Creating Dealer
			BJ_DealerInstance.createDealer();
			
			// Fading Out AlphaLayer
			mAlphaLayer.fadeOutAlphaLayer(0.2f);
			
			break;
		}
	}
	
	// Update is called once per frame
	void Update () { }
	
	// Making The Players and the Dealer To Deal after the Cards are drawn
	public void makeNextPlayerToDeal()
	{
		Debug.Log("makeNextPlayerToDeal");
		if(mCurrentGameState == BJ_GamePlayStates.BJ_DealersTurn)
		{
			// Payout
			Debug.Log("Payout");
			mCurrentGameState = BJ_GamePlayStates.BJ_PayOut;
			BJ_BottomBar.mInstance.disableAllButtons();
			CCallFunc.createCallAfterDelay(PayoutPlayer, 0.5f).runAction();
		}
		else
		{		
			bool isDealersTurn = !isPlayerToDeal;
			if(mCurrentGameState == BJ_GamePlayStates.BJ_PlayersTurn && isPlayerToDeal)	// Players Turn
			{
				Debug.Log("Player");
				if(!getCurrentPlayerToDeal().makePlayerToDeal())
				isDealersTurn = true;
				isPlayerToDeal = false;
			}
			if(isDealersTurn)	// Dealers Turn
			{
				Debug.Log("Dealer");
				if(mCurrentGameState != BJ_GamePlayStates.BJ_DealersTurn)
				{
					mCurrentGameState = BJ_GamePlayStates.BJ_DealersTurn;
					BJ_BottomBar.mInstance.disableSplit();
					BJ_DealerInstance.startDealingForDealerTurn();
				}
			}	
		}	
	}
	
	public void makePlayerToDeal()
	{
		mCurrentGameState = BJ_GamePlayStates.BJ_PlayersTurn;
		isPrevBetAvailable = true;
		isPlayerToDeal = true;
		makeNextPlayerToDeal();
		BJ_BottomBar.mInstance.enableAllButtons();	
	}
	
	// Bet Insurance	
	public void askForInsuranceDone()
	{		
		mCurrentGameState = BJ_GamePlayStates.BJ_InsurancePayout;
		BJ_BottomBar.mInstance.removeBetInsurance();
		BJ_DealerInstance.makeDealerCheckForBlackJack();
	}
	
	public void makePlayerToBetInsurance()
	{ 
		StopCoroutine("DealingCards");
		mCurrentGameState = BJ_GamePlayStates.BJ_WaitForInsurance;
		isPlayerToDeal = true;
		
		int totalBet = (int)BJ_GamePlayLayer.mInstance.player.totalBet;
		int startChipValue = (int)BJ_BottomBar.mInstance.mArrChipValues[0];
		if(totalBet == startChipValue) 
		{
			Debug.Log("InsuranceNotPossible "+startChipValue+" "+totalBet+" "+BJ_BottomBar.mInstance.PlayerChipsBalance);
			mCurrentGameState = BJ_GamePlayStates.BJ_InsurancePayout;
			BJ_DealerInstance.makeDealerCheckForBlackJack();
		}
		else
		{
			Debug.Log("ShowInsurance");
			BJ_BottomBar.mInstance.showBetInsurance();
		}
	}	
	
	public void collectAllInsuranceMoney()
	{
		player.moveTheInsuranceToTheDealer();
	}
	
	// Getters
	public BJ_Player getCurrentPlayerToDeal()
	{
		return player;
	}
	
	// Clear Cards
	public void stackAllCards()
	{
		if(mCurrentGameState != BJ_GamePlayStates.BJ_PayOut) return;
		mCurrentGameState = BJ_GamePlayStates.BJ_StackingCards;
		
		// Moving Fetched Cards To Stack
		StartCoroutine(MovingCardsToStack());
	}
	
	public void placePreviousBet()
	{
		getCurrentPlayerToDeal().rebetTheChips();
		setTotalBet();
	}
	
	public void clearChipsFromTable()
	{
		getCurrentPlayerToDeal().reloadPlayer();
		setTotalBet();
	}
	
	IEnumerator MovingCardsToStack()
	{
		int i=0;
		while(true)
		{
			float waitForSeconds = (i == 2 ? CasinoCard_Shoe.mIntance.CasinoCardDealSpeed_Max : 
				CasinoCard_Shoe.mIntance.CasinoCardDealSpeed_Min);
			yield return new WaitForSeconds(waitForSeconds);
			if(i == 2)
			{		
				// Deleting All Previous Cards
					getCurrentPlayerToDeal().reloadPlayer();
				BJ_DealerInstance.deleteAllCards();
				setTotalBet();
				
				// Checking if the Shuffling is needed
				if(CasinoCard_Shoe.mIntance.checkIfShuffleNeeded())
				{
					//mCurrentGameState = BJ_GamePlayStates.BJ_Shuffling;
					//break;
				}
				
				mCurrentGameState = BJ_GamePlayStates.BJ_Betting;	
				BJ_BottomBar.mInstance.changeToBetting();
				break;
			}
			else //if(i==0)				// Player
			{
				getCurrentPlayerToDeal().moveCardsToStack();
			//else 						// Dealer
				BJ_DealerInstance.moveCardsToStack();
				i++;
			}
			i++;
		}
	}
	
	// Deal Cards
	public bool dealCards()
	{
		if(mCurrentGameState != BJ_GamePlayStates.BJ_Betting) return false;
		if(getCurrentPlayerToDeal().isBetPlaced)
		{		
			getCurrentPlayerToDeal().moveTheBetPlacedToTheTable();
			mCurrentGameState = BJ_GamePlayStates.BJ_Dealing;
			// Fetching New Cards From Deck
			StartCoroutine("DealingCards");
			return true;
		}
		BJ_BottomBar.mInstance.showChips();
		return false;
	}
	
	IEnumerator DealingCards()
	{
		int i=0;
		int j=0;
		while(true)
		{
			yield return new WaitForSeconds(CasinoCard_Shoe.mIntance.CasinoCardDealSpeed_Min);
			if(i>=2) 
			{				
				makePlayerToDeal();
				break;
			} 
			if(j==0)	// Player
			{
				getCurrentPlayerToDeal().dealNewCard();
			}
			if(j >= 1)	// Dealer
			{				
				j=0; i++;
				BJ_DealerInstance.dealNewCard();
				continue;
			}
			j++;			
		}
	}
	
	// Start Payout
	void PayoutPlayer()
	{		
		getCurrentPlayerToDeal().payoutPlayer(BJ_DealerInstance.cardsCount, BJ_DealerInstance.isBlackJack, BJ_DealerInstance.isBusted);
		//stackAllCards();
	}
	
	// 
	public void setTotalBet()
	{
		BJ_BottomBar.mInstance.setTotalBet(getCurrentPlayerToDeal().totalBet);
	}
	public void setTotalWin()
	{
		BJ_BottomBar.mInstance.setTotalWin(getCurrentPlayerToDeal().totalWin);
	}
}
