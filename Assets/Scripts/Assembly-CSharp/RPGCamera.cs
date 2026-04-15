using UnityEngine;

public class RPGCamera : MonoBehaviour
{
	public Transform Target;

	public float MaximumDistance;

	public float MinimumDistance;

	public float ScrollModifier;

	public float TurnModifier;

	private Transform m_CameraTransform;

	private Vector3 m_LookAtPoint;

	private Vector3 m_LocalForwardVector;

	private float m_Distance;

	private void Start()
	{
		m_CameraTransform = base.transform.GetChild(0);
		m_LocalForwardVector = m_CameraTransform.forward;
		m_Distance = (0f - m_CameraTransform.localPosition.z) / m_CameraTransform.forward.z;
		m_Distance = Mathf.Clamp(m_Distance, MinimumDistance, MaximumDistance);
		m_LookAtPoint = m_CameraTransform.localPosition + m_LocalForwardVector * m_Distance;
	}

	private void LateUpdate()
	{
		UpdateDistance();
		UpdateZoom();
		UpdatePosition();
		UpdateRotation();
	}

	private void UpdateDistance()
	{
		m_Distance = Mathf.Clamp(m_Distance - Input.GetAxis("Mouse ScrollWheel") * ScrollModifier, MinimumDistance, MaximumDistance);
	}

	private void UpdateZoom()
	{
		m_CameraTransform.localPosition = m_LookAtPoint - m_LocalForwardVector * m_Distance;
	}

	private void UpdatePosition()
	{
		if (!(Target == null))
		{
			base.transform.position = Target.transform.position;
		}
	}

	private void UpdateRotation()
	{
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
		{
			base.transform.Rotate(0f, Input.GetAxis("Mouse X") * TurnModifier, 0f);
		}
		if (Input.GetMouseButton(1) && Target != null)
		{
			Target.rotation = Quaternion.Euler(0f, base.transform.rotation.eulerAngles.y, 0f);
		}
	}
}
