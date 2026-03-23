using UnityEngine;
using System.Collections;

public class LocalNotificationHandler : MonoBehaviour {
	
	public static LocalNotificationHandler mInstance = null;	
	public static bool IsLocalNotificationEnabled
	{
		get { return PlayerPrefs.GetInt("LocalNotificationEnabled") == 1; }
		set { PlayerPrefs.SetInt("LocalNotificationEnabled", value ? 1 : 0); }
	}
	
#if UNITY_IPHONE
	public LocalNotification localNotification = null;
#elif UNITY_ANDROID
	public string androidLocalNotification = "";
#endif
	
	public static string debugLog = "";
	
	// Use this for initialization
	void Start () { mInstance = this; DontDestroyOnLoad(this);
//		ScheduleLocalNotificationAfter(0, 0, 0, 0, 5, 0, "Hello Gamer", CalendarUnit.Minute);
//	this.createNotificationForBlackjack(localNotificationMessageforBonusNotCollected, 0, 2, 0);
//		PresentLocalNotificationNow("Hello Gamer");
	}	
	
	// -----------------------		
	private string playerName = "";
	public string localNotificationMessageforBonusReady = "250 Free Chips Awaits!!! Come Play!!";
	public string localNotificationMessageforBonusNotCollected = "You seem to have forgotten about your BONUS!!! Come Back and Collect it!!";
	public string localNotificationMessageforDailyBonus = "Your Daily BONUS is Waiting!!! Come SPIN away!!!";
	public void createNotificationForBlackjack(string message, int hours, int minutes, int seconds)
	{	
		debugLog = "";
		playerName = "Hi, ";
//		if(OnlineSocialHandler.Instance.GetIsLoggedIn())
//			playerName = "Hi " + OnlineSocialHandler.Instance.GetCurrentUserData()._UserName + ", ";
		Debug.Log("player Name : "+playerName);
		
#if UNITY_IPHONE
			localNotification = new LocalNotification();
			localNotification.applicationIconBadgeNumber = NotificationServices.localNotificationCount+1;
			localNotification.alertBody = playerName + message;
			System.DateTime fireDate = System.DateTime.Now;
			fireDate = fireDate.AddHours(hours);
			fireDate = fireDate.AddMinutes(minutes);
			fireDate = fireDate.AddSeconds(seconds);
			Debug.Log("OriginalData : "+System.DateTime.Now+" FireDate : "+fireDate);
			localNotification.fireDate = fireDate;
			localNotification.alertAction = "Show me";
			localNotification.repeatInterval = CalendarUnit.Day;
			localNotification.soundName = LocalNotification.defaultSoundName;
#elif UNITY_ANDROID
			string date = "0_0_0_"+hours+"_"+minutes+"_"+seconds;
			string msg = playerName + message;
			androidLocalNotification = date+"_"+msg;
			Debug.Log("OriginalData : "+System.DateTime.Now+" FireDate : "+date);
			System.DateTime fireDate = System.DateTime.Now;
			fireDate = fireDate.AddHours(hours);
			fireDate = fireDate.AddMinutes(minutes);
			fireDate = fireDate.AddSeconds(seconds);
			debugLog += "\nOriginalData : "+System.DateTime.Now+"\nFireDate : "+fireDate;
#endif
		
		scheduleLocalNotificationForBlackjack();
	}
	public void scheduleLocalNotificationForBlackjack()
	{
#if UNITY_IPHONE
			NotificationServices.CancelAllLocalNotifications();
			if(!IsLocalNotificationEnabled || localNotification == null) return;
			NotificationServices.ScheduleLocalNotification(localNotification);
#elif UNITY_ANDROID		
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.CancelAndroidLocalNotification, "", null);
			if(!IsLocalNotificationEnabled || androidLocalNotification == "") return;
			Debug.Log("Schedule Local Notification");
			debugLog += "Schedule Local Notification";
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.SendAndroidLocalNotification, androidLocalNotification+"_0", null);
#endif
		CommonData.removeAllActionsFrom(gameObject);		
		CCallFunc call = gameObject.AddComponent<CCallFunc>();
		CDelayTime delay = gameObject.AddComponent<CDelayTime>();
		CSequence seq = gameObject.AddComponent<CSequence>();
		call.actionWithCallBack(createDailyBonusNotificationforBlackjack);
		delay.actionWithDuration(5.0f);
		seq.actionWithActions(delay, call);
		seq.runAction();
