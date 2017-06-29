using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	private float musicVolume;
	private float soundVolume;

	void Awake() {
		G.Sys.audioManager = this;
	}

	public void SetSoundVolume(float v) {
		soundVolume = v;
	}

	public void SetMusicVolume(float v) {
		musicVolume = v;
	}
}
