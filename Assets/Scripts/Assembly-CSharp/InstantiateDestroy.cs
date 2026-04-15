using UnityEngine;

public class InstantiateDestroy : MonoBehaviour
{
	public float xInstantiate = -5f;

	public float xDestroy = -100f;

	public float xOffset = 88f;

	public GameObject gameObjectToInstantiate;

	public float Speed = 3f;

	private Transform myTransform;

	private bool instanceCreated;

	private void Start()
	{
		myTransform = base.transform;
		instanceCreated = false;
	}

	private void Update()
	{
		if (!instanceCreated)
		{
			if (myTransform.position.x < xInstantiate)
			{
				Object.Instantiate(gameObjectToInstantiate, new Vector3(myTransform.position.x + xOffset, myTransform.position.y, myTransform.position.z), myTransform.rotation);
				instanceCreated = true;
			}
		}
		else if (myTransform.position.x < xDestroy)
		{
			Object.Destroy(base.gameObject);
		}
		myTransform.position += Vector3.left * Speed * Time.deltaTime;
	}
}
