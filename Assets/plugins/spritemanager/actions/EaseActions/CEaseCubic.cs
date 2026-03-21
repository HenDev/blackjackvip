using UnityEngine;
using System.Collections;

public class CEaseCubic : CEaseAction 
{
	// Use this for initialization
	private CAction mTargetAction;
	ActionState meState = ActionState.ActionNotRunning;
	float mfCompletion = 0.0f;
	float mfInnerActionCompletion = 0.0f;
	float mfRate = 0.0f;
	float mfTime = 0.0f;
	float mfTimeRatio = 0.0f;
	float mfReverseRate = 0.0f;
	
	// STATIC CREATE WITH AUTORELEASE
	public static CEaseCubic create(CAction pAction,EaseType pEaseType)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CEaseCubic";
		
		CEaseCubic action = actionObject.AddComponent<CEaseCubic>();
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
		mfReverseRate = 1.0f/mfRate;
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
			
			float tempRatio = 2*mfTimeRatio;
			switch(meEasetype)
			{
				case EaseType.EaseIn:
					mfInnerActionCompletion =  Mathf.Pow(mfTimeRatio,mfRate);
					break;
				case EaseType.EaseOut:
					mfInnerActionCompletion =  Mathf.Pow(mfTimeRatio,mfReverseRate);
					break;
				case EaseType.EaseInOut:
					if (mfTimeRatio <= 0.5f) 
						mfInnerActionCompletion =  0.5f * Mathf.Pow(tempRatio,mfRate);
					else
   						mfInnerActionCompletion =  1.0f - (0.5f * Mathf.Pow(2 - tempRatio,mfRate));
					break;
				default:
					break;
			}
			
			mTargetAction.setCompletion(mfInnerActionCompletion);
			if(mfCompletion >= 1.0f)
			{
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
