using UnityEngine;
using System.Collections;

[System.Serializable]
public class AudioFiles
{
	public string mFileName = "";
	public AudioClip mAudioClip = null;
	private AudioSource mAudioSource = null;
	
	public void setAudioSource()
	{		
		GameObject audioObj = new GameObject();
		audioObj.name = mFileName;
		audioObj.transform.parent = PlayAudioSounds.sharedHandler().transform;
		audioObj.transform.position = PlayAudioSounds.sharedHandler().transform.position;
		mAudioSource = audioObj.AddComponent<AudioSource>();
		mAudioSource.clip = mAudioClip;
		mAudioSource.playOnAwake = false;
	}
	
	public void playSound()
	{
		if(mAudioSource != null) 
			mAudioSource.Play();
	}	
	
	public void stopSound()
	{
		if(mAudioSource != null) 
			mAudioSource.Stop();
	}	
}

public class PlayAudioSounds : MonoBehaviour {
	
	private static PlayAudioSounds mSharedHandler = null;
	public static PlayAudioSounds sharedHandler()
	{ 
		if(mSharedHandler == null)
		{
			GameObject PlayAudioObject = (GameObject)Instantiate(Resources.Load("Audio/PlayAudioSoundsPrefab") as GameObject);
			PlayAudioObject.name = "PlayAudioSounds";			
			PlayAudioObject.transform.position = Camera.main.transform.position + new Vector3(0,0,1);
		}
		return mSharedHandler; 
	}	
	
	public AudioFiles[] arrAudioFiles = null;
	public AudioFiles[] arrBackgroundFiles = null;
	public string mSoundEnableKey = "IsSoundEnable";
	public string mMusicEnableKey = "IsMusicEnable";
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
		mSharedHandler = this;
		if(arrAudioFiles != null)
		foreach(AudioFiles audioFile in arrAudioFiles)
			audioFile.setAudioSource();		
	}	
	
	public void playSound(string soundName)
	{
		if(PlayerPrefs.GetInt(mSoundEnableKey)==0) return;
		foreach(AudioFiles audioFile in arrAudioFiles)
			if(audioFile.mFileName == soundName) 
				audioFile.playSound();
	}
	
	public void stopSound(string soundName)
	{
		foreach(AudioFiles audioFile in arrAudioFiles)
			if(audioFile.mFileName == soundName) 
				audioFile.stopSound();
	}
	
	public void playBgMusic(string musicName)
	{
		if(GetComponent<AudioSource>() == null)
			gameObject.AddComponent<AudioSource>();
		
		if(arrBackgroundFiles == null) return;
		foreach(AudioFiles audioFile in arrBackgroundFiles)
		{
			if(audioFile.mFileName == musicName)
			{
				AudioSource audioSource = null;
				audioSource = GetComponent<AudioSource>();
				audioSource.loop = true;
				audioSource.playOnAwake = false;
				audioSource.clip = audioFile.mAudioClip;
			}
		}		
		toggleBgMusic();		
	}
	
	public void toggleBgMusic()
	{
		if(GetComponent<AudioSource>() == null	|| 
			GetComponent<AudioSource>().clip == null) return;
		if(PlayerPrefs.GetInt(mMusicEnableKey)==0)
			GetComponent<AudioSource>().Stop();
		else
			GetComponent<AudioSource>().Play();	
	}
}
