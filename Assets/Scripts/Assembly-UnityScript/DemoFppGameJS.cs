using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/DemoFppGameJS")]
public class DemoFppGameJS : MonoBehaviour
{
	public GUISkin guiSkin;

	public TouchController touchCtrl;

	public DemoFppCharaJS chara;

	public PopupBoxJS popupBox;

	private bool pauseMenuOn;

	private AnimTimer pauseMenuTimer;

	private bool pauseMenuFadeIn;

	public float pauseMenuAnimDuration;

	private TouchController.AnimFloat pauseMenuAnim;

	private Rect pauseMenuRect;

	[NonSerialized]
	public static int STICK_WALK;

	[NonSerialized]
	public static int ZONE_FIRE;

	[NonSerialized]
	public static int ZONE_ZOOM = 1;

	[NonSerialized]
	public static int ZONE_RELOAD = 2;

	[NonSerialized]
	public static int ZONE_AIM = 3;

	[NonSerialized]
	public static int ZONE_PAUSE = 4;

	[NonSerialized]
	private static string CAPTION_COLOR_BEGIN = "<color='#FF0000'>";

	[NonSerialized]
	private static string CAPTION_COLOR_END = "</color>";

	[NonSerialized]
	private static string INSTRUCTIONS_TITLE = "Inctructions";

	[NonSerialized]
	private static string INSTRUCTIONS_BUTTON_TEXT = string.Empty;

	[NonSerialized]
	private static string INSTRUCTIONS_TEXT = CAPTION_COLOR_BEGIN + "* Walking.\n" + CAPTION_COLOR_END + "Press on the left half of the screen to activate the dynamic stick.\n" + "\n" + CAPTION_COLOR_BEGIN + "* Aiming.\n" + CAPTION_COLOR_END + "Drag on the right half of the screen.\n" + "\n" + CAPTION_COLOR_BEGIN + "* Reset Vertical Aim.\n" + CAPTION_COLOR_END + "Double tap on the right half of the screen.\n" + "\n" + CAPTION_COLOR_BEGIN + "* Toggle Zoom.\n" + CAPTION_COLOR_END + "Tap on the ZOOM button.\n" + "\n" + CAPTION_COLOR_BEGIN + "* Zoom In/Out.\n" + CAPTION_COLOR_END + "When zoom is activated, touch the ZOOM button and drag up or down.";

	public DemoFppGameJS()
	{
		pauseMenuAnimDuration = 0.3f;
	}

	public virtual void Start()
	{
		if (touchCtrl != null)
		{
			touchCtrl.InitController();
			touchCtrl.HideController(0f);
			touchCtrl.ShowController(2f);
		}
		if (chara != null)
		{
			chara.SetTouchController(touchCtrl);
		}
	}

