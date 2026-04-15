using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/SimpleRotatorCS")]
public class SimpleRotatorCS : MonoBehaviour
{
	public Vector3 rotSpeed = new Vector3(0f, 90f, 0f);

	private Quaternion initialRot;

	private Vector3 curRot;

	private void Start()
	{
		initialRot = base.transform.localRotation;
	}

	private void Update()
	{
		curRot += rotSpeed * Time.deltaTime;
		base.transform.localRotation = Quaternion.Euler(curRot) * initialRot;
	}
}
