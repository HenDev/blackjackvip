using UnityEngine;
using System.Collections;

public class CDelayTime : CAction {

	// Use this for initialization
	float mfCompletion = 0.0f;
	float mfRate = 0.0f;
    ActionState meState = ActionState.ActionNotRunning;
	
	
    override public void runAction()
	{
		mfCompletion = 0.0f;
		meState = ActionState.ActionRun;
		mfRate = 1.0f/_duration;
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
	
	// STATIC CREATE WITH AUTORELEASE
	public static CDelayTime create(float duration)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CDelayTime";
		
		CDelayTime action = actionObject.AddComponent<CDelayTime>();
		action.actionWithDuration(duration);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
	public void actionWithDuration(float duration)
	{
		_duration = duration;
	}
	
	override public void  setCompletion(float completion)
	{
		
	}
	
    // Update is called once per frame
    void Update () 
    {
        if(meState == ActionState.ActionRun)
        {
			mfCompletion += Time.deltaTime * mfRate;
            if(mfCompletion >= 1.0f) 
            {
				onActionComplete();
            }
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
}
