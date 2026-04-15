using UnityEngine;

public class CarDrive : MonoBehaviour
{
	public GameObject player;

	public Transform catTf;

	public bool isDriving;

	private CarController ctrl;

	private CarAudio audio;

	private void Start()
	{
		ctrl = GetComponentInChildren<CarController>();
		audio = GetComponentInChildren<CarAudio>();
		audio.enabled = false;
		if (!PhotonNetwork.offlineMode)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player").gameObject.transform.root.gameObject;
		}
		if (!isDriving)
		{
			return;
		}
		if (CFInput.GetButton("Jump"))
		{
			isDriving = false;
			player.GetComponent<Rigidbody>().isKinematic = false;
			player.gameObject.GetComponentInChildren<CatMotor>().enabled = true;
			Quaternion rotation = player.transform.rotation;
			rotation.x = 0f;
			rotation.z = 0f;
			player.transform.rotation = rotation;
			audio.enabled = false;
		}
		else
		{
			player.transform.position = catTf.position;
			player.transform.rotation = catTf.rotation;
			float accelBrakeInput = 0f;
			float num = 0f;
			if (CFInput.GetAxis("Vertical") != 0f)
			{
				accelBrakeInput = ((!(CFInput.GetAxis("Vertical") > 0f)) ? (-1f) : 1f);
			}
			ctrl.Move(CFInput.GetAxis("Horizontal"), accelBrakeInput);
		}
	}

	private void OnCollisionEnter(Collision c)
	{
		if (c.collider.CompareTag("Player"))
		{
			isDriving = true;
			if (!player.GetComponentInParent<Rigidbody>().isKinematic)
			{
				Rigidbody component = player.GetComponent<Rigidbody>();
				Vector3 zero = Vector3.zero;
				player.GetComponent<Rigidbody>().angularVelocity = zero;
				component.velocity = zero;
				player.GetComponent<Rigidbody>().isKinematic = true;
			}
			player.gameObject.GetComponentInChildren<CatMotor>().DrivingMode();
			player.gameObject.GetComponentInChildren<CatMotor>().enabled = false;
			audio.enabled = true;
		}
	}
}
