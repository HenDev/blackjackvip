using UnityEngine;
using System.Collections;

public class CEaseElastic : CEaseAction 
{

	// Use this for initialization
	// Use this for initialization
	public float _Period = 0.3f;
	
	private CAction mTargetAction;
	ActionState meState = ActionState.ActionNotRunning;
	float mfCompletion = 0.0f;
	float mfInnerActionCompletion = 0.0f;
	float mfRate = 0.0f;
	float mfTime = 0.0f;
	float mfTimeRatio = 0.0f;
	
	
	// STATIC CREATE WITH AUTORELEASE
	public static CEaseElastic create(CAction pAction,EaseType pEaseType)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CEaseElastic";
		
		CEaseElastic action = actionObject.AddComponent<CEaseElastic>();
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
		_Period = 0.3f;
	}
	
	override public void runAction()
	{
		if(mTargetAction != null)
		{
			mfTime = 0.0f;
			mfCompletion = 0.0f;
			mfInnerActionCompletion = 0.0f;
			mTargetAction.runAction();
			meState = ActionState.ActionRun;
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
			
			float tempTimeRatio1;
			float tempTimeRatio2;
			switch(meEasetype)
			{
				case EaseType.EaseIn:
					tempTimeRatio1 = 0;
					tempTimeRatio2 = mfTimeRatio;
					if (mfTimeRatio == 0 || mfTimeRatio == 1)
						tempTimeRatio1 = mfTimeRatio;
	
					else 
					{
						float s = _Period / 4.0f;
						tempTimeRatio2 = tempTimeRatio2 - 1.0f;
						tempTimeRatio1 = -1*Mathf.Pow(2, 10 * tempTimeRatio2) * Mathf.Sin( (tempTimeRatio2-s) *(Mathf.PI*2) / _Period);
					}
					mfInnerActionCompletion = tempTimeRatio1;
					break;
				case EaseType.EaseOut:
					tempTimeRatio1 = 0;
					tempTimeRatio2 = mfTimeRatio;
					if (mfTimeRatio == 0 || mfTimeRatio == 1)
						tempTimeRatio1 = mfTimeRatio;
	
					else 
					{
						float s = _Period / 4.0f;
						tempTimeRatio1 = Mathf.Pow(2, -10 * tempTimeRatio2) * Mathf.Sin( (tempTimeRatio2-s) *(Mathf.PI*2) / _Period) + 1;
					}
					mfInnerActionCompletion = tempTimeRatio1;
					break;
				
				case EaseType.EaseInOut:
					
					if (mfTimeRatio == 0 || mfTimeRatio == 1)
						tempTimeRatio1 = mfTimeRatio;
	
					else 
					{
						tempTimeRatio2 = 2*mfTimeRatio;
						//if(! _Period )
						//	_Period = 0.3f * 1.5f;
						float s = _Period / 4.0f;
					
						tempTimeRatio2 = tempTimeRatio2 - 1.0f;
						if( tempTimeRatio2 < 0 )
							tempTimeRatio1 = -0.5f * Mathf.Pow(2, 10 * tempTimeRatio2) * Mathf.Sin( (tempTimeRatio2-s) *(Mathf.PI*2) / _Period);
						else
							tempTimeRatio1 = Mathf.Pow(2, -10 * tempTimeRatio2) * Mathf.Sin( (tempTimeRatio2-s) *(Mathf.PI*2) / _Period)*0.5f + 1;
					}
					mfInnerActionCompletion = tempTimeRatio1;
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
