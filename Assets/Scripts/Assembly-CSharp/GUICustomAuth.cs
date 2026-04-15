using UnityEngine;

public class GUICustomAuth : MonoBehaviour
{
	private enum GuiState
	{
		AuthOrNot = 0,
		AuthInput = 1,
		AuthHelp = 2,
		AuthFailed = 3
	}

	public Rect GuiRect;

	private string authName = "usr";

	private string authToken = "usr";

	private string authDebugMessage = string.Empty;

	private GuiState guiState;

	public GameObject RootOf3dButtons;

	private void Start()
	{
		GuiRect = new Rect(Screen.width / 4, 80f, Screen.width / 2, Screen.height - 100);
	}

	public void OnJoinedLobby()
	{
		base.enabled = false;
	}

	public void OnCustomAuthenticationFailed(string debugMessage)
	{
		authDebugMessage = debugMessage;
		SetStateAuthFailed();
	}

	public void SetStateAuthInput()
	{
		RootOf3dButtons.SetActive(false);
		guiState = GuiState.AuthInput;
	}

	public void SetStateAuthHelp()
	{
		RootOf3dButtons.SetActive(false);
		guiState = GuiState.AuthHelp;
	}

	public void SetStateAuthOrNot()
	{
		RootOf3dButtons.SetActive(true);
		guiState = GuiState.AuthOrNot;
	}

	public void SetStateAuthFailed()
	{
		RootOf3dButtons.SetActive(false);
		guiState = GuiState.AuthFailed;
	}

	public void ConnectWithNickname()
	{
		RootOf3dButtons.SetActive(false);
		AuthenticationValues authenticationValues = new AuthenticationValues();
		authenticationValues.UserId = PhotonNetwork.playerName;
		PhotonNetwork.AuthValues = authenticationValues;
		PhotonNetwork.playerName += "Nick";
		PhotonNetwork.ConnectUsingSettings("1.0");
	}

	private void OnGUI()
	{
		if (PhotonNetwork.connected)
		{
			GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
			return;
		}
		GUILayout.BeginArea(GuiRect);
		switch (guiState)
		{
		case GuiState.AuthFailed:
			GUILayout.Label("Authentication Failed");
			GUILayout.Space(10f);
			GUILayout.Label("Error message:\n'" + authDebugMessage + "'");
			GUILayout.Space(10f);
			GUILayout.Label("For this demo set the Authentication URL in the Dashboard to:\nhttp://photon.webscript.io/auth-demo-equals");
			GUILayout.Label("That authentication-service has no user-database. It confirms any user if 'name equals password'.");
			GUILayout.Label("The error message comes from that service and can be customized.");
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Back"))
			{
				SetStateAuthInput();
			}
			if (GUILayout.Button("Help"))
			{
				SetStateAuthHelp();
			}
			GUILayout.EndHorizontal();
			break;
		case GuiState.AuthHelp:
			GUILayout.Label("By default, any player can connect to Photon.\n'Custom Authentication' can be enabled to reject players without valid user-account.");
			GUILayout.Label("The actual authentication must be done by a web-service which you host and customize. Example sourcecode for these services is available on the docs page.");
			GUILayout.Label("For this demo set the Authentication URL in the Dashboard to:\nhttp://photon.webscript.io/auth-demo-equals");
			GUILayout.Label("That authentication-service has no user-database. It confirms any user if 'name equals password'.");
			GUILayout.Space(10f);
			if (GUILayout.Button("Configure Authentication (Dashboard)"))
			{
				Application.OpenURL("https://www.exitgames.com/dashboard");
			}
			if (GUILayout.Button("Authentication Docs"))
			{
				Application.OpenURL("http://doc.exitgames.com/en/pun/current/tutorials/pun-and-facebook-custom-authentication");
			}
			GUILayout.Space(10f);
			if (GUILayout.Button("Back to input"))
			{
				SetStateAuthInput();
			}
			break;
		case GuiState.AuthInput:
			GUILayout.Label("Authenticate yourself");
			GUILayout.BeginHorizontal();
			authName = GUILayout.TextField(authName, GUILayout.Width(Screen.width / 4 - 5));
			GUILayout.FlexibleSpace();
			authToken = GUILayout.TextField(authToken, GUILayout.Width(Screen.width / 4 - 5));
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Authenticate"))
			{
				PhotonNetwork.AuthValues = new AuthenticationValues();
				PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
				PhotonNetwork.AuthValues.AddAuthParameter("username", authName);
				PhotonNetwork.AuthValues.AddAuthParameter("token", authToken);
				PhotonNetwork.ConnectUsingSettings("1.0");
			}
			GUILayout.Space(10f);
			if (GUILayout.Button("Help", GUILayout.Width(100f)))
			{
				SetStateAuthHelp();
			}
			break;
		}
		GUILayout.EndArea();
	}
}
