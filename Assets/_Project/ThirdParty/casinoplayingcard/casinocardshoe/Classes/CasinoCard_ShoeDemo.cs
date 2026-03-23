using UnityEngine;
using System.Collections;

public class CasinoCard_ShoeDemo : MonoBehaviour {

	public CasinoCard_Shoe CasinoCard_Shoe = null;
	public int noOfDecks = 1;
	
	private GameObject[] arrCurrentFetchedCards = null;
	private GameObject[] arrNewCardsFetched = null;
	private bool isFetchingCards = false;
	private bool isShuffled = true;
	
	private bool[] arrItemToggle = new bool[5];
	// Use this for initialization
	void Start () {	
			StartCoroutine("removeShuffled");
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	// Remove Cards
	void removeAlreadyFetchedCards()
	{
		if(arrCurrentFetchedCards == null) return;
		foreach(GameObject cardObject in arrCurrentFetchedCards)
			DestroyObject(cardObject);
		arrCurrentFetchedCards = null;
	}
	
	void assignNewCardsToBeFetched()
	{
		if(!isFetchingCards) return;
		removeAlreadyFetchedCards();
		arrCurrentFetchedCards = new GameObject[arrNewCardsFetched.Length];
		for(int i=0; i< arrNewCardsFetched.Length; i++)
			arrCurrentFetchedCards[i] = arrNewCardsFetched[i];
		arrNewCardsFetched = null;
		isFetchingCards = false;
	}
	
	// Buttons
	void reloadDemo()
	{
		removeAlreadyFetchedCards();
		CasinoCard_Shoe.shuffleTheShoe();
	}
	
	void dealNewCards(int count)
	{
		isFetchingCards = true;
		
		// Moving Fetched Cards To Stack
		if(arrCurrentFetchedCards != null)
		foreach(GameObject cardObject in arrCurrentFetchedCards)
			CasinoCard_Shoe.moveCardToTheStack(cardObject);
		
		// Checking if the Shuffling is needed
		if(CasinoCard_Shoe.checkIfShuffleNeeded())
		{
			isShuffled = true;
			StartCoroutine("removeShuffled");
		}
		
		// Fetching New Cards From Deck
		float gap = 5.0f * CasinoCard_Shoe.CasinoCardScaleRatio;
		arrNewCardsFetched = new GameObject[count];
		for(int i=0; i<count; i++)
		{
			float xPos = -((float)(count-1))/2.0f*gap + (gap*i);
			GameObject cardObject = CasinoCard_Shoe.dealACardTo(new Vector3(xPos,0,0), onCardDealed);
			arrNewCardsFetched[i] = cardObject;			
		}
	}
	
	void onCardDealed(GameObject sender)
	{
		sender.GetComponent<CasinoCard>().flipCardVertically();
		assignNewCardsToBeFetched();
	}	
	
	IEnumerator removeShuffled()
	{
		while(true)
		{
			yield return new WaitForSeconds(3.0f);
			isShuffled = false;
		}
	}
	
	void windowSelected(int windowId)
	{
	}
	
	void OnGUI()
	{
		float height = 50;
		float yPos = Screen.height - height - 10;
		
		// Selectors
		GUI.Box(new Rect(0,0, 100, 100), "Items");
		for(int i=0 ;i<5; i++)
			arrItemToggle[i] = GUI.Toggle(new Rect(0,  20*(i+1), 100, 20), arrItemToggle[i], "Item"+i);	
		
//		string[] arrSelectors = {"Item1","Item2"};
//		 GUI.SelectionGrid(new Rect(150,50,200,height), 0, arrSelectors, 1);
		
		
		GUI.Box(new Rect(0,0, 100, 100), "Items");
		for(int i=0 ;i<5; i++)
			arrItemToggle[i] = GUI.Toggle(new Rect(0,  20*(i+1), 100, 20), arrItemToggle[i], "Item"+i);
		
		
		// Display Labels
		if(isShuffled) 
		{		
			GUI.contentColor = new Color(0,255,0,255);
			GUI.backgroundColor = new Color(255,0,0,255);			
			GUI.Window(0, new Rect(150,20,200,height), windowSelected, "Shoe Shuffled");
			GUI.contentColor = Color.white;
			GUI.backgroundColor = Color.white;					
		}
		if(isFetchingCards) 
		{
			GUI.Label(new Rect(150,yPos+20,300,height), "Fetching New Cards");
			return;
		}
		
		// Buttons
		if(GUI.Button(new Rect(0,yPos,100,height), "Reload"))
			reloadDemo();
		if(GUI.Button(new Rect(110,yPos,100,height), "Deal_1_Card"))
			dealNewCards(1);
		if(GUI.Button(new Rect(220,yPos,100,height), "Deal_3_Cards"))
			dealNewCards(3);
		if(GUI.Button(new Rect(330,yPos,100,height), "Deal_5_Cards"))
			dealNewCards(5);
	}
}
