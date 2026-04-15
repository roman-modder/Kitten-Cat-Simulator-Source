using System;
using UnityEngine;

[RequireComponent(typeof(WheelCollider))]
public class Wheel : MonoBehaviour
{
	public Transform wheelModel;

	public Transform skidTrailPrefab;

	public static Transform skidTrailsDetachedParent;

	public float loQualDist = 100f;

	public bool steerable;

	public bool powered;

	[SerializeField]
	private float particleRate = 3f;

	[SerializeField]
	private float slideThreshold = 10f;

	private float spinAngle;

	private float particleEmit;

	private float sidewaysStiffness;

	private float forwardStiffness;

	private float spinoutFactor;

	private float sideSlideFactor;

	private float springCompression;

	private Rigidbody rb;

	private WheelFrictionCurve sidewaysFriction;

	private WheelFrictionCurve forwardFriction;

	private Transform skidTrail;

	private bool leavingSkidTrail;

	private RaycastHit hit;

	private Vector3 relativeVelocity;

	private float sideSlideFactorTarget;

	private float spinoutFactorTarget;

	private float accelAmount;

	private float burnoutFactor;

	private float burnoutGrip;

	private float spinoutGrip;

	private float sideSlideGrip;

	private float minGrip;

	private float springCompressionGripModifier;

	private float burnoutRpm;

	private float skidFactorTarget;

	private bool ignore;

	private Vector3 originalWheelModelPosition;

	public float Rpm { get; private set; }

	public float MaxRpm { get; private set; }

	public float SkidFactor { get; private set; }

	public bool OnGround { get; private set; }

	public Transform Hub { get; set; }

	public WheelCollider wheelCollider { get; private set; }

	public CarController car { get; private set; }

	public float suspensionSpringPos { get; private set; }

	private void Start()
	{
		car = base.transform.parent.GetComponent<CarController>();
		wheelCollider = GetComponent<Collider>() as WheelCollider;
		if (wheelModel != null)
		{
			originalWheelModelPosition = wheelModel.localPosition;
			base.transform.position = wheelModel.position;
		}
		sidewaysFriction = wheelCollider.sidewaysFriction;
		forwardFriction = wheelCollider.forwardFriction;
		sidewaysStiffness = wheelCollider.sidewaysFriction.stiffness;
		forwardStiffness = wheelCollider.forwardFriction.stiffness;
		MaxRpm = car.MaxSpeed / ((float)Math.PI * wheelCollider.radius * 2f) * 60f;
		rb = wheelCollider.attachedRigidbody;
		if (skidTrailsDetachedParent == null)
		{
			skidTrailsDetachedParent = new GameObject("Skid Trails - Detached").transform;
		}
	}

