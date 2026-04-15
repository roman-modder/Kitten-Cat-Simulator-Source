using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarAudio : MonoBehaviour
{
	public enum EngineAudioOptions
	{
		Simple = 0,
		FourChannel = 1
	}

	public EngineAudioOptions engineSoundStyle = EngineAudioOptions.FourChannel;

	public AudioClip lowAccelClip;

	public AudioClip lowDecelClip;

	public AudioClip highAccelClip;

	public AudioClip highDecelClip;

	public AudioClip skidClip;

	public float pitchMultiplier = 1f;

	public float lowPitchMin = 1f;

	public float lowPitchMax = 6f;

	public float highPitchMultiplier = 0.25f;

	public float maxRolloffDistance = 500f;

	public float dopplerLevel = 1f;

	public bool useDoppler = true;

	private AudioSource lowAccel;

	private AudioSource lowDecel;

	private AudioSource highAccel;

	private AudioSource highDecel;

	private AudioSource skidSource;

	private bool startedSound;

	private CarController carController;

	private void StartSound()
	{
		carController = GetComponent<CarController>();
		highAccel = SetUpEngineAudioSource(highAccelClip);
		if (engineSoundStyle == EngineAudioOptions.FourChannel)
		{
			lowAccel = SetUpEngineAudioSource(lowAccelClip);
			lowDecel = SetUpEngineAudioSource(lowDecelClip);
			highDecel = SetUpEngineAudioSource(highDecelClip);
		}
		skidSource = SetUpEngineAudioSource(skidClip);
		startedSound = true;
	}

	private void StopSound()
	{
		AudioSource[] components = GetComponents<AudioSource>();
		foreach (AudioSource obj in components)
		{
			Object.Destroy(obj);
		}
		startedSound = false;
	}

	private void Update()
	{
		float sqrMagnitude = (Camera.main.transform.position - base.transform.position).sqrMagnitude;
		if (startedSound && sqrMagnitude > maxRolloffDistance * maxRolloffDistance)
		{
			StopSound();
		}
		if (!startedSound && sqrMagnitude < maxRolloffDistance * maxRolloffDistance)
		{
			StartSound();
		}
		if (!startedSound)
		{
			return;
		}
		float b = ULerp(lowPitchMin, lowPitchMax, carController.RevsFactor);
		b = Mathf.Min(lowPitchMax, b);
		if (engineSoundStyle == EngineAudioOptions.Simple)
		{
			if (highAccel != null)
			{
				highAccel.pitch = b * pitchMultiplier * highPitchMultiplier;
				highAccel.dopplerLevel = ((!useDoppler) ? 0f : dopplerLevel);
				highAccel.volume = 0.2f;
			}
		}
		else if (lowAccel != null && highAccel != null && lowDecel != null && highDecel != null)
		{
			lowAccel.pitch = b * pitchMultiplier;
			lowDecel.pitch = b * pitchMultiplier;
			highAccel.pitch = b * highPitchMultiplier * pitchMultiplier;
			highDecel.pitch = b * highPitchMultiplier * pitchMultiplier;
			float num = Mathf.Abs(carController.AccelInput);
			float num2 = 1f - num;
			float num3 = Mathf.InverseLerp(0.2f, 0.8f, carController.RevsFactor);
			float num4 = 1f - num3;
			num3 = 1f - (1f - num3) * (1f - num3);
			num4 = 1f - (1f - num4) * (1f - num4);
			num = 1f - (1f - num) * (1f - num);
			num2 = 1f - (1f - num2) * (1f - num2);
			lowAccel.volume = num4 * num;
			lowDecel.volume = num4 * num2;
			highAccel.volume = num3 * num;
			highDecel.volume = num3 * num2;
			highAccel.dopplerLevel = ((!useDoppler) ? 0f : dopplerLevel);
			lowAccel.dopplerLevel = ((!useDoppler) ? 0f : dopplerLevel);
			highDecel.dopplerLevel = ((!useDoppler) ? 0f : dopplerLevel);
			lowDecel.dopplerLevel = ((!useDoppler) ? 0f : dopplerLevel);
		}
		skidSource.volume = Mathf.Clamp01(carController.AvgSkid * 3f - 1f);
		skidSource.pitch = Mathf.Lerp(0.8f, 1.3f, carController.SpeedFactor);
		skidSource.dopplerLevel = ((!useDoppler) ? 0f : dopplerLevel);
	}

	private AudioSource SetUpEngineAudioSource(AudioClip clip)
	{
		if (clip == null)
		{
			return null;
		}
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = 0f;
		audioSource.loop = true;
		audioSource.time = Random.Range(0f, clip.length);
		audioSource.Play();
		audioSource.minDistance = 5f;
		audioSource.maxDistance = maxRolloffDistance;
		audioSource.dopplerLevel = 0f;
		return audioSource;
	}

	private float ULerp(float from, float to, float value)
	{
		return (1f - value) * from + value * to;
	}

	private float UInverseLerp(float from, float to, float value)
	{
		return (value - from) / (to - from);
	}
}
