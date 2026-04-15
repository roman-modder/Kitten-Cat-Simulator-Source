using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/DemoDualStickShooterCharaJS")]
public class DemoDualStickShooterCharaJS : MonoBehaviour
{
	public GunJS gun;

	public Transform spineBone;

	public float swayFreq;

	public Vector3 swayAngles;

	public float swayFadeTime;

	private float swayBlend;

	private Quaternion spineInitialRot;

	public float runForwardSpeed;

	public float runSideSpeed;

	public float runBackSpeed;

	public float maxTurnSpeed;

	public float turnSmoothingTime;

	public float aimStickDeadZone;

	public float aimStickMinSpeed;

	public float aimStickMaxSpeed;

	private float aimInputAngle;

	private float aimInputPow;

	private bool isShooting;

	private bool isWalking;

	private float orientCur;

	private float orientTarget;

	private float orientVel;

	private float moveSpeed;

	private Vector3 worldMoveVec;

	private CharacterController charaCtrl;

	public DemoDualStickShooterCharaJS()
	{
		swayFreq = 0.3f;
		swayAngles = new Vector3(0f, 0f, 10f);
		swayFadeTime = 0.4f;
		runForwardSpeed = 6f;
		runSideSpeed = 4f;
		runBackSpeed = 3f;
		maxTurnSpeed = 500f;
		turnSmoothingTime = 0.3f;
		aimStickDeadZone = 0.2f;
		aimStickMaxSpeed = 500f;
	}

	public virtual void Init(DemoDualStickShooterGameJS game)
	{
		charaCtrl = (CharacterController)gameObject.GetComponent(typeof(CharacterController));
		if (spineBone != null)
		{
			spineInitialRot = spineBone.localRotation;
		}
	}

	public virtual void Move(Vector3 worldDir, float speed)
	{
		moveSpeed = Mathf.Clamp01(speed);
		if (!(moveSpeed >= 0.001f))
		{
			worldMoveVec = Vector3.zero;
			return;
		}
		Vector3 vector = RotateVec(worldDir, 0f - orientCur);
		if (!(vector.z <= 0f))
		{
			vector.z *= runForwardSpeed;
		}
		else
		{
			vector.z *= runBackSpeed;
		}
		vector.x *= runSideSpeed;
		worldMoveVec = RotateVec(vector * speed, orientCur);
	}

	public virtual void Aim(float angle, float pow)
	{
		aimInputAngle = angle;
		aimInputPow = pow;
	}

	public virtual void SetTriggerState(bool on)
	{
		isShooting = on;
	}

	public virtual void UpdateChara()
	{
		if (gun != null)
		{
			gun.SetTriggerState(isShooting);
		}
		if (!(aimInputPow <= aimStickDeadZone) && !(aimInputPow <= 0.0001f))
		{
			float t = Mathf.Clamp01((aimInputPow - aimStickDeadZone) / (1f - aimStickDeadZone));
			orientTarget = Mathf.MoveTowardsAngle(orientTarget, aimInputAngle, Time.deltaTime * Mathf.Lerp(aimStickMinSpeed, aimStickMaxSpeed, t));
		}
		orientCur = Mathf.SmoothDampAngle(orientCur, orientTarget, ref orientVel, turnSmoothingTime * 0.2f, maxTurnSpeed);
		swayBlend = Mathf.MoveTowards(swayBlend, moveSpeed, Time.deltaTime * (1f / swayFadeTime));
		if (spineBone != null)
		{
			spineBone.localRotation = spineInitialRot * Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(swayAngles * Mathf.Sin((float)Math.PI * (Time.time / swayFreq))), swayBlend);
		}
		transform.localRotation = Quaternion.Euler(0f, orientCur, 0f);
		if (charaCtrl != null)
		{
			charaCtrl.Move(worldMoveVec * Time.deltaTime);
		}
		else
		{
			transform.position += worldMoveVec * Time.deltaTime;
		}
	}

	public virtual void OnPause()
	{
		Move(Vector3.zero, 0f);
		SetTriggerState(false);
	}

	public virtual void OnUnpause()
	{
	}

	private static Vector3 RotateVec(Vector3 vec, float angle)
	{
		return Quaternion.Euler(0f, angle, 0f) * vec;
	}

	public virtual void Main()
	{
	}
}
