using UnityEngine;

[RequireComponent(typeof(ChatGui))]
public class NamePickGui : MonoBehaviour
{
	public Vector2 GuiSize = new Vector2(200f, 300f);

	public string InputLine = string.Empty;

	private Rect guiCenteredRect;

	private ChatGui chatComponent;

	private string helpText = "Welcome to the Photon Chat demo.\nEnter a nickname to start. This demo does not require users to authenticate.";

	private const string UserNamePlayerPref = "NamePickUserName";

	public void Awake()
	{
		guiCenteredRect = new Rect((float)(Screen.width / 2) - GuiSize.x / 2f, (float)(Screen.height / 2) - GuiSize.y / 4f, GuiSize.x, GuiSize.y);
		chatComponent = GetComponent<ChatGui>();
		if (chatComponent != null && chatComponent.enabled)
		{
			Debug.LogWarning("When using the NamePickGui, ChatGui should be disabled initially.");
			if (chatComponent.chatClient != null)
			{
				chatComponent.chatClient.Disconnect();
			}
			chatComponent.enabled = false;
		}
		string text = PlayerPrefs.GetString("NamePickUserName");
		if (!string.IsNullOrEmpty(text))
		{
			InputLine = text;
		}
	}

	public void OnGUI()
	{
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return) && !string.IsNullOrEmpty(InputLine))
		{
			StartChat();
			return;
		}
		GUI.skin.label.wordWrap = true;
		GUILayout.BeginArea(guiCenteredRect);
		if (chatComponent != null && string.IsNullOrEmpty(chatComponent.ChatAppId))
		{
			GUILayout.Label("To continue, configure your Chat AppId.\nIt's listed in the Chat Dashboard (online).\nStop play-mode and edit:\nScripts/ChatGUI in the Hierarchy.");
			if (GUILayout.Button("Open Chat Dashboard"))
			{
				Application.OpenURL("https://www.exitgames.com/en/Chat/Dashboard");
			}
			GUILayout.EndArea();
			return;
		}
		GUILayout.Label(helpText);
		GUILayout.BeginHorizontal();
		GUI.SetNextControlName("NameInput");
		InputLine = GUILayout.TextField(InputLine);
		if (GUILayout.Button("Connect", GUILayout.ExpandWidth(false)))
		{
			StartChat();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		GUI.FocusControl("NameInput");
	}

	private void StartChat()
	{
		chatComponent.UserName = InputLine;
		chatComponent.enabled = true;
		base.enabled = false;
		PlayerPrefs.SetString("NamePickUserName", InputLine);
	}
}
