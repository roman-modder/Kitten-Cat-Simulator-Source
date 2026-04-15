using System;
using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/DemoFppCharaCS")]
public class DemoFppCharaCS : MonoBehaviour
{
	public Camera playerViewCam;

	public CharacterController charaCtrl;

	public Texture2D scopeTex;

	public GunCS gun;

	public Transform headBone;

	public Transform upperBodyBone;

	public Transform gunBone;

	public float normalFov = 40f;

	public float zoomFovMin = 20f;

	public float zoomFovMax = 10f;

	private bool zoomActive;

	private float zoomFactor;

	public float zoomFactorPerCm = 0.5f;

	public float walkSpeed = 8f;

	public float strafeSpeed = 5f;

	public float aimSensitivity = 1f;

	public float horzAimSpeed = 20f;

	public float vertAimSpeed = 10f;

	public Vector3 camOfs = new Vector3(0f, 1.6f, 0f);

	private float aimHorz;

	private float aimVert;

	private float aimHorzDisplay;

	private float aimVertDisplay;

	public float aimSmoothingTime = 0.1f;

	private float aimHorzDampVel;

	private float aimVertDampVel;

	private bool isAiming;

	private float aimVertRaw;

	private float aimHorzRaw;

	private bool isZoomDragging;

	private float zoomFactorRaw;

	private const float WALK_DEADZONE = 0.1f;

	private Vector2 walkTargetDir;

	private Vector2 walkCurDir;

	private bool walkingCur;

	private bool walkingPrev;

	private float walkElapsed;

	public float walkVertBobScale = 0.1f;

	public float walkVertBobFreq = 0.5f;

	public float walkSmoothingTime = 0.2f;

	private Vector3 walkDirDampVel;

	private Vector3 walkBobVec;

	private Vector3 upperBodyInitialPos;

	private TouchController touchCtrl;

	private const float MIN_AIM_ANGLE = -50f;

	private const float MAX_AIM_ANGLE = 70f;

	private void Start()
	{
		if (charaCtrl == null)
		{
			charaCtrl = base.gameObject.GetComponent<CharacterController>();
		}
		if (upperBodyBone != null)
		{
			upperBodyInitialPos = upperBodyBone.localPosition;
		}
	}

	public void ControlByTouch()
	{
		if (touchCtrl == null)
		{
			return;
		}
		TouchStick stick = touchCtrl.GetStick(0);
		TouchZone zone = touchCtrl.GetZone(3);
		TouchZone zone2 = touchCtrl.GetZone(0);
		TouchZone zone3 = touchCtrl.GetZone(1);
		TouchZone zone4 = touchCtrl.GetZone(2);
		if (stick.Pressed())
		{
			Vector2 vec = stick.GetVec();
			SetWalkSpeed(vec.y, vec.x);
		}
		else
		{
			SetWalkSpeed(0f, 0f);
		}
		SetTriggerState(zone2.UniPressed(true, false));
		if (zone4.JustUniPressed(true, true))
		{
			ReloadWeapon();
		}
		if (zone3.JustTapped())
		{
			zoomActive = !zoomActive;
		}
		if (zoomActive && zone3.UniPressed(false, false))
		{
			if (!isZoomDragging)
			{
				isZoomDragging = true;
				zoomFactorRaw = zoomFactor;
			}
			zoomFactorRaw += zoomFactorPerCm * zone3.GetUniDragDelta(TouchCoordSys.SCREEN_CM, true).y;
			zoomFactor = Mathf.Clamp(zoomFactorRaw, 0f, 1f);
		}
		else
		{
			isZoomDragging = false;
		}
		if (zone.UniPressed(false, false) || zone2.UniPressed(false, false))
		{
			if (!isAiming)
			{
				isAiming = true;
				aimVertRaw = aimVert;
				aimHorzRaw = aimHorz;
			}
			Vector2 vector = zone.GetUniDragDelta(TouchCoordSys.SCREEN_CM, true) + zone2.GetUniDragDelta(TouchCoordSys.SCREEN_CM, true);
			vector *= Mathf.Lerp(0.1f, 1f, aimSensitivity);
			vector.x *= horzAimSpeed;
			vector.y *= vertAimSpeed;
			aimHorzRaw += vector.x;
			aimVertRaw += vector.y;
			Aim(aimHorzRaw, aimVertRaw);
		}
		else
		{
			isAiming = false;
		}
		if (zone.JustDoubleTapped())
		{
			Aim(aimHorz, 0f);
		}
	}

