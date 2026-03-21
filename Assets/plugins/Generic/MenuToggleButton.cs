using UnityEngine;
using System.Collections;

public class MenuToggleButton : MonoBehaviour {
		
	MenuButton toggleButton1 = null;
	MenuButton toggleButton2 = null;
	int currentToggleIndex = 0;
	public delegate void ToggleButtonCallback();
	ToggleButtonCallback toggleButtonCallback = null;
	// Use this for initialization
	void Start () { }
	
	public void create(string toggleImage1, string toggleImage2, string disabledImage1, string disabledImage2, int zOrder, 
		LinkedSpriteManager spriteManager, SpriteAtlasDataHandler spriteAtlasDataHandler, ToggleButtonCallback toggleButtonCallBack, int startToggleIndex)
	{
		// Toggle Button 1
		toggleButton1 = MenuButtonManager.sharedManager().createMenuItem(toggleImage1, toggleImage1, disabledImage1, "", zOrder, spriteManager, spriteAtlasDataHandler, buttonSelected);
		toggleButton1.setPosition(Vector3.zero);
		toggleButton1.addParent(gameObject);
		toggleButton1.name = "ToggleButton_1";	
		
		// Toggle Button 2
		toggleButton2 = MenuButtonManager.sharedManager().createMenuItem(toggleImage2, toggleImage2, disabledImage2, "", zOrder, spriteManager, spriteAtlasDataHandler, buttonSelected);
		toggleButton2.setPosition(Vector3.zero);
		toggleButton2.addParent(gameObject);
		toggleButton2.name = "ToggleButton_2";
		
		currentToggleIndex = startToggleIndex;// == 0 ? 1 : 0;
		toggleButton();
		
		transform.position = Vector3.zero;
		this.toggleButtonCallback = toggleButtonCallBack;
	}
	
	public void toggleButton()
	{
		switch(currentToggleIndex)
		{
		case 0: 
			toggleButton1.setTouchEnable(false);
			toggleButton2.setTouchEnable(true);
			toggleButton1.setVisible(false);
			toggleButton2.setVisible(true);
			currentToggleIndex = 1;
			break;
		case 1:
			toggleButton1.setTouchEnable(true);
			toggleButton2.setTouchEnable(false);
			toggleButton1.setVisible(true);
			toggleButton2.setVisible(false);
			currentToggleIndex = 0;
			break;
		}
	}
	
	void buttonSelected()
	{
		toggleButton();
		if(toggleButtonCallback != null) toggleButtonCallback();
	}
	
	public void removeButton()
	{
		if(toggleButton1 != null)
		{
			MenuButtonManager.sharedManager().removeChild(toggleButton1);
			toggleButton1 = null;
		}
		if(toggleButton2 != null)
		{
			MenuButtonManager.sharedManager().removeChild(toggleButton2);
			toggleButton2 = null;
		}
		
	}
	
	// Getters
	public float getButtonWidth() { return toggleButton1.getContentSize().x; }
	public float getButtonHeight() { return toggleButton1.getContentSize().y; }
	public int getCurrentToggleIndex() { return currentToggleIndex; }
	
	public void addParent(GameObject _object) { transform.parent = _object.transform; }
	public void setPosition(Vector3 position) { transform.position = (transform.parent != null ? transform.parent.position : Vector3.zero) + position; }
	
	// Menu Button Functionalites
	public void setLayer(LayerMask layer) { 
		toggleButton1.gameObject.layer = layer; 
		toggleButton2.gameObject.layer = layer; 
		gameObject.layer = layer;
	}
	
	public void setTouchEnable(bool _enable)
	{
		if(toggleButton1.isTouchEnabled() == _enable) return;
		toggleButton1.setTouchEnable(_enable);
		toggleButton2.setTouchEnable(_enable);
	}
	
	public bool isTouchEnabled()
	{
		return toggleButton1.isTouchEnabled();
	}
	
	public void setVisible(bool _visible)
	{
		if(toggleButton1.isVisible() == _visible) return;
		toggleButton1.setVisible(_visible);
		toggleButton2.setVisible(_visible);
	}
	
	public bool isVisible() { return toggleButton1.isVisible(); }
	
	public void setTag(int tag)
	{
		toggleButton1.setTag(tag);
		toggleButton2.setTag(tag);
	}
	
	public int getTag()
	{
		return toggleButton1.getTag();
	}
	
	public void setEnable(bool _enable)
	{
		if(toggleButton1._isEnabled == _enable) return;
		toggleButton1.setEnable(_enable);
		toggleButton2.setEnable(_enable);
	}
	
	public static MenuToggleButton createToggleButton(string toggleImage1, string toggleImage2, string disabledImage1, string disabledImage2, int zOrder, 
		LinkedSpriteManager spriteManager, SpriteAtlasDataHandler spriteAtlasDataHandler, ToggleButtonCallback toggleButtonCallBack, int startToggleIndex)
	{
		GameObject toggleButton = new GameObject();
		toggleButton.name = "ToggleButton";
		toggleButton.AddComponent<MenuToggleButton>().create(toggleImage1, toggleImage2, disabledImage1, disabledImage2, zOrder, 
		spriteManager, spriteAtlasDataHandler, toggleButtonCallBack, startToggleIndex);
		return toggleButton.GetComponentInChildren<MenuToggleButton>();
	}	
}
