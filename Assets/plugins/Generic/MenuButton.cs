using UnityEngine;
using System.Collections;

public class MenuButton : MonoBehaviour {
		
    public delegate void CallBack();
	public delegate void CallBack1(MenuButton _button);
    public CallBack _buttonPressedCallBack = null;
	public CallBack1 _buttonPressedCallBack1 = null;
	public LinkedSpriteManager _SpriteManager = null;
	public SpriteAtlasDataHandler _SpriteAtlasDataHandler = null;
	public string _imageNormal = "", _imageSelected = "", _imageDisabled = "", _imageMouseOver = "";
	public int _ZOrder = 0;	
	public bool _isEnabled = true;
	public bool _isTouchEnabled = true;
	public Vector2 _buttonTouchAreaSize = Vector2.zero;
	
	public enum TouchState {ButtonNormal, ButtonPressed, ButtonDeselected, ButtonDisabled, ButtonMouseOver};
	private bool _isInitialized = false;
	public TouchState _touchState = TouchState.ButtonNormal;
	public int _tag = 0;
	private bool _isRemoved = false;
	private Vector2 _contentSize = Vector2.zero;
	private bool _isTouchEnable = false;
	private bool _isVisible = true;
	
	// Use this for initialization
	void Start () {
		// Adding SpriteData for the Images
		SpriteData sprData = gameObject.AddComponent<SpriteData>();
		sprData._SpriteAtlasDataHandler = _SpriteAtlasDataHandler;
		sprData._SpriteManager = _SpriteManager;
		sprData._SpriteFrameName = ((_isEnabled || _imageDisabled == "") ? _imageNormal : _imageDisabled);
		sprData._ZOrder = _ZOrder;
		if(!(_isEnabled || _imageDisabled == "")) _touchState = TouchState.ButtonDisabled; // Setting current state		
	}
	
