using UnityEngine;
using System.Collections;

public class CRotateTo : CAction
{
	// Use this for initialization
	public GameObject _TargetGameObject;
	
	public Vector3 _StartRot = new Vector3(0,0,0);
	public Vector3 _DestinationRot = new Vector3(0,0,0);
	
	private Vector3 mvNormalized = new Vector3(0,0,0);
	private Vector3 mvRot3d = new Vector3(0,0,0);
	
	private Transform mGameObjTransform = null;
	float mfDistanceMagnitude = 0.0f;
	float mfMagnitudeDisplacement = 0.0f;
	float mfRate = 0.0f;
	ActionState meState = ActionState.ActionNotRunning;
	
	// Use this for initialization
	void Start ()
	{
		_SelfUpdate = true;
	}
	
	// STATIC CREATE WITH AUTORELEASE
	public static CRotateTo create(GameObject pTargetObject, Vector3 pEndRot,float pDuration)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CRotateTo";
		
		CRotateTo action = actionObject.AddComponent<CRotateTo>();
		action.actionWith(pTargetObject, pEndRot,pDuration);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
	public void actionWith(GameObject pTargetObject, Vector3 pEndRot,float pDuration)
	{
		_DestinationRot = pEndRot;
		_duration = pDuration;
		_TargetGameObject = pTargetObject;
	}
		
	override public void runAction()
	{
		if(_TargetGameObject == null)
		{
			Debug.Log("Missing target object in action script");
			return;
		}
		mGameObjTransform = _TargetGameObject.transform;
		_StartRot = mGameObjTransform.eulerAngles;
		meState = ActionState.ActionRun;
		mfRate = 1.0f/_duration;
		mvNormalized = _DestinationRot - _StartRot;
		mfDistanceMagnitude = Mathf.Sqrt(Vector3.SqrMagnitude(mvNormalized));
		mvNormalized = Vector3.Normalize(mvNormalized);
		mfMagnitudeDisplacement = 0.0f;
	}
	
	override public void pauseAction()
	{
		if(meState == ActionState.ActionRun)
			meState = ActionState.ActionPause;
	}
	
	override public void resumeAction()
	{
		if(meState == ActionState.ActionPause)
			meState = ActionState.ActionRun;
	}
	
	override public void setCompletion(float completion)
	{
		mfMagnitudeDisplacement = completion;
		if(mfDistanceMagnitude > 0)
		{
			mvRot3d = _StartRot + mvNormalized * (mfDistanceMagnitude*mfMagnitudeDisplacement);
   			mGameObjTransform.eulerAngles = mvRot3d;
		}
	}
	
	void  performAction()
	{
		if(mfDistanceMagnitude > 0)
		{
			mvRot3d = _StartRot + mvNormalized * (mfDistanceMagnitude*mfMagnitudeDisplacement);
   			mGameObjTransform.eulerAngles = mvRot3d;
		}
	
		if(mfMagnitudeDisplacement >= 1.0f)
		{
			mvRot3d = _DestinationRot;
			mGameObjTransform.eulerAngles = mvRot3d;
			onActionComplete();
		}
	}
	
	void onActionComplete()
	{
		meState = ActionState.ActionComplete;
		if(internalAnimationCallBack != null)
			internalAnimationCallBack(this);
		if(autoReleaseCallBack != null)
			autoReleaseCallBack(this);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(_SelfUpdate == true)
		{
			if(meState == ActionState.ActionRun)
			{
				mfMagnitudeDisplacement += Time.deltaTime * mfRate;
				performAction();
			}
		}
	}
}