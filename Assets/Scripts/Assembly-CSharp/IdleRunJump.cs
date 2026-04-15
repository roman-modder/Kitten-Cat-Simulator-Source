using UnityEngine;

public class IdleRunJump : MonoBehaviour
{
	protected Animator animator;

	public float DirectionDampTime = 0.25f;

	public bool ApplyGravity = true;

	public float SynchronizedMaxSpeed;

	public float TurnSpeedModifier;

	public float SynchronizedTurnSpeed;

	public float SynchronizedSpeedAcceleration;

	protected PhotonView m_PhotonView;

	private PhotonTransformView m_TransformView;

	private float m_SpeedModifier;

	private void Start()
	{
		animator = GetComponent<Animator>();
		m_PhotonView = GetComponent<PhotonView>();
		m_TransformView = GetComponent<PhotonTransformView>();
		if (animator.layerCount >= 2)
		{
			animator.SetLayerWeight(1, 1f);
		}
	}

	private void Update()
	{
		if ((!m_PhotonView.isMine && PhotonNetwork.connected) || !animator)
		{
			return;
		}
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Run"))
		{
			if (Input.GetButton("Fire1"))
			{
				animator.SetBool("Jump", true);
			}
		}
		else
		{
			animator.SetBool("Jump", false);
		}
		if (Input.GetButtonDown("Fire2") && animator.layerCount >= 2)
		{
			animator.SetBool("Hi", !animator.GetBool("Hi"));
		}
		float axis = Input.GetAxis("Horizontal");
		float num = Input.GetAxis("Vertical");
		if (num < 0f)
		{
			num = 0f;
		}
		animator.SetFloat("Speed", axis * axis + num * num);
		animator.SetFloat("Direction", axis, DirectionDampTime, Time.deltaTime);
		float num2 = animator.GetFloat("Direction");
		float target = Mathf.Abs(num);
		if (Mathf.Abs(num2) > 0.2f)
		{
			target = TurnSpeedModifier;
		}
		m_SpeedModifier = Mathf.MoveTowards(m_SpeedModifier, target, Time.deltaTime * 25f);
		Vector3 speed = base.transform.forward * SynchronizedMaxSpeed * m_SpeedModifier;
		float turnSpeed = num2 * SynchronizedTurnSpeed;
		m_TransformView.SetSynchronizedValues(speed, turnSpeed);
	}
}
