﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using AmplitudeNS;

public enum Menu {
	Main,
	Parameters,
	Credits,
	Pause,
	Score,
	Game,
	SGP,
	Shop,
	LevelSelection,
	NONE,
}

[System.Serializable]
public class LevelUnlock {
	public int Level;
	public List<int> UnlockedLevels;
}

public class MenuManager : MonoBehaviour {
	public Sprite RedTimerCircle;
	public float FireAlertBlinkTime;
	public float FireAlertAlphaStart;
	public float FireAlertAlphaEnd;
	public Menu CurrentMenu = Menu.NONE;
	public float[] ZoomLevels;
	public LevelUnlock[] LevelUnlocks;

    public float FireAlertUiOffset = 100;
    public float FireAlertUIMoveTime = 1;
    public float FireAlertUIWaitTime = 2;

	private Vector3 cameraOrigin;

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
	private GameObject FadeUI;
	private GameObject LevelSelectionUI;
    //private List<GameObject> LevelButtons;
    private GameObject WinEndGameUI;
	private GameObject LoseEndGameUI;
	private GameObject MessageBox;
	private Text ObjectivesText;
	private Text MessageText;
	private Image FireAlertVignette;
    private GameObject FireAlertUI;

	private Image TimePie;
	private Text TimeTxt;
	private Text WaveNumber;
	private Text TravelerNumber;
	private Text Money;
	private Text MoneyAdded;

	private Transform tf;

	private GameObject[] ShopButtons;

	private Transform cameraTransform;

	private int ShopIndex = 0;

    private WinMenuDatas winMenuDatas;

    private SubscriberList subscriberList = new SubscriberList();

	void Awake() {
		/*Amplitude amp = Amplitude.Instance;
		amp.logging = true;
		amp.init ("c91e37a9a64907dcd158927243246732");

		amp.logEvent ("Coucou");*/

		subscriberList.Add(new Event<WinGameEvent>.Subscriber(onWinGameEvent));
		subscriberList.Add (new Event<StartFireAlertEvent>.Subscriber (OnFireAlert));
		subscriberList.Subscribe();

		Time.timeScale = 1f;
		G.Sys.menuManager = this;

		cameraTransform = G.Sys.MainCamera.transform;
		cameraOrigin = cameraTransform.position;

		tf = transform;
		FadeUI = tf.Find ("FadeUI").gameObject;
		MainUI = tf.Find ("MainUI").gameObject;
		ParametersUI = tf.Find ("OptionsUI").gameObject;
		CreditsUI = tf.Find ("CreditsUI").gameObject;
		PauseUI = tf.Find ("PauseUI").gameObject;
		ScoreUI = tf.Find ("ScoresUI").gameObject;
		GameUI = tf.Find ("GameUI").gameObject;
		FireAlertVignette = GameUI.transform.Find ("FireAlertVignette").GetComponent<Image> ();
		FireAlertVignette.color = new Color (1, 1, 1, 0);
		LevelSelectionUI = tf.Find ("LevelSelectionUI").gameObject;
        WinEndGameUI = tf.Find("WinEndGameUI").gameObject;
        winMenuDatas = new WinMenuDatas(WinEndGameUI);
		LoseEndGameUI = tf.Find ("LoseEndGameUI").gameObject;
		MessageBox = GameUI.transform.Find ("MessageBox").gameObject;
		MessageText = MessageBox.transform.Find ("Text").GetComponent<Text> ();
		ObjectivesText = GameUI.transform.Find ("ObjectivesText").GetComponent<Text> ();
        FireAlertUI = tf.Find("FireAlertUI").gameObject;

		//LevelButtons = new List<GameObject> ();
		for (int i = 0; i < LevelSelectionUI.transform.childCount; i++) {
			var c = LevelSelectionUI.transform.GetChild (i);
			if (c.name.ToLower ().Contains ("level")) {
				if (c.name.Remove (0, 5) != "1") {
					bool isUnlocked = PlayerPrefs.GetInt (c.name + "Unlocked", 0) == 1;
					c.GetComponent<Button> ().interactable = isUnlocked;
					if(!isUnlocked)
						c.Find ("Text").GetComponent<Image> ().color = new Color (100f / 255f, 100f / 255f, 100f / 255f);
					c.Find ("Lock").GetComponent<Image> ().enabled = !isUnlocked;
				}
			}
		}

		var menuTf = GameUI.transform.Find ("Menu");
		ShopUI = menuTf.Find ("ShopUI").gameObject;

		TimePie = menuTf.Find ("Time").Find ("Pie").Find ("Wedge").GetComponent<Image> ();
		TimeTxt = menuTf.Find ("Time").Find ("Text").GetComponent<Text> ();
		WaveNumber = menuTf.Find ("Middle").Find ("Wave").Find ("Text").GetComponent<Text> ();
		TravelerNumber = menuTf.Find ("Middle").Find ("Travelers").Find ("Text").GetComponent<Text> ();
		Money = menuTf.Find ("Middle").Find ("Money").Find ("Text").GetComponent<Text> ();
		MoneyAdded = menuTf.Find ("Middle").Find("Money").Find ("MoneyAdded").GetComponent<Text> ();

		SGPUI = tf.Find ("SGPUI").gameObject;

		ParametersUI.transform.Find ("FullscreenToggle").GetComponent<Toggle> ().isOn = Screen.fullScreen;

		CurrentZoomLevel = 0;

		FadeUI.SetActive (false);
		LevelSelectionUI.SetActive (false);
		MainUI.SetActive (false);
		ParametersUI.SetActive (false);
		CreditsUI.SetActive (false);
		PauseUI.SetActive (false);
		ScoreUI.SetActive (false);
		GameUI.SetActive (false);
		ShopUI.SetActive (false);
		SGPUI.SetActive (false);
        WinEndGameUI.SetActive(false);
		LoseEndGameUI.SetActive (false);
		MessageBox.SetActive (false);
        FireAlertUI.SetActive(false);
		var obj = GetCorrespondantUI (CurrentMenu);
		if (obj != null)
			obj.SetActive (true);

		ShopButtons = new GameObject[10];
		ShopButtons [0] = ShopUI.transform.Find ("Infos").gameObject;
		ShopButtons [1] = ShopUI.transform.Find ("Podotactile").gameObject;
		ShopButtons [2] = ShopUI.transform.Find ("Speaker").gameObject;
		ShopButtons [3] = ShopUI.transform.Find ("FoodDistrib").gameObject;
		ShopButtons [4] = ShopUI.transform.Find ("Bench").gameObject;

		ShopButtons [5] = ShopUI.transform.Find ("Escalator").gameObject;
		ShopButtons [6] = ShopUI.transform.Find ("TicketDistrib").gameObject;
		ShopButtons [7] = ShopUI.transform.Find ("Bin").gameObject;
		ShopButtons [8] = ShopUI.transform.Find ("Cleaner").gameObject;
		ShopButtons [9] = ShopUI.transform.Find ("Agent").gameObject;

		UpdateShopUI ();
	}

