using UnityEngine;
using System.Collections;

public class TableData : MonoBehaviour {
	
	public int minBet = 1;
	public int maxBet = 10;
	public MenuButton tableButton = null;
	
	// Table Design Data	
	public int tableBgNo = 7;
	public bool isInsuranceDesign = true;
	public bool isTableDesign = true;
	public int tableWoodPropNo = 4;
	public int startChipIndex = 0;
	
	// Table Flurry Data
	public int tableIndex = 0;
	public string tableName = "";
	
	// Use this for initialization
	void Start () { }	
	// Update is called once per frame
	void Update () { }
	
	public void init()
	{
		LinkedSpriteManager spriteManager = GetComponentInChildren<SpriteData>()._SpriteManager;
		SpriteAtlasDataHandler spriteAtlasDataHandler = GetComponentInChildren<SpriteData>()._SpriteAtlasDataHandler;
		
		CLabelAtlas minLabel = CLabelAtlas.create(getTheNumberWithComma(minBet), spriteManager, spriteAtlasDataHandler, "NumberFont.png", 1);
		minLabel.gameObject.layer = gameObject.layer;
		minLabel.addParent(gameObject);
		minLabel.setScale(0.8f);
		minLabel.setSpacing(-5.0f);
		minLabel.setPosition(new Vector3(-5.0f,-1.0f,0));
		
		
		CLabelAtlas maxLabel = CLabelAtlas.create(getTheNumberWithComma(maxBet), spriteManager, spriteAtlasDataHandler, "NumberFont.png", 1);
		maxLabel.gameObject.layer = gameObject.layer;
		maxLabel.addParent(gameObject);
		maxLabel.setScale(0.8f);
		maxLabel.setSpacing(-5.0f);
		maxLabel.setPosition(new Vector3(5.0f,-1.0f,0));
		
		tableButton = MenuButtonManager.sharedManager().createMenuItem("TableHolderNormal.png", "TableHolderSelected.png", "", "", 2, spriteManager, spriteAtlasDataHandler, tableSelected);
		tableButton.addParent(gameObject);
		tableButton.gameObject.layer = gameObject.layer;
		tableButton.setPosition(new Vector3(0, 0.2f, 0));
	}
	
	void tableSelected()
	{
		float tableDataTouchOffset = transform.parent.GetComponent<TableListHolder>().mTableDataTouchOffset - 2.0f;
//		Debug.Log("position : "+transform.position.y + " offset : "+tableDataTouchOffset);
		if(Mathf.Abs(transform.position.y + 0.6f) > tableDataTouchOffset) return;		
		PlayAudioSounds.sharedHandler().playSound("ButtonSelect");
		
		if(BJ_BottomBar.mInstance.PlayerChipsBalance < minBet)
		{
			MainMenuLayer.mInstance.disableMainMenu();
			PopUpManager.mInstance.showPopUp(PopUpManager.PopUpType.InsufficientFunds, MainMenuLayer.mInstance.enableMainMenu);
			return;
		}
		
		TableHandler.tableBg = tableBgNo;
		TableHandler.isInsuranceDesign = isInsuranceDesign;
		TableHandler.isTableDesign = isTableDesign;
		TableHandler.tableWoodProp = tableWoodPropNo;
		
		TableHandler.minBet = minBet;
		TableHandler.maxBet = maxBet;
		TableHandler.startChipIndex = startChipIndex;
		
		MainMenuLayer.mInstance.mAlphaLayer.fadeInAlphaLayer(0.2f, 1.0f);
		CCallFunc.createCallAfterDelay(loadGamePlay, 0.3f).runAction();
		BJ_AchievementHandler.mInstance.checkForHighRoller(minBet);
		BlackjackFlurryHandler.sendTableJoinedFlurryData(tableIndex, tableName);
		MainMenuLayer.mInstance.disableMainMenu();
	}
	
	void loadGamePlay()
	{
		Application.LoadLevel(2);
	}
	
	string getTheNumberWithComma(int number)
	{
		if(number == -1) return "";
		string numberString = number.ToString();		
		int noOfCommas = Mathf.FloorToInt((numberString.Length-1)/3.0f);		
		if(noOfCommas > 0 && numberString.Length > 4)
			for(int i=0; i< noOfCommas; i++)
				numberString = numberString.Insert(numberString.Length-3 - i*4, "/");				
		return numberString;
	}
}
