using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarUserControl : MonoBehaviour
{
	private CarController car;

	private void Awake()
	{
		car = GetComponent<CarController>();
	}

	private void FixedUpdate()
	{
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		car.Move(axis, axis2);
	}
}
