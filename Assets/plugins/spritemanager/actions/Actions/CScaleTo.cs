using UnityEngine;
using System.Collections;

public class CScaleTo : CAction {

	// Use this for initialization
	public GameObject _TargetGameObject;
	
	public Vector3 _StartScale = new Vector3(0,0,0);
	public Vector3 _DestinationScale = new Vector3(0,0,0);
	
	private Vector3 mvNormalized = new Vector3(0,0,0);
	private Vector3 mvScale = new Vector3(0,0,0);
	private Transform mGameObjTransform = null;
	float mfDistanceMagnitude = 0.0f;
	float mfMagnitudeDisplacement = 0.0f;
	float mfRate = 0.0f;
	ActionState meState = ActionState.ActionNotRunning;
	
	// Use this for initialization
	void Start ()
	{
		_SelfUpdate = true;
		//internalAnimationCallBack = null;
	}
	
	// STATIC CREATE WITH AUTORELEASE
	public static CScaleTo create(GameObject pTargetObject, Vector2 pEndScale,float pDuration)
	{		
		return create(pTargetObject, new Vector3(pEndScale.x,pEndScale.y, pTargetObject.transform.localScale.z), pDuration);
	}
	
	public static CScaleTo create(GameObject pTargetObject, Vector3 pEndScale,float pDuration)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CScaleTo";
		
		CScaleTo action = actionObject.AddComponent<CScaleTo>();
		action.actionWith(pTargetObject, pEndScale,pDuration);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
	public void actionWith(GameObject pTargetObject, Vector3 pEndScale,float pDuration)
	{
		_DestinationScale = pEndScale;
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
		_StartScale = mGameObjTransform.localScale;
		meState = ActionState.ActionRun;
		mfRate = 1.0f/_duration;
		mGameObjTransform.localScale = _StartScale;
		mvNormalized = _DestinationScale - _StartScale;
		mfDistanceMagnitude = Mathf.Sqrt(Vector3.SqrMagnitude(mvNormalized));
		mvNormalized = new Vector3(mvNormalized.x/mfDistanceMagnitude,mvNormalized.y/mfDistanceMagnitude, mvNormalized.z/mfDistanceMagnitude);
		mfMagnitudeDisplacement = 0.0f;
		mvScale = mGameObjTransform.localScale;
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
	
	override public void  setCompletion(float completion)
	{
		if(mfDistanceMagnitude != 0)
		{
			Vector3 tScale3d;
			tScale3d = _StartScale + mvNormalized * (mfDistanceMagnitude*completion);
			mvScale.x = tScale3d.x;
			mvScale.y = tScale3d.y;
			mvScale.z = tScale3d.z;
   			mGameObjTransform.localScale = mvScale;
		}
	}
	
	void  performAction()
	{
		if(mfMagnitudeDisplacement >= 0 && mfMagnitudeDisplacement < 1.0f)
		{
			setCompletion(mfMagnitudeDisplacement);
		}
	
		else if(mfMagnitudeDisplacement >= 1.0f)
		{
			mfMagnitudeDisplacement = 1.0f;
			setCompletion(1.0f);
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
