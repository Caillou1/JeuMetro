using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	private float musicVolume;
	private float soundVolume;

	void Awake() {
		G.Sys.audioManager = this;
		musicVolume = PlayerPrefs.GetFloat ("musicVolume", 1f);
		soundVolume = PlayerPrefs.GetFloat ("soundVolume", 1f);
	}

	public void SetSoundVolume(float v) {
		soundVolume = v;
	}

	public void SetMusicVolume(float v) {
		musicVolume = v;
	}

	void OnDestroy() {
		PlayerPrefs.SetFloat ("soundVolume", soundVolume);
		PlayerPrefs.SetFloat ("musicVolume", musicVolume);
	}
}
