using UnityEngine;
using System.Collections;

public class CasinoCardDeckDemo : MonoBehaviour {
	
	public GameObject DeckOfCardsPrefab = null;
	private GameObject DeckOfCards = null;
	private GameObject[] arrCards = null;
	// Use this for initialization
	void Start () { loadNewCards(); }	
	// Update is called once per frame
	void Update () { }
	
	void createCards()
	{
		arrCards = new GameObject[52];
		for(int i=0; i<arrCards.Length; i++)
			arrCards[i] = null;		
		
		float gapWidth = 5.0f;
		float gapHeight = 7.0f;
		for(int i=0; i<arrCards.Length; i++){
			arrCards[i] = DeckOfCards.GetComponent<CasinoCardDeck>().createAndGetNewCardFromTheDeck(new Vector3(gapWidth*(i%13),-Mathf.FloorToInt(i/13.0f)*gapHeight,0));
			arrCards[i].GetComponent<CasinoCard>().flipCardVertically();}
	}
	
	void loadNewCards()
	{
		destroyPreviousDeck();	
		
		DeckOfCards = (GameObject)Instantiate(DeckOfCardsPrefab);
		DeckOfCards.GetComponent<CasinoCardDeck>().setDeckData(2, true, 1);
		createCards();
	}
	
	void destroyPreviousDeck()
	{
		if(DeckOfCards == null) return;
		foreach(GameObject CardObject in arrCards)
			if(CardObject!=null) DestroyObject(CardObject);
		arrCards = null;
		DestroyObject(DeckOfCards);
		DeckOfCards = null;
	}
	
	void shuffleCards(CasinoCardsDeckShuffler.TypeOfShuffler shuffler)
	{	
		DeckOfCards.GetComponent<CasinoCardDeck>().shufflerType = shuffler;
		DeckOfCards.GetComponent<CasinoCardDeck>().shuffleTheDeck();
		foreach(GameObject CardObject in arrCards)
			if(CardObject!=null) DestroyObject(CardObject);
		createCards();
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(0,0,100,50), "Reload"))
			loadNewCards();
		if(GUI.Button(new Rect(110,0,100,50), ((CasinoCardsDeckShuffler.TypeOfShuffler)0)+""))
			shuffleCards(CasinoCardsDeckShuffler.TypeOfShuffler.Linear);
		if(GUI.Button(new Rect(220,0,100,50), ((CasinoCardsDeckShuffler.TypeOfShuffler)1)+""))
			shuffleCards(CasinoCardsDeckShuffler.TypeOfShuffler.FisherYates);
		if(GUI.Button(new Rect(330,0,100,50), ((CasinoCardsDeckShuffler.TypeOfShuffler)2)+""))
			shuffleCards(CasinoCardsDeckShuffler.TypeOfShuffler.InsideOut);
	}
}
