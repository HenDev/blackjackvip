using UnityEngine;
using System.Collections;

public class CProgressBar : MonoBehaviour {
	
	public LinkedSpriteManager _spriteManager = null;
	public SpriteAtlasDataHandler _spriteAtlasDataHandler = null;
	public string _progressBarFillName = "";
	public string _progressBarSlotName = "";
	public int _zOrder = 0;
	
	public enum ProgressType {BottomToTop,TopToBottom,LeftToRight,RightToLeft}	
	public ProgressType _ProgressType = ProgressType.LeftToRight;
	
	private Vector2 _anchorPoint = new Vector2(0.0f,0.5f);
	private Sprite _progressBarSlot = null;
	private Sprite _progressBarFill = null;
	private float _percentage = 0.0f;
	private bool _isInit = false;
	private bool _isVisible = true;
	private Vector2 mfScale = new Vector2(1.0f,1.0f);
	private float mfInversePTMRatio = 0.03125f; // 1 divided by PTM RATIO (unity 1 unit == 32 pixcel)
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!_isInit) init();			
	}
	
	void init()
	{		
		_isInit = true;
		createProgressBarEmpty();
		setPercentage(_percentage);
	}
	
	private void init(LinkedSpriteManager spriteManager, SpriteAtlasDataHandler spriteAtlasDataHandler, string fileName, int zOrder)
	{
		_spriteManager = spriteManager;
		_spriteAtlasDataHandler = spriteAtlasDataHandler;
		_progressBarFillName = fileName;
		_zOrder = zOrder;
		init();
	}
	
	// Static Functions to add ProgressBar
	// Example CProgressBar progress = CProgressBar.create( "Pass All the Parameters" );
	public static CProgressBar create(LinkedSpriteManager spriteManager, SpriteAtlasDataHandler spriteAtlasDataHandler, string fileName, int zOrder)
	{
		GameObject progressBar = new GameObject();
		progressBar.name = "ProgressBar";
		CProgressBar progress = progressBar.AddComponent<CProgressBar>();
		progress.init(spriteManager,spriteAtlasDataHandler,fileName,zOrder);
		
		return progress;
	}
	
	public static CProgressBar create(LinkedSpriteManager spriteManager, SpriteAtlasDataHandler spriteAtlasDataHandler, string fileName, int zOrder, ProgressType progressType)
	{
		CProgressBar progress = create(spriteManager,spriteAtlasDataHandler,fileName,zOrder);
		progress._ProgressType = progressType;
		return progress;
	}
	
	public void createProgressBarEmpty()
	{
		if(_progressBarSlotName == "") return;
		LoadedSpriteDataParams loadedSpriteData = getSpriteDataParamWithName(_progressBarSlotName);
		_progressBarSlot = _spriteManager.AddSprite(gameObject,loadedSpriteData._iLeftPixel,loadedSpriteData._iBottomPixel,
				loadedSpriteData._iWidth,loadedSpriteData._iHeight,false);
		
		//SETTING SPRITE SIZE TO MATCH TRANSFORM
		_progressBarSlot.SetSizeXY(_progressBarSlot.width*mfInversePTMRatio*mfScale.x,_progressBarSlot.height*mfInversePTMRatio*mfScale.y);				
		_progressBarSlot.SetDrawLayer(_zOrder);
	}
	
	// Call This Function before destroying the ProgressBar
	public void destroyProgressBar()
	{		
		gameObject.transform.parent = null;
		if(_progressBarSlot != null)
		{
			_progressBarSlot.hidden = false;
			_spriteManager.RemoveSprite(_progressBarSlot);
			_progressBarSlot = null;
		}
		
		if(_progressBarFill != null)
		{
			_progressBarFill.hidden = false;
			_spriteManager.RemoveSprite(_progressBarFill);
			_progressBarFill = null;
		}
	}
	
	public void addParent(GameObject _gameObject)
	{
		gameObject.transform.parent = _gameObject.transform;
		setPosition(Vector3.zero);
	}
	
	public void setAnchorPoint(float x,float y)
	{
		_anchorPoint = new Vector2(x,y);
		setPercentage(_percentage);
	}
	
	public void setProgressType(ProgressType progressType)
	{
		_ProgressType = progressType;
		setPercentage(_percentage);
	}
	
	public void setPercentage(float _percent)
	{
		if(_progressBarFill != null)
		{
			_progressBarFill.hidden = false;
			_spriteManager.RemoveSprite(_progressBarFill);
			_progressBarFill = null;
		}
		
		if(_percent < 0) _percent = 0.0f;
		else if(_percent > 100) _percent = 100.0f;
		_percentage = _percent;
		if(!_isVisible) return;
		
		LoadedSpriteDataParams loadedSpriteData = getSpriteDataParamWithName(_progressBarFillName);	
		int _iLeftPixel = loadedSpriteData._iLeftPixel;
		int _iBottomPixel = loadedSpriteData._iBottomPixel;
		int _iHeight = loadedSpriteData._iHeight;
		int _iWidth = loadedSpriteData._iWidth;
		
		Vector3 _offset = Vector3.zero;
		switch(_ProgressType)
		{
			case ProgressType.BottomToTop:
				_offset = new Vector3((0.5f-_anchorPoint.x)*_iWidth*mfInversePTMRatio,((0.5f-_anchorPoint.y)*_iHeight) * ((100.0f-_percent)/100.0f) * mfInversePTMRatio,0);
				_iHeight = (int)((float)_iHeight * ((float)_percent/100.0f));		
				break;
			case ProgressType.TopToBottom:
				_offset = new Vector3((0.5f-_anchorPoint.x)*_iWidth*mfInversePTMRatio,((0.5f-_anchorPoint.y)*_iHeight) * ((100.0f-_percent)/100.0f) * mfInversePTMRatio,0);
				_iBottomPixel -= _iHeight;
				_iHeight = (int)((float)_iHeight * ((float)_percent/100.0f));
				_iBottomPixel += _iHeight;
				break;
			case ProgressType.LeftToRight:
				_offset = new Vector3(((_anchorPoint.x-0.5f)*_iWidth) * ((100.0f-_percent)/100.0f) * mfInversePTMRatio,(0.5f-_anchorPoint.y)*_iHeight*mfInversePTMRatio,0);
				_iWidth = (int)((float)_iWidth * ((float)_percent/100.0f));
				break;
			case ProgressType.RightToLeft:
				_offset = new Vector3(((_anchorPoint.x-0.5f)*_iWidth) * ((100.0f-_percent)/100.0f) * mfInversePTMRatio,(0.5f-_anchorPoint.y)*_iHeight*mfInversePTMRatio,0);
				_iLeftPixel += _iWidth;
				_iWidth = (int)((float)_iWidth * ((float)_percent/100.0f));
				_iLeftPixel -= _iWidth;
				break;
		}
		_progressBarFill = _spriteManager.AddSprite(gameObject,_iLeftPixel,_iBottomPixel,
				_iWidth,_iHeight,false);
		_progressBarFill.offset = _offset;
		
		
		//SETTING SPRITE SIZE TO MATCH TRANSFORM
		_progressBarFill.SetSizeXY(_progressBarFill.width*mfInversePTMRatio*mfScale.x,_progressBarFill.height*mfInversePTMRatio*mfScale.y);				
		_progressBarFill.SetDrawLayer(_zOrder+1);
	}
	
	//LOADING SPRITE PARAMS
	private LoadedSpriteDataParams getSpriteDataParamWithName(string p_filename)
	{
		//GET LOADED SPRITE PARAMS FOR FRAME NAME
		LoadedSpriteDataParams loadedSpriteData = null;
		loadedSpriteData = _spriteAtlasDataHandler.getLoadedSpriteData(p_filename);	

		if(loadedSpriteData == null)
			Debug.Log("Sprite frame named "+p_filename+" unavailable");
		
		return loadedSpriteData;
	}
	
	public void setVisible(bool visible)
	{
		_isVisible = visible;
		if(_progressBarFill != null) _progressBarFill.hidden = !visible;		
		if(_progressBarSlot != null) _progressBarSlot.hidden = !visible;		
		if(visible) setPercentage(_percentage);
	}
	
	public bool isVisible()
	{
		return _isVisible;
	}
	
	public float getPercentage()
	{
		return _percentage;
	}
	
	// Set the position of the label relative to the parent if parent is present else sets the position
	public void setPosition(Vector3 position)
	{
		gameObject.transform.position = position + (gameObject.transform.parent != null ? gameObject.transform.parent.transform.position : Vector3.zero);
	}
	
	// Change the ProgressBar scale	
	public void setScale(float scale)
	{
		gameObject.transform.localScale = new Vector3(scale,scale,gameObject.transform.localScale.z);
	}
	
	public void setScale(Vector2 scale)
	{
		gameObject.transform.localScale = new Vector3(scale.x,scale.y,gameObject.transform.localScale.z);
	}	
	
	public Vector2 getScale()
	{		
		return new Vector2(gameObject.transform.localScale.x,gameObject.transform.localScale.y);
	}
	
	// Call this Function before destroying Object
	public void destroy()
	{
		if(_progressBarSlot != null)
		{
			_progressBarSlot.hidden = false;
			_spriteManager.RemoveSprite(_progressBarSlot);
			_progressBarSlot = null;
		}
		
		if(_progressBarFill != null)
		{
			_progressBarFill.hidden = false;
			_spriteManager.RemoveSprite(_progressBarFill);
			_progressBarFill = null;
		}
	}
}
