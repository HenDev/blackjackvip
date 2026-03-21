using UnityEngine;
using System.Collections;

// MOUSEOVER SENSOR TYPES
public enum MouseOverSensorType {Rectangle,Circle};
// CALLBACK DELEGATES
public delegate void CallBack();
public delegate void CallBackWithSender(GameObject _object);

// MOUSEOVER SENSOR HANDLER DELEGATE
public class MouseOverHandlerDelegate : MonoBehaviour {
	public virtual void onMouseOverBegan(GameObject _object) {}
	public virtual void onMouseOverEnded(GameObject _object) {}	
	public virtual void onMouseOver(GameObject _object) {}
}

// STORES THE DATA OF EACH GAMEOBJECT
public class MouseOverHandlerObject : MonoBehaviour {
	public MouseOverSensorType _sensorType;
	public GameObject _gameObject = null;
	public Vector2 _sensorArea = Vector2.zero;
	public float _senseRadius = 0.0f;
	public MouseOverHandlerDelegate _delegate = null;
	public bool _isMouseOver = false;
	public CallBackWithSender _callBackWithSender = null;
	public CallBack _callBack = null;
}

// MOUSE OVER SENSOR CLASS
public class MouseOverHandler : MonoBehaviour {	
	
	// Static Object for handling
	private static MouseOverHandler mSharedHandler = null;
//	private static float mfInversePTMRatio = 0.03125f; // 1 divided by PTM RATIO (unity 1 unit == 32 pixcel)
//	private static Vector2 win_sz = new Vector2(1024.0f, 768.0f);
//	private static float widthRatio = Screen.width/win_sz.x;
//	private static float heightRatio = Screen.height/win_sz.y;
	// Use this for initialization
	void Start () { }	
	// Update is called once per frame
	void Update () { removeNullGameObjects(); senseMouseOver(); }
	
	// STATIC HANDLER
	public static MouseOverHandler sharedHandler()
	{
		if(mSharedHandler == null)
		{
			GameObject mouseHandler = new GameObject();
			mouseHandler.name = "MouseOverHandler";
			mSharedHandler = mouseHandler.AddComponent<MouseOverHandler>();
		}
		return mSharedHandler;
	}
	
