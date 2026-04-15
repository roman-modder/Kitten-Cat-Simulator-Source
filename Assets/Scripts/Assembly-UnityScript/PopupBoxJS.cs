using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/PopupBoxJS")]
public class PopupBoxJS : MonoBehaviour
{
	public GUISkin guiSkin;

	public int guiDepth;

	private bool visible;

	private bool complete;

	private string text;

	private string titleText;

	private string buttonText;

	private Vector2 textSize;

	private Vector2 scrollPos;

	private Rect boxRect;

	public PopupBoxJS()
	{
		guiDepth = 100;
	}

	public virtual void Show(Rect rect, string title, string text)
	{
		Show(rect, title, text, string.Empty);
	}

	public virtual void Show(Rect rect, string title, string text, string buttonText)
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

	public virtual void Show(string title, string text)
	{
		Show(title, text);
	}

	public virtual void Show(string title, string text, string buttonText)
	{
		Show(new Rect((float)Screen.width * 0.05f, (float)Screen.height * 0.05f, (float)Screen.width * 0.9f, (float)Screen.height * 0.9f), title, text, buttonText);
	}

	public virtual void End()
	{
		complete = true;
	}

	public virtual bool IsComplete()
	{
		return complete;
	}

	public virtual void Hide()
	{
		visible = false;
	}

	public virtual bool IsVisible()
	{
		return visible;
	}

	public virtual void DrawGUI()
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

	public virtual void Main()
	{
	}
}
