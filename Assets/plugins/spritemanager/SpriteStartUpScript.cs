using UnityEngine;
using System.Collections;

public class SpriteStartUpScript : MonoBehaviour {

	// Use this for initialization
	private bool mbInitialized = false;
	void Start () 
	{
	
	}
	
	void Init()
	{
		if(GetComponent<SpriteData>())
			GetComponent<SpriteData>().Init();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mbInitialized == false)
		{
			mbInitialized = true;
			Init();
		}
	}
}
