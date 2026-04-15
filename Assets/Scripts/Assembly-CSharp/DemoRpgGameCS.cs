using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/DemoRpgGameCS")]
public class DemoRpgGameCS : MonoBehaviour
{
	public DemoRpgCharaCS player;

	public Camera cam;

	public TouchController ctrl;

	public GUISkin guiSkin;

	public PopupBoxCS popupBox;

	public const int STICK_WALK = 0;

	public const int ZONE_SCREEN = 0;

	public const int ZONE_FIRE = 1;

	public const int ZONE_ACTION = 2;

	public float camOrbitalAngle;

	private float camFov;

	private float camDist;

	private float camBank;

	public float zoomFactorPerCm = 0.3f;

	public float camSmoothingTime = 0.2f;

	public float camZoom = 0.5f;

	private float camZoomForDisplay;

	private float camZoomVel;

	public float camFarBank = 60f;

	public float camCloseBank = 25f;

	public float camFarFov = 60f;

	public float camCloseFov = 50f;

	public float camFarDist = 10f;

	public float camCloseDist = 4f;

	private bool isMultiTouching;

	private float twistStartAngle;

	private float pinchStartZoom;

	private const string INSTRUCTIONS_TITLE = "Inctructions";

	private const string INSTRUCTIONS_BUTTON_TEXT = "";

	private const string CAPTION_COLOR_BEGIN = "<color='#FF0000'>";

	private const string CAPTION_COLOR_END = "</color>";

	private const string INSTRUCTIONS_TEXT = "<color='#FF0000'>* Walking.\n</color>Press anywhere on the screen to activate the dynamic stick.\n\n<color='#FF0000'>* Action.\n</color>Tap on the screen or press the ACTION button to perform ACTION move.\n\n<color='#FF0000'>* Zoom.\n</color>Place two fingers on the screen and spread them to zoom out or pinch to zoom-in.\n\n<color='#FF0000'>* Fire Weapon.\n</color>Hold FIRE button to fire yout weapon.\n";

	private void Start()
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

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			DemoSwipeMenuCS.LoadMenuScene();
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
					popupBox.Show("Inctructions", "<color='#FF0000'>* Walking.\n</color>Press anywhere on the screen to activate the dynamic stick.\n\n<color='#FF0000'>* Action.\n</color>Tap on the screen or press the ACTION button to perform ACTION move.\n\n<color='#FF0000'>* Zoom.\n</color>Place two fingers on the screen and spread them to zoom out or pinch to zoom-in.\n\n<color='#FF0000'>* Fire Weapon.\n</color>Hold FIRE button to fire yout weapon.\n", string.Empty);
				}
			}
			else if (Input.GetKeyDown(KeyCode.Space))
			{
				popupBox.Hide();
			}
		}
		TouchZone zone = ctrl.GetZone(0);
		TouchStick stick = ctrl.GetStick(0);
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
				pinchStartZoom += zoomFactorPerCm * zone.GetPinchDistDelta(TouchCoordSys.SCREEN_CM);
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

	public void OnGUI()
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
}
