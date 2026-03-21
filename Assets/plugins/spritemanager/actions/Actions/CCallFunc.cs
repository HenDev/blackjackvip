using UnityEngine;
using System.Collections;

public class CCallFunc : CAction 
{
	// Use this for initialization
    public delegate void CallBack();
    public CallBack animationCompleteCallBack = null;
    public delegate void CallBackWithSender(GameObject sender);
    public CallBackWithSender animationCompleteCallBackWithSender = null;
	
    void Start () 
    {
		
    }
	
	// STATIC CREATE WITH AUTORELEASE
	public static CCallFunc create(CallBack pCompletionCallBack)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CCallFunc";
		
		CCallFunc action = actionObject.AddComponent<CCallFunc>();
		action.actionWithCallBack(pCompletionCallBack);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	public static CAction createCallAfterDelay(CallBack pCompletionCallBack, float pDelay)
	{
		CCallFunc call = create(pCompletionCallBack);
		CDelayTime delay = CDelayTime.create(pDelay);
		CSequence seq = CSequence.create(delay, call);
		return seq;
	}
	
	
	//SETTING CALL BACK DELEGATE
	public void actionWithCallBack(CallBack pCompletionCallBack)
	{
		animationCompleteCallBack = pCompletionCallBack;
	}
	public void actionWithCallBack(CallBackWithSender pCompletionCallBack)
	{
		animationCompleteCallBackWithSender = pCompletionCallBack;
	}
	
	override public void runAction()
	{
		onActionComplete();
	}
	
	void onActionComplete()
	{
		if(animationCompleteCallBack != null)
			animationCompleteCallBack();
		
		if(animationCompleteCallBackWithSender != null)
			animationCompleteCallBackWithSender(gameObject);
		
		if(internalAnimationCallBack != null)
			internalAnimationCallBack(this);
		
		if(autoReleaseCallBack != null)
			autoReleaseCallBack(this);
	}
}
