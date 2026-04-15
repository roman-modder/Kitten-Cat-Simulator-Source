using UnityEngine;

public class PlayerDiamond : MonoBehaviour
{
	public Transform HeadTransform;

	public float HeightOffset = 0.5f;

	private PhotonView m_PhotonView;

	private Renderer m_DiamondRenderer;

	private float m_Rotation;

	private float m_Height;

	private PhotonView PhotonView
	{
		get
		{
			if (m_PhotonView == null)
			{
				m_PhotonView = base.transform.parent.GetComponent<PhotonView>();
			}
			return m_PhotonView;
		}
	}

	private Renderer DiamondRenderer
	{
		get
		{
			if (m_DiamondRenderer == null)
			{
				m_DiamondRenderer = GetComponentInChildren<Renderer>();
			}
			return m_DiamondRenderer;
		}
	}

	private void Start()
	{
		m_Height = HeightOffset;
		if (HeadTransform != null)
		{
			m_Height += HeadTransform.position.y;
		}
	}

	private void Update()
	{
		UpdateDiamondPosition();
		UpdateDiamondRotation();
		UpdateDiamondVisibility();
	}

	private void UpdateDiamondPosition()
	{
		Vector3 b = Vector3.zero;
		if (HeadTransform != null)
		{
			b = HeadTransform.position;
		}
		b.y = m_Height;
		if (!float.IsNaN(b.x) && !float.IsNaN(b.z))
		{
			base.transform.position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * 10f);
		}
	}

	private void UpdateDiamondRotation()
	{
		m_Rotation += Time.deltaTime * 180f;
		m_Rotation %= 360f;
		base.transform.rotation = Quaternion.Euler(0f, m_Rotation, 0f);
	}

	private void UpdateDiamondVisibility()
	{
		DiamondRenderer.enabled = true;
		if (PhotonView == null || !PhotonView.isMine)
		{
			DiamondRenderer.enabled = false;
		}
	}
}
