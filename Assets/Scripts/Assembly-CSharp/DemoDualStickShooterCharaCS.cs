using System;
using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/DemoDualStickShooterCharaCS")]
public class DemoDualStickShooterCharaCS : MonoBehaviour
{
	public GunCS gun;

	public Transform spineBone;

	public float swayFreq = 0.3f;

	public Vector3 swayAngles = new Vector3(0f, 0f, 10f);

	public float swayFadeTime = 0.4f;

	private float swayBlend;

	private Quaternion spineInitialRot;

	public float runForwardSpeed = 6f;

	public float runSideSpeed = 4f;

	public float runBackSpeed = 3f;

	public float maxTurnSpeed = 500f;

	public float turnSmoothingTime = 0.3f;

	public float aimStickDeadZone = 0.2f;

	public float aimStickMinSpeed;

	public float aimStickMaxSpeed = 500f;

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

	public void Init(DemoDualStickShooterGameCS game)
	{
		charaCtrl = (CharacterController)base.gameObject.GetComponent(typeof(CharacterController));
		if (spineBone != null)
		{
			spineInitialRot = spineBone.localRotation;
		}
	}

	public void Move(Vector3 worldDir, float speed)
	{
		moveSpeed = Mathf.Clamp01(speed);
		if (moveSpeed < 0.001f)
		{
			worldMoveVec = Vector3.zero;
			return;
		}
		Vector3 vector = RotateVec(worldDir, 0f - orientCur);
		if (vector.z > 0f)
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

	public void Aim(float angle, float pow)
	{
		aimInputAngle = angle;
		aimInputPow = pow;
	}

	public void SetTriggerState(bool on)
	{
		isShooting = on;
	}

	public void UpdateChara()
	{
		if (gun != null)
		{
			gun.SetTriggerState(isShooting);
		}
		if (aimInputPow > aimStickDeadZone && aimInputPow > 0.0001f)
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
		base.transform.localRotation = Quaternion.Euler(0f, orientCur, 0f);
		if (charaCtrl != null)
		{
			charaCtrl.Move(worldMoveVec * Time.deltaTime);
		}
		else
		{
			base.transform.position += worldMoveVec * Time.deltaTime;
		}
	}

	public void OnPause()
	{
		Move(Vector3.zero, 0f);
		SetTriggerState(false);
	}

	public void OnUnpause()
	{
	}

	private static Vector3 RotateVec(Vector3 vec, float angle)
	{
		return Quaternion.Euler(0f, angle, 0f) * vec;
	}
}
