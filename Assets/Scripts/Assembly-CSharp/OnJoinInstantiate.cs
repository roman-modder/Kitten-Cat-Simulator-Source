using UnityEngine;

public class OnJoinInstantiate : MonoBehaviour
{
	public Transform ObjectToInstantiate;

	public bool InstantiateSceneObjects;

	public GameObject newObj;

	public void OnJoinedRoom()
	{
		Vector3 zero = Vector3.zero;
		zero.x += PhotonNetwork.player.ID;
		if (!InstantiateSceneObjects)
		{
			newObj = PhotonNetwork.Instantiate(ObjectToInstantiate.name, zero, Quaternion.identity, 0, null);
		}
		else
		{
			newObj = PhotonNetwork.InstantiateSceneObject(ObjectToInstantiate.name, zero, Quaternion.identity, 0, null);
		}
	}
}
