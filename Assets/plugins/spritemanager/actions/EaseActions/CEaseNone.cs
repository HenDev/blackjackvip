using UnityEngine;
using System.Collections;

public class CEaseNone : CAction
{
	// Use this for initialization
	private CAction mTargetAction;
	ActionState meState = ActionState.ActionNotRunning;
	float mfCompletion = 0.0f;
	float mfInnerActionCompletion = 0.0f;
	float mfRate = 0.0f;
	
	// STATIC CREATE WITH AUTORELEASE
	public static CEaseNone create(CAction pAction)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CEaseNone";
		
		CEaseNone action = actionObject.AddComponent<CEaseNone>();
		action.actionWithAction(pAction);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
 	public void actionWithAction(CAction pAction)
	{
		mTargetAction = pAction;
		mTargetAction.internalAnimationCallBack = OnInternalCallBack;
		mTargetAction._SelfUpdate = false;
		_duration = mTargetAction._duration;
		mfRate = 1.0f/_duration;
	}
	
	override public void runAction()
	{
		if(mTargetAction != null)
		{
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
			mfCompletion += Time.deltaTime * mfRate;
			mfInnerActionCompletion = mfCompletion;
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
		//Debug.Log("Ease actionComplete");
		meState = ActionState.ActionComplete;
		if(internalAnimationCallBack != null)
			internalAnimationCallBack(this);
		if(autoReleaseCallBack != null)
			autoReleaseCallBack(this);
	}
}
