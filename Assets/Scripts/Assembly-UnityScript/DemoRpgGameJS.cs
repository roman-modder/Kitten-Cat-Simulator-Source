using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/DemoRpgGameJS")]
public class DemoRpgGameJS : MonoBehaviour
{
	public DemoRpgCharaJS player;

	public Camera cam;

	public TouchController ctrl;

	public GUISkin guiSkin;

	public PopupBoxJS popupBox;

	public float camOrbitalAngle;

	public float zoomFactorPerCm;

	public float camSmoothingTime;

	public float camZoom;

	private float camZoomForDisplay;

	private float camZoomVel;

	private float camFov;

	private float camDist;

	private float camBank;

	public float camFarBank;

	public float camCloseBank;

	public float camFarFov;

	public float camCloseFov;

	public float camFarDist;

	public float camCloseDist;

	private bool isMultiTouching;

	private float twistStartAngle;

	private float pinchStartZoom;

	[NonSerialized]
	public static int STICK_WALK;

	[NonSerialized]
	public static int ZONE_SCREEN;

	[NonSerialized]
	public static int ZONE_FIRE = 1;

	[NonSerialized]
	public static int ZONE_ACTION = 2;

	[NonSerialized]
	private static string CAPTION_COLOR_BEGIN = "<color='#FF0000'>";

	[NonSerialized]
	private static string CAPTION_COLOR_END = "</color>";

	[NonSerialized]
	private static string INSTRUCTIONS_TITLE = "Inctructions";

	[NonSerialized]
	private static string INSTRUCTIONS_BUTTON_TEXT = string.Empty;

	[NonSerialized]
	private static string INSTRUCTIONS_TEXT = CAPTION_COLOR_BEGIN + "* Walking.\n" + CAPTION_COLOR_END + "Press anywhere on the screen to activate the dynamic stick.\n" + "\n" + CAPTION_COLOR_BEGIN + "* Action.\n" + CAPTION_COLOR_END + "Tap on the screen or press the ACTION button to perform ACTION move.\n" + "\n" + CAPTION_COLOR_BEGIN + "* Zoom.\n" + CAPTION_COLOR_END + "Place two fingers on the screen and spread them to zoom out or pinch to zoom-in.\n" + "\n" + CAPTION_COLOR_BEGIN + "* Fire Weapon.\n" + CAPTION_COLOR_END + "Hold FIRE button to fire yout weapon.\n";

	public DemoRpgGameJS()
	{
		zoomFactorPerCm = 0.3f;
		camSmoothingTime = 0.2f;
		camZoom = 0.5f;
		camFarBank = 60f;
		camCloseBank = 25f;
		camFarFov = 60f;
		camCloseFov = 50f;
		camFarDist = 10f;
		camCloseDist = 4f;
	}

	public virtual void Start()
	{
		if (ctrl == null)
		{
			Debug.LogError("TouchController not assigned!");
		}
		if (cam == null)
		{
			Debug.LogError("Camera not assigned!");
		}
		ctrl.InitController();
		SnapCameraDisplay();
	}

	[ContextMenu("Snap Cam Display")]
	private void SnapCameraDisplay()
	{
		camZoomForDisplay = camZoom;
	}

	public virtual void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			DemoSwipeMenuJS.LoadMenuScene();
			return;
		}
		ctrl.PollController();
		ctrl.UpdateController();
		if (player != null)
		{
			player.ControlByTouch(ctrl, this);
			player.UpdateChara();
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
		TouchZone zone = ctrl.GetZone(ZONE_SCREEN);
		TouchStick stick = ctrl.GetStick(STICK_WALK);
		if (zone.MultiPressed(false, true))
		{
			if (!isMultiTouching)
			{
				pinchStartZoom = camZoom;
				isMultiTouching = true;
				zone.TakeoverTouches(stick);
			}
			if (zone.Pinched())
			{
				pinchStartZoom += zoomFactorPerCm * zone.GetPinchDistDelta(TouchCoordSys.SCREEN_CM, false);
				SetZoom(pinchStartZoom);
			}
		}
		else
		{
			isMultiTouching = false;
		}
		camZoom = Mathf.Clamp01(camZoom);
		camZoomForDisplay = Mathf.SmoothDamp(camZoomForDisplay, camZoom, ref camZoomVel, camSmoothingTime);
		PlaceCamera();
	}

	private void SetZoom(float zoomFactor)
	{
		camZoom = Mathf.Clamp01(zoomFactor);
	}

	private void PlaceCamera()
	{
		camZoom = Mathf.Clamp01(camZoom);
		camBank = Mathf.Lerp(camCloseBank, camFarBank, camZoomForDisplay);
		camDist = Mathf.Lerp(camCloseDist, camFarDist, camZoomForDisplay);
		camFov = Mathf.Lerp(camCloseFov, camFarFov, camZoomForDisplay);
		if (cam != null)
		{
			Quaternion quaternion = Quaternion.Euler(camBank, camOrbitalAngle, 0f);
			cam.transform.rotation = quaternion;
			cam.transform.position = player.transform.position + new Vector3(0f, 1.5f, 0f) + quaternion * new Vector3(0f, 0f, 0f - camDist);
			cam.fieldOfView = camFov;
		}
	}

	public virtual void OnGUI()
	{
		GUI.skin = guiSkin;
		if (ctrl != null)
		{
			ctrl.DrawControllerGUI();
		}
		if (popupBox != null && popupBox.IsVisible())
		{
			popupBox.DrawGUI();
			return;
		}
		GUI.color = Color.white;
		GUI.Label(new Rect(10f, 10f, Screen.width - 20, 100f), "RPG Demo - Press [Space] for help, [Esc] to quit.");
	}

	public virtual void Main()
	{
	}
}
