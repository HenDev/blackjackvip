using UnityEngine;
using System.Collections;

public class CasinoCard_Shoe : MonoBehaviour {
		
	public static CasinoCard_Shoe mIntance = null;
	public CasinoCard_ShoeMachine casinoCardShoeMachine = null;
	public Transform casinoCardShoeStack = null;
	public GameObject CasinoCard_ShoeStackCover = null;
	public GameObject CasinoCard_ShoeMachineCover = null;
	public GameObject DeckOfCardsPrefab = null;
	public CasinoCardsDeckShuffler.TypeOfShuffler deckShuffleType = CasinoCardsDeckShuffler.TypeOfShuffler.FisherYates;
	public int NoOfDecks = 1;
	public int ShuffleShoeBeforeCards = 1; 			// No of Cards before the shoe needs to be shuffled
	
	public string CardDeckParameters = "CardDeckParameters";
	public int NoOfCovers = 1;
	public bool createRandomCover = false;
	public float CasinoCardScaleRatio = 1.0f;
	public float CasinoCardFlipSpeed = 0.2f;
	
	public float CasinoCardDealSpeed_Min = 0.0f;
	public float CasinoCardDealSpeed_Max = 0.0f;
	
	public string CasinoCard_LayerName = "Cards";
	
	public string CardActionParameters = "CardActionParameters";	
	public EaseActionType CasinoCardDealActionType = EaseActionType.EaseNone;
	public EaseType CasinoCardDealAction = EaseType.EaseIn;		
	public EaseActionType CasinoCardStackActionType = EaseActionType.EaseNone;
	public EaseType CasinoCardStackAction = EaseType.EaseIn;
	
	private GameObject[] arrDecksOfCards = null;
	private int[] arrIndexOfDecksToFetchCard = null;
	
	private int mCurrentCardIndex = 0;
	private int TotalCardsInADeck = 52;	
	
	
	// Use this for initialization
	void Start () { 		init ();		
	}
	
	void init()
	{
		mIntance = this;
		if(NoOfDecks < 1) NoOfDecks = 1;	// Atleast one Deck of cards is needed
		ShuffleShoeBeforeCards = Mathf.Clamp(ShuffleShoeBeforeCards, 10, ShuffleShoeBeforeCards); // Min shuffle should happen before 10 Cards
		arrDecksOfCards = new GameObject[NoOfDecks];
		arrIndexOfDecksToFetchCard = new int[NoOfDecks * TotalCardsInADeck];
		for(int i=0; i<NoOfDecks; i++)
		{
			arrDecksOfCards[i] = (GameObject)Instantiate(DeckOfCardsPrefab);
			//arrDecksOfCards[i].transform.parent = transform;
			arrDecksOfCards[i].GetComponent<CasinoCardDeck>().CasinoCard_LayerName = CasinoCard_LayerName;
			arrDecksOfCards[i].GetComponent<CasinoCardDeck>().shufflerType = deckShuffleType;
			arrDecksOfCards[i].GetComponent<CasinoCardDeck>().CasinoCardFlipSpeed = CasinoCardFlipSpeed;
			arrDecksOfCards[i].GetComponent<CasinoCardDeck>().CasinoCardScaleRatio = CasinoCardScaleRatio;
			arrDecksOfCards[i].GetComponent<CasinoCardDeck>().setDeckData(NoOfCovers, createRandomCover, i+1);
			for(int j=0; j<TotalCardsInADeck; j++)
				arrIndexOfDecksToFetchCard[i*TotalCardsInADeck+j] = (i);
		}
		shuffleTheShoe();
	}
	
	// Update is called once per frame
	void Update () {
		CardDeckParameters = "CardDeckParameters";
		CardActionParameters = "CardActionParameters";
	}
	
	public void moveCardToTheStack(GameObject cardObject)
	{		
		cardObject.GetComponent<CasinoCard>().flipCardVertically();		
		// Move Action
		float duraion = CasinoCardDealSpeed_Min - CasinoCardFlipSpeed;
		
		CDelayTime delay = cardObject.AddComponent<CDelayTime>();
		CMoveTo move = cardObject.AddComponent<CMoveTo>();
		CCallFunc call = cardObject.AddComponent<CCallFunc>();
		CSequence seq = cardObject.AddComponent<CSequence>();
		
		delay.actionWithDuration(CasinoCardFlipSpeed);
		move.actionWith(cardObject, casinoCardShoeStack.position, duraion);
		call.actionWithCallBack(setStackCover);
		seq.actionWithActions(delay, addEaseActionToStackAction(move), call);		
		seq.runAction();
	}
	
	// Getters
	GameObject getNewCardFromTheShoe()
	{
		GameObject cardObject = null;
		int deckIndex = arrIndexOfDecksToFetchCard[mCurrentCardIndex++];
		cardObject = arrDecksOfCards[deckIndex].GetComponent<CasinoCardDeck>().createAndGetNewCardFromTheDeck(casinoCardShoeMachine.mShoeCardInitPosition.position);
		cardObject.transform.Rotate(new Vector3(0,0,60));
		setMachineCover();
		return cardObject;
	}
	