	private void FixedUpdate()
	{
		relativeVelocity = base.transform.InverseTransformDirection(rb.velocity);
		sideSlideFactorTarget = Mathf.Clamp01(Mathf.Abs(relativeVelocity.x * slideThreshold / car.MaxSpeed) * (car.SpeedFactor * 0.5f + 0.5f));
		sideSlideFactor = ((!(sideSlideFactorTarget > sideSlideFactor)) ? Mathf.Lerp(sideSlideFactor, sideSlideFactorTarget, Time.deltaTime) : sideSlideFactorTarget);
		spinoutFactorTarget = Mathf.Clamp01(rb.angularVelocity.magnitude * 57.29578f * 0.01f * ((1f - car.SpeedFactor) * 0.5f + 0.5f));
		spinoutFactorTarget = Mathf.Lerp(0f, spinoutFactorTarget, car.SpeedFactor + ((!powered) ? 0f : car.AccelInput));
		spinoutFactor = ((!(spinoutFactorTarget > spinoutFactor)) ? Mathf.Lerp(spinoutFactor, spinoutFactorTarget, Time.deltaTime) : spinoutFactorTarget);
		accelAmount = wheelCollider.motorTorque / car.MaxTorque;
		burnoutFactor = 0f;
		if (powered)
		{
			burnoutFactor = (accelAmount - (1f - car.BurnoutTendency)) / (1f - car.BurnoutTendency);
		}
		burnoutGrip = Mathf.Lerp(1f, 1f - car.BurnoutSlipEffect, burnoutFactor);
		spinoutGrip = Mathf.Lerp(1f, 1f - car.SpinoutSlipEffect, spinoutFactor);
		sideSlideGrip = Mathf.Lerp(1f, 1f - car.SideSlideEffect, sideSlideFactor);
		minGrip = Mathf.Min(burnoutGrip, spinoutGrip);
		minGrip = Mathf.Min(sideSlideGrip, minGrip);
		springCompressionGripModifier = springCompression + 0.6f;
		springCompressionGripModifier *= springCompressionGripModifier;
		sidewaysFriction.stiffness = sidewaysStiffness * minGrip * springCompressionGripModifier;
		forwardFriction.stiffness = forwardStiffness * burnoutGrip * springCompressionGripModifier;
		wheelCollider.sidewaysFriction = sidewaysFriction;
		wheelCollider.forwardFriction = forwardFriction;
		burnoutRpm = car.MaxSpeed * car.BurnoutTendency / ((float)Math.PI * wheelCollider.radius * 2f) * 60f;
		Rpm = ((!(burnoutRpm > wheelCollider.rpm)) ? wheelCollider.rpm : Mathf.Lerp(wheelCollider.rpm, burnoutRpm, burnoutFactor));
		if (OnGround)
		{
			skidFactorTarget = Mathf.Max(burnoutFactor * 2f, sideSlideFactor * rb.velocity.magnitude * 0.05f);
			skidFactorTarget = Mathf.Max(skidFactorTarget, spinoutFactor * rb.velocity.magnitude * 0.05f);
			skidFactorTarget = Mathf.Clamp01(-0.1f + skidFactorTarget * 1.1f);
			SkidFactor = Mathf.MoveTowards(SkidFactor, skidFactorTarget, Time.deltaTime * 2f);
		}
		if (skidTrailPrefab != null)
		{
			if (SkidFactor > 0.5f && OnGround)
			{
				if (!leavingSkidTrail)
				{
					skidTrail = UnityEngine.Object.Instantiate(skidTrailPrefab);
					if (skidTrail != null)
					{
						skidTrail.parent = base.transform;
						skidTrail.localPosition = -Vector3.up * (wheelCollider.radius - 0.1f);
					}
					leavingSkidTrail = true;
				}
			}
			else if (leavingSkidTrail)
			{
				skidTrail.parent = skidTrailsDetachedParent;
				UnityEngine.Object.Destroy(skidTrail.gameObject, 10f);
				leavingSkidTrail = false;
			}
		}
		spinAngle += Rpm * 6f * Time.deltaTime;
		float sqrMagnitude = (Camera.main.transform.position - base.transform.position).sqrMagnitude;
		bool flag = true;
		if (sqrMagnitude > loQualDist * loQualDist)
		{
			float num = Mathf.Lerp(1f, 0.2f, Mathf.InverseLerp(loQualDist * loQualDist, loQualDist * loQualDist * 4f, sqrMagnitude));
			flag = UnityEngine.Random.value < num;
		}
		if (flag)
		{
			if (Physics.Raycast(base.transform.position, -base.transform.up, out hit, wheelCollider.suspensionDistance + wheelCollider.radius))
			{
				suspensionSpringPos = 0f - (hit.distance - wheelCollider.radius);
				springCompression = Mathf.InverseLerp(0f - wheelCollider.suspensionDistance, wheelCollider.suspensionDistance, suspensionSpringPos);
				OnGround = true;
			}
			else
			{
				suspensionSpringPos = 0f - wheelCollider.suspensionDistance;
				OnGround = false;
				springCompression = 0f;
				SkidFactor = 0f;
			}
			if (wheelModel != null)
			{
				wheelModel.localPosition = originalWheelModelPosition + Vector3.up * suspensionSpringPos;
				wheelModel.localRotation = Quaternion.AngleAxis(wheelCollider.steerAngle, Vector3.up) * Quaternion.Euler(spinAngle, 0f, 0f);
			}
		}
	}
}
