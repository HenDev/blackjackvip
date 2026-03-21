using UnityEngine;
using System.Collections;

/*
 * UNITY_EDITOR				Define for calling Unity Editor scripts from your game code.
 * UNITY_STANDALONE_OSX		Platform define for compiling/executing code specifically for Mac OS (This includes Universal, PPC and Intel architectures).
 * UNITY_DASHBOARD_WIDGET	Platform define when creating code for Mac OS dashboard widgets.
 * UNITY_STANDALONE_LINUX	Use this when you want to compile/execute code for Linux stand alone applications.
 * UNITY_STANDALONE			Use this to compile/execute code for any standalone platform (Mac, Windows or Linux).
 * UNITY_WEBPLAYER			Platform define for web player content (this includes Windows and Mac Web player executables).
 * UNITY_WII				Platform define for compiling/executing code for the Wii console.
 * UNITY_IPHONE				Platform define for compiling/executing code for the iPhone platform.
 * UNITY_ANDROID			Platform define for the Android platform.
 * UNITY_PS3				Platform define for running PlayStation 3 code.
 * UNITY_XBOX360			Platform define for executing Xbox 360 code.
 * UNITY_NACL				Platform define when compiling code for Google native client (this will be set additionally to UNITY_WEBPLAYER).
 * UNITY_FLASH				Platform define when compiling code for Adobe Flash.
 * UNITY_BLACKBERRY			Platform define for a Blackberry10 device.
 * UNITY_WP8				Platform define for Windows Phone 8.
 * UNITY_METRO				Platform define for Windows Store Apps (additionally NETFX_CORE is defined when compiling C# files against .NET Core).
*/

public enum MyDeviceType {UnityEditor=0, Ipod, Iphone, Ipad, Android, WindowsPhone, Blackberry, Flash, UnityWeb, PC, Unknown}; 

public class DeviceModelInfo 
{
	public static MyDeviceType GetCurrentDeviceType() 
	{
		#if UNITY_EDITOR
				return MyDeviceType.UnityEditor;
		#elif UNITY_IPHONE
			if(SystemInfo.deviceModel.Contains("iPod"))
				return MyDeviceType.Ipod;
			else if(SystemInfo.deviceModel.Contains("iPhone"))
				return MyDeviceType.Iphone;
			else if(SystemInfo.deviceModel.Contains("iPad"))
				return MyDeviceType.Ipad;
			else
				return MyDeviceType.UnityEditor;
		#elif UNITY_ANDROID
				return MyDeviceType.Android;
		#elif UNITY_WP8
				return MyDeviceType.WindowsPhone;
		#elif UNITY_BLACKBERRY
				return MyDeviceType.Blackberry;
		#elif UNITY_FLASH
				return MyDeviceType.Flash;
		#elif UNITY_WEBPLAYER
				return MyDeviceType.UnityWeb;
		#elif UNITY_STANDALONE
				return MyDeviceType.PC;
		#endif
		return MyDeviceType.Unknown;
	}
	
	public static bool getIsIosDevice()
	{
		MyDeviceType currentDeviceType = GetCurrentDeviceType();
		bool isIosDevice = false;
		switch(currentDeviceType)
		{
		case MyDeviceType.Ipad:
		case MyDeviceType.Iphone:
		case MyDeviceType.Ipod:
			isIosDevice = true;
			break;
		}
		return isIosDevice;
	}
	
	public static bool getIsIphone5()
	{
		MyDeviceType currentDeviceType = GetCurrentDeviceType();
		switch(currentDeviceType)
		{
		case MyDeviceType.Iphone:
		case MyDeviceType.Ipod:
			if(Screen.height > 1000 || Screen.width > 1000)			return true;
			break;
		}
//		if(currentDeviceType == MyDeviceType.UnityEditor) return true; // Temp
		return false;
	}
	
}
