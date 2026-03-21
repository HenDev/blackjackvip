using UnityEngine;
using System.Collections;

public class AlphaLayerHandler : MonoBehaviour {

	// Use this for initialization
	void Start () { }
	// Update is called once per frame
	void Update () { }
	
	public void fadeOutAlphaLayer(float duration)
	{
		changeColorAction(duration, 0.0f);
	}
	
	public void fadeInAlphaLayer(float duration, float alphaValue)
	{
		changeColorAction(duration, alphaValue);
	}
	
	void changeColorAction(float duration, float alphaValue)
	{
		if(GetComponent<CChangeColorMat>() != null)
			DestroyImmediate(GetComponent<CChangeColorMat>());
		
		CChangeColorMat colorChange = gameObject.AddComponent<CChangeColorMat>();
		colorChange.actionWith(transform.GetComponent<Renderer>().material, new Color(1,1,1,alphaValue), duration);
		
		colorChange.runAction();
	}
}
