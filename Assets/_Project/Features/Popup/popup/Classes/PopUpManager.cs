using UnityEngine;
using System.Collections;

public class PopUpManager : MonoBehaviour {
	
	public enum PopUpType {MoreChips, InsufficientFunds, Stats, Settings, FriendsLeaderboard};
	
	public delegate void PopUpRemoveCallBack();
	public PopUpRemoveCallBack mPopUpRemoveCallBack = null;
	public static PopUpManager mInstance = null;
	public GameObject moreChipsHandler = null;
	public GameObject statsHandler = null;
	public GameObject insufficientFundsHandler = null;
	public GameObject settingsHandler = null;
	public GameObject alphalayer = null;	
	public bool isPopUpActive = false;
	
	void Start () { StartCoroutine(Instantiate()); mInstance = this; alphalayer.GetComponent<AlphaLayerHandler>().fadeOutAlphaLayer(0.01f); }	

	
	IEnumerator Instantiate()
	{
		while(true)
		{
			yield return new WaitForEndOfFrame();
			init();
			break;
		}
	}
	
	public void init()
	{
		try { statsHandler.GetComponent<StatsHandler>().init(); } catch {}
		try { insufficientFundsHandler.GetComponent<InsufficientChipsHandler>().init(); } catch {}
		try { moreChipsHandler.GetComponent<MoreChipsHandler>().init(); } catch {}
		try { settingsHandler.GetComponent<SettingsHandler>().init(); } catch {}
	}
	
	public void showPopUp(PopUpType createPopUpType, PopUpRemoveCallBack removedCallBack)
	{
		mPopUpRemoveCallBack = removedCallBack;
		switch(createPopUpType)
		{
		case PopUpType.MoreChips:
			moreChipsHandler.GetComponent<MoreChipsHandler>().showPopUp();
			break;
		case PopUpType.InsufficientFunds:
			insufficientFundsHandler.GetComponent<InsufficientChipsHandler>().showPopUp();
			break;
		case PopUpType.Stats:
			statsHandler.GetComponent<StatsHandler>().showPopUp();
			break;
		case PopUpType.Settings:
			settingsHandler.GetComponent<SettingsHandler>().showPopUp();
			break;
		}
		
		alphalayer.GetComponent<AlphaLayerHandler>().fadeInAlphaLayer(0.5f, 0.8f);
		isPopUpActive = true;
	}
	
	public void updateChipsBalance(int chipsBalance)
	{
		moreChipsHandler.GetComponent<MoreChipsHandler>().mChipsBalanceLabel.setString(chipsBalance.ToString());
		insufficientFundsHandler.GetComponent<InsufficientChipsHandler>().mChipsBalanceLabel.setString(chipsBalance.ToString());
	}
}