	void Start() {
		

		ParametersUI.transform.Find("MusicSlider").GetComponent<Slider>().value = PlayerPrefs.GetFloat ("musicVolume", 1f);
		ParametersUI.transform.Find("SoundSlider").GetComponent<Slider>().value = PlayerPrefs.GetFloat ("soundVolume", 1f);

		for (int i = 0; i < ZoomLevels.Length-1; i++) {
			Zoom ();
		}
	}

	public void LoadScene(string sceneName) {
		SceneManager.LoadScene (sceneName);
	}

	public void Lose() {
		Time.timeScale = 0;
		FadeUI.SetActive (true);
		LoseEndGameUI.SetActive (true);
	}

    private void OnDestroy()
    {
		subscriberList.Unsubscribe();
    }

	void OnFireAlert(StartFireAlertEvent fireAlertEvent) {
		TimePie.sprite = RedTimerCircle;
		DOVirtual.Float (0f, FireAlertAlphaStart, .5f, (float x) => {
			FireAlertVignette.color = new Color (1, 1, 1, x);
		}).OnComplete (() => BlinkVignette ());
        FireAlertUI.SetActive(true);
        FireAlertUI.transform.DOLocalMoveY(FireAlertUI.transform.localPosition.y - FireAlertUiOffset, FireAlertUIMoveTime)
                   .OnComplete(() => DOVirtual.DelayedCall(FireAlertUIWaitTime, () => FireAlertUI.transform.DOLocalMoveY(FireAlertUI.transform.localPosition.y + FireAlertUiOffset, FireAlertUIMoveTime)
                                                           .OnComplete(() =>  FireAlertUI.SetActive(false))));
	}

