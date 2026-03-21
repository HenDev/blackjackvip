using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	int width = 200;
	int height = 50;
	int coins = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Label("Consumable Purchase"); 
		
		//......1..........//
		if(GUILayout.Button("1", GUILayout.Width(width), GUILayout.Height(height)))
		{
#if UNITY_STANDALONE_OSX
			// disable current Screen button
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StartLoadingScreen,"",null);
#endif
			
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.InAppConsumable,"1",InAppPurchase);
			
		}
		//......2..........//
		if(GUILayout.Button("2",GUILayout.Width(width), GUILayout.Height(height)))
		{
#if UNITY_STANDALONE_OSX
			// disable current Screen button
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StartLoadingScreen,"",null);
#endif
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.InAppConsumable,"2",InAppPurchase);
		}
		GUILayout.Label("NonConsumable Purchase");  //2
		//......3..........//
		if(GUILayout.Button("1",GUILayout.Width(width), GUILayout.Height(height)))
		{
#if UNITY_STANDALONE_OSX
			// disable current Screen button
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StartLoadingScreen,"",null);
#endif
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.InAppNonConsumable,"1",InAppPurchase);
		}
		//......4..........//
		if(GUILayout.Button("2",GUILayout.Width(width), GUILayout.Height(height)))
		{
#if UNITY_STANDALONE_OSX
			// disable current Screen button
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StartLoadingScreen,"",null);
#endif
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.InAppNonConsumable,"2",InAppPurchase);
		}
		
		//......5..........//
		if(GUILayout.Button("Submit Score _ int",GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Score,"100",null);
		}
		
		
		
		//......6..........//
		if(GUILayout.Button("Submit Score with LB",GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Score,"2_100",null);
		}
		
		//......7..........//
		if(GUILayout.Button("Submit Achievement",GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement,"1_80",null);
		}
		
		//......7..........//
		if(GUILayout.Button("Submit Achievement_100%",GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement,"1",null); // pass only acheivement number
		}
		
		if(GUILayout.Button("Submit Flurry",GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Flurry,"ssss",null);
		}
		
		
		
		
		GUILayout.EndVertical();
		
		GUILayout.Space(10);
		GUILayout.BeginVertical();
		if(GUILayout.Button("FullAds", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Show_FullScreen_Ads,"",null);
		}
		if(GUILayout.Button("TopBannerAds", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Show_Banner_Top_Ads,"",null);
		}
		if(GUILayout.Button("BottomBannerAds", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Show_Banner_Bottom_Ads,"",null);
		}
		if(GUILayout.Button("HideBannerAds", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Hide_Banner_Ads,"",null);
		}
		if(GUILayout.Button("Show PopUp", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.ShowPopup,"Test_Displayed",null);
		}
		
		if(GUILayout.Button("Show Score", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Show_Score,"",null);
		}
		if(GUILayout.Button("Show Achievent", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Show_Achievement,"",null);
		}
		
		if(GUILayout.Button("FaceBook", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Facebook,"",null);
		}
		
		if(GUILayout.Button("Twitter", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Twitter,"",null);
		}
		if(GUILayout.Button("MoreGames", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.MoreGames,"",null);
		}
		
		
		
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		if(GUILayout.Button("Application Quit", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.ApplicationQuit,"",null);
		}
		
		if(GUILayout.Button("Rate Application", GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.RateApplication,"",null);
		}
		
		if(GUILayout.Button("Submit Score _ float",GUILayout.Width(width), GUILayout.Height(height)))
		{
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Score,"100.05",null);
		}
		
		if(GUILayout.Button("Loading Screen",GUILayout.Width(width), GUILayout.Height(height)))
		{
			
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Initialize,"",null);
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StartLoadingScreen,"",null);
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.GetInAppData,"",GetInAppData);
		}
		GUILayout.EndVertical();
		GUILayout.Label(""+coins); 
		GUILayout.EndHorizontal();
	}
	
	
	void InAppPurchase(eEXTERNAL_REQ_TYPE reqType , string requestedId , string receivedStatus)
	{
#if UNITY_STANDALONE_OSX
			// enable current Screen button
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StopLoadingScreen,"",null);
#endif
		Debug.Log("InAppPurchase().....Data Received..."+receivedStatus);
		
		switch(reqType)
		{
		case eEXTERNAL_REQ_TYPE.InAppConsumable:
			if(receivedStatus == "true")
			{
				switch(requestedId)
				{
					case "1":
					Debug.Log("InAppConsumable: Request 1 completed");
					break;
					case "2":
					Debug.Log("InAppConsumable: Request 2 completed");
					break;
					case "3":
					break;
				}
			}
			else
			{
				// purchase failed..
			}
			break;
		case eEXTERNAL_REQ_TYPE.InAppNonConsumable:
			if(receivedStatus == "true")
			{
				switch(requestedId)
				{
					case "1":
					Debug.Log("InAppNonConsumable: Request 1 completed");
					break;
					case "2":
					Debug.Log("InAppNonConsumable: Request 1 completed");
					break;
					case "3":
					break;
				}
			}
			else
			{
				// purchase failed.
			}
			break;
		default:
			Debug.Log("InAppPurchase()....Invalid received Data");
			break;
			
		}
		
	}
	
	void GetInAppData(eEXTERNAL_REQ_TYPE reqType , string requestedId , string receivedStatus)
	{
		Debug.Log("GetInAppData()....Data Received_"+receivedStatus);
#if UNITY_STANDALONE_OSX
			// enable current Screen button
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.StopLoadingScreen,"",null);
#endif
		switch(reqType)
		{
			case eEXTERNAL_REQ_TYPE.GetInAppData:
			if(receivedStatus == "true")
			{
				// update currency gui text with stored player prefs string type.
				// use "kimberley Dynamic" for currency value available in resource folder.
			}
			else
			{
				// enable all button except inapp purchase button.
				// leave currency value empty.
			}
			break;
		default:
			Debug.Log("GetInAppData()....Invalid received Data");
			break;
		}
	}
}