	public virtual void Update()
	{
		if (touchCtrl != null)
		{
			touchCtrl.PollController();
			if (touchCtrl.LayoutChanged())
			{
				touchCtrl.GetZone(ZONE_PAUSE).SetRect(new Rect(5f, 5f, 32f, 32f));
				if (chara != null && chara.playerViewCam != null)
				{
					Rect screenEmuRect = touchCtrl.GetScreenEmuRect(true);
					chara.playerViewCam.pixelRect = screenEmuRect;
				}
				touchCtrl.LayoutChangeHandled();
			}
		}
		if (touchCtrl != null)
		{
			touchCtrl.UpdateController();
		}
		if (IsPaused())
		{
			UpdatePauseMenu();
			return;
		}
		if (popupBox != null)
		{
			if (!popupBox.IsVisible())
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					popupBox.Show(INSTRUCTIONS_TITLE, INSTRUCTIONS_TEXT, INSTRUCTIONS_BUTTON_TEXT);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Space))
			{
				popupBox.Hide();
			}
		}
		if (chara != null)
		{
			chara.UpdateChara();
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			DemoSwipeMenuJS.LoadMenuScene();
		}
		else if (touchCtrl.GetZone(ZONE_PAUSE).JustUniReleased(true, false))
		{
			StartPauseMenu();
		}
	}

	public virtual void OnGUI()
	{
		GUI.skin = guiSkin;
		if (chara != null)
		{
			chara.DrawGUIBG();
		}
		if (touchCtrl != null)
		{
			touchCtrl.DrawControllerGUI();
		}
		if (chara != null)
		{
			chara.DrawCustomGUI();
		}
		if (IsPaused())
		{
			DrawPauseMenu();
		}
		if (popupBox != null && popupBox.IsVisible())
		{
			popupBox.DrawGUI();
			return;
		}
		GUI.color = Color.white;
		GUI.Label(new Rect(40f, 10f, Screen.width - 100, 100f), "FPP Demo - Press [Space] for help, [Esc] to quit.");
	}

	public virtual void StartPauseMenu()
	{
		chara.OnPauseStart();
		pauseMenuOn = true;
		pauseMenuTimer.Start(pauseMenuAnimDuration);
		pauseMenuFadeIn = true;
		pauseMenuRect = new Rect(0.05f * (float)Screen.width, 0.05f * (float)Screen.height, 0.9f * (float)Screen.width, 0.9f * (float)Screen.height);
		pauseMenuAnim.Reset(-Screen.height);
		pauseMenuAnim.MoveTo(pauseMenuRect.y);
		touchCtrl.DisableController();
	}

	public virtual void ExitPauseMenu()
	{
		pauseMenuTimer.Start(pauseMenuAnimDuration);
		pauseMenuFadeIn = false;
		pauseMenuAnim.MoveTo(-Screen.height);
	}

	private void OnPauseEnd()
	{
		pauseMenuOn = false;
		chara.OnPauseEnd();
		touchCtrl.EnableController();
	}

	private void UpdatePauseMenu()
	{
		if (!pauseMenuOn)
		{
			return;
		}
		if (pauseMenuTimer.Enabled)
		{
			if (pauseMenuTimer.Completed)
			{
				pauseMenuTimer.Disable();
				if (!pauseMenuFadeIn)
				{
					OnPauseEnd();
				}
			}
			else
			{
				pauseMenuTimer.Update(Time.deltaTime);
				pauseMenuAnim.Update(pauseMenuTimer.Nt * pauseMenuTimer.Nt);
			}
		}
		else if (Input.GetKeyDown(KeyCode.Escape))
		{
			ExitPauseMenu();
		}
	}

	private void DrawPauseMenu()
	{
		if (pauseMenuOn)
		{
			Rect position = pauseMenuRect;
			position.y = pauseMenuAnim.cur;
			GUI.color = Color.white;
			GUI.contentColor = new Color(1f, 1f, 1f, 1f);
			GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);
			GUI.Box(position, "PAUSE");
			position.x += pauseMenuRect.width * 0.1f;
			position.y += 30f;
			position.width = pauseMenuRect.width * 0.8f;
			position.height = 30f;
			GUI.Label(position, "Aim Sensitivity");
			position.y += position.height + 10f;
			chara.aimSensitivity = GUI.HorizontalSlider(position, chara.aimSensitivity, 0f, 1f);
			position.y += position.height + 10f;
			bool leftHandedMode = touchCtrl.GetLeftHandedMode();
			if (leftHandedMode != GUI.Toggle(position, leftHandedMode, "Left Handed Mode"))
			{
				touchCtrl.SetLeftHandedMode(!leftHandedMode);
			}
			position.y += position.height + 10f;
			position.height = 30f;
			if (GUI.Button(position, "EXIT"))
			{
				ExitPauseMenu();
			}
			GUI.color = Color.white;
		}
	}

	public virtual bool IsPaused()
	{
		return pauseMenuOn;
	}

	public virtual void Main()
	{
	}
}
