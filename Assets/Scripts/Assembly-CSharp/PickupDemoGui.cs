using System.Collections.Generic;
using UnityEngine;

public class PickupDemoGui : MonoBehaviour
{
	public bool ShowScores;

	public bool ShowDropButton;

	public bool ShowTeams;

	public float DropOffset = 0.5f;

	public void OnGUI()
	{
		if (!PhotonNetwork.inRoom)
		{
			return;
		}
		if (ShowScores)
		{
			GUILayout.Label("Your Score: " + PhotonNetwork.player.GetScore());
		}
		if (ShowDropButton)
		{
			foreach (PickupItem disabledPickupItem in PickupItem.DisabledPickupItems)
			{
				if (disabledPickupItem.PickupIsMine && disabledPickupItem.SecondsBeforeRespawn <= 0f)
				{
					if (GUILayout.Button("Drop " + disabledPickupItem.name))
					{
						disabledPickupItem.Drop();
					}
					GameObject gameObject = PhotonNetwork.player.TagObject as GameObject;
					if (gameObject != null && GUILayout.Button("Drop here " + disabledPickupItem.name))
					{
						Vector3 insideUnitSphere = Random.insideUnitSphere;
						insideUnitSphere.y = 0f;
						insideUnitSphere = insideUnitSphere.normalized;
						Vector3 newPosition = gameObject.transform.position + DropOffset * insideUnitSphere;
						disabledPickupItem.Drop(newPosition);
					}
				}
			}
		}
		if (!ShowTeams)
		{
			return;
		}
		foreach (PunTeams.Team key in PunTeams.PlayersPerTeam.Keys)
		{
			GUILayout.Label("Team: " + key);
			List<PhotonPlayer> list = PunTeams.PlayersPerTeam[key];
			foreach (PhotonPlayer item in list)
			{
				GUILayout.Label("  " + item.ToStringFull() + " Score: " + item.GetScore());
			}
		}
		if (GUILayout.Button("to red"))
		{
			PhotonNetwork.player.SetTeam(PunTeams.Team.red);
		}
		if (GUILayout.Button("to blue"))
		{
			PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
		}
	}
}
