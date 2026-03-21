using UnityEngine;
using System.Collections;

public class AnimationFrameHandler : MonoBehaviour 
{
	
	//[SerializeField]
	public GameObject _anchor;
	public AnimationDataParams[] _ArrAnimationdata = null;
	public LinkedSpriteManager _SpriteManager = null;
	public SpriteAtlasDataHandler _SpriteAtlasDataHandler = null;
	public string _SpriteFrameName = null;
	public Vector2 _scale = new Vector2(0.5f,0.5f);
	
	//ANIMATION DELEGATE
	public delegate void animationComplete();
	public animationComplete animationCompleteCallBack = null;
	
	private bool mbInitialized = false;
	private AnimationStartUpScr tStartupScript = null;
	private FrameAnimation mCurFrameAnimation = null;
	private string mdefaultAnimation;
	
	// Use this for initialization
	void Start () 
	{
		if(_anchor == null)
			_anchor = gameObject;
		
		runAnimationNamed("Run");
		tStartupScript = gameObject.AddComponent<AnimationStartUpScr>();
	}
		
	//INIT WITH DEFAULT VALUES
	public void Init()
	{
		//DON'T INITIALIZE MORE THAN ONCE
		if(mbInitialized == true)
		{
			Debug.Log("Initiailization more than once");
			return;
		}
		
		if(_SpriteManager == null || _SpriteAtlasDataHandler == null)
		{
			Debug.Log("Sprite manager or atlas not available");
			return;
		}
				
		if(tStartupScript != null)
		{
			Destroy(tStartupScript);
			tStartupScript = null;
		}
		
		GetComponent<Renderer>().material = _SpriteManager.material;
		setFrameWithName(_SpriteFrameName);
		mbInitialized = true;	
		if(mdefaultAnimation != null)
		{
			runAnimationNamed(mdefaultAnimation);
			mdefaultAnimation = null;
		}
	}		
	
	//LOADING SPRITE PARAMS
	private LoadedSpriteDataParams getSpriteDataParamWithName(string p_filename)
	{
		//GET LOADED SPRITE PARAMS FOR FRAME NAME
		LoadedSpriteDataParams loadedSpriteData = null;
		loadedSpriteData = _SpriteAtlasDataHandler.getLoadedSpriteData(p_filename);	

		if(loadedSpriteData == null)
			Debug.Log("Sprite frame named "+p_filename+" unavailable");
		
		return loadedSpriteData;
	}
	
	//TOGGLE SPRITE VISIBILILTY
	public void setVisible(bool p_boolVal)
	{
		GetComponent<Renderer>().enabled = false;
	}
	
	public void setFrameWithName(string frameName)
	{
		LoadedSpriteDataParams loadedSpriteData = getSpriteDataParamWithName(frameName);	
		if(loadedSpriteData == null)
			return;
		
		//SET UV ACCORDINGLY
		Vector2 startPosUV = _SpriteManager.PixelCoordToUVCoord(loadedSpriteData._iLeftPixel, loadedSpriteData._iBottomPixel);
        Vector2 spriteSize = _SpriteManager.PixelSpaceToUVSpace(loadedSpriteData._iWidth, loadedSpriteData._iHeight);
		GetComponent<Renderer>().material.mainTextureOffset = startPosUV;
		GetComponent<Renderer>().material.mainTextureScale = spriteSize;
		
		//_anchor.transform.localScale = new Vector3(loadedSpriteData._iWidth*mfInversePTMRatio*_scale.x,transform.localScale.y,loadedSpriteData._iHeight*mfInversePTMRatio*_scale.y);
	}
	
	AnimationDataParams getAnimationDataForAnimationName(string animationName)
	{
		AnimationDataParams requiredParam = null;
		for(int i = 0; i< _ArrAnimationdata.Length ; i++)
		{
			AnimationDataParams param = _ArrAnimationdata[i];
			if(param._name == animationName)
			{
				requiredParam = _ArrAnimationdata[i];
				break;
			}
		}
		return requiredParam;
	}
	
	Vector4 getUVCoordForFrame(string frameName)
	{
		LoadedSpriteDataParams loadedSpriteData = getSpriteDataParamWithName(frameName);	
		if(loadedSpriteData == null)
			return Vector4.zero;
		
		//SET UV ACCORDINGLY
		Vector2 startPosUV = _SpriteManager.PixelCoordToUVCoord(loadedSpriteData._iLeftPixel, loadedSpriteData._iBottomPixel);
        Vector2 spriteSize = _SpriteManager.PixelSpaceToUVSpace(loadedSpriteData._iWidth, loadedSpriteData._iHeight);
		return new Vector4(startPosUV.x,startPosUV.y,spriteSize.x,spriteSize.y);
	}
	
	public Vector4[] getUVCoordForAnimationName(string animationName)
	{
		AnimationDataParams param = getAnimationDataForAnimationName(animationName);
		Vector4[] uvCoords;
		if(param == null)
		{
			uvCoords = new Vector4[1];
			Debug.Log("No such animation available");
		}
		else
			 uvCoords = new Vector4[param._frameNames.Length];
		
		for(int i = 0; i< param._frameNames.Length; i++)
		{
			uvCoords[i] = getUVCoordForFrame(param._frameNames[i]);
		}
		return uvCoords;
	}
	
	public void runAnimationNamed(string animationName)
	{
		if(mbInitialized == false)
		{
			mdefaultAnimation = animationName;
			return;
		}
		
		if(mCurFrameAnimation != null)
		{
			mCurFrameAnimation.stopAction();
			Destroy(mCurFrameAnimation);
		}
		
		AnimationDataParams requiredParam = null;
		for(int i = 0; i< _ArrAnimationdata.Length ; i++)
		{
			AnimationDataParams param = _ArrAnimationdata[i];
			if(param._name == animationName)
			{
				requiredParam = _ArrAnimationdata[i];
				break;
			}
		}
		
		if(requiredParam == null)
		{
			Debug.Log("Animation doesnt exist");
			return;
		}
		
		FrameAnimation anim = gameObject.AddComponent<FrameAnimation>();
		anim.animationName = requiredParam._name;
		anim._duration = (requiredParam._fps/60.0f);
		anim._repeatCount = requiredParam._loopCycles;
		anim.animationCompleteCallBack = AnimationCompleted;
		anim.runAction();
		
		mCurFrameAnimation = anim;
	}
	
	void AnimationCompleted(FrameAnimation animation)
	{
		animationCompleteCallBack();
	}
}
