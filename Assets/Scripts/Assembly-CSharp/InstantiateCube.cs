using UnityEngine;

public class InstantiateCube : MonoBehaviour
{
	public GameObject Prefab;

	public int InstantiateType;

	public bool showGui;

	private void OnClick()
	{
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joined)
		{
			switch (InstantiateType)
			{
			case 0:
				PhotonNetwork.Instantiate(Prefab.name, base.transform.position + 3f * Vector3.up, Quaternion.identity, 0);
				break;
			case 1:
				PhotonNetwork.InstantiateSceneObject(Prefab.name, InputToEvent.inputHitPos + new Vector3(0f, 5f, 0f), Quaternion.identity, 0, null);
				break;
			}
		}
	}
}
