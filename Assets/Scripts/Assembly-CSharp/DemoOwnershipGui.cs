using UnityEngine;

public class DemoOwnershipGui : MonoBehaviour
{
	public GUISkin Skin;

	public bool TransferOwnershipOnRequest = true;

	public void OnOwnershipRequest(object[] viewAndPlayer)
	{
		PhotonView photonView = viewAndPlayer[0] as PhotonView;
		PhotonPlayer photonPlayer = viewAndPlayer[1] as PhotonPlayer;
		Debug.Log(string.Concat("OnOwnershipRequest(): Player ", photonPlayer, " requests ownership of: ", photonView, "."));
		if (TransferOwnershipOnRequest)
		{
			photonView.TransferOwnership(photonPlayer.ID);
		}
	}

	public void OnGUI()
	{
		GUI.skin = Skin;
		GUILayout.BeginArea(new Rect(Screen.width - 200, 0f, 200f, Screen.height));
		string text = ((!TransferOwnershipOnRequest) ? "rejecting to pass" : "passing objects");
		if (GUILayout.Button(text))
		{
			TransferOwnershipOnRequest = !TransferOwnershipOnRequest;
		}
		GUILayout.EndArea();
		if (PhotonNetwork.inRoom)
		{
			int iD = PhotonNetwork.player.ID;
			string arg = ((!PhotonNetwork.player.isMasterClient) ? string.Empty : "(master) ");
			string colorName = PlayerVariables.GetColorName(PhotonNetwork.player.ID);
			GUILayout.Label(string.Format("player {0}, {1} {2}(you)", iD, colorName, arg));
			PhotonPlayer[] otherPlayers = PhotonNetwork.otherPlayers;
			foreach (PhotonPlayer photonPlayer in otherPlayers)
			{
				iD = photonPlayer.ID;
				arg = ((!photonPlayer.isMasterClient) ? string.Empty : "(master)");
				colorName = PlayerVariables.GetColorName(photonPlayer.ID);
				GUILayout.Label(string.Format("player {0}, {1} {2}", iD, colorName, arg));
			}
			if (PhotonNetwork.inRoom && PhotonNetwork.otherPlayers.Length == 0)
			{
				GUILayout.Label("Join more clients to switch object-control.");
			}
		}
		else
		{
			GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		}
	}
}
