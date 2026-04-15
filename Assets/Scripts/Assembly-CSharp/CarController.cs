using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
	[Serializable]
	public class Advanced
	{
		[Range(0f, 1f)]
		public float burnoutSlipEffect = 0.4f;

		[Range(0f, 1f)]
		public float burnoutTendency = 0.2f;

		[Range(0f, 1f)]
		public float spinoutSlipEffect = 0.5f;

		[Range(0f, 1f)]
		public float sideSlideEffect = 0.5f;

		public float downForce = 30f;

		public int numGears = 5;

		[Range(0f, 1f)]
		public float gearDistributionBias = 0.2f;

		public float steeringCorrection = 2f;

		public float oppositeLockSteeringCorrection = 4f;

		public float reversingSpeedFactor = 0.3f;

		public float skidGearLockFactor = 0.1f;

		public float accelChangeSmoothing = 2f;

		public float gearFactorSmoothing = 5f;

		[Range(0f, 1f)]
		public float revRangeBoundary = 0.8f;
	}

	[SerializeField]
	private float maxSteerAngle = 28f;

	[SerializeField]
	private float steeringResponseSpeed = 200f;

	[SerializeField]
	[Range(0f, 1f)]
	private float maxSpeedSteerAngle = 0.23f;

	[SerializeField]
	[Range(0f, 0.5f)]
	private float maxSpeedSteerResponse = 0.5f;

	[SerializeField]
	private float maxSpeed = 60f;

	[SerializeField]
	private float maxTorque = 35f;

	[SerializeField]
	private float minTorque = 10f;

	[SerializeField]
	private float brakePower = 40f;

	[SerializeField]
	private float adjustCentreOfMass = 0.25f;

	[SerializeField]
	private Advanced advanced;

	[SerializeField]
	private bool preserveDirectionWhileInAir;

	private float[] gearDistribution;

	private Wheel[] wheels;

	private float accelBrake;

	private float smallSpeed;

	private float maxReversingSpeed;

	private bool immobilized;

	private bool anyOnGround;

	private float curvedSpeedFactor;

	private bool reversing;

	private float targetAccelInput;

	public int GearNum { get; private set; }

	public float CurrentSpeed { get; private set; }

	public float CurrentSteerAngle { get; private set; }

	public float AccelInput { get; private set; }

	public float BrakeInput { get; private set; }

	public float GearFactor { get; private set; }

	public float AvgPowerWheelRpmFactor { get; private set; }

	public float AvgSkid { get; private set; }

	public float RevsFactor { get; private set; }

	public float SpeedFactor { get; private set; }

	public int NumGears
	{
		get
		{
			return advanced.numGears;
		}
	}

	public float MaxSpeed
	{
		get
		{
			return maxSpeed;
		}
	}

	public float MaxTorque
	{
		get
		{
			return maxTorque;
		}
	}

	public float BurnoutSlipEffect
	{
		get
		{
			return advanced.burnoutSlipEffect;
		}
	}

	public float BurnoutTendency
	{
		get
		{
			return advanced.burnoutTendency;
		}
	}

	public float SpinoutSlipEffect
	{
		get
		{
			return advanced.spinoutSlipEffect;
		}
	}

	public float SideSlideEffect
	{
		get
		{
			return advanced.sideSlideEffect;
		}
	}

	public float MaxSteerAngle
	{
		get
		{
			return maxSteerAngle;
		}
	}

	private void Awake()
	{
		wheels = GetComponentsInChildren<Wheel>();
		SetUpGears();
		base.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
		smallSpeed = maxSpeed * 0.05f;
		maxReversingSpeed = maxSpeed * advanced.reversingSpeedFactor;
		SpeedFactor = 0f;
	}

	private void OnEnable()
	{
		GetComponent<Rigidbody>().centerOfMass = Vector3.up * adjustCentreOfMass;
	}

	public void Move(float steerInput, float accelBrakeInput)
	{
		if (immobilized)
		{
			accelBrakeInput = 0f;
		}
		ConvertInputToAccelerationAndBraking(accelBrakeInput);
		CalculateSpeedValues();
		HandleGearChanging();
		CalculateGearFactor();
		ProcessWheels(steerInput);
		ApplyDownforce();
		CalculateRevs();
		PreserveDirectionInAir();
	}

	private void ConvertInputToAccelerationAndBraking(float accelBrakeInput)
	{
		reversing = false;
		if (accelBrakeInput > 0f)
		{
			if (CurrentSpeed > 0f - smallSpeed)
			{
				targetAccelInput = accelBrakeInput;
				BrakeInput = 0f;
			}
			else
			{
				BrakeInput = accelBrakeInput;
				targetAccelInput = 0f;
			}
		}
		else if (CurrentSpeed > smallSpeed)
		{
			BrakeInput = 0f - accelBrakeInput;
			targetAccelInput = 0f;
		}
		else
		{
			BrakeInput = 0f;
			targetAccelInput = accelBrakeInput;
			reversing = true;
		}
		AccelInput = Mathf.MoveTowards(AccelInput, targetAccelInput, Time.deltaTime * advanced.accelChangeSmoothing);
	}

	private void CalculateSpeedValues()
	{
		CurrentSpeed = base.transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z;
		SpeedFactor = Mathf.InverseLerp(0f, (!reversing) ? maxSpeed : maxReversingSpeed, Mathf.Abs(CurrentSpeed));
		curvedSpeedFactor = ((!reversing) ? CurveFactor(SpeedFactor) : 0f);
	}

	private void HandleGearChanging()
	{
		if (!reversing)
		{
			if (SpeedFactor < gearDistribution[GearNum] && GearNum > 0)
			{
				GearNum--;
			}
			if (SpeedFactor > gearDistribution[GearNum + 1] && AvgSkid < advanced.skidGearLockFactor && GearNum < advanced.numGears - 1)
			{
				GearNum++;
			}
		}
	}

	private void CalculateGearFactor()
	{
		float b = Mathf.InverseLerp(gearDistribution[GearNum], gearDistribution[GearNum + 1], Mathf.Abs(AvgPowerWheelRpmFactor));
		GearFactor = Mathf.Lerp(GearFactor, b, Time.deltaTime * advanced.gearFactorSmoothing);
	}

	private void ProcessWheels(float steerInput)
	{
		AvgPowerWheelRpmFactor = 0f;
		AvgSkid = 0f;
		int num = 0;
		anyOnGround = false;
		Wheel[] array = wheels;
		foreach (Wheel wheel in array)
		{
			WheelCollider wheelCollider = wheel.wheelCollider;
			if (wheel.steerable)
			{
				float num2 = Mathf.Lerp(steeringResponseSpeed, steeringResponseSpeed * maxSpeedSteerResponse, curvedSpeedFactor);
				float num3 = Mathf.Lerp(maxSteerAngle, maxSteerAngle * maxSpeedSteerAngle, curvedSpeedFactor);
				if (steerInput == 0f)
				{
					num2 *= advanced.steeringCorrection;
				}
				if (Mathf.Sign(steerInput) != Mathf.Sign(CurrentSteerAngle))
				{
					num2 *= advanced.oppositeLockSteeringCorrection;
				}
				CurrentSteerAngle = Mathf.MoveTowards(CurrentSteerAngle, steerInput * num3, Time.deltaTime * num2);
				wheelCollider.steerAngle = CurrentSteerAngle;
			}
			AvgSkid += wheel.SkidFactor;
			if (wheel.powered)
			{
				float num4 = Mathf.Lerp(maxTorque, (!(SpeedFactor < 1f)) ? 0f : minTorque, (!reversing) ? curvedSpeedFactor : SpeedFactor);
				wheelCollider.motorTorque = AccelInput * num4;
				AvgPowerWheelRpmFactor += wheel.Rpm / wheel.MaxRpm;
				num++;
			}
			wheelCollider.brakeTorque = BrakeInput * brakePower;
			if (wheel.OnGround)
			{
				anyOnGround = true;
			}
		}
		AvgPowerWheelRpmFactor /= num;
		AvgSkid /= wheels.Length;
	}

	private void ApplyDownforce()
	{
		if (anyOnGround)
		{
			GetComponent<Rigidbody>().AddForce(-base.transform.up * curvedSpeedFactor * advanced.downForce);
		}
	}

	private void CalculateRevs()
	{
		float num = (float)GearNum / (float)NumGears;
		float num2 = ULerp(0f, advanced.revRangeBoundary, CurveFactor(num));
		float to = ULerp(advanced.revRangeBoundary, 1f, num);
		RevsFactor = ULerp(num2, to, GearFactor);
	}

	private void PreserveDirectionInAir()
	{
		if (!anyOnGround && preserveDirectionWhileInAir && GetComponent<Rigidbody>().velocity.magnitude > smallSpeed)
		{
			GetComponent<Rigidbody>().MoveRotation(Quaternion.Slerp(GetComponent<Rigidbody>().rotation, Quaternion.LookRotation(GetComponent<Rigidbody>().velocity), Time.deltaTime));
			GetComponent<Rigidbody>().angularVelocity = Vector3.Lerp(GetComponent<Rigidbody>().angularVelocity, Vector3.zero, Time.deltaTime);
		}
	}

	private float CurveFactor(float factor)
	{
		return 1f - (1f - factor) * (1f - factor);
	}

	private float ULerp(float from, float to, float value)
	{
		return (1f - value) * from + value * to;
	}

	private void SetUpGears()
	{
		gearDistribution = new float[advanced.numGears + 1];
		for (int i = 0; i <= advanced.numGears; i++)
		{
			float num = (float)i / (float)advanced.numGears;
			float b = num * num * num;
			float b2 = 1f - (1f - num) * (1f - num) * (1f - num);
			num = ((!(advanced.gearDistributionBias < 0.5f)) ? Mathf.Lerp(num, b2, (advanced.gearDistributionBias - 0.5f) * 2f) : Mathf.Lerp(num, b, 1f - advanced.gearDistributionBias * 2f));
			gearDistribution[i] = num;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(GetComponent<Rigidbody>().position + Vector3.up * adjustCentreOfMass, 0.2f);
	}

	public void Immobilize()
	{
		immobilized = true;
	}

	public void Reset()
	{
		immobilized = false;
	}
}
