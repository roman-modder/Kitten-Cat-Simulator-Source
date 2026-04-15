using UnityEngine;

public class DemoBoxesGui : MonoBehaviour
{
	public bool HideUI;

	private void OnGUI()
	{
		if (HideUI)
		{
			return;
		}
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		if (!PhotonNetwork.connected)
		{
			if (GUILayout.Button("Connect"))
			{
				PhotonNetwork.ConnectUsingSettings(null);
			}
		}
		else if (GUILayout.Button("Disconnect"))
		{
			PhotonNetwork.Disconnect();
		}
	}
}
