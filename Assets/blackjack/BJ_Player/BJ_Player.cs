using UnityEngine;
using System.Collections;

public class BJ_Player : MonoBehaviour {
	
	public Transform mDealerEndPosition = null;
	public Transform mPlayerEndPosition = null;
	public Transform mPlayerBetPosition = null;
	public Transform mInsurancePosition = null;
	public Transform mPlayerChipsPosition = null;
	public Transform mPlayerDoublePosition = null;
	public Transform mPlayerPayoutPosition = null;
		
	public bool isBetPlaced = false;
	public bool isInsured = false;
	public bool isSplitted = false;
		
	int dealerCardCount = 0;
	bool isDealerBlackjack = false;
	bool isDealerBusted = false;
	
	ArrayList prevBetsPlaced = null;
	ArrayList currentBetsPlaced = null;			
	ArrayList arrPlacedChips = null;
	ArrayList arrInsuredChips = null;	
	ArrayList arrPayoutChips = null;	
	
	ArrayList arrActivePlayers = null;	
	int mCurrentPlayerToDeal = 0;
	public float totalBet = 0.0f;
	public float totalWin = 0.0f;
	
	float gapBetweenPlayers = 10.0f;
	
	ArrayList arrPlayerBetChipsHolder = null;
	
	GameObject mPlayerBetChips = null;
	GameObject mDoubledPlaceHolder = null;
	GameObject mInsurancePlaceHolder = null;
	GameObject mPayoutPlaceHolder = null;
	
	// Use this for initialization
	void Start () {
		
	}
	
	public void createPlayer()
	{	
		float yPos = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(2.0f, 0.0f, -2.0f, 4.5f, 4.5f, 2.0f, 1.0f, 4.5f, 5.5f);
		mPlayerEndPosition.position -= new Vector3(0,yPos,0);
		mDealerEndPosition.position += new Vector3(0,yPos,0);
		mInsurancePosition.position += new Vector3(0,MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(1,1,Loading.IsAdsRemoved?1:0,2,3,Loading.IsAdsRemoved?2:1,1,3,4.0f),0);		
		transform.position += new Vector3(0,MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(0,0,0,1,1,0,0,1,2.0f),0);		
		
		createPlayerBetHolder();
		//mPlayerBetChips = CommonData.createEmptyGameObject("PlacedChipsHolder", gameObject, Vector3.zero);
		mDoubledPlaceHolder = CommonData.createEmptyGameObject("DoubledPlaceHolder", gameObject, Vector3.zero);
		mInsurancePlaceHolder = CommonData.createEmptyGameObject("InsurancePlaceHolder", gameObject, Vector3.zero);
		mPayoutPlaceHolder = CommonData.createEmptyGameObject("PayoutPlaceHolder", gameObject, Vector3.zero);
		
		// Testing
		mDoubledPlaceHolder.AddComponent<BoxCollider>();
		mInsurancePlaceHolder.AddComponent<BoxCollider>();
		mPayoutPlaceHolder.AddComponent<BoxCollider>();		
		// -------
		
		initializeChipsAnimations();
	}
	
	void createPlayerBetHolder()
	{
		if(arrPlayerBetChipsHolder == null) arrPlayerBetChipsHolder = new ArrayList();
		mPlayerBetChips = CommonData.createEmptyGameObject("PlacedChipsHolder", gameObject, Vector3.zero);
		arrPlayerBetChipsHolder.Add(mPlayerBetChips);
		mPlayerBetChips.AddComponent<BoxCollider>();
	}
	
	public void rebetTheChips()
	{
		if(prevBetsPlaced == null || prevBetsPlaced.Count == 0) return;
		isBetPlaced = true;
		mCurrentPlayerToDeal = 0;
		currentBetsPlaced = new ArrayList();
		currentBetsPlaced.AddRange(prevBetsPlaced);	
		arrangeChipsBasedOnChipValue();
	}
	
