using UnityEngine;

public class ToHubButton : MonoBehaviour
{
	public Texture2D ButtonTexture;

	private Rect ButtonRect;

	private static ToHubButton instance;

	public static ToHubButton Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType(typeof(ToHubButton)) as ToHubButton;
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (ButtonTexture == null)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	public void OnGUI()
	{
		if (Application.loadedLevel != 0)
		{
			int num = ButtonTexture.width + 4;
			int num2 = ButtonTexture.height + 4;
			ButtonRect = new Rect(Screen.width - num, Screen.height - num2, num, num2);
			if (GUI.Button(ButtonRect, ButtonTexture, GUIStyle.none))
			{
				PhotonNetwork.Disconnect();
				Application.LoadLevel(0);
			}
		}
	}
}
