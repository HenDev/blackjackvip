using UnityEngine;
using System.Collections;

public class FrameAnimation : CAction
{
	public GameObject _anchor;
	public int _repeatCount = 0;
	public bool _returnToOriginalFrame = false;
	public Vector2 _scale = new Vector2(0.5f,0.5f);
	
	public Vector4[] uvCoords;
	public string animationName;
	public Vector4 originalUV;
	public Vector3 originalScale;
	
	int mfCompletion = 0;
	float mfDurationPerFrame = 1.0f;
	float mfTime = 0.0f;
	ActionState meState = ActionState.ActionNotRunning;
	int curFrameNumber = 0;
	
	//ANIMATION DELEGATE
	public delegate void animationComplete(FrameAnimation animationScr);
	public animationComplete animationCompleteCallBack = null;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		float modifiedtimeFactor = Time.deltaTime;
		if(meState == ActionState.ActionRun)
		{
			
			mfTime += modifiedtimeFactor;
			if(mfTime >= mfDurationPerFrame)
			{
				StepAnimation();
				mfTime = 0.0f;
			}
		}		
	}
	
	public void ActionWith(float duration)
	{
		_duration = duration;
	}
	
	public void ActionWith(float duration, int repeat)
	{
		_duration = duration;
		_repeatCount = repeat;
	}
	
	public void stopAction()
	{
		meState = ActionState.ActionNotRunning;
		
	}
	
	override public void runAction()
	{
		if(meState == ActionState.ActionNotRunning) 
		{
			uvCoords = GetComponent<AnimationFrameHandler>().getUVCoordForAnimationName(animationName);
			_anchor = GetComponent<AnimationFrameHandler>()._anchor;
			originalUV = new Vector4(GetComponent<Renderer>().material.mainTextureOffset.x,GetComponent<Renderer>().material.mainTextureOffset.y,GetComponent<Renderer>().material.mainTextureScale.x,GetComponent<Renderer>().material.mainTextureScale.y);
			originalScale = _anchor.transform.localScale;
			meState = ActionState.ActionRun;
			mfDurationPerFrame = _duration/uvCoords.Length;
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
	
	void StepAnimation()
	{
		curFrameNumber++;
		if(curFrameNumber >= uvCoords.Length)
			curFrameNumber = 0;
		
		//SET UV HERE
		Vector4 tUVCoord = uvCoords[curFrameNumber];
		GetComponent<Renderer>().material.mainTextureOffset = new Vector2(tUVCoord.x,tUVCoord.y);
		GetComponent<Renderer>().material.mainTextureScale = new Vector2(tUVCoord.z,tUVCoord.w);
		_anchor.transform.localScale = new Vector3((tUVCoord.z/originalUV.z)*originalScale.x,(tUVCoord.w/originalUV.w)*originalScale.y,originalScale.z);
		
		mfCompletion++;
		if(mfCompletion >= uvCoords.Length)
		{
			onActionComplete();
		}
	}
	
	void onActionComplete()
	{
		if(_repeatCount == 0)
		{
			//COMPLETION HANDLER CODE
			meState = ActionState.ActionComplete;
			
			if(animationCompleteCallBack != null)
				animationCompleteCallBack(this);
		}
		else if(_repeatCount > 0)
		{
			_repeatCount--;
			mfCompletion = 0;
			runAction();
		}
	}
	
	void setScale(Vector2 pScale)
	{
		_scale = pScale;
	}
}