	public void placeBetsSelected()
	{
		if(BJ_GamePlayLayer.mInstance.mCurrentGameState != BJ_GamePlayStates.BJ_Betting) return;
		isBetPlaced = true;
		if(currentBetsPlaced == null) currentBetsPlaced = new ArrayList();
		currentBetsPlaced.Add(BJ_BottomBar.mInstance.getCurrentSelectedChipIndex());
		BJ_BottomBar.mInstance.enableButtonsToDeal();
	
		arrangeChipsBasedOnChipValue();
		BJ_GamePlayLayer.mInstance.setTotalBet();
	}
	
	public void doubleTheBetAndDeal()
	{
		getCurrentPlayerToDeal().doubleTheBetAndDeal();
		
		BJ_BottomBar.mInstance.updateChipsBalance(-Mathf.CeilToInt(totalBet));
		totalBet += totalBet;		
		
		// Adding the Chips for the Doubled Bet
		ArrayList tempArrOfChips = decodeTheChipsToFinerDenominationsAndStackAt(currentBetsPlaced, Vector3.zero, mDoubledPlaceHolder);
		moveTheDoublePlacedToTheTable();
		arrPlacedChips.AddRange(tempArrOfChips);	
	}
	
	public bool dealNewCard()
	{
		if(!isBetPlaced) return false;
		if(arrActivePlayers == null) createNewActivePlayer();
		for(; mCurrentPlayerToDeal< arrActivePlayers.Count; mCurrentPlayerToDeal++)
		{
			if(!getCurrentPlayerToDeal().isBlackJack)
			{
				getCurrentPlayerToDeal().dealNewCard();
				return true;
			}
		}
		return false;
	}
	
	void removePlacedChips(ArrayList placedChips)
	{
		if(placedChips == null) return;
		for(int i=0; i<placedChips.Count; i++)
			CommonData.destroyMyObject((GameObject)placedChips[i]);
		placedChips.Clear();
		placedChips = null;
	}
	
	public void removeAllPlacedChips()
	{
		removePlacedChips(arrPlacedChips);
		arrPlacedChips = null;
		removePlacedChips(arrPayoutChips);
		arrPayoutChips = null;
		removePlacedChips(arrInsuredChips);
		arrInsuredChips = null;
	}
	
	public void reloadPlayer()
	{
		removeAllActivePlayers();
		if(!isBetPlaced) prevBetsPlaced = new ArrayList();
		isBetPlaced = false;
		isInsured = false;
		isSplitted = false;
		currentBetsPlaced = null;
		totalBet = 0.0f;
		removeAllPlacedChips();
		initializeChipsAnimations();
		createPlayerBetHolder();
		moveTheBetPlacedToPlaceBet();
	}
	
	void removeAllActivePlayers()
	{
		if(arrActivePlayers == null) return;
		foreach(BJ_ActivePlayer activePlayer in arrActivePlayers)
			activePlayer.destroyActivePlayer();
		arrActivePlayers.Clear();
		arrActivePlayers = null;
		foreach(GameObject playerBetHolder in arrPlayerBetChipsHolder)
			DestroyObject(playerBetHolder);
		arrPlayerBetChipsHolder.Clear();
		arrPlayerBetChipsHolder = null;			
	}
	
	void createNewActivePlayer()
	{
		if(arrActivePlayers == null) arrActivePlayers = new ArrayList();
		mCurrentPlayerToDeal = 0;
		GameObject activePlayerObject = new GameObject();
		BJ_ActivePlayer activePlayer = activePlayerObject.AddComponent<BJ_ActivePlayer>();
		activePlayer.createTheActivePlayer(transform.position, this, totalBet);
		arrActivePlayers.Add(activePlayer);
	}
	
	public bool makePlayerToDeal()
	{
		if(!isBetPlaced) return false;
		prevBetsPlaced = new ArrayList();
		prevBetsPlaced.AddRange(currentBetsPlaced);
		totalWin = 0.0f;		
		mCurrentPlayerToDeal = 0;
		if(!getCurrentPlayerToDeal().isBlackJack)
		{
			getCurrentPlayerToDeal().makePlayerToDeal();
			return true;
		}
		return false;
	}
	
	public void moveCardsToStack()
	{
		foreach(BJ_ActivePlayer activePlayer in arrActivePlayers)
			activePlayer.moveCardsToStack();
	}
	
