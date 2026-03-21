using UnityEngine;
using System.Collections;

public class ClickableGameObjectMenu : MonoBehaviour {

	// Use this for initialization
	public GameObject[] _ButtonArray;
	void Start () 
	{
		
	}
	
	public void Init()
	{
		for(int i = 0; i< _ButtonArray.Length ;i++)
		{
			_ButtonArray[i].GetComponent<ClickableGameObject>()._CallBack = OnClick;
		}
	}
	
	public virtual void OnClick(GameObject pObj, string pTag)
	{
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
