using UnityEngine;
using System.Collections;

public class CMoveTo : CAction
{
	// Use this for initialization
	public GameObject _TargetGameObject;
	
	public Vector2 _StartPos = new Vector2(0,0);
	public Vector2 _DestinationPos = new Vector2(0,0);
	
	private Vector2 mvNormalized = new Vector2(0,0);
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
	public static CMoveTo create(GameObject pTargetObject, Vector2 pEndPos,float pDuration)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CMoveTo";
		
		CMoveTo action = actionObject.AddComponent<CMoveTo>();
		action.actionWith(pTargetObject, pEndPos,pDuration);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
	public void actionWith(GameObject pTargetObject, Vector2 pEndPos,float pDuration)
	{
		_DestinationPos = pEndPos;
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
		_StartPos = mGameObjTransform.position;
		meState = ActionState.ActionRun;
		mfRate = 1.0f/_duration;
		mGameObjTransform.position = new Vector3(_StartPos.x,_StartPos.y,mGameObjTransform.position.z);
		mvNormalized = _DestinationPos - _StartPos;
		mfDistanceMagnitude = Mathf.Sqrt(Vector2.SqrMagnitude(mvNormalized));
		mvNormalized = new Vector2(mvNormalized.x/mfDistanceMagnitude,mvNormalized.y/mfDistanceMagnitude);
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
		Vector2 tPos2d;
		Vector3 tPos3d;
		if(mfDistanceMagnitude > 0)
		{
			tPos2d = _StartPos + mvNormalized * (mfDistanceMagnitude*mfMagnitudeDisplacement);
			tPos3d = new Vector3(tPos2d.x,tPos2d.y,mGameObjTransform.position.z);
   			mGameObjTransform.position = tPos3d;
		}
	}
	
	void  performAction()
	{
		Vector2 tPos2d;
		Vector3 tPos3d;
		if(mfDistanceMagnitude > 0)
		{
			tPos2d = _StartPos + mvNormalized * (mfDistanceMagnitude*mfMagnitudeDisplacement);
			tPos3d = new Vector3(tPos2d.x,tPos2d.y,mGameObjTransform.position.z);
   			mGameObjTransform.position = tPos3d;
		}
	
		if(mfMagnitudeDisplacement >= 1.0f)
		{
			tPos2d = _DestinationPos;
	   		tPos3d = new Vector3(tPos2d.x,tPos2d.y,mGameObjTransform.position.z);
			mGameObjTransform.position = tPos3d;
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