	public void makeNextPlayerToDeal()
	{
		mCurrentPlayerToDeal++;
		
		// Make the Current Player Active
		if(mCurrentPlayerToDeal != arrActivePlayers.Count)
		{
			foreach(BJ_ActivePlayer activePlayer in arrActivePlayers)
				activePlayer.moveThePlayerBy(new Vector3(gapBetweenPlayers,0,0));
			foreach(GameObject playerBetHolder in arrPlayerBetChipsHolder)
				playerBetHolder.transform.position += new Vector3(gapBetweenPlayers,0,0);
		}
		
		if(mCurrentPlayerToDeal == arrActivePlayers.Count)
			BJ_GamePlayLayer.mInstance.makeNextPlayerToDeal();
		else if(isSplitted)
			getCurrentPlayerToDeal().dealNewCard();
		else
			BJ_BottomBar.mInstance.enableAllButtons();
	}
	
	public void insureThePlayer()
	{
		isInsured = true;
		
		ArrayList arrOfInsuredChips = new ArrayList();
		decodeTheChipsValueIntoFinerDenomnations(totalBet/2.0f, out arrOfInsuredChips);
		createChipStackAt(arrOfInsuredChips, Vector3.zero, out arrInsuredChips, mInsurancePlaceHolder);
		moveTheInsuranceToTheTable();
		
		BJ_BottomBar.mInstance.updateChipsBalance(-Mathf.CeilToInt(totalBet/2.0f));
		totalBet += totalBet/2.0f;
	}
	
	public void splitTheCards()
	{			
		isSplitted = true;
		createNewActivePlayer();
		((BJ_ActivePlayer)arrActivePlayers[arrActivePlayers.Count-1]).addAlreadyFetchedCardTo(getCurrentPlayerToDeal().getTheSplitCard());
				
		BJ_BottomBar.mInstance.updateChipsBalance(-Mathf.CeilToInt(totalBet));
		totalBet += totalBet;
		
		//getCurrentPlayerToDeal().moveThePlayerBy(new Vector3(gapBetweenPlayers,0,0));
		getCurrentPlayerToDeal().dealNewCard();
		((BJ_ActivePlayer)arrActivePlayers[arrActivePlayers.Count-1]).moveThePlayerBy(-new Vector3(gapBetweenPlayers,0,0));
		
		createPlayerBetHolder();
		mPlayerBetChips.transform.position = mPlayerChipsPosition.position - new Vector3(gapBetweenPlayers, 0, 0);
		// Adding the Chips for the Split Bet
		ArrayList tempArrOfChips = decodeTheChipsToFinerDenominationsAndStackAt(currentBetsPlaced, Vector3.zero, mPlayerBetChips);
		arrPlacedChips.AddRange(tempArrOfChips);	
	}
	
	public void surrenderTheHand()
	{
		getCurrentPlayerToDeal().surrenderTheHand();
	}
		
	public bool isAllPlayersBustedOrBlackjacked()
	{
		foreach(BJ_ActivePlayer activePlayer in arrActivePlayers)
			if(!(activePlayer.isBusted || activePlayer.isBlackJack || activePlayer.isSurrender))
				return false;
		return true;
	}
	
	int getCasinoCardValueOf(GameObject cardObject) { return cardObject.GetComponent<CasinoCard>().getCardValue(); }
	public float getPrevBetAmount() { return getTheChipTotalValueFrom(prevBetsPlaced); }
	
