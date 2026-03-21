using UnityEngine;
using System.Collections;

public class TouchDelegate : MonoBehaviour
{
	// Use this for initialization
	public TouchDelegateInternal _Instance;
	void Start ()
	{
		if(_Instance == null)
			_Instance = TouchDelegateInternal.Instance;
	}
	
	// Update is called once per frame
	void Update () 
	{
		#if UNITY_EDITOR
			if(_Instance != null)
				_Instance.HandleMouseClick();
	    #elif UNITY_IPHONE
			_Instance.HandleTouch();
		#else
			_Instance.HandleMouseClick();
	    #endif		
	}
	
	
	// Make sure the instance isn't referenced anymore when the user quit, just in case.
    private void OnApplicationQuit()
    {
        _Instance = null;
    }
}
