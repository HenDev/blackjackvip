using UnityEngine;
using System.Collections;

public class AnimationStartUpScr : MonoBehaviour 
{

	// Use this for initialization
	private bool mbInitialized = false;
	void Start () 
	{
		
	}
	
	void Init()
	{		
		if(GetComponent<AnimationFrameHandler>())
			GetComponent<AnimationFrameHandler>().Init();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mbInitialized == false)
		{
			Init();
			mbInitialized = true;
		}
	}
}
