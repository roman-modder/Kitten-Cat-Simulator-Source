using UnityEngine;

public class InteractiveItem : MonoBehaviour
{
	public Logic logic;

	private Vector3 startPos;

	private Quaternion startRot;

	public AudioClip crashClip;

	private AudioSource audioSource;

	public float aliveTimer;

	public bool alive = true;

	public bool shouldReset = true;

	private void Start()
	{
		audioSource = base.gameObject.AddComponent<AudioSource>();
		startPos = base.transform.position;
		startRot = base.transform.rotation;
		GetComponent<Rigidbody>().Sleep();
	}

	private void FixedUpdate()
	{
		if (!alive && shouldReset && aliveTimer <= 0f)
		{
			alive = true;
			base.transform.position = startPos;
			base.transform.rotation = startRot;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			GetComponent<Rigidbody>().Sleep();
		}
		else
		{
			aliveTimer -= Time.deltaTime;
		}
	}

	public void OnCollisionEnter(Collision c)
	{
		audioSource.clip = crashClip;
		audioSource.volume = 0.45f;
		audioSource.pitch = 1f;
		audioSource.Play();
		if (alive)
		{
			alive = false;
			logic.AddScore(25);
			aliveTimer = 15f;
		}
	}
}
