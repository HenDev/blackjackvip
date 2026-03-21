using UnityEngine;
using System.Collections;

public class CScreenChangeColor : CAction {
	
	// Use this for initialization
	// Use this for initialization	
	public Vector4 _StartColor = new Vector4(0,0,0,0);
	public Vector4 _DestinationColor = new Vector4(0,0,0,0);
	
	private Vector4 mvNormalized = new Vector4(0,0,0,0);
	public Vector4 mvColor4d = new Vector4(0,0,0,0);
	private Color mColor = new Color(0,0,0,0);
	
	public float mfDistanceMagnitude = 0.0f;
	public float mfMagnitudeDisplacement = 0.0f;
	public float mfRate = 0.0f;
	ActionState meState = ActionState.ActionNotRunning;
	
	// Use this for initialization
	void Start ()
	{
		_SelfUpdate = true;
	}
	
	// STATIC CREATE WITH AUTORELEASE
	public static CScreenChangeColor create(Color pEndColor,float pDuration)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CScreenChangeColor";
		
		CScreenChangeColor action = actionObject.AddComponent<CScreenChangeColor>();
		action.actionWith(pEndColor,pDuration);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
	public void actionWith(Color pEndColor,float pDuration)
	{
		_DestinationColor = new Vector4(pEndColor.r,pEndColor.g,pEndColor.b,pEndColor.a);
		_duration = pDuration;
	}
		
	override public void runAction()
	{
		Color tStartColor = RenderSettings.ambientLight;
		_StartColor = new Vector4(tStartColor.r,tStartColor.g,tStartColor.b,tStartColor.a);
		meState = ActionState.ActionRun;
		mfRate = 1.0f/_duration;
		mvNormalized = _DestinationColor - _StartColor;
		mfDistanceMagnitude = Mathf.Sqrt(Vector4.SqrMagnitude(mvNormalized));
		mvNormalized = Vector4.Normalize(mvNormalized);
		mfMagnitudeDisplacement = 0.0f;
		mvColor4d = _StartColor;
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
		mfMagnitudeDisplacement = completion;
		mvColor4d = _StartColor + mvNormalized * (mfDistanceMagnitude*mfMagnitudeDisplacement);
		mColor.r = mvColor4d.x;
		mColor.g = mvColor4d.y;
		mColor.b = mvColor4d.z;
		mColor.a = mvColor4d.w;
   		RenderSettings.ambientLight = mColor;
	}
	
	void  performAction()
	{
		if(mfMagnitudeDisplacement > 0 && mfMagnitudeDisplacement < 1.0f)
		{
			setCompletion(mfMagnitudeDisplacement);
		}
	
		else if(mfMagnitudeDisplacement >= 1.0f)
		{
			setCompletion(1.0f);
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
				mfMagnitudeDisplacement += (0.02f * mfRate);
				performAction();
			}
		}
	}
}
