using UnityEngine;
using System.Collections;

public class SpinWheelHandler : MonoBehaviour {

	public delegate void SpinWheelSelectedCallBack(int selected);
	private SpinWheelSelectedCallBack mCallBackForSelectedSector = null;
	
	public GameObject mSpinWheelGameObject = null;
	public float AngleOffsetForFirstSector = 0.0f; 	// Set the initial offset angle to rectify the error of sector detection
													// The arrow initial position is between the first and the last sector (starting of the first sector)
	public int noOfSectors = 2;						// No of Sectors needs to be detected. Min 2 is required
	public bool isResetWheelStopPositionToCenterOfSector = false; // Setting this true will make the wheel to always stop at the center of the selected sector
	public float spinSpeedMin = 25.0f;				// Min Speed in which the wheel starts to spin
	public float spinSpeedMax = 35.0f;				// Max Speed in which the wheel starts to spin
													// A Random speed is selected as the starting speed for the spin (needed for the randomness in selecting sectors)
	public float spinSpeedVariationMin = 5.0f;		// Min value to reduce the speed of the spin
	public float spinSpeedVariationMax = 15.0f;		// Max value to reduce the speed of the spin
													// A Random value is selected as the speed reducer for the spin (needed for the randomness in selecting sectors)
	
	public bool startSpin = false;
	
	float spinSpeed = 0.0f;
	bool isSpinStarted = false;
	bool isResetStarted = false;
	int selectedSector = 0;
	Vector3 resetToAngle = Vector3.zero;
	
	private float spinTimer = 0.0f;
	
	// Use this for initialization
	void Start () { if(noOfSectors < 2) noOfSectors = 2; }	
	// Update is called once per frame
	void Update () { if(startSpin) makeWheelToSpin(null); spinTheWheel(); resetTheWheelToSelectedSector(); }
	
	public void makeWheelToSpin(SpinWheelSelectedCallBack callBackAfterSelection)
	{
		if(isSpinStarted || isResetStarted) return;
		startSpin = false;
		isSpinStarted = true;
		isResetStarted = false;
		mSpinWheelGameObject.transform.localRotation = Quaternion.identity;
		spinSpeed = Random.Range(spinSpeedMin,spinSpeedMax);
		mCallBackForSelectedSector = callBackAfterSelection;
	}
	
	void spinTheWheel()
	{
		if(!isSpinStarted) return;
		spinTimer += Time.deltaTime;
		
		mSpinWheelGameObject.transform.Rotate(Vector3.forward * spinSpeed); 
		spinSpeed -= Time.deltaTime* Random.Range(spinSpeedVariationMin,spinSpeedVariationMax);
		
		if(spinSpeed < 0.0f)
		{
			Debug.Log("spinTime : "+spinTimer);
			spinSpeed = 0.0f;
			isSpinStarted = false;
			
			calculateTheSector();
		}
	}
	
	void calculateTheSector()
	{
		float SpinStoppedAtAngle = mSpinWheelGameObject.transform.localRotation.eulerAngles.z;
		SpinStoppedAtAngle %= 360.0f;
		SpinStoppedAtAngle -= AngleOffsetForFirstSector;
		SpinStoppedAtAngle = 360.0f + SpinStoppedAtAngle;
		SpinStoppedAtAngle %= 360.0f;
		float angleGapForOneSector = 360.0f/noOfSectors;
		int wheelStopedinSector = Mathf.CeilToInt(SpinStoppedAtAngle / angleGapForOneSector);
		if(wheelStopedinSector > noOfSectors) wheelStopedinSector = 1;
		
//		Debug.Log("Stoped At Rotation   : "+SpinStoppedAtAngle);
//		Debug.Log("Angle Gap For Sector : "+angleGapForOneSector);
//		Debug.Log("Wheel Stoped Sector  : "+wheelStopedinSector);
		
		selectedSector = wheelStopedinSector;
		isResetStarted = isResetWheelStopPositionToCenterOfSector;
	}
	
	void resetTheWheelToSelectedSector()
	{
		if(!isResetStarted) return;
		
		float angleGapForOneSector = 360.0f/noOfSectors;
		float angleToReset = angleGapForOneSector * ((float)selectedSector-0.5f) + angleGapForOneSector;
		if(angleToReset > 360.0f) angleToReset %= 360.0f;
		resetToAngle = new Vector3(0, 0, angleToReset);
		
		CRotateTo rotateToAction = GetComponent<CRotateTo>();
		CCallFunc callBackAction = GetComponent<CCallFunc>();
		CSequence seqenceAction  = GetComponent<CSequence>();
		
		rotateToAction.actionWith(mSpinWheelGameObject, resetToAngle, 0.5f);
		callBackAction.actionWithCallBack(resetCompleted);
		seqenceAction.actionWithActions(rotateToAction, callBackAction);
		seqenceAction.runAction();
		
		isResetStarted = false;
	}
	
	void resetCompleted()
	{
		if(mCallBackForSelectedSector != null)
			mCallBackForSelectedSector(selectedSector);
		Debug.Log("SpinCompleteTime : "+spinTimer);
	}
}
