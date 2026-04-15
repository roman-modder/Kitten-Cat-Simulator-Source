using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RPGMovement : MonoBehaviour
{
	public float ForwardSpeed;

	public float BackwardSpeed;

	public float StrafeSpeed;

	public float RotateSpeed;

	private CharacterController m_CharacterController;

	private Vector3 m_LastPosition;

	private Animator m_Animator;

	private PhotonView m_PhotonView;

	private PhotonTransformView m_TransformView;

	private float m_AnimatorSpeed;

	private Vector3 m_CurrentMovement;

	private float m_CurrentTurnSpeed;

	private void Start()
	{
		m_CharacterController = GetComponent<CharacterController>();
		m_Animator = GetComponent<Animator>();
		m_PhotonView = GetComponent<PhotonView>();
		m_TransformView = GetComponent<PhotonTransformView>();
	}

	private void Update()
	{
		if (m_PhotonView.isMine)
		{
			ResetSpeedValues();
			UpdateRotateMovement();
			UpdateForwardMovement();
			UpdateBackwardMovement();
			UpdateStrafeMovement();
			MoveCharacterController();
			ApplyGravityToCharacterController();
			ApplySynchronizedValues();
		}
		UpdateAnimation();
	}

	private void UpdateAnimation()
	{
		Vector3 vector = base.transform.position - m_LastPosition;
		float num = Vector3.Dot(vector.normalized, base.transform.forward);
		float num2 = Vector3.Dot(vector.normalized, base.transform.right);
		if (Mathf.Abs(num) < 0.2f)
		{
			num = 0f;
		}
		if (num > 0.6f)
		{
			num = 1f;
			num2 = 0f;
		}
		if (num >= 0f && Mathf.Abs(num2) > 0.7f)
		{
			num = 1f;
		}
		m_AnimatorSpeed = Mathf.MoveTowards(m_AnimatorSpeed, num, Time.deltaTime * 5f);
		m_Animator.SetFloat("Speed", m_AnimatorSpeed);
		m_Animator.SetFloat("Direction", num2);
		m_LastPosition = base.transform.position;
	}

	private void ResetSpeedValues()
	{
		m_CurrentMovement = Vector3.zero;
		m_CurrentTurnSpeed = 0f;
	}

	private void ApplySynchronizedValues()
	{
		m_TransformView.SetSynchronizedValues(m_CurrentMovement, m_CurrentTurnSpeed);
	}

	private void ApplyGravityToCharacterController()
	{
		m_CharacterController.Move(base.transform.up * Time.deltaTime * -9.81f);
	}

	private void MoveCharacterController()
	{
		m_CharacterController.Move(m_CurrentMovement * Time.deltaTime);
	}

	private void UpdateForwardMovement()
	{
		if (Input.GetKey(KeyCode.W))
		{
			m_CurrentMovement = base.transform.forward * ForwardSpeed;
		}
	}

	private void UpdateBackwardMovement()
	{
		if (Input.GetKey(KeyCode.S))
		{
			m_CurrentMovement = -base.transform.forward * BackwardSpeed;
		}
	}

	private void UpdateStrafeMovement()
	{
		if (Input.GetKey(KeyCode.Q))
		{
			m_CurrentMovement = -base.transform.right * StrafeSpeed;
		}
		if (Input.GetKey(KeyCode.E))
		{
			m_CurrentMovement = base.transform.right * StrafeSpeed;
		}
	}

	private void UpdateRotateMovement()
	{
		if (Input.GetKey(KeyCode.A))
		{
			m_CurrentTurnSpeed = 0f - RotateSpeed;
			base.transform.Rotate(0f, (0f - RotateSpeed) * Time.deltaTime, 0f);
		}
		if (Input.GetKey(KeyCode.D))
		{
			m_CurrentTurnSpeed = RotateSpeed;
			base.transform.Rotate(0f, RotateSpeed * Time.deltaTime, 0f);
		}
	}
}
