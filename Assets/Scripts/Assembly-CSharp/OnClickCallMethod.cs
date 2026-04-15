using Photon;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OnClickCallMethod : Photon.MonoBehaviour
{
	public GameObject TargetGameObject;

	public string TargetMethod;

	public void OnClick()
	{
		if (TargetGameObject == null || string.IsNullOrEmpty(TargetMethod))
		{
			Debug.LogWarning(string.Concat(this, " can't call, cause GO or Method are empty."));
		}
		else
		{
			TargetGameObject.SendMessage(TargetMethod);
		}
	}
}
