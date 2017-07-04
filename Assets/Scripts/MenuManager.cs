using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public enum Menu {
	Main,
	Parameters,
	Credits,
	Pause,
	Score,
	Game,
	SGP,
	Shop,
	NONE
}

public class MenuManager : MonoBehaviour {
	public Menu CurrentMenu = Menu.NONE;

	public float ZoomPower = 10f;
	public float[] ZoomLevels;

	private int CurrentZoomLevel;

	private Menu LastMenu = Menu.NONE;

	private GameObject MainUI;
	private GameObject ParametersUI;
	private GameObject CreditsUI;
	private GameObject PauseUI;
	private GameObject ScoreUI;
	private GameObject GameUI;
	private GameObject ShopUI;
	private GameObject SGPUI;

	private Image TimePie;
	private Text TimeTxt;
	private Text WaveNumber;
	private Text TravelerNumber;
	private Text Money;

	private Transform tf;

	private GameObject[] ShopButtons;

	private int ShopIndex = 0;

	void Awake() {
		G.Sys.menuManager = this;

		tf = transform;
		MainUI = tf.Find ("MainUI").gameObject;
		ParametersUI = tf.Find ("OptionsUI").gameObject;
		CreditsUI = tf.Find ("CreditsUI").gameObject;
		PauseUI = tf.Find ("PauseUI").gameObject;
		ScoreUI = tf.Find ("ScoresUI").gameObject;
		GameUI = tf.Find ("GameUI").gameObject;

		var menuTf = GameUI.transform.Find ("Menu");
		ShopUI = menuTf.Find ("ShopUI").gameObject;

		TimePie = menuTf.Find ("Time").Find ("Pie").Find ("Wedge").GetComponent<Image> ();
		TimeTxt = menuTf.Find ("Time").Find ("Text").GetComponent<Text> ();
		WaveNumber = menuTf.Find ("Middle").Find ("Wave").Find ("Text").GetComponent<Text> ();
		TravelerNumber = menuTf.Find ("Middle").Find ("Travelers").Find ("Text").GetComponent<Text> ();
		Money = menuTf.Find ("Middle").Find ("Money").Find ("Text").GetComponent<Text> ();

		SGPUI = tf.Find ("SGPUI").gameObject;

		ParametersUI.transform.Find ("FullscreenToggle").GetComponent<Toggle> ().isOn = Screen.fullScreen;

		CurrentZoomLevel = 0;
		Camera.main.fieldOfView = ZoomLevels [0];

		MainUI.SetActive (false);
		ParametersUI.SetActive (false);
		CreditsUI.SetActive (false);
		PauseUI.SetActive (false);
		ScoreUI.SetActive (false);
		GameUI.SetActive (false);
		ShopUI.SetActive (false);
		SGPUI.SetActive (false);
		var obj = GetCorrespondantUI (CurrentMenu);
		if (obj != null)
			obj.SetActive (true);

		ShopButtons = new GameObject[8];
		ShopButtons [0] = ShopUI.transform.Find ("Escalator").gameObject;
		ShopButtons [1] = ShopUI.transform.Find ("Bench").gameObject;
		ShopButtons [2] = ShopUI.transform.Find ("TicketDistrib").gameObject;
		ShopButtons [3] = ShopUI.transform.Find ("FoodDistrib").gameObject;
		ShopButtons [4] = ShopUI.transform.Find ("Bin").gameObject;
		ShopButtons [5] = ShopUI.transform.Find ("Infos").gameObject;
		ShopButtons [6] = ShopUI.transform.Find ("Speaker").gameObject;
		ShopButtons [7] = ShopUI.transform.Find ("Podotactile").gameObject;

		UpdateShopUI ();
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
		case Menu.Shop:
			return ShopUI;
		default:
			return null;
		}
	}

	public void SetWaveNumber(int wave, int maxWave) {
		WaveNumber.text = wave + "/" + maxWave;
	}

	public void SetTravelerNumber(int traveler) {
		TravelerNumber.text = traveler + "";
	}

	public void SetMoneyNumber(int money) {
		Money.text = money + "";
	}

	public void SetPieTime(float timePercentage, int secondTime) {
		TimePie.fillAmount = timePercentage;
		TimeTxt.text = IntToString (secondTime);
	}

	private string IntToString(int seconds) {
		string time = "";

		int minutes = Mathf.FloorToInt (seconds / 60);
		int secondes = seconds % 60;

		if (minutes < 10)
			time += "0";
		time += minutes.ToString () + ":";
		if (secondes < 10)
			time += "0";
		time += secondes.ToString ();

		if (seconds < 0)
			time = "00:00";

		return time;
	}

	private void UpdateShopUI() {
		foreach (var b in ShopButtons)
			b.SetActive (false);

		for (int i = ShopIndex * 5; i < Mathf.Min(ShopIndex * 5 + 5, ShopButtons.Length); i++) {
			ShopButtons [i].SetActive (true);
		}
	}

	public void ShopLeft() {
		ShopIndex = (ShopIndex + Mathf.CeilToInt (ShopButtons.Length / 5f) - 1) % Mathf.CeilToInt (ShopButtons.Length / 5f);
		UpdateShopUI ();
	}

	public void ShopRight() {
		ShopIndex = (ShopIndex + 1) % Mathf.CeilToInt (ShopButtons.Length / 5f);
		UpdateShopUI ();
	}

	public void Zoom() {
		CurrentZoomLevel = (CurrentZoomLevel + 1) % ZoomLevels.Length;
		DOVirtual.Float (Camera.main.fieldOfView, ZoomLevels [CurrentZoomLevel], .3f, (float f) => Camera.main.fieldOfView = f);
	}

	public void ZoomIn() {
		DOVirtual.Float (Camera.main.fieldOfView, Mathf.Max(ZoomLevels[0], Camera.main.fieldOfView - ZoomPower), .15f, (float f) => Camera.main.fieldOfView = f);
	}

	public void ZoomOut() {
		DOVirtual.Float (Camera.main.fieldOfView, Mathf.Min(ZoomLevels[ZoomLevels.Length-1], Camera.main.fieldOfView + ZoomPower), .15f, (float f) => Camera.main.fieldOfView = f);
	}

	public void Play() {
		Debug.Log ("Play");
	}

	public void MainMenu() {
		SceneManager.LoadScene("MainMenu");
	}

	public void Score() {
		ScoreUI.SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Score;
	}

	public void Options() {
		ParametersUI.SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Parameters;
	}

	public void Credits() {
		CreditsUI.SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Credits;
	}

	public void ToggleShopUI() {
		ShopUI.SetActive (!ShopUI.activeInHierarchy);
	}

	public void Quit() {
		Application.Quit ();
	}

	public void SGP() {
		SGPUI.SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.SGP;
	}

	public void Pause () {
		PauseUI.SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Pause;
		Time.timeScale = 0f;
	}

	public void Resume() {
		PauseUI.SetActive (false);
		GetCorrespondantUI(LastMenu).SetActive (true);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Game;
		Time.timeScale = 1f;
	}

	public void Replay() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void Back() {
		var tmp = LastMenu;
		GetCorrespondantUI (LastMenu).SetActive (true);
		GetCorrespondantUI (CurrentMenu).SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = tmp;
	}

	public void SetSoundVolume(float v) {
		G.Sys.audioManager.SetSoundVolume (v);
	}

	public void SetMusicVolume(float v) {
		G.Sys.audioManager.SetMusicVolume (v);
	}

	public void SetFullscreen(bool b) {
		Screen.fullScreen = b;
	}
}
