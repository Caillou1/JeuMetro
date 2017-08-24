using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
	[BoxGroup("Music")]
	public AudioClip TitleMusic;
	[BoxGroup("Music")]
	public AudioClip MusicStart;
	[BoxGroup("Music")]
	public AudioClip MusicLoop;
	[BoxGroup("Music")]
	public AudioClip FireMusicStart;
	[BoxGroup("Music")]
	public AudioClip FireMusicLoop;
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

	[BoxGroup("LevelSelected")]
	public AudioClip LevelSelectedClip;
	[BoxGroup("LevelSelected")]
	[Range(0f, 1f)]
	public float LevelSelectedVolume = 1f;

	[BoxGroup("Accept")]
	public AudioClip AcceptClip;
	[BoxGroup("Accept")]
	[Range(0f, 1f)]
	public float AcceptVolume = 1f;

	[BoxGroup("Cancel")]
	public AudioClip CancelClip;
	[BoxGroup("Cancel")]
	[Range(0f, 1f)]
	public float CancelVolume = 1f;

	private float musicVolume;
	private float soundVolume;

	private AudioSource soundSource;
	private AudioSource musicSource;

	void Awake() {
		soundSource = GetComponent<AudioSource> ();
		musicSource = transform.GetChild (0).GetComponent<AudioSource> ();
		musicVolume = PlayerPrefs.GetFloat ("musicVolume", 1f);
		soundVolume = PlayerPrefs.GetFloat ("soundVolume", 1f);
		musicSource.volume = musicVolume;
		soundSource.volume = soundVolume;

		bool playTitle = false;
		if (SceneManager.GetActiveScene ().name.ToLower () == "mainmenu")
			playTitle = true;

		if (playTitle) {
			musicSource.clip = TitleMusic;
			musicSource.loop = true;
			musicSource.Play ();
		} else {
			musicSource.clip = MusicStart;
			musicSource.Play ();
			StartCoroutine (WaitForEndOfStart (MusicLoop));
		}

		if (G.Sys.audioManager == null) {
			G.Sys.audioManager = this;
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}

	public void StartFireMusic() {
		musicSource.Stop ();
		musicSource.clip = FireMusicStart;
		musicSource.loop = false;
		musicSource.Play ();
		StartCoroutine (WaitForEndOfStart (FireMusicLoop));
	}

	IEnumerator WaitForEndOfStart(AudioClip loop) {
		yield return new WaitUntil (() => {
			return musicSource.time >= musicSource.clip.length-0.01f;
		});
		musicSource.clip = loop;
		musicSource.loop = true;
		musicSource.Play ();
	}

	public void SetSoundVolume(float v) {
		soundVolume = v;
		soundSource.volume = soundVolume;
	}

	public void SetMusicVolume(float v) {
		musicVolume = v;
		musicSource.volume = musicVolume;
	}

	public void PlayBuyFood() {
		soundSource.PlayOneShot (BuyFoodClip, BuyFoodVolume);
	}

	public void PlayBuyTicket() {
		soundSource.PlayOneShot (BuyTicketClip, BuyTicketVolume);
	}

	public void PlayConstruct() {
		soundSource.PlayOneShot (ConstructClip, ConstructVolume);
	}

	public void PlayFaint() {
		soundSource.PlayOneShot (FaintClip, FaintVolume);
	}

	public void PlayStairsFall() {
		soundSource.PlayOneShot (StairsFallClip, StairsFallVolume);
	}

	public void PlayTrainStart() {
		soundSource.PlayOneShot (TrainStartClip, TrainStartVolume);
	}

	public void PlayTrainStop() {
		soundSource.PlayOneShot (TrainStopClip, TrainStopVolume);
	}

	public void PlayTrash() {
		soundSource.PlayOneShot (TrashClip, TrashVolume);
	}

	public void PlayAccept() {
		soundSource.PlayOneShot (AcceptClip, AcceptVolume);
	}

	public void PlayCancel() {
		soundSource.PlayOneShot (CancelClip, CancelVolume);
	}

	public void PlayLevelSelected() {
		soundSource.PlayOneShot (LevelSelectedClip, LevelSelectedVolume);
	}

	void OnDestroy() {
		PlayerPrefs.SetFloat ("soundVolume", soundVolume);
		PlayerPrefs.SetFloat ("musicVolume", musicVolume);
	}
}
