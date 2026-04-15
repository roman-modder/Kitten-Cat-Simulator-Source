using UnityEngine;

public class ModeManager : MonoBehaviour
{
	public GameObject[] modes;

	private void Awake()
	{
		for (int i = 0; i < modes.Length; i++)
		{
			modes[i].gameObject.SetActive(false);
		}
	}

	private void Start()
	{
		int num = 0;
		switch (PlayerPrefs.GetString("mode"))
		{
		case "free":
			num = 0;
			break;
		case "time":
			num = 1;
			break;
		case "survival":
			num = 2;
			break;
		}
		modes[num].gameObject.SetActive(true);
	}
}
