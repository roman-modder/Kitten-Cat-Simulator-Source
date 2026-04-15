using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/DemoSwipeMenuJS")]
public class DemoSwipeMenuJS : MonoBehaviour
{
	public TouchController ctrl;

	public SwipeMenuJS menu;

	public GUISkin guiSkin;

	public Texture2D imgFpp;

	public Texture2D imgRpg;

	public Texture2D imgMap;

	public Texture2D imgDualStick;

	public float fadeInDuration;

	private AnimTimer fadeInTimer;

	[NonSerialized]
	private static string MENU_SCENE_NAME = "CF-Demo-JS-Menu";

	[NonSerialized]
	private static string FPP_DEMO_SCENE_NAME = "CF-Demo-JS-FPP";

	[NonSerialized]
	private static string RPG_DEMO_SCENE_NAME = "CF-Demo-JS-RPG";

	[NonSerialized]
	private static string MAP_DEMO_SCENE_NAME = "CF-Demo-JS-Map";

	[NonSerialized]
	private static string DSS_DEMO_SCENE_NAME = "CF-Demo-JS-Dual-Stick-Shooter";

	[NonSerialized]
	private static string EXIT_SCREEN_SCENE_NAME = "CF-Demo-Exit-Screen";

	[NonSerialized]
	public static int ITEM_DSS;

	[NonSerialized]
	public static int ITEM_FPP = 1;

	[NonSerialized]
	public static int ITEM_RPG = 2;

	[NonSerialized]
	public static int ITEM_MAP = 3;

	[NonSerialized]
	public static int ITEM_COUNT = 4;

	[NonSerialized]
	private static string FPP_DEMO_DESCRIPTION = "FPP DEMO\nDynamic Stick, tapping, dragging, etc.";

	[NonSerialized]
	private static string RPG_DEMO_DESCRIPTION = "RPG DEMO\nDynamic Stick, Tap, Pinch, Canceling";

	[NonSerialized]
	private static string MAP_DEMO_DESCRIPTION = "MAP DEMO\nTwist, Pinch, Drag, Double Tap, Multi-finger Tap";

	[NonSerialized]
	private static string DSS_DEMO_DESCRIPTION = "DUAL STICK SHOOTER DEMO\nSimple Dual Sticks and Pause Button.";

	public DemoSwipeMenuJS()
	{
		fadeInDuration = 1f;
	}

	public static void LoadMenuScene()
	{
		Application.LoadLevel(MENU_SCENE_NAME);
	}

	public virtual void Start()
	{
		menu.Init(ITEM_COUNT, 0, Screen.width, ctrl.GetDPI(), false);
		fadeInTimer.Start(fadeInDuration);
	}

	private void StartDemo(string sceneName)
	{
		Application.LoadLevel(sceneName);
	}

	public virtual void Update()
	{
		if (ctrl != null && ctrl.LayoutChanged())
		{
			menu.SetWindowSize(Screen.width, ctrl.GetDPI());
		}
		if (fadeInTimer.Enabled)
		{
			fadeInTimer.Update(Time.deltaTime);
			if (fadeInTimer.Completed)
			{
				fadeInTimer.Disable();
			}
		}
		else
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				Application.LoadLevel(EXIT_SCREEN_SCENE_NAME);
				return;
			}
			TouchZone zone = ctrl.GetZone(0);
			if (zone.JustTapped())
			{
				menu.OnTap();
			}
			if (zone.JustUniPressed(false, false))
			{
				menu.OnPress();
			}
			if (zone.UniDragged())
			{
				menu.Move(0f - zone.GetUniDragDelta(TouchCoordSys.SCREEN_PX, false).x);
			}
			else if (zone.JustUniReleased(true, true))
			{
				menu.OnRelease(0f - zone.GetReleasedUniDragVel(TouchCoordSys.SCREEN_PX, false).x);
			}
		}
		menu.UpdateMenu();
		if (menu.JustCompleted())
		{
			int curItem = menu.GetCurItem();
			if (curItem == ITEM_FPP)
			{
				StartDemo(FPP_DEMO_SCENE_NAME);
			}
			else if (curItem == ITEM_RPG)
			{
				StartDemo(RPG_DEMO_SCENE_NAME);
			}
			else if (curItem == ITEM_DSS)
			{
				StartDemo(DSS_DEMO_SCENE_NAME);
			}
			else if (curItem == ITEM_MAP)
			{
				StartDemo(MAP_DEMO_SCENE_NAME);
			}
		}
	}

	public virtual void OnGUI()
	{
		GUI.skin = guiSkin;
		GUI.color = ((!fadeInTimer.Enabled) ? Color.white : new Color(1f, 1f, 1f, fadeInTimer.Nt));
		float num = ((!fadeInTimer.Enabled) ? 0f : Mathf.Lerp(Screen.height, 0f, fadeInTimer.Nt));
		float x = (float)Screen.width * 0.5f - menu.windowSize * 0.5f + (0f - menu.displayPos);
		Rect rect = new Rect(x, num, menu.windowSize, (float)Screen.height * 0.66f);
		for (int i = 0; i < ITEM_COUNT; i++)
		{
			Texture2D image = null;
			string text = string.Empty;
			int num2 = i;
			if (num2 == ITEM_FPP)
			{
				image = imgFpp;
				text = FPP_DEMO_DESCRIPTION;
			}
			else if (num2 == ITEM_RPG)
			{
				image = imgRpg;
				text = RPG_DEMO_DESCRIPTION;
			}
			else if (num2 == ITEM_MAP)
			{
				image = imgMap;
				text = MAP_DEMO_DESCRIPTION;
			}
			else if (num2 == ITEM_DSS)
			{
				image = imgDualStick;
				text = DSS_DEMO_DESCRIPTION;
			}
			float num3 = 1f;
			if (menu.Selected() && i == menu.curItem)
			{
				num3 = 1f - Mathf.Clamp01(menu.GetTimeSinceSelection() / menu.completionDuration);
				num3 *= num3;
			}
			Rect position = rect;
			position.width *= num3;
			position.height *= num3;
			position.x = rect.center.x - position.width * 0.5f;
			position.y = rect.center.y - position.height * 0.5f;
			GUI.DrawTexture(position, image, ScaleMode.ScaleToFit);
			GUI.Box(new Rect(rect.x, num + (float)Screen.height * 0.65f, rect.width, (float)Screen.height * 0.35f), text);
			rect.x += menu.windowSize;
		}
	}

	public virtual void Main()
	{
	}
}
