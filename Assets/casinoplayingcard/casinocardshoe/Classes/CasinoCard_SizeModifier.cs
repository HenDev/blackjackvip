using UnityEngine;
using System.Collections;

public class CasinoCard_SizeModifier : MonoBehaviour {
	
	public float mScaleRatio = 1.0f;
	// Use this for initialization
	void Start () {
		StartCoroutine("modifySize");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator modifySize()
	{		
		while(true)
		{
			yield return new WaitForSeconds(0.5f);		
			// Initializing the transform Scale
			float PtmRatio = 32.0f;
			float Width = GetComponent<Renderer>().material.mainTexture.width;
			float Height = GetComponent<Renderer>().material.mainTexture.height;
			float widthRatio = Width/PtmRatio;
			float heightRatio = Height/PtmRatio;
			transform.localScale = new Vector3(widthRatio*mScaleRatio, heightRatio*mScaleRatio,0.02f);
			DestroyObject(this);
		}
	}
}
