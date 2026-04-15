using System;
using UnityEngine;

public class ModeTime : MonoBehaviour
{
	public float totalSecs;

	public UILabel lblTime;

	public Logic logic;

	private void Start()
	{
		lblTime.gameObject.SetActive(true);
	}

	private void FixedUpdate()
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.RoundToInt(totalSecs));
		string text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
		lblTime.text = text;
		if (totalSecs <= 0f)
		{
			logic.GameOver();
		}
		else
		{
			totalSecs -= Time.deltaTime;
		}
	}
}
