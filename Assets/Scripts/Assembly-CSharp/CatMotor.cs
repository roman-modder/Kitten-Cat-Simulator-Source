using Crosstales.BadWord;
using Photon;
using UnityEngine;

public class CatMotor : Photon.MonoBehaviour
{
	internal struct State
	{
		internal double timestamp;

		internal float inputV;

		internal float inputH;

		internal float inputJ;

		internal Vector3 pos;

		internal Quaternion rot;

		internal Vector3 vel;

		internal Vector3 anVel;
	}

	public GameObject target;

	public GameObject nameHold;

	public TextMesh catName;

	public AudioClip[] clipMeow;

	private Animator animator;

	private float jumpTime;

	private int hashWalk;

	private float vertForce;

	private float horForce;

	private float inputV;

	private float inputH;

	private float inputJump;

	private AudioSource audio;

	private double interpolationBackTime = 0.15;

	private State[] m_BufferedState = new State[10];

	private int m_TimestampCount;

	private void Awake()
	{
		audio = GetComponent<AudioSource>();
		audio.playOnAwake = false;
	}

	private void Start()
	{
		animator = target.GetComponent<Animator>();
		hashWalk = Animator.StringToHash("Walk");
		if (base.photonView.isMine)
		{
			Camera.main.transform.SetParent(target.transform, false);
			Camera.main.transform.localPosition = new Vector3(0f, 0.2f, -0.5f);
			catName.text = string.Empty;
		}
		else
		{
			catName.text = base.photonView.owner.name;
			catName.text = BadWordManager.ReplaceAll(catName.text);
		}
	}

	private void Update()
	{
		if (base.photonView.isMine)
		{
			inputV = CFInput.GetAxis("Vertical");
			inputH = CFInput.GetAxis("Horizontal");
			inputJump = (CFInput.GetButton("Jump") ? 1 : 0);
			if (CFInput.GetButtonDown("Meow") && !audio.isPlaying)
			{
				base.photonView.RPC("Meow", PhotonTargets.All, base.photonView.viewID, Random.Range(0, clipMeow.Length));
			}
			DoCalc();
			return;
		}
		nameHold.transform.LookAt(Camera.main.transform);
		double time = PhotonNetwork.time;
		double num = time - interpolationBackTime;
		if (m_BufferedState[0].timestamp > num)
		{
			for (int i = 0; i < m_TimestampCount; i++)
			{
				if (m_BufferedState[i].timestamp <= num || i == m_TimestampCount - 1)
				{
					State state = m_BufferedState[Mathf.Max(i - 1, 0)];
					State state2 = m_BufferedState[i];
					double num2 = state.timestamp - state2.timestamp;
					float t = 0f;
					if (num2 > 0.0001)
					{
						t = (float)((num - state2.timestamp) / num2);
					}
					base.transform.position = Vector3.Lerp(state2.pos, state.pos, t);
					base.transform.rotation = Quaternion.Slerp(state2.rot, state.rot, t);
					GetComponent<Rigidbody>().velocity = Vector3.Lerp(state2.vel, state.vel, t);
					GetComponent<Rigidbody>().angularVelocity = Vector3.Lerp(state2.anVel, state.anVel, t);
					inputV = state.inputV;
					inputH = state.inputH;
					inputJump = state.inputJ;
					DoCalc();
					break;
				}
			}
		}
		else
		{
			State state3 = m_BufferedState[0];
			GetComponent<Rigidbody>().velocity = state3.vel;
			GetComponent<Rigidbody>().angularVelocity = state3.anVel;
			inputV = state3.inputV;
			inputH = state3.inputH;
			inputJump = state3.inputJ;
		}
	}

	[PunRPC]
	public void Meow(int viewId, int clipId)
	{
		if (base.photonView.viewID == viewId && clipMeow != null && clipId <= clipMeow.Length - 1)
		{
			audio.clip = clipMeow[clipId];
			audio.Play();
		}
	}

	[PunRPC]
	public void SetSkin(int viewId, int skinId)
	{
		if (base.photonView.viewID == viewId)
		{
			Logic.FindInstance().ChangePlayerSkin(base.gameObject, skinId);
		}
	}

	private void DoCalc()
	{
		if (vertForce == 1f)
		{
			float num = 2.15f;
			if (target.GetComponent<Rigidbody>().velocity.magnitude < num && target.GetComponent<Rigidbody>().velocity.magnitude > 0f - num)
			{
				target.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 50f * target.GetComponent<Rigidbody>().mass * inputV);
			}
		}
		if (horForce == 1f)
		{
			target.GetComponent<Rigidbody>().AddForce(Vector3.up * 6f * target.GetComponent<Rigidbody>().mass, ForceMode.Impulse);
		}
		vertForce = 0f;
		horForce = 0f;
		animator.SetFloat("Speed", Mathf.Abs(inputV) + Mathf.Abs(inputH));
		if (inputV != 0f)
		{
			vertForce = 1f;
		}
		else
		{
			animator.speed = 1f;
		}
		if ((inputJump == 1f || Input.GetKey(KeyCode.Space)) && jumpTime <= 0f)
		{
			Vector3 position = target.transform.position;
			position.y += 0.1f;
			RaycastHit hitInfo;
			if (Physics.Raycast(position, Vector3.down, out hitInfo, 0.25f) && hitInfo.collider != null)
			{
				jumpTime = 0.25f;
				horForce = 1f;
			}
		}
		else
		{
			jumpTime -= Time.deltaTime;
		}
		if (inputH >= 0.15f || inputH <= -0.15f)
		{
			if (inputH < 0f)
			{
				inputH += 0.15f;
			}
			else
			{
				inputH -= 0.15f;
			}
			target.transform.Rotate(Vector3.up, inputH * 200f * Time.fixedDeltaTime);
		}
	}

	public void DrivingMode()
	{
		animator.SetFloat("Speed", 0f);
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting && base.photonView.isMine)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(inputV);
			stream.SendNext(inputH);
			stream.SendNext(inputJump);
			stream.SendNext(GetComponent<Rigidbody>().velocity);
			stream.SendNext(GetComponent<Rigidbody>().angularVelocity);
		}
		else if (!base.photonView.isMine)
		{
			State state = default(State);
			state.pos = (Vector3)stream.ReceiveNext();
			state.rot = (Quaternion)stream.ReceiveNext();
			state.inputV = (float)stream.ReceiveNext();
			state.inputH = (float)stream.ReceiveNext();
			state.inputJ = (float)stream.ReceiveNext();
			state.vel = (Vector3)stream.ReceiveNext();
			state.anVel = (Vector3)stream.ReceiveNext();
			for (int num = m_BufferedState.Length - 1; num >= 1; num--)
			{
				m_BufferedState[num] = m_BufferedState[num - 1];
			}
			state.timestamp = info.timestamp;
			m_BufferedState[0] = state;
			m_TimestampCount = Mathf.Min(m_TimestampCount + 1, m_BufferedState.Length);
		}
	}
}
