using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/DemoRpgCharaJS")]
public class DemoRpgCharaJS : MonoBehaviour
{
	[Serializable]
	public enum CharaState
	{
		IDLE = 0,
		WALK = 1,
		RUN = 2,
		USE = 3,
		NONE = 4
	}

	public GunJS gun;

	public float walkSpeed;

	public float runSpeed;

	public float walkAnimSpeed;

	public float runAnimSpeed;

	public float walkStickThreshold;

	public float runStickThreshold;

	public float angle;

	public float angleSmoothingTime;

	public float turnMaxSpeed;

	private Animation charaAnim;

	private CharacterController charaCtrl;

	private CharaState charaState;

	private float useAnimDuration;

	private float useElapsed;

	public AudioClip actionSound;

	public string ANIM_IDLE;

	public string ANIM_WALK_F;

	public string ANIM_RUN_F;

	public string ANIM_USE;

	public DemoRpgCharaJS()
	{
		walkSpeed = 7f;
		runSpeed = 15f;
		walkAnimSpeed = 0.8f;
		runAnimSpeed = 1.6f;
		walkStickThreshold = 0.2f;
		runStickThreshold = 0.9f;
		angleSmoothingTime = 0.6f;
		turnMaxSpeed = 400f;
		charaState = CharaState.NONE;
		useAnimDuration = 1f;
		ANIM_IDLE = "idle";
		ANIM_WALK_F = "run_forward";
		ANIM_RUN_F = "run_forward";
		ANIM_USE = "use";
	}

	public virtual void Start()
	{
		charaAnim = GetComponent<Animation>();
		if (charaAnim == null)
		{
			Debug.LogError("Character Animation Component is not available!");
		}
		charaCtrl = GetComponent<Collider>() as CharacterController;
		if (charaCtrl == null)
		{
			Debug.LogError("CharacterController is not assigned!");
		}
		if (charaAnim != null && charaAnim.GetClip(ANIM_USE) != null)
		{
			charaAnim[ANIM_USE].layer = 2;
			useAnimDuration = charaAnim.GetClip(ANIM_USE).length;
		}
		StartIdle();
	}

	public virtual void UpdateChara()
	{
		transform.localRotation = Quaternion.Euler(0f, angle, 0f);
	}

	public virtual void ControlByTouch(TouchController ctrl, DemoRpgGameJS game)
	{
		TouchStick stick = ctrl.GetStick(DemoRpgGameJS.STICK_WALK);
		TouchZone zone = ctrl.GetZone(DemoRpgGameJS.ZONE_SCREEN);
		TouchZone zone2 = ctrl.GetZone(DemoRpgGameJS.ZONE_ACTION);
		TouchZone zone3 = ctrl.GetZone(DemoRpgGameJS.ZONE_FIRE);
		Vector3 vec3d = stick.GetVec3d(TouchStick.Vec3DMode.XZ, true, game.camOrbitalAngle);
		float target = stick.GetAngle() + game.camOrbitalAngle;
		float tilt = stick.GetTilt();
		bool triggerState = zone3.UniPressed(true, false);
		bool num = zone2.JustUniPressed(true, true);
		if (!num)
		{
			num = zone.JustTapped();
		}
		bool flag = num;
		if (charaState == CharaState.IDLE || charaState == CharaState.WALK || charaState == CharaState.RUN)
		{
			if (!(tilt <= walkStickThreshold))
			{
				float num2 = 0f;
				if (!(tilt >= runStickThreshold))
				{
					num2 = walkSpeed;
					if (charaState != CharaState.WALK)
					{
						StartWalking();
					}
					charaAnim[ANIM_WALK_F].speed = walkAnimSpeed;
				}
				else
				{
					num2 = runSpeed;
					if (charaState != CharaState.RUN)
					{
						StartRunning();
					}
					charaAnim[ANIM_RUN_F].speed = runAnimSpeed;
				}
				angle = DampAngle(angle, target, angleSmoothingTime, turnMaxSpeed, Time.deltaTime);
				charaCtrl.Move(vec3d * num2 * Time.deltaTime);
			}
			else if (charaState == CharaState.WALK || charaState == CharaState.RUN)
			{
				StartIdle();
			}
			if (flag)
			{
				PerformUseAction();
			}
		}
		if (charaState != CharaState.USE)
		{
			if (gun != null)
			{
				gun.SetTriggerState(triggerState);
			}
			if (flag)
			{
				PerformUseAction();
			}
			return;
		}
		if (gun != null)
		{
			gun.SetTriggerState(false);
		}
		useElapsed += Time.deltaTime;
		if (!(useElapsed <= useAnimDuration))
		{
			StartIdle();
		}
	}

	public virtual void StartIdle()
	{
		charaState = CharaState.IDLE;
		charaAnim.CrossFade(ANIM_IDLE, 0.3f);
	}

	public virtual void StartWalking()
	{
		charaState = CharaState.WALK;
		charaAnim.CrossFade(ANIM_WALK_F, 0.3f);
	}

	public virtual void StartRunning()
	{
		charaState = CharaState.RUN;
		charaAnim.CrossFade(ANIM_RUN_F, 0.3f);
	}

	public virtual void PerformUseAction()
	{
		charaState = CharaState.USE;
		charaAnim.CrossFade(ANIM_IDLE, 0.3f);
		charaAnim.CrossFade(ANIM_USE, 0.3f, PlayMode.StopSameLayer);
		useElapsed = 0f;
		if (actionSound != null && GetComponent<AudioSource>() != null)
		{
			GetComponent<AudioSource>().PlayOneShot(actionSound);
		}
	}

	private static float DampAngle(float cur, float target, float smoothingTime, float maxSpeed, float dt)
	{
		float num = Mathf.DeltaAngle(cur, target);
		smoothingTime *= 0.2f;
		if (!(dt >= smoothingTime))
		{
			num = Mathf.Lerp(0f, num, dt / smoothingTime);
		}
		maxSpeed *= dt;
		if (!(num <= maxSpeed))
		{
			num = maxSpeed;
		}
		else if (!(num >= 0f - maxSpeed))
		{
			num = 0f - maxSpeed;
		}
		return cur + num;
	}

	public virtual void Main()
	{
	}
}
