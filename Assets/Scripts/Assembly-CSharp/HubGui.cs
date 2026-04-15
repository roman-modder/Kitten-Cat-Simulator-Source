using UnityEngine;

public class HubGui : MonoBehaviour
{
	private struct DemoBtn
	{
		public string Text;

		public string Link;
	}

	public GUISkin Skin;

	private Vector2 scrollPos = default(Vector2);

	private string demoDescription = "<color=orange>PUN Demo Hub</color>\n\nSelect a demo to learn more about it.\n\nYou should open individual scenes in the Editor to dissect how they work.\n\nLook out for Console output. Especially in Editor (double click logs to jump to their origin in source).";

	private DemoBtn demoBtn;

	private DemoBtn webLink;

	private GUIStyle m_Headline;

	private void Start()
	{
		if (PhotonNetwork.connected || PhotonNetwork.connecting)
		{
			PhotonNetwork.Disconnect();
		}
		m_Headline = new GUIStyle(Skin.label);
		m_Headline.padding = new RectOffset(3, 0, 0, 0);
	}

	private void OnGUI()
	{
		GUI.skin = Skin;
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal();
		GUILayout.Space(10f);
		scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(320f));
		GUILayout.Label("Basics", m_Headline);
		if (GUILayout.Button("Demo Boxes", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>Demo Boxes</color>\n\nUses ConnectAndJoinRandom script.\n(joins a random room or creates one)\n\nInstantiates simple prefab.\nSynchronizes positions without smoothing.";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "DemoBoxes-Scene"
			};
		}
		if (GUILayout.Button("Demo Worker", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>Demo Worker</color>\n\nJoins the default lobby and shows existing rooms.\nLets you create or join a room.\nInstantiates an animated character.\nSynchronizes position and animation state of character with smoothing.\nImplements simple in-room Chat via RPC calls.";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "DemoWorker-Scene"
			};
		}
		if (GUILayout.Button("Movement Smoothing", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>Movement Smoothing</color>\n\nUses ConnectAndJoinRandom script.\nShows several basic ways to update positions of remote objects.";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "DemoSynchronization-Scene"
			};
		}
		GUILayout.Label("Advanced", m_Headline);
		if (GUILayout.Button("Ownership Transfer", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>Ownership Transfer</color>\n\nShows how to transfer the ownership of a PhotonView.\nThe owner will send position updates of the GameObject.\nTransfer can be edited per PhotonView and set to Fixed (no transfer), Request (owner has to agree) or Takeover (owner can't object).";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "DemoChangeOwner-Scene"
			};
			webLink = default(DemoBtn);
		}
		if (GUILayout.Button("Pickup, Teams, Scores", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>Pickup, Teams, Scores</color>\n\nUses ConnectAndJoinRandom script.\nImplements item pickup with RPCs.\nUses Custom Properties for Teams.\nCounts score per player and team.\nUses PhotonPlayer extension methods for easy Custom Property access.";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "DemoPickup-Scene"
			};
			webLink = default(DemoBtn);
		}
		GUILayout.Label("Feature Demos", m_Headline);
		if (GUILayout.Button("Chat", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>Chat</color>\n\nUses the Chat API (now part of PUN).\nSimple UI.\nYou can enter any User ID.\nAutomatically subscribes some channels.\nAllows simple commands via text.\n\nRequires configuration of Chat App ID in scene.";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "DemoChat-Scene"
			};
			webLink = default(DemoBtn);
		}
		if (GUILayout.Button("RPG Movement", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>RPG Movement</color>\n\nDemonstrates how to use the PhotonTransformView component to synchronize position updates smoothly using inter- and extrapolation.\n\nThis demo also shows how to setup a Mecanim Animator to update animations automatically based on received position updates (without sending explicit animation updates).";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "DemoRPGMovement-Scene"
			};
			webLink = default(DemoBtn);
		}
		if (GUILayout.Button("Mecanim Animations", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>Mecanim Animations</color>\n\nThis demo shows how to use the PhotonAnimatorView component to easily synchronize Mecanim animations.\n\nIt also demonstrates another feature of the PhotonTransformView component which gives you more control how position updates are inter-/extrapolated by telling the component how fast the object moves and turns using SetSynchronizedValues().";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "DemoMecanim-Scene"
			};
			webLink = default(DemoBtn);
		}
		if (GUILayout.Button("2D Game", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>2D Game Demo</color>\n\nSynchronizes animations, positions and physics in a 2D scene.";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "Demo2DJumpAndRunWithPhysics-Scene"
			};
			webLink = default(DemoBtn);
		}
		if (GUILayout.Button("Friends & Authentication", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>Friends & Authentication</color>\n\nShows connect with or without (server-side) authentication.\n\nAuthentication requires minor server-side setup (in Dashboard).\n\nOnce connected, you can find (made up) friends.\nJoin a room just to see how that gets visible in friends list.";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "DemoFriends-Scene"
			};
			webLink = default(DemoBtn);
		}
		GUILayout.Label("Tutorial", m_Headline);
		if (GUILayout.Button("Marco Polo Tutorial", GUILayout.Width(280f)))
		{
			demoDescription = "<color=orange>Marco Polo Tutorial</color>\n\nFinal result you could get when you do the Marco Polo Tutorial.\nSlightly modified to be more compatible with this package.";
			demoBtn = new DemoBtn
			{
				Text = "Start",
				Link = "MarcoPolo-Scene"
			};
			webLink = new DemoBtn
			{
				Text = "Open Tutorial (www)",
				Link = "http://tinyurl.com/nmylf44"
			};
		}
		GUILayout.EndScrollView();
		GUILayout.BeginVertical(GUILayout.Width(Screen.width - 345));
		GUILayout.Label(demoDescription);
		GUILayout.Space(10f);
		if (!string.IsNullOrEmpty(demoBtn.Text) && GUILayout.Button(demoBtn.Text))
		{
			Application.LoadLevel(demoBtn.Link);
		}
		if (!string.IsNullOrEmpty(webLink.Text) && GUILayout.Button(webLink.Text))
		{
			Application.OpenURL(webLink.Link);
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
}
