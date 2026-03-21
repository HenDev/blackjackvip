using UnityEngine;
using System.Collections;


public class MouseDownHandler : MonoBehaviour {
	
	public Camera mCameraToDetectMouseDown = null;
	public bool isMouseOver = false;
	
	// Use this for initialization
	void Start () { if(mCameraToDetectMouseDown == null) mCameraToDetectMouseDown = Camera.main; }	
	// Update is called once per frame
	void Update () {	
		RaycastHit hit = new RaycastHit();
		Vector3 origin = mCameraToDetectMouseDown.ScreenToWorldPoint(Input.mousePosition);
		origin = new Vector3(origin.x, origin.y, mCameraToDetectMouseDown.transform.position.z);
		Vector3 direction = Vector3.forward*100;
		if(Physics.Raycast(origin, direction, out hit) && hit.collider.gameObject == gameObject)
		{
			isMouseOver = true;
		}
		else
		{
			isMouseOver = false;
		}		

		if(DeviceModelInfo.getIsIosDevice() && !Input.GetMouseButton(0)) isMouseOver = false;
	}
}
