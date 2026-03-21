using UnityEngine;
using System.Collections;

public class FlipCard : MonoBehaviour {	
	
	// Use this for initialization	
	public enum FlipType {Horizontal,Vertical,Left,Right,Top,Bottom};	
	
	public Vector3 _StartRot = new Vector3(0,0,0);
	public Vector3 _DestinationRot = new Vector3(0,180,0);
	
	private Vector3 mvNormalized = new Vector3(0,0,0);
	private Vector3 mvRot3d = new Vector3(0,0,0);
	
	private Vector3 mRotatePoint = Vector3.zero;
	private Vector3 mRotateAxis = Vector3.zero;
	
	private FlipType mCurrentFlipType = FlipType.Vertical;
	
	float mfDistanceMagnitude = 0.0f;
	float mfMagnitudeDisplacement = 0.0f;
	float mfRate = 0.0f;
	bool mStartUpdate = false;
	
	// Use this for initialization
	void Start ()
	{ 	}	
	
	public static void createFlipAction(GameObject targetObject, FlipType flipType, float duration)
	{		
		if(targetObject.GetComponent<FlipCard>() != null)
			DestroyObject(targetObject.GetComponent<FlipCard>());
		FlipCard flipCard = targetObject.AddComponent<FlipCard>();
		flipCard.runAction(flipType, duration);
	}
		
	public void runAction(FlipType flipType, float duration)
	{		
		mCurrentFlipType = flipType;
		_StartRot = transform.eulerAngles;
		// Adjustment to not allow Horizontal sets
		if(!(mCurrentFlipType == FlipType.Vertical || mCurrentFlipType == FlipType.Left || mCurrentFlipType == FlipType.Right))
			mCurrentFlipType = FlipType.Vertical;
		switch(mCurrentFlipType)
		{
		case FlipType.Vertical:
			mRotatePoint = transform.position;
			break;
		case FlipType.Left:
			mRotatePoint = transform.position - new Vector3(transform.localScale.x/2.0f,0,0);
			break;
		case FlipType.Right:
			mRotatePoint = transform.position + new Vector3(transform.localScale.x/2.0f,0,0);
			break;
		case FlipType.Horizontal:
			mRotatePoint = transform.position;
			break;
		case FlipType.Top:
			mRotatePoint = transform.position + new Vector3(0,transform.localScale.x/2.0f,0);
			break;
		case FlipType.Bottom:
			mRotatePoint = transform.position - new Vector3(0,transform.localScale.x/2.0f,0);
			break;			
		}
		if(mCurrentFlipType == FlipType.Vertical || mCurrentFlipType == FlipType.Left || mCurrentFlipType == FlipType.Right)
		{
			mRotateAxis = Vector3.up;
			if(_StartRot.y > 360) _StartRot -= new Vector3(0,360,0);
			_DestinationRot = new Vector3(_StartRot.x,(_StartRot.y > 90 && _StartRot.y < 270)?360:180,_StartRot.z);
		}
		else
		{
			mRotateAxis = Vector3.right;
			if(_StartRot.x > 360) _StartRot -= new Vector3(360,0,0);
			_DestinationRot = new Vector3((_StartRot.x > 90 && _StartRot.x < 270)?360:180, _StartRot.y,_StartRot.z);
		}
		mfRate = 1.0f/duration;
		mvNormalized = _DestinationRot - _StartRot;
		mfDistanceMagnitude = Mathf.Sqrt(Vector3.SqrMagnitude(mvNormalized));
		mvNormalized = Vector3.Normalize(mvNormalized);
		mfMagnitudeDisplacement = 0.0f;
		mStartUpdate = true;
	}
	
	void  performAction()
	{
		if(mfDistanceMagnitude > 0)
		{
			mvRot3d = _StartRot + mvNormalized * (mfDistanceMagnitude*mfMagnitudeDisplacement);
			float angle = 0.0f;
			if(mCurrentFlipType == FlipType.Vertical || mCurrentFlipType == FlipType.Left || mCurrentFlipType == FlipType.Right)
				angle = mvRot3d.y - transform.eulerAngles.y;
			else
				angle = mvRot3d.x - transform.eulerAngles.x;
			transform.RotateAround(mRotatePoint, mRotateAxis, angle);
		}
	
		if(mfMagnitudeDisplacement >= 1.0f)
		{
			mvRot3d = _DestinationRot;
			transform.eulerAngles = mvRot3d;
			DestroyObject(this);		
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(mStartUpdate)
		{
			mfMagnitudeDisplacement += Time.deltaTime * mfRate;
			performAction();
		}
	}
}
