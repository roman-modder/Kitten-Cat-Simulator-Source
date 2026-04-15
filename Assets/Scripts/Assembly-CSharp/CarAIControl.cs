using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarAIControl : MonoBehaviour
{
	public enum BrakeCondition
	{
		NeverBrake = 0,
		TargetDirectionDifference = 1,
		TargetDistance = 2
	}

	[SerializeField]
	[Range(0f, 1f)]
	private float cautiousSpeedFactor = 0.05f;

	[SerializeField]
	[Range(0f, 180f)]
	private float cautiousMaxAngle = 50f;

	[SerializeField]
	private float cautiousMaxDistance = 100f;

	[SerializeField]
	private float cautiousAngularVelocityFactor = 30f;

	[SerializeField]
	private float steerSensitivity = 0.05f;

	[SerializeField]
	private float accelSensitivity = 0.04f;

	[SerializeField]
	private float brakeSensitivity = 1f;

	[SerializeField]
	private float lateralWanderDistance = 3f;

	[SerializeField]
	private float lateralWanderSpeed = 0.1f;

	[SerializeField]
	[Range(0f, 1f)]
	public float accelWanderAmount = 0.1f;

	[SerializeField]
	private float accelWanderSpeed = 0.1f;

	[SerializeField]
	private BrakeCondition brakeCondition = BrakeCondition.TargetDistance;

	[SerializeField]
	private bool driving;

	[SerializeField]
	private Transform target;

	[SerializeField]
	private bool stopWhenTargetReached;

	[SerializeField]
	private float reachTargetThreshold = 2f;

	private float randomPerlin;

	private CarController carController;

	private float avoidOtherCarTime;

	private float avoidOtherCarSlowdown;

	private float avoidPathOffset;

	private void Awake()
	{
		carController = GetComponent<CarController>();
		randomPerlin = Random.value * 100f;
	}

	private void FixedUpdate()
	{
		if (target == null || !driving)
		{
			float accelBrakeInput = Mathf.Clamp(0f - carController.CurrentSpeed, -1f, 1f);
			carController.Move(0f, accelBrakeInput);
			return;
		}
		Vector3 to = base.transform.forward;
		if (GetComponent<Rigidbody>().velocity.magnitude > carController.MaxSpeed * 0.1f)
		{
			to = GetComponent<Rigidbody>().velocity;
		}
		float num = carController.MaxSpeed;
		switch (brakeCondition)
		{
		case BrakeCondition.TargetDirectionDifference:
		{
			float b2 = Vector3.Angle(target.forward, to);
			float a = GetComponent<Rigidbody>().angularVelocity.magnitude * cautiousAngularVelocityFactor;
			float t2 = Mathf.InverseLerp(0f, cautiousMaxAngle, Mathf.Max(a, b2));
			num = Mathf.Lerp(carController.MaxSpeed, carController.MaxSpeed * cautiousSpeedFactor, t2);
			break;
		}
		case BrakeCondition.TargetDistance:
		{
			Vector3 vector = target.position - base.transform.position;
			float b = Mathf.InverseLerp(cautiousMaxDistance, 0f, vector.magnitude);
			float value = GetComponent<Rigidbody>().angularVelocity.magnitude * cautiousAngularVelocityFactor;
			float t = Mathf.Max(Mathf.InverseLerp(0f, cautiousMaxAngle, value), b);
			num = Mathf.Lerp(carController.MaxSpeed, carController.MaxSpeed * cautiousSpeedFactor, t);
			break;
		}
		}
		Vector3 position = target.position;
		if (Time.time < avoidOtherCarTime)
		{
			num *= avoidOtherCarSlowdown;
			position += target.right * avoidPathOffset;
		}
		else
		{
			position += target.right * (Mathf.PerlinNoise(Time.time * lateralWanderSpeed, randomPerlin) * 2f - 1f) * lateralWanderDistance;
		}
		float num2 = ((!(num < carController.CurrentSpeed)) ? accelSensitivity : brakeSensitivity);
		float num3 = Mathf.Clamp((num - carController.CurrentSpeed) * num2, -1f, 1f);
		num3 *= 1f - accelWanderAmount + Mathf.PerlinNoise(Time.time * accelWanderSpeed, randomPerlin) * accelWanderAmount;
		Vector3 vector2 = base.transform.InverseTransformPoint(position);
		float num4 = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
		float steerInput = Mathf.Clamp(num4 * steerSensitivity, -1f, 1f) * Mathf.Sign(carController.CurrentSpeed);
		carController.Move(steerInput, num3);
		if (stopWhenTargetReached && vector2.magnitude < reachTargetThreshold)
		{
			driving = false;
		}
	}

	private void OnCollisionStay(Collision col)
	{
		if (!(col.rigidbody != null))
		{
			return;
		}
		CarAIControl component = col.rigidbody.GetComponent<CarAIControl>();
		if (component != null)
		{
			avoidOtherCarTime = Time.time + 1f;
			if (Vector3.Angle(base.transform.forward, component.transform.position - base.transform.position) < 90f)
			{
				avoidOtherCarSlowdown = 0.5f;
			}
			else
			{
				avoidOtherCarSlowdown = 1f;
			}
			Vector3 vector = base.transform.InverseTransformPoint(component.transform.position);
			float f = Mathf.Atan2(vector.x, vector.z);
			avoidPathOffset = lateralWanderDistance * (0f - Mathf.Sign(f));
		}
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
		driving = true;
	}
}
