using UnityEngine;
using System.Collections;

public class BlackjackFlurryHandler {
	
	public static bool isFlurryActive = true;		
	private static void sendFlurryData(string key)
	{
        Debug.Log("TestLog: " + key);

        //TODO COMMENTED	if(isFlurryActive)
        //TODO COMMENTED	ExternalInterfaceHandler.Instance.SendRequest(eEXTERNAL_REQ_TYPE.Send_Flurry,key,null);
    }

    public static void sendTableJoinedFlurryData(int tableIndex, string tableName)
	{
		sendFlurryData("Blackjack_TableEntered_"+tableIndex+"_"+tableName);
	}
	
	public static void sendClaimBonusFlurryData()
	{
		sendFlurryData("Blackjack_ClaimBonus");		
	}
	
	public static void sendDailyBonusCreatedFlurryData()
	{
		sendFlurryData("Blackjack_DailyBonus_Created");
	}
	
	public static void sendDailyBonusClaimedFlurryData()
	{
		sendFlurryData("Blackjack_DailyBonus_Claimed");
	}
	
	public static void sendChipPurchaseFlurryData(int index, int chipCount, int cost)
	{
		sendFlurryData("Blackjack_Purchase_"+index+"_"+chipCount+"_"+cost);
	}
	
	public static void sendHandPlayedFlurryData()
	{
		sendFlurryData("Blackjack_HandPlayed");
	}
	 
	public static void sendHandWonFlurryData()
	{
		sendFlurryData("Blackjack_HandWon");
	}
	
	public static void sendHandLostFlurryData()
	{
		sendFlurryData("Blackjack_HandLost");
	}
	
	public static void sendHandBlackjackFlurryData()
	{
		sendFlurryData("Blackjack_HandBlackjack");
	}
	
	public static void sendHandPushedFlurryData()
	{
		sendFlurryData("Blackjack_HandPushed");
	}
}
