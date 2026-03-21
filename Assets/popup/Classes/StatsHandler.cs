using UnityEngine;
using System.Collections;

public class StatsHandler : PopUpBase {
	
	float labelsXPosition = 6.2f;
	
	// Properties
	public static int HandsPlayed
	{
		get { return PlayerPrefs.GetInt("HandsPlayed"); }
		set { PlayerPrefs.SetInt("HandsPlayed", value); }
	}
	
	public static int BlackJacks
	{
		get { return PlayerPrefs.GetInt("PlayerBlackJacks"); }
		set { BlackjackFlurryHandler.sendHandBlackjackFlurryData(); PlayerPrefs.SetInt("PlayerBlackJacks", value); }
	}
	
	public static int HandsWon
	{
		get { return PlayerPrefs.GetInt("PlayerHandsWon"); }
		set { BlackjackFlurryHandler.sendHandWonFlurryData(); PlayerPrefs.SetInt("PlayerHandsWon", value); }
	}
	
	public static int HandsPushed
	{
		get { return PlayerPrefs.GetInt("PlayerHandsPushed"); }
		set { BlackjackFlurryHandler.sendHandPushedFlurryData(); PlayerPrefs.SetInt("PlayerHandsPushed", value); }
	}
	
	public static int HandsLost
	{
		get { return PlayerPrefs.GetInt("PlayerHandsLost"); }
		set { BlackjackFlurryHandler.sendHandLostFlurryData(); PlayerPrefs.SetInt("PlayerHandsLost", value); }
	}
	
	public static int TotalBets
	{
		get { return PlayerPrefs.GetInt("PlayerTotalBets"); }
		set { PlayerPrefs.SetInt("PlayerTotalBets", value); }
	}
	
	public static int TotalWin
	{
		get { return PlayerPrefs.GetInt("PlayerTotalWin"); }
		set { PlayerPrefs.SetInt("PlayerTotalWin", value); }
	}
	
	public static int MaxBet
	{
		get { return PlayerPrefs.GetInt("PlayerMaxBet"); }
		set { PlayerPrefs.SetInt("PlayerMaxBet", Mathf.Max(value,MaxBet)); }
	}
	
	public static int MaxWin
	{
		get { return PlayerPrefs.GetInt("PlayerMaxWin"); }
		set { PlayerPrefs.SetInt("PlayerMaxWin", Mathf.Max(value,MaxWin)); }
	}
	
	// Use this for initialization
	void Start () { }	
	// Update is called once per frame
	void Update () { checkAndroidKeys(); }
	
	public override void init ()
	{
		IsInitStarted = true;
		base.init ();
		
		getLabelAt(HandsPlayed, new Vector3(labelsXPosition,7.5f,0));
		
		getLabelAt(BlackJacks, new Vector3(labelsXPosition,4.4f,0));
		getLabelAt(HandsWon, new Vector3(labelsXPosition,2.6f,0));
		getLabelAt(HandsPushed, new Vector3(labelsXPosition,0.8f,0));
		getLabelAt(HandsLost, new Vector3(labelsXPosition,-0.9f,0));
		
		getLabelAt(TotalBets, new Vector3(labelsXPosition,-4.1f,0));
		getLabelAt(TotalWin, new Vector3(labelsXPosition,-5.9f,0));
		getLabelAt(MaxBet, new Vector3(labelsXPosition,-7.6f,0));
		getLabelAt(MaxWin, new Vector3(labelsXPosition,-9.4f,0));	
		IsInitDone = true;		
	}
	
	CLabelAtlas getLabelAt(int labelValue, Vector3 position)
	{
		CLabelAtlas label = CLabelAtlas.create(labelValue.ToString(), mSpriteManager, mSpriteAtlasDataHandler, "NumberFont.png", 2, CLabelAtlas.LabelTextAlignment.Right);
		label.setScale(0.7f);
		label.addParent(mPopUpBgHolder);
		label.setPosition(position);
		label.gameObject.layer = gameObject.layer;
		
		return label;
	}
}
