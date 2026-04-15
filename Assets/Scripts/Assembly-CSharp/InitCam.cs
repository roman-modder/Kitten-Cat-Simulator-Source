using UnityEngine;

public class InitCam : MonoBehaviour
{
	public AudioSource bgMusic;

	private void Awake()
	{
		if (PlayerPrefs.GetInt("music_off", 0) != 0)
		{
			bgMusic.enabled = false;
		}
		else
		{
			bgMusic.enabled = true;
		}
	}
}
