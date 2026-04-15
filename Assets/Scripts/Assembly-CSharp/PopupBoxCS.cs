using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/PopupBoxCS")]
public class PopupBoxCS : MonoBehaviour
{
	public GUISkin guiSkin;

	public int guiDepth = 100;

	private bool visible;

	private bool complete;

	private string text;

	private string titleText;

	private string buttonText;

	private Vector2 textSize;

	private Vector2 scrollPos;

	private Rect boxRect;

	private Rect titleRect;

	private Rect buttonRect;

	private Rect textRect;

	public void Show(Rect rect, string title, string text, string buttonText = "")
	{
		if (!(guiSkin == null))
		{
			visible = true;
			complete = false;
			titleText = title;
			this.buttonText = buttonText;
			this.text = text;
			boxRect = rect;
		}
	}

	public void Show(string title, string text, string buttonText = "")
	{
		Show(new Rect((float)Screen.width * 0.05f, (float)Screen.height * 0.05f, (float)Screen.width * 0.9f, (float)Screen.height * 0.9f), title, text, buttonText);
	}

	public void End()
	{
		complete = true;
	}

	public bool IsComplete()
	{
		return complete;
	}

	public void Hide()
	{
		visible = false;
	}

	public bool IsVisible()
	{
		return visible;
	}

	public void DrawGUI()
	{
		if (visible)
		{
			bool flag = GUI.enabled;
			GUISkin skin = GUI.skin;
			int depth = GUI.depth;
			GUI.enabled = !complete;
			GUI.skin = guiSkin;
			GUI.depth = guiDepth;
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;
			GUI.contentColor = Color.white;
			GUI.Box(boxRect, string.Empty);
			GUILayout.BeginArea(boxRect);
			GUILayout.Label(titleText);
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			GUILayout.Box(text, guiSkin.customStyles[0]);
			GUILayout.EndScrollView();
			if (buttonText != null && buttonText.Length > 0 && GUILayout.Button(buttonText))
			{
				End();
			}
			GUILayout.EndArea();
			GUI.depth = depth;
			GUI.skin = skin;
			GUI.enabled = flag;
		}
	}
}
