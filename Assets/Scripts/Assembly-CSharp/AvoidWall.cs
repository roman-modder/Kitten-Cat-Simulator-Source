using UnityEngine;

public class AvoidWall : MonoBehaviour
{
	public Transform tfTarget;

	public Vector3 startPos;

	private void Update()
	{
		if (tfTarget == null)
		{
			if (base.transform.parent == null)
			{
				return;
			}
			tfTarget = base.transform.parent;
			startPos = base.transform.localPosition;
		}
		Vector3 b = startPos;
		Vector3 position = tfTarget.position;
		position.y = 0.05f;
		RaycastHit hitInfo;
		if (Physics.Raycast(position, -base.transform.forward, out hitInfo, 0.5f))
		{
			b.z = -0.15f;
		}
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, b, Time.deltaTime * 2.5f);
	}
}
