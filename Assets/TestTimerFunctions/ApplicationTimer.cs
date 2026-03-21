using UnityEngine;
using System.Collections;

public class ApplicationTimer : MonoBehaviour {
	
	public static string saveYearKey = "AppTimer_Year";
	public static string saveMonthKey = "AppTimer_Month";	
	public static string saveDayKey = "AppTimer_Day";
	public static string saveHrKey = "AppTimer_Hr";
	public static string saveMinKey = "AppTimer_Min";
	public static string saveSecKey = "AppTimer_Sec";
	public bool isLogCurrentDateTime = false;
	public bool isLogDateTimePassed = false;
	// Update is called once per frame
	void Update () {
		if(isLogCurrentDateTime) logDateAndTime();
		if(isLogDateTimePassed) logDaysAndTimePassed();
	}
	// Logs
	void logDateAndTime()
	{
		int year = System.DateTime.Now.Year;
		int month = System.DateTime.Now.Month;
		int day = System.DateTime.Now.Day;
		int hr = System.DateTime.Now.Hour;
		int min = System.DateTime.Now.Minute;
		int sec = System.DateTime.Now.Second;
		Debug.Log("DateTime : "+day+"/"+month+"/"+year+" : "+hr+"-"+min+"-"+sec);
	}
	void logDaysAndTimePassed()
	{
		int year = getNoOfYearsPassed();;
		int month = getNoOfMonthsPassed();
		int day = getNoOfDaysPassed();
		int hr = getNoOfHoursPassed();
		int min = getNoOfMinutesPassed();
		int sec = getNoOfSecondsPassed();
		Debug.Log("DateTimePassed : "+day+"/"+month+"/"+year+" : "+hr+"-"+min+"-"+sec);		
	}
	
	// Saves the current Date and Time to the Prefab.
	// Call this function to detect the time taken from now
	public static void resetSavedTimer()
	{				
		PlayerPrefs.SetInt(saveYearKey,System.DateTime.Now.Year);
		PlayerPrefs.SetInt(saveMonthKey,System.DateTime.Now.Month);
		PlayerPrefs.SetInt(saveDayKey,System.DateTime.Now.Day);
		PlayerPrefs.SetInt(saveHrKey,System.DateTime.Now.Hour);
		PlayerPrefs.SetInt(saveMinKey,System.DateTime.Now.Minute);
		PlayerPrefs.SetInt(saveSecKey,System.DateTime.Now.Second);
	}
	public static void resetSavedTimerBefore(int secOffset, int minOffset, int hourOffset)
	{				
		System.DateTime currentDateTime = System.DateTime.Now.Subtract(new System.TimeSpan(hourOffset, minOffset, secOffset));
		
		PlayerPrefs.SetInt(saveYearKey,currentDateTime.Year);
		PlayerPrefs.SetInt(saveMonthKey,currentDateTime.Month);
		PlayerPrefs.SetInt(saveDayKey,currentDateTime.Day);
		PlayerPrefs.SetInt(saveHrKey,currentDateTime.Hour);
		PlayerPrefs.SetInt(saveMinKey,currentDateTime.Minute);
		PlayerPrefs.SetInt(saveSecKey,currentDateTime.Second);
	}
	public static void resetSavedTimerAfter(int secOffset, int minOffset, int hourOffset)
	{				
		System.DateTime currentDateTime = System.DateTime.Now.Add(new System.TimeSpan(hourOffset, minOffset, secOffset));
		
		PlayerPrefs.SetInt(saveYearKey,currentDateTime.Year);
		PlayerPrefs.SetInt(saveMonthKey,currentDateTime.Month);
		PlayerPrefs.SetInt(saveDayKey,currentDateTime.Day);
		PlayerPrefs.SetInt(saveHrKey,currentDateTime.Hour);
		PlayerPrefs.SetInt(saveMinKey,currentDateTime.Minute);
		PlayerPrefs.SetInt(saveSecKey,currentDateTime.Second);
	}
	
	// Getters
	public static int getNoOfSecondsPassed()
	{	
		int dYear = System.DateTime.Now.Year - PlayerPrefs.GetInt(saveYearKey);
		int dMonth = System.DateTime.Now.Month - PlayerPrefs.GetInt(saveMonthKey);
		int dDay = System.DateTime.Now.Day - PlayerPrefs.GetInt(saveDayKey);
		int dHour = System.DateTime.Now.Hour - PlayerPrefs.GetInt(saveHrKey);
		int dMin = System.DateTime.Now.Minute - PlayerPrefs.GetInt(saveMinKey);
		int dSec = System.DateTime.Now.Second - PlayerPrefs.GetInt(saveSecKey);
		
		int secondsPassed = 0;
		secondsPassed = dSec+(dMin+(dHour+(dDay+(dMonth+dYear*12)*30)*24)*60)*60;		
		return secondsPassed;
	}	
	public static int getNoOfMinutesPassed()	{
		return Mathf.FloorToInt(getNoOfSecondsPassed()/60);	}
	public static int getNoOfHoursPassed()	{
		return Mathf.FloorToInt(getNoOfMinutesPassed()/60);	}
	public static int getNoOfDaysPassed()	{
		return Mathf.FloorToInt(getNoOfHoursPassed()/24);	}
	public static int getNoOfMonthsPassed()	{
		return Mathf.FloorToInt(getNoOfDaysPassed()/30);	}
	public static int getNoOfYearsPassed()	{
		return Mathf.FloorToInt(getNoOfMonthsPassed()/12);	}
	
	// Functions to Check //  ###### Need to be tested #####
	public static bool isNoOfYearsPassed(int years) { return (getNoOfYearsPassed() > years); }
	public static bool isNoOfMonthsPassed(int months) { return (getNoOfMonthsPassed() > months); }
	public static bool isNoOfDaysPassed(int days) { return (getNoOfDaysPassed() > days); }
	public static bool isNoOfHoursPassed(int hours) { return (getNoOfHoursPassed() > hours); }
	public static bool isNoOfMinutesPassed(int minutes) { return (getNoOfMinutesPassed() > minutes); }
	public static bool isNoOfSecondsPassed(int seconds) { return (getNoOfSecondsPassed() > seconds); }
}
