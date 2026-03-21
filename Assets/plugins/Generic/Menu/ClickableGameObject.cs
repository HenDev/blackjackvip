using UnityEngine;
using System.Collections;

public class ClickableGameObject : MonoBehaviour 
{
	public TouchDelegate mTouchDelegate = null;
	public delegate void callBackDelegate(GameObject pObj,string pTagString);
	public callBackDelegate _CallBack = null;
	public string _ObjectTag = null;
	
	private bool mbIsActive = false;
	
	// Use this for initialization
	void Start ()
	{
		//MAKE SURE IT IS SET THROUGH INSPECTOR ALSO
		gameObject.tag = "ClickableGameObject";
	}
	
	//Called By TouchDelegate Singleton
	public void onClicked(Vector3 pPosition)
	{
		if(_CallBack != null)
			_CallBack(gameObject,_ObjectTag);
	}
	
	//So That it doesn't get called after deallocation
	void OnDisable()
	{
		if(mTouchDelegate != null && mbIsActive)
		{
			mTouchDelegate._Instance._ToucheBegan -= onClicked;
			mbIsActive = false;
		}
	}
	
	//Just incase the object is not actually destroyed but just disabled
	void OnEnable()
	{
		if(mTouchDelegate != null && mTouchDelegate._Instance != null)
		{
			mTouchDelegate._Instance._ToucheBegan += onClicked;
			mbIsActive = true;
		}
	}
}
