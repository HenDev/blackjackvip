using UnityEngine;
using System.Collections;

public class BonusHandler : MonoBehaviour {
	
	public int BonusChipsCount = 0;
	
	public int MinHoursToBonus = 0;
	public int MinMinutesToBonus = 0;
	public int MinSecondsToBonus = 0;
	
	public int timerHours = 0;
	public int timerMinutes = 0;
	public float timerSeconds = 0;
	
	public bool isBonusActive = false;
	
	// Use this for initialization
	void Start () {
//		LocalNotificationHandler.mInstance.createNotificationForBlackjack(LocalNotificationHandler.mInstance.localNotificationMessageforBonusNotCollected, 0, 2, 0);
		LocalNotificationHandler.mInstance.createNotificationForBlackjack(LocalNotificationHandler.mInstance.localNotificationMessageforBonusNotCollected, Mathf.CeilToInt(MinHoursToBonus/2), Mathf.CeilToInt(MinMinutesToBonus/2), Mathf.CeilToInt(MinSecondsToBonus/2));
		isBonusActive = ApplicationTimer.getNoOfYearsPassed() > 0 || ApplicationTimer.getNoOfMonthsPassed()%12 > 0 || ApplicationTimer.getNoOfDaysPassed()%30 > 0; 
		if(isBonusActive) return;
		
		int hours = ApplicationTimer.getNoOfHoursPassed()%24;
		int minutes = ApplicationTimer.getNoOfMinutesPassed()%60;
		int seconds = ApplicationTimer.getNoOfSecondsPassed()%60;
		
		Debug.Log(hours+":"+minutes+":"+seconds);
		
		if(hours > MinHoursToBonus) { isBonusActive = true; return; }
		if(hours == MinHoursToBonus && minutes > MinMinutesToBonus)  { isBonusActive = true; return; }
		if(hours == MinHoursToBonus && minutes == MinMinutesToBonus && seconds > MinSecondsToBonus)  { isBonusActive = true; return; }
		
		timerHours = MinHoursToBonus;
		timerMinutes = MinMinutesToBonus;
		timerSeconds = MinSecondsToBonus;
		
		if(timerSeconds == 0)
		{
			timerMinutes--;				
			if(timerMinutes < 0)
			{
				timerHours--;
				if(timerHours < 0) timerHours = 0;					
				timerMinutes = 59;
			}		
			timerSeconds = 60;
		}
		
		timerSeconds -= seconds;
		timerMinutes -= minutes;
		timerHours -= hours;
//		LocalNotificationHandler.mInstance.createNotificationForBlackjack(LocalNotificationHandler.mInstance.localNotificationMessageforBonusReady, 0, 2, 0);
		LocalNotificationHandler.mInstance.createNotificationForBlackjack(LocalNotificationHandler.mInstance.localNotificationMessageforBonusReady, timerHours, timerMinutes+10, Mathf.CeilToInt(timerSeconds));
		Debug.Log(timerHours+":"+timerMinutes+":"+timerSeconds);		
	}	
	
	public void resetTimer()
	{
		isBonusActive = false;
		timerHours = MinHoursToBonus;
		timerMinutes = MinMinutesToBonus;
		timerSeconds = MinSecondsToBonus;
		ApplicationTimer.resetSavedTimer();
	}
	
	// Update is called once per frame
	void Update () { 
		if(!isBonusActive)
		{
			timerSeconds -= Time.deltaTime;
			if(timerSeconds < 0.0f)
			{
				timerSeconds = 60.0f;
				timerMinutes--;
				if(timerMinutes < 0)
				{
					timerMinutes = 59;
					timerHours--;
					if(timerHours < 0)
					{
						timerSeconds = 0.0f;
						timerMinutes = 0;
						timerHours = 0;
						activateClaimBonus();
					}
				}
			}
			BJ_BottomBar.mInstance.setTimer(timerHours, timerMinutes, Mathf.FloorToInt(timerSeconds));
			PopUpManager.mInstance.moreChipsHandler.GetComponent<MoreChipsHandler>().setTimer(timerHours, timerMinutes, Mathf.FloorToInt(timerSeconds));
		}		
	}		
	
	public void activateClaimBonus()
	{
		isBonusActive = true;
		BJ_BottomBar.mInstance.toggleClaimBonusDisplay();
		PopUpManager.mInstance.moreChipsHandler.GetComponent<MoreChipsHandler>().toggleBonusDisplay();
	}
	
	public void claimBonus()
	{		
		BJ_BottomBar.mInstance.updateChipsBalance(BonusChipsCount);
		
		resetTimer();
		BJ_BottomBar.mInstance.toggleClaimBonusDisplay();
		PopUpManager.mInstance.moreChipsHandler.GetComponent<MoreChipsHandler>().toggleBonusDisplay();
		LocalNotificationHandler.mInstance.createNotificationForBlackjack(LocalNotificationHandler.mInstance.localNotificationMessageforBonusReady, MinHoursToBonus, MinMinutesToBonus+10, MinSecondsToBonus);
	}
}
