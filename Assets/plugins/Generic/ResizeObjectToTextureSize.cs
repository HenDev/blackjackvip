using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ResizeObjectToTextureSize : MonoBehaviour {
			
	public Vector2 _screenPixelSize = new Vector2(1024,768);
	public float _cameraSize = 12;
	public enum ObjectType {UnityPlane, UnityCube};
	public ObjectType currentObjectType = ObjectType.UnityPlane;
	
	
	// Use this for initialization
	void Start () {
//		DestroyObject(this);
	}
	
	// Update is called once per frame
	void Update () {
		
		
		Material material = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
		if(material == null || material.mainTexture == null) return;
		if(currentObjectType == ObjectType.UnityPlane)
		transform.localScale = new Vector3((material.mainTexture.width/_screenPixelSize.x)*3.2f,transform.localScale.y,(material.mainTexture.height/_screenPixelSize.y)*2.4f);
		else
		transform.localScale = new Vector3(material.mainTexture.width/32.0f,material.mainTexture.height/32.0f,transform.localScale.z);
		
//		if(_CorrectWidth == true)
//		{
//			correctWidthRatio();
//			_CorrectWidth = false;	
//		}
//		if(_CorrectHeight == true)
//		{
//			correctHeightRatio();
//			_CorrectHeight = false;	
//		}
//		if(_CorrectSize == true)
//		{
//			correctSizeRatio();
//			_CorrectSize = false;	
//		}
	}
	
//	void correctWidthRatio()
//	{
//		Material material = gameObject.GetComponent<MeshRenderer>().material;
//		if(material == null || material.mainTexture == null) return;
//			
//		transform.localScale = new Vector3(2,2,2);
//	}
//	
//	void correctHeightRatio()
//	{
//		Material material = gameObject.GetComponent<MeshRenderer>().material;
//		if(material == null || material.mainTexture == null) return;
//		transform.localScale = new Vector3(2,2,2);
//	}
//	
//	void correctSizeRatio()
//	{
//		Material material = gameObject.GetComponent<MeshRenderer>().material;
//		if(material == null || material.mainTexture == null) return;
//		transform.localScale = new Vector3(2,2,2);
//	}
	
}
