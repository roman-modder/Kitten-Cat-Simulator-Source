using UnityEngine;

public class AdLoading : MonoBehaviour
{
	private void Start()
	{
		Screen.sleepTimeout = -1;
		Application.targetFrameRate = 30;
		Application.LoadLevel("Splash");
	}
}