	public void UpdateChara()
	{
		if (touchCtrl != null)
		{
			ControlByTouch();
		}
		aimHorzDisplay = Mathf.SmoothDamp(aimHorzDisplay, aimHorz, ref aimHorzDampVel, aimSmoothingTime);
		aimVertDisplay = Mathf.SmoothDamp(aimVertDisplay, aimVert, ref aimVertDampVel, aimSmoothingTime);
		walkCurDir = Vector3.SmoothDamp(walkCurDir, walkTargetDir, ref walkDirDampVel, walkSmoothingTime);
		float magnitude = walkCurDir.magnitude;
		walkingPrev = walkingCur;
		walkingCur = magnitude > 0.1f;
		if (walkingCur)
		{
			if (!walkingPrev)
			{
				walkElapsed = 0f;
			}
			else
			{
				walkElapsed += Time.deltaTime;
			}
		}
		if (walkingCur)
		{
			walkBobVec = Vector3.zero;
			walkBobVec.y = SineBobPositive(walkElapsed, walkVertBobFreq) * walkVertBobScale;
			walkBobVec *= Mathf.Clamp01(magnitude);
		}
		else
		{
			walkBobVec = Vector3.zero;
		}
		Vector3 vector = new Vector3(walkCurDir.x, 0f, walkCurDir.y);
		vector.z *= walkSpeed;
		vector.x *= strafeSpeed;
		vector *= Time.deltaTime;
		vector = Quaternion.Euler(0f, aimHorz, 0f) * vector;
		if (charaCtrl != null)
		{
			charaCtrl.Move(vector);
		}
		else
		{
			base.transform.position += vector;
		}
		base.transform.localRotation = Quaternion.Euler(0f, aimHorzDisplay, 0f);
		if (upperBodyBone != null)
		{
			upperBodyBone.localRotation = Quaternion.Euler(aimVertDisplay, 0f, 0f);
			upperBodyBone.localPosition = upperBodyInitialPos + walkBobVec;
		}
	}

	public void OnPauseStart()
	{
		if (gun != null)
		{
			gun.SetTriggerState(false);
		}
	}

	public void OnPauseEnd()
	{
	}

	private static float SineBobPositive(float t, float freq)
	{
		return (Mathf.Sin((t / freq + 0.75f) * (float)Math.PI * 2f) + 1f) / 2f;
	}

	private static float SineBob(float t, float freq)
	{
		return Mathf.Sin(t / freq) * (float)Math.PI * 2f;
	}

	public void OnInventoryChange()
	{
	}

	public void SetWalkSpeed(float forward, float side)
	{
		walkTargetDir.y = Mathf.Clamp(forward, -1f, 1f);
		walkTargetDir.x = Mathf.Clamp(side, -1f, 1f);
	}

	public void SetWalkSpeed(Vector2 vec)
	{
		SetWalkSpeed(vec.x, vec.y);
	}

	public void Aim(float horzAngle, float vertAngle)
	{
		vertAngle = Mathf.Clamp(vertAngle, -50f, 70f);
		aimVert = vertAngle;
		aimHorz = horzAngle;
	}

	public void PickupAmmo(int amount)
	{
	}

	public void PerformAction()
	{
	}

	public void ReloadWeapon()
	{
		if (gun != null)
		{
			gun.Reload();
		}
	}

	public void SetTriggerState(bool on)
	{
		if (gun != null)
		{
			gun.SetTriggerState(on);
		}
	}

	public void SetTouchController(TouchController joy)
	{
		touchCtrl = joy;
	}

	public void ChangeWeapon(int delta)
	{
	}

	public void PositionCamera()
	{
		if (!(playerViewCam == null))
		{
			Transform transform = playerViewCam.transform;
			if (headBone != null)
			{
				transform.position = headBone.position;
				transform.rotation = headBone.rotation;
			}
			else
			{
				transform.position = base.transform.position + camOfs;
				transform.rotation = Quaternion.Euler(aimVertDisplay, aimHorzDisplay, 0f);
			}
			if (zoomActive)
			{
				playerViewCam.fieldOfView = Mathf.Lerp(zoomFovMin, zoomFovMax, zoomFactor);
			}
			else
			{
				playerViewCam.fieldOfView = normalFov;
			}
		}
	}

	private void LateUpdate()
	{
		PositionCamera();
	}

	public void DrawGUIBG()
	{
		if (zoomActive && scopeTex != null && Event.current.type == EventType.Repaint)
		{
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), scopeTex, ScaleMode.ScaleAndCrop);
		}
	}

	public void DrawCustomGUI()
	{
		if (gun != null && Event.current.type == EventType.Repaint)
		{
			GUI.color = ((gun.bulletCount <= 0) ? Color.red : Color.white);
			GUI.depth = 1;
			GUI.Label(new Rect(Screen.width - 50, 10f, 50f, 30f), string.Empty + gun.bulletCount + "/" + gun.bulletCapacity);
			if (zoomActive)
			{
				Rect position = touchCtrl.GetZone(1).GetDisplayRect(true);
				position = new Rect(position.center.x - 50f, position.y - 30f, 100f, 30f);
				GUI.color = Color.white;
				GUI.Label(position, "ZOOM " + ((int)(zoomFactor * 100f)).ToString("00") + "%");
			}
		}
	}
}
