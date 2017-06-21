using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Menu {
	Main,
	Parameters,
	Credits,
	Pause,
	Score,
	Game,
	SGP,
	NONE
}

public class MenuManager : MonoBehaviour {
	public Menu CurrentMenu = Menu.NONE;

	private Menu LastMenu = Menu.NONE;

	private GameObject MainUI;
	private GameObject ParametersUI;
	private GameObject CreditsUI;
	private GameObject PauseUI;
	private GameObject ScoreUI;
	private GameObject GameUI;
	private GameObject SGPUI;
	private GameObject BlackScreen;

	private Transform tf;

	void Start() {
		tf = transform;
		MainUI = tf.Find ("MainUI").gameObject;
		ParametersUI = tf.Find ("OptionsUI").gameObject;
		CreditsUI = tf.Find ("CreditsUI").gameObject;
		PauseUI = tf.Find ("PauseUI").gameObject;
		ScoreUI = tf.Find ("ScoresUI").gameObject;
		GameUI = tf.Find ("GameUI").gameObject;
		SGPUI = tf.Find ("SGPUI").gameObject;
		BlackScreen = tf.Find ("BlackScreen").gameObject;

		MainUI.SetActive (false);
		ParametersUI.SetActive (false);
		CreditsUI.SetActive (false);
		PauseUI.SetActive (false);
		ScoreUI.SetActive (false);
		GameUI.SetActive (false);
		SGPUI.SetActive (false);
		var obj = GetCorrespondantUI (CurrentMenu);
		if(obj != null)
			obj.SetActive (true);
		BlackScreen.SetActive (false);
	}

	GameObject GetCorrespondantUI(Menu menu) {
		switch (menu) {
		case Menu.Main:
			return MainUI;
		case Menu.Parameters:
			return ParametersUI;
		case Menu.Credits:
			return CreditsUI;
		case Menu.Pause:
			return PauseUI;
		case Menu.Score:
			return ScoreUI;
		case Menu.Game:
			return GameUI;
		case Menu.SGP:
			return SGPUI;
		default:
			return null;
		}
	}

	public void Play() {
		Debug.Log ("Jeu");
	}

	public void Score() {
		Debug.Log ("Score");
		ScoreUI.SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Score;
	}

	public void Options() {
		Debug.Log ("Options");
		ParametersUI.SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Parameters;
	}

	public void Credits() {
		Debug.Log ("Credits");
		CreditsUI.SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Credits;
	}

	public void Quit() {
		Debug.Log ("Quit");
		Application.Quit ();
	}

	public void SGP() {
		Debug.Log ("SGP");
		SGPUI.SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.SGP;
	}

	public void Pause () {
		Debug.Log ("Pause");
		PauseUI.SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Pause;
		Time.timeScale = 0f;
	}

	public void Resume() {
		Debug.Log ("Resume");
		PauseUI.SetActive (false);
		GetCorrespondantUI(LastMenu).SetActive (true);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Game;
		Time.timeScale = 1f;
	}

	public void Replay() {
		Debug.Log ("Replay");
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void Back() {
		Debug.Log ("Back");
		var tmp = LastMenu;
		GetCorrespondantUI (LastMenu).SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = tmp;
	}

	public void SetSoundVolume(float v) {
		Debug.Log ("Sound volume set to : " + v);
	}

	public void SetMusicVolume(float v) {
		Debug.Log ("Music volume set to : " + v);
	}

	public void SetFullscreen(bool b) {
		Screen.fullScreen = b;
		Debug.Log ("Fullscreen : " + b);
	}
}
