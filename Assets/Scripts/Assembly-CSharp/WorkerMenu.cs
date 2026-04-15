using UnityEngine;

public class WorkerMenu : MonoBehaviour
{
	public GUISkin Skin;

	public Vector2 WidthAndHeight = new Vector2(600f, 400f);

	private string roomName = "myRoom";

	private Vector2 scrollPos = Vector2.zero;

	private bool connectFailed;

	public static readonly string SceneNameMenu = "DemoWorker-Scene";

	public static readonly string SceneNameGame = "DemoWorkerGame-Scene";

	private string errorDialog;

	private double timeToClearDialog;

	public string ErrorDialog
	{
		get
		{
			return errorDialog;
		}
		private set
		{
			errorDialog = value;
			if (!string.IsNullOrEmpty(value))
			{
				timeToClearDialog = Time.time + 4f;
			}
		}
	}

	public void Awake()
	{
		PhotonNetwork.automaticallySyncScene = true;
		if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
		{
			PhotonNetwork.ConnectUsingSettings("0.9");
		}
		if (string.IsNullOrEmpty(PhotonNetwork.playerName))
		{
			PhotonNetwork.playerName = "Guest" + Random.Range(1, 9999);
		}
	}

	public void OnGUI()
	{
		if (Skin != null)
		{
			GUI.skin = Skin;
		}
		if (!PhotonNetwork.connected)
		{
			if (PhotonNetwork.connecting)
			{
				GUILayout.Label("Connecting to: " + PhotonNetwork.ServerAddress);
			}
			else
			{
				GUILayout.Label(string.Concat("Not connected. Check console output. Detailed connection state: ", PhotonNetwork.connectionStateDetailed, " Server: ", PhotonNetwork.ServerAddress));
			}
			if (connectFailed)
			{
				GUILayout.Label("Connection failed. Check setup and use Setup Wizard to fix configuration.");
				GUILayout.Label(string.Format("Server: {0}", new object[1] { PhotonNetwork.ServerAddress }));
				GUILayout.Label("AppId: " + PhotonNetwork.PhotonServerSettings.AppID);
				if (GUILayout.Button("Try Again", GUILayout.Width(100f)))
				{
					connectFailed = false;
					PhotonNetwork.ConnectUsingSettings("0.9");
				}
			}
			return;
		}
		Rect rect = new Rect(((float)Screen.width - WidthAndHeight.x) / 2f, ((float)Screen.height - WidthAndHeight.y) / 2f, WidthAndHeight.x, WidthAndHeight.y);
		GUI.Box(rect, "Join or Create Room");
		GUILayout.BeginArea(rect);
		GUILayout.Space(40f);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player name:", GUILayout.Width(150f));
		PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
		GUILayout.Space(158f);
		if (GUI.changed)
		{
			PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Roomname:", GUILayout.Width(150f));
		roomName = GUILayout.TextField(roomName);
		if (GUILayout.Button("Create Room", GUILayout.Width(150f)))
		{
			PhotonNetwork.CreateRoom(roomName, new RoomOptions
			{
				maxPlayers = 10
			}, null);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Join Room", GUILayout.Width(150f)))
		{
			PhotonNetwork.JoinRoom(roomName);
		}
		GUILayout.EndHorizontal();
		if (!string.IsNullOrEmpty(ErrorDialog))
		{
			GUILayout.Label(ErrorDialog);
			if (timeToClearDialog < (double)Time.time)
			{
				timeToClearDialog = 0.0;
				ErrorDialog = string.Empty;
			}
		}
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal();
		GUILayout.Label(PhotonNetwork.countOfPlayers + " users are online in " + PhotonNetwork.countOfRooms + " rooms.");
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Join Random", GUILayout.Width(150f)))
		{
			PhotonNetwork.JoinRandomRoom();
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(15f);
		if (PhotonNetwork.GetRoomList().Length == 0)
		{
			GUILayout.Label("Currently no games are available.");
			GUILayout.Label("Rooms will be listed here, when they become available.");
		}
		else
		{
			GUILayout.Label(PhotonNetwork.GetRoomList().Length + " rooms available:");
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			foreach (RoomInfo roomInfo in roomList)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(roomInfo.name + " " + roomInfo.playerCount + "/" + roomInfo.maxPlayers);
				if (GUILayout.Button("Join", GUILayout.Width(150f)))
				{
					PhotonNetwork.JoinRoom(roomInfo.name);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}
		GUILayout.EndArea();
	}

	public void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom");
	}

	public void OnPhotonCreateRoomFailed()
	{
		ErrorDialog = "Error: Can't create room (room name maybe already used).";
		Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
	}

	public void OnPhotonJoinRoomFailed(object[] cause)
	{
		ErrorDialog = "Error: Can't join room (full or unknown room name). " + cause[1];
		Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
	}

	public void OnPhotonRandomJoinFailed()
	{
		ErrorDialog = "Error: Can't join random room (none found).";
		Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
	}

	public void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		PhotonNetwork.LoadLevel(SceneNameGame);
	}

	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("Disconnected from Photon.");
	}

	public void OnFailedToConnectToPhoton(object parameters)
	{
		connectFailed = true;
		Debug.Log(string.Concat("OnFailedToConnectToPhoton. StatusCode: ", parameters, " ServerAddress: ", PhotonNetwork.networkingPeer.ServerAddress));
	}
}
