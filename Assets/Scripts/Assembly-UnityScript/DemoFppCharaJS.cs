using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/DemoFppCharaJS")]
public class DemoFppCharaJS : MonoBehaviour
{
	public Camera playerViewCam;

	public CharacterController charaCtrl;

	public Texture2D scopeTex;

	public GunJS gun;

	public Transform headBone;

	public Transform upperBodyBone;

	public Transform gunBone;

	public float normalFov;

	public float zoomFovMin;

	public float zoomFovMax;

	private bool zoomActive;

	private float zoomFactor;

	public float zoomFactorPerCm;

	public float walkSpeed;

	public float strafeSpeed;

	public float aimSensitivity;

	public float horzAimSpeed;

	public float vertAimSpeed;

	public Vector3 camOfs;

	private float aimHorz;

	private float aimVert;

	private float aimHorzDisplay;

	private float aimVertDisplay;

	public float aimSmoothingTime;

	private float aimHorzDampVel;

	private float aimVertDampVel;

	private bool isAiming;

	private float aimVertRaw;

	private float aimHorzRaw;

	private bool isZoomDragging;

	private float zoomFactorRaw;

	[NonSerialized]
	private static float WALK_DEADZONE = 0.1f;

	private Vector2 walkTargetDir;

	private Vector2 walkCurDir;

	private bool walkingCur;

	private bool walkingPrev;

	private float walkElapsed;

	public float walkVertBobScale;

	public float walkVertBobFreq;

	public float walkSmoothingTime;

	private Vector3 walkDirDampVel;

	private Vector3 walkBobVec;

	private Vector3 upperBodyInitialPos;

	private TouchController touchCtrl;

	[NonSerialized]
	private static float MIN_AIM_ANGLE = -50f;

	[NonSerialized]
	private static float MAX_AIM_ANGLE = 70f;

	public DemoFppCharaJS()
	{
		normalFov = 40f;
		zoomFovMin = 20f;
		zoomFovMax = 10f;
		zoomFactorPerCm = 0.5f;
		walkSpeed = 8f;
		strafeSpeed = 5f;
		aimSensitivity = 1f;
		horzAimSpeed = 20f;
		vertAimSpeed = 10f;
		camOfs = new Vector3(0f, 1.6f, 0f);
		aimSmoothingTime = 0.1f;
		walkVertBobScale = 0.1f;
		walkVertBobFreq = 0.5f;
		walkSmoothingTime = 0.2f;
	}

	public virtual void Start()
	{
		if (charaCtrl == null)
		{
			charaCtrl = (CharacterController)gameObject.GetComponent(typeof(CharacterController));
		}
		if (upperBodyBone != null)
		{
			upperBodyInitialPos = upperBodyBone.localPosition;
		}
	}

	public virtual void ControlByTouch()
	{
		if (touchCtrl == null)
		{
			return;
		}
		TouchStick stick = touchCtrl.GetStick(DemoFppGameJS.STICK_WALK);
		TouchZone zone = touchCtrl.GetZone(DemoFppGameJS.ZONE_AIM);
		TouchZone zone2 = touchCtrl.GetZone(DemoFppGameJS.ZONE_FIRE);
		TouchZone zone3 = touchCtrl.GetZone(DemoFppGameJS.ZONE_ZOOM);
		TouchZone zone4 = touchCtrl.GetZone(DemoFppGameJS.ZONE_RELOAD);
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

	public virtual void UpdateChara()
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
		walkingCur = magnitude > WALK_DEADZONE;
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
			transform.position += vector;
		}
		transform.localRotation = Quaternion.Euler(0f, aimHorzDisplay, 0f);
		if (upperBodyBone != null)
		{
			upperBodyBone.localRotation = Quaternion.Euler(aimVertDisplay, 0f, 0f);
			upperBodyBone.localPosition = upperBodyInitialPos + walkBobVec;
		}
	}

	public virtual void OnPauseStart()
	{
		if (gun != null)
		{
			gun.SetTriggerState(false);
		}
	}

	public virtual void OnPauseEnd()
	{
	}

	private static float SineBobPositive(float t, float freq)
	{
		return (Mathf.Sin((t / freq + 0.75f) * (float)Math.PI * 2f) + 1f) / 2f;
	}

	public virtual void OnInventoryChange()
	{
	}

	public virtual void SetWalkSpeed(float forward, float side)
	{
		walkTargetDir.y = Mathf.Clamp(forward, -1f, 1f);
		walkTargetDir.x = Mathf.Clamp(side, -1f, 1f);
	}

	public virtual void SetWalkSpeed(Vector2 vec)
	{
		SetWalkSpeed(vec.x, vec.y);
	}

	public virtual void Aim(float horzAngle, float vertAngle)
	{
		vertAngle = Mathf.Clamp(vertAngle, MIN_AIM_ANGLE, MAX_AIM_ANGLE);
		aimVert = vertAngle;
		aimHorz = horzAngle;
	}

	public virtual void PickupAmmo(int amount)
	{
	}

	public virtual void PerformAction()
	{
	}

	public virtual void ReloadWeapon()
	{
		if (gun != null)
		{
			gun.Reload();
		}
	}

	public virtual void SetTriggerState(bool triggerOn)
	{
		if (gun != null)
		{
			gun.SetTriggerState(triggerOn);
		}
	}

	public virtual void SetTouchController(TouchController joy)
	{
		touchCtrl = joy;
	}

	public virtual void ChangeWeapon(int delta)
	{
	}

	public virtual void PositionCamera()
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
				transform.position = this.transform.position + camOfs;
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

	public virtual void LateUpdate()
	{
		PositionCamera();
	}

	public virtual void DrawGUIBG()
	{
		if (zoomActive && scopeTex != null && Event.current.type == EventType.Repaint)
		{
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), scopeTex, ScaleMode.ScaleAndCrop);
		}
	}

	public virtual void DrawCustomGUI()
	{
		if (touchCtrl != null)
		{
		}
		if (gun != null && Event.current.type == EventType.Repaint)
		{
			GUI.color = ((gun.bulletCount <= 0) ? Color.red : Color.white);
			GUI.depth = 1;
			GUI.Label(new Rect(Screen.width - 50, 10f, 50f, 30f), string.Empty + gun.bulletCount + "/" + gun.bulletCapacity);
			if (zoomActive)
			{
				float num = 100f;
				float num2 = 30f;
				Rect displayRect = touchCtrl.GetZone(DemoFppGameJS.ZONE_ZOOM).GetDisplayRect(true);
				displayRect = new Rect(displayRect.center.x - num * 0.5f, displayRect.y - num2, num, num2);
				GUI.color = Color.white;
				GUI.Label(displayRect, "ZOOM " + Mathf.FloorToInt(zoomFactor * 100f).ToString("00") + "%");
			}
		}
	}

	public virtual void Main()
	{
	}
}
