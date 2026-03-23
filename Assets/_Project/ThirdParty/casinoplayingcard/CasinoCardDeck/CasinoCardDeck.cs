using UnityEngine;
using System.Collections;

public class CasinoCardDeck : MonoBehaviour {
	
	public GameObject cardPrefab = null;
	public CasinoCardsDeckShuffler.TypeOfShuffler shufflerType = CasinoCardsDeckShuffler.TypeOfShuffler.FisherYates;
	public int deckNo = 1;
	public int NoOfCovers = 1;
	public int coverIndex = 0;
	public bool createRandomCover = false;
	public float CasinoCardFlipSpeed = 0.2f;
	public float CasinoCardScaleRatio = 1.0f;
	public string CasinoCard_LayerName = "Cards";
	private int totalCards = 52;
	private int currentCardIndex = 0;
	int[] playingCards = null;
	private bool isInitialized = false;
	
	// Use this for initialization
	void Start () { 
		if(isInitialized) return;
		isInitialized = true;
		createCards();
		if(createRandomCover) 
			coverIndex = Random.Range(0, NoOfCovers); 
		gameObject.layer = LayerMask.NameToLayer(CasinoCard_LayerName);
	}		
	void createCards()
	{
		currentCardIndex = 0;
		playingCards = new int[totalCards];
		for(int i=0; i<totalCards; i++)
			playingCards[i] = i;
	}
	public void setDeckData(int noOfCovers, bool createRandomCover, int deckNo)
	{
		isInitialized = false;
		this.NoOfCovers = noOfCovers;
		this.createRandomCover = createRandomCover;
		this.deckNo = deckNo;
		Start();
	}
	
	// Update is called once per frame
	void Update () { }
	
	// Shuffle
	public void shuffleTheDeck()
	{
		currentCardIndex = 0;
		playingCards = CasinoCardsDeckShuffler.shuffleTheDeckUsing(shufflerType, playingCards);
	}
	
	void shuffleAfterReachingEndOfTheDeck()
	{
		shuffleTheDeck();
	}
	
	// Getters
	public int getNewCardFromTheDeck()
	{
		if(currentCardIndex == totalCards) shuffleAfterReachingEndOfTheDeck();
		return playingCards[currentCardIndex++];
	}
	
	public CasinoCard.CardClass getCardClassFromCardIndex(int cardIndex)
	{
		return (CasinoCard.CardClass)(Mathf.FloorToInt(cardIndex/13.0f));
	}
	
	public CasinoCard.CardType getCardTypeFromCardIndex(int cardIndex)
	{
		return (CasinoCard.CardType)(cardIndex%13);
	}
	
	// Create New Card
	public GameObject createAndGetNewCardFromTheDeck(Vector3 position)
	{
		cardPrefab.GetComponent<CasinoCard>().mCardCoverIndex = coverIndex;
		GameObject cardObject = (GameObject)Instantiate(cardPrefab, position, Quaternion.identity);
		cardObject.transform.parent = transform;
		int newCard = getNewCardFromTheDeck();
		cardObject.GetComponent<CasinoCard>().mCardLayerName = CasinoCard_LayerName;
		cardObject.GetComponent<CasinoCard>().mCardFlipSpeed = CasinoCardFlipSpeed;
		cardObject.GetComponent<CasinoCard>().mCardScaleRatio = CasinoCardScaleRatio;
		cardObject.GetComponent<CasinoCard>().createCard(getCardClassFromCardIndex(newCard), getCardTypeFromCardIndex(newCard));
		return cardObject;
	}
}