//		createDailyBonusNotificationforBlackjack();
		debugLog += "\nNotificationScheduled "+IsLocalNotificationEnabled;
		Debug.Log("NotificationScheduled "+IsLocalNotificationEnabled);
	}	
	void createDailyBonusNotificationforBlackjack()
	{
		int storedDay = PlayerPrefs.GetInt("DailyBonusDay");
		int storedMonth = PlayerPrefs.GetInt("DailyBonusMonth");
		int storedYear = PlayerPrefs.GetInt("DailyBonusYear");	
		int hours = Random.Range(8, 20);
		int minutes = Random.Range(0, 61);
		int seconds = Random.Range(0, 61);
		string msg = (playerName == "" ?  "Hi There, " : playerName) +  localNotificationMessageforDailyBonus;
#if UNITY_IPHONE
			LocalNotification localNotification1 = new LocalNotification();
			localNotification1.applicationIconBadgeNumber = NotificationServices.localNotificationCount+1;
			localNotification1.alertBody = msg;
			System.DateTime fireDate = new System.DateTime(storedYear, storedMonth, storedDay);
			fireDate = fireDate.AddDays(1);
			fireDate = fireDate.AddHours(hours);
			fireDate = fireDate.AddMinutes(minutes);
			fireDate = fireDate.AddSeconds(seconds);
			Debug.Log("OriginalData : "+(new System.DateTime(storedYear, storedMonth, storedDay))+" FireDate : "+fireDate);
			localNotification1.fireDate = fireDate;
			localNotification1.alertAction = "Show me";
			localNotification1.repeatInterval = CalendarUnit.Day;
			localNotification1.soundName = LocalNotification.defaultSoundName;
			NotificationServices.ScheduleLocalNotification(localNotification1);
#elif UNITY_ANDROID
		System.DateTime currentDateTime = System.DateTime.Now;
		System.DateTime fireDate = new System.DateTime(storedYear, storedMonth, storedDay, hours, minutes, seconds);
		fireDate.AddDays(1);
		System.TimeSpan diffInDateTime = fireDate.Subtract(currentDateTime);		
		string date = "0_0_"+diffInDateTime.Days+"_"+diffInDateTime.Hours+"_"+diffInDateTime.Minutes+"_"+diffInDateTime.Seconds;
		Debug.Log("OriginalData : "+System.DateTime.Now+" FireDate : "+date);
		debugLog += "\nOriginalData : "+System.DateTime.Now+"\nFireDate : "+fireDate;
		if(diffInDateTime.Days < 0 || diffInDateTime.Hours < 0 || diffInDateTime.Minutes < 0 || diffInDateTime.Seconds < 0) return;
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.SendAndroidLocalNotification, date+"_"+msg+"_1", null);
#endif
	}
	// -----------------------
	
#if UNITY_IPHONE
	public void PresentLocalNotificationNow(string message)
	{
		LocalNotification localNotification = new LocalNotification();
		localNotification.alertBody = message;
		NotificationServices.PresentLocalNotificationNow(localNotification);
	}
	
	public void ScheduleLocalNotificationWithinaDay(int hours, int min, int sec, string message)
	{		
		ScheduleLocalNotificationAfter(0, 0, 0, hours, min, sec, message, CalendarUnit.Day);
	}
	
	public void ScheduleLocalNotificationWithinaYear(int years, int months, int days, int hours, int min, int sec, string message)
	{		
		ScheduleLocalNotificationAfter(0, months, days, hours, min, sec, message, CalendarUnit.Day);
	}
	
	public void ScheduleLocalNotificationAfter(int years, int months, int days, int hours, int min, int sec, string message, CalendarUnit calanderUnit)
	{		
		LocalNotification localNotification = new LocalNotification();
		localNotification.alertBody = message;
		System.DateTime fireDate = System.DateTime.Now;
		localNotification.applicationIconBadgeNumber = NotificationServices.localNotificationCount+1;
		fireDate.AddYears(years);
		fireDate.AddMonths(months);
		fireDate.AddDays(days);
		fireDate.AddHours(hours);
		fireDate.AddMinutes(min);
		fireDate.AddSeconds(sec);
		Debug.Log("FireDate : "+fireDate);
		localNotification.fireDate = fireDate;
		localNotification.alertAction = "Show me";
		localNotification.repeatInterval = calanderUnit;
		localNotification.soundName = LocalNotification.defaultSoundName;
		NotificationServices.ScheduleLocalNotification(localNotification);
	}
#endif
	
//	void OnGUI()
//	{		
//		GUI.Label(new Rect(10, 10, Screen.width-10, Screen.height-10), debugLog);
//	}
}
