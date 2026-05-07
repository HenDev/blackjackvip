using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TableListHolder : TouchHandlerDelegate {
		
	Vector3 initHolderPosition = Vector3.zero;
	Vector3 nextHolderPosition = Vector3.zero;
	Vector3 touchBeganPosition = Vector3.zero;
	public float mTableDataTouchOffset = 10.5f;
	
	public Vector3 minHolderPosition = Vector3.zero;
	public Vector3 maxHolderPosition = Vector3.zero;
	
	public bool saveMinHolderPosition = false;
	public bool saveMaxHolderPosition = false;
	
	bool isTableListActive = true;
	bool isTouched = false;
	
	// Use this for initialization
	void Start () { StartCoroutine(Instantiate()); }
	// Update is called once per frame
	void Update () { SaveHolderScrollPositions(); moveHolder(); }
	
	IEnumerator Instantiate()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.1f);
			init();
			break;
		}
	}
	
	void init()
	{
		float yOffset = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
			13.0f,   
			12.0f,   
			11.0f,   
			10.5f,  
			10.0f,  
			9.5f,  
			9.0f,   
			5.5f,
			5.0f,  
			4.5f, 
			3.0f,  
			2.0f, 
			1.0f   
		);
		mTableDataTouchOffset = MultiplePlatformPortingHandler.Instance.GetPositionBasedOnDeviceResolution(
			25.0f,  
			24.0f, 
			23.0f, 
			22.0f, 
			21.0f, 
			20.5f,  
			20.0f,  
			15.0f,  
			15.0f,  
			14.0f,  
			13.0f,  
			12.0f, 
			11.0f   
		);

		minHolderPosition += new Vector3(0, yOffset, 0);
		maxHolderPosition -= new Vector3(0, yOffset, 0);		
		
		TouchHandler.sharedHandler().addTouchHandlerDelegate(this);
		initHolderPosition = minHolderPosition;
		nextHolderPosition = minHolderPosition;
		
		TableData[] arrTableData = GetComponentsInChildren<TableData>();
		foreach(TableData tableData in arrTableData)
			tableData.init();
	}
	
	
	// Touch Delegates
	public override bool touchBegan (Vector3 position)
	{
		if(!isTableListActive || Mathf.Abs(position.y + 0.6f) > mTableDataTouchOffset) return false;		
		
		initHolderPosition = transform.position;
		touchBeganPosition = position;
		isTouched = true;
		
		return true;
	}
	
	public override void touchMoved (Vector3 position)
	{
		if(!isTouched || !isTableListActive || Mathf.Abs(position.y + 0.6f) > mTableDataTouchOffset) return;
		setNextScrollPosition(position);
	}
	
	public override void touchEnded (Vector3 position)
	{		
		isTouched = false;
	}
	
	
	// ScrollView
	void setNextScrollPosition(Vector3 position)
	{		
		nextHolderPosition = initHolderPosition + new Vector3(0, position.y - touchBeganPosition.y, 0);		
	}
	
	void moveHolder()
	{		
		transform.position = Vector3.Lerp(transform.position, nextHolderPosition, Time.deltaTime*10.0f);
		if(!isTouched && Mathf.Abs(transform.position.y - nextHolderPosition.y) < 0.5f)			
			nextHolderPosition.y = Mathf.Clamp(nextHolderPosition.y, minHolderPosition.y, maxHolderPosition.y);
	}
	
	// Scroll View Editior
	void SaveHolderScrollPositions()
	{
		if(saveMaxHolderPosition)
		{
			saveMaxHolderPosition = false;
			maxHolderPosition = transform.position;
		}
		
		if(saveMinHolderPosition)
		{
			saveMinHolderPosition = false;
			minHolderPosition = transform.position;
		}
	}
	
	// Disable / Enable
	public void enableTableListHandler()
	{
		isTableListActive = true;
		MenuButton[] arrTableButtons = GetComponentsInChildren<MenuButton>();
		foreach(MenuButton button in arrTableButtons)
			button.setTouchEnable(true);
	}
	
	public void disableTableListHandler()
	{
		isTableListActive = false;
		MenuButton[] arrTableButtons = GetComponentsInChildren<MenuButton>();
		foreach(MenuButton button in arrTableButtons)
			button.setTouchEnable(false);
	}
}
