using UnityEngine;
using System.Collections;

public class FpsScr : MonoBehaviour {

	// Use this for initialization
	public float _updateInterval = 0.5F;
    private double mdLastInterval;
    private int miFrames = 0;
	private float mfFps;
	
	void Start () 
	{
		mdLastInterval = Time.realtimeSinceStartup;
		QualitySettings.vSyncCount = 0;
        miFrames = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		++miFrames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > mdLastInterval + _updateInterval) 
		{
            mfFps = (float)(miFrames / (timeNow - mdLastInterval));
            miFrames = 0;
            mdLastInterval = timeNow;
		}		
	}
	
	void OnGUI()
	{
		GUILayout.Label("" + mfFps.ToString("f2"));
	}
}
