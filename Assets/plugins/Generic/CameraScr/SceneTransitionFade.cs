using UnityEngine;
using System.Collections;

public class SceneTransitionFade : MonoBehaviour {
	
	public float fadeInDuration = 0.5f;
	public float fadeOutDuration = 0.5f;
	public float fadeOutInitialDelay = 0.0f;
	public GameObject mAlphaLayer = null;
	private string sceneNameToLoad = "";
	
	private static SceneTransitionFade mSharedHandler = null;
	public static SceneTransitionFade shaerdHandler()
	{
		if(mSharedHandler == null)
		{
			mSharedHandler = Camera.main.GetComponent<SceneTransitionFade>();
			if(mSharedHandler == null) {
				mSharedHandler = Camera.main.gameObject.AddComponent<SceneTransitionFade>();
				mSharedHandler.stopFadeOut(); }
		}
		return mSharedHandler;
	}
	
	// Use this for initialization
	void Start () { showFadeOut(); }
	
//	private void createAlphaLayer(float alpha)
//	{		
//		if(mAlphaLayer != null) return;
//		mAlphaLayer = GameObject.CreatePrimitive(PrimitiveType.Cube);
//		mAlphaLayer.name = "AlphaLayer";
//		mAlphaLayer.transform.parent = Camera.main.gameObject.transform;
//		Destroy(mAlphaLayer.GetComponent<BoxCollider>());
//		mAlphaLayer.transform.localScale = new Vector3(32.0f * (Camera.main.orthographicSize/12),24.0f * (Camera.main.orthographicSize/12),0.0f);
//		mAlphaLayer.transform.position = Camera.main.transform.position + new Vector3(0,0,1);
//		mAlphaLayer.transform.renderer.material.shader = Shader.Find("Transparent/Diffuse");
//		mAlphaLayer.transform.renderer.material.color = new Color(0,0,0,alpha);
//	}
	
	private void stopFadeOut()
	{
		mAlphaLayer.transform.position = new Vector3(0,0,Camera.main.transform.position.z-1);
	}	
	
	public void showFadeOut(float duration)
	{		
		CDelayTime delay = CDelayTime.create(fadeOutInitialDelay);
		CChangeColorMat fadeOut = CChangeColorMat.create(mAlphaLayer.transform.GetComponent<Renderer>().material, new Color(0,0,0,0), duration);
		CCallFunc call = CCallFunc.create(onFadeCompleted);
		CSequence seq = CSequence.create(delay,fadeOut,call);
		seq.runAction();
	}
	
	public void showFadeOut()
	{		
		//showFadeOut(fadeOutDuration);
	}
	
	public void showFadeIn(string sceneName)
	{
		showFadeIn(fadeInDuration);		
		sceneNameToLoad = sceneName;
		CCallFunc.createCallAfterDelay(onFadeInCompleted,1.0f).runAction();
	}
	
	public void showFadeIn(float duration)
	{
//		createAlphaLayer(0.0f);
		
		CChangeColorMat fadeIn = CChangeColorMat.create(mAlphaLayer.transform.GetComponent<Renderer>().material, new Color(0,0,0,1.0f), duration);
		fadeIn.runAction();
	}
	
	public void onFadeCompleted()
	{
		if(mAlphaLayer != null)	Destroy(mAlphaLayer);
		mAlphaLayer = null;
	}
	
	public void onFadeInCompleted()
	{
		Application.LoadLevel(sceneNameToLoad);
	}
}
