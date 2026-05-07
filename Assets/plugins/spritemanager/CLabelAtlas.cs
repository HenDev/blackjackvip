using UnityEngine;
using System.Collections;

public class CLabelAtlas : MonoBehaviour {
	
	
	public LinkedSpriteManager mSpriteManager = null;
	public SpriteAtlasDataHandler mSpriteAtlasDataHandler = null;
	public string mFontFileName = "";
	public int mZorder = 0;
	public float mLineSpacing = 0;
	private Color mColor = Color.white;

	private ArrayList mArrSprite = null;
	public string mString = "";
	private bool mInitialized = false;
	private bool mIsVisible = true;
	//private Vector3 mLabelPosition = Vector3.zero;
	private Vector2 mfScale = new Vector2(1.0f,1.0f);
	private float mfInversePTMRatio = 0.03125f; // 1 divided by PTM RATIO (unity 1 unit == 32 pixcel)

	public enum LabelTextAlignment {Left,Right,Center};
	public LabelTextAlignment mCurrentTextAlignment = LabelTextAlignment.Center;
	private CLabelAtlas _labelAtlas = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Application.isPlaying)
			if(!mInitialized) init();
	}
	
	// Init Function
	public void init()		
	{
		mInitialized = true;
		if (Application.isPlaying)
		{
			CLabelAtlas label = create(mString,mSpriteManager,mSpriteAtlasDataHandler,mFontFileName,mZorder,mCurrentTextAlignment);
			label.addParent(gameObject);
			gameObject.GetComponent<CLabelAtlas>().enabled = false;
			_labelAtlas = label;
			_labelAtlas.setSpacing(mLineSpacing);
		}
	}
	
	public void init(string _strValue, LinkedSpriteManager _spriteManager, SpriteAtlasDataHandler _spriteAtlasDataHandler, string _fontFileName, int _zOrder, LabelTextAlignment _alignment)
	{		
		mCurrentTextAlignment = _alignment;
		mSpriteManager = _spriteManager;
		mSpriteAtlasDataHandler = _spriteAtlasDataHandler;
		mFontFileName = _fontFileName;
		mZorder = _zOrder;
		setString(_strValue);
		setPosition(Vector3.zero);
		mInitialized = true;
	}
	
	// Static Functions to add Label
	// Example CLabelAtlas label = CLabelAtlas.create( "Pass All the Parameters" );
	public static CLabelAtlas create(string _strValue, LinkedSpriteManager _spriteManager, SpriteAtlasDataHandler _spriteAtlasDataHandler, string _fontFileName,int _zOrder)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "LabelAtlas";
		CLabelAtlas labelAtlas = gameObject.AddComponent<CLabelAtlas>();
		labelAtlas.init(_strValue,_spriteManager,_spriteAtlasDataHandler,_fontFileName,_zOrder,LabelTextAlignment.Center);
		return labelAtlas;
	}
	
	public static CLabelAtlas create(string _strValue, LinkedSpriteManager _spriteManager, SpriteAtlasDataHandler _spriteAtlasDataHandler, string _fontFileName,int _zOrder, LabelTextAlignment _alignment)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "LabelAtlas";
		CLabelAtlas labelAtlas = gameObject.AddComponent<CLabelAtlas>();
		labelAtlas.init(_strValue,_spriteManager,_spriteAtlasDataHandler,_fontFileName,_zOrder,_alignment);
		return labelAtlas;
	}
	
	// Sets the string 
	public void setString(string strValue)
	{
		if(_labelAtlas != null) {_labelAtlas.setString(strValue);return;}
		if(mArrSprite != null)  
		{
			for(int i=0; i<mArrSprite.Count; i++)
			{
				((Sprite)mArrSprite[i]).hidden = false;
				mSpriteManager.RemoveSprite((Sprite)mArrSprite[i]);
			}
			mArrSprite.Clear();
		}
		else
			mArrSprite = new ArrayList();
		mString = strValue;
		
		for(int i=0 ; i < strValue.Length ; i++)
			addSpriteAt(i);
		
		setVisible(mIsVisible);
	}
	
	protected void addSpriteAt(int _index)
	{
		int index = 0;
		if(mString[_index] == '.')
			index = 0;
		if(mString[_index] == '/')
			index = 1;
		if(mString[_index] == '0')
			index = 2;
		if(mString[_index] == '1')
			index = 3;
		if(mString[_index] == '2')
			index = 4;
		if(mString[_index] == '3')
			index = 5;
		if(mString[_index] == '4')
			index = 6;
		if(mString[_index] == '5')
			index = 7;
		if(mString[_index] == '6')
			index = 8;
		if(mString[_index] == '7')
			index = 9;
		if(mString[_index] == '8')
			index = 10;
		if(mString[_index] == '9')
			index = 11;
		
		LoadedSpriteDataParams loadedSpriteData = getSpriteDataParamWithName(mFontFileName);
		Vector4 boundaryOffset = new Vector4((1.0f/12)*index,0,(1.0f/12),1.0f);
		Sprite _Sprite = mSpriteManager.AddSprite(gameObject,loadedSpriteData._iLeftPixel + (int)(loadedSpriteData._iWidth * boundaryOffset.x),loadedSpriteData._iBottomPixel + (int)(loadedSpriteData._iHeight * boundaryOffset.y),
				(int)(loadedSpriteData._iWidth * boundaryOffset.z),(int)(loadedSpriteData._iHeight * boundaryOffset.w),false);
		
		float width = _Sprite.width + mLineSpacing;
		// Adjusting the offset of the sprite based on the index and alignment
		switch(mCurrentTextAlignment)
		{
			case LabelTextAlignment.Center:
			float centerIndex = ((float)mString.Length)/2.0f;
			_Sprite.offset = new Vector3((_index-centerIndex+0.5f)*width*mfInversePTMRatio,0,0);
				break;
			case LabelTextAlignment.Left:
			_Sprite.offset = new Vector3((_index+0.5f)*width*mfInversePTMRatio,0,0);
				break;
			case LabelTextAlignment.Right:
			_Sprite.offset = new Vector3(0.5f-(mString.Length-_index)*width*mfInversePTMRatio,0,0);
				break;
		}		
		
		_Sprite.SetColor(mColor);
		//SETTING SPRITE SIZE TO MATCH TRANSFORM
		_Sprite.SetSizeXY(_Sprite.width*mfInversePTMRatio*mfScale.x,_Sprite.height*mfInversePTMRatio*mfScale.y);
				
		_Sprite.SetDrawLayer(mZorder);
		mArrSprite.Add(_Sprite);	
	}
	
	public void addParent(GameObject _gameObject)
	{
		if(_labelAtlas != null) {_labelAtlas.addParent(_gameObject);return;}
		gameObject.transform.parent = _gameObject.transform;
		setPosition(Vector3.zero);
	}
	
	//LOADING SPRITE PARAMS
	private LoadedSpriteDataParams getSpriteDataParamWithName(string p_filename)
	{
		//GET LOADED SPRITE PARAMS FOR FRAME NAME
		LoadedSpriteDataParams loadedSpriteData = null;
		loadedSpriteData = mSpriteAtlasDataHandler.getLoadedSpriteData(p_filename);	

		if(loadedSpriteData == null)
			Debug.Log("Sprite frame named "+p_filename+" unavailable");
		
		return loadedSpriteData;
	}
	
	// Set Space Between Digits
	public void setSpacing(float _space)
	{
		mLineSpacing = _space;
		setString(mString);
	}
	
	// Set the position of the label relative to the parent if parent is present else sets the position
	public void setPosition(Vector3 position)
	{
		if(_labelAtlas != null) {_labelAtlas.setPosition(position);return;}
		//mLabelPosition = position;
		gameObject.transform.position = position + (gameObject.transform.parent != null ? gameObject.transform.parent.transform.position : Vector3.zero);
	}
	
	// Changing the visibility
	public void setVisible(bool visible)
	{
		if(_labelAtlas != null) {_labelAtlas.setVisible(visible);return;}
		mIsVisible = visible;
		for(int i=0; i<mArrSprite.Count; i++)
			((Sprite)mArrSprite[i]).hidden = !mIsVisible;
	}
	
	public bool isVisible()
	{
		return (_labelAtlas!=null ? _labelAtlas.isVisible() : mIsVisible);
	}
	
	// Call This Function before destroying the Label
	public void destroyLabel()
	{		
		if(_labelAtlas!=null) {_labelAtlas.destroyLabel();return;}
		gameObject.transform.parent = null;
		setString("");
	}
	
	// Change the sprite image of the text with the same text
	public void changeSprite(string _fontFileName)
	{
		if(_labelAtlas!=null) {_labelAtlas.changeSprite(_fontFileName);return;}		
		mFontFileName = _fontFileName;
		setString(mString);
	}
	
	// Change the label scale	
	public void setScale(float scale)
	{
		if(_labelAtlas) {_labelAtlas.setScale(scale);return;}
		gameObject.transform.localScale = new Vector3(scale,scale,gameObject.transform.localScale.z);
	}
	
	public void setScale(Vector2 scale)
	{
		if(_labelAtlas!=null) {_labelAtlas.setScale(scale);return;}
		gameObject.transform.localScale = new Vector3(scale.x,scale.y,gameObject.transform.localScale.z);
	}	
	
	public Vector2 getScale()
	{		
		return (_labelAtlas!=null ? _labelAtlas.getScale() : new Vector2(gameObject.transform.localScale.x,gameObject.transform.localScale.y));
	}
	
	// Label Color
	public Color getLabelColor()
	{
		return (_labelAtlas!=null ? _labelAtlas.getLabelColor() : mColor);
	}
	
	public void setLabelColor(Color labelColor)
	{
		if(_labelAtlas!=null)
			_labelAtlas.setLabelColor(labelColor);
		else
		{
			mColor = labelColor;
			foreach(Sprite sprite_ in mArrSprite)
				sprite_.SetColor(labelColor);
		}
	}
}