	void payoutNextPlayer()
	{
		mCurrentPlayerToDeal++;
		
		if(mCurrentPlayerToDeal == arrActivePlayers.Count)
		{
			BJ_GamePlayLayer.mInstance.stackAllCards();
			return;
		}
		
		mPlayerBetChips = (GameObject)arrPlayerBetChipsHolder[mCurrentPlayerToDeal];
		
		if(mCurrentPlayerToDeal > 0)
		{
			// Make the Current Player Active
			if(mCurrentPlayerToDeal != arrActivePlayers.Count)
			{
				foreach(BJ_ActivePlayer activePlayer in arrActivePlayers)
					activePlayer.moveThePlayerBy(new Vector3(gapBetweenPlayers,0,0));
				foreach(GameObject playerBetHolder in arrPlayerBetChipsHolder)
					playerBetHolder.transform.position += new Vector3(gapBetweenPlayers,0,0);
			}
		}		
		
		getCurrentPlayerToDeal().payoutPlayer(dealerCardCount, isDealerBlackjack, isDealerBusted);
		totalWin = getCurrentPlayerToDeal().totalWin;
		
		removePlacedChips(arrPayoutChips);
		mPayoutPlaceHolder.transform.position = mDealerEndPosition.position;
		//Debug.Break();
		
		if(totalWin > 0.0f)
		{
			ArrayList arrofPayoutChips = new ArrayList();
			decodeTheChipsValueIntoFinerDenomnations(totalWin, out arrofPayoutChips);
			createChipStackAt(arrofPayoutChips, Vector3.zero, out arrPayoutChips, mPayoutPlaceHolder);
			moveThePayoutToThePlayer();
			
			if(getCurrentPlayerToDeal().isSurrender)
			{
				BJ_BottomBar.mInstance.updateChipsBalance((int)(totalWin));
				moveTheBetPlacedToTheDealer();
			}
			else		
			{
				moveTheBetPlacedToThePlayer();
				BJ_BottomBar.mInstance.updateChipsBalance((int)(totalBet+totalWin));
			}
			moveTheDoublePlacedToThePlayer();
				
			BJ_BottomBar.mInstance.startDgChipsAnimation();
		}
		else
		{
			if(getCurrentPlayerToDeal().isPushed)
			{
				moveTheBetPlacedToThePlayer();
				BJ_BottomBar.mInstance.updateChipsBalance((int)(totalBet));
			}
			else
			moveTheBetPlacedToTheDealer();
			moveTheDoublePlacedToTheDealer();
			moveTheInsuranceToTheDealer();
		}
		
		CCallFunc.createCallAfterDelay(payoutNextPlayer, 1.0f).runAction();
	}
	
	public void payoutPlayer(int dealerCardCount, bool isDealerBlackjack, bool isDealerBusted)
	{		
		this.dealerCardCount = dealerCardCount;
		this.isDealerBlackjack = isDealerBlackjack;
		this.isDealerBusted = isDealerBusted;
		
		mCurrentPlayerToDeal = -1;
		
		if(arrActivePlayers.Count > 1)
		{
			// Make the Current Player Active
			foreach(BJ_ActivePlayer activePlayer in arrActivePlayers)
				activePlayer.moveThePlayerBy(new Vector3(-gapBetweenPlayers*(arrActivePlayers.Count-1),0,0));
			foreach(GameObject playerBetHolder in arrPlayerBetChipsHolder)
				playerBetHolder.transform.position += new Vector3(-gapBetweenPlayers*(arrActivePlayers.Count-1),0,0);		
		}
		
		// Achievement
		bool isAllPlayersBlackJack = true;
		foreach(BJ_ActivePlayer activePlayer in arrActivePlayers)
			if(!activePlayer.isBlackJack) isAllPlayersBlackJack = false;
		if(isAllPlayersBlackJack && arrActivePlayers.Count > 1)
			BJ_AchievementHandler.mInstance.BlindSplitDone();
		// ---------
				
		payoutNextPlayer();
	}
	
