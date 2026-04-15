using UnityEngine;

public class JumpAndRunMovement : MonoBehaviour
{
	public float Speed;

	public float JumpForce;

	private Animator m_Animator;

	private Rigidbody2D m_Body;

	private PhotonView m_PhotonView;

	private bool m_IsGrounded;

	private void Awake()
	{
		m_Animator = GetComponent<Animator>();
		m_Body = GetComponent<Rigidbody2D>();
		m_PhotonView = GetComponent<PhotonView>();
	}

	private void Update()
	{
		UpdateIsGrounded();
		UpdateIsRunning();
		UpdateFacingDirection();
	}

	private void FixedUpdate()
	{
		if (m_PhotonView.isMine)
		{
			UpdateMovement();
			UpdateJumping();
		}
	}

	private void UpdateFacingDirection()
	{
		if (m_Body.velocity.x > 0.2f)
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else if (m_Body.velocity.x < -0.2f)
		{
			base.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
	}

	private void UpdateJumping()
	{
		if (Input.GetKey(KeyCode.Space) && m_IsGrounded)
		{
			m_Animator.SetTrigger("IsJumping");
			m_Body.AddForce(Vector2.up * JumpForce);
			m_PhotonView.RPC("DoJump", PhotonTargets.Others);
		}
	}

	[PunRPC]
	private void DoJump()
	{
		m_Animator.SetTrigger("IsJumping");
	}

	private void UpdateMovement()
	{
		Vector2 velocity = m_Body.velocity;
		if (Input.GetAxisRaw("Horizontal") > 0.5f)
		{
			velocity.x = Speed;
		}
		else if (Input.GetAxisRaw("Horizontal") < -0.5f)
		{
			velocity.x = 0f - Speed;
		}
		else
		{
			velocity.x = 0f;
		}
		m_Body.velocity = velocity;
	}

	private void UpdateIsRunning()
	{
		m_Animator.SetBool("IsRunning", Mathf.Abs(m_Body.velocity.x) > 0.1f);
	}

	private void UpdateIsGrounded()
	{
		Vector2 origin = new Vector2(base.transform.position.x, base.transform.position.y);
		m_IsGrounded = Physics2D.Raycast(origin, -Vector2.up, 0.1f, 1 << LayerMask.NameToLayer("Ground")).collider != null;
		m_Animator.SetBool("IsGrounded", m_IsGrounded);
	}
}
