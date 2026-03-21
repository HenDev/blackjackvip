using UnityEngine;
using System.Collections;

public class CSequence : CAction 
{

	// Use this for initialization
	CAction[] marrActionsArray;
	int actionIndex = 0;
	int actionCount = 0;
	void Start () 
	{
	
	}
	
	// STATIC CREATE WITH AUTORELEASE
	public static CSequence create(params CAction[] actions)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CSequence";
		
		CSequence action = actionObject.AddComponent<CSequence>();
		action.actionWithActions(actions);
		for(int i = 0; i<actions.Length; i++)
			actions[i].removeAutoRelease();
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
	public void actionWithActions(params CAction[] actions)
	{
		marrActionsArray = actions;
		actionCount = 0;
		foreach (CAction action in actions)
    	{
        	action.internalAnimationCallBack = onInternalCallBack;
			actionCount++;
    	}
	}
	
	override public void runAction()
	{
		if(actionCount > 0)
		{
			actionIndex = 0;
			//Debug.Log("Action number: "+actionIndex);
			if(marrActionsArray[actionIndex] != null)
				marrActionsArray[actionIndex].runAction();
		}
	}
	
	override public void pauseAction()
	{
		foreach (CAction action in marrActionsArray)
    	{
        	action.pauseAction();
    	}
	}
	
	override public void resumeAction()
	{
		foreach (CAction action in marrActionsArray)
    	{
        	action.resumeAction();
    	}
	}
	
	override public void autoReleaseAfterCompletion(CAction action)
	{
		if(marrActionsArray != null)
			foreach(CAction _action in marrActionsArray)
				_action.autoReleaseAfterCompletion(_action);
		if(action.gameObject != null)
			DestroyObject(action.gameObject);			
	}
	
	void onInternalCallBack(CAction action)
	{
		actionIndex++;
		//action.enabled = false;
		if(actionIndex >= actionCount)
		{
			if(internalAnimationCallBack != null)
			internalAnimationCallBack(this);
			if(autoReleaseCallBack != null)
				autoReleaseCallBack(this);
		}
		else
		{
			//Debug.Log("Action number: "+actionIndex);
			if(marrActionsArray[actionIndex] != null)
				marrActionsArray[actionIndex].runAction();
		}
	}
}
