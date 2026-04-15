using UnityEngine;

[AddComponentMenu("ControlFreak-Demos-CS/GunCS")]
public class GunCS : MonoBehaviour
{
	public ParticleSystem shotParticles;

	public AudioClip shotSound;

	public AudioClip emptySound;

	public AudioClip reloadSound;

	private bool isFiring;

	public float shotInterval = 0.175f;

	private float lastShotTime;

	public bool unlimitedAmmo;

	public int bulletCapacity = 40;

	public int bulletCount = 40;

	public Transform projectileOrigin;

	public BulletCS bulletPrefab;

	private void Start()
	{
		isFiring = false;
	}

	public void SetTriggerState(bool fire)
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

	private void FixedUpdate()
	{
		if (isFiring)
		{
			FireBullet();
		}
	}

	public void Reload()
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
		if (!(Time.time - lastShotTime >= shotInterval))
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
				BulletCS bulletCS = Object.Instantiate(bulletPrefab, projectileOrigin.position, projectileOrigin.rotation);
				if (bulletCS != null)
				{
					bulletCS.Init(this);
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
}
