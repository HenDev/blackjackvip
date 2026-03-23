using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TTF_Timer : MonoBehaviour {
	
	public bool resetTimer = false; 
	public Text mCurrentDateTimeText = null;
	public Text mSavedDateTimeText = null;
	public Text mDateTimePassedText = null;
	
	// Update is called once per frame
	void Update () {
		if(resetTimer)
		{
			resetTimer = false;
			ApplicationTimer.resetSavedTimer();
		}
		mCurrentDateTimeText.text = logDateAndTime();
		mSavedDateTimeText.text = logSavedDateTime();
		mDateTimePassedText.text = logDaysAndTimePassed();
	}
	
	// Logs
	string logDateAndTime()
	{
		int year = System.DateTime.Now.Year;
		int month = System.DateTime.Now.Month;
		int day = System.DateTime.Now.Day;
		int hr = System.DateTime.Now.Hour;
		int min = System.DateTime.Now.Minute;
		int sec = System.DateTime.Now.Second;
		return day+"/"+month+"/"+year+" : "+hr+"-"+min+"-"+sec;
	}
	string logDaysAndTimePassed()
	{
		int year = ApplicationTimer.getNoOfYearsPassed();
		int month = ApplicationTimer.getNoOfMonthsPassed();
		int day = ApplicationTimer.getNoOfDaysPassed();
		int hr = ApplicationTimer.getNoOfHoursPassed();
		int min = ApplicationTimer.getNoOfMinutesPassed();
		int sec = ApplicationTimer.getNoOfSecondsPassed();
		return day+"/"+month+"/"+year+" : "+hr+"-"+min+"-"+sec;		
	}
	string logSavedDateTime()
	{		
		int year = PlayerPrefs.GetInt(ApplicationTimer.saveYearKey);
		int month = PlayerPrefs.GetInt(ApplicationTimer.saveMonthKey);
		int day = PlayerPrefs.GetInt(ApplicationTimer.saveDayKey);
		int hr = PlayerPrefs.GetInt(ApplicationTimer.saveHrKey);
		int min = PlayerPrefs.GetInt(ApplicationTimer.saveMinKey);
		int sec = PlayerPrefs.GetInt(ApplicationTimer.saveSecKey);
		return day+"/"+month+"/"+year+" : "+hr+"-"+min+"-"+sec;		
	}
}
