using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/DemoDualStickShooterGameCS")]
public class DemoDualStickShooterGameCS : MonoBehaviour
{
	public TouchController ctrl;

	public DemoDualStickShooterCharaCS player;

	public Camera cam;

	public GUISkin guiSkin;

	public PopupBoxCS popupBox;

	public const int STICK_WALK = 0;

	public const int STICK_FIRE = 1;

	public const int ZONE_SCREEN = 0;

	public const int ZONE_PAUSE = 1;

	private Vector3 camOfs;

	private const string CAPTION_COLOR_BEGIN = "<color='#FF0000'>";

	private const string CAPTION_COLOR_END = "</color>";

	private const string INFO_TITLE = "Inctructions";

	private const string INFO_BUTTON_TEXT = "OK";

	private const string INFO_BODY = "<color='#FF0000'>* Walking.\n</color>Press on the left half of the screen to activate blue dynamic stick.\n\n<color='#FF0000'>* Shooting.\n</color>Press on the right half of the screen to activate red dynamic stick.\nHolding the stick will activate gun's trigger.\n\n<color='#FF0000'>* Aiming.\n</color>Moving the stick from it's neutral position will orient the character in it's direction.\nAiming speed is proportional to stick's tilt.\n";

	private void Start()
	{
		player.Init(this);
		if (cam != null && player != null)
		{
			camOfs = cam.transform.position - player.transform.position;
		}
		else
		{
			camOfs = new Vector3(0f, 5f, -5f);
		}
	}

	private void Update()
	{
		if (popupBox != null && popupBox.IsVisible())
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				popupBox.End();
			}
			if (popupBox.IsComplete())
			{
				popupBox.Hide();
				ctrl.EnableController();
				player.OnUnpause();
			}
		}
		else
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				DemoSwipeMenuCS.LoadMenuScene();
				return;
			}
			if ((bool)ctrl)
			{
				TouchStick stick = ctrl.GetStick(0);
				TouchStick stick2 = ctrl.GetStick(1);
				TouchZone zone = ctrl.GetZone(1);
				if (zone.JustUniReleased() || Input.GetKeyUp(KeyCode.Space))
				{
					popupBox.Show("Inctructions", "<color='#FF0000'>* Walking.\n</color>Press on the left half of the screen to activate blue dynamic stick.\n\n<color='#FF0000'>* Shooting.\n</color>Press on the right half of the screen to activate red dynamic stick.\nHolding the stick will activate gun's trigger.\n\n<color='#FF0000'>* Aiming.\n</color>Moving the stick from it's neutral position will orient the character in it's direction.\nAiming speed is proportional to stick's tilt.\n", "OK");
					ctrl.DisableController();
					player.OnPause();
				}
				else
				{
					if (stick.Pressed())
					{
						player.Move(stick.GetVec3d(true, 0f), stick.GetTilt());
					}
					else
					{
						player.Move(Vector3.zero, 0f);
					}
					if (stick2.Pressed())
					{
						player.SetTriggerState(true);
						player.Aim(stick2.GetAngle(), stick2.GetTilt());
					}
					else
					{
						player.SetTriggerState(false);
						player.Aim(0f, 0f);
					}
				}
			}
		}
		player.UpdateChara();
		if (cam != null)
		{
			Transform transform = cam.transform;
			transform.position = player.transform.position + camOfs;
		}
	}

	private void OnGUI()
	{
		if (ctrl != null)
		{
			ctrl.DrawControllerGUI();
		}
		if (popupBox != null && !popupBox.IsVisible())
		{
			GUI.skin = guiSkin;
			GUI.color = Color.white;
			GUI.Label(new Rect(40f, 10f, Screen.width - 100, 100f), "Dual Stick Demo - Press [Space] for help, [Esc] to quit.");
		}
		if (popupBox != null)
		{
			popupBox.DrawGUI();
		}
	}
}
