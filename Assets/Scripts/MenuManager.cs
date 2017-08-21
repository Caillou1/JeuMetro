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
	LevelSelection,
	NONE,
}

[System.Serializable]
public class LevelUnlock {
	public int Level;
	public List<int> UnlockedLevels;
}

public class MenuManager : MonoBehaviour {
	public Menu CurrentMenu = Menu.NONE;
	public float[] ZoomLevels;
	public LevelUnlock[] LevelUnlocks;
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
	private GameObject PersonnelUI;
	private GameObject SGPUI;
	private GameObject FadeUI;
	private GameObject LevelSelectionUI;
    //private List<GameObject> LevelButtons;
    private GameObject WinEndGameUI;

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

    private SubscriberList substriberList = new SubscriberList();

	void Awake() {
        substriberList.Add(new Event<WinGameEvent>.Subscriber(onWinGameEvent));
        substriberList.Subscribe();

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
		LevelSelectionUI = tf.Find ("LevelSelectionUI").gameObject;
        WinEndGameUI = tf.Find("WinEndGameUI").gameObject;
        winMenuDatas = new WinMenuDatas(WinEndGameUI);

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
		PersonnelUI = menuTf.Find ("PersonnelUI").gameObject;

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
		PersonnelUI.SetActive (false);
		SGPUI.SetActive (false);
        WinEndGameUI.SetActive(false);
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

	void Start() {
		ParametersUI.transform.Find("MusicSlider").GetComponent<Slider>().value = PlayerPrefs.GetFloat ("musicVolume", 1f);
		ParametersUI.transform.Find("SoundSlider").GetComponent<Slider>().value = PlayerPrefs.GetFloat ("soundVolume", 1f);

		for (int i = 0; i < ZoomLevels.Length-1; i++) {
			Zoom ();
		}
	}

    private void OnDestroy()
    {
        substriberList.Unsubscribe();
    }

	public void ShowMessages(List<string> messages) {

	}

	public void ShowObjectives(List<Objectif> objectives) {

	}

	public bool AreAllMessagesRead() {
		return true;
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

	public void SetTravelerNumber(int traveler) {
		if(TravelerNumber != null)
			TravelerNumber.text = traveler + "";
	}

	public void SetMoneyNumber(int money) {
		if(Money != null)
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
			SceneManager.LoadScene ("Level" + i);
		}
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

	public void TogglePersonnelUI() {
		PersonnelUI.SetActive (!PersonnelUI.activeInHierarchy);
	}

	public void DisableShopUI() {
		ShopUI.SetActive (false);
	}

	public void DisablePersonnelUI() {
		PersonnelUI.SetActive (false);
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

        Text score;
        Text bestScore;

        GameObject bubble;
    }
}
