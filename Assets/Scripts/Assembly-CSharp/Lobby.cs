using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
	private const string NET_VERSION = "0.0.2";

	public Slider sliderPlayers;

	public InputField gameNameInput;

	public InputField gamePrivateName;

	public Text txtStatus;

	public Text txtGames;

	public Toggle togglePrivate;

	public GameObject content;

	public GameObject prefab;

	public Button[] btns;

	private void Start()
	{
		for (int i = 0; i < btns.Length; i++)
		{
			btns[i].gameObject.SetActive(false);
		}
		txtStatus.text = "Connecting...";
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings("0.0.2");
		}
		else
		{
			OnJoinedLobby();
		}
		sliderPlayers.value = PlayerPrefs.GetInt("Game_Players", 2);
		gameNameInput.text = PlayerPrefs.GetString("Game_Name", string.Empty);
	}

	private void OnJoinedLobby()
	{
		for (int i = 0; i < btns.Length; i++)
		{
			btns[i].gameObject.SetActive(true);
		}
		txtStatus.text = "Refreshing...";
		Invoke("ChckRooms", 1f);
	}

	public void ChckRooms()
	{
		for (int i = 0; i < content.transform.childCount; i++)
		{
			Object.Destroy(content.transform.GetChild(i).gameObject);
		}
		if (!PhotonNetwork.connected)
		{
			txtStatus.text = "Error: Connection issue";
		}
		else
		{
			txtStatus.text = "Refreshing...";
		}
		int num = 0;
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo in roomList)
		{
			if (roomInfo.maxPlayers > 0)
			{
				GameObject gameObject = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
				gameObject.GetComponent<ServerItem>().Init(roomInfo.name, roomInfo.playerCount, roomInfo.maxPlayers, roomInfo.customProperties["mode"].ToString());
				gameObject.transform.SetParent(content.transform, false);
				gameObject.transform.localScale = Vector3.one;
				num++;
			}
		}
		if (num <= 0)
		{
			txtStatus.text = "- server list empty -";
			Invoke("ChckRooms", 5f);
		}
		else
		{
			txtStatus.text = string.Empty;
			Invoke("ChckRooms", 30f);
		}
		txtGames.text = num + " Game(s)";
	}

	public void JoinPrivate()
	{
		string text = gamePrivateName.text.Trim();
		if (text.Length > 0)
		{
			PhotonNetwork.JoinRoom(text);
		}
	}

	public void OnJoinedRoom()
	{
		txtStatus.text = "Joining...";
		PhotonNetwork.isMessageQueueRunning = false;
		Application.LoadLevel("Level1");
	}

	public void CreateGameFree()
	{
		PlayerPrefs.SetString("mode", "free");
		Create();
	}

	public void CreateGameSurvival()
	{
		PlayerPrefs.SetString("mode", "survival");
		Create();
	}

	private void Create()
	{
		string text = gameNameInput.text.Trim();
		if (text.Length <= 0)
		{
			return;
		}
		txtStatus.text = "Creating...";
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo in roomList)
		{
			if (roomInfo.name == text)
			{
				ChckRooms();
				txtStatus.text = "Error: Room already exists";
				return;
			}
		}
		PlayerPrefs.SetInt("Game_Private", togglePrivate.isOn ? 1 : 0);
		PlayerPrefs.SetString("Game_Name", text);
		PlayerPrefs.SetInt("Game_Players", (int)sliderPlayers.value);
		string[] propsToListInLobby = new string[1] { "mode" };
		Hashtable hashtable = new Hashtable();
		hashtable.Add("mode", PlayerPrefs.GetString("mode"));
		Hashtable customRoomProperties = hashtable;
		PhotonNetwork.CreateRoom(PlayerPrefs.GetString("Game_Name"), PlayerPrefs.GetInt("Game_Private") != 1, true, PlayerPrefs.GetInt("Game_Players"), customRoomProperties, propsToListInLobby);
	}

	public void Menu()
	{
		txtStatus.text = "Disconnecting...";
		PhotonNetwork.Disconnect();
		Application.LoadLevel("Menu");
	}

	public void Username()
	{
		txtStatus.text = "Disconnecting...";
		PhotonNetwork.Disconnect();
		Application.LoadLevel("PreLobby");
	}

	private void OnPhotonCreateRoomFailed(object[] codeAndMsg)
	{
		txtStatus.text = "Error: Creating Game";
	}

	private void OnPhotonJoinRoomFailed(object[] codeAndMsg)
	{
		txtStatus.text = "Error: Joining (Game full?)";
		Invoke("ChckRooms", 2f);
	}

	private void OnConnectionFail(DisconnectCause cause)
	{
		txtStatus.text = "Error: Connection failure";
	}

	private void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		OnConnectionFail(cause);
	}
}
