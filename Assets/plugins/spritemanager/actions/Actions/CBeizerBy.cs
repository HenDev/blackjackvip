using UnityEngine;
using System.Collections;

public class cBezierConfig {
	public Vector2 controlPoint_1;
	public Vector2 controlPoint_2;
	public Vector2 endPosition;
}

public class CBezierBy : CAction
{
	// Use this for initialization
	public GameObject _TargetGameObject;
	
	public Vector2 _StartPos = new Vector2(0,0);	
	protected Vector2 _DestinationPos = new Vector2(0,0);
	protected Transform mGameObjTransform = null;
	protected cBezierConfig m_sConfig = null;
	protected float mfMagnitudeDisplacement = 0.0f;
	protected float mfRate = 0.0f;
	protected ActionState meState = ActionState.ActionNotRunning;
	
	// Use this for initialization
	void Start ()
	{
		_SelfUpdate = true;
	}
	
	// STATIC CREATE WITH AUTORELEASE
	public static CBezierBy create(GameObject pTargetObject, Vector2 pEndPos, Vector2 pControlPoint1, Vector2 pControlPoint2,float pDuration)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CBezierBy";
		
		CBezierBy action = actionObject.AddComponent<CBezierBy>();
		action.actionWith(pTargetObject, pEndPos, pControlPoint1, pControlPoint2,pDuration);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
	public virtual void actionWith(GameObject pTargetObject, Vector2 pEndPos, Vector2 pControlPoint1, Vector2 pControlPoint2,float pDuration)
	{
		m_sConfig = new cBezierConfig();
		m_sConfig.controlPoint_1 = pControlPoint1;
		m_sConfig.controlPoint_2 = pControlPoint2;
		m_sConfig.endPosition = pEndPos;
		_DestinationPos = _StartPos + pEndPos;
		
		_duration = pDuration;
		_TargetGameObject = pTargetObject;
	}
		
	override public void runAction()
	{
		if(_TargetGameObject == null)
		{
			Debug.Log("Missing target object in action script");
			return;
		}
		mGameObjTransform = _TargetGameObject.transform;
		_StartPos = mGameObjTransform.position;
		meState = ActionState.ActionRun;
		mfRate = 1.0f/_duration;
		mfMagnitudeDisplacement = 0.0f;
		mGameObjTransform.position = new Vector3(_StartPos.x,_StartPos.y,mGameObjTransform.position.z);
		_DestinationPos = _StartPos + m_sConfig.endPosition;
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
	
	override public void setCompletion(float completion)
	{
//		mfMagnitudeDisplacement = completion;
//		Vector2 tPos2d;
//		Vector3 tPos3d;
//		if(mfDistanceMagnitude > 0)
//		{
//			tPos2d = _StartPos + mvNormalized * (mfDistanceMagnitude*mfMagnitudeDisplacement);
//			tPos3d = new Vector3(tPos2d.x,tPos2d.y,mGameObjTransform.position.z);
//   			mGameObjTransform.position = tPos3d;
//		}
	}
	
	void  performAction()
	{
		// --------------------------------------------------------
		// --------------------------------------------------------
		/*// From cocos2dx
		
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
		*/		
		// --------------------------------------------------------
		// --------------------------------------------------------
		
		Vector2 tPos2d;
		Vector3 tPos3d;
		float time = mfMagnitudeDisplacement;
		if (mGameObjTransform != null)
	    {
	        float xa = 0;
	        float xb = m_sConfig.controlPoint_1.x;
	        float xc = m_sConfig.controlPoint_2.x;
	        float xd = m_sConfig.endPosition.x;
	
	        float ya = 0;
	        float yb = m_sConfig.controlPoint_1.y;
	        float yc = m_sConfig.controlPoint_2.y;
	        float yd = m_sConfig.endPosition.y;
	
	        float x = bezierat(xa, xb, xc, xd, time);
	        float y = bezierat(ya, yb, yc, yd, time);
			
			tPos2d = _StartPos + new Vector2(x, y);
	   		tPos3d = new Vector3(tPos2d.x,tPos2d.y,mGameObjTransform.position.z);
			mGameObjTransform.position = tPos3d;
	    }
		
		if(mfMagnitudeDisplacement >= 1.0f)
		{
			tPos2d = _DestinationPos;
	   		tPos3d = new Vector3(tPos2d.x,tPos2d.y,mGameObjTransform.position.z);
			mGameObjTransform.position = tPos3d;
			onActionComplete();
		}
	}
	
	// Bezier cubic formula:
	//    ((1 - t) + t)3 = 1 
	// Expands to°≠ 
	//   (1 - t)3 + 3t(1-t)2 + 3t2(1 - t) + t3 = 1 
	static float bezierat( float a, float b, float c, float d, float t )
	{
	    return (Mathf.Pow(1-t,3) * a + 
	            3*t*(Mathf.Pow(1-t,2))*b + 
	            3*Mathf.Pow(t,2)*(1-t)*c +
	            Mathf.Pow(t,3)*d );
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
	}
}
