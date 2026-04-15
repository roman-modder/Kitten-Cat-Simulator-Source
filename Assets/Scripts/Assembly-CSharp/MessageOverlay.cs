using UnityEngine;

public class MessageOverlay : MonoBehaviour
{
	public GameObject[] Objects;

	public void Start()
	{
		SetActive(true);
	}

	public void OnJoinedRoom()
	{
		SetActive(false);
	}

	public void OnLeftRoom()
	{
		SetActive(true);
	}

	private void SetActive(bool enable)
	{
		GameObject[] objects = Objects;
		foreach (GameObject gameObject in objects)
		{
			gameObject.SetActive(enable);
		}
	}
}
