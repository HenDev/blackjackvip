using UnityEngine;
using System.Collections;

public class CCallFuncWithObj : CAction 
{
	// Use this for initialization
    public delegate void CallBack(GameObject obj);
    public CallBack animationCompleteCallBack = null;
	public GameObject _CallBackGameObject;
	
    void Start () 
    {
		
    }
	
	// STATIC CREATE WITH AUTORELEASE
	public static CCallFuncWithObj create(CallBack pCompletionCallBack, GameObject pGameObject)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CCallFuncWithObj";
		
		CCallFuncWithObj action = actionObject.AddComponent<CCallFuncWithObj>();
		action.actionWithCallBack(pCompletionCallBack,pGameObject);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
	//SETTING CALL BACK DELEGATE
	public void actionWithCallBack(CallBack pCompletionCallBack,GameObject pGameObject)
	{
		animationCompleteCallBack = pCompletionCallBack;
		_CallBackGameObject = pGameObject;
	}
	
	override public void runAction()
	{
		onActionComplete();
	}
	
	void onActionComplete()
	{
		if(animationCompleteCallBack != null)
			animationCompleteCallBack(_CallBackGameObject);
		
		if(internalAnimationCallBack != null)
			internalAnimationCallBack(this);	
		
		if(autoReleaseCallBack != null)
			autoReleaseCallBack(this);
	}
}