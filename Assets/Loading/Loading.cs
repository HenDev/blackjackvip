using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour {
	
	public static bool IsAdsRemoved 
	{
		get { return PlayerPrefs.GetInt("IsAdsRemoved")==1;}
		set { PlayerPrefs.SetInt("IsAdsRemoved", value?1:0); }
	}
	
	
	// Use this for initialization
	void Start () { firstTimeInitialize(); StartCoroutine(Initialize()); }
	
	IEnumerator Initialize()
	{
		yield return new WaitForFixedUpdate();
		Application.LoadLevel(1);
	}
	
	void firstTimeInitialize()
	{
		// First Time Initialization
		if(PlayerPrefs.GetInt("FirstTimeLoad") == 0)
		{
			PlayerPrefs.SetInt("FirstTimeLoad",1);
			PlayerPrefs.SetInt("IsSoundEnable", 1);
			PlayerPrefs.SetInt("IsMusicEnable", 1);
			
			PlayerPrefs.SetInt("PlayerChipBalance", 2000);			
			ApplicationTimer.resetSavedTimerBefore(0, 0, 5);
			
			MoreChipsHandler.RateReviewDone = false;
			IsAdsRemoved = false;					
		}
		
		// Update
		if(PlayerPrefs.GetInt("FirstTimeLoadUpdate") == 0)
		{
			PlayerPrefs.SetInt("FirstTimeLoadUpdate",1);
			PlayerPrefs.SetInt("LocalNotificationEnabled", 1);
			
			// Daily Bonus
			int currentDay = System.DateTime.Now.Day;
			int currentMonth = System.DateTime.Now.Month;
			int currentYear = System.DateTime.Now.Year;			
			PlayerPrefs.SetInt("DailyBonusDay", currentDay);
			PlayerPrefs.SetInt("DailyBonusMonth", currentMonth);
			PlayerPrefs.SetInt("DailyBonusYear", currentYear);
			PlayerPrefs.SetInt("NoOfContiniousDays", 0);
		}
	}
}