	void BlinkVignette() {
		DOVirtual.Float (FireAlertAlphaStart, FireAlertAlphaEnd, FireAlertBlinkTime / 2f, (float x) => {
			FireAlertVignette.color = new Color (1, 1, 1, x);
		}).OnComplete(() => {
			DOVirtual.Float (FireAlertAlphaEnd, FireAlertAlphaStart, FireAlertBlinkTime / 2f, (float x) => {
				FireAlertVignette.color = new Color (1, 1, 1, x);
			}).OnComplete(() => BlinkVignette());
		});
	}

	private bool StopBlink;
	public void StartBlinkMoney() {
		Money.color = Color.yellow;
		DOVirtual.DelayedCall (.5f, () => {
			Money.color = Color.white;
			if(StopBlink)
				StopBlink = false;
			else
				DOVirtual.DelayedCall(.5f, () => StartBlinkMoney());
		});
	}

	public void StopBlinkMoney() {
		StopBlink = true;
	}

	private List<string> Messages;
	private int currentMessage;

	public void ShowMessages(List<string> messages) {
		if (messages.Count > 0) {
			Messages = messages;

			currentMessage = 0;

			MessageText.text = Messages [0];
			MessageBox.SetActive (true);

			Time.timeScale = 0f;
		}
	}

	public void ShowObjectives(List<Objectif> objectives) {
		ObjectivesText.text = "";
		foreach (var o in objectives) {
			ObjectivesText.text += o.ToString () + "\n";
		}
	}

	public void HideObjectives() {
		ObjectivesText.text = "";
	}

	public void NextMessage() {
		if (currentMessage >= Messages.Count - 1) {
			MessageBox.SetActive (false);
			currentMessage++;
			Time.timeScale = 1f;
		} else {
			currentMessage++;

			MessageText.text = Messages [currentMessage];
		}
	}

