using UnityEngine;
using System.Collections;

public class CommonCamScr : MonoBehaviour {

	// Use this for initialization
	public Transform _clientTransform;
	public bool _removeVsync = true;
	public bool _fastestFrameRate = true;
	// test
	public Vector3 _camOffset = Vector3.zero;
	public bool _fpsEnabled = false;
	public bool _statsDisplay = false;
	public bool _smoothFollow = false;
	public int _defaultFrameRate = 50;
	int _curFrameRate;
	
	public bool _limitX;
	public Vector2 _camLimitX = new Vector3(0,0); //Lower/Upper
	
	public bool _limitY;
	public Vector2 _camLimitY = new Vector3(0,0);
	
	
 	private Vector3 speedVec = new Vector3(1,0,0);
 	private Vector3 previousPos = new Vector3(0,0,0);
	
	void Start () 
	{
		if(_fastestFrameRate)
		{
			_defaultFrameRate = -1;
			Application.targetFrameRate = -1;
		}
		else
			Application.targetFrameRate = _defaultFrameRate;
		_curFrameRate = _defaultFrameRate;

		
		if(_removeVsync)
			QualitySettings.vSyncCount = 0;
		
		if(_fpsEnabled)
			gameObject.AddComponent<FpsScr>();
		
		if(_statsDisplay)
			gameObject.AddComponent<DetectLeaks>();
		
		previousPos = transform.position;
	}
		
	// Update is called once per frame
	void Update () 
	{		
		if(_curFrameRate != _defaultFrameRate)
		{
			if(_defaultFrameRate < 0)
				_defaultFrameRate = 0;
			_curFrameRate = _defaultFrameRate;
			Application.targetFrameRate = _defaultFrameRate;
		}
		
		if(_clientTransform != null)
		{
			Vector3 newPos = _clientTransform.position + _camOffset;
			if(_limitX)
			{
				if(newPos.x < _camLimitX.x)
					newPos.x = _camLimitX.x;
				else if(newPos.x > _camLimitX.y)
					newPos.x = _camLimitX.y;
			}
			if(_limitY)
			{
				if(newPos.y < _camLimitY.x)
					newPos.y = _camLimitY.x;
				else if(newPos.y > _camLimitY.y)
					newPos.y = _camLimitY.y;
			}
			
			if(_smoothFollow)
			{
		  		newPos = Vector3.SmoothDamp(previousPos,newPos,ref speedVec, 0.2f);
	  			previousPos = newPos;
			}
			
			transform.position = newPos;
		}
	}
	
}
