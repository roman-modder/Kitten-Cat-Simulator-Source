using UnityEngine;
using UnityEngine.UI;

public class PreLobby : MonoBehaviour
{
	public InputField catName;

	private void Start()
	{
		PhotonNetwork.offlineMode = false;
		catName.text = PlayerPrefs.GetString("CatName", "Kitty_" + Random.Range(0, 10000));
	}

	public void Play()
	{
		if (catName.text.Length > 0)
		{
			PhotonNetwork.player.name = catName.text;
			PlayerPrefs.SetString("CatName", catName.text);
			PlayerPrefs.Save();
			Application.LoadLevel("Lobby");
		}
	}

	public void Back()
	{
		PhotonNetwork.offlineMode = true;
		Application.LoadLevel("Menu");
	}
}
