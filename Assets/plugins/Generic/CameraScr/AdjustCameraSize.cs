using UnityEngine;
using System.Collections;

public class AdjustCameraSize : MonoBehaviour {
	
	public float cameraSizeIpad = 12;
	public float cameraSizeIpod = 12;
	public float cameraSizeIphone5 = 12;
	
	// Use this for initialization
	void Start () {
			switch(DeviceModelInfo.GetCurrentDeviceType())
			{
			case MyDeviceType.Ipad:
			case MyDeviceType.PC:
				GetComponent<Camera>().orthographicSize = cameraSizeIpad;
				break;
			case MyDeviceType.Iphone:
			case MyDeviceType.Ipod:
				Debug.Log(Screen.height);
				Debug.Log(Screen.width);
				if(Screen.height > 1000 || Screen.width > 1000)
				GetComponent<Camera>().orthographicSize = cameraSizeIphone5;
				else
				GetComponent<Camera>().orthographicSize = cameraSizeIpod;
				break;
			default:
				GetComponent<Camera>().orthographicSize = cameraSizeIpad;
				break;
			}
		
		
//		GetComponent<Camera>().orthographicSize = Screen.height/64;
	}
}
