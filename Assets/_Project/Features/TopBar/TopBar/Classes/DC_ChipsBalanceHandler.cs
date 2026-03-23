using UnityEngine;
using System.Collections;

public class DC_ChipsBalanceHandler : MonoBehaviour {
		
	public LinkedSpriteManager mSpriteManager = null;
	public SpriteAtlasDataHandler mSpriteAtlasDataHandler = null;
	
	MenuButton mAddChipsButton = null;
	GameObject mChipsBalanceHolder = null;
	CLabelAtlas mChipsBalanceLabel = null;
	
	public int ChipsBalance
	{
		get { return PlayerPrefs.GetInt("DC_ChipsBalance"); }
		set { PlayerPrefs.SetInt("DC_ChipsBalance", value); }
	}
	
	// Use this for initialization
	void Start () {	}	
	// Update is called once per frame
	void Update () { }
	
	public void init()
	{	
		ChipsBalance = 999999999;
		
		mAddChipsButton = MenuButtonManager.sharedManager().createMenuItem("Button_Add.png", "Button_Add_Selected.png", "", "", 3, mSpriteManager, mSpriteAtlasDataHandler, addChipsSelected);
		mAddChipsButton.gameObject.layer = gameObject.layer;
		mAddChipsButton.addParent(gameObject);
		mAddChipsButton.setPosition(new Vector3(9,0,0));
		
		mChipsBalanceHolder = CommonData.createGameObject("Credits Holder", gameObject, new Vector3(4.8f,0,0), mSpriteManager, mSpriteAtlasDataHandler, "CreditsHolder.png", 1);
		mChipsBalanceHolder.layer = gameObject.layer;
		
		mChipsBalanceLabel = CLabelAtlas.create(ChipsBalance.ToString(), mSpriteManager, mSpriteAtlasDataHandler, "Item_Numbers.png", 2, CLabelAtlas.LabelTextAlignment.Right);
		mChipsBalanceLabel.addParent(gameObject);
		mChipsBalanceLabel.gameObject.layer = gameObject.layer;
		mChipsBalanceLabel.setSpacing(-5.0f);
		mChipsBalanceLabel.setPosition(new Vector3(7.8f,0,0));
		mChipsBalanceLabel.setScale(1.0f);
	}
	
	// Buttons Selected	
	void addChipsSelected()
	{
	}
	
}
