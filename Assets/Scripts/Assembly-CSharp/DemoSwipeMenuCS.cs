using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/DemoSwipeMenuCS")]
public class DemoSwipeMenuCS : MonoBehaviour
{
	public TouchController ctrl;

	public SwipeMenuCS menu;

	public GUISkin guiSkin;

	public float fadeInDuration = 1f;

	private AnimTimer fadeInTimer;

	public Texture2D imgFpp;

	public Texture2D imgRpg;

	public Texture2D imgMap;

	public Texture2D imgDualStick;

	private const string MENU_SCENE_NAME = "CF-Demo-CS-Menu";

	private const string FPP_DEMO_SCENE_NAME = "CF-Demo-CS-FPP";

	private const string RPG_DEMO_SCENE_NAME = "CF-Demo-CS-RPG";

	private const string MAP_DEMO_SCENE_NAME = "CF-Demo-CS-Map";

	private const string DUAL_STICK_DEMO_SCENE_NAME = "CF-Demo-CS-Dual-Stick-Shooter";

	private const string EXIT_SCREEN_SCENE_NAME = "CF-Demo-Exit-Screen";

	public const int ITEM_DSS = 0;

	public const int ITEM_FPP = 1;

	public const int ITEM_RPG = 2;

	public const int ITEM_MAP = 3;

	public const int ITEM_COUNT = 4;

	private const string FPP_DEMO_DESCRIPTION = "FPP DEMO\nDynamic Stick, tapping, dragging, etc.";

	private const string RPG_DEMO_DESCRIPTION = "RPG DEMO\nDynamic Stick, Tap, Pinch, Canceling";

	private const string MAP_DEMO_DESCRIPTION = "MAP DEMO\nTwist, Pinch, Drag, Double Tap, Multi-finger Tap";

	private const string DSS_DEMO_DESCRIPTION = "DUAL STICK SHOOTER DEMO\nSimple Dual Sticks and Pause Button.";

	public static void LoadMenuScene()
	{
		Application.LoadLevel("CF-Demo-CS-Menu");
	}

	private void Start()
	{
		menu.Init(4, 0, Screen.width, ctrl.GetDPI(), false);
		fadeInTimer.Start(fadeInDuration);
	}

	private void StartDemo(string sceneName)
	{
		Application.LoadLevel(sceneName);
	}

	private void Update()
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
				Application.LoadLevel("CF-Demo-Exit-Screen");
				return;
			}
			TouchZone zone = ctrl.GetZone(0);
			if (zone.JustTapped())
			{
				menu.OnTap();
			}
			if (zone.JustUniPressed())
			{
				menu.OnPress();
			}
			if (zone.UniDragged())
			{
				menu.Move(0f - zone.GetUniDragDelta(TouchCoordSys.SCREEN_PX).x);
			}
			else if (zone.JustUniReleased())
			{
				menu.OnRelease(0f - zone.GetReleasedUniDragVel().x);
			}
		}
		menu.UpdateMenu();
		if (menu.JustCompleted())
		{
			switch (menu.GetCurItem())
			{
			case 1:
				StartDemo("CF-Demo-CS-FPP");
				break;
			case 2:
				StartDemo("CF-Demo-CS-RPG");
				break;
			case 3:
				StartDemo("CF-Demo-CS-Map");
				break;
			case 0:
				StartDemo("CF-Demo-CS-Dual-Stick-Shooter");
				break;
			}
		}
	}

	private void OnGUI()
	{
		GUI.skin = guiSkin;
		GUI.color = ((!fadeInTimer.Enabled) ? Color.white : new Color(1f, 1f, 1f, fadeInTimer.Nt));
		float num = ((!fadeInTimer.Enabled) ? 0f : Mathf.Lerp(Screen.height, 0f, fadeInTimer.Nt));
		float x = (float)Screen.width * 0.5f - menu.windowSize * 0.5f + (0f - menu.displayPos);
		Rect rect = new Rect(x, num, menu.windowSize, (float)Screen.height * 0.66f);
		for (int i = 0; i < 4; i++)
		{
			Texture2D image = null;
			string text = string.Empty;
			switch (i)
			{
			case 1:
				image = imgFpp;
				text = "FPP DEMO\nDynamic Stick, tapping, dragging, etc.";
				break;
			case 2:
				image = imgRpg;
				text = "RPG DEMO\nDynamic Stick, Tap, Pinch, Canceling";
				break;
			case 3:
				image = imgMap;
				text = "MAP DEMO\nTwist, Pinch, Drag, Double Tap, Multi-finger Tap";
				break;
			case 0:
				image = imgDualStick;
				text = "DUAL STICK SHOOTER DEMO\nSimple Dual Sticks and Pause Button.";
				break;
			}
			float num2 = 1f;
			if (menu.Selected() && i == menu.curItem)
			{
				num2 = 1f - Mathf.Clamp01(menu.GetTimeSinceSelection() / menu.completionDuration);
				num2 *= num2;
			}
			Rect position = rect;
			position.width *= num2;
			position.height *= num2;
			position.x = rect.center.x - position.width * 0.5f;
			position.y = rect.center.y - position.height * 0.5f;
			GUI.DrawTexture(position, image, ScaleMode.ScaleToFit);
			GUI.Box(new Rect(rect.x, num + (float)Screen.height * 0.65f, rect.width, (float)Screen.height * 0.35f), text);
			rect.x += menu.windowSize;
		}
	}
}
