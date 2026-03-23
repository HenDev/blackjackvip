using UnityEngine;
using System.Collections;

public enum eADS_DISPLAY_INTERVAL_TYPE
{
	TimeDelay = 0,
	Counter,
}

public class AdsHandler : MonoBehaviour 
{
	float 			mfPrevAdsShownTime = 0.0f;
	int				miCurrentDisplayedCounter = 0;
	
	public float 	_fAdsDisplayInterval = 20.0f;
	public int		_iCounterInterval = 1;
	
	eADS_DISPLAY_INTERVAL_TYPE	_eAdsDisplayIntervalType = eADS_DISPLAY_INTERVAL_TYPE.Counter;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	public bool IsAdsDisplayAllowed(eEXTERNAL_REQ_TYPE eExtRequestType)
	{
		if(eExtRequestType == eEXTERNAL_REQ_TYPE.Show_Banner_Top_Ads || eExtRequestType == eEXTERNAL_REQ_TYPE.Show_Banner_Bottom_Ads
			|| eExtRequestType == eEXTERNAL_REQ_TYPE.Hide_Banner_Ads)
			return true;
		  
		switch(_eAdsDisplayIntervalType)
		{
			case eADS_DISPLAY_INTERVAL_TYPE.Counter:
			{
				miCurrentDisplayedCounter++;
				if(miCurrentDisplayedCounter >= _iCounterInterval)
				{
					miCurrentDisplayedCounter = 0;
					return true;
				}
			}
			break;
		case eADS_DISPLAY_INTERVAL_TYPE.TimeDelay:
			{
				if(Time.time - mfPrevAdsShownTime >= _fAdsDisplayInterval)
				{
					mfPrevAdsShownTime = Time.time;
					return true;
				}
			}
			break;
		}
		
		return false;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
