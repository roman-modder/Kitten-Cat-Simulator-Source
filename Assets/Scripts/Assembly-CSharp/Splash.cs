using System.Collections;
using UnityEngine;

public class Splash : MonoBehaviour
{
	private void Start()
	{
		Screen.sleepTimeout = -1;
		PlayerPrefs.SetInt("startAd", 0);
		StartCoroutine(DelaySplash());
	}

	private IEnumerator DelaySplash()
	{
		yield return new WaitForSeconds(3.5f);
		Application.LoadLevel("Menu");
	}
}
