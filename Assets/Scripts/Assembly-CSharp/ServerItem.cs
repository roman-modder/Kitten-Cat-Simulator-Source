using Crosstales.BadWord;
using UnityEngine;
using UnityEngine.UI;

public class ServerItem : MonoBehaviour
{
	public Text text;

	public Button btn;

	private string roomName;

	public void Init(string name, int minPlayers, int maxPlayers, string mode)
	{
		roomName = name;
		if (BadWordManager.Contains(name))
		{
			minPlayers = maxPlayers;
		}
		name = BadWordManager.ReplaceAll(name);
		text.text = name + "\n" + minPlayers + "/" + maxPlayers + "\nMode: " + mode;
		if (minPlayers >= maxPlayers)
		{
			btn.gameObject.SetActive(false);
		}
	}

	public void Join()
	{
		btn.gameObject.SetActive(false);
		PhotonNetwork.JoinRoom(roomName);
	}
}
