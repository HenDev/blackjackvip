using UnityEngine;
using System.Collections;

public class TableHandler : MonoBehaviour {
	
	public static TableHandler mInstance = null;
	public LinkedSpriteManager mSpriteManager = null;
	public SpriteAtlasDataHandler mSpriteAtlasDataHandler = null;
	
	public Material mTableBaseMaterial = null;
	public GameObject mTableDesign = null;
	public GameObject mTableText = null;
	
	public Texture2D[] arrTableBaseTextures = null;
		
	public static int tableBg = 7;
	public static bool isInsuranceDesign = true;
	public static bool isTableDesign = true;
	public static int tableWoodProp = 4;
	
	public static int minBet = 0;
	public static int maxBet = 0;
	public static int startChipIndex = 0;	
	
	public int min_Bet = 0;
	public int max_Bet = 0;
	
	// Use this for initialization
	void Start () { mInstance = this;  StartCoroutine(Instantiate()); }	
	
	IEnumerator Instantiate()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.05f);
			createTableWith();
			break;
		}
	}
	
	public void createTableWith()
	{
		float yOffset = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(1.0f, 1.0f, Loading.IsAdsRemoved?1.0f:0.0f, 2.0f, 3.0f, Loading.IsAdsRemoved?2.0f:1.0f, 1.0f, 3.5f, 4.0f);
		mTableText.transform.position += new Vector3(0, yOffset, 0);
		
		min_Bet = minBet;
		max_Bet = maxBet;
		
		mTableBaseMaterial.mainTexture = arrTableBaseTextures[tableBg-1];
		mTableDesign.SetActive(isTableDesign);
		
		if(isInsuranceDesign)
		{			
			float yPos = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(4.4f, 4.4f, Loading.IsAdsRemoved?4.4f:3.4f, 5.4f, 6.4f, Loading.IsAdsRemoved?5.4f:4.4f, 4.4f, 7.0f, Loading.IsAdsRemoved?6.2f:5.2f);
			GameObject insuranceDesignLeft = CommonData.createGameObject("InsuranceDesign_Left", gameObject, new Vector3(7,yPos,0), mSpriteManager, mSpriteAtlasDataHandler, "InsuranceProp.png", 0);
			insuranceDesignLeft.layer = gameObject.layer;
			
			GameObject insuranceDesignRight = CommonData.createGameObject("InsuranceDesign_Right", gameObject, new Vector3(-7,yPos,0), mSpriteManager, mSpriteAtlasDataHandler, "InsuranceProp.png", 0);
			insuranceDesignRight.layer = gameObject.layer;			
			insuranceDesignRight.transform.Rotate(new Vector3(0,180,0));
		}
		
		
		if(tableWoodProp > 0)
		{
			GameObject tableWoodPropObject = CommonData.createGameObject("TableWoodProp_1", gameObject, new Vector3(0,-7.2f,0), mSpriteManager, mSpriteAtlasDataHandler, "TableWoodProp1.png", 0);
			tableWoodPropObject.layer = gameObject.layer;						
		
			if(tableWoodProp > 1)
			{
				tableWoodPropObject = CommonData.createGameObject("TableWoodProp_2", gameObject, new Vector3(0,-10.0f,0), mSpriteManager, mSpriteAtlasDataHandler, "TableWoodProp2.png", 0);
				tableWoodPropObject.layer = gameObject.layer;	
				
				if(tableWoodProp > 2)
				{
					tableWoodPropObject = CommonData.createGameObject("TableWoodProp_3_Right", gameObject, new Vector3(2.5f,-10.0f,0), mSpriteManager, mSpriteAtlasDataHandler, "TableWoodProp3.png", 0);
					tableWoodPropObject.layer = gameObject.layer;	
					
					tableWoodPropObject = CommonData.createGameObject("TableWoodProp_3_Left", gameObject, new Vector3(-2.5f,-10.0f,0), mSpriteManager, mSpriteAtlasDataHandler, "TableWoodProp3.png", 0);
					tableWoodPropObject.layer = gameObject.layer;	
					tableWoodPropObject.transform.Rotate(new Vector3(0,180,0));
				
					if(tableWoodProp > 3)
					{
						tableWoodPropObject = CommonData.createGameObject("TableWoodProp_4_Left", gameObject, new Vector3(-5.5f,-9.6f,0), mSpriteManager, mSpriteAtlasDataHandler, "TableWoodProp4.png", 0);
						tableWoodPropObject.layer = gameObject.layer;	
						
						tableWoodPropObject = CommonData.createGameObject("TableWoodProp_4_Right", gameObject, new Vector3(5.5f,-9.6f,0), mSpriteManager, mSpriteAtlasDataHandler, "TableWoodProp4.png", 0);
						tableWoodPropObject.layer = gameObject.layer;	
					}
				}
			}
		}
		
//		BJ_AchievementHandler.mInstance.checkForHighRoller(min_Bet);
	}
}
