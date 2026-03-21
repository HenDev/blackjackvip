#pragma strict

function Start() {		
		/*
		AppsFlyer.setAppsFlyerKey("4EKZryyrnqCixGVSbzki6k");
		AppsFlyer.setAppID("902142283"); 
		AppsFlyer.trackAppLaunch();


		AppsFlyer.setCurrencyCode("USD");
		AppsFlyer.setCustomerUserID(SystemInfo.deviceUniqueIdentifier.ToString());
		AppsFlyer.trackEvent("aaa","bb");
		AppsFlyer.loadConversionData(this.name,"takeConversionData");*/
	}
	
	function takeConversionData(json){
		Debug.Log("AppsFlyer conversion data: "+json);
	}