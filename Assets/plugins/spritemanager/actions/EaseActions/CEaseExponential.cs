using UnityEngine;
using System.Collections;

public class CEaseExponential : CEaseAction {

	// Use this for initialization
	private CAction mTargetAction;
	ActionState meState = ActionState.ActionNotRunning;
	float mfCompletion = 0.0f;
	float mfInnerActionCompletion = 0.0f;
	float mfRate = 0.0f;
	float mfTime = 0.0f;
	float mfTimeRatio = 0.0f;	
	
	// STATIC CREATE WITH AUTORELEASE
	public static CEaseExponential create(CAction pAction,EaseType pEaseType)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CEaseExponential";
		
		CEaseExponential action = actionObject.AddComponent<CEaseExponential>();
		action.actionWithAction(pAction,pEaseType);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
 	public void actionWithAction(CAction pAction,EaseType pEaseType)
	{
		mTargetAction = pAction;
		mTargetAction.internalAnimationCallBack = OnInternalCallBack;
		mTargetAction._SelfUpdate = false;
		meEasetype = pEaseType;
		_duration = mTargetAction._duration;
		mfRate = 1.0f/_duration;
	}
	
	override public void runAction()
	{
		if(mTargetAction != null)
		{
			mfTime = 0.0f;
			mfCompletion = 0.0f;
			mfInnerActionCompletion = 0.0f;
			meState = ActionState.ActionRun;
			mTargetAction.runAction();
		}
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
	
	void Update()
	{
		if(meState == ActionState.ActionRun)
		{
			//LINEAR
			mfTime += Time.deltaTime;
			mfTimeRatio = mfTime/_duration;
			mfCompletion += Time.deltaTime * mfRate;
			
			float tempRatio;
			switch(meEasetype)
			{
				case EaseType.EaseIn:
					mfInnerActionCompletion =  (mfTimeRatio==0) ? 0 :(Mathf.Pow(2,10*(mfTimeRatio/1.0f-1.0f))-1 * 0.001f);
					break;
				case EaseType.EaseOut:
					mfInnerActionCompletion =  (mfTimeRatio==1) ? 1 :(-1*Mathf.Pow(2,-10*(mfTimeRatio/1.0f))+1);
					break;
				case EaseType.EaseInOut:
					tempRatio = 2*mfTimeRatio;
					if (tempRatio <= 1) 
						mfInnerActionCompletion =  0.5f * Mathf.Pow(2,10*(tempRatio-1));
					else
   						mfInnerActionCompletion =  0.5f * (-1*Mathf.Pow(2,-10*(tempRatio-1))+2);
					break;
				default:
					break;
			}
			
			mTargetAction.setCompletion(mfInnerActionCompletion);
			if(mfCompletion >= 1.0f)
			{
//				Debug.Log("ActionComplete");
				mfInnerActionCompletion = 1.0f;
				mTargetAction.setCompletion(mfInnerActionCompletion);
				onActionComplete();
			}
		}
		
	}
	
	void OnInternalCallBack(CAction action)
	{
		
	}
	
	void onActionComplete()
	{
		meState = ActionState.ActionComplete;
		if(internalAnimationCallBack != null)
			internalAnimationCallBack(this);
		if(autoReleaseCallBack != null)
			autoReleaseCallBack(this);
	}
}
