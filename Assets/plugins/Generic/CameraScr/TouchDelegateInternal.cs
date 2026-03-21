using UnityEngine;
using System.Collections;

public class TouchDelegateInternal
{
	private static volatile TouchDelegateInternal _Instance;
	private static object _lock = new object();

	public delegate void touchDelegate(Vector3 position);
	public touchDelegate _ToucheBegan;
	public touchDelegate _ToucheMoved;
	public touchDelegate _ToucheEnded;
	
	Vector3 mvClickPosition;
	RaycastHit mRayhit;
	
	public static TouchDelegateInternal Instance
	{
		get
		{
			if (_Instance == null)
			{
				lock(_lock)
				{
					if (_Instance == null)
						_Instance = new TouchDelegateInternal();
				}
			}
			return _Instance;
		}
	}
	
	
	void Awake() 
	{
		if(_Instance == null)
			_Instance = this;
	}
	
	void Start ()
	{
	
	}
		
	public void HandleTouch()
	{
		int touchCount = Input.touchCount;
	    if(touchCount > 0)
	    {
			Touch touch = Input.GetTouch(touchCount-1);
			
			//IF THE BUTTON IS NOT YET CLICKED
			if (touch.phase == TouchPhase.Began)
	        {
				mvClickPosition = touch.position;
				mvClickPosition = Camera.main.ScreenToWorldPoint(mvClickPosition);
				if(Physics.Raycast(mvClickPosition,new Vector3(0,0,1),out mRayhit,10))
				{
					if(mRayhit.collider.gameObject.tag == "Profile")
					{
						GameObject tGameObj = mRayhit.collider.gameObject;
						ClickableGameObject scr = tGameObj.GetComponent<ClickableGameObject>();
						if(scr != null)
						{
							scr.onClicked(mvClickPosition);
						}
					}
				}
	        }
		}
	}
	
	public void HandleMouseClick()
	{
		if (Input.GetMouseButtonDown(0))
	    {
			mvClickPosition = Input.mousePosition;
			mvClickPosition = Camera.main.ScreenToWorldPoint(mvClickPosition);
			
			if(Physics.Raycast(mvClickPosition,new Vector3(0,0,1),out mRayhit,50))
			{
				if(mRayhit.collider.gameObject.tag == "ClickableGameObject")
				{
					GameObject tGameObj = mRayhit.collider.gameObject;
					ClickableGameObject scr = tGameObj.GetComponent<ClickableGameObject>();
					if(scr != null)
					{
						scr.onClicked(mvClickPosition);
					}
				}
			}
		}
	}	
}