	BJ_ActivePlayer getCurrentPlayerToDeal() { return (BJ_ActivePlayer)arrActivePlayers[mCurrentPlayerToDeal]; }
		
	
	// Arrange chips in Smaller to Larger Approach
	// Replace the sum of smaler denominations into immediate a larger one if possible	
	public static void decodeTheChipsArrayIntoFinerDenomnations(ArrayList arrCurrentChipsArray, out ArrayList arrFinerDenominationChips)
	{
		Debug.Log("Start Decoding The Chips");
		Debug.Log("No of Chips : "+arrCurrentChipsArray.Count);
		arrFinerDenominationChips = new ArrayList();
		arrFinerDenominationChips.AddRange(arrCurrentChipsArray.GetRange(0, arrCurrentChipsArray.Count));
		
//		return;
//		
//		// Getting Only the current 5 Chips
//		float[] arrChipValues = new float[5];// BJ_BottomBar.mInstance.mArrChipValues;
//		for(int i=0; i<5; i++)
//			arrChipValues[i] = BJ_BottomBar.mInstance.mArrChipValues[i+TableHandler.startChipIndex];
//		
//		
//		arrFinerDenominationChips.Sort();
//		//bool isChanged = true;		
//		ArrayList tempArrChipsToDecode = new ArrayList();		
//		
//		//for(int iterator =0; iterator < arrChipValues.Length; iterator++)
//		//while(isChanged)
//		{
//			//isChanged = false;	
//			tempArrChipsToDecode = new ArrayList();
//			tempArrChipsToDecode.AddRange(arrFinerDenominationChips.GetRange(0, arrFinerDenominationChips.Count));
//			arrFinerDenominationChips = new ArrayList();
//
//			for(int startIndex = 0; startIndex < tempArrChipsToDecode.Count; )
//			{
//				int currentChipValueIndex = (int)tempArrChipsToDecode[startIndex];
//#if UNITY_FLASH
//			int endIndex = 0;
//#else
//				int endIndex = tempArrChipsToDecode.LastIndexOf(currentChipValueIndex);
//#endif
//				for(int nextChipValueIndex = currentChipValueIndex+1; nextChipValueIndex <= arrChipValues.Length; nextChipValueIndex++)
//				{
//					if(nextChipValueIndex >= arrChipValues.Length)
//					{
//						Debug.Log("Adding the HighestDenomination : "+startIndex+" Count : "+(tempArrChipsToDecode.Count - startIndex));
//						arrFinerDenominationChips.AddRange(tempArrChipsToDecode.GetRange (startIndex, tempArrChipsToDecode.Count - startIndex));
////						if(arrFinerDenominationChips.Count != tempArrChipsToDecode.Count) 
////						isChanged = true;
//						break;
//					}
//					float currentChipValue = arrChipValues[currentChipValueIndex];
//					float nextChipValue = arrChipValues[nextChipValueIndex];
//								
//					if(startIndex != -1 && endIndex != -1)
//					{
//						float chipValue = (endIndex+1-startIndex) * arrChipValues[currentChipValueIndex];
//						float subChipValue = chipValue % nextChipValue;
//						int noOfNextDenominationsCanBeAdded = Mathf.FloorToInt(chipValue/nextChipValue);
//						int noOfCurrDenominationsCanBeAdded = Mathf.FloorToInt(subChipValue/currentChipValue);
//						
//						Debug.Log("ChipValue : "+chipValue);
//						Debug.Log("NextValue : "+nextChipValue);
//						Debug.Log("SubChipValue : "+subChipValue);
//						Debug.Log("noOfCurrDenominationsCanBeAdded : "+noOfCurrDenominationsCanBeAdded+" ChipIndex : " + currentChipValueIndex);
//						Debug.Log("noOfNextDenominationsCanBeAdded : "+noOfNextDenominationsCanBeAdded+" ChipIndex : " + nextChipValueIndex);
//						if(noOfNextDenominationsCanBeAdded >= 1 && subChipValue%currentChipValue == 0)
//						{
//							for(int i=0; i<noOfCurrDenominationsCanBeAdded; i++)
//								arrFinerDenominationChips.Add(currentChipValueIndex);
//							for(int i=0; i<noOfNextDenominationsCanBeAdded; i++)
//								arrFinerDenominationChips.Add(nextChipValueIndex);
//							//isChanged = true;
//							break;
//						}
//					}
//					else if(startIndex != -1)
//					{
//						Debug.Log("Adding the HighestDenomination : "+startIndex+" Count : "+(tempArrChipsToDecode.Count - startIndex-1));
////						arrFinerDenominationChips.AddRange(tempArrChipsToDecode.GetRange (startIndex, tempArrChipsToDecode.Count - startIndex-1));
////						if(arrFinerDenominationChips.Count != tempArrChipsToDecode.Count) 
////						isChanged = true;
//					}
//				}
//				startIndex += endIndex+1;
//			}
//		}
//		
//		if(arrFinerDenominationChips.Count == 0)
//			arrFinerDenominationChips.AddRange(tempArrChipsToDecode.GetRange(0, tempArrChipsToDecode.Count));
//			
//		
//		Debug.Log("Final Denominations Count : "+arrFinerDenominationChips.Count);
//		Debug.Log("End Decoding The Chips");		
	}
	
