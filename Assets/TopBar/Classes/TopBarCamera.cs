using UnityEngine;
using System.Collections;

public class TopBarCamera : MonoBehaviour {
	
	public bool maintainCurrentCameraDepth = false;
	// Use this for initialization
	void Start () {
	StartCoroutine(Initialize());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void init()
	{	
		gameObject.GetComponent<Camera>().cullingMask = 0;
		gameObject.GetComponent<Camera>().cullingMask = 1 << gameObject.layer;
		
		Camera[] arrAvailableCameras = Camera.allCameras;
		foreach(Camera camera in arrAvailableCameras)
		{
			if(camera.name != name)
			{				
				camera.cullingMask &=  ~(1 << gameObject.layer);
				if(!maintainCurrentCameraDepth && gameObject.GetComponent<Camera>().depth < camera.depth)
					gameObject.GetComponent<Camera>().depth = camera.depth+1;
			}
		}
	}
	
	IEnumerator Initialize()
	{
		while(true)
		{
			yield return new WaitForEndOfFrame();			
			init();					
			break;
		}
	}
}
