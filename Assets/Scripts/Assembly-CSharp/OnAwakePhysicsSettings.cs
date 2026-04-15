using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class OnAwakePhysicsSettings : Photon.MonoBehaviour
{
	public void Awake()
	{
		if (base.photonView.isMine)
		{
			return;
		}
		Rigidbody component = GetComponent<Rigidbody>();
		if (component != null)
		{
			component.isKinematic = true;
			return;
		}
		Rigidbody2D component2 = GetComponent<Rigidbody2D>();
		if (component2 != null)
		{
			component2.isKinematic = true;
		}
	}
}
