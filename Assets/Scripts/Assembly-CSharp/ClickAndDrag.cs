using Photon;
using UnityEngine;

public class ClickAndDrag : Photon.MonoBehaviour
{
	private Vector3 camOnPress;

	private bool following;

	private float factor = -0.1f;

	private void Update()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		InputToEvent component = Camera.main.GetComponent<InputToEvent>();
		if (component == null)
		{
			return;
		}
		if (!following)
		{
			if (component.Dragging)
			{
				camOnPress = base.transform.position;
				following = true;
			}
		}
		else if (component.Dragging)
		{
			Vector3 b = camOnPress - new Vector3(component.DragVector.x, 0f, component.DragVector.y) * factor;
			base.transform.position = Vector3.Lerp(base.transform.position, b, Time.deltaTime * 0.5f);
		}
		else
		{
			camOnPress = Vector3.zero;
			following = false;
		}
	}
}
