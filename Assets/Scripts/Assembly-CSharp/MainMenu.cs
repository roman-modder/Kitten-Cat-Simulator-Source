using System;
using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	public UISlider sliderProgress;

	public UILabel lblLoading;

	public UITexture background;

	public Material[] catTextures;

	public GameObject catMesh;

	public UIButton buttonNext;

	public UIButton buttonPrev;

	public UIPanel panelMenu;

	public UIPanel panelSurvival;

	public UIPanel panelQuit;

	public UILabel lblTglMusic;

	public GameObject follower;

	public UIButton btnRate;

	private float loadValue;

	private int curTexture;

	private int curSkin;

	public Material[] skins;

	private void Start()
	{
		HandleRateButton();
		sliderProgress.gameObject.SetActive(false);
		lblLoading.gameObject.SetActive(false);
		background.gameObject.SetActive(false);
		if (PlayerPrefs.GetInt("startAd") == 0)
		{
			PlayerPrefs.SetInt("startAd", 1);
		}
		Time.timeScale = 1f;
		curSkin = PlayerPrefs.GetInt("oskin");
		follower.GetComponent<Renderer>().material = skins[curSkin];
		curTexture = PlayerPrefs.GetInt("curText");
		catMesh.GetComponent<Renderer>().material = catTextures[curTexture];
		PhotonNetwork.offlineMode = true;
		SetLblMusic();
	}

	private void HandleRateButton()
	{
		PlayerPrefs.SetInt("app_startup", PlayerPrefs.GetInt("app_startup") + 1);
		if (!PlayerPrefs.HasKey("app_first_start"))
		{
			PlayerPrefs.SetString("app_first_start", DateTime.Now.Date.ToString());
		}
		DateTime dateTime = DateTime.Parse(PlayerPrefs.GetString("app_first_start"));
		double num = (DateTime.Now - dateTime).Days;
		if (PlayerPrefs.GetInt("app_startup") >= 7 && num >= 3.0)
		{
			btnRate.gameObject.SetActive(true);
		}
		else
		{
			btnRate.gameObject.SetActive(false);
		}
	}

	private void OnRateApp()
	{
		Application.OpenURL("market://details?id=com.koenigz.KittenCatSimulator");
	}

	private void Update()
	{
		buttonNext.gameObject.SetActive(curTexture < catTextures.Length - 1);
		buttonPrev.gameObject.SetActive(curTexture > 0);
		if (Input.GetKey(KeyCode.Escape))
		{
			panelMenu.gameObject.SetActive(false);
			panelQuit.gameObject.SetActive(true);
		}
	}

	public void OnChgSkin()
	{
		if (curSkin >= skins.Length - 1)
		{
			curSkin = 0;
		}
		else
		{
			curSkin++;
		}
		PlayerPrefs.SetInt("oskin", curSkin);
		follower.GetComponent<Renderer>().material = skins[curSkin];
	}

	public void OnQuitYes()
	{
		Application.Quit();
	}

	public void OnQuitNo()
	{
		panelMenu.gameObject.SetActive(true);
		panelQuit.gameObject.SetActive(false);
	}

	public void OnNext()
	{
		if (curTexture < catTextures.Length - 1)
		{
			curTexture++;
			catMesh.GetComponent<Renderer>().material = catTextures[curTexture];
			PlayerPrefs.SetInt("curText", curTexture);
			PlayerPrefs.SetInt("CatTexture", curTexture);
		}
	}

	public void OnMultiplayer()
	{
		PhotonNetwork.offlineMode = false;
		PlayerPrefs.SetInt("HumanCnt", 6);
		Application.LoadLevel("PreLobby");
	}

	public void OnPrev()
	{
		if (curTexture > 0)
		{
			curTexture--;
			catMesh.GetComponent<Renderer>().material = catTextures[curTexture];
			PlayerPrefs.SetInt("curText", curTexture);
			PlayerPrefs.SetInt("CatTexture", curTexture);
		}
	}

	public void OnModeFree()
	{
		PhotonNetwork.offlineMode = true;
		PhotonNetwork.JoinRoom("Offline");
		PlayerPrefs.SetString("mode", "free");
		StartCoroutine(LoadingLevel("Level1"));
	}

	public void OnToggleMusic()
	{
		int value = ((PlayerPrefs.GetInt("music_off", 0) == 0) ? 1 : 0);
		PlayerPrefs.SetInt("music_off", value);
		SetLblMusic();
	}

	public void SetLblMusic()
	{
		if (PlayerPrefs.GetInt("music_off", 0) != 0)
		{
			lblTglMusic.text = "Music: off";
		}
		else
		{
			lblTglMusic.text = "Music: on";
		}
	}

	public void OnModeTime()
	{
		PhotonNetwork.offlineMode = true;
		PhotonNetwork.JoinRoom("Offline");
		PlayerPrefs.SetString("mode", "time");
		StartCoroutine(LoadingLevel("Level1"));
	}

	public void OnModeSurvival()
	{
		PhotonNetwork.offlineMode = true;
		PhotonNetwork.JoinRoom("Offline");
		PlayerPrefs.SetString("mode", "survival");
		panelSurvival.gameObject.SetActive(true);
		panelMenu.gameObject.SetActive(false);
	}

	public void OnSurivalCnt1()
	{
		PlayerPrefs.SetInt("HumanCnt", 1);
		StartCoroutine(LoadingLevel("Level1"));
	}

	public void OnSurivalCnt2()
	{
		PlayerPrefs.SetInt("HumanCnt", 2);
		StartCoroutine(LoadingLevel("Level1"));
	}

	public void OnSurivalCnt3()
	{
		PlayerPrefs.SetInt("HumanCnt", 3);
		StartCoroutine(LoadingLevel("Level1"));
	}

	public void OnSurivalCnt4()
	{
		PlayerPrefs.SetInt("HumanCnt", 4);
		StartCoroutine(LoadingLevel("Level1"));
	}

	public void OnSurivalCnt5()
	{
		PlayerPrefs.SetInt("HumanCnt", 5);
		StartCoroutine(LoadingLevel("Level1"));
	}

	public void OnSurivalCnt6()
	{
		PlayerPrefs.SetInt("HumanCnt", 6);
		StartCoroutine(LoadingLevel("Level1"));
	}

	public void OnSurivalCnt7()
	{
		PlayerPrefs.SetInt("HumanCnt", 7);
		StartCoroutine(LoadingLevel("Level1"));
	}

	public void OnModeSurvivalBack()
	{
		panelSurvival.gameObject.SetActive(false);
		panelMenu.gameObject.SetActive(true);
		Camera.main.transform.position = new Vector3(0f, -1.693947f, -8.963148f);
	}

	public void OnMoreGames()
	{
		Application.OpenURL("https://play.google.com/store/apps/developer?id=Patrick+K%C3%B6nig");
	}

	private IEnumerator LoadingLevel(string levelName)
	{
		PlayerPrefs.SetInt("CatTexture", curTexture);
		panelMenu.gameObject.SetActive(true);
		panelSurvival.gameObject.SetActive(false);
		sliderProgress.gameObject.SetActive(true);
		lblLoading.gameObject.SetActive(true);
		background.gameObject.SetActive(true);
		AsyncOperation async = Application.LoadLevelAsync(levelName);
		sliderProgress.sliderValue = loadValue;
		while (!async.isDone)
		{
			loadValue = async.progress;
			sliderProgress.sliderValue = loadValue;
			yield return null;
		}
	}
}
