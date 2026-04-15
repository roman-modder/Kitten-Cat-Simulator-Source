using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/DemoRpgCharaCS")]
public class DemoRpgCharaCS : MonoBehaviour
{
	public enum CharaState
	{
		IDLE = 0,
		WALK = 1,
		RUN = 2,
		USE = 3,
		NONE = 4
	}

	public GunCS gun;

	public float walkSpeed = 7f;

	public float runSpeed = 15f;

	public float walkAnimSpeed = 0.8f;

	public float runAnimSpeed = 1.6f;

	public float walkStickThreshold = 0.2f;

	public float runStickThreshold = 0.9f;

	public float angle;

	public float angleSmoothingTime = 0.6f;

	public float turnMaxSpeed = 400f;

	private Animation charaAnim;

	private CharacterController charaCtrl;

	private CharaState charaState = CharaState.NONE;

	private float useAnimDuration = 1f;

	private float useElapsed;

	public AudioClip actionSound;

	public string ANIM_IDLE = "idle";

	public string ANIM_WALK_F = "run_forward";

	public string ANIM_RUN_F = "run_forward";

	public string ANIM_USE = "use";

	private void Start()
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

	public void ControlByTouch(TouchController ctrl, DemoRpgGameCS game)
	{
		TouchStick stick = ctrl.GetStick(0);
		TouchZone zone = ctrl.GetZone(0);
		TouchZone zone2 = ctrl.GetZone(2);
		TouchZone zone3 = ctrl.GetZone(1);
		Vector3 vec3d = stick.GetVec3d(TouchStick.Vec3DMode.XZ, true, game.camOrbitalAngle);
		float target = stick.GetAngle() + game.camOrbitalAngle;
		float tilt = stick.GetTilt();
		bool triggerState = zone3.UniPressed(true, false);
		bool flag = zone2.JustUniPressed(true, true) || zone.JustTapped();
		if (charaState == CharaState.IDLE || charaState == CharaState.WALK || charaState == CharaState.RUN)
		{
			if (tilt > walkStickThreshold)
			{
				float num = 0f;
				if (tilt < runStickThreshold)
				{
					num = walkSpeed;
					if (charaState != CharaState.WALK)
					{
						StartWalking();
					}
					charaAnim[ANIM_WALK_F].speed = walkAnimSpeed;
				}
				else
				{
					num = runSpeed;
					if (charaState != CharaState.RUN)
					{
						StartRunning();
					}
					charaAnim[ANIM_RUN_F].speed = runAnimSpeed;
				}
				angle = DampAngle(angle, target, angleSmoothingTime, turnMaxSpeed, Time.deltaTime);
				charaCtrl.Move(vec3d * num * Time.deltaTime);
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
		if (useElapsed > useAnimDuration)
		{
			StartIdle();
		}
	}

	private static float DampAngle(float cur, float target, float smoothingTime, float maxSpeed, float dt)
	{
		float num = Mathf.DeltaAngle(cur, target);
		smoothingTime *= 0.2f;
		if (dt < smoothingTime)
		{
			num = Mathf.Lerp(0f, num, dt / smoothingTime);
		}
		maxSpeed *= dt;
		if (num > maxSpeed)
		{
			num = maxSpeed;
		}
		else if (num < 0f - maxSpeed)
		{
			num = 0f - maxSpeed;
		}
		return cur + num;
	}

	public void StartIdle()
	{
		charaState = CharaState.IDLE;
		charaAnim.CrossFade(ANIM_IDLE, 0.3f);
	}

	public void StartWalking()
	{
		charaState = CharaState.WALK;
		charaAnim.CrossFade(ANIM_WALK_F, 0.3f);
	}

	public void StartRunning()
	{
		charaState = CharaState.RUN;
		charaAnim.CrossFade(ANIM_RUN_F, 0.3f);
	}

	public void PerformUseAction()
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

	public void UpdateChara()
	{
		base.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
	}
}
