using UnityEngine;
using System.Collections;

public class CChangeColorSprirte : CAction
{
	public Sprite _TargetSprite = null;
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
	public static CChangeColorSprirte create(Sprite pSprite, Color pEndColor,float pDuration)
	{
		GameObject actionObject = new GameObject();
		actionObject.name = "CChangeColotSprirte";
		
		CChangeColorSprirte action = actionObject.AddComponent<CChangeColorSprirte>();
		action.actionWith(pSprite, pEndColor, pDuration);
		
		action.autoReleaseCallBack = action.autoReleaseAfterCompletion;
		return action;
	}
	
	public void actionWith(Sprite pSprite, Color pEndColor,float pDuration)
	{
		_DestinationColor = new Vector4(pEndColor.r,pEndColor.g,pEndColor.b,pEndColor.a);
		_duration = pDuration;
		_TargetSprite = pSprite;
	}
		
	override public void runAction()
	{
		if(_TargetSprite == null)
		{
			Debug.Log("Missing target object in action script");
			return;
		}
		Color tStartColor = _TargetSprite.color;
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
   		_TargetSprite.color = mColor;
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