	// Arrange chips in Larger to Smaller Approach
	// Add all the chips value and decode the chips from larger to smaller	
	public static void decodeTheChipsValueIntoFinerDenomnations(float chipValue, out ArrayList arrFinerDenominationChips)
	{
		arrFinerDenominationChips = new ArrayList();
		// Decoding the Total Cost into smaller ones
		float[] arrChipValues = BJ_BottomBar.mInstance.mArrChipValues;
		int maxDenomionationPossible = arrChipValues.Length-1;
		float currentValueToDecode = chipValue;
		while(currentValueToDecode > 0.0f && maxDenomionationPossible >= 0)
		{			
			if(arrChipValues[maxDenomionationPossible] <= currentValueToDecode)
			{
				currentValueToDecode -= arrChipValues[maxDenomionationPossible];
				arrFinerDenominationChips.Add(maxDenomionationPossible);
			}
			else
			{
				maxDenomionationPossible--;
			}
		}	
		Debug.Log("Remaining Value Discarded : "+currentValueToDecode);
	}
	
	float getTheChipTotalValueFrom(ArrayList arrChipsIndex)
	{
		float[] arrChipValues = BJ_BottomBar.mInstance.mArrChipValues;
		arrChipsIndex.Sort();

		float chipTotalValue = 0.0f;
		for(int startIndex = 0; startIndex < arrChipsIndex.Count; )
		{
			int endIndex = 0;
#if UNITY_FLASH
#else
			endIndex = arrChipsIndex.LastIndexOf(arrChipsIndex[startIndex]);
#endif	
			if(startIndex != -1 && endIndex != -1)
				chipTotalValue += (endIndex+1-startIndex) * arrChipValues[(int)arrChipsIndex[startIndex]];			
			startIndex = endIndex+1;	
		}
		return chipTotalValue;
	}
	
	public void createChipStackAt(ArrayList arrOfChipIndex, Vector3 position, out ArrayList arrChipsPlaced, GameObject mChipsHolder)
	{		
		// Creating new Chips
		LinkedSpriteManager spriteManager = BJ_GamePlayLayer.mInstance.mChipsSpriteManager;
		SpriteAtlasDataHandler spriteAtlasDataHandler = BJ_GamePlayLayer.mInstance.mIChipsSpriteAtlasDataHandler;
				
		arrChipsPlaced = new ArrayList();
		int zOrderDec=arrOfChipIndex.Count;
		foreach(int chipIndex in arrOfChipIndex)
		{
			string chipName = "Chip_"+chipIndex;
			GameObject newChipObject = CommonData.createGameObject(chipName, mChipsHolder, position + new Vector3(0,zOrderDec*0.2f,0), spriteManager, spriteAtlasDataHandler, chipName+".png", 10+(zOrderDec--));
			newChipObject.layer = gameObject.layer;
			float angleGap = 10.0f;
			newChipObject.transform.Rotate(new Vector3(0,0,Random.Range(-angleGap,angleGap)));
			arrChipsPlaced.Add(newChipObject);
		}		
	}
	
	ArrayList decodeTheChipsToFinerDenominationsAndStackAt(ArrayList arrChipsValue, Vector3 position, GameObject mChipsHolder)
	{		
		// Temp Array for creating Chip Images
		ArrayList tempArrOfChipsPlaced = new ArrayList();		
		// Decoding the Total Cost into smaller ones
		decodeTheChipsArrayIntoFinerDenomnations(arrChipsValue, out tempArrOfChipsPlaced);
		tempArrOfChipsPlaced.Sort ();
		
		// Array of ChipObjects to return the previously added chips
		ArrayList arrChipObjectsCreated = null;
		// Creating the Stack
		createChipStackAt(tempArrOfChipsPlaced, position, out arrChipObjectsCreated, mChipsHolder);
		return arrChipObjectsCreated;
	}
	
