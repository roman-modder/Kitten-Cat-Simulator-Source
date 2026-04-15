using UnityEngine;

public class BrakeLight : MonoBehaviour
{
	public CarController car;

	private void Update()
	{
		GetComponent<Renderer>().enabled = car.BrakeInput > 0f;
	}
}
