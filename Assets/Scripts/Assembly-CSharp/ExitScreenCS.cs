using UnityEngine;

public class ExitScreenCS : MonoBehaviour
{
	public float forcedDuration = 2f;

	private const string siteUrl = "https://www.facebook.com/DansGameTools/app_251458316228";

	private const string assetStoreUrl = "http://u3d.as/5GT";

	public GUISkin guiSkin;

	private float elapsed;

	private void Start()
	{
		elapsed = 0f;
	}

	private void Update()
	{
		elapsed += Time.deltaTime;
		if (elapsed > forcedDuration && Input.GetKeyUp(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void OnGUI()
	{
		GUI.skin = guiSkin;
		GUI.color = new Color(0.4f, 0.4f, 0.4f, 1f);
		GUI.Label(new Rect(0f, (float)Screen.height * 0.05f, Screen.width, 200f), "Thank you for trying our demo!");
		GUI.color = Color.white;
		GUILayout.BeginArea(new Rect((float)Screen.width * 0.1f, Screen.height - 150, (float)Screen.width * 0.8f, 130f));
		if (GUILayout.Button("Visit our Shop"))
		{
			Application.OpenURL("https://www.facebook.com/DansGameTools/app_251458316228");
			Application.Quit();
		}
		if (GUILayout.Button("Visit Asset Store"))
		{
			Application.OpenURL("http://u3d.as/5GT");
			Application.Quit();
		}
		if (GUILayout.Button("Return to Main Menu"))
		{
			DemoSwipeMenuCS.LoadMenuScene();
		}
		else
		{
			GUILayout.EndArea();
		}
	}
}
