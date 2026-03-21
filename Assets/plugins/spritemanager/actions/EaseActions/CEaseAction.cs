using UnityEngine;
using System.Collections;

public enum EaseType
{
	EaseIn,
	EaseOut,
	EaseInOut,
}

public enum EaseActionType
{
	EaseNone,
	EaseNormal,
	EaseCubic,
	EaseExponential,
	EaseElastic,
}

public class CEaseAction : CAction 
{
	// Use this for initialization
	public EaseType meEasetype = EaseType.EaseIn;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