	public GameObject dealACardTo(Vector3 position, CCallFunc.CallBackWithSender onDealCompleted)
	{
		GameObject cardObject = getNewCardFromTheShoe();
		// MoveAction
		float duration = Random.Range(CasinoCardDealSpeed_Min, CasinoCardDealSpeed_Max);
		
		CMoveTo moveOut = cardObject.AddComponent<CMoveTo>();
		CMoveTo move = cardObject.AddComponent<CMoveTo>();
		CRotateTo rotate = cardObject.AddComponent<CRotateTo>();		
		CSpawnAction spawn = cardObject.AddComponent<CSpawnAction>();		
		CCallFunc call = cardObject.AddComponent<CCallFunc>();
		CSequence seq = cardObject.AddComponent<CSequence>();
		
		moveOut.actionWith(cardObject, casinoCardShoeMachine.mShoeCardEndPosition.position, 0.05f);
		duration -= 0.05f;
		move.actionWith(cardObject, position, duration);		
		rotate.actionWith(cardObject, Vector3.zero, duration);
		spawn.actionWithActions(addEaseActionToDealAction(move), addEaseActionToDealAction(rotate));
		call.actionWithCallBack(onDealCompleted);		
		//seq.actionWithActions(addEaseActionToAction(moveOut, EaseActionType.EaseExponential, EaseType.EaseInOut), spawn, call);
		seq.actionWithActions(moveOut, spawn, call);
		seq.runAction();
		
		return cardObject;
	}
	
	// Adding Ease Actions
	CAction addEaseActionToAction(CAction action, EaseActionType easeActionType, EaseType easeType)
	{		
		CAction easeAction = null;
		switch(easeActionType)
		{
			case EaseActionType.EaseNormal:
				easeAction = action.gameObject.AddComponent<CEaseNone>();
				((CEaseNone)easeAction).actionWithAction(action);
				break;
			case EaseActionType.EaseCubic:
				easeAction = action.gameObject.AddComponent<CEaseCubic>();
				((CEaseCubic)easeAction).actionWithAction(action,easeType);
				break;
			case EaseActionType.EaseExponential:
				easeAction = action.gameObject.AddComponent<CEaseExponential>();
				((CEaseExponential)easeAction).actionWithAction(action,easeType);
				break;
			case EaseActionType.EaseElastic:
				easeAction = action.gameObject.AddComponent<CEaseElastic>();
				((CEaseElastic)easeAction).actionWithAction(action,easeType);
				break;
			default:
				easeAction = action;
				break;
		}		
		return easeAction;
	}
	
	CAction addEaseActionToDealAction(CAction action)
	{	
		return addEaseActionToAction(action, CasinoCardDealActionType, CasinoCardDealAction);
	}
		
	CAction addEaseActionToStackAction(CAction action)
	{
		return addEaseActionToAction(action, CasinoCardStackActionType, CasinoCardStackAction);
	}
	
	// Shuffle The Shoe
	// Shuffling the decks seperately and doing a seperate shuffle with the deck index
		// the new card will be fetched from the deck based on the deck index
	public void shuffleTheShoe()
	{		
		initCovers();
		mCurrentCardIndex = 0;
		arrIndexOfDecksToFetchCard = CasinoCardsDeckShuffler.shuffleTheDeckUsing(deckShuffleType, arrIndexOfDecksToFetchCard);
		foreach(GameObject _deck in arrDecksOfCards)
			_deck.GetComponent<CasinoCardDeck>().shuffleTheDeck();
		setMachineCover();
	}
	
	public bool checkIfShuffleNeeded()
	{
		if((arrIndexOfDecksToFetchCard.Length - mCurrentCardIndex) <= ShuffleShoeBeforeCards)
		{
			Debug.Log("shuffleTheShoe");
			shuffleTheShoe();
			return true;
		}
		return false;
	}
	
	
	// Changing the covers in Stack and Machine
	void initCovers()
	{
		Vector2 textureSize = new Vector2(1.0f/NoOfCovers, 1.0f);
		CasinoCard_ShoeMachineCover.GetComponent<Renderer>().material.mainTextureScale = textureSize;
		CasinoCard_ShoeStackCover.GetComponent<Renderer>().material.mainTextureScale = textureSize;
		CasinoCard_ShoeMachineCover.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(1.0f, 1.0f);
		CasinoCard_ShoeStackCover.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(1.0f, 1.0f);
	}
	
	void setMachineCover()
	{
		int coverIndex = arrDecksOfCards[arrIndexOfDecksToFetchCard[mCurrentCardIndex]].GetComponent<CasinoCardDeck>().coverIndex;
		Vector2 textureSize = CasinoCard_ShoeMachineCover.GetComponent<Renderer>().material.mainTextureScale;
		CasinoCard_ShoeMachineCover.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(coverIndex*textureSize.x, 0.0f);
	}
	
	void setStackCover()
	{		
		if(mCurrentCardIndex < 2) return;
		int ShoeCardIndex = mCurrentCardIndex-2;
		int DeckIndex = arrIndexOfDecksToFetchCard[ShoeCardIndex];
		int CoverIndex = arrDecksOfCards[DeckIndex].GetComponent<CasinoCardDeck>().coverIndex;
		Vector2 textureSize = CasinoCard_ShoeStackCover.GetComponent<Renderer>().material.mainTextureScale;
		CasinoCard_ShoeStackCover.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(CoverIndex*textureSize.x, 0.0f);
	}
}
