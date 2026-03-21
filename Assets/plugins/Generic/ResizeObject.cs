using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ResizeObject : MonoBehaviour {
	
	public enum ImageDeviceType {Ipod=0, Iphone5, Ipad};
	
	public ImageDeviceType ImageMadeForDevice = ImageDeviceType.Ipad;
	public bool isPortraitMode = false;
	public string CurrentDeviceMode = "";
	public Vector2 _screenPixelSize = new Vector2(1024,768);
	public float _cameraSize = 1;
	
	public bool CorrectTheImage = false;	
	
	// Use this for initialization
	void Start () { initializeForDevice(); }	
	// Update is called once per frame
	void Update () { /*if(CorrectTheImage)*/ initializeForDevice();	}
		
	void initializeForDevice()
	{
		CorrectTheImage = false;
		Debug.Log(ImageMadeForDevice.ToString());
		switch(ImageMadeForDevice)
		{
			case ImageDeviceType.Ipad:
				_screenPixelSize = new Vector2(1024,768);
				break;
			case ImageDeviceType.Ipod:
				_screenPixelSize = new Vector2(960,640);
				break;
			case ImageDeviceType.Iphone5:
				_screenPixelSize = new Vector2(1136,640);
				break;			
		}
		
		CurrentDeviceMode = isPortraitMode ? "Portrait" : "Landscape";
		
		if(isPortraitMode) _screenPixelSize = new Vector2(_screenPixelSize.y,_screenPixelSize.x);
		_cameraSize = _screenPixelSize.y/64.0f;		
		
		Material material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
		if(material == null || material.mainTexture == null) return;
		
		float scale = 1.0f;// _cameraSize/Camera.main.orthographicSize;///_cameraSize;		
		transform.localScale = new Vector3((material.mainTexture.width/32.0f)*scale,(material.mainTexture.height/32.0f)*scale,transform.localScale.z);		
	}
		
}