	public bool AreAllMessagesRead() {
		return currentMessage >= Messages.Count;
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
		case Menu.LevelSelection:
			return LevelSelectionUI;
		default:
			return null;
		}
	}

	public void SetWaveNumber(int wave, int maxWave) {
		if(WaveNumber != null)
			WaveNumber.text = wave + "/" + maxWave;
	}

	public void SetTravelerNumber(int traveler, int maxTraveler) {
		if(TravelerNumber != null)
			TravelerNumber.text = traveler + "/" + maxTraveler;
	}

	public void SetMoneyNumber(int money) {
		if(Money != null)
			Money.text = money + "";
	}

	public void SetPieTime(float timePercentage, int secondTime) {
		TimePie.fillAmount = timePercentage;
		TimeTxt.text = IntToString (secondTime);
	}

    public void EnableUITutoMode()
    {
        TimeTxt.text = "∞";
        TimeTxt.fontSize = 150;
        Money.transform.parent.gameObject.SetActive(false);
        TravelerNumber.transform.parent.gameObject.SetActive(false);
        WaveNumber.transform.parent.gameObject.SetActive(false);
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

	public void ShowMoneyAdded(int value) {
		string str = "";
		if (value > 0)
			str += "+";
		str += value;

		MoneyAdded.text = str;

		DOVirtual.Float (1f, 0f, 2f, (float x) => {
			MoneyAdded.color = new Color(MoneyAdded.color.r, MoneyAdded.color.g, MoneyAdded.color.b, x);
		}).OnComplete(() => {
			MoneyAdded.text = "";
		});
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
		cameraTransform.position = cameraOrigin - cameraTransform.forward * ZoomLevels[CurrentZoomLevel];
	}

	public void Zoom(float ZoomPower) {
		bool done = false;
		int iter = 0;
		while (!done && iter < 3) {
			Vector3 pos = cameraTransform.position + cameraTransform.forward * ZoomPower;
			if (pos.y >= cameraOrigin.y && pos.y <= (cameraOrigin - cameraTransform.forward * ZoomLevels [ZoomLevels.Length - 1]).y) {
				cameraTransform.position = pos;
				done = true;
			}

			ZoomPower /= 2;
			iter++;
		}
	}

	public float GetZoomRatio() {
		return (cameraTransform.position.y - cameraOrigin.y) / (cameraOrigin - cameraTransform.forward * ZoomLevels [ZoomLevels.Length - 1]).y + .1f;
	}

	public void LevelSelection() {
		G.Sys.audioManager.PlayAccept ();
		FadeUI.SetActive (true);
		LevelSelectionUI.SetActive (true);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.LevelSelection;
	}

	public void Play(int i) {
        G.Sys.levelIndex = i;
		if (i == 0) {
			LevelSelection ();
		} else {
			G.Sys.audioManager.PlayLevelSelected ();
			G.Sys.tilemap.clear ();
			SceneManager.LoadScene ("Level" + i);
		}
	}

	public void MainMenu() {
		G.Sys.tilemap.clear ();
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

	public void DisableShopUI() {
		ShopUI.SetActive (false);
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
		FadeUI.SetActive (true);
		PauseUI.SetActive (true);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Pause;
		Time.timeScale = 0f;
	}

	public void Resume() {
		FadeUI.SetActive (false);
		PauseUI.SetActive (false);
		LastMenu = CurrentMenu;
		CurrentMenu = Menu.Game;
		Time.timeScale = 1f;
	}

	public void Replay() {
		Time.timeScale = 1f;
		G.Sys.tilemap.clear ();
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void Back() {
		if (CurrentMenu == Menu.LevelSelection) {
			FadeUI.SetActive (false);
		}
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

    void onWinGameEvent(WinGameEvent e)
    {
        FadeUI.SetActive(true);
        WinEndGameUI.SetActive(true);
        winMenuDatas.set(e.score);
        Time.timeScale = 0;
    }

    public void OnWinInfosClick()
    {
        winMenuDatas.toggleBubble();
    }

    public void OnWinRetryClick()
    {
        Time.timeScale = 1;
        Play(G.Sys.levelIndex);
    }

    public void OnWinHomeClick()
    {

        Time.timeScale = 1;
        MainMenu();
    }

    public void OnWinNextClick()
    {
        Play(G.Sys.levelIndex + 1);
    }

    class WinMenuDatas
    {
        public WinMenuDatas(GameObject parent)
        {
            medalTime = parent.transform.Find("GoldTime").gameObject;
            medalMoney = parent.transform.Find("GoldMoney").gameObject;
            medalSurface = parent.transform.Find("GoldSurface").gameObject;

            bubble = parent.transform.Find("Bubble").gameObject;
            bubble.SetActive(false);
            var timeObj = bubble.transform.Find("Time");
            var moneyObj = bubble.transform.Find("Money");
            var surfaceObj = bubble.transform.Find("Surface");

            currentTime = timeObj.Find("Value").GetComponent<Text>();
            targetTime = timeObj.Find("Target").GetComponent<Text>();

			currentMoney = moneyObj.Find("Value").GetComponent<Text>();
			targetMoney = moneyObj.Find("Target").GetComponent<Text>();

			currentSurface = surfaceObj.Find("Value").GetComponent<Text>();
			targetSurface = surfaceObj.Find("Target").GetComponent<Text>();

            score = parent.transform.Find("Score").GetComponent<Text>();
            bestScore = parent.transform.Find("Best").GetComponent<Text>();

			fireAlert = bubble.transform.Find("FireAlert").Find("Value").GetComponent<Text>();
        }

        public void set(Score s)
        {
            medalTime.SetActive(s.HaveTimeMedal);
            medalMoney.SetActive(s.HaveMoneyMedal);
            medalSurface.SetActive(s.HaveSurfaceMedal);

            currentTime.text = s.AverageTime.ToString("F");
            currentMoney.text = s.MoneyLeft.ToString();
            currentSurface.text = s.SpaceUsed.ToString();

            targetTime.text = s.GoldAverageTime.ToString("F");
            targetMoney.text = s.GoldMoneyLeft.ToString();
            targetSurface.text = s.GoldSurface.ToString();

            score.text = s.ScoreValue.ToString();
            int best = PlayerPrefs.GetInt("Level" + G.Sys.levelIndex, 0);
            if(best < s.ScoreValue)
            {
                best = s.ScoreValue;
                PlayerPrefs.SetInt("Level" + G.Sys.levelIndex, best);
                PlayerPrefs.Save();
            }
            bestScore.text = best.ToString();

			fireAlert.text = (s.WonFireAlert) ? "Réussie" : "Échec";
        }

        public void toggleBubble()
        {
            bubble.SetActive(!bubble.activeSelf);
        }

        GameObject medalTime;
        GameObject medalMoney;
        GameObject medalSurface;

        Text currentTime;
        Text targetTime;
        Text currentMoney;
        Text targetMoney;
        Text currentSurface;
        Text targetSurface;
		Text fireAlert;

        Text score;
        Text bestScore;

        GameObject bubble;
    }
}
