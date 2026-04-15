using UnityEngine;

public class MenuPolicy : MonoBehaviour
{
	public GameObject panelMenu;

	private void Awake()
	{
		Screen.sleepTimeout = -1;
		if (!ShowingPolicy())
		{
			Agree();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void Show()
	{
		panelMenu.gameObject.SetActive(true);
	}

	public void Agree()
	{
		PlayerPrefs.SetInt("policy", 1);
		PlayerPrefs.Save();
		Application.LoadLevel(Application.loadedLevel + 1);
	}

	public void ReadPolicy()
	{
		Application.OpenURL("http://www.koenigz.com/?p=privacy_policy");
	}

	public void ReadToS()
	{
		Application.OpenURL("http://www.koenigz.com/?p=terms");
	}

	private bool ShowingPolicy()
	{
		if (PlayerPrefs.GetInt("policy") != 0)
		{
			return false;
		}
		return true;
	}
}