	public void init()
	{
		// Adding the BoxCollider for the button for touch
		if(_isInitialized || gameObject.GetComponent<SpriteData>()._Sprite == null) return;
		//if(!gameObject.GetComponent<BoxCollider>()) gameObject.AddComponent<BoxCollider>();
		// Setting the BoxCollider Size With Image Size
		Vector3 size = new Vector3(gameObject.GetComponent<SpriteData>()._Sprite.width, gameObject.GetComponent<SpriteData>()._Sprite.height, 1);
		// Setting the BoxCollider Size With the User Dimension
		if(_buttonTouchAreaSize.x > 0 && _buttonTouchAreaSize.y > 0) 
			size = new Vector3(_buttonTouchAreaSize.x, _buttonTouchAreaSize.y, 1);
		if(_contentSize.x != size.x || _contentSize.y != size.y)
		{
			_isInitialized = true;
			_contentSize = size;
		}
		
		setEnable(_isEnabled);
		setVisible(_isVisible);
		/*BoxCollider collider = gameObject.GetComponent<BoxCollider>();
		if(collider.size.x != size.x || collider.size.y != size.y)
		{
			_isInitialized = true;
			collider.size = size;
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		if(!_isRemoved)
		{
			if(_isInitialized) this.checkTouch();
			else this.init();
		}
		
		if(_isTouchEnable && !_isTouchEnabled) 
		{
			_isTouchEnable = false;
			_isTouchEnabled = true;
		}
	}
	
	public void destroyButton()
	{
		_isRemoved = true;
		_touchState = TouchState.ButtonDisabled;
		setVisible(true);
		gameObject.transform.parent = null;
		gameObject.GetComponent<SpriteData>().removeSprite();
	}
	
	public void addParent(GameObject _object)
	{
		gameObject.transform.parent = _object.transform;
		setPosition(Vector3.zero);
	}
	
	
	void checkTouch()
	{		
		if(_touchState == TouchState.ButtonDisabled || !_isTouchEnabled) return; // No Action If Disabled
		
		// Getting the Bounding Box of the button
		Vector3 position = gameObject.transform.position;
		Vector3 size = _contentSize;//gameObject.GetComponent<BoxCollider>().size;
		Vector2 scale = new Vector2(gameObject.transform.localScale.x,gameObject.transform.localScale.y);
		if(gameObject.transform.parent != null)
			scale = new Vector2(gameObject.transform.parent.transform.localScale.x * gameObject.transform.localScale.x,gameObject.transform.parent.transform.localScale.y * gameObject.transform.localScale.y);
		Vector4 rect = new Vector4(position.x - size.x/2 * scale.x, position.y - size.y/2 * scale.y, size.x * scale.x, size.y * scale.y);
		bool isMouseInsideButtonArea = CommonData.isMouseTouchInsideRect(rect);
		//Debug.Log("TTObject Name: " + gameObject.name);

		// SpritaData For changing Sprite
		bool isMouseButtonDown = Input.GetMouseButtonDown(0); // True at the mouse down insrance
		bool isMouesButtonUp = Input.GetMouseButtonUp(0);	  // True at the mouse up instance		
		bool isMouseButton = Input.GetMouseButton(0);		  // True with mouse down
		if(isMouseButton){}
				
		switch(_touchState)
		{
			case TouchState.ButtonDeselected:
				break;
			
			case TouchState.ButtonDisabled:
				break;
			
			case TouchState.ButtonNormal:
				if(isMouseInsideButtonArea)
				{
					if(_isTouchEnabled && isMouseButtonDown)
					{_touchState = TouchState.ButtonPressed; changeSprite(_imageSelected);}
					else if(_imageMouseOver != "")
					{changeSprite(_imageMouseOver);_touchState = TouchState.ButtonMouseOver;}
				}				
				break;
			
			case TouchState.ButtonPressed:
				if(isMouesButtonUp)
				{
					if(isMouseInsideButtonArea) // Calling the button Pressed Function using callback
						{if(_buttonPressedCallBack != null)_buttonPressedCallBack();
						else if(_buttonPressedCallBack1 != null)_buttonPressedCallBack1(this);}
					if(_isRemoved || !_isEnabled) return;
					if(isMouseInsideButtonArea && _imageMouseOver != "")
					{_touchState = TouchState.ButtonMouseOver;changeSprite(_imageMouseOver);}
					else
					{_touchState = TouchState.ButtonNormal;changeSprite(_imageNormal);}					
				}
				break;
			
			case TouchState.ButtonMouseOver:
				if(_isTouchEnabled && isMouseInsideButtonArea && isMouseButtonDown)
				{_touchState = TouchState.ButtonPressed; changeSprite(_imageSelected);}
				else if(!isMouseInsideButtonArea)
				{_touchState = TouchState.ButtonNormal; changeSprite(_imageNormal);}
				break;
		}			
	}
	
	private void changeSprite(string _imageName)
	{
		if(_isRemoved) return;
		SpriteData sprData = gameObject.GetComponent<SpriteData>();
		sprData.changeSprite(_imageName);
		sprData.setVisible(_isVisible);
	}
	
	public void setEnable(bool _enable)
	{
		_isEnabled = _enable;
		if(gameObject.GetComponent<SpriteData>() == null) return;
		if(_touchState == TouchState.ButtonDisabled && _isEnabled)
		{
			_touchState = TouchState.ButtonNormal;
			gameObject.GetComponent<SpriteData>().changeSprite(_imageNormal);
		}
		else if(!_isEnabled)
		{
			_touchState = TouchState.ButtonDisabled;
			if(_imageDisabled != "")
			gameObject.GetComponent<SpriteData>().changeSprite(_imageDisabled);
			else 
			gameObject.GetComponent<SpriteData>().changeSprite(_imageNormal);
		}
	}
	
	public void setVisible(bool _visible)
	{
		_isVisible = _visible;
		if(gameObject.GetComponent<SpriteData>() == null) return;
		gameObject.GetComponent<SpriteData>().setVisible(_isVisible);
	}
	
	public bool isVisible() { return _isVisible; }
	
	public void setTouchEnable(bool _enable)
	{
		if(Input.GetMouseButton(0) && _enable && !_isTouchEnabled)
		{_isTouchEnable = true;}
		else
		{
			_isTouchEnable = false;
			_isTouchEnabled = _enable;
		}
	}
	
	public bool isTouchEnabled()
	{
		return _isTouchEnabled;
	}
	
	public void setTag(int tag)
	{
		_tag = tag;
	}
	
	public int getTag()
	{
		return _tag;
	}
	
	public void setPosition(Vector3 position)
	{
		gameObject.transform.position = position + (gameObject.transform.parent != null ? gameObject.transform.parent.transform.position : Vector3.zero);
	}
	
	public Vector3 getPosition()
	{
		return gameObject.transform.position - (gameObject.transform.parent != null ? gameObject.transform.parent.transform.position : Vector3.zero);
	}
	
	public Vector2 getContentSize()
	{		
		return new Vector2(gameObject.GetComponent<SpriteData>()._Sprite.width,gameObject.GetComponent<SpriteData>()._Sprite.height);
	}
}