	// ADDING OBJECTS TO SENSOR TO SENSE
	private MouseOverHandlerObject createMouseOverHandlerObject(GameObject _gameObject, MouseOverHandlerDelegate _delegate, float senseRadius, MouseOverSensorType _sensorType)
	{
		MouseOverHandlerObject _dataObject = gameObject.AddComponent<MouseOverHandlerObject>();
		_dataObject._delegate = _delegate;
		_dataObject._gameObject = _gameObject;
		_dataObject._senseRadius = senseRadius;
		_dataObject._sensorType = _sensorType;
//		switch(_sensorType)		{
//		case MouseOverSensorType.Circle: Debug.Log("Circular SensorAdded"); break;
//		case MouseOverSensorType.Rectangle: Debug.Log("Rectangle SensorAdded"); break;		}
		return _dataObject;
	}
	public void addObjectToSensor(GameObject _gameObject, MouseOverHandlerDelegate _delegate, Vector2 senseArea, CallBackWithSender callBack)
	{
		if(checkObjectAlreadyAdded(_gameObject)) return;
		float senseRadius = Mathf.Sqrt(Mathf.Pow(senseArea.x,2)+Mathf.Pow(senseArea.y,2))/2.0f; // diognal length / 2.0f
		MouseOverHandlerObject _dataObject = createMouseOverHandlerObject(_gameObject, _delegate, senseRadius, MouseOverSensorType.Rectangle);
		_dataObject._sensorArea = senseArea;
		_dataObject._callBackWithSender = callBack;
	}	
	public void addObjectToSensor(GameObject _gameObject, MouseOverHandlerDelegate _delegate, Vector2 senseArea, CallBack callBack)
	{
		if(checkObjectAlreadyAdded(_gameObject)) return;
		float senseRadius = Mathf.Sqrt(Mathf.Pow(senseArea.x,2)+Mathf.Pow(senseArea.y,2))/2.0f; // diognal length / 2.0f
		MouseOverHandlerObject _dataObject = createMouseOverHandlerObject(_gameObject, _delegate, senseRadius, MouseOverSensorType.Rectangle);
		_dataObject._sensorArea = senseArea;
		_dataObject._callBack = callBack;
	}	
	public void addObjectToSensor(GameObject _gameObject, MouseOverHandlerDelegate _delegate, Vector2 senseArea)
	{
		if(checkObjectAlreadyAdded(_gameObject)) return;
		float senseRadius = Mathf.Sqrt(Mathf.Pow(senseArea.x,2)+Mathf.Pow(senseArea.y,2))/2.0f; // diognal length / 2.0f
		MouseOverHandlerObject _dataObject = createMouseOverHandlerObject(_gameObject, _delegate, senseRadius, MouseOverSensorType.Rectangle);
		_dataObject._sensorArea = senseArea;
	}
	public void addObjectToSensor(GameObject _gameObject, MouseOverHandlerDelegate _delegate, float senseRadius, CallBackWithSender callBack)
	{
		if(checkObjectAlreadyAdded(_gameObject)) return;
		MouseOverHandlerObject _dataObject = createMouseOverHandlerObject(_gameObject, _delegate, senseRadius, MouseOverSensorType.Circle);
		_dataObject._callBackWithSender = callBack;
	}
	public void addObjectToSensor(GameObject _gameObject, MouseOverHandlerDelegate _delegate, float senseRadius, CallBack callBack)
	{
		if(checkObjectAlreadyAdded(_gameObject)) return;
		MouseOverHandlerObject _dataObject = createMouseOverHandlerObject(_gameObject, _delegate, senseRadius, MouseOverSensorType.Circle);
		_dataObject._callBack = callBack;
	}
	public void addObjectToSensor(GameObject _gameObject, MouseOverHandlerDelegate _delegate, float senseRadius)
	{
		if(checkObjectAlreadyAdded(_gameObject)) return;
		createMouseOverHandlerObject(_gameObject, _delegate, senseRadius, MouseOverSensorType.Circle);
	}
	public void addObjectToSensor(GameObject _gameObject, MouseOverHandlerDelegate _delegate)
	{
		BoxCollider boxCollider = _gameObject.GetComponent<BoxCollider>();
		SphereCollider spheareCollider = _gameObject.GetComponent<SphereCollider>();
		if(spheareCollider != null) addObjectToSensor(_gameObject, _delegate, spheareCollider.radius);
		else if(boxCollider != null) addObjectToSensor(_gameObject, _delegate, new Vector2(boxCollider.size.x, boxCollider.size.y));
		else addObjectToSensor(_gameObject, _delegate, new Vector2(_gameObject.transform.localScale.x,_gameObject.transform.localScale.y));
	}
	// Check is Object Already Present
	private bool checkObjectAlreadyAdded(GameObject _gameObject)
	{
		MouseOverHandlerObject[] arrObjects = gameObject.GetComponents<MouseOverHandlerObject>();
		if(arrObjects == null) return false;		
		foreach(MouseOverHandlerObject _object in arrObjects)
			if(_object._gameObject == _gameObject) {Debug.Log("GameObject Already Added");return true;}
		return false;
	}
	// Remove GameObject From Sensor
	public void removeObjectFromSensor(GameObject _gameObject)
	{
		MouseOverHandlerObject[] arrObjects = gameObject.GetComponents<MouseOverHandlerObject>();
		if(arrObjects == null) return;		
		foreach(MouseOverHandlerObject _object in arrObjects)
			if(_object._gameObject == _gameObject) {_object._gameObject = null;break;}
	}
	
