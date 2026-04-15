using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/BulletCS")]
public class BulletCS : MonoBehaviour
{
	private GunCS gun;

	public float maxLifetime = 5f;

	private float lifetime;

	public void Init(GunCS gun)
	{
		this.gun = gun;
		lifetime = 0f;
	}

	private void FixedUpdate()
	{
		if ((lifetime += Time.deltaTime) > maxLifetime)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider objectHit)
	{
		if (gun != null)
		{
		}
		Object.Destroy(base.gameObject);
	}
}
