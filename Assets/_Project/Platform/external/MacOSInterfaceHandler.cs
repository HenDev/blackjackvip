using UnityEngine;
using System;
using System.Collections;

using System.Runtime.InteropServices;

public class MacOSInterfaceHandler : ExternalInterfaceHandler 
{
#if UNITY_STANDALONE_OSX
	public delegate void UnityCallbackDelegate(IntPtr objectName,IntPtr commandName,IntPtr commandData);
	
	[DllImport("UnityMacPlugin")]
	public static extern void ConnectCallback([MarshalAs(UnmanagedType.FunctionPtr)]UnityCallbackDelegate callbackMethod);
	
	[DllImport("UnityMacPlugin")]
	public  static extern void SendUnityBridgeMessage(IntPtr objectName, IntPtr messageName, IntPtr parameterString);
	
	[DllImport ("UnityMacPlugin")]
	public static extern void InitPlugin();
	
	[DllImport ("UnityMacPlugin")]
	private static extern void _SendInAppRequest(bool isConsumable , int requestedID);
#endif
	void Awake()
	{
	Initialize();
	
		#if UNITY_STANDALONE_OSX
		ConnectCallback((objectName, commandName, commandData) => {
		string objName = Marshal.PtrToStringAuto(objectName);
		string commName = Marshal.PtrToStringAuto(commandName);
		string commData = Marshal.PtrToStringAuto(commandData);
		
		GameObject foundObject = GameObject.Find(objName);
		if(foundObject != null)
		{
			foundObject.SendMessage(commName,commData);
		}
		});
		#endif
		
	}
	
	void OnGUI() {
	 
        GUI.DrawTexture(rect1,bg);
		GUIUtility.RotateAroundPivot(rotAngle, pivotPoint);
        rotAngle += speed * Time.deltaTime;
		GUI.DrawTexture(rect2,_tRotateTexture);
	}
	
	public override bool SendRequest(eEXTERNAL_REQ_TYPE eRequestType, string strData, ReceivedCallBack callback)
	{
		if(callback != null)
		{
			mePrevRequestedType = eRequestType;
			mPrevRequestedData = strData;
			OnCallBack = callback;
		}
#if UNITY_STANDALONE_OSX
		switch(eRequestType)
		{
			
		case eEXTERNAL_REQ_TYPE.Initialize:
			InitPlugin();
			break;
		case eEXTERNAL_REQ_TYPE.GetInAppData:
		{
			IntPtr objectName = Marshal.StringToHGlobalAuto(this.name);
			IntPtr messageName = Marshal.StringToHGlobalAuto("Receiver");
			IntPtr parameterName = Marshal.StringToHGlobalAuto("GET");
			SendUnityBridgeMessage(objectName,messageName,parameterName);
		}
			break;
		case eEXTERNAL_REQ_TYPE.InAppConsumable:
		{
		
			IntPtr objectName = Marshal.StringToHGlobalAuto(this.name);
			IntPtr messageName = Marshal.StringToHGlobalAuto("Receiver");
			IntPtr parameterName = Marshal.StringToHGlobalAuto((strData));
			SendUnityBridgeMessage(objectName,messageName,parameterName);
		}
			break;
		
		case eEXTERNAL_REQ_TYPE.StartLoadingScreen:
		{
			enabled = true;
		}
			break;
		case eEXTERNAL_REQ_TYPE.StopLoadingScreen:
		{
			enabled = false;
		}
			break;
		default:
		{
		}
			break;
		}
#endif
		return false;
	}
}
