using Crosstales.BadWord;
using Photon;
using UnityEngine;
using UnityEngine.UI;

public class Logic : Photon.MonoBehaviour
{
	public int score;

	public int finalScore;

	public UILabel lblScore;

	public UILabel lblScoreLost;

	public UILabel lblAdd;

	public UIPanel panelGame;

	public UIPanel panelLost;

	public Material skyboxDay;

	public Material skyboxMid;

	public Material skyboxNight;

	public GameObject follower;

	public GameObject chatButton;

	public Material[] skins;

	private TouchScreenKeyboard tsk;

	public Text txtChat;

	private float fadeTimer;

	private float dayFac = -1f;

	private GameObject player;

	private Vector3 spawnPos = new Vector3(-8.28f, 0.47f, -4.29f);

	public Material[] catTextures;

	private void Start()
	{
		panelLost.gameObject.SetActive(false);
		Time.timeScale = 1f;
		AudioListener.pause = false;
		Color color = lblAdd.color;
		color.a = 0f;
		lblAdd.color = color;
		AddScore(0);
		follower.GetComponent<Renderer>().material = skins[PlayerPrefs.GetInt("oskin")];
		spawnPos += Vector3.up * Random.Range(0.5f, 1.25f);
		spawnPos += Vector3.right * Random.Range(-0.5f, 0.5f);
		if (!PhotonNetwork.offlineMode)
		{
			PhotonNetwork.isMessageQueueRunning = true;
			player = PhotonNetwork.Instantiate("cu_cat", spawnPos, Quaternion.identity, 0);
		}
		else
		{
			player = PhotonNetwork.Instantiate("cu_cat", spawnPos, Quaternion.identity, 0);
			chatButton.gameObject.SetActive(false);
		}
		CatMotor component = player.GetComponent<CatMotor>();
		component.photonView.RPC("SetSkin", PhotonTargets.AllBuffered, component.photonView.viewID, PlayerPrefs.GetInt("CatTexture"));
		AddTextChatLocal("Game", "Welcome!");
		if (!PhotonNetwork.offlineMode)
		{
			for (int i = 0; i < 3; i++)
			{
				AddTextChatLocal("ATTENTION", "NEVER SHARE PERSONAL INFORMATION ONLINE!");
			}
		}
	}

	public void HandleChat()
	{
		tsk = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false, false);
	}

	[PunRPC]
	public void AddTextChat(string usr, string txt)
	{
		txt = BadWordManager.ReplaceAll(txt);
		usr = BadWordManager.ReplaceAll(usr);
		AddTextChatLocal(usr, txt);
	}

	public void AddTextChatLocal(string usr, string txt)
	{
		AddText(txtChat, "<color=red>" + usr + "</color>: " + txt);
	}

	private void OnPhotonPlayerConnected(PhotonPlayer p)
	{
		AddTextChat(p.name, "connected");
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer p)
	{
		AddTextChat(p.name, "disconnected");
	}

	private void AddText(Text txt, string strTxt)
	{
		string text = strTxt + "\n" + txt.text;
		string[] array = text.Split('\n');
		text = string.Empty;
		for (int i = 0; i < Mathf.Min(array.Length, 6); i++)
		{
			text = text + array[i] + "\n";
		}
		txt.text = text;
		fadeTimer = 0f;
	}

	private void RemoveLast(Text txt)
	{
		string[] array = txt.text.Split('\n');
		for (int num = array.Length - 1; num >= 0; num--)
		{
			if (array[num] != string.Empty)
			{
				array[num] = string.Empty;
				break;
			}
		}
		string text = string.Empty;
		for (int i = 0; i < Mathf.Min(array.Length, 6); i++)
		{
			text = text + array[i] + "\n";
		}
		txt.text = text;
	}

	public static Logic FindInstance()
	{
		return Object.FindObjectOfType<Logic>();
	}

	public void ChangePlayerSkin(GameObject go, int skinId)
	{
		go.transform.Find("cu_cat_mesh").GetComponent<Renderer>().material = catTextures[skinId];
	}

	private void Update()
	{
		lblScore.text = score.ToString();
		if (tsk != null && tsk.done)
		{
			string text = tsk.text.Trim();
			if (text.Length > 0)
			{
				base.photonView.RPC("AddTextChat", PhotonTargets.Others, PhotonNetwork.player.name, text);
				AddTextChatLocal(PhotonNetwork.player.name, text);
			}
			tsk.text = string.Empty;
			tsk = null;
		}
		fadeTimer += Time.deltaTime;
		if (fadeTimer > 5f)
		{
			fadeTimer = 0f;
			RemoveLast(txtChat);
		}
		if (score < finalScore)
		{
			score++;
			if (Random.Range(0, 2) == 0)
			{
				lblScore.color = Color.white;
			}
			else
			{
				lblScore.color = Color.black;
			}
		}
		else
		{
			lblScore.color = Color.white;
		}
		Color color = lblAdd.color;
		color.a -= Time.deltaTime * 0.45f;
		lblAdd.color = color;
		Color ambientLight = RenderSettings.ambientLight;
		if (ambientLight.r <= 0.15f)
		{
			dayFac = 1f;
		}
		else if (ambientLight.r >= 1f)
		{
			dayFac = -1f;
		}
		float num = 0.025f;
		if (ambientLight.r <= 0.1f)
		{
			RenderSettings.skybox = skyboxNight;
			num = 0.005f;
		}
		else if (ambientLight.r < 0.5f)
		{
			RenderSettings.skybox = skyboxMid;
			num = 0.015f;
		}
		else if (ambientLight.r < 0.7f)
		{
			RenderSettings.skybox = skyboxDay;
		}
		ambientLight.r += dayFac * num * Time.deltaTime;
		ambientLight.g = ambientLight.r;
		ambientLight.b = ambientLight.r;
		RenderSettings.ambientLight = ambientLight;
	}

	public void AddScore(int addScore)
	{
		lblScoreLost.text = "Your score: " + finalScore;
		if (addScore > 0)
		{
			finalScore += addScore;
			lblAdd.text = "+ " + addScore;
			lblAdd.color = Color.white;
		}
	}

	public void GameOver()
	{
		if (!AudioListener.pause)
		{
			if (PhotonNetwork.offlineMode)
			{
				Time.timeScale = 0f;
			}
			else
			{
				base.photonView.RPC("AddTextChat", PhotonTargets.Others, PhotonNetwork.player.name, "got caught");
				AddTextChatLocal(PhotonNetwork.player.name, "got caught");
			}
			panelGame.gameObject.SetActive(false);
			panelLost.gameObject.SetActive(true);
			AudioListener.pause = true;
		}
	}

	public void OnMenu()
	{
		PhotonNetwork.Disconnect();
		Application.LoadLevel("Menu");
	}

	public void OnRetry()
	{
		if (!PhotonNetwork.offlineMode)
		{
			AudioListener.pause = false;
			panelGame.gameObject.SetActive(true);
			panelLost.gameObject.SetActive(false);
			spawnPos.y = 5f;
			player.transform.position = spawnPos;
			player.GetComponent<Rigidbody>().velocity = Vector3.zero;
			player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			Time.timeScale = 1f;
			finalScore = (score = 0);
		}
		else
		{
			Application.LoadLevel(Application.loadedLevelName);
		}
	}
}
