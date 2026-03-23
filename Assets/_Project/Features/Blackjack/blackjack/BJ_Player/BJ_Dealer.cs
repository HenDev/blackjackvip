using UnityEngine;
using System.Collections;

public class BJ_Dealer : BJ_CardsDealSpot {
	
	
	// Use this for initialization
	void Start () {
	}
	
	public void createDealer()
	{			
		if(!Loading.IsAdsRemoved)
		{
			InitPosition = transform.localPosition + new Vector3(0, MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(0.0f, -2.0f, -3.0f, 0.0f, 3.0f, 0.0f, -1.0f, 3.0f, 4.5f), 0);
			transform.position += new Vector3(0, MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(0.0f, -2.0f, -3.0f, 0.0f, 3.0f, 0.0f, -1.0f, 3.0f, 4.5f), 0);
		}
		else
		{
			InitPosition = transform.localPosition + new Vector3(0, MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(0.0f, 0.0f, 0.0f, 4.0f, 4.0f, 2.0f, 0.0f, 4.0f, 5.5f), 0);
			transform.position += new Vector3(0, MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(0.0f, 0.0f, 0.0f, 4.0f, 4.0f, 2.0f, 0.0f, 4.0f, 5.5f), 0);
		}		
		
		createCardCountDisplay();
		mCardCountLabel.setPosition(new Vector3(0.0f,-3.0f,0));
	}
	
	protected override void onCardDealt (GameObject sender)
	{
		if(arrFetchedCards.Count < 2 || BJ_GamePlayLayer.mInstance.mCurrentGameState == BJ_GamePlayStates.BJ_DealersTurn)
		{		
			sender.transform.position += new Vector3(0,0,-arrFetchedCards.Count);
			sender.GetComponent<CasinoCard>().flipCardVertically();
			cardsCount = getCardsCount();
		}
		else if(arrFetchedCards.Count == 2 && cardsCount == 11 && !BJ_GamePlayLayer.mInstance.getCurrentPlayerToDeal().isAllPlayersBustedOrBlackjacked())
		{
			BJ_GamePlayLayer.mInstance.makePlayerToBetInsurance();
		}
	}
	
	protected override bool checkForGameEndConditions ()
	{
		if((base.checkForGameEndConditions ()) || (cardsCount >= 17 && cardsCount <= 21) || 
			BJ_GamePlayLayer.mInstance.player.isAllPlayersBustedOrBlackjacked())
		{
			Debug.Log("Dealer Won");
			StopCoroutine("dealCardsForDealer");
			BJ_GamePlayLayer.mInstance.makeNextPlayerToDeal();
		}
		else if(arrFetchedCards.Count == 2)
		{			
			//Debug.Log("DEalersTurn");
			StartCoroutine("dealCardsForDealer");
		}
		return true;
	}
	
//	protected override Vector3 getNewCardPosition ()
//	{
//		float gapWidth = 2.5f;
//		return new Vector3(-gapWidth/2.0f + gapWidth * arrFetchedCards.Count,0,0);
//	}
	
	public void startDealingForDealerTurn()
	{
		BJ_GamePlayLayer.mInstance.mCurrentGameState = BJ_GamePlayStates.BJ_DealersTurn;
		BJ_BottomBar.mInstance.disableAllButtons();
		onCardDealt((GameObject)arrFetchedCards[1]);
	}
	
	public void makeDealerCheckForBlackJack()
	{
		if(((GameObject)arrFetchedCards[1]).GetComponent<CasinoCard>().getCardValue() >= 10)
		{
			startDealingForDealerTurn();
			
			// Achievement
			if(BJ_GamePlayLayer.mInstance.player.isInsured)
				BJ_AchievementHandler.mInstance.InsuranceClaimerDone();
			// -----------
		}
		else
		{
			BJ_GamePlayLayer.mInstance.collectAllInsuranceMoney();
			BJ_GamePlayLayer.mInstance.makePlayerToDeal();
		}
		
	}
	
	IEnumerator dealCardsForDealer()
	{
		while(true)
		{
			yield return new WaitForSeconds(CasinoCard_Shoe.mIntance.CasinoCardDealSpeed_Max);
			if(cardsCount >= 17 && cardsCount < 21) 
				break;
			dealNewCard();	
		}
	}
}
