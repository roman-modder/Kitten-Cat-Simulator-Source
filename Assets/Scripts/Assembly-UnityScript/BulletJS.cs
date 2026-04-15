using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/BulletJS")]
public class BulletJS : MonoBehaviour
{
	private GunJS gun;

	public float maxLifetime;

	private float lifetime;

	public BulletJS()
	{
		maxLifetime = 5f;
	}

	public virtual void Init(GunJS gun)
	{
		this.gun = gun;
		lifetime = 0f;
	}

	public virtual void FixedUpdate()
	{
		lifetime += Time.deltaTime;
		if (!(lifetime <= maxLifetime))
		{
			UnityEngine.Object.Destroy(gameObject);
		}
	}

	public virtual void OnTriggerEnter(Collider objectHit)
	{
		if (gun != null)
		{
		}
		UnityEngine.Object.Destroy(gameObject);
	}

	public virtual void Main()
	{
	}
}
