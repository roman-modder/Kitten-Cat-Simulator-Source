using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/DemoDualStickShooterGameJS")]
public class DemoDualStickShooterGameJS : MonoBehaviour
{
	public TouchController ctrl;

	public DemoDualStickShooterCharaJS player;

	public Camera cam;

	public GUISkin guiSkin;

	public PopupBoxJS popupBox;

	[NonSerialized]
	public static int STICK_WALK;

	[NonSerialized]
	public static int STICK_FIRE = 1;

	[NonSerialized]
	public static int ZONE_SCREEN;

	[NonSerialized]
	public static int ZONE_PAUSE = 1;

	private Vector3 camOfs;

	[NonSerialized]
	private static string CAPTION_COLOR_BEGIN = "<color='#FF0000'>";

	[NonSerialized]
	private static string CAPTION_COLOR_END = "</color>";

	[NonSerialized]
	private static string INFO_TITLE = "Inctructions";

	[NonSerialized]
	private static string INFO_BUTTON_TEXT = "OK";

	[NonSerialized]
	private static string INFO_BODY = CAPTION_COLOR_BEGIN + "* Walking.\n" + CAPTION_COLOR_END + "Press on the left half of the screen to activate blue dynamic stick.\n" + "\n" + CAPTION_COLOR_BEGIN + "* Shooting.\n" + CAPTION_COLOR_END + "Press on the right half of the screen to activate red dynamic stick.\n" + "Holding the stick will activate gun's trigger.\n" + "\n" + CAPTION_COLOR_BEGIN + "* Aiming.\n" + CAPTION_COLOR_END + "Moving the stick from it's neutral position will orient the character in it's direction.\n" + "Aiming speed is proportional to stick's tilt.\n" + string.Empty;

	public virtual void Start()
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

	public virtual void Update()
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
				DemoSwipeMenuJS.LoadMenuScene();
				return;
			}
			if ((bool)ctrl)
			{
				TouchStick stick = ctrl.GetStick(STICK_WALK);
				TouchStick stick2 = ctrl.GetStick(STICK_FIRE);
				TouchZone zone = ctrl.GetZone(ZONE_PAUSE);
				if (zone.JustUniReleased() || Input.GetKeyUp(KeyCode.Space))
				{
					popupBox.Show(INFO_TITLE, INFO_BODY, INFO_BUTTON_TEXT);
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

	public virtual void OnGUI()
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

	public virtual void Main()
	{
	}
}
