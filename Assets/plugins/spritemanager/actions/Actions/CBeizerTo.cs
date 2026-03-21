using UnityEngine;
using System.Collections;

public class CBezierTo : CBezierBy
{	
	// Use this for initialization
	void Start ()
	{
		_SelfUpdate = true;
	}
	
	// STATIC CREATE WITH AUTORELEASE
	public static CBezierTo create(GameObject pTargetObject, Vector2 pEndPos, Vector2 pControlPoint1, Vector2 pControlPoint2,float pDuration)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CBezierTo";
		
		CBezierTo action = actionObject.AddComponent<CBezierTo>();
		action.actionWith(pTargetObject, pEndPos, pControlPoint1, pControlPoint2,pDuration);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
	public override void actionWith(GameObject pTargetObject, Vector2 pEndPos, Vector2 pControlPoint1, Vector2 pControlPoint2,float pDuration)
	{				
		_duration = pDuration;
		_TargetGameObject = pTargetObject;
		_StartPos = _TargetGameObject.transform.position;
		m_sConfig = new cBezierConfig();
		m_sConfig.controlPoint_1 = pControlPoint1 - _StartPos;
		m_sConfig.controlPoint_2 = pControlPoint2 - _StartPos;
		m_sConfig.endPosition = pEndPos - _StartPos;
		_DestinationPos = pEndPos;
	}
	
	override public void setCompletion(float completion)
	{
	}
		
	override public void runAction()
	{
		Vector2 _tempDest = _DestinationPos;
		if(_TargetGameObject == null)
		{
			Debug.Log("Missing target object in action script");
			return;
		}
		base.runAction();		
		_DestinationPos = _tempDest;					
	}
	
//	override public void pauseAction()
//	{
//		if(meState == ActionState.ActionRun)
//			meState = ActionState.ActionPause;
//	}
//	
//	override public void resumeAction()
//	{
//		if(meState == ActionState.ActionPause)
//			meState = ActionState.ActionRun;
//	}
	
//	override public void setCompletion(float completion)
//	{
//		mfMagnitudeDisplacement = completion;
//		Vector2 tPos2d;
//		Vector3 tPos3d;
//		if(mfDistanceMagnitude > 0)
//		{
//			tPos2d = _StartPos + mvNormalized * (mfDistanceMagnitude*mfMagnitudeDisplacement);
//			tPos3d = new Vector3(tPos2d.x,tPos2d.y,mGameObjTransform.position.z);
//   			mGameObjTransform.position = tPos3d;
//		}
//	}
	
	/*void  performAction()
	{
		// --------------------------------------------------------
		// --------------------------------------------------------
		// From cocos2dx
		
		// Uses BezierBy for  BezierTo
		// Corrections for BezierTo for BezierBy
		
	    m_sConfig.controlPoint_1 = ccpSub(m_sConfig.controlPoint_1, m_startPosition);
	    m_sConfig.controlPoint_2 = ccpSub(m_sConfig.controlPoint_2, m_startPosition);
	    m_sConfig.endPosition = ccpSub(m_sConfig.endPosition, m_startPosition);
		
		// Update
		
		if (m_pTarget)
	    {
	        float xa = 0;
	        float xb = m_sConfig.controlPoint_1.x;
	        float xc = m_sConfig.controlPoint_2.x;
	        float xd = m_sConfig.endPosition.x;
	
	        float ya = 0;
	        float yb = m_sConfig.controlPoint_1.y;
	        float yc = m_sConfig.controlPoint_2.y;
	        float yd = m_sConfig.endPosition.y;
	
	        float x = bezierat(xa, xb, xc, xd, time);		// time - DeltaTime
	        float y = bezierat(ya, yb, yc, yd, time);
	        m_pTarget->setPosition(ccpAdd(m_startPosition, ccp(x, y)));
	    }
				
		// --------------------------------------------------------
		// --------------------------------------------------------
		
		Vector2 tPos2d;
		Vector3 tPos3d;
//		if(mfDistanceMagnitude > 0)
//		{	
//			tPos2d = _StartPos + mvNormalized * (mfDistanceMagnitude*mfMagnitudeDisplacement);
//			tPos3d = new Vector3(tPos2d.x,tPos2d.y,mGameObjTransform.position.z);
//   			mGameObjTransform.position = tPos3d;
//		}
	
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
	}*/
}