	void arrangeChipsBasedOnChipValue()
	{		
		// Calculating the total bet
		// Temp Array for creating Chip Images
		totalBet = getTheChipTotalValueFrom(currentBetsPlaced);	
		
		// Removing the previously added chips
		removePlacedChips(arrPlacedChips);
		arrPlacedChips = decodeTheChipsToFinerDenominationsAndStackAt(currentBetsPlaced, Vector3.zero, mPlayerBetChips); 
	}
	
	
	// Chips Animation
	public void moveTheHolderTo(GameObject mHolder, Vector2 position, EaseType easeActionType, bool isEaseAction, float duration)
	{
		if(mHolder.GetComponent<CMoveTo>() != null)
			DestroyImmediate(mHolder.GetComponent<CMoveTo>());
		if(mHolder.GetComponent<CEaseExponential>() != null)
			DestroyImmediate(mHolder.GetComponent<CEaseExponential>());
		
		CMoveTo move = mHolder.AddComponent<CMoveTo>();
		CEaseExponential ease = mHolder.AddComponent<CEaseExponential>();
		
		move.actionWith(mHolder, position, duration);
		ease.actionWithAction(move, easeActionType);
		
		if(!isEaseAction) move.runAction();
		else		ease.runAction();
	}
	
	// Initialize
	void initializeChipsAnimations()
	{
		moveTheBetPlacedToPlaceBet();
		moveThePayoutToTheDealer();
		moveTheInsuranceToThePlayer();
		moveTheDoublePlacedToThePlayer();
	}
	
	// BetPlaced
	public void moveTheBetPlacedToTheTable()
	{
		moveTheHolderTo(mPlayerBetChips, mPlayerChipsPosition.position, EaseType.EaseInOut, true, 0.5f);
	}
	
	void moveTheBetPlacedToThePlayer()
	{
		moveTheHolderTo(mPlayerBetChips, mPlayerEndPosition.position, EaseType.EaseOut, true, 1.0f);
	}
	
	void moveTheBetPlacedToTheDealer()
	{
		moveTheHolderTo(mPlayerBetChips, mDealerEndPosition.position, EaseType.EaseOut, true, 1.0f);
	}
	
	void moveTheBetPlacedToPlaceBet()
	{
		moveTheHolderTo(mPlayerBetChips, mPlayerBetPosition.position, EaseType.EaseInOut, false, 0.1f);
	}	
	
	// Double Bet
	public void moveTheDoublePlacedToTheTable()
	{
		moveTheHolderTo(mDoubledPlaceHolder, mPlayerDoublePosition.position, EaseType.EaseOut, true, 1.0f);
	}
	
	void moveTheDoublePlacedToThePlayer()
	{
		moveTheHolderTo(mDoubledPlaceHolder, mPlayerEndPosition.position, EaseType.EaseOut, true, 1.0f);
	}
	
	void moveTheDoublePlacedToTheDealer()
	{
		moveTheHolderTo(mDoubledPlaceHolder, mDealerEndPosition.position, EaseType.EaseOut, true, 1.0f);
	}
	
	// Insurance
	public void moveTheInsuranceToTheTable()
	{
		moveTheHolderTo(mInsurancePlaceHolder, mInsurancePosition.position, EaseType.EaseOut, true, 1.0f);
	}
	
	void moveTheInsuranceToThePlayer()
	{
		moveTheHolderTo(mInsurancePlaceHolder, mPlayerEndPosition.position, EaseType.EaseOut, true, 1.0f);
	}
	
	public void moveTheInsuranceToTheDealer()
	{
		moveTheHolderTo(mInsurancePlaceHolder, mDealerEndPosition.position, EaseType.EaseOut, true, 1.0f);
	}
	
	// Payout
	public void moveThePayoutToTheTable()
	{
		moveTheHolderTo(mPayoutPlaceHolder, mPlayerPayoutPosition.position, EaseType.EaseOut, true, 0.5f);
	}
	
	void moveThePayoutToThePlayer()
	{
		moveTheHolderTo(mPayoutPlaceHolder, mPlayerEndPosition.position, EaseType.EaseOut, true, 1.0f);
	}
	
	void moveThePayoutToTheDealer()
	{
		moveTheHolderTo(mPayoutPlaceHolder, mDealerEndPosition.position, EaseType.EaseOut, true, 0.1f);
	}	
}
