using UnityEngine;
using System.Collections;

public class MenuButtonManager : MonoBehaviour {
	
	private static MenuButtonManager mSharedManager = null;
	private ArrayList mArrMenuButton = null;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}	
	
	public static MenuButtonManager sharedManager()
	{
		if(mSharedManager == null)
		{
			GameObject gameObject = new GameObject();
			mSharedManager = gameObject.AddComponent<MenuButtonManager>();
			gameObject.name = "MenuButtonsHandler";
		}
		return mSharedManager;
	}
	
	public void addChild(MenuButton menuButton)
	{
		if(mArrMenuButton == null) mArrMenuButton = new ArrayList();
		menuButton.setTag(-1);
		mArrMenuButton.Add(menuButton);
	}
	
	public void removeChildAt(int index)
	{
		if(index >= mArrMenuButton.Count || index < 0) return;
		((MenuButton)mArrMenuButton[index]).destroyButton();
		DestroyObject(((MenuButton)mArrMenuButton[index]).gameObject);
		mArrMenuButton.RemoveAt(index);
	}
	
	public void removeChild(MenuButton menuButton)
	{
		/*Debug.Log("ArrayCount : "+mArrMenuButton.Count+" MenuButtonTag : "+menuButton.getTag()+" DestroyMenuItem_"+mArrMenuButton.IndexOf(menuButton));				
		menuButton.destroyButton();
		DestroyObject(menuButton.gameObject);
		mArrMenuButton.Remove(menuButton);*/
		if(mArrMenuButton.IndexOf(menuButton) >= mArrMenuButton.Count || mArrMenuButton.IndexOf(menuButton) < 0) return;
		removeChildAt(mArrMenuButton.IndexOf(menuButton));
	}	
		
	public MenuButton createMenuItem(string _imageNormal, string _imageSelected, string _imageDisabled, string _imageMouseOver, int _ZOrder, Vector2 _touchAreaSize, LinkedSpriteManager _spriteManager, SpriteAtlasDataHandler _spriteAtlasDataHandler)
	{	
		GameObject menuButton = new GameObject();
#if UNITY_EDITOR
		menuButton.name = _imageNormal.Substring(_imageNormal.LastIndexOf('/')+1, _imageNormal.LastIndexOf('.')-_imageNormal.LastIndexOf('/')-1);// "MenuButton";		
#endif
		MenuButton button = menuButton.AddComponent<MenuButton>();
		sharedManager().addChild(button);
		button._imageNormal = _imageNormal;
		button._imageSelected = _imageSelected;
		button._imageMouseOver = _imageMouseOver;
		button._imageDisabled = _imageDisabled;
		button._ZOrder = _ZOrder;
		button._SpriteAtlasDataHandler = _spriteAtlasDataHandler;
		button._SpriteManager = _spriteManager;		
		button._buttonTouchAreaSize = _touchAreaSize;
		
		return button;
	}
	
	public MenuButton createMenuItem(string _imageNormal, string _imageSelected, string _imageDisabled, string _imageMouseOver, int _ZOrder, LinkedSpriteManager _spriteManager, SpriteAtlasDataHandler _spriteAtlasDataHandler, MenuButton.CallBack _buttonSelector)
	{			
		return createMenuItem(_imageNormal,_imageSelected,_imageDisabled,_imageMouseOver,_ZOrder,Vector2.zero,_spriteManager,_spriteAtlasDataHandler,_buttonSelector);
	}
	
	public MenuButton createMenuItem(string _imageNormal, string _imageSelected, string _imageDisabled, string _imageMouseOver, int _ZOrder, Vector2 _touchAreaSize, LinkedSpriteManager _spriteManager, SpriteAtlasDataHandler _spriteAtlasDataHandler, MenuButton.CallBack _buttonSelector)
	{
		MenuButton button = createMenuItem(_imageNormal, _imageSelected, _imageDisabled, _imageMouseOver, _ZOrder, _touchAreaSize, _spriteManager, _spriteAtlasDataHandler);
		button._buttonPressedCallBack = _buttonSelector;
		
		return button;
	}
	
	public MenuButton createMenuItem(string _imageNormal, string _imageSelected, string _imageDisabled, string _imageMouseOver, int _ZOrder, LinkedSpriteManager _spriteManager, SpriteAtlasDataHandler _spriteAtlasDataHandler, MenuButton.CallBack1 _buttonSelector)
	{			
		return createMenuItem(_imageNormal,_imageSelected,_imageDisabled,_imageMouseOver,_ZOrder,Vector2.zero,_spriteManager,_spriteAtlasDataHandler,_buttonSelector);
	}
	
	public MenuButton createMenuItem(string _imageNormal, string _imageSelected, string _imageDisabled, string _imageMouseOver, int _ZOrder, Vector2 _touchAreaSize, LinkedSpriteManager _spriteManager, SpriteAtlasDataHandler _spriteAtlasDataHandler, MenuButton.CallBack1 _buttonSelector)
	{
		MenuButton button = createMenuItem(_imageNormal, _imageSelected, _imageDisabled, _imageMouseOver, _ZOrder, _touchAreaSize, _spriteManager, _spriteAtlasDataHandler);
		button._buttonPressedCallBack1 = _buttonSelector;
		
		return button;
	}
	
}
