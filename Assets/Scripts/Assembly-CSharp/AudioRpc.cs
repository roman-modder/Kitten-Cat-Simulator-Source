using Photon;
using UnityEngine;

public class AudioRpc : Photon.MonoBehaviour
{
	public AudioClip marco;

	public AudioClip polo;

	private AudioSource m_Source;

	private void Awake()
	{
		m_Source = GetComponent<AudioSource>();
	}

	[PunRPC]
	private void Marco()
	{
		if (base.enabled)
		{
			Debug.Log("Marco");
			m_Source.clip = marco;
			m_Source.Play();
		}
	}

	[PunRPC]
	private void Polo()
	{
		if (base.enabled)
		{
			Debug.Log("Polo");
			m_Source.clip = polo;
			m_Source.Play();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		base.enabled = focus;
	}
}
