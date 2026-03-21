using UnityEngine;
using System.Collections;

public class ParallaxBgScr : MonoBehaviour
{
	
	public Transform _goPlayerObjectTransform;
	public float _fspeedFactor;
	public bool _bContiniousScroll = true;
	
	float mfdist = 0; //Virtual Distance
	private Vector2 uvPosition;
	private float tSpeed =  0.0005f;
	Material meshMaterial = null;
	private float previousPos = 0;
	
	void Start ()
	{
		if(_goPlayerObjectTransform == null)
			_goPlayerObjectTransform = gameObject.transform;
		
		uvPosition = new Vector3(0,0);
		GetComponent<Renderer>().material.mainTextureOffset = uvPosition;
		tSpeed = _fspeedFactor * 0.0005f;
		meshMaterial = GetComponent<Renderer>().material;
	}
	
	void Update () 
	{
		if(!_bContiniousScroll)
		{
			int pos = (int)(_goPlayerObjectTransform.position.x * tSpeed);
			float betterPos = _goPlayerObjectTransform.position.x * tSpeed - pos;
			if(betterPos - previousPos > 0)
			{
				betterPos = (betterPos+previousPos)/2.0f;
				previousPos = betterPos;
			}
			else
			{
				previousPos = betterPos;
				betterPos = 1.0f;
			}

			uvPosition.x = betterPos;
			meshMaterial.mainTextureOffset = uvPosition;
		}
		else
		{
			mfdist += tSpeed;
			int pos = (int)(mfdist);
			float betterPos = mfdist - pos;			
			uvPosition.x = betterPos;
			meshMaterial.mainTextureOffset = uvPosition;
		}
	}
}