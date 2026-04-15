using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Chat;
using UnityEngine;

public class ChatGui : MonoBehaviour, IChatClientListener
{
	public string ChatAppId;

	public string[] ChannelsToJoinOnConnect;

	public int HistoryLengthToFetch;

	public bool DemoPublishOnSubscribe;

	private ChatChannel selectedChannel;

	private string selectedChannelName;

	private int selectedChannelIndex;

	private bool doingPrivateChat;

	public ChatClient chatClient;

	public Rect GuiRect = new Rect(0f, 0f, 250f, 300f);

	public bool IsVisible = true;

	public bool AlignBottom;

	public bool FullScreen;

	private string inputLine = string.Empty;

	private string userIdInput = string.Empty;

	private Vector2 scrollPos = Vector2.zero;

	private static string WelcomeText = "Welcome to chat.\\help lists commands.";

	private static string HelpText = "\n\\subscribe <list of channelnames> subscribes channels.\n\\unsubscribe <list of channelnames> leaves channels.\n\\msg <username> <message> send private message to user.\n\\clear clears the current chat tab. private chats get closed.\n\\help gets this help message.";

	private static ChatGui instance;

	public string UserName { get; set; }

	public static ChatGui Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<ChatGui>();
			}
			return instance;
		}
	}

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (instance != this)
		{
			Debug.LogWarning(string.Concat("Destroying duplicate instance of ", base.gameObject, ". ChatGui applies DontDestroyOnLoad() to this GameObject."));
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Start()
	{
		Application.runInBackground = true;
		if (string.IsNullOrEmpty(UserName))
		{
			UserName = "user" + Environment.TickCount % 99;
		}
		chatClient = new ChatClient(this);
		chatClient.Connect(ChatAppId, "1.0", new ExitGames.Client.Photon.Chat.AuthenticationValues(UserName));
		if (AlignBottom)
		{
			GuiRect.y = (float)Screen.height - GuiRect.height;
		}
		if (FullScreen)
		{
			GuiRect.x = 0f;
			GuiRect.y = 0f;
			GuiRect.width = Screen.width;
			GuiRect.height = Screen.height;
		}
		Debug.Log(UserName);
	}

	public void OnApplicationQuit()
	{
		if (chatClient != null)
		{
			chatClient.Disconnect();
		}
	}

	public void OnDestroy()
	{
		if (Instance != null && Instance == this && chatClient != null)
		{
			chatClient.Disconnect();
		}
	}

	public void Update()
	{
		if (chatClient != null)
		{
			chatClient.Service();
		}
	}

	public void OnGUI()
	{
		if (!IsVisible)
		{
			return;
		}
		GUI.skin.label.wordWrap = true;
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
		{
			if ("ChatInput".Equals(GUI.GetNameOfFocusedControl()))
			{
				GuiSendsMsg();
				return;
			}
			GUI.FocusControl("ChatInput");
		}
		GUI.SetNextControlName(string.Empty);
		GUILayout.BeginArea(GuiRect);
		GUILayout.FlexibleSpace();
		if (chatClient == null || chatClient.State != ChatState.ConnectedToFrontEnd)
		{
			GUILayout.Label("Not in chat yet.");
		}
		else
		{
			List<string> list = new List<string>(chatClient.PublicChannels.Keys);
			int count = list.Count;
			list.AddRange(chatClient.PrivateChannels.Keys);
			if (list.Count > 0)
			{
				int num = selectedChannelIndex;
				int num2 = list.IndexOf(selectedChannelName);
				selectedChannelIndex = ((num2 >= 0) ? num2 : 0);
				selectedChannelIndex = GUILayout.Toolbar(selectedChannelIndex, list.ToArray(), GUILayout.ExpandWidth(false));
				scrollPos = GUILayout.BeginScrollView(scrollPos);
				doingPrivateChat = selectedChannelIndex >= count;
				selectedChannelName = list[selectedChannelIndex];
				if (selectedChannelIndex != num)
				{
					scrollPos.y = float.MaxValue;
					if (doingPrivateChat)
					{
						string[] array = selectedChannelName.Split(new char[1] { ':' }, 3);
						userIdInput = array[1];
					}
				}
				GUILayout.Label(WelcomeText);
				if (chatClient.TryGetChannel(selectedChannelName, doingPrivateChat, out selectedChannel))
				{
					for (int i = 0; i < selectedChannel.Messages.Count; i++)
					{
						string arg = selectedChannel.Senders[i];
						object arg2 = selectedChannel.Messages[i];
						GUILayout.Label(string.Format("{0}: {1}", arg, arg2));
					}
				}
				GUILayout.EndScrollView();
			}
		}
		GUILayout.BeginHorizontal();
		if (doingPrivateChat)
		{
			GUILayout.Label("to:", GUILayout.ExpandWidth(false));
			GUI.SetNextControlName("WhisperTo");
			userIdInput = GUILayout.TextField(userIdInput, GUILayout.MinWidth(100f), GUILayout.ExpandWidth(false));
			string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
			if (nameOfFocusedControl.Equals("WhisperTo"))
			{
				if (userIdInput.Equals("username"))
				{
					userIdInput = string.Empty;
				}
			}
			else if (string.IsNullOrEmpty(userIdInput))
			{
				userIdInput = "username";
			}
		}
		GUI.SetNextControlName("ChatInput");
		inputLine = GUILayout.TextField(inputLine);
		if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)))
		{
			GuiSendsMsg();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	private void GuiSendsMsg()
	{
		if (string.IsNullOrEmpty(inputLine))
		{
			GUI.FocusControl(string.Empty);
			return;
		}
		if (inputLine[0].Equals('\\'))
		{
			string[] array = inputLine.Split(new char[1] { ' ' }, 2);
			if (array[0].Equals("\\help"))
			{
				PostHelpToCurrentChannel();
			}
			if (array[0].Equals("\\state"))
			{
				int num = int.Parse(array[1]);
				chatClient.SetOnlineStatus(num, new string[1] { "i am state " + num });
			}
			else if (array[0].Equals("\\subscribe") && !string.IsNullOrEmpty(array[1]))
			{
				chatClient.Subscribe(array[1].Split(' ', ','));
			}
			else if (array[0].Equals("\\unsubscribe") && !string.IsNullOrEmpty(array[1]))
			{
				chatClient.Unsubscribe(array[1].Split(' ', ','));
			}
			else if (array[0].Equals("\\clear"))
			{
				ChatChannel channel;
				if (doingPrivateChat)
				{
					chatClient.PrivateChannels.Remove(selectedChannelName);
				}
				else if (chatClient.TryGetChannel(selectedChannelName, doingPrivateChat, out channel))
				{
					channel.ClearMessages();
				}
			}
			else if (array[0].Equals("\\msg") && !string.IsNullOrEmpty(array[1]))
			{
				string[] array2 = array[1].Split(new char[2] { ' ', ',' }, 2);
				string target = array2[0];
				string message = array2[1];
				chatClient.SendPrivateMessage(target, message);
			}
		}
		else if (doingPrivateChat)
		{
			chatClient.SendPrivateMessage(userIdInput, inputLine);
		}
		else
		{
			chatClient.PublishMessage(selectedChannelName, inputLine);
		}
		inputLine = string.Empty;
		GUI.FocusControl(string.Empty);
	}

	private void PostHelpToCurrentChannel()
	{
		ChatChannel chatChannel = selectedChannel;
		if (chatChannel != null)
		{
			chatChannel.Add("info", HelpText);
		}
		else
		{
			Debug.LogError("no channel for help");
		}
	}

	public void OnConnected()
	{
		if (ChannelsToJoinOnConnect != null && ChannelsToJoinOnConnect.Length > 0)
		{
			chatClient.Subscribe(ChannelsToJoinOnConnect, HistoryLengthToFetch);
		}
		chatClient.AddFriends(new string[2] { "tobi", "ilya" });
		chatClient.SetOnlineStatus(2);
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		Debug.Log(message);
	}

	public void OnDisconnected()
	{
	}

	public void OnChatStateChange(ChatState state)
	{
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		if (DemoPublishOnSubscribe)
		{
			foreach (string channelName in channels)
			{
				chatClient.PublishMessage(channelName, "says 'hi' in OnSubscribed(). ");
			}
		}
	}

	public void OnUnsubscribed(string[] channels)
	{
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		if (channelName.Equals(selectedChannelName))
		{
			scrollPos.y = float.MaxValue;
		}
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		ChatChannel chatChannel = selectedChannel;
		if (chatChannel != null)
		{
			chatChannel.Add("info", string.Format("{0} is {1}. Msg:{2}", user, status, message));
		}
		Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
	}
}
