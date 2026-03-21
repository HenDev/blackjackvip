using UnityEngine;
using System.Collections;

public class CardsDemo : MonoBehaviour {
	
	public GameObject cardPrefab = null;
	public int coverIndex = 0;
		
	public CasinoCard.CardType mCurrentSpade;
	public CasinoCard.CardType mCurrentHearts;
	public CasinoCard.CardType mCurrentDiamond;
	public CasinoCard.CardType mCurrentClub;
	
	CasinoCard[] arrCards = null;
	// Use this for initialization
	void Start () {		
		float gapWidth = 5.0f;
		arrCards = new CasinoCard[4];
		for(int i=0; i<arrCards.Length; i++)
			arrCards[i] = ((GameObject)Instantiate(cardPrefab, new Vector3(-((float)(arrCards.Length-1))/2.0f*gapWidth + (gapWidth*i),0,0),
				Quaternion.identity)).GetComponent<CasinoCard>();
		loadNewCards();	
		
		Debug.Log("Vector3.left : "+Vector3.left);
		Debug.Log("Vector3.back : "+Vector3.back);
		Debug.Log("Vector3.down : "+Vector3.down);
		Debug.Log("Vector3.forward : "+Vector3.forward);
		Debug.Log("Vector3.right : "+Vector3.right);
		Debug.Log("Vector3.up : "+Vector3.up);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void loadNewCards()
	{
		for(int i=0; i<arrCards.Length; i++)
		{
			CasinoCard.CardType cardType = (CasinoCard.CardType)Random.Range(0,13);
			switch(i){
			case 0: mCurrentSpade = cardType; break;
			case 1: mCurrentHearts = cardType; break;
			case 2: mCurrentDiamond = cardType; break;
			case 3: mCurrentClub = cardType; break;}
			arrCards[i].createCard((CasinoCard.CardClass)i, cardType);
		}
	}
	
	void flipCards()
	{
		foreach(CasinoCard card in arrCards)
			card.flipCardVertically();
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(0,0,100,50), "Reload"))
			loadNewCards();
		if(GUI.Button(new Rect(110,0,100,50), "Flip"))
			flipCards();
	}
}
