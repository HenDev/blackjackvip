using UnityEngine;
using System.Collections;

// TOUCHHANDLER DELEGATES
public class TouchHandlerDelegate : MonoBehaviour {
	virtual public bool touchBegan(Vector3 position){return true;}
	virtual public void touchMoved(Vector3 position){}
	virtual public void touchEnded(Vector3 position){}
}

// STORES THE DATA OF EACH GAMEOBJECT
public class TouchHandlerObject : MonoBehaviour {
	public TouchHandlerDelegate _delegate = null;
}

// TOUCH HANDLER
public class TouchHandler : MonoBehaviour {
	
	private Vector3 currentTouchPosition = Vector3.zero;
	private bool mIsTouchCancled = false;
	// Static Object for handling
	private static TouchHandler mSharedHandler = null;
	// Use this for initialization
	void Start () {	}	
	// Update is called once per frame
	void Update () { removeNullDelegates(); dispatchTouches(); }
	
	// STATIC HANDLER
	public static TouchHandler sharedHandler()
	{
		if(mSharedHandler == null)
		{
			GameObject touchHandler = new GameObject();
			touchHandler.name = "TouchHandler";
			mSharedHandler = touchHandler.AddComponent<TouchHandler>();
		}
		return mSharedHandler;
	}
	
	// ADDING DELEGATES TO TOUCH HANDLER
	public void addTouchHandlerDelegate(TouchHandlerDelegate _delegate)
	{
		TouchHandlerObject _object = gameObject.AddComponent<TouchHandlerObject>();
		_object._delegate = _delegate;
	}
	
	private Vector3 getCurrentMousePosition()
	{	
		Vector2 mousePos = MouseOverHandler.getMousePosition();
		return new Vector3(mousePos.x,mousePos.y,Input.mousePosition.z);
	}
	
	// REMOVE NULL DELEGATES
	private void removeNullDelegates()
	{
		TouchHandlerObject[] arrObjects = gameObject.GetComponents<TouchHandlerObject>();
		if(arrObjects == null) return;
		int prevLength = arrObjects.Length;
		int currLength = prevLength;
		foreach(TouchHandlerObject _object in arrObjects)
			if(_object._delegate == null)	{Destroy(_object);currLength--;}
		if(currLength == 0 /*&& prevLength > currLength*/) { DestroyObject(gameObject); return; }
	}
	
	// DISPATCH TOUCH
	private void dispatchTouches()
	{
		if(Input.GetMouseButtonDown(0)) touchBegan();
		if(Input.GetMouseButtonUp(0)) touchEnded();
		if(Input.GetMouseButton(0)) touchMoved();
	}
	
	// DELEGATES
	private void touchBegan()
	{
		currentTouchPosition = getCurrentMousePosition();
		mIsTouchCancled = false;
		
		TouchHandlerObject[] arrObjects = gameObject.GetComponents<TouchHandlerObject>();
		if(arrObjects == null) return;
		foreach(TouchHandlerObject _object in arrObjects)
			if(_object._delegate != null) _object._delegate.touchBegan(getCurrentMousePosition());
	}	
	private void touchMoved()
	{
		if(mIsTouchCancled) return;
		if(Vector3.Distance(currentTouchPosition,getCurrentMousePosition()) > 1.0f*CommonData.getInversePTM())
		{		
			TouchHandlerObject[] arrObjects = gameObject.GetComponents<TouchHandlerObject>();
			if(arrObjects == null) return;
			foreach(TouchHandlerObject _object in arrObjects)
				if(_object._delegate != null) _object._delegate.touchMoved(getCurrentMousePosition());
			currentTouchPosition = getCurrentMousePosition();
		}
	}
	private void touchEnded()
	{
		if(mIsTouchCancled) return;
		TouchHandlerObject[] arrObjects = gameObject.GetComponents<TouchHandlerObject>();
		if(arrObjects == null) return;
		foreach(TouchHandlerObject _object in arrObjects)
			if(_object._delegate != null) _object._delegate.touchEnded(getCurrentMousePosition());
	}
}
