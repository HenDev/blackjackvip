using UnityEngine;
using System.Collections;

public class BJ_AchievementHandler : MonoBehaviour {
	
	// Singleton Object
	public static BJ_AchievementHandler mInstance = null;
	
	// Achievements Enum
	enum AchievementsList {
		BigWin=1,
		MajorWin,
		WittySurrender,
		InsuranceClaimer,
		BlackjackMegaWin,
		MegaBigWin,
		SilverMegaWin,
		LuckyHit,
		BlindSplit,
		DoubleDown,
		LucksYours,
		WinStreak,
		HighRoller};
	
	// Properties
	private int Ach_WinStreak
	{
		get { return PlayerPrefs.GetInt("Ach_WinStreak"); }
		set { PlayerPrefs.SetInt("Ach_WinStreak", value); }
	}
	
	private int Ach_WinChip
	{
		get { return PlayerPrefs.GetInt("Ach_WinChip"); }
		set { PlayerPrefs.SetInt("Ach_WinChip", value); }
	}
	
	private int Ach_WinScoreChip
	{
		get { return PlayerPrefs.GetInt("Ach_WinScoreChip"); }
		set { PlayerPrefs.SetInt("Ach_WinScoreChip", value); }
	}
	
	public bool Ach_HighRoller
	{
		get { return PlayerPrefs.GetInt("Ach_HighRoller") == 1; }
		set { PlayerPrefs.SetInt("Ach_HighRoller", value?1:0); }
	}
	
	public bool Ach_LucksYours
	{
		get { return PlayerPrefs.GetInt("Ach_LucksYours") == 1; }
		set { PlayerPrefs.SetInt("Ach_LucksYours", value?1:0); }
	}
	
	public bool Ach_WittySurrender
	{
		get { return PlayerPrefs.GetInt("Ach_WittySurrender") == 1; }
		set { PlayerPrefs.SetInt("Ach_WittySurrender", value?1:0); }
	}
	
	public bool Ach_InsuranceClaimer
	{
		get { return PlayerPrefs.GetInt("Ach_InsuranceClaimer") == 1; }
		set { PlayerPrefs.SetInt("Ach_InsuranceClaimer", value?1:0); }
	}
	
	public bool Ach_LuckyHit
	{
		get { return PlayerPrefs.GetInt("Ach_LuckyHit") == 1; }
		set { PlayerPrefs.SetInt("Ach_LuckyHit", value?1:0); }
	}
	
	public bool Ach_DoubleDown
	{
		get { return PlayerPrefs.GetInt("Ach_DoubleDown") == 1; }
		set { PlayerPrefs.SetInt("Ach_DoubleDown", value?1:0); }
	}
	
	public bool Ach_BlindSplit
	{
		get { return PlayerPrefs.GetInt("Ach_BlindSplit") == 1; }
		set { PlayerPrefs.SetInt("Ach_BlindSplit", value?1:0); }
	}
	
	// Private Variables
	
	// Use this for initialization
	void Start () { mInstance = this; DontDestroyOnLoad(this); }	
	// Update is called once per frame
	void Update () { }
	
	
	public void checkForPlayerWinBasedAchievements(bool playerWin, int cardValue, int WinScore)
	{
		// Achievement WinStreak
		if(Ach_WinStreak != -1) 
		{
			Ach_WinStreak = playerWin ? ++Ach_WinStreak : 0;
			
			// Achievement Achieved
			if(Ach_WinStreak == 10) 
			{
				Ach_WinStreak = -1;
				ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.WinStreak)+"", null);
			}
		}
		
		// Achievement LucksYours
		if(!Ach_LucksYours) 
		{
			Ach_LucksYours = playerWin && cardValue < 10;
			
			// Achievement Achieved
			if(Ach_LucksYours) 
			{
				ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.LucksYours)+"", null);
			}
		}
		
		checkForGameEndAchievements();
		checkForScoreBasedAchievements(WinScore);
	}
	
	void checkForScoreBasedAchievements(int score)
	{
		if(Ach_WinScoreChip == 2) return; // Achievement Done
		
		int[] arrWinChipValues = {20000,50000};
		for(int i=1; i>=Ach_WinScoreChip; i--)
		{
			if(score >= arrWinChipValues[i])
			{
				while(Ach_WinScoreChip != i+1)
				{
					ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.BigWin+Ach_WinScoreChip)+"", null);		
					Ach_WinScoreChip++;
				}
				break;
			}
		}
	}
	
	void checkForGameEndAchievements()
	{
		if(Ach_WinChip == 3) return; // Achievement Done
		
		int[] arrWinChipValues = {100000,250000,500000};
		for(int i=2; i>=Ach_WinChip; i--)
		{
			if(StatsHandler.TotalWin >= arrWinChipValues[i])
			{
				while(Ach_WinChip != i+1)
				{
					ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.BlackjackMegaWin+Ach_WinChip)+"", null);		
					Ach_WinChip++;
				}
				break;
			}
		}
	}
	
	public void checkForHighRoller(int tableMin)
	{
		if(Ach_HighRoller) return; // Achievement Done
		Debug.Log(((int)AchievementsList.HighRoller)+"");
		// Achievement Achieved
		if(tableMin >= 500000)
		{
			Ach_HighRoller = true;
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.HighRoller)+"", null);
			
		}
	}
	
	public void checkForWittySurrender(bool isSurrender, bool isDealerBlackJack)
	{
		if(Ach_WittySurrender) return; // Achievement Done
		
		// Achievement Achieved
		if(isSurrender && isDealerBlackJack)
		{
			Ach_WittySurrender = true;
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.WittySurrender)+"", null);			
		}
	}
	
	public void checkForLuckyHitAchievement(int cardValue, int lastCardValue)
	{
		if(Ach_LuckyHit) return; // Achievement Done
		
		// Achievement Achieved
		if(cardValue == 21 && (lastCardValue == 2 || lastCardValue == 10))
		{
			Ach_LuckyHit = true;
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.LuckyHit)+"", null);			
		}
	}
	
	public void checkForDoubleDownAchievement(bool isDoubled, int cardValue)
	{
		if(Ach_DoubleDown) return; // Achievement Done
		
		// Achievement Achieved
		if(cardValue == 21 && isDoubled)
		{
			Ach_DoubleDown = true;
			ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.DoubleDown)+"", null);			
		}
	}
	
	public void InsuranceClaimerDone()
	{
		if(Ach_InsuranceClaimer) return; // Achievement Done
		
		// Achievement Achieved
		Ach_WittySurrender = true;
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.InsuranceClaimer)+"", null);			
	}
	
	public void BlindSplitDone()
	{
		if(Ach_BlindSplit) return; // Achievement Done
		
		// Achievement Achieved
		Ach_BlindSplit = true;
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.BlindSplit)+"", null);			
	}
	
	public void DoubleDownDone()
	{
		if(Ach_DoubleDown) return; // Achievement Done
		
		// Achievement Achieved
		Ach_DoubleDown = true;
		ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Achievement, ((int)AchievementsList.DoubleDown)+"", null);			
	}
	
}