	// GETTERS
	public bool isMouseOverAnyObject()
	{
		MouseOverHandlerObject[] arrObjects = gameObject.GetComponents<MouseOverHandlerObject>();
		if(arrObjects == null) return false;		
		foreach(MouseOverHandlerObject _object in arrObjects)
			if(_object._isMouseOver) return true;
		return false;
	}
	
	// REMOVE NULL OBJECTS
	private void removeNullGameObjects()
	{
		MouseOverHandlerObject[] arrObjects = gameObject.GetComponents<MouseOverHandlerObject>();
		if(arrObjects == null) return;
		int prevLength = arrObjects.Length;
		int currLength = prevLength;
		foreach(MouseOverHandlerObject _object in arrObjects)
			if(_object._gameObject == null)	{Destroy(_object);currLength--;}
		if(currLength == 0 /*&& prevLength > currLength*/) { DestroyObject(gameObject); return; }
	}
	
	// ITERATING ALL THE OBJECTS FOR MOUSEOVERS AND MOUSEOVER ENDED
	private void senseMouseOver()
	{
		MouseOverHandlerObject[] arrObjects = gameObject.GetComponents<MouseOverHandlerObject>();
		if(arrObjects == null) return;
		foreach(MouseOverHandlerObject _object in arrObjects)
		{
			if(_object._gameObject == null) continue;
			// Checking for MouseOver Begin
			if(!_object._isMouseOver && isPointNearMouse(_object._gameObject.transform.position, _object._senseRadius))
			{
				if(_object._sensorType == MouseOverSensorType.Circle) mouseOverBegan(_object);
				else if(_object._sensorType == MouseOverSensorType.Rectangle && 
					(isMouseInsideRect(_object._gameObject.transform.position, _object._sensorArea))) mouseOverBegan(_object);
			}
			// Checking for MouseOver Ended
			else if(_object._isMouseOver && !isPointNearMouse(_object._gameObject.transform.position, _object._senseRadius)) mouseOverEnded(_object);
			// Mouse Over
			else if(_object._isMouseOver && isPointNearMouse(_object._gameObject.transform.position, _object._senseRadius)) mouseOver(_object);
		}
	}
	
	private bool isMouseInsideRect(Vector2 position, Vector2 size)
	{
		Vector4 rect = new Vector4(position.x - size.x/2, position.y - size.y/2, size.x, size.y);
		Vector3 point = getMousePosition();
		return (point.x > rect.x && point.x < rect.x + rect.z && point.y > rect.y && point.y < rect.y + rect.w);
	}
	
	public static Vector2 getMousePosition()
	{
//		float xPos = ((Input.mousePosition.x - Screen.width/2)*mfInversePTMRatio)/widthRatio + Camera.mainCamera.gameObject.transform.position.x;
//		float yPos = ((Input.mousePosition.y - Screen.height/2)*mfInversePTMRatio)/heightRatio + Camera.mainCamera.gameObject.transform.position.y;
//		return new Vector2(xPos, yPos);
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
	
	private bool isPointNearMouse(Vector2 point, float radius)
	{
		return Mathf.Abs(Vector2.Distance(point, getMousePosition())) <= radius;
	}
	
	// DELEGATES
	private void mouseOverBegan(MouseOverHandlerObject _object)
	{
		_object._isMouseOver = true;
		if(_object._delegate != null) _object._delegate.onMouseOverBegan(_object._gameObject);
		if(_object._callBack != null) _object._callBack();
		if(_object._callBackWithSender != null) _object._callBackWithSender(_object._gameObject);
	}
	private void mouseOverEnded(MouseOverHandlerObject _object)
	{
		_object._isMouseOver = false;
		if(_object._delegate != null) _object._delegate.onMouseOverEnded(_object._gameObject);
		if(_object._callBack != null) _object._callBack();
		if(_object._callBackWithSender != null) _object._callBackWithSender(_object._gameObject);
	}
	private void mouseOver(MouseOverHandlerObject _object)
	{
		if(_object._delegate != null) _object._delegate.onMouseOver(_object._gameObject);
	}
}
