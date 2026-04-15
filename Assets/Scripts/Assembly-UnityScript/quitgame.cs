using System;
using UnityEngine;

[Serializable]
public class quitgame : MonoBehaviour
{
	public virtual void Update()
	{
		if (Input.GetKey("escape"))
		{
			Application.Quit();
		}
	}

	public virtual void Main()
	{
	}
}
