using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
	public Transform target;

	public Transform eyes;

	public float rotationDamping;

	public AudioClip heartbeatClip;

	private AudioSource audioSource;

	public Animator animator;

	private float timeElapsed;

	private Vector3 targetPos;

	private Quaternion newRotation;

	public bool isOver;

	private void Start()
	{
	}

	private void Update()
	{
		CatMotor[] array = Object.FindObjectsOfType<CatMotor>();
		if (array.Length <= 0)
		{
			return;
		}
		target = array[Random.Range(0, array.Length)].transform;
		if (target == null)
		{
			return;
		}
		if (audioSource == null)
		{
			audioSource = target.gameObject.AddComponent<AudioSource>();
			audioSource.loop = true;
			audioSource.playOnAwake = false;
			audioSource.clip = heartbeatClip;
			audioSource.pitch = 1f;
			audioSource.volume = 0f;
			audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
			audioSource.Play();
		}
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, newRotation, Time.deltaTime * rotationDamping);
		audioSource.volume = 0f;
		animator.speed = GetComponent<Rigidbody>().velocity.magnitude * 0.8f;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		float num = Vector3.Distance(base.transform.position, target.position);
		PhotonView component = target.GetComponent<PhotonView>();
		if (num < 9f && component != null && component.isMine)
		{
			audioSource.volume = (9f - num) / 9f;
		}
		if (num < 1.2f)
		{
			if (Physics.Linecast(eyes.position, target.position) && (bool)component && component.isMine)
			{
				isOver = true;
				target.position = new Vector3(0f, 100f, 0f);
				GameObject.Find("_Logic").GetComponent<Logic>().GameOver();
			}
		}
		else
		{
			if (!PhotonNetwork.isMasterClient)
			{
				return;
			}
			RaycastHit hitInfo;
			if (num > 8f || !(Vector3.Angle(target.position - eyes.position, -base.transform.forward) < 80f) || !Physics.Linecast(eyes.position, target.position, out hitInfo) || !(hitInfo.transform == target.transform))
			{
				Vector3 position = base.transform.position;
				position.y = 0.05f;
				if (timeElapsed <= 0f)
				{
					targetPos.x = Random.Range(-15, 15);
					targetPos.z = Random.Range(-15, 15);
					newRotation = Quaternion.LookRotation(base.transform.position - targetPos);
					newRotation.x = 0f;
					newRotation.z = 0f;
					timeElapsed = Random.Range(2, 5);
				}
				else
				{
					timeElapsed -= Time.deltaTime;
				}
				if (!Physics.Raycast(position, -base.transform.forward, 0.5f))
				{
					GetComponent<Rigidbody>().AddRelativeForce(-Vector3.forward * 45.9f);
				}
			}
			else
			{
				audioSource.volume = 1f;
				newRotation = Quaternion.LookRotation(base.transform.position - target.position);
				newRotation.x = 0f;
				newRotation.z = 0f;
				base.transform.rotation = newRotation;
				GetComponent<Rigidbody>().AddRelativeForce(-Vector3.forward * 95.9f);
			}
		}
	}
}
