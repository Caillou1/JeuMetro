using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AudioManager : MonoBehaviour {
	[BoxGroup("Music")]
	public bool PlayTitle;
	[BoxGroup("Music")]
	[ShowIf("PlayTitle")]
	public AudioClip TitleMusic;
	[BoxGroup("Music")]
	[HideIf("PlayTitle")]
	public AudioClip MusicStart;
	[BoxGroup("Music")]
	[HideIf("PlayTitle")]
	public AudioClip MusicLoop;
	[BoxGroup("Music")]
	[Range(0f, 1f)]
	public float MusicVolume = 1f;

	[BoxGroup("BuyFood")]
	public AudioClip BuyFoodClip;
	[BoxGroup("BuyFood")]
	[Range(0f, 1f)]
	public float BuyFoodVolume = 1f;

	[BoxGroup("BuyTicket")]
	public AudioClip BuyTicketClip;
	[BoxGroup("BuyTicket")]
	[Range(0f, 1f)]
	public float BuyTicketVolume = 1f;

	[BoxGroup("Construct")]
	public AudioClip ConstructClip;
	[BoxGroup("Construct")]
	[Range(0f, 1f)]
	public float ConstructVolume = 1f;

	[BoxGroup("Faint")]
	public AudioClip FaintClip;
	[BoxGroup("Faint")]
	[Range(0f, 1f)]
	public float FaintVolume = 1f;

	[BoxGroup("StairsFall")]
	public AudioClip StairsFallClip;
	[BoxGroup("StairsFall")]
	[Range(0f, 1f)]
	public float StairsFallVolume = 1f;

	[BoxGroup("TrainStart")]
	public AudioClip TrainStartClip;
	[BoxGroup("TrainStart")]
	[Range(0f, 1f)]
	public float TrainStartVolume = 1f;

	[BoxGroup("TrainStop")]
	public AudioClip TrainStopClip;
	[BoxGroup("TrainStop")]
	[Range(0f, 1f)]
	public float TrainStopVolume = 1f;

	[BoxGroup("Trash")]
	public AudioClip TrashClip;
	[BoxGroup("Trash")]
	[Range(0f, 1f)]
	public float TrashVolume = 1f;

	private float musicVolume;
	private float soundVolume;

	private AudioSource source;

	void Awake() {
		G.Sys.audioManager = this;
		source = GetComponent<AudioSource> ();
		musicVolume = PlayerPrefs.GetFloat ("musicVolume", 1f);
		soundVolume = PlayerPrefs.GetFloat ("soundVolume", 1f);
		source.volume = musicVolume;

		if (PlayTitle) {
			source.clip = TitleMusic;
			source.loop = true;
			source.Play ();
		} else {
			source.clip = MusicStart;
			source.Play ();
			StartCoroutine (WaitForEndOfStart ());
		}
	}

	IEnumerator WaitForEndOfStart() {
		yield return new WaitUntil (() => {
			return source.time >= MusicStart.length-0.01f;
		});
		source.clip = MusicLoop;
		source.loop = true;
		source.Play ();
	}

	public void SetSoundVolume(float v) {
		soundVolume = v;
	}

	public void SetMusicVolume(float v) {
		musicVolume = v;
		source.volume = musicVolume;
	}

	public void PlayBuyFood() {
		source.PlayOneShot (BuyFoodClip, BuyFoodVolume * soundVolume / musicVolume);
	}

	public void PlayBuyTicket() {
		source.PlayOneShot (BuyTicketClip, BuyTicketVolume * soundVolume / musicVolume);
	}

	public void PlayConstruct() {
		source.PlayOneShot (ConstructClip, ConstructVolume * soundVolume / musicVolume);
	}

	public void PlayFaint() {
		source.PlayOneShot (FaintClip, FaintVolume * soundVolume / musicVolume);
	}

	public void PlayStairsFall() {
		source.PlayOneShot (StairsFallClip, StairsFallVolume * soundVolume / musicVolume);
	}

	public void PlayTrainStart() {
		source.PlayOneShot (TrainStartClip, TrainStartVolume * soundVolume / musicVolume);
	}

	public void PlayTrainStop() {
		source.PlayOneShot (TrainStopClip, TrainStopVolume * soundVolume / musicVolume);
	}

	public void PlayTrash() {
		source.PlayOneShot (TrashClip, TrashVolume * soundVolume / musicVolume);
	}

	void OnDestroy() {
		PlayerPrefs.SetFloat ("soundVolume", soundVolume);
		PlayerPrefs.SetFloat ("musicVolume", musicVolume);
	}
}
