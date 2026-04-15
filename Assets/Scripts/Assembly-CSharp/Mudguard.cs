using UnityEngine;

public class Mudguard : MonoBehaviour
{
	public Wheel wheel;

	private Quaternion originalRotation;

	private Vector3 offset;

	private void Start()
	{
		originalRotation = base.transform.localRotation;
		offset = base.transform.position - wheel.wheelModel.transform.position;
	}

	private void Update()
	{
		base.transform.localRotation = originalRotation * Quaternion.Euler(0f, wheel.car.CurrentSteerAngle, 0f);
		base.transform.position = wheel.wheelModel.transform.position + offset;
	}
}
