using UnityEngine;

public class CarSelfRighting : MonoBehaviour
{
	[SerializeField]
	private float waitTime = 3f;

	[SerializeField]
	private float velocityThreshold = 1f;

	private float lastOkTime;

	private void Update()
	{
		if (base.transform.up.y > 0f || GetComponent<Rigidbody>().velocity.magnitude > velocityThreshold)
		{
			lastOkTime = Time.time;
		}
		if (Time.time > lastOkTime + waitTime)
		{
			RightCar();
		}
	}

	private void RightCar()
	{
		base.transform.position += Vector3.up;
		base.transform.rotation = Quaternion.LookRotation(base.transform.forward);
	}
}
