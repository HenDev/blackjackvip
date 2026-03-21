using UnityEngine;
using System.Collections;


//UNAUTORIZED MODIFICATION IS NOT ALLOWED
public class CRepeat : CAction 
{
    void Start () {  }
	// Use this for initialization
    private int _loop = 0;
	private CAction mTargetAction;
	
	// STATIC CREATE WITH AUTORELEASE
	public static CRepeat create(CAction pAction,int pLoops)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CRepeat";
		
		CRepeat action = actionObject.AddComponent<CRepeat>();
		action.actionWithAction(pAction,pLoops);
		pAction.removeAutoRelease();
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
 	public void actionWithAction(CAction pAction,int pLoops)
	{
		mTargetAction = pAction;
		mTargetAction.internalAnimationCallBack = OnInternalCallBack;
		_loop = pLoops;
	}
	
	override public void runAction()
	{
		if(mTargetAction != null)
			mTargetAction.runAction();
	}
	
	override public void pauseAction()
	{
		if(mTargetAction != null)
			mTargetAction.pauseAction();
	}
	
	override public void resumeAction()
	{
		if(mTargetAction != null)
			mTargetAction.resumeAction();
	}
	
	override public void autoReleaseAfterCompletion(CAction action)
	{
		if(mTargetAction != null)
			mTargetAction.autoReleaseAfterCompletion(mTargetAction);
		if(action.gameObject != null)
			DestroyObject(action.gameObject);			
	}
	
	public void completeAction()
	{
		_loop = 0;
		OnInternalCallBack(this);
	}
	
	void OnInternalCallBack(CAction action)
	{
		//Debug.Log("Repeat callback");
		if(_loop != 0)
        {
            if(_loop > 0)
                _loop--;
			
			mTargetAction.runAction();
        }
		else if(_loop == 0)
		{
			if(internalAnimationCallBack != null)
				internalAnimationCallBack(this);
			if(autoReleaseCallBack != null)
				autoReleaseCallBack(this);
		}
	}
}
