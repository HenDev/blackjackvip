using UnityEngine;
using System.Collections;

public class CasinoCard : MonoBehaviour {
	
	public enum CardClass {Spade=0, Heart, Diamond, Club};		// Type of the Cards
	public enum CardType {Ace=0, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King};		//Different Cards in each Type
	
	public GameObject mCardFace = null;		// The GameObject of the CardFace added as the child
	public GameObject mCardCover = null;	// The GameObject of the CardCover added as the child
	public float mCardScaleRatio = 1.0f;	// The scale of the card 1.0f implies actual size
	public float mCardFlipSpeed = 0.5f;		// The duration taken for the card to flip, 0.5f takes half second to flip
	public int mCardCoverIndex = 0;			// Index of the cover for the card
	public string mCardLayerName = "Cards";	// Layer Name to set the Common Layer for all cards
	float PtmRatio = 32.0f;					// Pixel To Meter Ratio used in convertion
	int row = 4;							// Four Types 
	int col = 13;							// 13 Cards of each Type
	private bool isInitialized = false;
	
	private int mCardValue = 0;
	private int mCardClass = 0;
	
	// Use this for initialization
	void Start () {
		if(isInitialized) return;
		gameObject.layer = LayerMask.NameToLayer(mCardLayerName);
		mCardFace.layer = LayerMask.NameToLayer(mCardLayerName);
		mCardCover.layer = LayerMask.NameToLayer(mCardLayerName);
		isInitialized = true;
		// Initializing the Card Scale
		float cardWidth = mCardFace.GetComponent<Renderer>().material.mainTexture.width/col;
		float cardHeight = mCardFace.GetComponent<Renderer>().material.mainTexture.height/row;
		float widthRatio = cardWidth/PtmRatio;
		float heightRatio = cardHeight/PtmRatio;
		transform.localScale = new Vector3(widthRatio*mCardScaleRatio, heightRatio*mCardScaleRatio,0.02f);
		
		// Initializing Card Face		
		Vector2 faceScale = new Vector2(1.0f/col,1.0f/row);
		mCardFace.GetComponent<Renderer>().material.mainTextureScale = faceScale;
		mCardFace.GetComponent<Renderer>().material.mainTextureOffset = Vector2.zero;
		
		// Initializing Card Cover
		float coverWidth = mCardCover.GetComponent<Renderer>().material.mainTexture.width;
		float coverHeight = mCardCover.GetComponent<Renderer>().material.mainTexture.height;
		Vector2 coverScale = new Vector2((cardWidth/coverWidth), (cardHeight/coverHeight));
		int noOfColsInCover = Mathf.CeilToInt(cardWidth / coverScale.x);
		int coverRowNo = Mathf.FloorToInt((float)mCardCoverIndex/(float)noOfColsInCover);
		int coverColNo = mCardCoverIndex%noOfColsInCover;
		mCardCover.GetComponent<Renderer>().material.mainTextureScale = coverScale;
		mCardCover.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(coverScale.x*coverColNo,coverScale.y*coverRowNo);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public void createCard(CardClass cardClass, CardType cardType)
	{		
		Start();
		int xIndex = (int)cardType;
		int yIndex = (row-1) - (int)cardClass;
		Vector2 scale = mCardFace.GetComponent<Renderer>().material.mainTextureScale;
		mCardFace.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(xIndex * scale.x,yIndex * scale.y);
		transform.rotation = Quaternion.identity;
		name = cardClass+"_"+cardType;
		
		mCardValue = ((int)cardType)+1;
		mCardClass = (int)cardClass;
	}
	
	public void flipCardVertically()
	{
		FlipCard.createFlipAction(gameObject, FlipCard.FlipType.Vertical, mCardFlipSpeed);
	}
	
	// Getters
	public int getCardValue(){ return mCardValue; }	
	public int getCardClass(){ return mCardClass; }
}
