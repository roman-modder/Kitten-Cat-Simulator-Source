using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("ControlFreak-Demos-JS/GunJS")]
public class GunJS : MonoBehaviour
{
	public ParticleSystem shotParticles;

	public AudioClip shotSound;

	public AudioClip emptySound;

	public AudioClip reloadSound;

	private bool isFiring;

	public float shotInterval;

	private float lastShotTime;

	public bool unlimitedAmmo;

	public int bulletCapacity;

	public int bulletCount;

	public Transform projectileOrigin;

	public BulletJS bulletPrefab;

	public GunJS()
	{
		shotInterval = 0.175f;
		bulletCapacity = 40;
		bulletCount = 40;
	}

	public virtual void Start()
	{
		isFiring = false;
	}

	public virtual void SetTriggerState(bool fire)
	{
		if (fire != isFiring)
		{
			isFiring = fire;
			if (fire)
			{
				FireBullet();
			}
		}
	}

	public virtual void FixedUpdate()
	{
		if (isFiring)
		{
			FireBullet();
		}
	}

	public virtual void Reload()
	{
		bulletCount = bulletCapacity;
		if (GetComponent<AudioSource>() != null && reloadSound != null)
		{
			GetComponent<AudioSource>().loop = false;
			GetComponent<AudioSource>().PlayOneShot(reloadSound);
		}
	}

	private void FireBullet()
	{
		if (Time.time - lastShotTime < shotInterval)
		{
			return;
		}
		lastShotTime = Time.time;
		if (unlimitedAmmo || bulletCount > 0)
		{
			if (!unlimitedAmmo)
			{
				bulletCount--;
			}
			if (shotParticles != null)
			{
				shotParticles.Play();
			}
			if (projectileOrigin != null && bulletPrefab != null)
			{
				BulletJS bulletJS = UnityEngine.Object.Instantiate(bulletPrefab, projectileOrigin.position, projectileOrigin.rotation) as BulletJS;
				if (bulletJS != null)
				{
					bulletJS.Init(this);
				}
			}
			if (GetComponent<AudioSource>() != null && shotSound != null)
			{
				GetComponent<AudioSource>().loop = false;
				GetComponent<AudioSource>().PlayOneShot(shotSound);
			}
		}
		else if (GetComponent<AudioSource>() != null && emptySound != null)
		{
			GetComponent<AudioSource>().loop = false;
			GetComponent<AudioSource>().PlayOneShot(emptySound);
		}
	}

	public virtual void Main()
	{
	}
}
